using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Report.NxlReader
{
    public static class Geom
    {
        public static List<IElement> Read(XElement node)
        {
            var list = new List<IElement>();

            var start = new Point();

            foreach (var g in node.Element("Geometry")?.Elements()!)
            {
                switch (g.Name.LocalName)
                {
                    case "Start":
                        start.X = float.Parse(g.Element("X")?.Value!, CultureInfo.InvariantCulture);
                        start.Y = float.Parse(g.Element("Y")?.Value!, CultureInfo.InvariantCulture);
                        break;

                    case "LineTo":
                        var l = new Line
                        {
                            End = new Point
                            {
                                X = float.Parse(g.Element("X")?.Value!, CultureInfo.InvariantCulture),
                                Y = float.Parse(g.Element("Y")?.Value!, CultureInfo.InvariantCulture)
                            }
                        };

                        if (list.Count == 0)
                        {
                            l.Start = new Point
                            {
                                X = start.X,
                                Y = start.Y
                            };
                        }
                        else
                        {
                            l.Start = new Point
                            {
                                X = list.Last().End.X,
                                Y = list.Last().End.Y
                            };
                        }

                        list.Add(l);
                        break;

                    case "ArcTo":
                        var a = new Arc
                        {
                            End = new Point
                            {
                                X = float.Parse(g.Element("X")?.Value!, CultureInfo.InvariantCulture),
                                Y = float.Parse(g.Element("Y")?.Value!, CultureInfo.InvariantCulture)
                            },
                            Center = new Point
                            {
                                X = float.Parse(g.Element("CX")?.Value!, CultureInfo.InvariantCulture),
                                Y = float.Parse(g.Element("CY")?.Value!, CultureInfo.InvariantCulture)
                            },

                            Direction = g.Element("D")?.Value
                        };

                        if (list.Count == 0)
                        {
                            a.Start = new Point
                            {
                                X = start.X,
                                Y = start.Y
                            };
                        }
                        else
                        {
                            a.Start = new Point
                            {
                                X = list.Last().End.X,
                                Y = list.Last().End.Y
                            };
                        }

                        list.Add(a);
                        break;

                    default:
                        Console.WriteLine($"unknown profile geometry: {node.Name.LocalName}");
                        break;
                }
            }

            return list;
        }
    }
}