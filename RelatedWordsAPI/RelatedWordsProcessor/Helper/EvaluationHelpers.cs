using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RelatedWordsAPI.RelatedWordsProcessor.Helper
{
    public class EvaluationHelpers
    {
        public static Dictionary<string, int> DictWithWordCount(string content)
        {
            var dict = new Dictionary<string, int>();
            foreach (string word in content.Split())
                dict[word] = dict.ContainsKey(word) ? dict[word] + 1 : 1;
            return dict;
        }
    }
}
