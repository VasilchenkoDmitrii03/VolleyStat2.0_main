using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionsLib.TextRepresentation
{
    public class ActionTextRepresentation
    {
        ActionAuthorType authorType;
        public ActionTextRepresentation(ActionAuthorType authorType)
        {
            this.authorType = authorType;
        }

    }
    public class PlayerActionTextRepresentation : ActionTextRepresentation
    {
        VolleyActionType _actionType;
        MetricTypeList _metricTypeList;
        Metric[] _metrics;
        string[] _shortStrings;
        Player _player;
        public PlayerActionTextRepresentation(VolleyActionType type,  MetricTypeList metricTypeList): base(ActionAuthorType.Player)
        {
            this._actionType = type;
            this._metricTypeList = metricTypeList;
            _metrics = new Metric[metricTypeList.Count()];
            _shortStrings = new string[metricTypeList.Count()];
        }
        public void SetMetricByObject(MetricType mType, object value)
        {
            int index = this._metricTypeList.IndexOf(mType);
            if(index == -1) { throw new Exception($"No {mType.ToString()} metric in action {this._actionType}"); }
            _metrics[index] = new Metric(mType, value);
            _shortStrings[index] = _metrics[index].getShortString();
        }
        public void SetMetricByShortString(MetricType mType, string shortString)
        {
            object obj = mType.getObjectByShortString(shortString);
            SetMetricByObject(mType, obj);
        }
        public void SetPlayer(Player player)
        {
            this._player = player;
        }
        public string LongStringFormat()
        {
            string res = $"{_player.Number}:{_actionType}";
            foreach(string str in _shortStrings)
            {
                res += ":" + str;
            }
            return res;
        }
    }
}
