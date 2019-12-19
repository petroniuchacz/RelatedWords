using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RelatedWordsAPI.Models
{
    public class Word
    {
        public int WordId { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string WordContent { get; set; }

        public ICollection<WordSentence> WordSentences { get; }
        public ICollection<WordPage> WordPages { get; }
        public Word() { }
        public Word(Project project, string wordContnet)
        {
            Project = project;
            WordContent = wordContnet;
        }

    }
}
