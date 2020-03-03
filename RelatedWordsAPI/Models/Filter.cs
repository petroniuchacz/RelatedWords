using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RelatedWordsAPI.Models
{
    public class Filter
    {
        public Filter()
        {
            FilterWords = new HashSet<FilterWord>();
        }
        public int FilterId { get; set; }
        public int UserId { get; set; }
        // Increased, when something changes in the project. May serve to refresh UI.
        public int EditRevisionNumber { get; set; }

        public string Name { get; set; }
        public User User { get; set; }
        public DateTime CreatedDate { get; set; }
        public ISet<FilterWord> FilterWords { get; private set; }
        public ICollection<ProjectFilter> ProjectFilters { get; set; }
    }
}

