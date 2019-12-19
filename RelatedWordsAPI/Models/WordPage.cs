using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RelatedWordsAPI.Models
{
    public class WordPage
    {
        [Key, Column(Order = 1)]
        public int WordId { get; set; }
        [Key, Column(Order = 2)]
        public int PageId { get; set; }
        public Page Page { get; set; }
        public Word Word { get; set; }
        public int Count { get; set; }
        public WordPage() { }
        public WordPage(Word word, Page page, int count)
        {
            Word = word;
            Page = page;
            Count = count;
        }
    }

}
