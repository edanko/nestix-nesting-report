using System.Collections.Generic;
using System.Xml.Linq;

namespace Report.NxlReader
{
    public class Profile : IElement
    {
        public string MachiningMode { get; set; }
        public string ToolGroupName { get; set; }
        public List<IElement> Geometry { get; set; } = new();
        public Technology Tech { get; set; }

        public void ReadPartInfo(XElement node)
        {
            MachiningMode = node.Element("MachiningMode")?.Value;
            ToolGroupName = node.Element("ToolGroupName")?.Value;

            var t = new Technology();
            t.Read(node.Element("Technology"));

            Tech = t;
        }

        public void Read(XElement node)
        {
            MachiningMode = node.Element("MachiningMode")?.Value;
            ToolGroupName = node.Element("ToolGroupName")?.Value;

            Geometry = Geom.Read(node);
        }

        public Point Start { get; set; }
        public Point End { get; set; }
        public Point Center { get; set; }
    }
}