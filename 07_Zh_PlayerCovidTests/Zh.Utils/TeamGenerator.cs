using System;
using System.Xml.Linq;

namespace Zh.Utils
{
    public class TeamGenerator
    {
        static Random rnd = new Random();
        string[] familyNames = { "Szucsánszki", "Schatzl", "Márton", "Kovacsics", "Háfra", "Klujber" };
        string[] firstNames = { "Zita", "Nadine", "Gréta", "Anikó", "Noémi", "Katrin" };
        string[] codes = { "AH58HU", "BL23IK", "XL91OP", "SO78UU", "AX48CC", "9544CC" };
        string[] positions = { "RightWing", "LeftWing", "Pivot", "Centre", "Left Back", "Right Back" };

        public XDocument GetTeam(int numPlayers) 
        {
            XDocument doc = new XDocument();
            doc.Add(new XElement("players"));
            for (int i=0; i<numPlayers; i++)
            {
                XElement node = new XElement("player",
                    new XAttribute("code", codes[rnd.Next(codes.Length)]),
                    new XElement("familyName", familyNames[rnd.Next(familyNames.Length)]),
                    new XElement("firstName", firstNames[rnd.Next(firstNames.Length)]),
                    new XElement("position", positions[rnd.Next(positions.Length)])
                );
                doc.Root.Add(node);
            }
            return doc;
        } 
    }
}
