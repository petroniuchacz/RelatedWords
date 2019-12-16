using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RelatedWordsAPI.Models
{
    public class Sentence
    {
        public int SentenceId { get; set; }
        public int PageId { get; set; }
        public Page Page { get; set; }
        public int SentenceNumber { get; set; }
        public ICollection<WordSentence> WordInSentences { get; }
        public Sentence(Page page, int sentenceNumber)
        {
            Page = page;
            SentenceNumber = sentenceNumber;
        }
    }
}
