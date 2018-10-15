using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SearchResultCount
{
    class SearchCount
    {
        public static void SearchFight(string[] searchTerms)
        {
            Dictionary<string, long> google = new Dictionary<string, long>();
            Dictionary<string, long> bing = new Dictionary<string, long>();
            foreach (var element in searchTerms) {
                Console.WriteLine(element);
                var googleCount = searchCount(@"http://www.google.com/search?q=" + element, "\"resultStats\">");
                Console.Write("Google result count: " + googleCount);
                var bingCount = searchCount(@"http://www.bing.com/search?q=" + element, "\"sb_count\">");
                Console.Write("\tBing result count: " + bingCount);
                Console.Write(Environment.NewLine);
                google.Add(element, googleCount);
                bing.Add(element, bingCount);
            }
            Dictionary<string, long> winners = new Dictionary<string, long>();
            winners.Add(google.Max(x => x.Key), google.Max(x => x.Value));
            if (!winners.ContainsKey(bing.Max(x => x.Key)))
                winners.Add(bing.Max(x => x.Key), bing.Max(x => x.Value));

            Console.WriteLine("Google winner: " + google.Max(x => x.Key));
            Console.WriteLine("Bing winner: " + bing.Max(x => x.Key));

            Console.WriteLine("Total winner: " + winners.Max(x => x.Key));

            Console.ReadLine();
        }

        public static long searchCount(string url, string resultTag)
        {
            using (Stream resultStream = (new WebClient()).OpenRead(url))
            {
                using (StreamReader streamReader = new StreamReader(resultStream))
                {
                    string line;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (!line.Contains(resultTag))
                            continue;
                        try
                        {
                            string firstSplit = line.Split(new String[] { resultTag }, StringSplitOptions.None)[1];
                            string secondSplit = firstSplit.Split('<')[0];
                            string number = Regex.Replace(secondSplit, "[^\\d]", "");
                            return long.Parse(number);
                        }
                        catch (Exception e) { throw e; }
                        
                    }
                    
                }
            }
            return 0;
        }
    }
}
