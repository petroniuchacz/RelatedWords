/**
using System;
using System.Linq;
//using LemmaSharp;
using opennlp.tools.tokenize;
using opennlp.tools.lemmatizer;

namespace DevLab
{
    class Program
    {
        static void Main()
        {
            //ILemmatizer lmtz = new LemmatizerPrebuiltCompact(LemmaSharp.LanguagePrebuilt.EnglishMT);
            //var result = lmtz.Lemmatize("I'm.");
            //Console.WriteLine(result);

            SimpleTokenizer simpleTokenizer = new SimpleTokenizer();
            string[] dupens = simpleTokenizer.tokenize("I'm such a dick? But I lova dupen's. [cannot wanna couldn't]!");

            Lemmatizer lem = new Lemmatizer();
            var lemm = lem.lemmatize(lem);

            Console.WriteLine(dupens);
        }
    }
}
**/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Dynamic;
using System.IO;
using System.Linq;
using NltkNet;
using System.Net.Http;
using AngleSharp.Html.Dom;
using System.Text;
using Knyaz.Optimus;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using Knyaz.Optimus.Dom.Elements;
using Console = System.Console;

namespace DevLab
{
    class Program
    {
        static string text = "This IronPython script works fine when I run it by itself.";



        private static void TestNltkResultClass()
        {
            var corpus = new Nltk.Corpus.Inaugural();

            // example of using NltkResult class
            var fileidsResult = corpus.FileIds();

            // Get .NET List<string>
            List<string> fileidsNet = fileidsResult.AsNet;

            // Get IronPython.Runtime.List
            IronPython.Runtime.List fileidsPython = fileidsResult.AsPython;

            // Cast to Dynamic to access object fields in Python-like style
            dynamic fileids = fileidsResult;

            // using DynamicObject
            Console.WriteLine(fileids[0]);
            Console.WriteLine(fileids.__len__());

            // access sentences (list of list of strings)
            var sentencesResult = corpus.Sents();
            dynamic sentences = sentencesResult;

            // Manipulating with Python object: first word in first sentense
            Console.WriteLine(sentences[0][0]);
            List<List<string>> netSentences = sentencesResult.AsNet;

            Console.WriteLine(netSentences[0][0]);              // the same with .NET object
            Console.WriteLine(netSentences.First().First());     // using LINQ
        }

        static void TestTokenize()
        {
            var tuples = Nltk.Tokenize.Util.RegexpSpanTokenize(text, "\\s");

            var list = Nltk.Tokenize.SentTokenize(text).AsNet;
            foreach (var item in list)
                Console.Write(item + ", ");
        }

        static void TestProbability()
        {
            var words = Nltk.Tokenize.WordTokenize(text);
            var fd = new Nltk.Probability.FreqDist(words.AsPython);

            var result = fd.MostCommon(null).AsNet;
            foreach (var item in result)
                Console.WriteLine(item.Key + ": " + item.Value);
        }

        static void TestStem()
        {
            var stemmer = new Nltk.Stem.PorterStemmer();
            var words = new List<string>() { "program", "programs", "programmer", "programming", "programmers" };
            var stem = stemmer.Stem("girls");

            Console.WriteLine("Stem: " + stem);

            var lemmatizer = new Nltk.Stem.WordNetLemmatizer();
            Console.WriteLine("Lemmatize: " + lemmatizer.Lemmatize("best"));
        }


        private static void TestCorpus()
        {
            // NOTE: brown corpus have to be installed. By default to %appdata%\nltk_data\corpora\brown
            // See https://github.com/nrcpp/NltkNet/blob/master/NltkNet/Nltk/Nltk.Corpus.cs for more corpora
            var corpus = new Nltk.Corpus.Brown();

            var fileidsResult = corpus.FileIds();
            List<string> fileidsNet = fileidsResult.AsNet;
            dynamic fileids = fileidsResult;

            Console.WriteLine(fileids[0]);

            var words = corpus.Words(fileidsNet.First());
            var sentences = corpus.Sents(fileidsNet.First());
            var paragraphs = corpus.Paras(fileidsNet.First());
            string text = corpus.Raw(fileidsNet.First());
            var taggedWords = corpus.TaggedWords(fileidsNet.First());

            var stopWordsCorpus = new Nltk.Corpus.StopWords();
            var stopWords = stopWordsCorpus.Words(null);

            // Process given 
            Console.WriteLine("Stopwords: \r\n" + string.Join(", ", stopWords));
            Console.WriteLine("Words from Brown corpus: \r\n" + string.Join(", ", taggedWords.AsNet));
        }


        static async Task Main(string[] args)
        {
            Nltk.Init(new List<string>
            {
                @"C:\Program Files\IronPython 2.7\Lib\site-packages",
                @"C:\Program Files\IronPython 2.7\Lib\",
            });

            //TestNltkResultClass();
            //TestCorpus();
            //TestTokenize();
            //TestProbability();
            //TestStem();
            //var page = await GetPage(@"https://www.cnbc.com/2019/12/12/trade-domm-191212-ec.html").ConfigureAwait(false);
            var page = FilteredPage();
             List<Func<string, string>> _filters = new List<Func<string, string>>
            {
                Filters.Instance.HtmlFilter,
                // Filters.Instance.LowerCase,
                Filters.Instance.SegmentAndLammatize,
                Filters.Instance.RemoveLooseInterpuction,
                Filters.Instance.Myfilter,
            };

            string filteredContent = _filters.Aggregate(page,
                        (content, filter) => filter(content));


            Console.Write(filteredContent);
            //Console.Write("######################################################");
            //Console.Write(page);
        }



        static StringBuilder ExtractTextFromHtml (AngleSharp.Dom.IElement htmlElement)
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

        async static Task<string> GetPage(string url)
        {
            var engine = EngineBuilder.New()
                .ConfigureResourceProvider(x => x.Http().Notify(
                    request => { request.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36"; },
                    response => { /*handle response */}))
                .UseJint()
                .Build();

            var page = await engine.OpenUrl(url);
            
            var document = page.Document;
            
            return document.Body.OuterHTML;
        }

        public static string FilteredPage()
        {
            return @"
market watchlist business invest tech politics cnbc tv menu search quote market watchlist business invest tech politics cnbc tv menu wimpy trade deal will enough for market  long  there  no new tariff key point president donald trump  expect hold off new tariff chinese good but he may not have much trade deal hand when he do
strategist expect trump either say there  promise deal talk continue announce very slim agreement that would result china buy more u.s agricultural product
president  meet with his advisor thursday form plan ahead dec 15 deadline for new tariff $ 156 billion chinese good
video 2:52 02:52 u reach trade deal principle with china president donald trump  not expect impose new tariff china this weekend but he may not have much trade deal show either
source tell cnbc that trump administration have reach agreement principle phase one trade deal
source say u.s negotiator offer cancel new tariff that would take effect sunday  also willing cut exist tariff up 50 $ 360 billion chinese good
rollback exist tariff  consider important potentially deal-breaking request from china
trump set dec 15 deadline new batch tariff $ 156 billion chinese good which target many consumer product such  cell phone laptop toy
video 1:51 01:51 three wall street expert u china reach trade deal principal this  president help santa claus rally even more say prudential financial chief market strategist quincy krosby
this  as much signal  we ve want
trump meet with his advisor white house thursday  weekend deadline approach
devil  detail
you just look his sound bite oct 11 they re identical what he just say
chance more tariff 15th  slim say greg valliere chief u.s policy strategist agf investment
i think they ll find some way finesse that
term some big sweeping deal s either wimpy deal no deal
one thing you can sure  trump will spin this  great victory
fair he s have hell week
he s go acquitted senate
he get defense bill with family leave for federal worker apparently usmca valliere say note that trade agreement with canada mexico could take awhile get through senate
we avoid tariff 15th s plus for market
base case for many wall street firm have  that tariff would not implement sunday there would either phase one deal focus china buy u.s agricultural product least commitment agreement near future
tough part negotiation  expect resume next year such issue  intellectual property technology transfer
video 2:58 02:58 removal tariff would significant upside surprise strategist most important likely include increase agricultural import china indefinite delay october december tariff
beyond this there  great uncertainty he note
our base case  that china make modest compromise financial-sector access intellectual-property protection fx transparency
return we think u remove manipulator label extend waiver for u company sell input huawei
harris put low odds comprehensive deal
us-china conflict  much broad than trade war
 struggle between world s lead superpower between fundamentally different economic model he note
there  many flash point this struggle
most important hardest resolve  tech war which combine concern about fairness trade long-run economic growth national security
goldman sachs economist also expect deal which u.s would delay scrap december tariff roll back sept 1 tariff about $ 100 billion return for china buy agricultural product other concession
under this baseline scenario we expect trade war drag growth fade end 2020 they write
however escalation continue december tariff  go into effect trade drag sequential 2020 growth would 0.4pp large than our baseline
cnbc s michael bloom contribute this story
related tag subscribe cnbc pro license reprint join cnbc panel supply chain value advertise with u close caption digital product term service privacy policy news release internships correction about cnbc adchoices site map contact career help news tip get confidential news tip
we want hear from you
get touch cnbc newsletter sign up for free newsletter get more cnbc deliver your inbox sign up now data  real-time snapshot data  delay least 15 minute
global business financial news stock quote market data analysis";
        }
    }
}