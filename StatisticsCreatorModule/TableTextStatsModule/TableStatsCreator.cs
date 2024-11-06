using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ActionsLib;
using System.ComponentModel.Design;
using DocumentFormat.OpenXml.Office2013.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace StatisticsCreatorModule.TableTextStatsModule
{
    public abstract class TableStatsCreator
    {
        public TableStatsCreator() { }
        public abstract Table process(Team team, VolleyActionSequence seq);
        public abstract Table process(Team team, VolleyActionSegmentSequence seq);

        protected string convertValuesToString(int value)
        {
            if (value == 0) return ".";
            return value.ToString();
        }
        protected string convertValuesToPercentString(int value, int total)
        {
            if (total == 0) return ".";
            return $"{value * 100 / total}%";
        }
        protected TableCell CreateCell(string text, bool hasRightBorder = false, bool hasBottomBorder = false, string width = "720")
        {
            var cell = new TableCell(new Paragraph(new Run(new Text(text))));

            // Установка ширины ячейки
            TableCellProperties cellProperties = new TableCellProperties();
            cellProperties.TableCellWidth = new TableCellWidth() { Width = width, Type = TableWidthUnitValues.Dxa }; // dxa - единицы измерения для ширины
            cellProperties.TableCellBorders = new TableCellBorders();
            cellProperties.TableCellBorders.RightBorder = new RightBorder() { Val = BorderValues.Single, Size = 4 };
            cellProperties.TableCellBorders.LeftBorder = new LeftBorder() { Val = BorderValues.Single, Size = 4 };
            cellProperties.TableCellBorders.BottomBorder = new BottomBorder() { Val = BorderValues.Single, Size = 4 };
            cellProperties.TableCellBorders.TopBorder = new TopBorder() { Val = BorderValues.Single, Size = 4 };
            // Настройка границ: слева, справа, снизу, сверху
            if (hasRightBorder)
            {
                cellProperties.TableCellBorders.RightBorder = new RightBorder() { Val = BorderValues.Single, Size = 12 }; // Толщина 12
            }
            if (hasBottomBorder)
            {
                cellProperties.TableCellBorders.BottomBorder = new BottomBorder() { Val = BorderValues.Single, Size = 12 }; // Жирная граница вниз
            }

            cell.TableCellProperties = cellProperties;
            return cell;
        }
    }
    public class BaseStatTable : TableStatsCreator
    {
        // player | pts |  w-l | total serve | error | eff% | ace | total receive | errors | neg% | %+ | %# | total attack | error | blocked | pts | eff% | pts% | block error | neg% | pos% | pts | defence tot | pos% 
        public BaseStatTable() { }
        public override Table process(Team team, VolleyActionSegmentSequence seq)
        {
            throw new NotImplementedException();
        }
        public override Table process(Team team, VolleyActionSequence seq)
        {
            Table table = new Table();
            TableGrid tableGrid = new TableGrid();
            int[] boldColumnsNumbers = new int[] { 0, 2, 6, 11, 17, 21 };
            for (int i = 0; i < 24; i++) // Всего 24 столбца
            {
                tableGrid.Append(new GridColumn());
            }
            table.Append(tableGrid);

            table.Append(createHeaderTableRow());
            TableRow headerRow = new TableRow();
            foreach(string header in getHeader())
            {
                TableCell cell = CreateCell(header);
                headerRow.Append(cell);
            }
            table.Append(headerRow);
            foreach(Player p in team.Players)
            {
                table.Append(createPlayerTableRow(p, seq, boldColumnsNumbers));
            }
            table.Append(createTotalTableRow(seq, boldColumnsNumbers));
            return table;
        }
        private TableRow createHeaderTableRow()
        {
            TableRow firstRow = new TableRow();

            // Первая ячейка на 1 столбец
            TableCell cell1 = CreateCell("Player", true, true);
            cell1.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 1 });
            firstRow.Append(cell1);

            // Вторая ячейка на 2 столбца
            TableCell cell2 = CreateCell("Points", true, true);
            cell2.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 2 });
            firstRow.Append(cell2);

            // Третья ячейка на 4 столбца
            TableCell cell3 = CreateCell("Serve", true, true);
            cell3.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 4 });
            firstRow.Append(cell3);

            // Четвертая ячейка на 5 столбцов
            TableCell cell4 = CreateCell("Reception", true, true);
            cell4.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 5 });
            firstRow.Append(cell4);

            // Пятая ячейка на 6 столбцов
            TableCell cell5 = CreateCell("Attack", true, true);
            cell5.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 6 });
            firstRow.Append(cell5);

            // Шестая ячейка на 4 столбца
            TableCell cell6 = CreateCell("Block", true, true);
            cell6.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 4 });
            firstRow.Append(cell6);

            // Седьмая ячейка на 2 столбца
            TableCell cell7 = CreateCell("Defence", true, true);
            cell7.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 2 });
            firstRow.Append(cell7);
            return firstRow;
        }
        private TableRow createPlayerTableRow(Player p, VolleyActionSequence seq, int[] boldColumnsNumbers)
        {
            string[] strs = getPlayerStats(seq, p);
            TableRow row = new TableRow();
            for(int i = 0;i < strs.Length; i++)
            {
                row.Append(CreateCell(strs[i], boldColumnsNumbers.Contains(i), false));
            }
            return row;
        }
        private TableRow createTotalTableRow(VolleyActionSequence seq, int[] boldColumnsNumbers)
        {
            string[] strs = getTotalStats(seq);
            TableRow row = new TableRow();
            for (int i = 0; i < strs.Length; i++)
            {
                row.Append(CreateCell(strs[i], boldColumnsNumbers.Contains(i), false));
            }
            return row;
        }
        
        private string[] getPlayerStats(VolleyActionSequence seq, Player p) 
        {
            return getRowStats(seq.SelectActionsByCondition((pl) => { return pl.Player == p; }), $"#{p.Number} {p.Surname}");
        }
        private string[] getTotalStats(VolleyActionSequence seq)
        {
            return getRowStats(seq, "Total");
        }
        private string[] getHeader()
        {
            return new string[] {"", "Pts", "W-L", "Tot", "Err", "Eff%", "Ace", "Tot", "Err", "Neg%", "Pos%", "Per%", "Tot", "Err", "Bk", "Pts", "Eff%", "Pts%", "Err", "Neg%", "Pos%", "Pts", "Tot", "Pos%" };
        }
        private string[] getRowStats(VolleyActionSequence seq, string rowName)
        {
            List<string> result = new List<string>();
            
            Func<VolleyActionSequence, List<string>>[] funcs = new Func<VolleyActionSequence, List<string>>[] { getPtsStats, getServeStats, getReceptionStats, getAttackStats, getBlockStats, getDefenceStats  };
            result.Add(rowName);
            foreach(var func in funcs)
            {
                List<string> stats = func(seq);
                result.AddRange(stats);
            }
            return result.ToArray();    
        }
        private List<string> getPtsStats(VolleyActionSequence seq)
        {
            List<string> res = new List<string>();
            int[] values = seq.CountActionsByCondition(wonAction, lostAction);
            res.Add(convertValuesToString(values[0]));
            res.Add(values[1].ToString());
            return res;
        }
        private bool wonAction(PlayerAction pl)
        {
            switch (pl.ActionType)
            {
                case VolleyActionType.Serve:
                case VolleyActionType.Attack:
                case VolleyActionType.Block:
                    return pl.GetQuality() == 6;
            }
            return false;
        }
        private bool lostAction(PlayerAction pl)
        {
            switch (pl.ActionType)
            {
                case VolleyActionType.Serve:
                case VolleyActionType.Reception:
                case VolleyActionType.Block:
                case VolleyActionType.Set:
                case VolleyActionType.FreeBall:
                case VolleyActionType.Transfer:
                    return pl.GetQuality() == 1;
                case VolleyActionType.Attack:
                    return pl.GetQuality() == 1 || pl.GetQuality() == 2;
            }
            return false;
        }
        private List<string> getServeStats(VolleyActionSequence seq)
        {
            List<string> res = new List<string>();
            VolleyActionSequence serves = seq.SelectActionsByCondition((pl) => { return pl.ActionType == VolleyActionType.Serve; });
            Func<PlayerAction, bool>[] funcs = new Func<PlayerAction, bool>[3];
            funcs[0] = (pl) => { return pl.GetQuality() == 1; };
            funcs[1] = (pl) => { return pl.GetQuality() >= 5; };
            funcs[2] = (pl) => { return pl.GetQuality() == 6; };
            int count = serves.Count;
            int[] values = serves.CountActionsByCondition(funcs);
            res.Add(convertValuesToString(count));
            res.Add(convertValuesToString(values[0]));
            res.Add(convertValuesToPercentString(values[1], count));
            res.Add(convertValuesToString(values[2]));
            return res;
        }
        private List<string> getReceptionStats(VolleyActionSequence seq)
        {
            List<string> res = new List<string>();
            VolleyActionSequence receptions = seq.SelectActionsByCondition((pl) => { return pl.ActionType == VolleyActionType.Reception; });
            Func<PlayerAction, bool>[] funcs = new Func<PlayerAction, bool>[4];
            funcs[0] = (pl) => { return pl.GetQuality() == 1; };
            funcs[1] = (pl) => { return pl.GetQuality() < 4; };
            funcs[2] = (pl) => { return pl.GetQuality() >= 5; };
            funcs[3] = (pl) => { return pl.GetQuality() == 6; };
            int count = receptions.Count;
            int[] values = receptions.CountActionsByCondition(funcs);
            res.Add(convertValuesToString(count));
            res.Add(convertValuesToString(values[0]));
            res.Add(convertValuesToPercentString(values[1], count));
            res.Add(convertValuesToPercentString(values[2], count));
            res.Add(convertValuesToPercentString(values[3], count));
            return res;
        }
        private List<string> getAttackStats(VolleyActionSequence seq)
        {
            List<string> res = new List<string>();
            VolleyActionSequence attacks = seq.SelectActionsByCondition((pl) => { return pl.ActionType == VolleyActionType.Attack; });
            Func<PlayerAction, bool>[] funcs = new Func<PlayerAction, bool>[4];
            funcs[0] = (pl) => { return pl.GetQuality() == 1; }; // errors
            funcs[1] = (pl) => { return pl.GetQuality() == 2; }; // blocked
            funcs[2] = (pl) => { return pl.GetQuality() == 6; };  // pts
            funcs[3] = (pl) => { return pl.GetQuality() >= 5; }; // pts + eff
            int count = attacks.Count;
            int[] values = attacks.CountActionsByCondition(funcs);
            res.Add(convertValuesToString(count));
            res.Add(convertValuesToString(values[0]));
            res.Add(convertValuesToString(values[1]));
            res.Add(convertValuesToString(values[2]));
            res.Add(convertValuesToPercentString(values[3], count));
            res.Add(convertValuesToPercentString(values[2], count));
            return res;
        }
        private List<string> getBlockStats(VolleyActionSequence seq)
        {
            List<string> res = new List<string>();
            VolleyActionSequence blocks = seq.SelectActionsByCondition((pl) => { return pl.ActionType == VolleyActionType.Block; });
            Func<PlayerAction, bool>[] funcs = new Func<PlayerAction, bool>[4];
            funcs[0] = (pl) => { return pl.GetQuality() == 1; }; // errors
            funcs[1] = (pl) => { return pl.GetQuality() < 4; }; // neg
            funcs[2] = (pl) => { return pl.GetQuality() >= 4; };  // pos
            funcs[3] = (pl) => { return pl.GetQuality() == 6; }; // pts
            int count = blocks.Count;
            int[] values = blocks.CountActionsByCondition(funcs);
            res.Add(convertValuesToString(values[0]));
            res.Add(convertValuesToPercentString(values[1], count));
            res.Add(convertValuesToPercentString(values[2], count));
            res.Add(convertValuesToString(values[3]));
            return res;
        }
        private List<string> getDefenceStats(VolleyActionSequence seq)
        {
            List<string> res = new List<string>();
            VolleyActionSequence defences = seq.SelectActionsByCondition((pl) => { return pl.ActionType == VolleyActionType.Defence; });
            Func<PlayerAction, bool>[] funcs = new Func<PlayerAction, bool>[1];
            funcs[0] = (pl) => { return pl.GetQuality() > 1; }; // pos
            int count = defences.Count;
            int[] values = defences.CountActionsByCondition(funcs);
            res.Add(convertValuesToString(count));
            res.Add(convertValuesToPercentString(values[0], count));
            return res;
        }

        
    }

    public class SetStatTable : TableStatsCreator
    {
        public SetStatTable() { }
        public override Table process(Team team, VolleyActionSegmentSequence seq)
        {
            throw new NotImplementedException();
        }
        public override Table process(Team team, VolleyActionSequence seq)
        {
            return null;
        }
        public Table[] getReceptionZoneDistributionStatistics(VolleyActionSegmentSequence sequence)
        {
            Table[] result = new Table[6];
            for(int i = 1; i <= 6; i++)
            {
                result[i - 1] = createReceptionZoneDistributionTable(sequence, i);
            }

            return result;
        }
        private Table createReceptionZoneDistributionTable(VolleyActionSegmentSequence sequence, int zone)
        {
            VolleyActionSegmentSequence RecepSetSegments = sequence.SelectByCondition((seg) => { return seg.ContainsActionType(VolleyActionType.Reception) && seg.ContainsActionType(VolleyActionType.Set); });
            VolleyActionSegmentSequence seq = RecepSetSegments.SelectByCondition((seg) =>
            {
                PlayerAction set = seg.getByActionType(VolleyActionType.Set);
                if (set == null) return false;
                return (int)set[set.MetricTypes[1]].Value == zone;
            }); //segments where setter arrangement position = zone
            Table table = new Table();

            TableGrid tableGrid = new TableGrid();
            for (int i = 0; i < 3; i++) // 3 columns
            {
                tableGrid.Append(new GridColumn());
            }
            table.Append(tableGrid);
            TableRow firstRow = new TableRow();
            for (int i = 4; i >=2; i--)
            {
                string str = getDirectionZoneData(seq, i);
                firstRow.Append(CreateCell(str));
            }

            TableRow secondRow = new TableRow();
            for (int i = 5; i <= 7; i++)
            {
                int zn = i;
                if (i == 7) zn = 1;
                string str = getDirectionZoneData(seq, zn );
                secondRow.Append(CreateCell(str));
            }
            table.Append(firstRow);
            table.Append(secondRow);
            return table;
        }
        private string getDirectionZoneData(VolleyActionSegmentSequence segments, int zone)
        {
            string res = "";
            VolleyActionSegmentSequence setsDirectedToZone = segments.SelectByCondition((seg) => {
                return (int)seg.getByActionType(VolleyActionType.Set)["Direction"].Value == zone;
            });
            VolleyActionSequence setSeq = setsDirectedToZone.ConvertToActionSequence().SelectActionsByCondition((act) => { return act.VolleyActionType == VolleyActionType.Set; });
            VolleyActionSequence attackSeq = setsDirectedToZone.ConvertToActionSequence().SelectActionsByCondition((act) => { return act.VolleyActionType == VolleyActionType.Attack; });
            int directedCount = setSeq.Count;
            int realizedAttacks = attackSeq.CountActionsByCondition((act) => { return act.GetQuality() == 6; })[0];

            double blockers = setSeq.Sum((act) => Convert.ToDouble(act["BlockersCount"].getShortString()));
            if (setSeq.Count > 0) blockers /= setSeq.Count;

            res = $@"{convertValuesToString(directedCount)}({convertValuesToPercentString(directedCount, segments.Count)})
{convertValuesToPercentString(realizedAttacks, attackSeq.Count)}
{blockers.ToString("F")}";
            return res;
        }

        public Table getBlockersDistributionByReceptionQuality(VolleyActionSegmentSequence seq, Game game)
        {
            VolleyActionSegmentSequence recepSetSegments = seq.SelectByCondition((seg) => { return seg.ContainsActionType(VolleyActionType.Reception) && seg.ContainsActionType(VolleyActionType.Set); });
            VolleyActionSequence sequence = recepSetSegments.ConvertToActionSequence().SelectActionsByCondition((act) => { return act.VolleyActionType == VolleyActionType.Set; });
            MetricType mt = game.ActionsMetricTypes.getByName(VolleyActionType.Set, "BlockersCount");
            Table table = new Table();
            TableGrid tableGrid = new TableGrid();
            for (int i = 0; i < 1 + mt.AcceptableValues.Count; i++) // Всего 24 столбца
            {
                tableGrid.Append(new GridColumn());
            }
            TableRow header = new TableRow();
            header.Append(CreateCell(""));
            string[] names = mt.AcceptableValuesNames.Values.ToArray();
            for(int i= 0;i < mt.AcceptableValues.Count; i++)
            {
                header.Append(CreateCell(names[i]));
            }
            table.Append(header);
            table.Append(getDistributionByReceptionQuality(sequence, 6));
            table.Append(getDistributionByReceptionQuality(sequence, 5));
            table.Append(getDistributionByReceptionQuality(sequence, 4));
            table.Append(getDistributionByReceptionQuality(sequence, 6, 5, 4));
            table.Append(getDistributionByReceptionQuality(sequence, 3,2));
            return table;
        }
        private TableRow getDistributionByReceptionQuality(VolleyActionSequence seq, params int[] receptionQuality)
        {
            VolleyActionSequence sequence = seq.SelectActionsByCondition((act) => { return receptionQuality.Contains((int)((PlayerAction)act)["ReceptionQuality"].Value); });
            TableRow row = new TableRow();
            List<Func<PlayerAction, bool>> funcs = new List<Func<PlayerAction, bool>>();
            MetricType metricType = sequence[0]["BlockersCount"].MetricType;
            MetricType qual = sequence[0]["Quality"].MetricType;
            foreach(int value in metricType.AcceptableValues)
            {
                funcs.Add((act) => { return (int)((PlayerAction)act)["BlockersCount"].Value == value; });
            }
            int[] values = sequence.CountActionsByCondition(funcs.ToArray());
            string first = "";
            foreach(int i in receptionQuality)
            {
                first += qual.AcceptableValuesNames[i];
            }
            first += $"({sequence.Count})";
            row.Append(CreateCell(first));
            for(int i= 0; i  < values.Length; i++)
            {
                row.Append(CreateCell($"{convertValuesToString(values[i])}({convertValuesToPercentString(values[i], sequence.Count)})"));
            }
            return row;
        }
    }
}
