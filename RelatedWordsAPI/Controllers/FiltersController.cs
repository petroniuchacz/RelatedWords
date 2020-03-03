using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using RelatedWordsAPI.Models;
using RelatedWordsAPI.Services;
using RelatedWordsAPI.App.Helpers;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RelatedWordsAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FiltersController : ControllerBase
    {
        private readonly RelatedWordsContext _context;

        public FiltersController(RelatedWordsContext context)
        {
            _context = context;
        }
        // GET: /<controller>/
        // GET: api/Filters
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Filter>>> GetFilters()
        {
            return await _context.Filters.ToListAsync();
        }

        [HttpGet("getuserfilters")]
        // GET: api/Filters/getuserfilters
        public async Task<ActionResult<IEnumerable<Filter>>> GetUserFilters()
        {
            int userId = int.Parse(User.Identity.Name);
            return await _context.Filters
                .Where(el => el.UserId == userId)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        [HttpGet("{filterId}")]
        // GET: api/Filters/getuserprojects
        public async Task<ActionResult<IEnumerable<Filter>>> GetFilter(int filterId)
        {
            var filter = await _context.Filters.FindAsync(filterId);
            if (filter == null)
                return NotFound();

            int userId = int.Parse(User.Identity.Name);
            if (await FilterValidation.DoesntBelongToUser(userId, filterId, _context))
                return Unauthorized();

            return Ok(filter);
        }

        [HttpPost]
        // POST: api/Filters
        public async Task<ActionResult<Filter>> PostFilter(Filter filter)
        {
            int userId = int.Parse(User.Identity.Name);
            filter.UserId = userId;

            if (String.IsNullOrEmpty(filter.Name))
                return BadRequest(new { message = "Filter name can't be empty"});

            var filterWithSameName = await _context.Filters
                .Where(e => e.Name == filter.Name && e.UserId == userId)
                .SingleOrDefaultAsync();

            if (filterWithSameName != null)
                return BadRequest(new { message = "Filter name must be unique." });

            _context.Filters.Add(filter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostFilter", new { id = filter.FilterId }, filter);
        }

        [HttpPut("{filterId}")]
        // PUT: api/Filters/1
        public async Task<ActionResult<Filter>> PutFilter(int filterId, [Bind("FilterId,Name")]Filter filter)
        {
            if (filterId != filter.FilterId)
                return BadRequest();

            var dbFilter = await _context.Filters.FindAsync(filterId);
            if (dbFilter == null)
                return NotFound();

            int userId = int.Parse(User.Identity.Name);
            if (await FilterValidation.DoesntBelongToUser(userId, filterId, _context))
                return Unauthorized();

            if (String.IsNullOrEmpty(filter.Name))
                return BadRequest("Filter name can't be empty");

            dbFilter.Name = filter.Name;
            dbFilter.EditRevisionNumber += 1;

            await _context.SaveChangesAsync();

            return Ok(dbFilter);
        }

        [HttpDelete("{filterId}")]
        // DELETE: api/Filters
        public async Task<ActionResult> DeleteFilter(int filterId)
        {
            var filter = await _context.Filters.FindAsync(filterId);
            if (filter == null)
                return NotFound();

            int userId = int.Parse(User.Identity.Name);
            if (await FilterValidation.DoesntBelongToUser(userId, filterId, _context))
                return Unauthorized();

            _context.Filters.Remove(filter);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("filterwords/{filterId}")]
        // POST: api/Filters/filterwords/{filterId}
        public async Task<ActionResult<IEnumerable<FilterWord>>> PostFilterWords(int filterId, ICollection<FilterWord> filterWords)
        {
            var filter = await _context.Filters.FindAsync(filterId);
            if (filter == null)
                return NotFound();

            int userId = int.Parse(User.Identity.Name);

            if (await FilterValidation.DoesntBelongToUser(userId, filterId, _context))
                return Unauthorized();

            var existingWords = await _context.FilterWords.Where(e => e.FilterId == filterId).ToListAsync();
            var existingWordsSet = existingWords.ToHashSet();
            var newWordSet = new HashSet<FilterWord>();
            var existingToReturnSet = new HashSet<FilterWord>();

            // Checking what words already exist and dividing them to collections which will be inserted into the table 
            // or only returned in the query response with included Ids.
            foreach (FilterWord e in filterWords) 
            { 
                if (existingWordsSet.Contains(e))
                {
                    FilterWord value;
                    existingWordsSet.TryGetValue(e, out value);
                    existingToReturnSet.Add(value);
                } 
                else
                {
                    e.FilterId = filterId;
                    newWordSet.Add(e);
                }
            }

            var newWordSetList = newWordSet.ToList();
            _context.FilterWords.AddRange(newWordSetList);
            filter.EditRevisionNumber += 1;

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return CreatedAtAction("PostFilterWords", CopyWithoutLoopReference(newWordSetList.Concat(existingToReturnSet)));
        }

        [HttpDelete("filterwords/{filterId}")]
        // DELETE: api/Filters/filterwords/{filterId}
        public async Task<ActionResult> DeleteFilterWords(int filterId, ICollection<FilterWord> filterWords)
        {
            var filter = await _context.Filters.FindAsync(filterId);
            if (filter == null)
                return NotFound();

            int userId = int.Parse(User.Identity.Name);

            if (await FilterValidation.DoesntBelongToUser(userId, filterId, _context))
                return Unauthorized();

            var existingWords = await _context.FilterWords.Where(e => e.FilterId == filterId).AsNoTracking().ToListAsync();
            var Ids = existingWords.Select(e => e.FilterWordId).ToHashSet();
            if (filterWords.Any(e => e.FilterId != filterId || !Ids.Contains(e.FilterWordId)))
                return NotFound(new { message = "One of the words marked for deletion was not found in the database." });


            _context.FilterWords.RemoveRange(filterWords);
            filter.EditRevisionNumber += 1;

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return NoContent();
        }

        [HttpGet("filterwords/{filterId}")]
        // GET: api/Filters/filterwords/{filterId}
        public async Task<ActionResult<List<FilterWord>>> GetFilterWords(int filterId)
        {
            var filter = await _context.Filters.FindAsync(filterId);
            if (filter == null)
                return NotFound();

            int userId = int.Parse(User.Identity.Name);

            if (await FilterValidation.DoesntBelongToUser(userId, filterId, _context))
                return Unauthorized();

            var existingWords = await _context.FilterWords.Where(e => e.FilterId == filterId).ToListAsync();

            return Ok(CopyWithoutLoopReference(existingWords));
        }

        private IEnumerable<FilterWord> CopyWithoutLoopReference(IEnumerable<FilterWord> filterWords)
        {
            return filterWords.Select(e =>
                new FilterWord { FilterId = e.FilterId, FilterWordId = e.FilterWordId, WordContent = e.WordContent });
        }
    }
}
