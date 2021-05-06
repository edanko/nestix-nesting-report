using System;
using System.Globalization;
using System.Xml.Linq;

namespace NxlReader
{
    public class DimensionLineAnnotation
    {
        public IElement Element { get; set; } 
        public string FontName { get; set; } 
        public string FontSize { get; set; } 
        public string FontStyle { get; set; } 
        public string Offset { get; set; } 
        public Point StartPoint { get; set; } 
        public Point EndPoint { get; set; } 
        public Point TextLocation { get; set; } 
        public string Precision { get; set; } 
        public string DistanceToTextLocation { get; set; } 
        public string ExtraText { get; set; } 
        public string Symbol { get; set; } 
        public string DrawDiameterOrRadius { get; set; }


        public static DimensionLineAnnotation Read(XElement node)
        {
            var dla = new DimensionLineAnnotation
            {

                FontName = node.Element("FontName")?.Value,
                FontSize = node.Element("FontSize")?.Value,
                FontStyle = node.Element("FontStyle")?.Value,
                Offset = node.Element("Offset")?.Value,
                StartPoint = new Point
                {
                    X = float.Parse(node.Element("StartPoint").Attribute("X").Value, CultureInfo.InvariantCulture),
                    Y = float.Parse(node.Element("StartPoint").Attribute("Y").Value, CultureInfo.InvariantCulture)

                },
                EndPoint = new Point
                {
                    X = float.Parse(node.Element("EndPoint").Attribute("X").Value, CultureInfo.InvariantCulture),
                    Y = float.Parse(node.Element("EndPoint").Attribute("Y").Value, CultureInfo.InvariantCulture)

                },
                TextLocation = new Point
                {
                    X = float.Parse(node.Element("TextLocation").Attribute("X").Value, CultureInfo.InvariantCulture),
                    Y = float.Parse(node.Element("TextLocation").Attribute("Y").Value, CultureInfo.InvariantCulture)

                },
                Precision = node.Element("Precision")?.Value,
                DistanceToTextLocation = node.Element("DistanceToTextLocation")?.Value
            };


            foreach (var g in node.Element("Element").Elements())
            {
                switch (g.Name.LocalName)
                {
                    case "Line":
                        dla.Element =  Line.Read(g);//.Element("Line"));

                        break;

                    default:
                        Console.WriteLine("unknown dimension element: {0}", node.Name.LocalName);
                        break;
                }
            }

            return dla;
        }
    }
}