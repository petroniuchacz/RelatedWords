using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RelatedWordsAPI.Models;
using Microsoft.AspNetCore.Authorization;
using RelatedWordsAPI.App.Helpers;
using RelatedWordsAPI.Models;
using RelatedWordsAPI.Services;
using System.Security.Claims;

namespace RelatedWordsAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly RelatedWordsContext _context;
        private readonly IUserService _userService;

        public ProjectsController(RelatedWordsContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/Projects
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }

        [HttpGet("getuserprojects")]
        // GET: api/UserProjects
        public async Task<ActionResult<IEnumerable<Project>>> GetUserProjects()
        {
            int userId = int.Parse(User.Identity.Name);
            return await _context.Projects
                .Where(p => p.UserId == userId)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            if (await DoesntBelongToUser(id, User, _context))
                return Unauthorized();

            return Ok(project);
        }

        // PUT: api/Projects/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, [Bind("ProjectId,Name")]Project project)
        {
            if (id != project.ProjectId)
            {
                return BadRequest();
            }

            int userId = int.Parse(User.Identity.Name);

            if (await DoesntBelongToUser(userId, id, _context))
                return Unauthorized();

            // we are updating "Bind" props, but making a copy of other props
            Project previous = await _context.Projects.FindAsync(id);
            previous.Name = project.Name;
            previous.EditRevisionNumber += 1;

            _context.Entry(previous).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Projects
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            project.UserId = int.Parse(User.Identity.Name);

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.ProjectId }, project);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Project>> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            if (await DoesntBelongToUser(id, User, _context))
                return Unauthorized();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return project;
        }

        // GET: api/ProjectsWords/5
        [HttpGet("projectwords/{projectId}")]
        public async Task<IActionResult> GetProjectWords(int projectId)
        {
            Func<Project, List<Word>> extractWords = (proj) => proj.Words.Select(
                w => new Word() { WordContent = w.WordContent, ProjectId = w.ProjectId, WordId = w.WordId }
                ).ToList();
            return await GetProjectRelatedCollections(projectId, User, "Words", extractWords).ConfigureAwait(false);
        }

        // GET: api/ProjectPages/5
        [HttpGet("projectpages/{projectId}")]
        public async Task<IActionResult> GetProjectPages(int projectId)
        {
            Func<Project, List<Page>> extractPages = (proj) => proj.Pages.Select(
                p => new Page() { ProjectId = p.ProjectId, PageId = p.PageId, Url = p.Url }
                ).ToList();
            return await GetProjectRelatedCollections(projectId, User, "Pages", extractPages).ConfigureAwait(false);
        }

        // GET: WordSentences/5
        [HttpGet("wordsentences/{wordId}")]
        public async Task<IActionResult> WordSentences(int wordId)
        {
            Func<Word, List<WordSentence>> extractWordSentences = (word) => word.WordSentences.Select(
                ws => new WordSentence() { SentenceId = ws.SentenceId, WordId = ws.WordId, Count = ws.Count }
                ).ToList();
            return await GetWordRelatedCollections(wordId, User, "WordSentences", extractWordSentences).ConfigureAwait(false);
        }

        [HttpGet("wordpages/{wordId}")]
        public async Task<IActionResult> WordPages(int wordId)
        {
            Func<Word, List<WordPage>> extractWordPages = (word) => word.WordPages.Select(
                wp => new WordPage() { PageId = wp.PageId, WordId = wp.WordId, Count = wp.Count }
                ).ToList();
            return await GetWordRelatedCollections(wordId, User, "WordPages", extractWordPages).ConfigureAwait(false);
        }

        // GET: RelatedWordSentences/5
        [HttpGet("relatedwordsentences/{wordId}")]
        public async Task<IActionResult> RelatedWordSentences(int wordId)
        {
            WordValidationResult result = await WordValidation.ValidateRequest(
                _context, User, wordId
                ).ConfigureAwait(false);

            switch (result)
            {
                case WordValidationResult.Unauthorized:
                    return Unauthorized();
                case WordValidationResult.NotFound:
                    return NotFound();
                default:
                    break;
            }

            var word = await _context.Words.FindAsync(wordId);

            Word wordWithInclude;

            try
            {
                wordWithInclude = await _context.Words
                .Where(w => w.WordId == wordId)
                .Include(w => w.WordSentences)
                .SingleAsync()
                .ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WordExists(wordId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            Func<WordSentence, Task<List<WordSentence>>> getRelated = async (ws) =>
                await _context.WordSentences
                .Where(e => ws.SentenceId == e.SentenceId && ws.WordId != e.WordId)
                .ToListAsync();

            var relatedWordSentencs = new List<WordSentence>();
            foreach (WordSentence wordSentence in wordWithInclude.WordSentences)
            {
                relatedWordSentencs.AddRange(await getRelated(wordSentence).ConfigureAwait(false));
            }

            return Ok(relatedWordSentencs);
        }

        // GET: RelatedWordPages/5
        [HttpGet("relatedwordpages/{wordId}")]
        public async Task<IActionResult> RelatedWordPages(int wordId)
        {
            WordValidationResult result = await WordValidation.ValidateRequest(
                _context, User, wordId
                ).ConfigureAwait(false);

            switch (result)
            {
                case WordValidationResult.Unauthorized:
                    return Unauthorized();
                case WordValidationResult.NotFound:
                    return NotFound();
                default:
                    break;
            }

            var word = await _context.Words.FindAsync(wordId);

            Word wordWithInclude;

            try
            {
                wordWithInclude = await _context.Words
                .Where(w => w.WordId == wordId)
                .Include(w => w.WordPages)
                .SingleAsync()
                .ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WordExists(wordId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            Func<WordPage, Task<List<WordPage>>> getRelated = async (wp) =>
                await _context.WordPages
                .Where(e => wp.PageId == e.PageId && wp.WordId != e.WordId)
                .ToListAsync();

            var relatedWordPage = new List<WordPage>();
            foreach (WordPage wordPage in wordWithInclude.WordPages)
            {
                relatedWordPage.AddRange(await getRelated(wordPage).ConfigureAwait(false));
            }

            return Ok(relatedWordPage);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }

        private bool WordExists(int id)
        {
            return _context.Words.Any(e => e.WordId == id);
        }

        internal static Task<bool> DoesntBelongToUser(int userId, int projectId, RelatedWordsContext context)
        {
            return ProjectValidation.DoesntBelongToUser(userId, projectId, context);
        }

        internal static Task<bool> DoesntBelongToUser(int projectId, ClaimsPrincipal User, RelatedWordsContext context)
        {
            return ProjectValidation.DoesntBelongToUser(projectId, User, context);
        }

        /// <summary>
        /// Method that loads a Project with a related property, transforms it 
        /// and returns an IActionResult, that contains the transformation result.
        /// </summary>
        /// <typeparam name="TResult">
        /// </typeparam>
        /// <param name="projectId"></param>
        /// <param name="User"></param>
        /// <param name="propertyPath">
        /// String containing the propert name of the related objects that should be included with the Project.
        /// </param>
        /// <param name="ExtractFromProject">
        /// Function that takes a Project as parameter and returns any type.
        /// </param>
        /// <returns></returns>
        private async Task<IActionResult> GetProjectRelatedCollections<TResult>(
            int projectId,
            ClaimsPrincipal User,
            string propertyPath,
            Func<Project, TResult> ExtractFromProject
                )
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }

            if (await DoesntBelongToUser(projectId, User, _context))
                return Unauthorized();

            Project projWithInclude;

            try
            {
                projWithInclude = await _context.Projects
                .Where(p => p.ProjectId == projectId)
                .Include(propertyPath)
                .SingleAsync()
                .ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WordExists(projectId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(ExtractFromProject(projWithInclude));
        }

        /// <summary>
        /// Method that loads a Word with a related property, transforms it 
        /// and returns an IActionResult, that contains the transformation result.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="wordId"></param>
        /// <param name="User"></param>
        /// <param name="propertyPath">
        /// String containing the property name of the related objects that should be included with the Word.
        /// </param>
        /// <param name="ExtractFromWord">
        /// Function that takes a Word as parameter and returns any type.
        /// </param>
        /// <returns></returns>
        private async Task<IActionResult> GetWordRelatedCollections<TResult>(
            int wordId,
            ClaimsPrincipal User,
            string propertyPath,
            Func<Word, TResult> ExtractFromWord
        )
        {
            var word = await _context.Words.FindAsync(wordId);
            if (word == null)
            {
                return NotFound();
            }

            var projectId = word.ProjectId;
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }

            if (await DoesntBelongToUser(projectId, User, _context))
                return Unauthorized();

            Word wordWithInclude;

            try
            {
                wordWithInclude = await _context.Words
                .Where(w => w.WordId == wordId)
                .Include(propertyPath)
                .SingleAsync()
                .ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WordExists(projectId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(ExtractFromWord(wordWithInclude));
        }
    }
}
