using System.Globalization;
using System.Xml.Linq;

namespace NxlReader
{
    public class Matrix33
    {
        public double[] M { get; } = new double[9];

        public static Matrix33 Read(XElement node)
        {
            var d = new Matrix33
            {
                M =
                {
                    [0] = double.Parse(node.Element("m00")?.Value!, CultureInfo.InvariantCulture),
                    [1] = double.Parse(node.Element("m01")?.Value!, CultureInfo.InvariantCulture),
                    [2] = double.Parse(node.Element("m02")?.Value!, CultureInfo.InvariantCulture),
                    [3] = double.Parse(node.Element("m10")?.Value!, CultureInfo.InvariantCulture),
                    [4] = double.Parse(node.Element("m11")?.Value!, CultureInfo.InvariantCulture),
                    [5] = double.Parse(node.Element("m12")?.Value!, CultureInfo.InvariantCulture),
                    [6] = double.Parse(node.Element("m20")?.Value!, CultureInfo.InvariantCulture),
                    [7] = double.Parse(node.Element("m21")?.Value!, CultureInfo.InvariantCulture),
                    [8] = double.Parse(node.Element("m22")?.Value!, CultureInfo.InvariantCulture)
                }
            };

            return d;
        }

        public Point TransformPoint(Point pt)
        {
            if (pt == null)
            {
                return null;
            }

            return new Point
            {
                X = (float)(M[0] * pt.X + M[3] * pt.Y + M[6]),
                Y = (float)(M[1] * pt.X + M[4] * pt.Y + M[7])
            };
        }
    }
}