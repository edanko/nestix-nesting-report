using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using Force.DeepCloner;

namespace Report.NxlReader
{
    public class Nest
    {
        public Machine Machine { get; private set; }
        public Plate Plate { get; private set; }
        public List<Remnant> OriginalRemnants { get; set; } = new();
        public List<Part> Parts { get; set; } = new();

        private List<Remnant> Remnants { get; set; } = new();

        //public List<DimensionLineAnnotation> DimensionLineAnnotations = new();
        public List<TextProfile> Texts { get; set; } = new();
        public int TextSymbolsCount { get; private set; }
        private List<Bridge> Bridges { get; set; } = new();
        public int BridgesCount { get; private set; }
        public int RidgesCount { get; private set; }

        public void Read(string filename)
        {
            if (!File.Exists(filename))
            {
                return;
            }

            var stream = new FileInfo(filename).OpenRead();
            var input = new GZipStream(stream, CompressionMode.Decompress);
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

            if (elems?.Element("Sheets")?.Element("Plate") != null)
            {
                Plate = new Plate(elems.Element("Sheets")?.Element("Plate"));
            }
            else if (elems?.Element("Sheets")?.Element("Sheet") != null)
            {
                Plate = new Plate(elems.Element("Sheets")?.Element("Sheet"));
            }

            #endregion

            #region OriginalParts

            var originalParts = new List<Part>();

            foreach (var p in elems?.Element("OriginalParts")?.Elements()!)
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
                                part.Profiles.Add(pp);
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
                                rem.Profiles.Add(pp);
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

            foreach (var pi in elems.Element("PartInfos")?.Elements())
            {
                switch (pi.Name.LocalName)
                {
                    case "PartInfo":
                        var orderlineInfo = pi.Element("DbInfo")?.Element("ID")?.Value;
                        var op = originalParts.Find(x => x.OrderlineInfo == orderlineInfo).DeepClone();
                        if (op == null)
                        {
                            continue;
                        }

                        var p = new Part
                        {
                            Matrix = Matrix33.Read(pi.Element("Matrix")),
                            Profiles = op.Profiles.ToList(),
                            Texts = op.Texts.ToList()
                        };

                        foreach (var node in pi.Element("Profiles").Elements())
                        {
                            switch (node.Name.LocalName)
                            {
                                case "Profile":
                                {
                                    var profile = new Profile();
                                    profile.ReadPartInfo(node);
                                    p.Profiles.Add(profile);
                                    break;
                                }
                            }
                        }

                        if (pi.Element("DetailId") != null)
                        {
                            p.DetailId = TextProfile.Read(pi.Element("DetailId").Element("TextProfile"));
                        }

                        Parts.Add(p);
                        break;

                    case "RemnantInfo":
                        var rem = new Remnant()
                        {
                            Matrix = Matrix33.Read(pi.Element("Matrix"))
                        };

                        foreach (var node in pi.Element("Profiles")?.Elements())
                        {
                            switch (node.Name.LocalName)
                            {
                                case "Profile":
                                {
                                    var profile = new Profile();
                                    profile.ReadPartInfo(node);
                                    rem.Profiles.Add(profile);
                                    break;
                                }
                            }
                        }

                        Remnants.Add(rem);
                        break;
                }
            }

            foreach (var e in Parts.SelectMany(p => p.Profiles))
            {
                if (e.Tech != null)
                {
                    RidgesCount += e.Tech.RidgesCount;
                }
            }

            #endregion

            #region manipulate

            foreach (var p in Parts)
            {
                //var p = Parts[i];
                var m = p.Matrix;

                if (m.M[8] < 0.0)
                {
                    foreach (var g in p.Profiles.SelectMany(e => e.Geometry))
                    {
                        if (g is Arc a)
                        {
                            a.Direction = a.Direction == "CCW" ? "CW" : "CCW";
                        }
                    }
                }

                // foreach (var g in Parts[i].Profiles.SelectMany(e => e.Geometry))
                // {
                //     g.Start = m.TransformPoint(g.Start);
                //     g.End = m.TransformPoint(g.End);
                //     g.Center = m.TransformPoint(g.Center);
                // }

                foreach (var t in p.Texts)
                {
                    t.Matrix = m;
                    t.ReferencePoint = m.TransformPoint(t.ReferencePoint);
                }
            }

            #endregion

            #region annotations

            /*foreach (var p in elems.Element("Annotations")?.Elements())
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
                         Annotations.AnnotationLengths.Add(ann);* /
                        var dim = p.Element("DimensionLineAnnotation");

                        DimensionLineAnnotations.Add(DimensionLineAnnotation.Read(dim));

                        break;
                    default:
                        Console.WriteLine("unknown annotation element: {0}", p.Name.LocalName);
                        break;
                }
            }*/

            #endregion

            #region texts

            foreach (var node in elems.Element("Texts").Elements())
            {
                if (node.Name.LocalName == "TextProfile")
                {
                    var textProfile = TextProfile.Read(node);
                    Texts.Add(textProfile);
                }
            }

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

        public RectangleF GetBBox()
        {
            var points = new List<Point>();

            points.AddRange(Plate.Profiles.Where(p => p.Geometry != null).SelectMany(p => p.Geometry)
                .Where(p => p.Start != null).Select(g => g.Start));
            points.AddRange(Plate.Profiles.Where(p => p.Geometry != null).SelectMany(p => p.Geometry)
                .Where(p => p.Center != null).Select(g => g.Center));
            points.AddRange(Plate.Profiles.Where(p => p.Geometry != null).SelectMany(p => p.Geometry)
                .Where(p => p.End != null).Select(g => g.End));

            // Parts may contain points outside the plate, so skip
            // points.AddRange(Parts.SelectMany(p => p.Profiles).Where(p => p.Geometry != null).SelectMany(p => p.Geometry).Where(p => p.Start != null).Select(g => g.Start));
            // points.AddRange(Parts.SelectMany(p => p.Profiles).Where(p => p.Geometry != null).SelectMany(p => p.Geometry).Where(p => p.Center != null).Select(g => g.Center));
            // points.AddRange(Parts.SelectMany(p => p.Profiles).Where(p => p.Geometry != null).SelectMany(p => p.Geometry).Where(p => p.End != null).Select(g => g.End));

            // Assume remnants always inside the plate, so skip
            // points.AddRange(Remnants.SelectMany(r => r.Profiles).Where(p => p.Geometry != null).SelectMany(p => p.Geometry).Where(p => p.Start != null).Select(g => g.Start));
            // points.AddRange(Remnants.SelectMany(r => r.Profiles).Where(p => p.Geometry != null).SelectMany(p => p.Geometry).Where(p => p.Center != null).Select(g => g.Center));
            // points.AddRange(Remnants.SelectMany(r => r.Profiles).Where(p => p.Geometry != null).SelectMany(p => p.Geometry).Where(p => p.End != null).Select(g => g.End));

            // points.AddRange(OriginalRemnants.SelectMany(r => r.Profiles).Where(p => p.Geometry != null).SelectMany(p => p.Geometry).Where(p => p.Start != null).Select(g => g.Start));
            // points.AddRange(OriginalRemnants.SelectMany(r => r.Profiles).Where(p => p.Geometry != null).SelectMany(p => p.Geometry).Where(p => p.Center != null).Select(g => g.Center));
            // points.AddRange(OriginalRemnants.SelectMany(r => r.Profiles).Where(p => p.Geometry != null).SelectMany(p => p.Geometry).Where(p => p.End != null).Select(g => g.End));

            // Texts on parts may be placed outside of profile
            points.AddRange(Parts.SelectMany(t => t.Texts).Select(p => p.ReferencePoint));
            // Texts usually placed under the plate
            points.AddRange(Texts.Select(t => t.ReferencePoint));

            var minX = points.Min(p => p.X);
            var minY = points.Min(p => p.Y);
            var maxX = points.Max(p => p.X);
            var maxY = points.Max(p => p.Y);

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }
    }
}