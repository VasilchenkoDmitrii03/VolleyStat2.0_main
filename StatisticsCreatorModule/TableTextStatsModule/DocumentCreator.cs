using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionsLib;
using System.Media;
using System.Drawing;
using System.Windows.Media;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.IO;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Office.Drawing;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace StatisticsCreatorModule.TableTextStatsModule
{
    internal class DocumentCreator
    {
        static int ImageID = 0;
        public static string basePath;
        static DocumentCreator()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            ImageID = 0;
            basePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImageHolder");
            if (!Directory.Exists(basePath)) //createDirectory if not exists
            {
                Directory.CreateDirectory(basePath);
            }
            else
            {
                foreach (string filePath in Directory.GetFiles(basePath))
                {
                    File.Delete(filePath);
                }
            }
        }
        public DocumentCreator() { }
        public void CreateBaseStatTable(Game _game, WordprocessingDocument document)
        {

            MainDocumentPart mainPart = document.AddMainDocumentPart();
            mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
            Body body = new Body();
            SectionProperties sectionProperties = new SectionProperties();
            PageSize pageSize = new PageSize()
            {
                Width = 16840,   // Ширина для альбомной ориентации (в полупунктах)
                Height = 11900,  // Высота для альбомной ориентации (в полупунктах)
                Orient = PageOrientationValues.Landscape
            };
            sectionProperties.Append(pageSize);
            body.Append(sectionProperties);
            BaseStatTable tableCreator = new BaseStatTable();
            body.Append(tableCreator.process(_game.Team, _game.getVolleyActionSequence()));
            mainPart.Document.Append(body);
        }
        public void CreateBaseStatTablePDF(Game _game, string path)
        {
            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(new QuestPDF.Helpers.PageSize(297, 210, Unit.Millimetre));
                    page.Margin(20);

                    page.Content().Column(column =>
                    {
                        column.Spacing(10);

                        int columnCount = 24;
                        // Добавляем длинную таблицу
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(35, Unit.Millimetre);
                                for (int i = 1; i < columnCount; i++) columns.RelativeColumn();
                            });
                            string[] Headers = new string[] { "Player", "Points", "Serve", "Reception", "Attack", "Block", "Defence" };
                            int[] boldColumnsNumbers = new int[] { 0, 2, 6, 11, 17, 21 };
                            table.Cell().ColumnSpan(1).Text(Headers[0]).FontSize(15);
                            table.Cell().ColumnSpan(2).Text(Headers[1]).FontSize(15);
                            table.Cell().ColumnSpan(4).Text(Headers[2]).FontSize(15);
                            table.Cell().ColumnSpan(5).Text(Headers[3]).FontSize(15);
                            table.Cell().ColumnSpan(6).Text(Headers[4]).FontSize(15);
                            table.Cell().ColumnSpan(4).Text(Headers[5]).FontSize(15);
                            table.Cell().ColumnSpan(2).Text(Headers[6]).FontSize(15);
                            List<string> lst = BaseStatTable.CreateTablePDF(_game);
                            int index = 0;
                            foreach (string s in lst)
                            {
                                if(index % 24 == 0){
                            table.Cell().Border(1) // Устанавливаем границу толщиной 1 пиксель
            .BorderColor(QuestPDF.Infrastructure.Color.FromRGB(0, 0, 0))
            .Background(QuestPDF.Infrastructure.Color.FromRGB(255, 255, 255))
            .Padding(2)
            .Text(s).FontSize(9);
                                }
                                else if(boldColumnsNumbers.Contains(index % 24))
                                {
                                    table.Cell().BorderRight(3).BorderBottom(1).BorderLeft(1).BorderTop(1) // Устанавливаем границу толщиной 1 пиксель
            .BorderColor(QuestPDF.Infrastructure.Color.FromRGB(0, 0, 0))
            .Background(QuestPDF.Infrastructure.Color.FromRGB(255, 255, 255))
            .Padding(2)
            .Text(s).FontSize(9);
                                }
                                else
                                {
                                    table.Cell().Border(1) // Устанавливаем границу толщиной 1 пиксель
           .BorderColor(QuestPDF.Infrastructure.Color.FromRGB(0, 0, 0))
           .Background(QuestPDF.Infrastructure.Color.FromRGB(255, 255, 255))
           .Padding(2).AlignCenter()
           .Text(s).FontSize(9);
                                }

                                index++;
                            }
                        });
                    });
                });
            })
       .GeneratePdf(path);
        }
        public void CreateBaseSetterTable(Game _game, WordprocessingDocument document)
        {
            MainDocumentPart mainPart = document.AddMainDocumentPart();
            mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
            Body body = new Body();
            SetStatTable tableCreator = new SetStatTable();

            DocumentFormat.OpenXml.Wordprocessing.Paragraph par = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Reception distribution")));
            body.Append(par);
            body.Append(tableCreator.getBlockersDistributionByReceptionQuality(_game.getVolleyActionSegmentSequence(), _game));

            DocumentFormat.OpenXml.Wordprocessing.Table[] tables = tableCreator.getReceptionZoneDistributionStatistics(_game.getVolleyActionSegmentSequence());
            for (int i = 0; i < tables.Length; i++)
            {
                DocumentFormat.OpenXml.Wordprocessing.Paragraph paragraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text($"P{i + 1}")));
                body.Append(paragraph);
                body.Append(tables[i]);
            }

            mainPart.Document.Append(body);
        }
        public void CreateBaseReceptionTable(Game _game, WordprocessingDocument document)
        {
            MainDocumentPart mainPart = document.AddMainDocumentPart();
            mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
            Body body = new Body();
            ReceptionStatTable tableCreator = new ReceptionStatTable();
            VolleyActionSequence seq = _game.getVolleyActionSequence();
            foreach (Player p in _game.Team.Players)
            {
                DocumentFormat.OpenXml.Wordprocessing.Table[] tables = tableCreator.createGliderAndJumpTablesForPlayer(p, seq);
                if (tables == null) continue;
                DocumentFormat.OpenXml.Wordprocessing.Paragraph player = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text($"Player #{p.Number}")));
                body.Append(player);

                DocumentFormat.OpenXml.Wordprocessing.Paragraph glider = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text($"Glider")));
                body.Append(glider);
                body.Append(tables[0]);
                DocumentFormat.OpenXml.Wordprocessing.Paragraph jump = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text($"Jump")));
                body.Append(jump);
                body.Append(tables[1]);
            }

            mainPart.Document.Append(body);
        }



        public void ImageTestDocument(Game _game, WordprocessingDocument document)
        {
            VolleyActionSequence serves = _game.getVolleyActionSequence().SelectActionsByCondition((act) => { return act.VolleyActionType == VolleyActionType.Serve && act.Player.Number == 1; });
            VolleyActionSequence sets = _game.getVolleyActionSequence().SelectActionsByCondition((act) => { return act.VolleyActionType == VolleyActionType.Set && act.Player.Number == 14; });
            VolleyActionSequence attacks = _game.getVolleyActionSequence().SelectActionsByCondition((act) => { return act.VolleyActionType == VolleyActionType.Attack && act.Player.Number == 1; });
            VolleyActionSequence reception = _game.getVolleyActionSequence().SelectActionsByCondition((act) => { return act.VolleyActionType == VolleyActionType.Reception && act.Player.Number == 17; });
            createImage(serves);
            createImage(sets);
            createImage(attacks);
            createImage(reception);
        }
        public void CreateFullReceptionStats(Game _game, string path)
        {

            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    //page.Size(PageSizes.A4.Landscape()); // Альбомная ориентация
                    page.Size(new QuestPDF.Helpers.PageSize(210, 297, Unit.Millimetre));
                    page.Margin(50);
                    page.Content().Row(row =>
                    {
                        // Таблица слева
                        row.RelativeItem().Table(table =>
                        {
                            // Определяем 3 столбца
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // Добавляем строки
                            for (int i = 0; i < 2; i++) // 2 строки
                            {
                                table.Cell().Text($"Row {i + 1}, Col 1");
                                table.Cell().Text($"Row {i + 1}, Col 2");
                                table.Cell().Text($"Row {i + 1}, Col 3");
                            }

                            // Добавляем границы для таблицы
                            table.Cell().Border(1).BorderColor(QuestPDF.Infrastructure.Color.FromRGB(100, 100, 100));
                        });
                        VolleyActionSequence attacks = _game.getVolleyActionSequence().SelectActionsByCondition((act) => { return act.VolleyActionType == VolleyActionType.Attack && act.Player.Number == 1; });
                        int id = createImage(attacks);
                        // Картинка справа
                        row.ConstantItem(150).Image(System.IO.Path.Combine(basePath, $"image_{id}.jpeg"), ImageScaling.FitWidth);

                    }); 
                    });
            })
        .GeneratePdf(path);

        }
        public static void AddImageToBody(MainDocumentPart mainPart, string relationshipId)
        {
            // Создаем элемент Drawing для вставки изображения
            var element = new DocumentFormat.OpenXml.Office.Drawing.Drawing(
                new Inline(
                    new Extent() { Cx = 990000L, Cy = 792000L }, // Размеры изображения
                    new DocProperties() { Id = (UInt32Value)1U, Name = "Picture 1" },
                    new BlipFill(
                        new Blip() { Embed = relationshipId },
                        new DocumentFormat.OpenXml.Drawing.Stretch(new FillRectangle())
                    ),
                    new DocumentFormat.OpenXml.Drawing.Charts.ShapeProperties(
                        new DocumentFormat.OpenXml.Drawing.Transform2D(
                            new Offset() { X = 0L, Y = 0L },
                            new Extents() { Cx = 990000L, Cy = 792000L }
                        )
                    )
                )
            );

            // Добавляем изображение в параграф и добавляем параграф в тело документа
            var paragraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(element));
            mainPart.Document.Body.Append(paragraph);
        }
        static void AddImageToWordDocument(MainDocumentPart mainPart, string imagePath, Body body)
        {
            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

            using (FileStream stream = new FileStream(imagePath, FileMode.Open))
            {
                imagePart.FeedData(stream);
            }
            
            // Создаем объект рисунка
            var drawing = new DocumentFormat.OpenXml.Office.Drawing.Drawing(
                new DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline(
                    new DocumentFormat.OpenXml.Drawing.BlipFill(
                        new DocumentFormat.OpenXml.Drawing.Blip { Embed = mainPart.GetIdOfPart(imagePart) }
                    )
                )
            );

            // Создаем параграф для изображения и добавляем его в тело документа
            DocumentFormat.OpenXml.Wordprocessing.Paragraph imageParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(drawing));
            body.Append(imageParagraph);
        }


        private void DrawAction(Graphics graphics, PointF[] points, PlayerAction action)
        {
            int quality = action.GetQuality();
            System.Drawing.Pen Pen = new System.Drawing.Pen(System.Drawing.Color.Black, 2);
            System.Drawing.Brush Brush = System.Drawing.Brushes.Black;
            int dotSize = 6;
            switch (quality)
            {
                case 6: case 5:
                    Pen = new System.Drawing.Pen(System.Drawing.Color.Green, 2);
                    Brush = System.Drawing.Brushes.Green;
                    break;
                case 4: case 3:
                    Pen = new System.Drawing.Pen(System.Drawing.Color.Black, 2);
                    Brush = System.Drawing.Brushes.Black;
                    break;
                case 2: case 1:
                    Pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2);
                    Brush = System.Drawing.Brushes.Red;
                    break;
            }
            switch (action.VolleyActionType)
            {
                case VolleyActionType.Attack:
                    DrawAttack(graphics, points, Pen, Brush, dotSize); break;
                case VolleyActionType.Serve:
                    DrawServe(graphics, points, Pen, Brush, dotSize); break;
                case VolleyActionType.Reception:
                    DrawReception(graphics, points, Pen, Brush, dotSize); break;
                case VolleyActionType.Set:
                    DrawSet(graphics, points, Pen, Brush, dotSize); break;
            }

        }
        private void DrawAttack(Graphics graphics, PointF[] points, System.Drawing.Pen Pen, System.Drawing.Brush Brush, int dotSize)
        {
            if(points.Length == 0) return;
            if(points.Length == 1) graphics.FillEllipse(Brush, points[0].X - dotSize / 2, points[0].Y - dotSize / 2, dotSize, dotSize);
            else
            {
                
                for(int i =1 ;i < points.Length; i++)
                {
                    if(i == points.Length - 1)
                    {
                        System.Drawing.Pen arrowPen = new System.Drawing.Pen(Pen.Color, Pen.Width);

                        AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 5);
                        arrowPen.CustomEndCap = bigArrow;
                        graphics.DrawLine(arrowPen, points[i - 1], points[i]);
                    }
                    else graphics.DrawLine(Pen, points[i - 1], points[i]);

                }
                graphics.FillEllipse(Brush, points[0].X - dotSize / 2, points[0].Y - dotSize / 2, dotSize, dotSize);
            }
        }
        private void DrawReception(Graphics graphics, PointF[] points, System.Drawing.Pen Pen, System.Drawing.Brush Brush, int dotSize)
        {
            if (points.Length == 0) return;
            if (points.Length == 1) graphics.FillEllipse(Brush, points[0].X - dotSize / 2, points[0].Y - dotSize / 2, dotSize, dotSize);
            else
            {
                graphics.FillEllipse(Brush, points[0].X - dotSize / 2, points[0].Y - dotSize / 2, dotSize, dotSize);
                System.Drawing.Pen arrowPen = new System.Drawing.Pen(Pen.Color, Pen.Width);

                AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 5);
                arrowPen.CustomEndCap = bigArrow;
                graphics.DrawLine(arrowPen, points[0], points[1]);
            }
        }
        private void DrawSet(Graphics graphics, PointF[] points, System.Drawing.Pen Pen, System.Drawing.Brush Brush, int dotSize)
        {
            if (points.Length == 0) return;
            if (points.Length == 1) graphics.FillEllipse(Brush, points[0].X - dotSize / 2, points[0].Y - dotSize / 2, dotSize, dotSize);
            else
            {
                graphics.FillEllipse(Brush, points[0].X - dotSize / 2, points[0].Y - dotSize / 2, dotSize, dotSize);
                System.Drawing.Pen arrowPen = new System.Drawing.Pen(Pen.Color, Pen.Width);

                AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 5);
                arrowPen.CustomEndCap = bigArrow;
                graphics.DrawLine(arrowPen, points[0], points[1]);
            }
        }
        private void DrawServe(Graphics graphics, PointF[] points, System.Drawing.Pen Pen, System.Drawing.Brush Brush, int dotSize)
        {
            if (points.Length == 0) return;
            if (points.Length == 1) graphics.FillEllipse(Brush, points[0].X - dotSize / 2, points[0].Y - dotSize / 2, dotSize, dotSize);
        }

        private PointF[] convert(ActionsLib.Point[] pts, int width, int height, int border)
        {
            PointF[] res = new PointF[pts.Length];
            for (int i = 0; i < pts.Length; i++)
            {
                res[i] = new PointF((float)pts[i].X * width +border , (float)pts[i].Y * height + border);
            }
            return res;
        }
        private int createImage(VolleyActionSequence seq)
        {
            if (seq.Count == 0) return -1;
            int width = 220;
            int height = 420;
            int borders = 10;
            using (Bitmap bitmap = new Bitmap(width, height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {

                    graphics.Clear(System.Drawing.Color.Orange);
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    // Рисуем прямоугольник синим цветом и толщиной в 2 пикселя
                    System.Drawing.Pen bluePen = new System.Drawing.Pen(System.Drawing.Color.White, 2);
                    graphics.DrawRectangle(bluePen, borders, borders, width - 2 * borders, height - 2 * borders);

                    graphics.DrawLine(bluePen, borders, height / 2, width - borders, height / 2);
                    graphics.DrawLine(bluePen, borders, (height - 2 * borders) / 3 + borders, width - borders, (height - 2 * borders) / 3 + borders);
                    graphics.DrawLine(bluePen, borders, (height - 2 * borders) * 2 / 3 + borders, width - borders, (height - 2 * borders) * 2 / 3 + borders);
                    // Рисуем точки красного цвета
                    foreach(ActionsLib.Action act in seq)
                    {
                        if (act.AuthorType != ActionAuthorType.Player) continue;
                        DrawAction(graphics, convert(((PlayerAction)act).Points, width - 2 * borders , height - 2 * borders, borders), (PlayerAction)act);
                    }
                }


                bitmap.Save(System.IO.Path.Combine(basePath, $"image_{ImageID}.jpeg"), System.Drawing.Imaging.ImageFormat.Jpeg);
                return ImageID++;
            }
        }
    }
}

