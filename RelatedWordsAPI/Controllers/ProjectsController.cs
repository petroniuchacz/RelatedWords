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

        [HttpGet]
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

            project.UserId = userId;

            _context.Entry(project).State = EntityState.Modified;

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
        [HttpGet("projectwords/{id}")]
        public async Task<IActionResult> GetProjectWords(int id)
        {
            Func<Project, List<Word>> extractWords = (proj) => proj.Words.Select(
                w => new Word() { WordContent = w.WordContent, ProjectId = w.ProjectId, WordId = w.WordId }
                ).ToList();
            return await GetProjectRelatedCollections(id, User, "Words", extractWords).ConfigureAwait(false);
        }

        // GET: api/ProjectPages/5
        [HttpGet("projectpages/{id}")]
        public async Task<IActionResult> GetProjectPages(int id)
        {
            Func<Project, List<Page>> extractPages = (proj) => proj.Pages.Select(
                p => new Page() { ProjectId = p.ProjectId, PageId = p.PageId, Url = p.Url }
                ).ToList();
            return await GetProjectRelatedCollections(id, User, "Pages", extractPages).ConfigureAwait(false);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }

        internal static Task<bool> DoesntBelongToUser(int userId, int projectId, RelatedWordsContext context)
        {
            return ProjectValidation.DoesntBelongToUser(userId, projectId, context);
        }

        internal static Task<bool> DoesntBelongToUser (int projectId, ClaimsPrincipal User, RelatedWordsContext context)
        {
            return ProjectValidation.DoesntBelongToUser(projectId, User, context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="User"></param>
        /// <param name="propertyPath"></param>
        /// <returns></returns>
        private async Task<IActionResult> GetProjectRelatedCollections<TResult>(
            int projectId, 
            ClaimsPrincipal User, 
            string propertyPath, 
            Func<Project,TResult> ExtractFromProject
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
                if (!ProjectExists(projectId))
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
    }
}
