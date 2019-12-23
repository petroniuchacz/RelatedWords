using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RelatedWordsAPI.Models;

namespace RelatedWordsAPI.App.Helpers
{
    public class PageValidation
    {
        public static bool ValidatePages (IEnumerable<Page> originalPages, IEnumerable<Page> changedPages)
        {
            bool verifyPageId = ValidateCollection.Validate<Page>(originalPages, changedPages, (Page p, IEnumerable<Page> original) =>
           {
               if (p.PageId != null)
               {
                   if (original.Where(oldP => oldP.PageId == p.PageId).SingleOrDefault() == null)
                   {
                       return false;
                   }
               }
               return true;
           });
            return verifyPageId;
        }
    }
}
