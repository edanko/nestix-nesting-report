using System;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using Path = System.IO.Path;

namespace Report.NxlReader.Drawer
{
    public class Pdf
    {
        private readonly string _consolas =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "consola.ttf");

        private PdfCanvas _c;

        public void Draw(PdfCanvas c, Rectangle rect, Nest n)
        {
            _c = c;

            var bb = n.GetBBox();

            var w = rect.GetWidth() / bb.Width;
            var h = rect.GetHeight() / bb.Height;
            var scale = Math.Min(w, h);

            var tr = new AffineTransform();

            var trX = -bb.X * scale + rect.GetX() + (rect.GetWidth() / 2 - bb.Width * scale / 2);
            var trY = -bb.Y * scale + rect.GetY() + (rect.GetHeight() / 2 - bb.Height * scale / 2);

            tr.Translate(trX, trY);
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

            /*foreach (var e in n.DimensionLineAnnotations)
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
            }*/
        }

        private void DrawProfile(Profile p)
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

        private void DrawLine(IElement e)
        {
            _c.MoveTo(e.Start.X, e.Start.Y);
            _c.LineTo(e.End.X, e.End.Y);
            _c.SetLineWidth(2);
            _c.FillStroke();
        }

        private void DrawArc(IElement e)
        {
            var el = (Arc) e;

            var x = el.Center.X - el.Radius;
            var y = el.Center.Y - el.Radius;
            var num = Math.Abs(el.Radius * 2.0);

            var startAngle = el.StartAngle;
            var sweepAngle = el.SweepAngle;

            if (Math.Abs(sweepAngle) < 3 || el.Radius > 10000)
            {
                _c.MoveTo(e.Start.X, e.Start.Y);
                _c.LineTo(e.End.X, e.End.Y);
                _c.SetLineWidth(2);
                _c.FillStroke();

                return;
            }

            _c.Arc(x, y, x + num, y + num, startAngle, sweepAngle);
            _c.SetLineWidth(2);
            _c.Stroke();
        }

        private void DrawTextProfile(TextProfile t)
        {
            if (string.IsNullOrEmpty(t.Text))
            {
                return;
            }

            var fontProgram = FontProgramFactory.CreateFont(_consolas);
            var font = PdfFontFactory.CreateFont(fontProgram, PdfEncodings.IDENTITY_H,
                PdfFontFactory.EmbeddingStrategy.PREFER_NOT_EMBEDDED);

            var lines = t.Text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                _c.BeginText();
                _c.SetFontAndSize(font, t.Height);

                var m = new AffineTransform();
                m.SetTransform(t.Matrix.M[0], t.Matrix.M[1], t.Matrix.M[3], t.Matrix.M[4], t.ReferencePoint.X,
                    t.ReferencePoint.Y - i * (t.Height + 2));
                m.Rotate(t.Angle * Math.PI / 180);
                _c.SetTextMatrix(m);

                _c.ShowText(line);
                _c.EndText();
            }
        }

        private void DrawSheet(Plate p)
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

        private void DrawRemnant(Remnant r)
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