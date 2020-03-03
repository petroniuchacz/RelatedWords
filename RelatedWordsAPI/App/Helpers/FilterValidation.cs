using RelatedWordsAPI.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RelatedWordsAPI.App.Helpers
{
    public static class FilterValidation
    {
        internal static async Task<bool> DoesntBelongToUser(int userId, int filterId, RelatedWordsContext context)
        {
            var filter = await context.Filters
                .Where(e => e.UserId == userId && e.FilterId == filterId)
                .AsNoTracking()
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            return filter == null ? true : false;
        }

        internal static Task<bool> DoesntBelongToUser(int userId, ClaimsPrincipal User, RelatedWordsContext context)
        {
            return DoesntBelongToUser(int.Parse(User.Identity.Name), userId, context);
        }
    }
}
