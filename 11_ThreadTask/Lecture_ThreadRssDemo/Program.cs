using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace Lecture_ThreadRssDemo
{
    class RSSAnalyser
    {
        string[] keywords;
        string source;
        Thread thread;

        public List<string> RelevantLinks { get; private set; }

        public RSSAnalyser(string[] keywords, string source)
        {
            this.keywords = keywords;
            this.source = source;
            thread = new Thread(DoWork);
            thread.Start();
        }

        public void Join()
        {
            thread.Join();
        }

        private void DoWork()
        {
            XDocument doc = XDocument.Load(source);
            var elements = doc.Element("rss").Element("channel").Elements("item");
            // var elements = doc.Descendants("item");
            var elementsWithKeywords =
                elements.Where(
                    elem => keywords.All(
                        word => elem.Element("title").Value.ToLower().Contains(word.ToLower()) ||
                                elem.Element("description").Value.ToLower().Contains(word.ToLower())
                ));

            RelevantLinks = elementsWithKeywords.Select(el => el.Element("link").Value).ToList();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string[] sources = {
            "http://feeds.bbci.co.uk/news/world/rss.xml?edition=uk",
            "http://rss.cnn.com/rss/edition_world.rss"};

            Console.WriteLine("Give me the wanted keywords (space separator): ");
            string[] keywords = Console.ReadLine().Split(' ');

            List<RSSAnalyser> analysers = new List<RSSAnalyser>();
            foreach (string source in sources)
            {
                analysers.Add(new RSSAnalyser(keywords, source));
            }
            foreach (RSSAnalyser akt in analysers)
            {
                akt.Join();
            }

            Console.Write("All Links:\n\t"+ 
                String.Join("\n\t", analysers.SelectMany(x => x.RelevantLinks)));
            Console.ReadLine();

            foreach (RSSAnalyser analyser in analysers)
            {
                foreach (string relevantLink in analyser.RelevantLinks)
                {
                    //Process.Start(relevantLink);
                    Process.Start("cmd", $"/c start {relevantLink}");
                    Thread.Sleep(500); //ddos :) 
                }
            }
            Console.WriteLine("Finished.");
            Console.ReadLine();
        }
    }
}
