namespace NxlReader
{
    public class NxlReader
    {
        /*
            Image bmp = new Bitmap(12000, 3600);
            using (var g = Graphics.FromImage(bmp))
            {

                g.Clear(Color.White);

                g.ScaleTransform(1.0f, -1.0f);
                g.TranslateTransform(0, -3600);

                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                foreach (var e in nest.Plate.Profiles.SelectMany(t => t.Geometry))
                {
                    g.DrawLine(new Pen(Color.Black, 2), e.Start.X, e.Start.Y, e.End.X, e.End.Y);
                }

                //var part = nest.Parts[0];

                foreach (Part op in nest.OriginalParts)
                {
                    foreach (var pr in op.Elements.Cast<Profile>())
                    {
                        if (pr.MachiningMode != "0")
                        {
                            continue;
                        }

                        foreach (Element e in pr.Geometry)
                        {
                            switch (e.GetType().Name)
                            {
                                case "Line":

                                    var ell = (Line) e;
                                    g.DrawLine(new Pen(Color.Black, 2), e.Start.X, e.Start.Y, e.End.X, e.End.Y);
                                    break;
                                case "Arc":

                                    var el = (Arc) e;

                                    var radius = el.Radius;
                                    var x = (float) (el.Center.X - radius);
                                    var y = (float) (el.Center.Y - radius);
                                    var num = (float) Math.Abs(radius * 2.0);
                                    var rect = new RectangleF(x, y, num, num);

                                    g.DrawArc(new Pen(Color.Black, 2), rect, (float) el.StartAngle,
                                        (float) el.SweepAngle);

                                    //g.DrawLine(new Pen(Color.Black,2), e.Start.X, e.Start.Y, e.End.X, e.End.Y);

                                    break;
                            }
                        }
                    }
                }

                foreach (var p in nest.Parts)
                {
                    var id = p.DetailId;

                    var drawFont = new Font("Arial", float.Parse(id.Height, CultureInfo.InvariantCulture));
                    var drawBrush = new SolidBrush(Color.Black);

                    //GraphicsState state = g.Save();
                    //g.ResetTransform();

                    //g.RotateTransform(180);
                    //g.TranslateTransform(id.ReferencePoint.X, id.ReferencePoint.Y, MatrixOrder.Append);

                    g.DrawString(id.Text, drawFont, drawBrush, id.ReferencePoint.X, id.ReferencePoint.Y);

                    //g.Restore(state);
                }


                foreach (var op in nest.OriginalParts)
                {
                    foreach (var t in op.Texts)
                    {
                        var drawFont = new Font("Arial", float.Parse(t.Height, CultureInfo.InvariantCulture));
                        var drawBrush = new SolidBrush(Color.Black);
                        GraphicsState state = g.Save();
                        g.ResetTransform();

                        g.ScaleTransform(1.0f, -1.0f);
                        //g.TranslateTransform(0, -3600);

                        g.RotateTransform(float.Parse(t.Angle, CultureInfo.InvariantCulture) * (float)Math.PI/180);
                        g.TranslateTransform(t.ReferencePoint.X, 3600-t.ReferencePoint.Y , MatrixOrder.Append);

                        g.DrawString(t.Text, drawFont, drawBrush, 0, 0);

                        g.Restore(state);
                    }
                }
            }

            //var res = ResizeImage(bmp, 12000 / 3, 3600 / 3);

            bmp.Save("test.png", ImageFormat.Png);
        }


        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using var graphics = Graphics.FromImage(destImage);
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using var wrapMode = new ImageAttributes();
            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

            return destImage;
        }*/
    }
}
