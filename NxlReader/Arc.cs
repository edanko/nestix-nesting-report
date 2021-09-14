using System;
using System.Globalization;
using System.Xml.Linq;

namespace NxlReader
{
    public class Arc : IElement
    {
        public Point Start { get; set; }
        public Point End { get; set; }
        public Point Center { get; set; }
        public string Direction { get; set; }
        public string MachiningMode { get; set; }
        public string ToolGroupName { get; set; }
        public string PartToolLayerType { get; set; }


        public double StartAngle
        {
            get
            {
                var num = Math.Atan2(Start.Y - Center.Y, Start.X - Center.X) * (180 / Math.PI);
                if (num < 0.0)
                {
                    num += 360;
                }

                return num;
            }
        }

        public double EndAngle
        {
            get
            {
                var num = Math.Atan2(End.Y - Center.Y, End.X - Center.X) * (180 / Math.PI);
                if (num < 0.0)
                {
                    num += 360;
                }

                return num;
            }
        }

        public double SweepAngle
        {
            get
            {
                var startAngle = StartAngle;
                var endAngle = EndAngle;
                if (Math.Abs(startAngle - endAngle) < 0.01)
                {
                    return 360;
                }

                if (Direction == "CCW")
                {
                    if (startAngle > endAngle)
                    {
                        return 360 - startAngle + endAngle;
                    }

                    return endAngle - startAngle;
                }

                if (startAngle > endAngle)
                {
                    return endAngle - startAngle;
                }

                return endAngle - startAngle - 360;
            }
        }

        public static double GetDistance(double sx, double sy, double ex, double ey)
        {
            var x = sx - ex;
            var y = sy - ey;
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        public double Radius
        {
            get
            {
                var num = Math.Abs(GetDistance(Start.X, Start.Y, Center.X, Center.Y));
                var num2 = Math.Abs(GetDistance(End.X, End.Y, Center.X, Center.Y));

                return (num + num2) / 2;
            }
        }

        public static IElement Read(XElement node)
        {
            var arc = new Arc
            {
                Start = new Point
                {
                    X = float.Parse(node.Element("StartPoint")?.Attribute("X")?.Value!, CultureInfo.InvariantCulture),
                    Y = float.Parse(node.Element("StartPoint")?.Attribute("Y")?.Value!, CultureInfo.InvariantCulture)
                },
                End = new Point
                {
                    X = float.Parse(node.Element("EndPoint")?.Attribute("X")?.Value!, CultureInfo.InvariantCulture),
                    Y = float.Parse(node.Element("EndPoint")?.Attribute("Y")?.Value!, CultureInfo.InvariantCulture)
                },
                Center = new Point
                {
                    X = float.Parse(node.Element("CenterPoint")?.Attribute("X")?.Value!, CultureInfo.InvariantCulture),
                    Y = float.Parse(node.Element("CenterPoint")?.Attribute("Y")?.Value!, CultureInfo.InvariantCulture)
                },
                MachiningMode = node.Element("MachiningMode")?.Value,
                ToolGroupName = node.Element("ToolGroupName")?.Value,
                PartToolLayerType = node.Element("PartToolLayerType")?.Value
            };

            return arc;
        }
    }
}