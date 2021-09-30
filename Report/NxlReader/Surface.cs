using System.Collections.Generic;
using System.Xml.Linq;

namespace Report.NxlReader
{
    public class Surface
    {
        public string MachiningMode { get; set; }
        public string ToolGroupName { get; set; }
        public List<IElement> Geometry { get; set; } = new();

        public void Read(XElement n)
        {
            MachiningMode = n.Element("MachiningMode")?.Value;
            ToolGroupName = n.Element("ToolGroupName")?.Value;
            Geometry = Geom.Read(n);
        }
    }
}