using System.Globalization;
using System.Xml.Linq;

namespace NxlReader
{
    public class StartLocation
    {
        public StartPoint StartPoint1 { get; set; }
        public StartPoint StartPoint2 { get; set; }
        public Point ReferencePoint { get; set; }

        // parameters

        public StartLocation()
        {
            StartPoint1 = new StartPoint();
            StartPoint2 = new StartPoint();
            ReferencePoint = new Point();
        }

        public void Read(XElement node)
        {

            ReferencePoint.X = float.Parse(node.Element("ReferencePoint").Attribute("X").Value,
                CultureInfo.InvariantCulture);
            ReferencePoint.Y = float.Parse(node.Element("ReferencePoint").Attribute("Y").Value, 
                CultureInfo.InvariantCulture);
        


                var sp1 = new StartPoint();
            StartPoint1 = sp1.Read(node.Element("StartPoint1"));

            var sp2 = new StartPoint();
            StartPoint2 = sp2.Read(node.Element("StartPoint1"));


        }
    }
}