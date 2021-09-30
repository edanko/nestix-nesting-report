using System.Globalization;
using System.Xml.Linq;

namespace Report.NxlReader
{
    public class Line : IElement
    {
        public Point Start { get; set; }
        public Point End { get; set; }
        public Point Center { get; set; }
        public string MachiningMode { get; set; }
        public string ToolGroupName { get; set; }

        public static Line Read(XElement node)
        {
            var line = new Line
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
                MachiningMode = node.Element("MachiningMode")?.Value,
                ToolGroupName = node.Element("ToolGroupName")?.Value
            };

            return line;
        }
    }
}