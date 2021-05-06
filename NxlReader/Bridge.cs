using System.Globalization;
using System.Xml.Linq;

namespace NxlReader
{
    public class Bridge
    {
        public string Type { get; set; } 
        public float Radius { get; set; }
        public float Distance { get; set; }
        public int StartId { get; set; } 
        public int EndId { get; set; } 

        public static Bridge Read(XElement node)
        {
            var b = new Bridge()
            {
                StartId = int.Parse(node.Element("BridgeStart").Attribute("id").Value),
                EndId = int.Parse(node.Element("BridgeEnd").Attribute("id").Value),

                Type = node.Element("Parameters").Element("Type").Value,
                Radius = float.Parse(node.Element("Parameters").Element("Radius").Value, CultureInfo.InvariantCulture),
                Distance = float.Parse(node.Element("Parameters").Element("Distance").Value, CultureInfo.InvariantCulture)
            };

            return b;
        }
    }
}