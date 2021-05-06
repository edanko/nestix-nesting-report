using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Xml.Linq;

namespace NxlReader
{
    public class Matrix33
    {
        //public double[,] m { get; set; } = new double[3, 3];
        public double[] m { get; set; } = new double[9]; 

        public Matrix33()
        {
                m[0] = 1.0;
                m[1] = 0.0;
                m[2] = 0.0;
                
                m[3] = 0.0;
                m[4] = 1.0;
                m[5] = 0.0;
                
                m[6] = 0.0;
                m[7] = 0.0;
                m[8] = 1.0;
        }

        public Matrix33 Read(XElement node)
        {
            var d = new Matrix33
            {
                m =
                {
                    [0] = double.Parse(node.Element("m00")?.Value, CultureInfo.InvariantCulture),
                    [1] = double.Parse(node.Element("m01")?.Value, CultureInfo.InvariantCulture),
                    [2] = double.Parse(node.Element("m02")?.Value, CultureInfo.InvariantCulture),
                    [3] = double.Parse(node.Element("m10")?.Value, CultureInfo.InvariantCulture),
                    [4] = double.Parse(node.Element("m11")?.Value, CultureInfo.InvariantCulture),
                    [5] = double.Parse(node.Element("m12")?.Value, CultureInfo.InvariantCulture),
                    [6] = double.Parse(node.Element("m20")?.Value, CultureInfo.InvariantCulture),
                    [7] = double.Parse(node.Element("m21")?.Value, CultureInfo.InvariantCulture),
                    [8] = double.Parse(node.Element("m22")?.Value, CultureInfo.InvariantCulture)
                }
            };




            return d;
        }


        public double ToRadians(double degrees)
        {
            return Math.PI / 180.0 * degrees;
        }

        public double ToDegrees(double rad)
        {
            return 180.0 / Math.PI * rad;
        }


        public void Mirror(double dAngle, double dX, double dY)
        {
            dAngle = ToRadians(dAngle);
            var num = Math.Sin(dAngle);
            var num2 = Math.Cos(dAngle);
            m[0] = 2.0 * num2 * num2 - 1.0;
            m[1] = 2.0 * num * num2;
            m[2] = 0.0;
            m[3] = 2.0 * num * num2;
            m[4] = 1.0 - 2.0 * num2 * num2;
            m[5] = 0.0;
            m[6] = 2.0 * dX * num * num - 2.0 * dY * num * num2;
            m[7] = 2.0 * dY * num2 * num2 - 2.0 * dX * num * num2;
            m[8] = -1.0;
        }

        public Point TransformPoint(Point pt)
        {
            if (pt == null)
            {
                return null;
            }
            return new Point
            {
                X = (float) (m[0] * pt.X + m[3] * pt.Y + m[6]),
                Y = (float) (m[1] * pt.X + m[4] * pt.Y + m[7])
            };
        }

        public void Translate(double x, double y)
        {
            m[2] = x * m[0] + y * m[1] + m[2];
            m[5] = x * m[3] + y * m[4] + m[5];
        }
        
        // degrees
        public Matrix33 Rotate(Matrix33 a, double angle)
        {
            // Matrix33 matrix = new Matrix33();
            // angle = ToRadians(angle);
            // double s = Math.Sin(angle);
            // double c = Math.Cos(angle);
            // matrix.M[0,0] = c;
            // matrix.M[0,1] = s;
            // matrix.M[1,0] = -s;
            // matrix.M[1,1] = c;
            // matrix.M[2,0] = (0.0 - x) * c + y * s + x;
            // matrix.M[2,1] = (0.0 - x) * s - y * c + y;
            // method_5(matrix);
            
            angle = ToRadians(angle);

            var c = Math.Cos(angle);
            var s = Math.Sin(angle);
            
            Matrix33 b = new Matrix33();
            
            b.m[0] = +c;
            b.m[1] = +s;
            b.m[2] = 0;
            
            b.m[3] = -s;
            b.m[4] = +c;
            b.m[5] = 0;

            b.m[6] = 0;
            b.m[7] = 0;
            b.m[8] = 1;

            return Mul(a,b);
        }

        public Matrix33 Mul(Matrix33 a, Matrix33 b)
        {
            var m = new Matrix33();
            m.m[0] = a.m[0] * b.m[0] + a.m[1] * b.m[3] + a.m[2] * b.m[6];
            m.m[1] = a.m[0] * b.m[1] + a.m[1] * b.m[4] + a.m[2] * b.m[7];
            m.m[2] = a.m[0] * b.m[2] + a.m[1] * b.m[5] + a.m[2] * b.m[8];

            m.m[3] = a.m[3] * b.m[0] + a.m[4] * b.m[3] + a.m[5] * b.m[6];
            m.m[4] = a.m[3] * b.m[1] + a.m[4] * b.m[4] + a.m[5] * b.m[7];
            m.m[5] = a.m[3] * b.m[2] + a.m[4] * b.m[5] + a.m[5] * b.m[8];

            m.m[6] = a.m[6] * b.m[0] + a.m[7] * b.m[3] + a.m[8] * b.m[6];
            m.m[7] = a.m[6] * b.m[1] + a.m[7] * b.m[4] + a.m[8] * b.m[7];
            m.m[8] = a.m[6] * b.m[2] + a.m[7] * b.m[5] + a.m[8] * b.m[8];

            return m;
        }
        
        public Matrix33 Scale(Matrix33 a, double x, double y)
        {
            var b = new Matrix33();
            b.m[0] = 1 / x;
            b.m[1] = 0;
            b.m[2] = 0;
            b.m[3] = 0;
            b.m[4] = 1 / y;
            b.m[5] = 0;
            b.m[6] = 0;
            b.m[7] = 0;
            b.m[8] = 1;
            
            return Mul(a, b);
        }
        

    }
}