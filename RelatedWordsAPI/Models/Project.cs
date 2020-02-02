using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
        public string Name { get; set; }
        public User User { get; set; }
        public ProjectProcessingStatus ProcessingStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<Page> Pages { get; set; }
        public ISet<Word> Words { get; private set; }
    }
}
