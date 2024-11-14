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

namespace StatisticsCreatorModule.TableTextStatsModule
{
    internal class DocumentCreator
    {
        static int ImageID = 0;
        public static string basePath;
        static DocumentCreator()
        {
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
            mainPart.Document = new Document();
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
        public void CreateBaseSetterTable(Game _game, WordprocessingDocument document)
        {
            MainDocumentPart mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document();
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
            mainPart.Document = new Document();
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
        public void CreateFullReceptionStats(Game _game, WordprocessingDocument document)
        {
            MainDocumentPart mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document(new Body());

            // Добавляем изображение как часть ImagePart
            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
            VolleyActionSequence attacks = _game.getVolleyActionSequence().SelectActionsByCondition((act) => { return act.VolleyActionType == VolleyActionType.Attack && act.Player.Number == 1; });
            int id = createImage(attacks);
            // Загружаем изображение в ImagePart
            using (FileStream stream = new FileStream(System.IO.Path.Combine(basePath, $"image_{id}.jpeg"), FileMode.Open))
            {
                imagePart.FeedData(stream);
            }

            // Получаем ID добавленной части изображения
            string relationshipId = mainPart.GetIdOfPart(imagePart);

            // Используем метод для добавления изображения в тело документа
            AddImageToBody(mainPart, relationshipId);

            // Сохраняем документ
            mainPart.Document.Save();

           
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

