using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TicketShop.DailyTransactions;

namespace TicketShop.XmlBuilder
{
    public class XmlGenerator
    {
        public static XDocument GenerateXml(List<SingleDaySales> list)
        {
            XDocument output = new XDocument(new XElement("stats"));

            var q1 = from singleStat in list
                     group singleStat by singleStat.Date into dateGroup
                     orderby dateGroup.Key
                     select dateGroup;
            foreach (var grp in q1)
            {
                XElement node = new XElement("day");
                node.SetAttributeValue("date", grp.Key.Date.ToShortDateString());

                var q2 = from item in grp
                         group item by item.Sector into sectorGrp
                         select new XElement("sector",
                            new XAttribute("code", sectorGrp.Key),
                            new XElement("sold", sectorGrp.Sum(x => x.TicketsSold)));
                var q3 = from item in grp
                         group item by item.Seller into sellerGrp
                         select new XElement("seller",
                            new XAttribute("name", sellerGrp.Key),
                            new XElement("sold", sellerGrp.Sum(x => x.TicketsSold)));
                q2.ToList().ForEach(subNode => node.Add(subNode));
                q3.ToList().ForEach(subNode => node.Add(subNode));

                output.Root.Add(node);
            }
            return output;
        }
    }
}
