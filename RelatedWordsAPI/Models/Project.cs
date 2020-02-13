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
        // Increased, when something changes in the project. May serve to refresh UI.
        public int EditRevisionNumber { get; set; }
        // Increased, when something changes in the project pages. 
        // Together with ProcessedPagesRevisionNumber may serve to know that the processing results are outdated.
        public int EditPagesRevisionNumber { get; set; }
        // Increased, when the project is processed.
        public int ProcessingRevisionNumber { get; set; }
        // Contains the EditPagesRevisionNumber, when the project was last processed.
        public int ProcessedPagesRevisionNumber { get; set; }
        public string Name { get; set; }
        public User User { get; set; }
        public ProjectProcessingStatus ProcessingStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<Page> Pages { get; set; }
        public ISet<Word> Words { get; private set; }
    }
}
