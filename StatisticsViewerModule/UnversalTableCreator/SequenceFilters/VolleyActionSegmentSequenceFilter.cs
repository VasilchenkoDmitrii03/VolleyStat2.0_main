using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionsLib;

namespace StatisticsViewerModule.UnversalTableCreator.SequenceFilters
{

    public class VolleyActionSequenceFilterManager
    {
        List<VolleyActionFilter> _actionFilters;
        List<VolleyActionSegmentFilter> _segmentFilters;

        public VolleyActionSequenceFilterManager()
        {
            _actionFilters = new List<VolleyActionFilter>();
            _segmentFilters = new List<VolleyActionSegmentFilter>();
        }

        public void Add(VolleyActionFilter filter)
        {
            this._actionFilters.Add(filter);
        }
        public void Add(VolleyActionSegmentFilter filter)
        {
            this._segmentFilters.Add(filter);
        }

        public bool ApplyFilters(VolleyActionSegment seg)
        {
            foreach(VolleyActionSegmentFilter filter in this._segmentFilters)
            {
                if(!filter.ApplyFilters(seg)) return false;
            }
            return true;
        }
        public bool ApplyFilters(PlayerAction act)
        {
            foreach(VolleyActionFilter filter in this._actionFilters)
            {
                if(!filter.ApplyFilter((PlayerAction)act)) return false;   
            }
            return true;
        }

        public VolleyActionSequence ApplyFilters(VolleyActionSequence seq)
        {
            VolleyActionSequence res = new VolleyActionSequence();
            foreach(PlayerAction act in seq)
            {
                if(ApplyFilters(act)) res.Add(act);
            }
            return res;
        }
        public VolleyActionSegmentSequence ApplyFilters(VolleyActionSegmentSequence seq)
        {
            VolleyActionSegmentSequence res = new VolleyActionSegmentSequence();
            foreach (VolleyActionSegment seg in seq)
            {
                if (ApplyFilters(seg)) res.Add(seg);
            }
            return res;
        }
    }


    public class VolleyActionSegmentFilter
    {
        public VolleyActionSegmentFilter() 
        {
            _filterData = new Dictionary<VolleyActionType, Dictionary<MetricType, int[]>>();
            _playerData = new Dictionary<VolleyActionType, List<Player>>();
        }
        Dictionary<VolleyActionType, Dictionary<MetricType, int[]>> _filterData;
        Dictionary<VolleyActionType, List<Player>> _playerData;

        public bool AddPlayerFilter(VolleyActionType type, List<Player> players)
        {
            if (!_playerData.ContainsKey(type))
            {
                _playerData[type] = players;
            }
            else
            {
                return false;
            }
            return true;
        }
        public bool AddMetricFilter(VolleyActionType actType, MetricType mtType, int[] values)
        {
            if (!_filterData.ContainsKey(actType))
            {
                _filterData[actType] = new Dictionary<MetricType, int[]>();
            }
            if (!_filterData[actType].ContainsKey(mtType))
            {
                _filterData[actType].Add(mtType, values);
            }
            else
            {
                return false;
            }
            return true;
        }

        public bool ApplyFilters(VolleyActionSegment seg)
        {
            //Players filters
            if(_playerData.Count != 0)
            {
                if (!ContainsActionType(seg, _playerData.Keys.ToArray())) return false;
                foreach(VolleyActionType actType in _playerData.Keys)
                {
                    if(!ActionPlayerCondition(seg, actType, _playerData[actType].ToArray())) return false;
                }
            }
            //MetricType filters
            if (_filterData.Keys.Count != 0)
            {
                if (!ContainsActionType(seg, _filterData.Keys.ToArray())) return false;
                foreach (VolleyActionType actType in _filterData.Keys)
                {
                    foreach (MetricType mtType in _filterData[actType].Keys)
                    {
                        if (!ActionMetricValueIn(seg, actType, mtType, _filterData[actType][mtType])) return false;
                    }
                }
            }
            return true;
        }

        public bool ContainsActionType(VolleyActionSegment segment, VolleyActionType[] volleyActionTypes)
        {
            foreach (VolleyActionType type in volleyActionTypes)
            {
                if (!segment.ContainsActionType(type)) return false;
            }
            return true;
        }
        public bool ActionMetricValueIn(VolleyActionSegment segment, VolleyActionType type, MetricType metricType, int[] values)
        {
            if (!segment.ContainsActionType(type)) return false;
            PlayerAction act = segment.getByActionType(type);
            object val = act[metricType].Value;
            return values.Contains((int)val);
        }
        public bool ActionMetricValueIn(VolleyActionSegment segment, VolleyActionType type, MetricType[] metricType, int[][] values)
        {
            int i = 0;
            foreach (MetricType mt in metricType)
            {
                if (!ActionMetricValueIn(segment, type, mt, values[i])) return false;
                i++;
            }
            return true;
        }
        public bool ScoreCondition()
        {
            return true;
        }
        public bool PhaseCondition()
        {
            return true;
        }
        public bool ActionPlayerCondition(VolleyActionSegment segment, VolleyActionType type, Player[] players)
        {
            if (!segment.ContainsActionType(type)) return false;
            PlayerAction act = segment.getByActionType((VolleyActionType)type);
            return players.Contains(act.Player);
        }
    }
    public class VolleyActionFilter
    {
        public VolleyActionFilter() {
            _playerFilterData = new List<Player>(); ;
            _filterData = new Dictionary<VolleyActionType, Dictionary<MetricType, int[]>>(); ;
        }
        Dictionary<VolleyActionType, Dictionary<MetricType, int[]>> _filterData;
        List<Player> _playerFilterData;

        public bool AddPlayerFilter(List<Player> players)
        {
            if(players == null || players.Count == 0) return false;
            _playerFilterData = players;
            return true;
        }
        public bool AddMetricValueFilter(VolleyActionType actType, MetricType mtType, int[] values)
        {
            if (!_filterData.ContainsKey(actType)) // if do not exist
            {
                _filterData.Add(actType, new Dictionary<MetricType, int[]>());
            }
            if (!_filterData[actType].ContainsKey(mtType))
            {
                _filterData[actType].Add(mtType, values);
            }
            else
            {
                return false;
            }
            return true;
        }


        public bool ApplyFilter(PlayerAction act)
        {
            //Player Filter
            if(_playerFilterData.Count != 0)
            {
                if (!PlayerCondition(act, _playerFilterData)) return false;
            }
            //MetricType filters
            if(_filterData.Keys.Count != 0)
            {
                if (!ActionTypeCondition(act, _filterData.Keys.ToArray())) ;
                foreach(VolleyActionType actType in _filterData.Keys)
                {
                    foreach(MetricType mtType in _filterData[actType].Keys)
                    {

                        if (!MetricValueInCondition(act, mtType, _filterData[actType][mtType])) return false;
                    }
                }
            }
            return true;
        }

        public bool MetricValueInCondition(PlayerAction act, MetricType type, int[] values)
        {
            return values.Contains((int)act[type].Value);
        }
        public bool ActionTypeCondition(PlayerAction act, VolleyActionType[] types)
        {
            return types.Contains(act.VolleyActionType);
        }
        public bool MetricValueInCondition(PlayerAction act, MetricType[] types, int[][] values)
        {
            int i = 0;
            foreach(MetricType mt in types)
            {
                if(!MetricValueInCondition(act, mt, values[i])) return false;
                i++;
            }
            return true;
        }
        public bool PlayerCondition(PlayerAction act, List<Player> players)
        {
            return players.Contains(act.Player);
        }
    }
}
