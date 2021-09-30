using System;
using System.Globalization;
using System.Xml.Linq;

namespace Report.NxlReader
{
    public class DimensionLineAnnotation
    {
        public float FontSize { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
        public Point TextLocation { get; set; }

        public string GetLength()
        {
            var x = Start.X - End.X;
            var y = Start.Y - End.Y;

            var num = Math.Sqrt(x * x + y * y);

            return num.ToString("F0", CultureInfo.InvariantCulture);
        }

        public double GetAngle()
        {
            var x = End.X - Start.X;
            var y = End.Y - Start.Y;

            return Math.Atan2(y, x);
        }

        public static DimensionLineAnnotation Read(XElement node)
        {
            var dla = new DimensionLineAnnotation
            {
                FontSize = float.Parse(node.Element("FontSize")?.Value!, CultureInfo.InvariantCulture),
                Start = new Point
                {
                    X = float.Parse(node.Element("StartPoint")?.Attribute("X")?.Value!,
                        CultureInfo.InvariantCulture),
                    Y = float.Parse(node.Element("StartPoint")?.Attribute("Y")?.Value!,
                        CultureInfo.InvariantCulture)
                },
                End = new Point
                {
                    X = float.Parse(node.Element("EndPoint")?.Attribute("X")?.Value!,
                        CultureInfo.InvariantCulture),
                    Y = float.Parse(node.Element("EndPoint")?.Attribute("Y")?.Value!, CultureInfo.InvariantCulture)
                }
            };
            if (node.Element("TextLocation") != null)
            {
                dla.TextLocation = new Point
                {
                    X = float.Parse(node.Element("TextLocation")?.Attribute("X")?.Value!,
                        CultureInfo.InvariantCulture),
                    Y = float.Parse(node.Element("TextLocation")?.Attribute("Y")?.Value!,
                        CultureInfo.InvariantCulture)
                };
            }

            // foreach (var g in node.Element("Element").Elements())
            // {
            //     switch (g.Name.LocalName)
            //     {
            //         default:
            //             Console.WriteLine("unknown dimension element: {0}", node.Name.LocalName);
            //             break;
            //     }
            // }

            return dla;
        }
    }
}