using System;
using System.Globalization;
using System.Xml.Linq;

namespace NxlReader
{
    public class StartPoint
    {
        public Point ReferencePoint { get; set; }
        public Bevel Bevel { get; set; }



        public StartPoint Read(XElement node)
        {
            var sp = new StartPoint();

            foreach (var elem in node.Elements())
            {

                switch (elem.Name.LocalName)
                {
                    case "ReferencePoint":

                        var rp = new Point
                        {
                            X = float.Parse(node.Element("ReferencePoint").Attribute("X").Value,
                                CultureInfo.InvariantCulture),
                            Y = float.Parse(node.Element("ReferencePoint").Attribute("Y").Value,
                                CultureInfo.InvariantCulture)
                        };
                        sp.ReferencePoint = rp;

                        break;

                    case "Lead":
                        break;

                    case "Bevel":
                        var b = new Bevel();

                        sp.Bevel = b.Read(node.Element("Bevel"));
                        break;
                    default:
                        Console.WriteLine("unknown startpoint elem: {0}", elem.Name.LocalName);
                        break;
                }

            }

            return sp;
        }
    }
}