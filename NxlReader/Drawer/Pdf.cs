using System;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

namespace NxlReader.Drawer
{
    public static class Pdf
    {
        private static PdfCanvas _c;

        public static void Draw(PdfCanvas c, Rectangle rect, Nest n)
        {
            _c = c;

            var bb = n.GetBBox();

            var w = rect.GetWidth() / bb.Width;
            var h = rect.GetHeight() / bb.Height;
            var scale = Math.Min(w, h);

            var tr = new AffineTransform();
            tr.Translate(rect.GetX(), rect.GetHeight() / 2);
            tr.Scale(scale, scale);
            _c.ConcatMatrix(tr);

            DrawSheet(n.Plate);

            foreach (var r in n.OriginalRemnants)
            {
                DrawRemnant(r);
            }

            foreach (var p in n.Parts)
            {
                foreach (var t in p.Texts)
                {
                    DrawTextProfile(t);
                }

                foreach (var pr in p.Profiles)
                {
                    foreach (var g in pr.Geometry)
                    {
                        g.Start = p.Matrix.TransformPoint(g.Start);
                        g.End = p.Matrix.TransformPoint(g.End);
                        g.Center = p.Matrix.TransformPoint(g.Center);
                    }

                    DrawProfile(pr);
                }

                DrawTextProfile(p.DetailId);
            }

            foreach (var text in n.Texts)
            {
                DrawTextProfile(text);
            }

            foreach (var e in n.DimensionLineAnnotations)
            {
                _c.MoveTo(e.Start.X, e.Start.Y);
                _c.LineTo(e.End.X, e.End.Y);
                _c.SetLineWidth(2);
                _c.FillStroke();

                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.COURIER);

                _c.BeginText();
                _c.SetFontAndSize(font, e.FontSize);

                var m = new AffineTransform();
                if (e.TextLocation != null)
                {
                    m.Translate(e.TextLocation.X, e.TextLocation.Y);
                }

                m.Rotate(e.GetAngle());

                _c.SetTextMatrix(m);

                _c.ShowText(e.GetLength());
                _c.EndText();
            }
        }

        private static void DrawProfile(Profile p)
        {
            foreach (var e in p.Geometry)
            {
                switch (e.GetType().Name)
                {
                    case "Line":
                        DrawLine(e);
                        break;

                    case "Arc":
                        DrawArc(e);
                        break;
                }
            }
        }

        private static void DrawLine(IElement e)
        {
            _c.MoveTo(e.Start.X, e.Start.Y);
            _c.LineTo(e.End.X, e.End.Y);
            _c.SetLineWidth(2);
            _c.FillStroke();
        }

        private static void DrawArc(IElement e)
        {
            var el = (Arc)e;

            var x = el.Center.X - el.Radius;
            var y = el.Center.Y - el.Radius;
            var num = Math.Abs(el.Radius * 2.0);

            _c.Arc(x, y, x + num, y + num, el.StartAngle, el.SweepAngle);
            _c.SetLineWidth(2);
            _c.Stroke();
        }

        private static void DrawTextProfile(TextProfile t)
        {
            if (string.IsNullOrEmpty(t.Text))
            {
                return;
            }

            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.COURIER);

            _c.BeginText();
            _c.SetFontAndSize(font, t.Height);

            var m = new AffineTransform();
            m.SetTransform(t.Matrix.M[0], t.Matrix.M[1], t.Matrix.M[3], t.Matrix.M[4], t.ReferencePoint.X,
                t.ReferencePoint.Y);
            m.Rotate(t.Angle * Math.PI / 180);
            _c.SetTextMatrix(m);

            //var radians = Math.Atan2(m.GetShearY(), m.GetScaleX());
            //var degrees = radians * 180 / Math.PI;


            _c.ShowText(t.Text);
            _c.EndText();

            /*var path = new GraphicsPath();

            path.AddString(t.Text, FontFamily.GenericSansSerif, (int) FontStyle.Regular,
                t.Height, new PointF(0, -t.Height), StringFormat.GenericDefault);

            var matrix = new Matrix();
            matrix.Rotate(-t.Angle);
            path.Transform(matrix);
            
            matrix = new Matrix();
            matrix.Scale(1f, -1f);
            path.Transform(matrix);

            matrix = new Matrix((float) t.Matrix.m[0], (float) t.Matrix.m[1], (float) t.Matrix.m[3],
                 (float) t.Matrix.m[4], 0, 0);
            path.Transform(matrix);*/
        }

        private static void DrawSheet(Plate p)
        {
            foreach (var t in p.Profiles)
            {
                DrawProfile(t);
            }

            foreach (var text in p.Texts)
            {
                DrawTextProfile(text);
            }
        }

        private static void DrawRemnant(Remnant r)
        {
            foreach (var t in r.Profiles)
            {
                DrawProfile(t);
            }

            foreach (var text in r.Texts)
            {
                DrawTextProfile(text);
            }
        }
    }
}