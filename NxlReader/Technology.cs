using System.Xml.Linq;

namespace NxlReader
{
    public class Technology
    {
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
        }
    }
}