using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RelatedWordsAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace RelatedWordsAPI.App.Helpers
{
    public static class ProjectValidation
    {
        internal static async Task<bool> DoesntBelongToUser(int userId, int projectId, RelatedWordsContext context)
        {
            var project = await context.Projects
                .Where(p => p.UserId == userId && p.ProjectId == projectId)
                .AsNoTracking()
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            return project == null ? true : false;
        }

        internal static Task<bool> DoesntBelongToUser(int projectId, ClaimsPrincipal User, RelatedWordsContext context)
        {
            return DoesntBelongToUser(int.Parse(User.Identity.Name), projectId, context);
        }
    }
}
