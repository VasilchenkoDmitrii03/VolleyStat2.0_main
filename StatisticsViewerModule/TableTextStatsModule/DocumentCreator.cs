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
using DocumentFormat.OpenXml.Office2013.Excel;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using System.Diagnostics.PerformanceData;
using System.Security.Policy;
using System.Drawing.Imaging;
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
                CreateBaseStatTablePDF(_game, container);
            })
       .GeneratePdf(path);
        }
        private void CreateBaseStatTablePDF(Game _game, IDocumentContainer container)
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
                        int[] boldRowsNumbers = new int[] { 0, _game.Team.Players.Count };
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
                            int bottom = 1;
                            int top = 1;
                            int left = 1;
                            int right = 1;
                            byte r = 255, g = 255, b = 255;
                            bool alignCenter = false;
                            if (index == 0)
                            {
                                alignCenter = true;
                            }
                            if (boldColumnsNumbers.Contains(index % 24)) right = 4;
                            if (boldRowsNumbers.Contains(index / 24)) bottom = 4;
                            if ((index / 24) % 2 == 1)
                            {
                                r = g = b = 216;
                            }
                            if (alignCenter)
                            {
                                table.Cell().BorderRight(right).BorderBottom(bottom).BorderLeft(left).BorderTop(top) // Устанавливаем границу толщиной 1 пиксель
        .BorderColor(QuestPDF.Infrastructure.Color.FromRGB(0, 0, 0))
        .Background(QuestPDF.Infrastructure.Color.FromRGB(255, 255, 255))
        .Padding(2)
        .Text(s).FontSize(9);
                            }
                            else
                            {
                                table.Cell().BorderRight(right).BorderBottom(bottom).BorderLeft(left).BorderTop(top) // Устанавливаем границу толщиной 1 пиксель
        .BorderColor(QuestPDF.Infrastructure.Color.FromRGB(0, 0, 0))
        .Background(QuestPDF.Infrastructure.Color.FromRGB(r, g, b))
        .Padding(2).AlignCenter()
        .Text(s).FontSize(9);
                            }
                            index++;
                        }
                    });
                });
            });
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

        #region Total file

        public void CreateTotalStatisticsFilePDF(Game _game, string path)
        {
            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    CreateBaseStatTablePDF(_game, container);
                    foreach(Player p in _game.Team.Players)
                    {
                        GetFullPlayerStatistics(_game, p, container);
                    }
                });
            })
       .GeneratePdf(path);
        }

        private void GetFullPlayerStatistics(Game _game, Player p, IDocumentContainer container)
        {
            VolleyActionSequence playersActions = _game.getVolleyActionSequence().SelectActionsByCondition((act) => { return act.Player == p; });
            int[] counts = playersActions.CountActionsByCondition((act) => { return act.ActionType == VolleyActionType.Serve; }, (act) => { return act.ActionType == VolleyActionType.Reception; }, (act) => { return act.ActionType == VolleyActionType.Attack; }, (act) => { return act.ActionType == VolleyActionType.Set; });
            if (playersActions.Count == 0) return;
            container.Page(page =>
            {
                page.Size(new QuestPDF.Helpers.PageSize(210, 297, Unit.Millimetre));
                page.Margin(10);
                page.Content().Column(column =>
                {
                    column.Item().Text($"Player #{p.Number} {p.Surname} {p.Name}").FontSize(70).Bold().AlignCenter();
                    if (counts[0] > 0)
                    {
                        column.Item().Text($"Serve").FontSize(25).Bold().AlignCenter();
                        CreatePlayerServeStats(_game.getVolleyActionSequence(), column, p);
                    }
                    if (counts[1] > 0)
                    {
                        column.Item().Text($"Reception").FontSize(25).Bold().AlignCenter();
                        CreatePlayerReceptionStats(_game.getVolleyActionSequence(), column, p);
                    }
                    if (counts[2] > 0)
                    {
                        column.Item().Text($"Attack").FontSize(25).Bold().AlignCenter();
                        CreatePlayerAttackStats(_game.getVolleyActionSequence(), column, p);
                    }
                    if (counts[3] > 0)
                    {
                        column.Item().Text($"Set").FontSize(25).Bold().AlignCenter();
                        CreatePlayerSetStats(_game.getVolleyActionSegmentSequence(), _game, column, p);
                    }
                    
                });
            });
        }
        #endregion

        #region Attack
        public void CreateBaseAttackTablePDF(Game _game, string path)
        {
            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(new QuestPDF.Helpers.PageSize(210, 297, Unit.Millimetre));
                    page.Margin(10);
                    ReceptionStatTable tableCreator = new ReceptionStatTable();
                    page.Content().Column(column =>
                    {
                        column.Item().Text("Attack").FontSize(60).Bold().AlignCenter();
                        foreach (Player p in _game.Team.Players)
                        {

                            CreatePlayerAttackStats(_game.getVolleyActionSequence(), column, p);

                        }


                    });
                });
            })
       .GeneratePdf(path);
        }
        private void CreatePlayerAttackStats(VolleyActionSequence sequence, ColumnDescriptor column, Player p)
        {
            ReceptionStatTable tableCreator = new ReceptionStatTable();
            VolleyActionSequence seq = sequence.SelectActionsByCondition(pl => { return pl.AuthorType == ActionAuthorType.Player && ((PlayerAction)(pl)).Player == p && pl.ActionType == VolleyActionType.Attack; });
            if (seq.Count == 0) return;
            column.Item().Text($"Player #{p.Number} {p.Surname} {p.Name}").FontSize(25).Bold().AlignCenter();
            for(int i = 1; i <=6; i++)
            {
                VolleyActionSequence positionSequence = seq.SelectActionsByCondition((act) => { return (int)act[act.MetricTypes[2]].Value == i; });
                FullAttackStatsWithImagesByPosition(positionSequence, column, tableCreator, i);
            }
        }
        private void FullAttackStatsWithImagesByPosition(VolleyActionSequence seq, ColumnDescriptor column, ReceptionStatTable tableCreator,int zoneNumber)
        {
            if (seq.Count == 0) return;
            column.Item().Text($"Position {zoneNumber}").FontSize(20).Bold();
            QualityStatTableWithImage(seq, column, tableCreator, $"Total position {zoneNumber} ");
            MetricType blockersCount = null;
            foreach(MetricType mt in seq[0].MetricTypes) if(mt.Name == "BlockersCount") { blockersCount = mt; break; }
            foreach(object val in blockersCount.AcceptableValues)
            {
                VolleyActionSequence blockersCountSeq = seq.SelectActionsByCondition((act) => { return act[blockersCount].Value == val; });
                FullAttackStatsWithImagesByPosition(blockersCountSeq, column, tableCreator, zoneNumber, blockersCount.AcceptableValuesNames[val]);
            }
        }
        private void FullAttackStatsWithImagesByPosition(VolleyActionSequence seq, ColumnDescriptor column, ReceptionStatTable tableCreator, int zoneNumber, string blockersCount)
        {
            if (seq.Count == 0) return;
            MetricType setQuality = null;
            foreach (MetricType Type in seq[0].MetricTypes) if (Type.Name == "SetQuality") setQuality = Type;
            column.Item().Text($" Zone #{zoneNumber} {blockersCount} blockers").FontSize(17).Bold();
            foreach (object val in setQuality.AcceptableValuesNames.Keys)
            {
                VolleyActionSequence setQualitySeq = seq.SelectActionsByCondition((act) => { return act[setQuality].Value == val; });
                QualityStatTableWithImage(setQualitySeq, column, tableCreator, $"Set quality: {setQuality.AcceptableValuesNames[val]}");
            }
        }


        #endregion

        #region Set
        public void CreateBaseSetTablePDF(Game _game, string path)
        {
            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(new QuestPDF.Helpers.PageSize(210, 297, Unit.Millimetre));
                    page.Margin(10);
                    ReceptionStatTable tableCreator = new ReceptionStatTable();
                    page.Content().Column(column =>
                    {
                        column.Item().Text("Set").FontSize(60).Bold().AlignCenter();
                        foreach (Player p in _game.Team.Players)
                        {
                            CreatePlayerSetStats(_game.getVolleyActionSegmentSequence(), _game, column, p);
                        }


                    });
                });
            })
       .GeneratePdf(path);
        }
        private void CreatePlayerSetStats(VolleyActionSegmentSequence sequence, Game game, ColumnDescriptor column, Player p)
        {
            SetStatTable tableCreator = new SetStatTable();
            VolleyActionSegmentSequence seq = sequence.SelectByCondition(pl => { return pl.ContainsActionType(VolleyActionType.Set) && pl.ContainsActionType(VolleyActionType.Reception); });
            if (seq.Count == 0) return;
            column.Item().Text($"Player #{p.Number} {p.Surname} {p.Name}").FontSize(25).Bold().AlignCenter();
            //blockers Distribution
            column.Item().Text($"Blockers count distribution").FontSize(20).Bold().AlignCenter();
            tableCreator.getBlockersDistibutionPDF(seq, game, column);
            //arrangement distribution
            column.Item().Text($"Reception direction distribution").FontSize(20).Bold().AlignCenter();
            for (int i = 1; i <= 6; i++) CreateArrangementReceptionDistributionStatWithImage(seq, i, tableCreator, column);
            //attackers distribution 
            foreach(Player P in game.Team.Players)
            {
                CreateAttackersSetDistributionStatWithImage(sequence, P, tableCreator, column);
            }

        }
        private void CreateArrangementReceptionDistributionStatWithImage(VolleyActionSegmentSequence seq,int zone, SetStatTable tableCreator,  ColumnDescriptor column)
        {
            column.Item().Text($"P{zone} arrangement").FontSize(15).Bold().AlignCenter();
            column.Item().Row(row => {
                tableCreator.createReceptionDistributionTable(seq, zone, row);
                VolleyActionSequence setsSeq = seq.ConvertToActionSequence().SelectActionsByCondition((pl) => { return pl.ActionType == VolleyActionType.Set &&(int) pl[pl.MetricTypes[1]].Value == zone; });
                int id = createImage(setsSeq);
                row.RelativeColumn()
           .Image(System.IO.Path.Combine(basePath, $"image_{id}.jpeg"))
           .FitArea();
            });
        }
        private void CreateAttackersSetDistributionStatWithImage(VolleyActionSegmentSequence seq, Player p, SetStatTable tableCreate, ColumnDescriptor column)
        {
            VolleyActionSegmentSequence sequence = seq.SelectByCondition((seg) => { return seg.ContainsActionType(VolleyActionType.Set) && seg.ContainsActionType(VolleyActionType.Attack) && seg.getByActionType(VolleyActionType.Attack).Player == p; });
            if (sequence.Count == 0) return;
            column.Item().Text($"Player #{p.Number}").FontSize(15).Bold().AlignCenter();
            column.Item().Row(row => {
                tableCreate.getPlayersSetDistributionTablePDF(sequence, p, row);
                VolleyActionSequence setsSeq = sequence.ConvertToActionSequence().SelectActionsByCondition((pl) => { return pl.ActionType == VolleyActionType.Set; });
                int id = createImage(setsSeq);
                row.RelativeColumn()
           .Image(System.IO.Path.Combine(basePath, $"image_{id}.jpeg"))
           .FitArea();
            });
        }
        #endregion

        #region Serve
        public void CreateBaseServeTablePDF(Game _game, string path)
        {
            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(new QuestPDF.Helpers.PageSize(210, 297, Unit.Millimetre));
                    page.Margin(10);
                    ReceptionStatTable tableCreator = new ReceptionStatTable();
                    page.Content().Column(column =>
                    {
                        column.Item().Text("Serve").FontSize(60).Bold().AlignCenter();
                        foreach (Player p in _game.Team.Players)
                        {
                            CreatePlayerServeStats(_game.getVolleyActionSequence(), column, p);
                        }


                    });
                });
            })
       .GeneratePdf(path);
        }
        private void CreatePlayerServeStats(VolleyActionSequence sequence, ColumnDescriptor column, Player p)
        {
            ReceptionStatTable tableCreator = new ReceptionStatTable();
            VolleyActionSequence seq = sequence.SelectActionsByCondition(pl => { return pl.AuthorType == ActionAuthorType.Player && ((PlayerAction)(pl)).Player == p && pl.ActionType == VolleyActionType.Serve; });
            if (seq.Count == 0) return;
            column.Item().Text($"Player #{p.Number} {p.Surname} {p.Name}").FontSize(25).Bold().AlignCenter();
            MetricType serveType = seq[0]["ServeType"].MetricType;
            foreach(object val in serveType.AcceptableValues)
            {
                VolleyActionSequence tmp = seq.SelectActionsByCondition(pl => { return pl[serveType].Value == val; });
                FullServeStatsWithImages(tmp, column, tableCreator, serveType.AcceptableValuesNames[val]);
            }
          
        }
        private void FullServeStatsWithImages(VolleyActionSequence seq, ColumnDescriptor column, ReceptionStatTable tableCreator, string name)
        {
            if (seq.Count == 0) return;
            column.Item().Text(name).FontSize(20).Bold();
            QualityStatTableWithImage(seq, column, tableCreator, "Total " + name);
            for (int i = 1; i <= 6; i++)
            {
                VolleyActionSequence directionSequence = seq.SelectActionsByCondition((act) => { return (int)act["Direction"].Value == i; });
                if (directionSequence.Count == 0) continue;
                column.Item().Text($"{name} Direction {i}").FontSize(10).Bold();
                column.Item().Row(row => {
                    tableCreator.QualityTable(directionSequence, row);
                    int id = createImage(directionSequence);
                    row.RelativeColumn()
           .Image(System.IO.Path.Combine(basePath, $"image_{id}.jpeg"))
           .FitArea();
                });
            }
        }
 
        #endregion

        #region Reception
        public void CreateBaseReceptionTablePDF(Game _game, string path)
        {
            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(new QuestPDF.Helpers.PageSize(210, 297, Unit.Millimetre));
                    page.Margin(10);
                    ReceptionStatTable tableCreator = new ReceptionStatTable();
                    page.Content().Column(column =>
                    {
                        column.Item().Text("Serve").FontSize(60).Bold().AlignCenter();
                        foreach (Player p in _game.Team.Players)
                        {

                            CreatePlayerReceptionStats(_game.getVolleyActionSequence(), column, p);

                        }


                    });
                });
            })
       .GeneratePdf(path);
        }
        private void CreatePlayerReceptionStats(VolleyActionSequence sequence, ColumnDescriptor column, Player p)
        {
            ReceptionStatTable tableCreator = new ReceptionStatTable();
            VolleyActionSequence seq = sequence.SelectActionsByCondition(pl => { return pl.AuthorType == ActionAuthorType.Player && ((PlayerAction)(pl)).Player == p && pl.ActionType == VolleyActionType.Reception; });
            if (seq.Count == 0) return;
            column.Item().Text($"Player #{p.Number} {p.Surname} {p.Name}").FontSize(25).Bold().AlignCenter();
            int[] counts = seq.CountActionsByCondition((pl => { return pl["ServeType"].getShortString() == "gldr"; }), (pl => { return pl["ServeType"].getShortString() == "jmp"; }));
            VolleyActionSequence gliders = seq.SelectActionsByCondition(pl => { return pl["ServeType"].getShortString() == "gldr"; });
            VolleyActionSequence jumps = seq.SelectActionsByCondition(pl => { return pl["ServeType"].getShortString() == "jmp"; });
            FullStatsWithImages(gliders, column, tableCreator, "Glider");
            FullStatsWithImages(jumps, column, tableCreator, "Jump");
        }
        private void FullStatsWithImages(VolleyActionSequence seq, ColumnDescriptor column, ReceptionStatTable tableCreator, string name)
        {
            if (seq.Count == 0) return;
            column.Item().Text(name).FontSize(20).Bold();
            QualityStatTableWithImage(seq, column, tableCreator, "Total " + name);
            tableCreator.createServeTypeTable(seq, column);
            for(int i = 1;i <= 6; i++)
            {
                VolleyActionSequence positionSequence = seq.SelectActionsByCondition((act) => { return (int)act[act.MetricTypes[2]].Value == i; });
                FullStatsWithImagesByPosition(positionSequence, column, tableCreator, i, name);
            }
        }
        private void FullStatsWithImagesByPosition(VolleyActionSequence seq, ColumnDescriptor column, ReceptionStatTable tableCreator, int zoneNumber, string preString)
        {
            if (seq.Count == 0) return;
            MetricType receptionType = null;
            foreach(MetricType Type in seq[0].MetricTypes) if(Type.Name == "ReceptionType") receptionType = Type;
            column.Item().Text($"{preString} Zone #{zoneNumber}").FontSize(17).Bold();
            foreach (object val in receptionType.AcceptableValuesNames.Keys)
            {
                VolleyActionSequence receptionTypeSeq = seq.SelectActionsByCondition((act) => { return act[receptionType].Value == val; });
                QualityStatTableWithImage(receptionTypeSeq, column, tableCreator, receptionType.AcceptableValuesNames[val]);
            }
        }
        private void QualityStatTableWithImage(VolleyActionSequence seq, ColumnDescriptor column, ReceptionStatTable tableCreator, string type)
        {
            if (seq.Count == 0) return;
            column.Item().Text(type).FontSize(10).Bold();
            column.Item().Row(row => {
                tableCreator.QualityTable(seq, row);
                int id = createImage(seq);
                row.RelativeColumn()
       .Image(System.IO.Path.Combine(basePath, $"image_{id}.jpeg"))
       .FitArea();
            });
        }

        #endregion



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
            //if (seq.Count == 0) return -1;
            int width = 220;
            int height = 420;
            int borders = 10;
            using (Bitmap bitmap = new Bitmap(width, height))
            {
                using (System.Drawing.Graphics graphics = Graphics.FromImage(bitmap))
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
                bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);

                bitmap.Save(System.IO.Path.Combine(basePath, $"image_{ImageID}.jpeg"), System.Drawing.Imaging.ImageFormat.Jpeg);
                return ImageID++;
            }
        }
    }
}

