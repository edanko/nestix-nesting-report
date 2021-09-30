using System.Globalization;
using System.Xml.Linq;

namespace Report.NxlReader
{
    public class TextProfile
    {
        public Point ReferencePoint { get; set; }
        public string Text { get; set; }
        public float Height { get; set; }
        public float Angle { get; set; }
        public float BoxWidth { get; set; }
        public float BoxHeight { get; set; }
        public bool IsMirrored { get; set; }
        public string GeometricMirroring { get; set; }
        public string StaticAngle { get; set; }
        public Matrix33 Matrix { get; set; }
        public string MachiningMode { get; set; }
        public string ToolGroupName { get; set; }

        public static TextProfile Read(XElement node)
        {
            var textProfile = new TextProfile
            {
                Angle = float.Parse(node.Element("Angle")?.Value!, CultureInfo.InvariantCulture),
                BoxHeight = float.Parse(node.Element("BoxHeight")?.Value!, CultureInfo.InvariantCulture),
                BoxWidth = float.Parse(node.Element("BoxWidth")?.Value!, CultureInfo.InvariantCulture),
                Matrix = Matrix33.Read(node.Element("DeltaMat33")),
                GeometricMirroring = node.Element("GeometricMirroring")?.Value,
                Height = float.Parse(node.Element("Height")?.Value!, CultureInfo.InvariantCulture),
                IsMirrored = bool.Parse(node.Element("IsMirrored")?.Value!),
                MachiningMode = node.Element("MachiningMode")?.Value,
                ToolGroupName = node.Element("ToolGroupName")?.Value,
                Text = node.Element("Text")?.Value,
                StaticAngle = node.Element("StaticAngle")?.Value,
                ReferencePoint = new Point
                {
                    X = float.Parse(node.Element("ReferencePoint")?.Attribute("X")?.Value!,
                        CultureInfo.InvariantCulture),
                    Y = float.Parse(node.Element("ReferencePoint")?.Attribute("Y")?.Value!,
                        CultureInfo.InvariantCulture)
                }
            };

            return textProfile;
        }
    }
}