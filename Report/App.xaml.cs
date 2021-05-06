using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Report.Forms;
using TextAlignment = iText.Layout.Properties.TextAlignment;
using VerticalAlignment = iText.Layout.Properties.VerticalAlignment;

namespace Report
{
    public partial class App //: System.Windows.Application
    {


        private void Application_Startup(object sender, StartupEventArgs e)
        {
			if (!e.Args.Any())
            {
                MessageBox.Show("no params", "error");
				return;
			}

            var pathIdsList = new List<List<string>>(); 
            
            foreach (var text in e.Args)
            {
                if (!text.StartsWith("-PARAMS="))
                {
                    continue;
                }
                
                var paramsList = text.TrimStart("-PARAMS=".ToCharArray()).Split(',').ToList();

                const int size = 1000;
                    
                for (int i = 0; i < paramsList.Count; i += size)
                {
                    var end = i + size;
                    if (end > paramsList.Count)
                    {
                        end = paramsList.Count;
                    }

                    pathIdsList.Add(paramsList.GetRange(i, end - i));
                }
            }
            
            var db = GetDbName(@"..\..\master\settings\nestix2.ini");
            
            if (string.IsNullOrWhiteSpace(db))
            {
                MessageBox.Show("db name not found", "error");
                return;
            }

            var connectionString = $"Data Source=BK-SSK-NESH01.CORP.LOCAL;Initial Catalog=NxSC_Zvezda_{db};Integrated Security=SSPI";

            var mdList = new List<Nest>();

            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            foreach (var pathIds in pathIdsList)
            {
                mdList.AddRange(Db.FillMasterData(sqlConnection, pathIds));
            }
            sqlConnection.Close();

            if (mdList.Count == 0)
            {
                MessageBox.Show("nc's not found", "error");
                return;
            }
            
            var all = new Dictionary<string, byte[]>();

            var launchWindow = new Launch();
            launchWindow.ShowDialog();

            foreach (var m in mdList)
            {
                var rep = new PdfReport();
                var bytes = rep.ProcessNest(m, launchWindow.LaunchString);
                all[m.NcName] = bytes;
            }

            if (all.Count == 0)
            {
                MessageBox.Show("Document has no pages.");
                return;
            }

            // sort by nc name asc
            var sorted = all.OrderBy(x => x.Key);

            #region merge
            using var mergedPdfStream = new MemoryStream();
            var writer = new PdfWriter(mergedPdfStream);
            var pdf = new PdfDocument(writer);

            foreach (var (_, bytes) in sorted)
            {
                var src = new PdfDocument(new PdfReader(new MemoryStream(bytes)));

                src.CopyPagesTo(1, src.GetNumberOfPages(), pdf);

                src.Close();
            }

            pdf.Close();

            var mergedBytes = mergedPdfStream.ToArray();
            #endregion

            #region apply stamp
            
            var ttf =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "consola.ttf");

            var fontProgram = FontProgramFactory.CreateFont(ttf);
            var font = PdfFontFactory.CreateFont(fontProgram, PdfEncodings.IDENTITY_H, true);
            
            using var outPdfStream = new MemoryStream();

            var pdfDoc = new PdfDocument(new PdfReader(new MemoryStream(mergedBytes)), new PdfWriter(outPdfStream));

            var doc = new Document(pdfDoc);

            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var pdfPage = pdfDoc.GetPage(i);
                pdfPage.SetIgnorePageRotationForContent(true);

                var pageSize = pdfPage.GetPageSize();

                float x1, x2, x3, y1, y2, y3;

                if (pdfPage.GetRotation() % 180 == 0)
                {
                    x1 = pageSize.GetLeft() + 30;
                    y1 = pageSize.GetBottom() + 15;

                    x2 = pageSize.GetWidth() / 2;
                    y2 = pageSize.GetBottom() + 15;

                    x3 = pageSize.GetRight() - 20;
                    y3 = pageSize.GetBottom() + 15;
                }
                else
                {
                    x1 = pageSize.GetBottom() + 20;
                    y1 = pageSize.GetLeft() + 15;

                    x2 = pageSize.GetHeight() / 2;
                    y2 = pageSize.GetLeft() + 15;

                    x3 = pageSize.GetTop() - 30;
                    y3 = pageSize.GetLeft() + 15;
                }

                var footer1 = new Paragraph($"{i}/{pdfDoc.GetNumberOfPages()}");
                footer1.SetFont(font);
                footer1.SetFontSize(8);
                doc.ShowTextAligned(footer1, x1, y1, i, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);

                var footer2 = new Paragraph(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                footer2.SetFont(font);
                footer2.SetFontSize(8);
                doc.ShowTextAligned(footer2, x2, y2, i, TextAlignment.CENTER, VerticalAlignment.BOTTOM, 0);

                var footer3 = new Paragraph("A3 (420x297 mm)");
                footer3.SetFont(font);
                footer3.SetFontSize(8);
                doc.ShowTextAligned(footer3, x3, y3, i, TextAlignment.RIGHT, VerticalAlignment.BOTTOM, 0);
            }

            doc.Close();
            pdfDoc.Close();
            #endregion

            string file;

            if (all.Keys.Count > 1)
            {
                var spl = all.Keys.ToArray()[0].Split("-");

                file = spl.Length == 4 ? $"{spl[1]}-{spl[2]}" : "merged";
            }
            else
            {
                file = all.Keys.ToArray()[0];
            }

            file += ".pdf";

            var o = outPdfStream.ToArray();

            #region save result
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //var resultFilename = Path.Join(folder, file);
            var resultFilename = Path.Join(folder, "Карты раскроя");

            if (!Directory.Exists(resultFilename))
            {
                Directory.CreateDirectory(resultFilename);
            }
            
            resultFilename = Path.Join(resultFilename, file);



            File.WriteAllBytes(resultFilename, o);
            #endregion
            
            #region open pdf
            var psi = new ProcessStartInfo(resultFilename)
            {
                UseShellExecute = true
            };
            Process.Start(psi);
            #endregion
        }

        private static string GetDbName(string iniPath)
        {
            var db = "";

            if (!File.Exists(iniPath))
            {
                //log.AddLine("ini file doesn't exists");
                return db;
            }
            

            var ini = File.ReadAllLines(iniPath);
            foreach (var l in ini)
            {
                if (!l.StartsWith("DataSource"))
                {
                    continue;
                }
                var l2 = l.TrimStart("DataSource=".ToCharArray());
                var lres = l2.Split(";");
                foreach (var s in lres)
                {
                    if (!s.StartsWith("DATABASE"))
                    {
                        continue;
                    }
                    db = s.Split("=")[1].TrimStart("NxSC_Zvezda_".ToCharArray());
                    return db;
                }
            }
            
            return db;
        }
    }
}
