using System;
using System.Globalization;
using System.IO;
using System.Linq;
using iText.Barcodes;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Report.NxlReader.Drawer;
using HorizontalAlignment = iText.Layout.Properties.HorizontalAlignment;
using Path = System.IO.Path;
using TextAlignment = iText.Layout.Properties.TextAlignment;
using VerticalAlignment = iText.Layout.Properties.VerticalAlignment;

namespace Report
{
    internal class PdfReport
    {
        private int _firstPagePartsCount = 33;
        private const int DefaultSecondPagePartsPerColumn = 39;
        private const int DefaultCellHeight = 16;

        private readonly string _consolas =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "consola.ttf");

        private FontProgram _fontProgram;
        private PdfFont _font;

        private Cell TextCell(string text, bool bold = false, bool borders = true, int rowspan = 1, int colspan = 1,
            int fontSize = 10)
        {
            var c = new Cell(rowspan, colspan);

            if (!borders)
            {
                c.SetBorder(Border.NO_BORDER);
            }

            var p = new Paragraph(text);

            p.SetTextAlignment(TextAlignment.CENTER);

            p.SetFont(_font);
            p.SetFontSize(fontSize);

            p.SetHeight(rowspan * DefaultCellHeight);

            if (bold)
            {
                p.SetBold();
            }

            c.Add(p);
            return c;
        }

        public byte[] ProcessNest(Nest d, string launch)
        {
            _fontProgram = FontProgramFactory.CreateFont(_consolas);
            _font = PdfFontFactory.CreateFont(_fontProgram, PdfEncodings.IDENTITY_H,
                PdfFontFactory.EmbeddingStrategy.PREFER_NOT_EMBEDDED);

            var fs = new MemoryStream();

            var writer = new PdfWriter(fs);
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf, PageSize.A3.Rotate());
            doc.SetMargins(15, 15, 14, 15);

            const string masterPath = @"..\..\master";

            var nest = new NxlReader.Nest();
            nest.Read(Path.Combine(masterPath, d.NxlFile));

            #region header table

            var table = new Table(UnitValue.CreatePercentArray(new[]
            {
                0.6f,
                0.85f,
                0.8f,
                0.9f,
                0.61f,
                0.4f,
                1f, // gas
                0.2f, // spacing
                1.8f
            }));

            table.SetHorizontalAlignment(HorizontalAlignment.LEFT);
            table.SetWidth(UnitValue.CreatePercentValue(100));
            table.SetFixedLayout();

            // first row
            table.AddCell(TextCell("Заказ", true));
            table.AddCell(TextCell("Секция", true));
            table.AddCell(TextCell("Заготовка", true, colspan: 4));

            var gasCell = new Cell(4, 1);
            gasCell.SetBorder(Border.NO_BORDER);
            gasCell.SetPadding(0);
            gasCell.SetPaddingLeft(3);


            var gasInnerTable = new Table(UnitValue.CreatePercentArray(new[]
            {
                1f,
                0.9f
            }));

            gasInnerTable.SetHorizontalAlignment(HorizontalAlignment.LEFT);
            gasInnerTable.SetWidth(UnitValue.CreatePercentValue(100));
            gasInnerTable.SetFixedLayout();

            gasInnerTable.AddCell(TextCell("Расход газов", true, colspan: 2));

            var g = GasAmount.GetGas(d.Machine, d.Plate.Quality, d.PartsWeight());

            if (g.Oxygen > 0)
            {
                gasInnerTable.AddCell(TextCell("Кислород, кг"));
                gasInnerTable.AddCell(TextCell(g.Oxygen.ToString("F3", CultureInfo.InvariantCulture).TrimEnd('0')));
            }

            if (g.Propan > 0)
            {
                gasInnerTable.AddCell(TextCell("Пропан, кг"));
                gasInnerTable.AddCell(TextCell(g.Propan.ToString("F3", CultureInfo.InvariantCulture).TrimEnd('0')));
            }

            if (g.Nitrogen > 0)
            {
                gasInnerTable.AddCell(TextCell("Азот, кг"));
                gasInnerTable.AddCell(
                    TextCell(g.Nitrogen.ToString("F3", CultureInfo.InvariantCulture).TrimEnd('0')));
            }

            if (g.LaserMix > 0)
            {
                gasInnerTable.AddCell(TextCell("Смесь, м3")); //"CO2 5% He 60% N2 35%, м3"
                gasInnerTable.AddCell(
                    TextCell(g.LaserMix.ToString("F5", CultureInfo.InvariantCulture).TrimEnd('0')));
            }

            gasCell.Add(gasInnerTable);


            table.AddCell(gasCell);

            table.AddCell(TextCell("", false, false));

            #region barcode

            var orderBarCode = launch.Split('.');

            var codeCell = new Cell(3, 1);
            codeCell.SetBorder(Border.NO_BORDER);

            var code = new Barcode128(pdf, _font);
            code.SetCode(orderBarCode.Length > 3 ? $"{orderBarCode[0]}|{d.NcName}" : d.NcName);
            code.SetSize(10);

            var barImage = new Image(code.CreateFormXObject(pdf));
            barImage.SetHorizontalAlignment(HorizontalAlignment.RIGHT);

            //barImage.SetMaxHeight(3 * DefaultCellHeight);
            barImage.ScaleToFit(250, 3 * DefaultCellHeight);
            //barImage.SetAutoScale(true);


            codeCell.Add(barImage);

            table.AddCell(codeCell);

            #endregion

            // second row
            var orders = d.Parts.Where(x => !string.IsNullOrWhiteSpace(x.Project)).Select(x => x.Project).Distinct();
            var order = orders.Count() > 1 ? "Несколько" : d.Parts[0].Project;

            var sections = d.Parts.Where(x => !string.IsNullOrWhiteSpace(x.Section)).Select(x => x.Section).Distinct()
                .ToList();

            string section;
            switch (sections.Count)
            {
                case 2:
                    section = $"{sections[0]}, {sections[1]}";
                    break;
                case 1:
                    section = $"{sections[0]}";
                    break;
                default:
                    section = "Несколько";
                    break;
            }

            table.AddCell(TextCell(order));
            table.AddCell(TextCell(section));

            table.AddCell(TextCell("Идентификац. №", true));

            var infoText = d.Info;

            var spl = infoText.Split(';');

            if (spl.Length == 2)
            {
                infoText = spl[1].Trim();
            }

            table.AddCell(TextCell(infoText));

            table.AddCell(TextCell("Марка", true));
            table.AddCell(TextCell(d.Plate.Quality.Replace("Grade ", "")));

            table.AddCell(TextCell("", borders: false));


            // third row
            table.AddCell(TextCell("УП", true));
            table.AddCell(TextCell(d.NcName));
            table.AddCell(TextCell("Габарит (ТхДхШ), мм", true));
            table.AddCell(TextCell($"{d.Plate.Thickness} x {d.Plate.Length:F0} x {d.Plate.Width:F0}"));

            table.AddCell(TextCell("Коэф. раскроя, %", true));
            table.AddCell(TextCell($"{d.Used:F1}"));

            table.AddCell(TextCell("", borders: false));

            // fourth row
            table.AddCell(TextCell("МТР (Технология)", true));

            string tech;
            switch (d.Machine)
            {
                case "PlasmaBevelOmniMatL8000":
                    tech = "OM8000 (Plasma)";
                    break;
                case "GasBevelOmniMatL8000":
                    tech = "OM8000 (Gas)";
                    break;
                case "LaserMat4200":
                    tech = "LM4200";
                    break;
                case "GasOmniMatL7000":
                    tech = "OM7000 (Gas)";
                    break;
                default:
                    tech = d.Machine;
                    break;
            }

            if (d.Machine == "PlasmaBevelOmniMatL8000" && nest.Machine != null)
            {
                var t = nest.Machine.Technology.Split('-')[2];

                tech = $"OM8000 (Plasma {t})";
            }

            table.AddCell(TextCell(tech));

            table.AddCell(TextCell("Масса мат. / дет. кг", true));
            table.AddCell(
                //TextCell($"{(d.MatWeight == 0.0 ? d.Plate.NestGrossWeight : d.MatWeight):F1} / {d.PartsWeight:F1}"));
                TextCell($"{(d.Plate.NestGrossWeight):F1} / {d.PartsWeight():F1}"));


            table.AddCell(TextCell("Масса ДМО, кг", true));
            table.AddCell(TextCell($"{d.RemnantWeight:F1}"));


            table.AddCell(TextCell("", borders: false));


            var launchCell = new Cell(1, 1);

            launchCell.SetBorder(Border.NO_BORDER);

            var lp = new Paragraph($"Альбом КР: {launch}");

            lp.SetTextAlignment(TextAlignment.RIGHT);

            lp.SetFont(_font);
            lp.SetFontSize(12);

            lp.SetHeight(DefaultCellHeight);

            lp.SetBold();

            launchCell.Add(lp);

            table.AddCell(launchCell);

            doc.Add(table);

            #endregion

            #region main content

            table = new Table(UnitValue.CreatePercentArray(new[] { 21f, 79f }));
            table.SetWidth(UnitValue.CreatePercentValue(100));
            //table.SetFixedLayout();
            table.SetHorizontalAlignment(HorizontalAlignment.LEFT);

            #region tools

            if (d.Tools.Sum(x => x.TotalTimeMin) == 0)
            {
                var toolsCell = new Cell();
                toolsCell.SetBorder(Border.NO_BORDER);
                toolsCell.SetPadding(0);

                table.AddCell(toolsCell);
            }
            else
            {
                var toolsTable = new Table(UnitValue.CreatePercentArray(new[] { 3f, 2f, 2f }));
                toolsTable.SetWidth(UnitValue.CreatePercentValue(100));
                //toolsTable.SetFixedLayout();

                toolsTable.SetMargin(0);

                toolsTable.SetHorizontalAlignment(HorizontalAlignment.LEFT);

                toolsTable.AddCell(TextCell("Инструменты", true, colspan: 3));

                toolsTable.AddCell(TextCell("Тип", true));
                toolsTable.AddCell(TextCell("Сколько", true));
                toolsTable.AddCell(TextCell("Время", true));

                _firstPagePartsCount -= 2;

                if (nest.BridgesCount > 0)
                {
                    toolsTable.AddCell(TextCell("Мостики"));
                    toolsTable.AddCell(TextCell($"{nest.BridgesCount}"));
                    toolsTable.AddCell(TextCell("-"));

                    _firstPagePartsCount--;
                }

                if (nest.RidgesCount > 0)
                {
                    toolsTable.AddCell(TextCell("Перемычки"));
                    toolsTable.AddCell(TextCell($"{nest.RidgesCount}"));
                    toolsTable.AddCell(TextCell("-"));

                    _firstPagePartsCount--;
                }

                var cut = (from t in d.Tools
                    where t.ToolName.StartsWith("Basic") || t.ToolName.StartsWith("Bevel") ||
                          t.ToolName.StartsWith("Основная") || t.ToolName.StartsWith("Фаска")
                    select t).ToList();

                if (cut.Count > 0)
                {
                    toolsTable.AddCell(TextCell("Пробивки"));

                    toolsTable.AddCell(TextCell($"{cut.Sum(x => x.StartCount)}"));
                    toolsTable.AddCell(TextCell("-"));

                    _firstPagePartsCount--;
                }

                var markingText = (from t in d.Tools
                    where t.ToolName.StartsWith("Marking text") || t.ToolName.StartsWith("Текстовая")
                    select t).ToList();

                if (markingText.Count > 0)
                {
                    toolsTable.AddCell(TextCell("Маркировка"));
                    toolsTable.AddCell(TextCell($"{nest.TextSymbolsCount} симв"));

                    toolsTable.AddCell(TextCell($"{markingText.Sum(x => x.TotalTimeMin):F1} мин"));

                    _firstPagePartsCount--;
                }

                var markingLine =
                    (from t in d.Tools
                        where t.ToolName.StartsWith("Marking line") || t.ToolName.StartsWith("Линейная")
                        select t).ToList();

                if (markingLine.Count > 0)
                {
                    toolsTable.AddCell(TextCell("Разметка"));
                    toolsTable.AddCell(TextCell($"{markingLine.Sum(x => x.DistanceM):F1} м"));
                    toolsTable.AddCell(TextCell($"{markingLine.Sum(x => x.TotalTimeMin):F1} мин"));

                    _firstPagePartsCount--;
                }

                if (cut.Count > 0)
                {
                    toolsTable.AddCell(TextCell("Основная резка"));

                    toolsTable.AddCell(TextCell($"{cut.Sum(x => x.DistanceM):F1} м"));
                    toolsTable.AddCell(TextCell($"{cut.Sum(x => x.TotalTimeMin):F1} мин"));

                    _firstPagePartsCount--;
                }

                var rapid = (from t in d.Tools where t.ToolName.StartsWith("Rapid") select t).ToList();

                if (rapid.Count > 0)
                {
                    toolsTable.AddCell(TextCell("Холостой ход"));
                    toolsTable.AddCell(TextCell($"{rapid.Sum(x => x.DistanceM):F1} м"));
                    toolsTable.AddCell(TextCell($"{rapid.Sum(x => x.TotalTimeMin):F1} мин"));

                    _firstPagePartsCount--;
                }

                toolsTable.AddCell(TextCell(""));
                toolsTable.AddCell(TextCell("Итого, мин", true));
                toolsTable.AddCell(TextCell($"{d.Tools.Sum(x => x.TotalTimeMin):F1}"));

                _firstPagePartsCount--;


                var toolsCell = new Cell();
                toolsCell.SetBorder(Border.NO_BORDER);
                toolsCell.SetPadding(0);
                toolsCell.Add(toolsTable);

                table.AddCell(toolsCell);
            }

            #endregion

            #region image

            var imgCell = new Cell(2, 1);
            imgCell.SetMargin(5);
            imgCell.SetBorder(Border.NO_BORDER);
            imgCell.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            imgCell.SetVerticalAlignment(VerticalAlignment.MIDDLE);


#if USE_EMF
            var imgPath = Path.Combine(masterPath, d.EmfImage);
            var mf = new Metafile(imgPath);
            Image img1;

            var wmfBytes = GetMetafileRawData(mf);
            if (wmfBytes.Length != 22)
            {
                var img = new PdfFormXObject(new WmfImageData(wmfBytes), pdf);
                img1 = new Image(img);
            }
            else
            {
                try
                {
                    mf = new Metafile(imgPath);

                    var thumb = mf.GetThumbnailImage(mf.Width / 5, mf.Height / 5, null, IntPtr.Zero);
                    var imageStream = new MemoryStream();
                    thumb.Save(imageStream, ImageFormat.Png);
                    imageStream.Position = 0;

                    img1 = new Image(ImageDataFactory.Create(imageStream.ToArray()));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, d.NcName);
                    return null;
                }
            }

            //var img1 = new Image(img);

            img1.SetBorder(Border.NO_BORDER);
            img1.ScaleToFit(900, 610);
            imgCell.Add(img1);
#endif

            table.AddCell(imgCell);

            #endregion

            #region part list

            var partsTable = new Table(UnitValue.CreatePercentArray(new[] { 0.6f, 2.5f, 0.6f, 1, 2.2f }));
            partsTable.SetWidth(UnitValue.CreatePercentValue(100));
            partsTable.SetHorizontalAlignment(HorizontalAlignment.LEFT);

            partsTable.AddCell(TextCell("Список деталей", true, colspan: 6));

            partsTable.AddCell(TextCell("#", true, fontSize: 8));
            partsTable.AddCell(TextCell("Наименование", true, fontSize: 8));
            partsTable.AddCell(TextCell("Q", true, fontSize: 8));
            partsTable.AddCell(TextCell("m, кг", true, fontSize: 8));
            partsTable.AddCell(TextCell("LxW, мм", true, fontSize: 8));

            for (var i = 0; i < _firstPagePartsCount; i++)
            {
                if (i < d.Parts.Count)
                {
                    var p = d.Parts[i];

                    partsTable.AddCell(TextCell(p.DetailCode.ToString(), fontSize: 8));

                    string pos = "";

                    // 16 - visible symbols count
                    if (p.Section.Length + p.Pos.Length + 1 > 16)
                    {
                        pos = p.Pos;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(p.Section))
                        {
                            pos += p.Section;
                        }

                        if (!string.IsNullOrWhiteSpace(p.Pos))
                        {
                            if (!string.IsNullOrWhiteSpace(p.Section))
                            {
                                pos += "-";
                            }

                            pos += p.Pos;
                        }
                    }

                    partsTable.AddCell(TextCell(pos, fontSize: 8));

                    partsTable.AddCell(TextCell(p.DetailCount.ToString(), fontSize: 8));
                    partsTable.AddCell(TextCell($"{p.Weight:F1}", fontSize: 8));
                    partsTable.AddCell(TextCell($"{p.Length:F1} x {p.Width:F1}", fontSize: 8));
                }
                else
                {
                    partsTable.AddCell(TextCell(""));
                    partsTable.AddCell(TextCell(""));
                    partsTable.AddCell(TextCell(""));
                    partsTable.AddCell(TextCell(""));
                    partsTable.AddCell(TextCell(""));
                }
            }

            var partsCell = new Cell();
            partsCell.SetBorder(Border.NO_BORDER);
            partsCell.SetPadding(0);
            partsCell.Add(partsTable);

            table.AddCell(partsCell);

            #endregion

            doc.Add(table);

#if !USE_EMF
            var page = pdf.GetPage(1);
            var canvas = new PdfCanvas(page);

            var rect = new Rectangle(270, 36, 910, 700);
            //canvas.Rectangle(rect);
            //canvas.Stroke();

            var dr = new Pdf();
            dr.Draw(canvas, rect, nest);
#endif

            #endregion

            #region parts page

            if (d.Parts.Count > _firstPagePartsCount)
            {
                var remainParts = d.Parts.Skip(_firstPagePartsCount).ToList();

                while (true)
                {
                    pdf.AddNewPage();
                    var columnsPopulated = 0;

                    table = new Table(UnitValue.CreatePercentArray(new[] { 1f, 1f, 1f, 1f }));
                    table.SetWidth(UnitValue.CreatePercentValue(100));
                    table.SetFixedLayout();

                    for (int i = 0; i < 4; i++)
                    {
                        var p2 = new Table(UnitValue.CreatePercentArray(new[] { 0.6f, 2.5f, 0.6f, 1, 2.2f }));

                        p2.SetWidth(UnitValue.CreatePercentValue(100));

                        p2.SetHorizontalAlignment(HorizontalAlignment.LEFT);

                        int counter = 0;

                        if (i == 0)
                        {
                            p2.AddCell(TextCell("Список деталей", true, colspan: 6));

                            p2.AddCell(TextCell("#", true, fontSize: 8));
                            p2.AddCell(TextCell("Наименование", true, fontSize: 8));
                            p2.AddCell(TextCell("Q", true, fontSize: 8));
                            p2.AddCell(TextCell("m, кг", true, fontSize: 8));
                            p2.AddCell(TextCell("LxW, мм", true, fontSize: 8));

                            counter += 2;
                        }

                        foreach (var p in remainParts)
                        {
                            int remainRowsCount = DefaultSecondPagePartsPerColumn - counter;

                            if (remainRowsCount > 0)
                            {
                                if (p == remainParts[remainParts.Count - 1])
                                {
                                    // last part
                                    p2.AddCell(TextCell(p.DetailCode.ToString(), fontSize: 8));

                                    string pos = "";

                                    if (!string.IsNullOrWhiteSpace(p.Section))
                                    {
                                        pos += p.Section;
                                    }

                                    if (!string.IsNullOrWhiteSpace(p.Pos))
                                    {
                                        if (!string.IsNullOrWhiteSpace(p.Section))
                                        {
                                            pos += "-";
                                        }

                                        pos += p.Pos;
                                    }

                                    p2.AddCell(TextCell(pos, fontSize: 8));


                                    p2.AddCell(TextCell(p.DetailCount.ToString(), fontSize: 8));
                                    p2.AddCell(TextCell($"{p.Weight:F1}", fontSize: 8));
                                    p2.AddCell(TextCell($"{p.Length:F1} x {p.Width:F1}", fontSize: 8));

                                    for (int j = 0; j < remainRowsCount - 1; j++)
                                    {
                                        p2.AddCell(TextCell(""));
                                        p2.AddCell(TextCell(""));
                                        p2.AddCell(TextCell(""));
                                        p2.AddCell(TextCell(""));
                                        p2.AddCell(TextCell(""));
                                    }
                                }
                                else
                                {
                                    p2.AddCell(TextCell(p.DetailCode.ToString(), fontSize: 8));

                                    string pos = "";

                                    if (!string.IsNullOrWhiteSpace(p.Section))
                                    {
                                        pos += p.Section;
                                    }

                                    if (!string.IsNullOrWhiteSpace(p.Pos))
                                    {
                                        if (!string.IsNullOrWhiteSpace(p.Section))
                                        {
                                            pos += "-";
                                        }

                                        pos += p.Pos;
                                    }

                                    p2.AddCell(TextCell(pos, fontSize: 8));

                                    p2.AddCell(TextCell(p.DetailCount.ToString(), fontSize: 8));
                                    p2.AddCell(TextCell($"{p.Weight:F1}", fontSize: 8));
                                    p2.AddCell(TextCell($"{p.Length:F1} x {p.Width:F1}", fontSize: 8));
                                }

                                counter++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        var c = new Cell();
                        c.SetBorder(Border.NO_BORDER);
                        c.SetPadding(1);
                        c.Add(p2);


                        table.AddCell(c);
                        columnsPopulated++;
                        remainParts = i == 0
                            ? remainParts.Skip(DefaultSecondPagePartsPerColumn - 2).ToList()
                            : remainParts.Skip(DefaultSecondPagePartsPerColumn).ToList();

                        if (remainParts.Count == 0)
                        {
                            break;
                        }
                    }

                    if (remainParts.Count == 0)
                    {
                        //table.CompleteRow();

                        for (int i = columnsPopulated; i <= 4; i++)
                        {
                            var c = new Cell();
                            c.SetBorder(Border.NO_BORDER);
                            c.SetPadding(1);

                            table.AddCell(c);
                        }

                        doc.Add(table);

                        break;
                    }

                    doc.Add(table);
                }
            }

            #endregion

            doc.Close();

            return fs.ToArray();
        }

#if USE_EMF
        [DllImport("gdiplus.dll", SetLastError = true)]
        private static extern int GdipEmfToWmfBits(IntPtr hEmf, int uBufferSize, byte[] bBuffer, int iMappingMode,
            EmfToWmfBitsFlags flags);

/*
        [DllImport("gdi32.dll")]
        private static extern IntPtr SetEnhMetaFileBits(uint cbBuffer, byte[] lpBuffer);
*/
        [DllImport("gdi32.dll")]
        private static extern bool DeleteEnhMetaFile(IntPtr hEmf);

        [Flags]
        private enum EmfToWmfBitsFlags
        {
            EmfToWmfBitsFlagsDefault = 0x00000000,

            //EmfToWmfBitsFlagsEmbedEmf = 0x00000001,
            EmfToWmfBitsFlagsIncludePlaceable = 0x00000002,
            //EmfToWmfBitsFlagsNoXORClip = 0x00000004
        }

        // private static byte[] GetWmfByteArray(Metafile mf) {
        //     const int MM_ANISOTROPIC = 8;
        //     var handle = mf.GetHenhmetafile();
        //     var bufferSize = GdipEmfToWmfBits(handle, 0, null, MM_ANISOTROPIC, EmfToWmfBitsFlags.EmfToWmfBitsFlagsIncludePlaceable);
        //     byte[] buf = new byte[bufferSize];
        //     GdipEmfToWmfBits(handle, bufferSize, buf, MM_ANISOTROPIC, EmfToWmfBitsFlags.EmfToWmfBitsFlagsIncludePlaceable);
        //     return buf;
        // }

        [StructLayout(LayoutKind.Sequential)]
        private struct Placeablemetaheader
        {
            public uint Key; // Magic number (always 9AC6CDD7h)
            public ushort Handle; // Metafile HANDLE number (always 0) /
            public short Left; // Left coordinate in metafile units /
            public short Top; // Top coordinate in metafile units /
            public short Right; // Right coordinate in metafile units /
            public short Bottom; // Bottom coordinate in metafile units /
            public ushort Inch; // Number of metafile units per inch /
            public uint Reserved; // Reserved (always 0) /
            public ushort Checksum; // Checksum value for previous 10 WORDs */
        }

        private static byte[] GetMetafileRawData(Metafile metafile)
        {
            byte[] data;

            var header = metafile.GetMetafileHeader();

            const int mmAnisotropic = 8;

            var handle = metafile.GetHenhmetafile();

            try
            {
                var flag = EmfToWmfBitsFlags.EmfToWmfBitsFlagsIncludePlaceable;

                var dataSize = GdipEmfToWmfBits(handle, 0, null, mmAnisotropic, flag);

                if (dataSize == 0) //Not Support EmfToWmfBitsFlagsIncludePlaceable
                {
                    flag = EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault;

                    dataSize = GdipEmfToWmfBits(handle, 0, null, mmAnisotropic, flag);
                }

                data = new byte[dataSize];

                _ = GdipEmfToWmfBits(handle, dataSize, data, mmAnisotropic, flag);

                if (flag == EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault) //Add PlaceableMetaHeader to Byte Array
                {
                    var placeableList = new List<byte>();

                    var placeable = new Placeablemetaheader
                    {
                        Key = 0x9AC6CDD7,
                        Left = (short)header.Bounds.Left,
                        Top = (short)header.Bounds.Top,
                        Right = (short)header.Bounds.Width,
                        Bottom = (short)header.Bounds.Height,
                        Inch = 1440,
                        Checksum = 0
                    };

                    placeable.Checksum ^= (ushort)(placeable.Key & 0x0000FFFF);
                    placeable.Checksum ^= (ushort)((placeable.Key & 0xFFFF0000) >> 16);
                    placeable.Checksum ^= placeable.Handle;
                    placeable.Checksum ^= (ushort)placeable.Left;
                    placeable.Checksum ^= (ushort)placeable.Top;
                    placeable.Checksum ^= (ushort)placeable.Right;
                    placeable.Checksum ^= (ushort)placeable.Bottom;
                    placeable.Checksum ^= placeable.Inch;
                    placeable.Checksum ^= (ushort)(placeable.Reserved & 0x0000FFFF);
                    placeable.Checksum ^= (ushort)((placeable.Reserved & 0xFFFF0000) >> 16);

                    placeableList.AddRange(SerializeSequentialStruct(placeable));
                    placeableList.AddRange(data);

                    data = placeableList.ToArray();
                }
            }
            finally
            {
                DeleteEnhMetaFile(handle);
            }

            return data;
        }

        private static byte[] SerializeSequentialStruct(object @struct)
        {
            int size = Marshal.SizeOf(@struct) - 2;
            var arr = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(@struct, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);

            Marshal.FreeHGlobal(ptr);

            return arr;
        }
#endif
    }
}