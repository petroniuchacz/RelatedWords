using System;
using System.Collections.Generic;
using System.Text;
using NltkNet;

namespace RelatedWordsAPI.RelatedWordsProcessor.Helper
{
    static class WordNetLaminizerExtent
    {
        public static string Lemmatize(this Nltk.Stem.WordNetLemmatizer lem, string word, string pos)
        {
           return lem.PyObject.lemmatize(word, pos);
        }
    }
}
