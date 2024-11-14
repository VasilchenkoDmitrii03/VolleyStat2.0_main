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

namespace StatisticsCreatorModule.TableTextStatsModule
{
    internal class DocumentCreator
    {
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

            DocumentFormat.OpenXml.Wordprocessing.Paragraph par = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new Text("Reception distribution")));
            body.Append(par);
            body.Append(tableCreator.getBlockersDistributionByReceptionQuality(_game.getVolleyActionSegmentSequence(), _game));

            DocumentFormat.OpenXml.Wordprocessing.Table[] tables = tableCreator.getReceptionZoneDistributionStatistics(_game.getVolleyActionSegmentSequence());
            for (int i = 0; i < tables.Length; i++)
            {
                DocumentFormat.OpenXml.Wordprocessing.Paragraph paragraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new Text($"P{i + 1}")));
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
                DocumentFormat.OpenXml.Wordprocessing.Paragraph player = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new Text($"Player #{p.Number}")));
                body.Append(player);

                DocumentFormat.OpenXml.Wordprocessing.Paragraph glider = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new Text($"Glider")));
                body.Append(glider);
                body.Append(tables[0]);
                DocumentFormat.OpenXml.Wordprocessing.Paragraph jump = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new Text($"Jump")));
                body.Append(jump);
                body.Append(tables[1]);
            }

            mainPart.Document.Append(body);
        }


        public void ImageTestDocument(Game _game, WordprocessingDocument document)
        {
            VolleyActionSequence attacks = _game.getVolleyActionSequence().SelectActionsByCondition((act) => { return act.VolleyActionType == VolleyActionType.Set && (int)act["Direction"].Value == 3; });
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

                    graphics.DrawLine(bluePen, borders, height / 2, width - borders, height/ 2);
                    graphics.DrawLine(bluePen, borders, (height - 2 * borders) / 3 + borders, width - borders, (height - 2 * borders) / 3 + borders);
                    graphics.DrawLine(bluePen, borders, (height - 2 * borders)*2 / 3 + borders, width - borders, (height - 2 * borders)*2 / 3 + borders);
                    // Рисуем точки красного цвета
                    System.Drawing.Brush redBrush = System.Drawing.Brushes.Black;
                    System.Drawing.Pen blackPen = new System.Drawing.Pen(System.Drawing.Color.Black, 1);
                    foreach (ActionsLib.Action act in attacks)
                    {
                        ActionsLib.Point[] points = ((PlayerAction)act).Points;
                        PointF[] pointsF = convert(points, width, height, 0);
                        if (pointsF.Length < 2) continue;
                        graphics.DrawLines(blackPen, pointsF);
                        int dotSize = 10;
                        graphics.FillEllipse(redBrush, pointsF[0].X - dotSize / 2, pointsF[0].Y - dotSize / 2, dotSize, dotSize);
                        graphics.FillEllipse(redBrush, pointsF.Last().X - dotSize/2, pointsF.Last().Y - dotSize / 2, dotSize, dotSize);
                    }
                }

                // Сохраняем изображение как JPEG
                bitmap.Save("RectangleWithPoints.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            

        }
        private PointF[] convert(ActionsLib.Point[] pts, int width, int height, int border)
        {
            PointF[] res = new PointF[pts.Length];
            for(int i=  0;i < pts.Length; i++)
            {
                res[i] = new PointF((float)pts[i].X * width + border, (float)pts[i].Y * height + border);
            }
        return  res;
        }
    }
}
