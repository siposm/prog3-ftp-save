using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace TicketShop.DailyTransactions
{
    public class SingleDaySales
    {
        public DateTime Date { get; set; }
        public string Seller { get; set; }
        public string Sector { get; set; }
        public int TicketsSold { get; set; }
        public override string ToString()
        {
            return $"{Date.ToShortDateString()}: Seller {Seller}, Sector {Sector}, Sold {TicketsSold}";
        }
    }

    public class DailyStatsFactory
    {
        static Random rnd = new Random();
        string[] sectors;
        string[] sellers;

        public DailyStatsFactory(string[] sellers, string[] sectors)
        {
            this.sectors = sectors;
            this.sellers = sellers;
        }

        public List<SingleDaySales> GenerateList(int numDays, int numInstances, int maxSold)
        {
            List<SingleDaySales> output = new List<SingleDaySales>();
            for (int i=0; i<numInstances; i++)
            {
                SingleDaySales sale = new SingleDaySales()
                {
                    Date = DateTime.Now.Date.AddDays(rnd.Next(-numDays, 0)),
                    Sector = sectors[rnd.Next(sectors.Length)],
                    Seller = sellers[rnd.Next(sellers.Length)],
                    TicketsSold = rnd.Next(0, maxSold) + 1
                };
                output.Add(sale);
            }
            return output;
        }
    }
}
