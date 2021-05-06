using System.Globalization;
using System.Xml.Linq;

namespace NxlReader
{
    public class TextProfile
    {
        public Point ReferencePoint { get; set; } 
        public string Text { get; set; } 
        public float Height { get; set; } 
        public string AlignmentType { get; set; } 
        public string VerticalAlignment { get; set; } 
        public string HorizontalAlignment { get; set; } 
        public string FontName { get; set; } 
        public float Angle { get; set; } 
        public float BoxWidth { get; set; } 
        public float BoxHeight { get; set; } 
        public bool IsMirrored { get; set; } 
        public string GeometricMirroring { get; set; } 
        public string StaticAngle { get; set; } 
        public Matrix33 Matrix33 { get; set; } 
        public string MachiningMode { get; set; } 
        public string ToolGroupName { get; set; }

        public TextProfile()
        {
            Matrix33 = new Matrix33();
        }

        public static TextProfile Read(XElement node)
        {

            var m = new Matrix33();
            var textProfile = new TextProfile
            {
                AlignmentType = node.Element("AlignmentType").Value,
                Angle = float.Parse(node.Element("Angle").Value, CultureInfo.InvariantCulture),
                BoxHeight = float.Parse(node.Element("BoxHeight").Value, CultureInfo.InvariantCulture),
                BoxWidth = float.Parse(node.Element("BoxWidth").Value, CultureInfo.InvariantCulture),
                Matrix33 = m.Read(node.Element("DeltaMat33")),
                FontName = node.Element("FontName").Value,
                GeometricMirroring = node.Element("GeometricMirroring").Value,
                Height = float.Parse(node.Element("Height").Value, CultureInfo.InvariantCulture),
                HorizontalAlignment = node.Element("HorizontalAlignment").Value,
                IsMirrored = bool.Parse(node.Element("IsMirrored").Value),
                VerticalAlignment = node.Element("VerticalAlignment").Value,
                MachiningMode = node.Element("MachiningMode").Value,
                ToolGroupName = node.Element("ToolGroupName").Value,
                Text = node.Element("Text").Value,
                StaticAngle = node.Element("StaticAngle").Value,
                ReferencePoint = new Point
                {
                    X = float.Parse(node.Element("ReferencePoint").Attribute("X").Value, CultureInfo.InvariantCulture),
                    Y = float.Parse(node.Element("ReferencePoint").Attribute("Y").Value, CultureInfo.InvariantCulture)
                }
            };
            
            return textProfile;
        }
    }
}