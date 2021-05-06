using System.Collections.Generic;
using System.Xml.Linq;

namespace NxlReader
{
    public class Plate { 
        
        public string PlateWidth { get; set; }
        public string PlateLength { get; set; }
        public Point Origin { get; set; }
        public List<Profile> Profiles { get; set; } = new List<Profile>();
        
        public Plate() {}

        public Plate(XElement n)
        {
            Read(n);
        }


        public void Read(XElement n)
        {
            PlateLength = n.Element("PlateLength").Value;
            PlateWidth = n.Element("PlateWidth").Value;
            
            foreach (var prof in n.Element("Profiles").Elements())
            {
                var p = new Profile();
                p.Read(prof);
                Profiles.Add(p);
            }
        }
    }
}
