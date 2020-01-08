using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using System.Text;
using NltkNet;


namespace RelatedWordsAPI.RelatedWordsProcessor.Helper
{
    public class TextFilters
    {
        private static readonly TextFilters instance = new TextFilters();
        private static Dictionary<string, string> _cache = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _tagTranslator = TagTranslatorCreator();
        private Nltk.Stem.WordNetLemmatizer _lemmatizer = new Nltk.Stem.WordNetLemmatizer();

        public Dictionary<string, string> TagTranslator
        {
            get { return _tagTranslator; }
        }
        static TextFilters()
        {
        }
        private TextFilters()
        {
        }
        public static TextFilters Instance
        {
            get
            {
                return instance;
            }
        }
        public string Myfilter(string content)
        {
            string name = "MyFilter";

            string pattern;
            if (_cache.TryGetValue(name, out pattern))
                return RegexFilter(content, pattern);

            string[] words = new[] {
                "a",
                "an",
                "at",
                "to",
                "on",
                "in",
                "of",
                "if",
                "it",
                "and",
                "or",
                "be",
                "the",
                "by",
                "'s",
            };

            pattern = ConvertToRegexPattern(words);
            _cache.Add(name, pattern);

            return RegexFilter(content, pattern);
        }

        public string HtmlFilter(string content)
        {
            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(content);

            return ExtractTextFromHtml(document.Body).ToString();
        }

        private static StringBuilder ExtractTextFromHtml(AngleSharp.Dom.IElement htmlElement)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (htmlElement.ChildElementCount != 0)
                htmlElement.Children.Aggregate(stringBuilder,
                    (prev, next) => prev.Append(ExtractTextFromHtml(next)));
            else if (!(
                        htmlElement is IHtmlScriptElement ||
                        htmlElement is IHtmlSelectElement ||
                        htmlElement is IHtmlAnchorElement ||
                        htmlElement is IHtmlInlineFrameElement ||
                        htmlElement is IHtmlLinkElement ||
                        htmlElement is IHtmlMenuElement ||
                        htmlElement is IHtmlMenuItemElement ||
                        htmlElement is IHtmlStyleElement
                        ))
                stringBuilder.Append(htmlElement.TextContent);

            stringBuilder.Append("\n");

            return stringBuilder;

        }

        public string SegmentAndLammatize(string content)
        {
            var sentences = Nltk.Tokenize.SentTokenize(content).AsNet;
            var sentencesWithWords = sentences.Select(s => Nltk.Tokenize.WordTokenize(s).AsNet).ToList();


            var sentencesWithLemma = new List<IEnumerable<string>>();
            foreach (List<string> sentence in sentencesWithWords)
            {
                List<string> newSentence = new List<string>();
                sentencesWithLemma.Add(newSentence);
                var taggedSentence = PosTag(sentence);
                foreach (Tuple<string, string> taggedWord in taggedSentence)
                {
                    if (taggedWord.Item1 != "")
                    {
                        string lemma = Lemmetize(taggedWord);
                        newSentence.Add(lemma);
                    }
                }

            }

            List<string> joinedLemms = sentencesWithLemma.Select(s => string.Join(" ", s)).ToList();
            return string.Join('\n', joinedLemms);
        }

        public string RemoveNonAlphaNumeric(string content)
        {
            var sentences = content.Split("\n");
            var filteredSentences = sentences.Select(s => Regex.Replace(s, @"\p{P}", ""));
            return string.Join("\n", filteredSentences);
        }

        /// <summary>
        /// " (word " -> " word ", " word' " -> " word ", " . " -> "  " 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string RemoveLooseInterpuction(string content)
        {
            var sentences = content.Split('\n');
            var sentencesWithWords = sentences.Select(s => s.Split());
            var newSentences = new List<string>();
            foreach (IEnumerable<string> sentence in sentencesWithWords)
            {
                var newWords = new List<string>();
                foreach (string word in sentence)
                {
                    var wordWithoutLooseInterpunct = RegexFilter(word, @"\A(\p{P}|`)+|(\p{P}|`)+$\z");
                    if (!string.IsNullOrEmpty(wordWithoutLooseInterpunct))
                        newWords.Add(wordWithoutLooseInterpunct);
                }
                if (newWords.Count > 0)
                    newSentences.Add(string.Join(" ", newWords));
            }

            return string.Join("\n", newSentences);
        }

        public string LowerCase(string content)
        {
            var lower = content?.ToLower();
            return lower;
        }

        private static List<Tuple<string, string>> PosTag(List<string> words)
        {
            var lowerWords = words.Select(word => word.ToLower()).ToList();
            return Nltk.PosTag(lowerWords).AsNet;
        }

        private string Lemmetize(Tuple<string, string> taggedWord)
        {
            try
            {
                var tag = TagTranslator[taggedWord.Item2];
                lock (_lemmatizer)
                {
                    return _lemmatizer.Lemmatize(taggedWord.Item1, tag);
                } 
            }
            catch (KeyNotFoundException e)
            {
                lock(_lemmatizer)
                {
                    return _lemmatizer.Lemmatize(taggedWord.Item1);
                }
            }
        }
        private static Dictionary<string, string> TagTranslatorCreator()
        {
            var dict = new Dictionary<string, string>();

            var noun = new List<string> { "NN", "NNS", "NNP", "NNPS" };
            noun.ForEach(s => dict.Add(s, "n"));
            var verb = new List<string> { "VB", "VBD", "VBG", "VBN", "VBP", "VBZ" };
            verb.ForEach(s => dict.Add(s, "v"));
            var adverb = new List<string> { "RB", "RBR", "RBS" };
            adverb.ForEach(s => dict.Add(s, "r"));
            var adjective = new List<string> { "JJ", "JJR", "JJS" };
            adjective.ForEach(s => dict.Add(s, "a"));

            return dict;
        }

        private string RegexFilter(string content, string pattern)
        {
            return Regex.Replace(content, pattern, String.Empty, RegexOptions.IgnoreCase);
        }

        private static string ConvertToRegexPattern(IEnumerable<string> words)
        {
            return @"\b(" + string.Join("|", words.Select(w => Regex.Escape(w))) + @")\b";
        }
    }
}
