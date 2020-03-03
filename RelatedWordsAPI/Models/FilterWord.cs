using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RelatedWordsAPI.Models
{
    public class FilterWord : IEquatable<FilterWord>
    {
        public int FilterWordId { get; set; }
        public int FilterId { get; set; }
        public Filter Filter { get; set; }
        public string WordContent { get; set; }
        public FilterWord() { }
        public FilterWord(string wordContnet, Filter filter)
        {
            WordContent = wordContnet;
            Filter = filter;
        }
        public override int GetHashCode()
        {
            return WordContent.GetHashCode() ^ FilterId.GetHashCode();
        }

        public bool Equals(FilterWord word)
        {
            return word.FilterId == FilterId && word.WordContent == WordContent;
        }
    }
}
