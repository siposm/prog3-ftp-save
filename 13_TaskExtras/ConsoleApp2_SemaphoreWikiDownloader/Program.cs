using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApp2_SemaphoreWikiDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] cities = new string[]
            {
                "Miskolc", "Budapest", "Szeged", "Madrid", "Manchester", "Glasgow", "Liverpool", "Detroit",
                "Dallas", "Berlin", "Washington", "Chicago", "Barcelona", "Eger", "Szeged", "Debrecen"
            };
            List<Task> tasks = new List<Task>();
            List<ArticleProcessor> processors = new List<ArticleProcessor>();
            foreach (string city in cities)
            {
                ArticleProcessor cpu = new ArticleProcessor(city);
                processors.Add(cpu);
                // tasks.Add(Task.Run(() => cpu.ProcessArticlesBlocking()));
                // tasks.Add(Task.Factory.StartNew(() => cpu.ProcessArticlesBlocking(), TaskCreationOptions.LongRunning));
                tasks.Add(Task.Run(() => cpu.ProcessArticlesAsync()));
            }
            Console.WriteLine("ALL STARTED");
            Task.WhenAll(tasks).ContinueWith(previousTask =>
            {
                Console.WriteLine("ALL DONE");
                var q = from article in processors.SelectMany(x => x.Articles)
                        group article by article into g
                        let articleUsage = g.Count()
                        orderby articleUsage descending
                        select new { Article = g.Key, Count = articleUsage };
                foreach (var article in q.Take(20)) Console.WriteLine(article);
                foreach (var cpu in processors)
                {
                    Console.WriteLine($"{cpu.City}: {cpu.Articles.Count} articles");
                }
            });
            Console.ReadLine();
        }
    }
    class ArticleProcessor
    {
        static string urlBase = "https://en.wikipedia.org/w/api.php?action=query&prop=links&pllimit=max&format=xml&titles=";
        static SemaphoreSlim semaphore = new SemaphoreSlim(3);

        public string City { get; private set; }
        public List<string> Articles { get; private set; }
        public ArticleProcessor(string city)
        {
            City = city;
            Articles = new List<string>();
        }

        public void ProcessArticlesBlocking()
        {
            string urlExtra = string.Empty;
            Console.WriteLine(City + " BEGIN");
            do
            {
                semaphore.Wait();
                Console.WriteLine($"{City} downloading with urlExtra = {urlExtra}");
                string url = urlBase + City + urlExtra;
                WebClient client = new WebClient();
                string xml = client.DownloadString(url);
                Console.WriteLine($"{City} download OK, parsing XML ...");
                semaphore.Release();

                XDocument doc = XDocument.Parse(xml);
                var q = from node in doc.Descendants("pl")
                        select node.Attribute("title").Value;
                Articles.AddRange(q);
                if (doc.Descendants("continue").Any())
                {
                    Console.WriteLine($"{City} one more page...");
                    urlExtra = "&plcontinue=" +
                        doc.Descendants("continue").Single().Attribute("plcontinue").Value;
                }
                else urlExtra = string.Empty;
            } while (urlExtra != string.Empty);
            Console.WriteLine(City + " END");
        }


        public async Task ProcessArticlesAsync() /* Start with void, only change to Task when we realize the contiuation doesn't work */
        {
            string urlExtra = string.Empty;
            Console.WriteLine(City + " BEGIN");
            do
            {
                await semaphore.WaitAsync();
                Console.WriteLine($"{City} downloading with urlExtra = {urlExtra}");
                string url = urlBase + City + urlExtra;
                WebClient client = new WebClient();
                string xml = await client.DownloadStringTaskAsync(url);
                Console.WriteLine($"{City} download OK, parsing XML ...");
                semaphore.Release();

                XDocument doc = XDocument.Parse(xml);
                var q = from node in doc.Descendants("pl")
                        select node.Attribute("title").Value;
                Articles.AddRange(q);
                if (doc.Descendants("continue").Any())
                {
                    Console.WriteLine($"{City} one more page...");
                    urlExtra = "&plcontinue=" +
                        doc.Descendants("continue").Single().Attribute("plcontinue").Value;
                }
                else urlExtra = string.Empty;
            } while (urlExtra != string.Empty);
            Console.WriteLine(City + " END");
        }
    }

}
