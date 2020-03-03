using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RelatedWordsAPI.Models
{
    public enum FilterType
    {
        BlackFilter, WhiteFilter
    }

    public class ProjectFilter
    {
        public int FilterId { get; set; }
        public Filter Filter { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public FilterType filterType { get; set; }
    }
}
