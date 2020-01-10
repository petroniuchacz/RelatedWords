using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RelatedWordsAPI.Models;
using RelatedWordsAPI.App.Helpers;

namespace RelatedWordsAPI.App.Helpers
{
    public enum WordValidationResult
    {
        NotFound,
        Unauthorized,
        Valid
    }
    public static class WordValidation
    {

        public static async Task<WordValidationResult> ValidateRequest(
            RelatedWordsContext context, 
            ClaimsPrincipal User,
            int wordId
            )
        {
            var word = await context.Words.FindAsync(wordId);
            if (word == null)
            {
                return WordValidationResult.NotFound;
            }

            var projectId = word.ProjectId;
            var project = await context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return WordValidationResult.NotFound;
            }

            if (await ProjectValidation.DoesntBelongToUser(projectId, User, context))
                return WordValidationResult.Unauthorized;

            return WordValidationResult.Valid;
        }
    }

}
