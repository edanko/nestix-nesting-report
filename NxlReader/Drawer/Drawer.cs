using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using SvgNet;
using SvgNet.SvgGdi;
using SvgNet.SvgTypes;

namespace NxlReader.Drawer
{
    public static class Drawer
    {
        private static SvgGraphics _g;

        public static string DrawNest(Nest n)
        {
            _g = new SvgGraphics(Color.White);
            
            _g.ScaleTransform(1, -1);
            

            // draw sheet
            DrawSheet(n.Plate);
            //DrawOrigo(n.Plate.Origin);

            foreach (var p in n.Parts)
            {
                //var p = n.Parts[10];
                foreach (var t in p.Texts)
                {
                    DrawTextProfile(t);
                }

                foreach (var pr in p.Elements.Cast<Profile>())
                {
                    DrawProfile(pr);
                }

                DrawTextProfile(p.DetailId);
            }

            //var ms = new MemoryStream();

            return _g.WriteSVGString();

            //ms.Position = 0;

            //var sr = new StreamReader(ms);

            //var str = sr.ReadToEnd();

            //return ms;
        }

        private static void DrawProfile(Profile p)
        {
            //if (p.MachiningMode != "0")
            //{
            //    return;
            //}

            foreach (var e in p.Geometry)
            {
                switch (e.GetType().Name)
                {
                    case "Line":
                        DrawLine(e, Color.Black, 2);
                        break;

                    case "Arc":
                        DrawArc(e, Color.Black, 2);
                        break;
                }
            }
        }

        private static void DrawLine(IElement e, Color color, int width)
        {
            var drawBrush = new SolidBrush(Color.Black);

            var drawPen = new Pen(drawBrush)
            {
                //Width = -1f
            };
            
            _g.DrawLine(drawPen, e.Start.X,e.Start.Y,e.End.X,e.End.Y);
        }

        private static void DrawLine(float sx, float sy, float ex, float ey, Color color, int width)
        {
            var drawBrush = new SolidBrush(Color.Black);

            var drawPen = new Pen(drawBrush)
            {
               // Width = -1f
            };
            
            _g.DrawLine(drawPen, sx,sy,ex,ey);
        }

        private static void DrawCircle(float cx, float cy, float r, Color color, int width)
        {
            var drawBrush = new SolidBrush(Color.Black);

            var drawPen = new Pen(drawBrush)
            {
                //Width = -1f
            };
            
            _g.DrawEllipse(drawPen, cx+r,cy+r,2*r,2*r);
        }

        private static void DrawArc(IElement e, Color color, int width)
        {
            var el = (Arc) e;

            var radius = el.Radius;
            var x = (float) (el.Center.X - radius);
            var y = (float) (el.Center.Y - radius);
            var num = (float) Math.Abs(radius * 2.0);
            var rect = new RectangleF(x, y, num, num);


            var startAngle = (float) el.StartAngle;
            var sweepAngle = (float) el.SweepAngle;
            var rectwidth = num;
            var rectheight = num;


            /*bool longArc = false;

            var start = new PointF();
            var end = new PointF();
            var center = new PointF(x + (rectwidth / 2f), y + (rectheight / 2f));

            startAngle = (startAngle / 360f) * 2f * (float) Math.PI;
            sweepAngle = (sweepAngle / 360f) * 2f * (float) Math.PI;

            sweepAngle += startAngle;

            if (sweepAngle > startAngle)
            {
                float tmp = startAngle;
                startAngle = sweepAngle;
                sweepAngle = tmp;
            }*/

            //if (sweepAngle - startAngle > Math.PI || startAngle - sweepAngle > Math.PI)
            //{
            //    longArc = true;
            //}

            //start.X = ((float) Math.Cos(startAngle) * (rectwidth / 2f)) + center.X;
            //start.Y = ((float) Math.Sin(startAngle) * (rectheight / 2f)) + center.Y;

            //end.X = ((float) Math.Cos(sweepAngle) * (rectwidth / 2f)) + center.X;
            //end.Y = ((float) Math.Sin(sweepAngle) * (rectheight / 2f)) + center.Y;

            //var arcSw = SvgArcSweep.Negative;//(el.Direction == "CW") ? SvgArcSweep.Positive : SvgArcSweep.Negative;
            //var arcType = (longArc) ? SvgArcSize.Large : SvgArcSize.Small;


            var drawBrush = new SolidBrush(Color.Black);

            var drawPen = new Pen(drawBrush)
            {
                //Width = -1f
            };
            
            _g.DrawArc(drawPen,rect,startAngle,sweepAngle);

            /*var a = new SvgPath();
            var pd = new SvgPathSegmentList();
            var arc = new SvgArcSegment(start, (float) radius, (float) radius,
                (float) ((startAngle - sweepAngle) * 180 / Math.PI), arcType, arcSw, end);
            pd.Add(arc);
            a.PathData = pd;

            a.Fill = SvgPaintServer.None;

            a.Stroke = new SvgColourServer(color);
            a.StrokeWidth = new SvgUnit(width);

            a.Transforms = _doc.Transforms;

            _g.Children.Add(a);*/
        }

        /*private static void DrawTextProfile(TextProfile t)
        {
            var text = new SvgText
            {
                Fill = new SvgColourServer(Color.Black),
                //Stroke = new SvgColourServer(Color.Black),
                //StrokeWidth = -1,
                Font = "Arial",
                FontWeight = SvgFontWeight.Normal,
                FontSize = Math.Abs(t.Height),
                Text = t.Text,
                Transforms = (SvgTransformCollection) _doc.Transforms.Clone()
            };

            var tr = new SvgTranslate(t.ReferencePoint.X, t.ReferencePoint.Y);
            text.Transforms.Add(tr);

            var m = new List<float>()
            {
                (float) t.Matrix33.m[0], 
                (float) t.Matrix33.m[1], 
                (float) t.Matrix33.m[3],
                (float) t.Matrix33.m[4], 
                0, 
                0
            };

            text.Transforms.Add(new SvgMatrix(m));

            var s = new SvgScale(1,-1);
            text.Transforms.Add(s);

            var r = new SvgRotate(-t.Angle);
            text.Transforms.Add(r);
            





            _g.Children.Add(text);

            /*var drawBrush = new SolidBrush(Color.Black);

            var drawPen = new Pen(drawBrush)
            {
                Width = -1f
            };

            TranslateTransform(t.ReferencePoint.X, t.ReferencePoint.Y);

            var path = new GraphicsPath();

            path.AddString(t.Text, FontFamily.GenericSansSerif, (int) FontStyle.Regular,
                t.Height, new PointF(0, -t.Height), StringFormat.GenericDefault);

            var matrix = new Matrix();
            matrix.Rotate(-t.Angle);
            path.Transform(matrix);
            
            matrix = new Matrix();
            matrix.Scale(1f, -1f);
            path.Transform(matrix);

            matrix = new Matrix((float) t.Matrix33.M[0, 0], (float) t.Matrix33.M[0, 1], (float) t.Matrix33.M[1, 0],
                 (float) t.Matrix33.M[1, 1], 0, 0);
            path.Transform(matrix);

            DrawPath(drawPen, path);
            FillPath(drawBrush, path);

            TranslateTransform(0 - t.ReferencePoint.X, 0 - t.ReferencePoint.Y)
        };*/

        private static void DrawTextProfile(TextProfile t)
        {
            var drawBrush = new SolidBrush(Color.Black);

            var drawPen = new Pen(drawBrush)
            {
                Width = -1f
            };

            _g.TranslateTransform(t.ReferencePoint.X, t.ReferencePoint.Y);

            var path = new GraphicsPath();

            path.AddString(t.Text, FontFamily.GenericSansSerif, (int) FontStyle.Regular,
                t.Height, new PointF(0, -t.Height), StringFormat.GenericDefault);

            var matrix = new Matrix();
            matrix.Rotate(-t.Angle);
            path.Transform(matrix);
            
            matrix = new Matrix();
            matrix.Scale(1f, -1f);
            path.Transform(matrix);

            matrix = new Matrix((float) t.Matrix33.m[0], (float) t.Matrix33.m[1], (float) t.Matrix33.m[3],
                 (float) t.Matrix33.m[4], 0, 0);
            path.Transform(matrix);

            _g.DrawPath(drawPen, path);
            _g.FillPath(drawBrush, path);

            _g.TranslateTransform(0 - t.ReferencePoint.X, 0 - t.ReferencePoint.Y);
        }

        private static void DrawSheet(Plate p)
        {
            foreach (var t in p.Profiles)
            {
                foreach (var e in t.Geometry)
                {
                    switch (e.GetType().Name)
                    {
                        case "Line":
                            DrawLine(e, Color.Black, 2);
                            break;

                        case "Arc":
                            DrawArc(e, Color.Black, 2);
                            break;
                    }
                }
            }

            //draw texts 
            // draw annotations 
        }

        private static void DrawOrigo(Point o)
        {
            if (o == null)
            {
                return;
            }
            const float size = 10.0f;
        
            DrawLine(o.X, o.Y - size / 2f, o.X, o.Y + size / 2f, Color.Black, 2);
            DrawLine(o.X - size / 2f, o.Y, o.X + size / 2f, o.Y, Color.Black, 2);
            DrawCircle(o.X, o.Y, size/2, Color.Black, 2);
        }
    }
}