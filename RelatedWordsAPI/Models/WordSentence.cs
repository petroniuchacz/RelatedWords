using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RelatedWordsAPI.Models
{
    public class WordSentence
    {
        [Key, Column(Order = 1)]
        public int WordId { get; set; }
        [Key, Column(Order = 2)]
        public int SentenceId { get; set; }
        public Word Word { get; set; }
        public Sentence Sentence { get; set; }
        public int Count { get; set; }
        public WordSentence() { }
        public WordSentence(Word word, Sentence sentence, int count)
        {
            Word = word;
            Sentence = sentence;
            Count = count;
        }
    }
}