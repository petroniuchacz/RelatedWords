using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using RelatedWordsAPI.Models;
using RelatedWordsAPI.App.Helpers;
using RelatedWordsAPI.Services;

namespace RelatedWordsAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly RelatedWordsContext _context;
        private readonly IUserService _userService;

        public PagesController(RelatedWordsContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/Pages
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Page>>> GetPages()
        {
            return await _context.Pages.ToListAsync();
        }

        // GET: api/Pages/5
        [HttpGet("{projectId}")]
        public async Task<ActionResult<List<Page>>> GetPages(int projectId)
        {
            if (await ProjectValidation.DoesntBelongToUser(projectId, User, _context).ConfigureAwait(false))
                return Unauthorized();
            
            var pages = await _context.Pages
                .Where(page => page.ProjectId == projectId)
                .ToListAsync()
                .ConfigureAwait(false);

            if (pages == null)
            {
                return NotFound();
            }

            return Ok(pages.Select(p => new Page { PageId = p.PageId, ProjectId = p.ProjectId, Url = p.Url }).ToList());
        }

        // PUT: api/Pages/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost("{projectId}")]
        public async Task<IActionResult> PostPages(int projectId, List<Page> pages)
        {
            if (await ProjectValidation.DoesntBelongToUser(projectId, User, _context))
                return Unauthorized();

            Project project = await _context.Projects.FindAsync(projectId);
            await _context.Entry(project).Collection(p => p.Pages).LoadAsync().ConfigureAwait(false);

            if (!PageValidation.ValidatePages(project.Pages, pages))
                return BadRequest();

            UpdatePages(project, pages);
            project.EditRevisionNumber += 1;
            project.EditPagesRevisionNumber += 1;

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

            return Ok(project.Pages.Select(p => new Page { PageId = p.PageId, ProjectId = p.ProjectId, Url = p.Url}).ToList());
        }


        private bool PageExists(int id)
        {
            return _context.Pages.Any(e => e.PageId == id);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }

        private void UpdatePages(Project project, ICollection<Page> newPages)
        {
            ICollection<Page> oldPages = project.Pages;
            for (int i = 0; i < oldPages.Count; i++)
            {
                Page oldP = oldPages.ElementAt(i);
                if (newPages.Where(np => np.PageId == oldP.PageId).SingleOrDefault() == null)
                {
                    oldPages.Remove(oldP);
                    _context.Entry(oldP).State = EntityState.Deleted;
                }
            }

            newPages
                .Where(np => np.PageId == null)
                .ToList()
                .ForEach(np => oldPages.Add(np));
        }
    }
}
