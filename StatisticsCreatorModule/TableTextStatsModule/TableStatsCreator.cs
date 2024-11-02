using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ActionsLib;

namespace StatisticsCreatorModule.TableTextStatsModule
{
    public abstract class TableStatsCreator
    {
        public TableStatsCreator() { }
        public abstract Table process(VolleyActionSequence seq);
        public abstract Table process(VolleyActionSegmentSequence seq);
    }
    public class BaseStatTable : TableStatsCreator
    {
        // player | pts | break pts | w-l | total serve | error | % | ace | total receive | errors | %+ | %# | total attack | ....
        public BaseStatTable() { }
        public override Table process(VolleyActionSegmentSequence seq)
        {
            throw new NotImplementedException();
        }
        public override Table process(VolleyActionSequence seq)
        {
            Table table = new Table();

            return table;
        }
        private string[] getPlayersStats(Player p, VolleyActionSequence seq)
        {
            List<string> result = new List<string>();
            result.Add($"#{p.Number} {p.Surname}");
            return result.ToArray();    
        }
    }
}
