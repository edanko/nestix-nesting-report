using System.Collections.Generic;
using System.Xml.Linq;

namespace NxlReader
{
    public class Bevel
    {
        //public Parameters Parameters { get; set; } 

        public List<Surface> Surfaces { get; set; } 

        //public string Id { get; set; } 

        public Bevel()
        {
            Surfaces = new List<Surface>();
        }

        public Bevel Read(XElement n)
        {
            var b = new Bevel();

            foreach (var s in n.Elements("Surfaces"))
            {
                var surf = new Surface();

                surf.Read(s.Element("Surface"));

                b.Surfaces.Add(surf);
            }

            return b;
        }
    }
}