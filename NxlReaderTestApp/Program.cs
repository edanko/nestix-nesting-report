using System;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using NxlReader;
using NxlReader.Drawer;
using Path = System.IO.Path;

namespace NxlReaderTestApp
{
    static class Program
    {
        static void Main()
        {
            // TODO: how to get files from project root?
            var files = Directory.GetFiles(@"..\\..\\..\\..\\_test_nxl");

            foreach (var file in files)
            {
                var n = new Nest();
                n.Read(file);

                var fs = new MemoryStream();

                var writer = new PdfWriter(fs);
                var pdf = new PdfDocument(writer);
                var doc = new Document(pdf, PageSize.A2.Rotate());

                var page = pdf.AddNewPage();
                var c = new PdfCanvas(page);

                var rect = new Rectangle(100, 100, 900, 500);
                c.Rectangle(rect);
                c.Stroke();

                Pdf.Draw(c, rect, n);

                var folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var resultFilename = Path.Combine(folder, "Карты раскроя");

                if (!Directory.Exists(resultFilename))
                {
                    Directory.CreateDirectory(resultFilename);
                }

                resultFilename = Path.Combine(resultFilename,
                    Path.GetFileName(file).TrimEnd(".nest.nxl".ToCharArray()) + ".pdf");

                doc.Close();
                pdf.Close();
                writer.Close();

                File.WriteAllBytes(resultFilename, fs.ToArray());

                var svg = Svg.Draw(n);
                File.WriteAllText(resultFilename.Replace(".pdf", ".svg"), svg);
            }
        }
    }
}