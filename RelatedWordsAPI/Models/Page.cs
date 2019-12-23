using System;
using System.Collections.Generic;
using System.Text;

namespace RelatedWordsAPI.Models
{
    public enum PageProcessingStatus
    {
        NotStarted,
        Processing,
        Filtered,
        Finished,
        Canceled,
        Failed,
        Unknown,
    }
    public class Page
    {
        public int? PageId { get; set; }
        public int? ProjectId { get; set; }
        public Project Project { get; set; }
        public string OriginalContent { get; set; }
        public string FilteredContent { get; set; }
        public string Url { get; set; }
        public PageProcessingStatus ProcessingStatus { get; set; }
        public ICollection<Sentence> Sentences { get; }
        public ICollection<WordPage> WordPage { get; }
    }

}
