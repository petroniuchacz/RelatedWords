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

            return project;
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectWords(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            if (await DoesntBelongToUser(id, User, _context))
                return Unauthorized();

            Project projWithWords;

            try
            {
                projWithWords = await _context.Projects
                .Where(p => p.ProjectId == id)
                .Include(p => p.Words)
                .SingleAsync()
                .ConfigureAwait(false);
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

            return Ok(projWithWords.Words);
        }

        // GET: api/ProjectPages/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectPages(int id)
        {
            return await GetProjectRelatedCollections(id, User, "Words").ConfigureAwait(false);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }

        private static async Task<bool> DoesntBelongToUser(int userId, int projectId, RelatedWordsContext context)
        {
            var project = await context.Projects
                .Where(p => p.UserId == userId && p.ProjectId == projectId)
                .AsNoTracking()
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            return project == null ? true : false;
        }

        private static Task<bool> DoesntBelongToUser (int projectId, ClaimsPrincipal User, RelatedWordsContext context)
        {
            return DoesntBelongToUser(int.Parse(User.Identity.Name), projectId, context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="User"></param>
        /// <param name="propertyPath"></param>
        /// <returns></returns>
        private async Task<IActionResult> GetProjectRelatedCollections(int projectId, ClaimsPrincipal User, string propertyPath)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }

            if (await DoesntBelongToUser(projectId, User, _context))
                return Unauthorized();

            Project projWithWords;

            try
            {
                projWithWords = await _context.Projects
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

            return Ok(projWithWords.Words);
        }
    }
}
