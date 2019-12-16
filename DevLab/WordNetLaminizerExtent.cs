using System;
using System.Collections.Generic;
using System.Text;
using NltkNet;

namespace DevLab
{
    static class WordNetLaminizerExtent
    {
        public static string Lemmatize(this Nltk.Stem.WordNetLemmatizer  lem, string word, string pos) => lem.PyObject.lemmatize(word, pos);
    }
}
