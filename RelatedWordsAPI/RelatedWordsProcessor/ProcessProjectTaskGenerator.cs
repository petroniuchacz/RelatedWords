using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        private static readonly List<Func<string, string>> _filters = new List<Func<string, string>>
                    {
                        TextFilters.Instance.HtmlFilter,
                        TextFilters.Instance.SegmentAndLammatize,
                        TextFilters.Instance.RemoveLooseInterpuction,
                        TextFilters.Instance.Myfilter,
                    };
        private readonly IServiceProvider _serviceProvider;

        public ProcessProjectTaskGenerator(Project p, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
        public async Task ProcessProjectTaskRunAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RelatedWordsContext>();
                _project = await GetProjectWithPagesAsync(_projectWithoutPages, context).ConfigureAwait(false);
                Validate(_project);

                await ProcessProjectAsync(_project, cancellationToken).ConfigureAwait(false);
            }
        }

        private void Validate(Project p)
        {
            var messages = new List<string>();
            
            if (p.Pages.Count == 0) 
                messages.Add("No pages defined.");

            if (p.Pages.Any(page => !Uri.IsWellFormedUriString(page.Url, UriKind.RelativeOrAbsolute)))
                messages.Add("Nonvalid page URL.");

            if (messages.Count > 0)
                throw new InputProjectNotValid(String.Join("\n", messages));
        }

        /// <summary>
        /// Returns a Task that represents the complete processing job for a given project.
        /// </summary>
        private async Task ProcessProjectAsync(Project passedProject, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var scope = _serviceProvider.CreateScope()) 
            {
                var context = scope.ServiceProvider.GetRequiredService<RelatedWordsContext>();
                var project = await GetProjectWithPagesAsync(passedProject, context);

                try
                {
                    await ToggleProjectStatusAsync(project, cancellationToken, ProjectProcessingStatus.Processing);

                    await TogglePagesStatusAsync(project, cancellationToken, PageProcessingStatus.Processing);

                    await PreparePageContentAsync(project, cancellationToken).ConfigureAwait(false);

                    await SaveProjectWordsAsync(project, cancellationToken).ConfigureAwait(false);

                    await SavePageSentenceWordsAsync(project, cancellationToken).ConfigureAwait(false);

                    await ToggleProjectStatusAsync(project, cancellationToken, ProjectProcessingStatus.Finished);
                }
                catch (TaskCanceledException)
                {
                    project.ProcessingStatus = ProjectProcessingStatus.Canceled;
                    await context.SaveChangesAsync();
                }

                catch (Exception)
                {
                    project.ProcessingStatus = ProjectProcessingStatus.Failed;
                    await context.SaveChangesAsync();
                }
            }
        }

        private async Task ToggleProjectStatusAsync(Project passedProject, CancellationToken cancellationToken, ProjectProcessingStatus status)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RelatedWordsContext>();
                var project = await GetProjectAsync(passedProject, context);
                project.ProcessingStatus = status;
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task TogglePagesStatusAsync(Project passedProject, CancellationToken cancellationToken, PageProcessingStatus status)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RelatedWordsContext>();
                var project = await GetProjectWithPagesAsync(passedProject, context);
                //project.Pages.ToList().ForEach(page =>
                //        page.ProcessingStatus =
                //            page.ProcessingStatus != PageProcessingStatus.Finished ?
                //            status : PageProcessingStatus.Finished
                //        );
                project.Pages.ToList().ForEach(page => page.ProcessingStatus = status);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task PreparePageContentAsync(Project passedProject, CancellationToken cancellationToken)
        {
            List<Task> tasks = new List<Task>();
            var pagesToTasks = new Dictionary<Page, Task>();
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RelatedWordsContext>();
                var project = await GetProjectWithPagesAsync(passedProject, context);
                foreach (Page page in project.Pages)
                {
                    if (page.ProcessingStatus != PageProcessingStatus.Processing)
                        continue;
                    Task t = PrepareContent(page, cancellationToken);
                    pagesToTasks[page] = t;
                    tasks.Add(t);
                }

                try
                {
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
                catch (TaskCanceledException e)
                {
                    throw e;
                }
                catch (Exception) { }

                SetPageFilteringResult(pagesToTasks);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private async Task SaveProjectWordsAsync(Project passedProject, CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RelatedWordsContext>();
                var project = await GetProjectWithPagesAsync(passedProject, context);
                await context.Entry(project).Collection(project => project.Words).LoadAsync(cancellationToken);
                context.RemoveRange(project.Words);
                var pages = project.Pages;
                var words = new HashSet<Word>();
                foreach (var page in pages)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (page.ProcessingStatus != PageProcessingStatus.Filtered)
                        continue;

                    string content = page.FilteredContent;

                    foreach (string word in content.Split())
                    {
                        if (word != "")
                        {
                            words.Add(new Word(word, project));
                        }
                        
                    }
                }
                context.Words.AddRange(words);
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private void SetPageFilteringResult(Dictionary<Page, Task> pagesToTasks)
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
        }

        private Task PrepareContent(Page page, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var httpEngine = scope.ServiceProvider.GetRequiredService<IHttpEngine>();
                    cancellationToken.ThrowIfCancellationRequested();
                    string innerHtml = await httpEngine.GetAsync(page.Url, cancellationToken).ConfigureAwait(false);
                    page.OriginalContent = innerHtml;
                    // Above three lines can be replaced with new helper method below
                    // string responseBody = await _httpEngine.GetStringAsync(page.Url);
                }
            }, cancellationToken)
                .ContinueWith(async (previous) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string filteredContent = _filters.Aggregate(page.OriginalContent,
                        (content, filter) => filter(content));

                    page.FilteredContent = filteredContent;

                },
            cancellationToken,
            TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.Default);
        }

        private async Task SavePageSentenceWordsAsync(Project passedProject, CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RelatedWordsContext>();
                var project = await GetProjectWithPagesAsync(passedProject, context);
                await context.Entry(project).Collection(project => project.Words).LoadAsync();
                ISet<Word> projectWords = project.Words;
                var pageTasks = new Dictionary<Page, Task>();

                foreach (Page page in project.Pages)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (page.ProcessingStatus != PageProcessingStatus.Filtered)
                        continue;

                    var sentences = page.FilteredContent.Split("\n").ToList();

                    var sentnecesWithWords = sentences.Select(s => s.Split().ToList()).ToList();

                    var pageWordCounter = new Dictionary<Word, int>();

                    for (int i = 0; i < sentnecesWithWords.Count; i++)
                    {
                        var words = sentnecesWithWords[i];
                        var sentneceWordCounter = new Dictionary<Word, int>();
                        Sentence sentence = new Sentence(page, i);
                        context.Add(sentence);
                        page.Sentences.Add(sentence);
                        foreach (string wordString in words)
                        {
                            if (wordString == "")
                                continue;

                            Word word = projectWords.Where(word => word.WordContent == wordString).Single();
                            pageWordCounter[word] = pageWordCounter.ContainsKey(word) ? pageWordCounter[word] + 1 : 1;
                            sentneceWordCounter[word] = sentneceWordCounter.ContainsKey(word) ? sentneceWordCounter[word] + 1 : 1;
                        }

                        foreach (Word word in sentneceWordCounter.Keys)
                        {
                            int count = sentneceWordCounter[word];
                            var wordSentence = new WordSentence(word, sentence, count);
                            context.Add(wordSentence);
                        }
                    }

                    foreach (Word word in pageWordCounter.Keys)
                    {
                        int count = pageWordCounter[word];
                        var wordPage = new WordPage(word, page, count);
                        context.Add(wordPage);
                    }

                    page.ProcessingStatus = PageProcessingStatus.Finished;

                }

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private static async Task<Project> GetProjectWithPagesAsync(Project passedProject, RelatedWordsContext context)
        {
            return await context.Projects
                .Include(project => project.Pages)
                .SingleAsync(project => project.ProjectId == passedProject.ProjectId)
                .ConfigureAwait(false);
        }

        private static async Task<Project> GetProjectAsync(Project passedProject, RelatedWordsContext context)
        {
            return await context.Projects
                .SingleAsync(project => project.ProjectId == passedProject.ProjectId)
                .ConfigureAwait(false);
        }
    }
}
