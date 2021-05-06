using System.Collections.Generic;
using System.Xml.Linq;

namespace NxlReader
{
    public class Surface
    {
        public string MachiningMode { get; set; }
        public string ToolGroupName { get; set; }
        public List<IElement> Geometry { get; set; } = new List<IElement>();

        public void Read(XElement n)
        {
            MachiningMode = n.Element("MachiningMode").Value;
            ToolGroupName = n.Element("ToolGroupName").Value;

            //var g = new Geom();
            Geometry = Geom.Read(n);
        }
    }
}