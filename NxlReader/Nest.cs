using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace NxlReader
{
    public class Nest
    {
        public Machine Machine { get; set; }

        public Plate Plate { get; set; } = new Plate();

        //public List<Part> OriginalParts { get; set; } = new List<Part>();
        public List<Remnant> OriginalRemnants { get; set; } = new List<Remnant>();
        public List<Part> Parts { get; set; } = new List<Part>();
        public List<Remnant> Remnants { get; set; } = new List<Remnant>();
        public Annotations Annotations { get; set; } = new Annotations();
        public List<TextProfile> Texts { get; set; } = new List<TextProfile>();

        public int TextSymbolsCount { get; set; }

        public List<Bridge> Bridges { get; set; } = new List<Bridge>();
        public int BridgesCount { get; set; }
        public int RidgesCount { get; set; }

        public void Read(string filename)
        {
            if (!File.Exists(filename))
            {
                return;
            }

            using var stream = new FileInfo(filename).OpenRead();
            using var input = new GZipStream(stream, CompressionMode.Decompress);
            var doc = XDocument.Load(input);
            var elems = doc.Element("NestFile")?.Element("Nest");

            #region machine

            Machine = new Machine
            {
                Name = elems?.Element("Machine")?.Attribute("Name")?.Value,
                Technology = elems?.Element("Machine")?.Attribute("Technology")?.Value
            };

            #endregion

            #region sheet

            if (elems.Element("Sheets").Element("Plate") != null)
            {
                Plate = new Plate(elems.Element("Sheets").Element("Plate"))
                {
                    Origin = new Point
                    {
                        X = float.Parse(elems.Element("Path").Element("HomeLocation").Attribute("X").Value, CultureInfo.InvariantCulture),
                        Y = float.Parse(elems.Element("Path").Element("HomeLocation").Attribute("Y").Value, CultureInfo.InvariantCulture),
                    }
                };
            }
            else // remnant 
            {
                Console.WriteLine("remnant as sheet");
            }
            #endregion

            #region OriginalParts
            var originalParts = new List<Part>();

            foreach (var p in elems.Element("OriginalParts").Elements())
            {
                switch (p.Name.LocalName)
                {
                case "Part":
                    var part = new Part
                    {
                        OrderlineInfo = p.Element("DbInfo").Element("ID").Value
                    };

                    foreach (var node in p.Element("Elements").Elements())
                    {
                        if (node.Name.LocalName == "Profile")
                        {
                            var pp = new Profile();
                            pp.Read(node);
                            part.Elements.Add(pp);
                        }
                    }

                    foreach (var node in p.Element("Texts").Elements())
                    {
                        if (node.Name.LocalName == "TextProfile")
                        {
                            var textProfile = TextProfile.Read(node);
                            part.Texts.Add(textProfile);
                        }
                    }

                    originalParts.Add(part);

                    break;

                case "Remnant":
                    var rem = new Remnant();

                    foreach (var node in p.Element("Elements").Elements())
                    {
                        if (node.Name.LocalName == "Profile")
                        {
                            var pp = new Profile();
                            pp.Read(node);
                            rem.Elements.Add(pp);
                        }
                    }

                    foreach (var node in p.Element("Texts").Elements())
                    {
                        if (node.Name.LocalName == "TextProfile")
                        {
                            var textProfile = TextProfile.Read(node);
                            rem.Texts.Add(textProfile);
                        }
                    }

                    OriginalRemnants.Add(rem);
                    break;
                }
            }
            #endregion

            #region PartInfos
            foreach (var p in elems.Element("PartInfos").Elements())
            {
                switch (p.Name.LocalName)
                {
                case "PartInfo":

                    var m = new Matrix33();

                    var orderlineInfo = p.Element("DbInfo").Element("ID").Value;

                    var op = originalParts.Find(x => x.OrderlineInfo == orderlineInfo);

                    var part = new Part
                    {
                        Matrix = m.Read(p.Element("Matrix"))
                    };


                    //part.OrigElements = op.Elements;
                    //part.OrigTexts = op.Texts;

                    part.Elements = op.Elements;
                    part.Texts = op.Texts;

                    foreach (var node in p.Element("Profiles").Elements())
                    {
                        if (node.Name.LocalName == "Profile")
                        {
                            var profile = new Profile();
                            profile.ReadPartInfo(node);
                            part.Elements.Add(profile);
                        }
                    }

                    // foreach (var node in p.Element("Texts").Elements())
                    // {
                    //     if (node.Name.LocalName == "TextProfile")
                    //     {
                    //         var textProfile = TextProfile.Read(node);
                    //         part.Texts.Add(textProfile);
                    //     }
                    // }

                    if (p.Element("DetailId") != null)
                    {
                        part.DetailId = TextProfile.Read(p.Element("DetailId").Element("TextProfile"));
                        //part.DetailId.ReferencePoint = m.Apply(part.DetailId.ReferencePoint);
                    }



                    Parts.Add(part);
                    break;


                case "RemnantInfo":
                    var mr = new Matrix33();

                    var rem = new Remnant()
                    {
                        Matrix = mr.Read(p.Element("Matrix"))
                    };


                    foreach (var node in p.Element("Profiles").Elements())
                    {
                        if (node.Name.LocalName == "Profile")
                        {
                            var profile = new Profile();
                            profile.ReadPartInfo(node);
                            rem.Elements.Add(profile);
                        }
                    }

                    // foreach (var node in p.Element("Texts").Elements())
                    // {
                    //     if (node.Name.LocalName == "TextProfile")
                    //     {
                    //         var textProfile = TextProfile.Read(node);
                    //         rem.Texts.Add(textProfile);
                    //
                    //     }
                    // }

                    Remnants.Add(rem);
                    break;
                }
            }

            foreach (var e in Parts.SelectMany(p => p.Elements).Cast<Profile>())
            {
                if (e.Tech != null)
                {
                    RidgesCount += e.Tech.RidgesCount;
                }
            }
            #endregion

            #region manipulate
            foreach (var part in Parts)
            {
                //var originalPart = OriginalParts[i];

                var m33 = part.Matrix;

                if (m33.m[8] < 0.0)
                {
                    foreach (var g in part.Elements.Cast<Profile>().SelectMany(e => e.Geometry))
                    {
                        if (g is Arc a)
                        {
                            a.Direction = a.Direction == "CCW" ? "CW" : "CCW";
                        }
                    }
                }

                foreach (var element in part.Elements)
                {
                    var e = (Profile) element;
                    foreach (var geom in e.Geometry)
                    {
                        geom.Start = m33.TransformPoint(geom.Start);
                        geom.End = m33.TransformPoint(geom.End);
                        geom.Center = m33.TransformPoint(geom.Center);
                    }
                }

                foreach (var e in part.Texts)
                {
                    e.Matrix33 = m33;
                    e.ReferencePoint = m33.TransformPoint(e.ReferencePoint);
                }
            }
            #endregion

            #region annotations
            foreach (var p in elems.Element("Annotations").Elements())
            {
                switch (p.Name.LocalName)
                {
                    case "AnnotationLength":
                       /* var ann = new AnnotationLength
                        {
                            DimensionLineAnnotation =
                                DimensionLineAnnotation.Read(p.Element("DimensionLineAnnotation")),
                            AnnotationSubType = p.Element("AnnotationSubType").Value,
                            Distance = p.Element("Distance").Value,
                            ElSource1 = ElSource.Read(p.Element("ElSource1")),
                            ElSource2 = ElSource.Read(p.Element("ElSource2")),
                            LineLeftContour = Line.Read(p.Element("LineLeftContour")),
                            LineRightContour = Line.Read(p.Element("LineRightContour"))
                        };
                        Annotations.AnnotationLengths.Add(ann);*/
                        break;
                    default:
                        Console.WriteLine("unknown annotation element: {0}", p.Name.LocalName);
                        break;
                }
            }
            #endregion

            #region texts

            #endregion

            #region bridges and half-bridges
            foreach (var p in elems.Element("Bridges").Elements())
            {
                switch (p.Name.LocalName)
                {
                    case "Bridge":
                        var b = Bridge.Read(p);
                        Bridges.Add(b);
                        break;


                    case "HalfBridge":
                        var hb = Bridge.Read(p);
                        Bridges.Add(hb);
                        break;

                    default:
                        Console.WriteLine("unknown bridges element: {0}", p.Name.LocalName);
                        break;
                }
            }
            #endregion

            BridgesCount = Bridges.Count;

            #region text symbols count
            TextSymbolsCount = (from op in originalParts
                from tp in op.Texts
                select tp.Text.Replace(" ", "")
                into stripped
                select stripped.Length).Sum();
            #endregion
        }
    }
}