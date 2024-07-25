using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
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
        public virtual Action GenerateAction() => null;

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
        public VolleyActionType ActionType
        {
            get { return _actionType; }
        }
        public Metric[] Metrics
        {
            get { return _metrics; }
        }

        public override Action GenerateAction()
        {
            return new PlayerAction(_player, _actionType, _metrics);
        }

    }
    public class OpponentActionTextRepresentation:ActionTextRepresentation
    {
        VolleyActionType _actionType;
        public OpponentActionTextRepresentation(VolleyActionType at) : base(ActionAuthorType.OpponentTeam) 
        {
            _actionType = at;
        }
        public override Action GenerateAction()
        {
            return new OpponentAction(_actionType);
        }
    }
    public class JudgeActionTextRepresentation :ActionTextRepresentation
    {
        VolleyActionType _actionType;
        public JudgeActionTextRepresentation(VolleyActionType at):base(ActionAuthorType.Judge) 
        {
            _actionType=at;
        }
        public override Action GenerateAction()
        {
            return new JudgeAction(_actionType);
        }
    }
    public class CoachActionTextRepresentation : ActionTextRepresentation
    {
        VolleyActionType _actionType;
        Player p1, p2; //for changes
        public CoachActionTextRepresentation(VolleyActionType actionType) : base(ActionAuthorType.Coach)
        {
            _actionType = actionType;
            p1 = p2 = null;
        }
        public CoachActionTextRepresentation(VolleyActionType actionType, Player p1, Player p2) : base(ActionAuthorType.Coach)
        {
            _actionType = actionType;
            if (actionType == VolleyActionType.Change)
            {
                this.p1 = p1;
                this.p2 = p2;
            }
        }

        public override Action GenerateAction()
        {
            return new CoachAction(_actionType, p1, p2);
        }

    }
}
