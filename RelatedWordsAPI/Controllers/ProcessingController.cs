using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RelatedWordsAPI.Models;
using RelatedWordsAPI.Services;
using RelatedWordsAPI.App.Helpers;

namespace RelatedWordsAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessingController : ControllerBase
    {
        private readonly RelatedWordsContext _context;
        private readonly IUserService _userService;
        private readonly IRelatedWordsProcessorService _relatedWordsProcessorService;

        public ProcessingController(RelatedWordsContext context, IUserService userService, IRelatedWordsProcessorService relatedWordsProcessorService)
        {
            _context = context;
            _userService = userService;
            _relatedWordsProcessorService = relatedWordsProcessorService;
        }

        [HttpPost("{projectId}")]
        public async Task<ActionResult> Start(int projectId)
        {
            Project project = await _context.Projects.FindAsync(projectId);

            if (project == null)
            {
                return BadRequest();
            }

            if (await ProjectValidation.DoesntBelongToUser(projectId, User, _context).ConfigureAwait(false))
                return Unauthorized();

            Page page = await _context.Pages.FirstOrDefaultAsync(pa => pa.ProjectId == projectId);
            if (page == null)
            {
                return NotFound(new { message = "No pages set for this project." });
            }

            bool started = _relatedWordsProcessorService.TryStartProcessing(project);
            if (started)
                return NoContent();
            else
                return BadRequest(new { message = "Could not start processing of the project." });
        }

        [HttpGet("{projectId}")]
        public async Task<ActionResult<TaskStatus>> Status(int projectId)
        {
            Project project = await _context.Projects.FindAsync(projectId);

            if (project == null)
            {
                return BadRequest();
            }

            if (await ProjectValidation.DoesntBelongToUser(projectId, User, _context).ConfigureAwait(false))
                return Unauthorized();

            TaskStatus taskStatus;
            if (_relatedWordsProcessorService.GetProjectTaskStatus(project, out taskStatus))
            {
                return Ok(taskStatus);
            }
            else
            {
                return NotFound(new { message = "No processing task found for this project."});
            }
        }
    }
}