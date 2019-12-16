using System;
using System.Collections.Generic;
using System.Text;

namespace RelatedWordsAPI.Models
{
    public enum ProjectProcessingStatus
    {
        NotStarted,
        Processing,
        Finished,
        Canceled,
        Failed,
        Unknown,
    }

    public class Project
    {
        public Project()
        {
            Words = new HashSet<Word>();
        }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ProjectProcessingStatus ProcessingStatus { get; set; }
        public ICollection<Page> Pages { get; }
        public ISet<Word> Words { get; private set; }
    }
}
