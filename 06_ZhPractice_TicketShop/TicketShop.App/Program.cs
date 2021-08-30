/*
Hozzon létre egy alkalmazást, amelyben egy koncerthelyszín jegyeladását tudjuk követni.

TicketShop.Db: három tábla, code first, adattal feltöltve
    Venue(id, name) - helyszín, ebből csak egy van most, ezt szimuláljuk
    Sector(id, code, capacity, venueId) - több szektor
    Seller(id, name, venueId) - több eladó/promóter cég

TicketShop.DailyTransactions:
    class SingleDaySales { Date + Seller + Sector + TicketsSold } => adattárolás
    class DailyStatsFactory => műveletvégzés, napi statisztikákat generál véletlenszerűen
    DailyStatsFactory(string[] sellers, string[] sectors) => konstruktor
    GenerateList(int numDays, int numInstances, int maxSold) => generáló metódus
        List<SingleDaySales> -t generál 
        A megadott seller/sector értékek közül véletlenszerűen
        A mai naphoz képest max minusz numDays nap
        Minden példányban maximum maxSold eladott jegy

TicketShop.XmlBuilder
    A listából generál napi összesítéseket
    XML, amiben naponta jelezve van, hogy aznap melyik seller és melyik sector összesen mennyi forgalmat generált

TicketShop.App
    Először kilistázza az összes sector-t és seller-t az adatbázisból
    Ezután DailyStatsFactory-val csinál egy listát
    Ezután XmlGenerator-ral csinál belőle napi statisztikákat
    Kiírja: seller-enként összesen mennyi jegyet adtak el
    Kiírja: sector-onként összesen mennyi jegyet adtak el
    Kiírja: sector-onként mennyi jegy maradt
    Kiírja: van -e hiba a statisztikákban (több jegyet adtak el, mint amennyi volt)
 */
using System;
using System.Collections.Generic;
using System.Linq;
using TicketShop.Db;
using TicketShop.DailyTransactions;
using TicketShop.XmlBuilder;

namespace TicketShop.App
{
    static class Extensions
    {
        public static void ToConsole<T>(this IEnumerable<T> input, string str)
        {
            Console.WriteLine("*** BEGIN " + str);
            foreach (T item in input)
            {
                Console.WriteLine(item.ToString());
            }
            Console.WriteLine("*** END " + str);
            Console.ReadLine();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            EventOrganizerCtx ctx = new EventOrganizerCtx();
            ctx.Venues.Select(x => x.Name).ToConsole("VENUES");
            string[] sectors = ctx.Sectors.Select(x => x.Code).ToArray();
            sectors.ToConsole("SECTORS");
            string[] sellers = ctx.Sellers.Select(x => x.Name).ToArray();
            sellers.ToConsole("SELLERS");

            var list = new DailyStatsFactory(sellers, sectors).GenerateList(10, 600, 10);
            list.ToConsole("LIST");

            var stats = XmlGenerator.GenerateXml(list);
            Console.WriteLine(stats);
            Console.ReadLine();

            var perSector = from sectorNode in stats.Descendants("sector")
                            group sectorNode by sectorNode.Attribute("code").Value into grp
                            orderby grp.Key
                            select new
                            {
                                Sector = grp.Key,
                                TotalSold = grp.Sum(x => (int)x.Element("sold"))
                            };
            var perSeller = from sellerNode in stats.Descendants("seller")
                            group sellerNode by sellerNode.Attribute("name").Value into grp
                            orderby grp.Key
                            select new
                            {
                                Seller = grp.Key,
                                TotalSold = grp.Sum(x => (int)x.Element("sold"))
                            };
            
            // Without ToList() ??? :) 
            var remainingSeats = ctx.Sectors.ToList().Select(sector => new 
            {
                Total = sector.Capacity,
                Remaining = sector.Capacity - perSector.SingleOrDefault(stat => stat.Sector == sector.Code).TotalSold
            });

            perSector.ToConsole("PerSector");
            perSeller.ToConsole("PerSeller");
            remainingSeats.ToConsole("RemainingSeats");

            Console.WriteLine("ALL OK: " + remainingSeats.All(x => x.Remaining > 0));
            Console.WriteLine("ALL OK: " + (remainingSeats.Count(x => x.Remaining < 0) == 0));
            Console.ReadLine();
        }
    }
}
