using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionsLib;
using ActionsLib.ActionTypes;

namespace StatisticsCreatorModule.StatisticsModule
{
    internal class FiltersHolder
    {
        ActionsMetricTypes _AMT;
        Dictionary<VolleyActionType, Dictionary<MetricType, List<object>>> _data;

        public FiltersHolder(ActionsMetricTypes AMT, VolleyActionType[] actiontypes)
        {
            _AMT = AMT;
            _data = new Dictionary<VolleyActionType, Dictionary<MetricType, List<object>>>();
            foreach (var actiontype in actiontypes)
            {
                _data.Add(actiontype, new Dictionary<MetricType, List<object>>());
                fillActionMetricDict(actiontype);

            }
        }
        private void fillActionMetricDict(VolleyActionType actType)
        {
            foreach (MetricType mt in _AMT[actType])
            {
                fillMetricDict(actType, mt);
            }
        }
        private void fillMetricDict(VolleyActionType act, MetricType type)
        {
            List<object> data = new List<object>();
            foreach (var item in type.AcceptableValues) data.Add(item);
            _data[act].Add(type, data);
        }

        public void update(VolleyActionType actType, MetricType mt, List<object> list)
        {
            _data[actType][mt] = list;
        }
        public void update(VolleyActionType actType, MetricType mt, List<string> list)
        {
            List<object> values = new List<object>();
            foreach (string str in list)
            {
                values.Add(mt.getObjectByLargeName(str));
            }
            update(actType, mt, values);
        }
        public void Clear()
        {
            this._data.Clear();
        }
        public void update(VolleyActionType actType, List<MetricTypeStatSelector> boxes)
        {
            this._data.Add(actType, new Dictionary<MetricType, List<object>>());
            foreach (MetricTypeStatSelector box in boxes)
            {
                update(actType, box.MetricType, box.SelectedOptions());
            }
        }
        public VolleyActionSequence ProcessSequence(VolleyActionSequence seq)
        {
            VolleyActionSequence res = new VolleyActionSequence();
            foreach(var i in seq)
            {
                if(i.AuthorType == ActionAuthorType.Player && _data.ContainsKey(i.ActionType)&& checkCondition((PlayerAction)i))
                {
                    res.Add(i);
                }
            }
            return res;
        }
        private bool checkCondition(PlayerAction playerAction)
        {
            bool res = true;
            foreach(MetricType mt in this._AMT[playerAction.ActionType])
            {
                res = res && (_data[playerAction.ActionType][mt].Contains(playerAction[mt].Value));
            }
            return res;
        }
    }
    public class PlayersFiltersHolder
    {
        Team _team;
        List<string> _selected;
        public PlayersFiltersHolder(Team team) 
        { _team = team;
            _selected = new List<string>();
        }
        public void update(List<string> selected)
        {
            _selected = selected;
        }
        public void clear()
        {
            _selected = new List<string>();
        }
        public VolleyActionSequence ProcessSequence(VolleyActionSequence seq)
        {
            VolleyActionSequence res = new VolleyActionSequence();
            foreach (var i in seq)
            {
                if (checkCondition((PlayerAction)i))
                {
                    res.Add(i);
                }
            }
            return res;
        }
        public bool checkCondition(PlayerAction playerAction)
        {
            return (_selected.Contains(playerAction.Player.Number.ToString()));
        }
    }
        


}
