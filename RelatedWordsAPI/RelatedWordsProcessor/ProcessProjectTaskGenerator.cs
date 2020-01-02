using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RelatedWordsAPI.Models;
using RelatedWordsAPI.App;
using System.Net.Http;
using System.Text.RegularExpressions;
using RelatedWordsAPI.RelatedWordsProcessor.Helper;
using RelatedWordsAPI.Services;

namespace RelatedWordsAPI.RelatedWordsProcessor
/// <summary>
/// Defines the complete processing flow for a project.
/// Returns a Task that represents the complete processing job for a given project.
/// </summary>
{
    public class ProcessProjectTaskGenerator : IProcessProjectTaskGenerator
    {
        private Project _project;
        private Project _projectWithoutPages;
        private RelatedWordsContext _context;
        private IHttpEngine _httpEngine;
        private CancellationToken _cancellationToken;
        private static readonly List<Func<string, string>> _filters = new List<Func<string, string>>
                    {
                        TextFilters.Instance.HtmlFilter,
                        TextFilters.Instance.SegmentAndLammatize,
                        TextFilters.Instance.RemoveLooseInterpuction,
                        TextFilters.Instance.Myfilter,
                    };

        public ProcessProjectTaskGenerator(Project p, RelatedWordsContext context, IHttpEngine httpEngine)
        {
            _context = context;
            _httpEngine = httpEngine;
            _projectWithoutPages = p;
        }

        /// <summary>
        /// Returns a Task that represents the complete processing job for a given project.
        /// </summary>
        /// <exception cref="RelatedWordsAPI.App.CouldNotGetFromDatabase">
        /// <paramref name="project"/> unable to retrive from database.
        /// </exception>
        /// <exception cref="RelatedWordsAPI.App.InputProjectNotValid">
        /// <paramref name="project"/> not valid.
        /// </exception>
        /// 
        public async Task ProcessProjectTaskRun(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _project = await GetProjectFromDB(_projectWithoutPages).ConfigureAwait(false);
            Validate(_project);

            await ProcessProject(_project, _cancellationToken).ConfigureAwait(false);
        }

        private Task<Project> GetProjectFromDB(Project p)
        {
            return _context.Projects
                .Where(savedp => savedp.ProjectId == p.ProjectId)
                .Include(savedp => savedp.Pages)
                .SingleOrDefaultAsync();
        }

        private void Validate(Project p)
        {
            var messages = new List<string>();
            
            if (p.Pages.Count == 0) 
                messages.Add("No pages defined.");

            if (p.Pages.Any(page => Uri.IsWellFormedUriString(page.Url, UriKind.RelativeOrAbsolute)))
                messages.Add("Nonvalid page URL.");

            if (messages.Count > 0)
                throw new InputProjectNotValid(String.Join("\n", messages));
        }

        /// <summary>
        /// Returns a Task that represents the complete processing job for a given project.
        /// </summary>
        /// <exception cref="HttpRequestException">
        /// Http error.
        /// </exception>
        private async Task ProcessProject(Project project, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            List<Task> tasks = new List<Task>();
            var pagesToTasks = new Dictionary<Page, Task>();

            project.Pages.ToList().ForEach(page => 
                page.ProcessingStatus = 
                    page.ProcessingStatus != PageProcessingStatus.Finished ? 
                    PageProcessingStatus.Processing : PageProcessingStatus.Finished
                );
            await _context.SaveChangesAsync().ConfigureAwait(false);

            foreach (Page page in project.Pages)
            {
                if (page.ProcessingStatus != PageProcessingStatus.Processing)
                    continue;
                Task t = PrepareContent(page, cancellationToken);
                pagesToTasks[page] = t;
                tasks.Add(t);
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);

            await Task.WhenAll(tasks).ConfigureAwait(false);

            await SavePageFilteringResult(pagesToTasks).ConfigureAwait(false);

            await SaveProjectWords(project).ConfigureAwait(false);

            await SavePageSentenceWords(project, cancellationToken).ConfigureAwait(false);
        }

        private async Task SaveProjectWords(Project project)
        {
            var pages = project.Pages;
            foreach (var page in pages)
            {
                if (page.ProcessingStatus != PageProcessingStatus.Filtered)
                    continue;

                string content = page.FilteredContent;

                foreach (string word in content.Split())
                    project.Words.Add(new Word(project, word));
            }
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }



        private async Task SavePageFilteringResult(Dictionary<Page, Task> pagesToTasks)
        {
            foreach (Page page in pagesToTasks.Keys)
            {
                Task t = pagesToTasks[page];
                switch (t.Status)
                {
                    case TaskStatus.RanToCompletion:
                        page.ProcessingStatus = PageProcessingStatus.Filtered;
                        break;
                    case TaskStatus.Faulted:
                        page.ProcessingStatus = PageProcessingStatus.Failed;
                        break;
                    case TaskStatus.Canceled:
                        page.ProcessingStatus = PageProcessingStatus.Canceled;
                        break;
                    default:
                        page.ProcessingStatus = PageProcessingStatus.Unknown;
                        break;
                }
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task SavePageProcessingResult(Dictionary<Page, Task> pagesToTasks)
        {
            foreach (Page page in pagesToTasks.Keys)
            {
                Task t = pagesToTasks[page];
                switch (t.Status)
                {
                    case TaskStatus.RanToCompletion:
                        page.ProcessingStatus = PageProcessingStatus.Finished;
                        break;
                    case TaskStatus.Faulted:
                        page.ProcessingStatus = PageProcessingStatus.Failed;
                        break;
                    case TaskStatus.Canceled:
                        page.ProcessingStatus = PageProcessingStatus.Canceled;
                        break;
                    default:
                        page.ProcessingStatus = PageProcessingStatus.Unknown;
                        break;
                }
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        private Task PrepareContent(Page page, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                string innerHtml = await _httpEngine.GetAsync(page.Url, cancellationToken).ConfigureAwait(false);
                page.OriginalContent = innerHtml;
                // Above three lines can be replaced with new helper method below
                // string responseBody = await _httpEngine.GetStringAsync(page.Url);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            }, cancellationToken)
                .ContinueWith(async (previous) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string filteredContent = _filters.Aggregate(page.OriginalContent,
                        (content, filter) => filter(content));

                    page.FilteredContent = filteredContent;

                    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                },
            cancellationToken,
            TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.Default);
        }

        private async Task SavePageSentenceWords(Project project, CancellationToken cancellationToken)
        {
            ISet<Word> projectWords = _context.Words.Where(word => word.ProjectId == project.ProjectId).ToHashSet();
            var pageTasks = new Dictionary<Page, Task>();

            foreach (Page page in project.Pages)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (page.ProcessingStatus != PageProcessingStatus.Filtered)
                    continue;

                Task t = Task.Run(async () =>
                {
                    var sentences = page.FilteredContent.Split("\n").ToList();

                    var sentnecesWithWords = sentences.Select(s => s.Split().ToList()).ToList();

                    var pageWordCounter = new Dictionary<Word, int>();

                    for (int i = 0; i < sentnecesWithWords.Count; i++)
                    {
                        var words = sentnecesWithWords[i];
                        var sentneceWordCounter = new Dictionary<Word, int>();
                        Sentence sentence = new Sentence(page, i);
                        _context.Add(sentence);
                        page.Sentences.Add(sentence);
                        foreach (string wordString in words)
                        {
                            Word word = projectWords.Where(word => word.WordContent == wordString).Single();
                            pageWordCounter[word] = pageWordCounter.ContainsKey(word) ? pageWordCounter[word] + 1 : 1;
                            sentneceWordCounter[word] = sentneceWordCounter.ContainsKey(word) ? sentneceWordCounter[word] + 1 : 1;
                        }

                        foreach (Word word in sentneceWordCounter.Keys)
                        {
                            int count = sentneceWordCounter[word];
                            var wordSentence = new WordSentence(word, sentence, count);
                            _context.Add(wordSentence);
                        }
                    }

                    foreach (Word word in pageWordCounter.Keys)
                    {
                        int count = pageWordCounter[word];
                        var wordPage = new WordPage(word, page, count);
                        _context.Add(wordPage);
                    }

                    page.ProcessingStatus = PageProcessingStatus.Finished;
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                });

                pageTasks[page] = t;
            }

            await Task.WhenAll(pageTasks.Values.ToList()).ConfigureAwait(false);
            await SavePageProcessingResult(pageTasks).ConfigureAwait(false);
        }
    }
}
