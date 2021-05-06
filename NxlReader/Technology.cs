using System.Collections.Generic;
using System.Xml.Linq;

namespace NxlReader
{
    public class Technology
    {
        public List<StartLocation> StartLocations { get; set; } = new List<StartLocation>();

        public int RidgesCount { get; set; }


        public void Read(XElement node)
        {

            RidgesCount = 0;

            foreach (var e in node.Elements("TechElements").Elements())
            {
                switch (e.Name.LocalName)
                {
                    case "Ridge":
                        RidgesCount++;
                        break;
                }
            }


            /*foreach (var st in node.Elements("StartLocations"))
            {
                var stLoc = new StartLocation();

                stLoc.Read(st.Element("StartLocation"));


                StartLocations.Add(stLoc);

            }*/
            
        }
    }
}
