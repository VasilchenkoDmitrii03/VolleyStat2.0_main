
using System.Net.Http.Metrics;

namespace ActionsLib
{
    public class Action
    {
        string _name;
        ActionAuthorType _authorType;
        protected VolleyActionType _actionType;
        protected Metric[] _metrics;
        public Action(string name, ActionAuthorType authorType = ActionAuthorType.Undefined, Metric[] metrics = null)
        {
            _authorType = authorType;
            _name = name;
            if (metrics == null) _metrics = new Metric[0];
            else _metrics = metrics;
        }

        public ActionAuthorType AuthorType
        {
            get { return _authorType; }
        }
        public List<MetricType> MetricTypes
        {
            get
            {
                List<MetricType> types = new List<MetricType>();
                foreach (Metric metric in _metrics)
                {
                    if (!types.Contains(metric.MetricType)) types.Add(metric.MetricType);
                }
                return types;
            }
        }
        public Metric this[MetricType metricType]
        {
            get
            {
                for (int i = 0; i < _metrics.Length; i++)
                {
                    if (_metrics[i].MetricType == metricType) return _metrics[i];
                }
                throw new Exception($"Action {_name} does not contains metric type {metricType.Name}");
            }
            set
            {
                for (int i = 0; i < _metrics.Length; i++)
                {
                    if (_metrics[i].MetricType == metricType) _metrics[i] = value;
                }
                throw new Exception($"Action {_name} does not contains metric type {metricType.Name}");
            }
        }
        public virtual string ExtendedString
        {
            get { string res =  _authorType.ToString();
                foreach(Metric metric in _metrics)
                {
                    res += " " + metric.getShortString();
                }
                return res;
            }
        }
    }
    public class PlayerAction : Action
    {
        Player _player;
        public PlayerAction(Player player, VolleyActionType actType, Metric[] metrics = null) : base(actType.ToString(), ActionAuthorType.Player, metrics)
        {
            _actionType = actType;
            _player = player;
        }

        public Player Player
        {
            get { return _player; }
        }
        public VolleyActionType VolleyActionType
        {
            get
            {
                return _actionType;
            }
        }
        public override string ExtendedString
        {
            get
            {
                    string res = "#" + Player.Number + " " + _actionType.ToString();
                    foreach (Metric metric in _metrics)
                    {
                        res += " " + metric.getShortString();
                    }
                    return res;
            }
        }
    }
    public class OpponentAction : Action
    {
        public OpponentAction(VolleyActionType actionType) : base(actionType.ToString(), ActionAuthorType.OpponentTeam, null)
        {
            _actionType = actionType;
        }
        public override string ExtendedString
        {
            get
            {
                return "Opponent " +  _actionType.ToString();
            }
        }
    }
    public class JudgeAction : Action
    {
        public JudgeAction(VolleyActionType actionType) : base(actionType.ToString(), ActionAuthorType.Judge)
        {
            _actionType = actionType;
        }
        public override string ExtendedString
        {
            get
            {
                return "Judge " + _actionType.ToString();
            }
        }
    }
    public class CoachAction : Action
    {
        Player p1, p2;
        public CoachAction(VolleyActionType actionType, Player p1 = null, Player p2 = null) : base(actionType.ToString(), ActionAuthorType.Coach)
        {
            _actionType = actionType;
            this.p1 = p1;
            this.p2 = p2;
        }
        public override string ExtendedString
        {
            get
            {
                string res = "Coach " + _actionType.ToString();
                if (!(p1 == null || p2 == null)) res += $"#{p1.Number} #{p2.Number}";
                return res;
            }
        }
    }
    
    public static class ActionTypeConverter
    {
        public static List<VolleyActionType> getActionTypesByAuthor(ActionAuthorType actionAuthorType)
        {
            switch (actionAuthorType)
            {
                case ActionAuthorType.Judge:
                    return new List<VolleyActionType>() {VolleyActionType.JudgeMistakeWon, VolleyActionType.JudgeMistakeLost, VolleyActionType.DisputableBall };
                case ActionAuthorType.Coach:
                    return new List<VolleyActionType>() {VolleyActionType.TimeOut, VolleyActionType.Change };
                case ActionAuthorType.OpponentTeam:
                    return new List<VolleyActionType>() { VolleyActionType.OpponentError, VolleyActionType.OpponentPoint };
                default:
                    return new List<VolleyActionType>() { VolleyActionType.Serve, VolleyActionType.Reception, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.Transfer, VolleyActionType.Block, VolleyActionType.Defence, VolleyActionType.FreeBall };
            }
        }
        public static ActionAuthorType getActionAuthor(VolleyActionType vat)
        {
            List<ActionAuthorType> types = new List<ActionAuthorType>() { ActionAuthorType.Player, ActionAuthorType.Coach, ActionAuthorType.Judge, ActionAuthorType.OpponentTeam };
            foreach(ActionAuthorType aat in types)
            {
                if (getActionTypesByAuthor(aat).Contains(vat)) return aat;
            }
            return ActionAuthorType.Undefined;
        }
    }

    public enum ActionAuthorType
    {
        Player = 0,
        OpponentTeam = 1,
        Judge = 2,
        Coach = 3,
        Undefined = -1
    }
    public enum VolleyActionType
    {
        Serve = 0,
        Reception = 1,
        Set = 2,
        Attack = 3,
        Block = 4,
        Defence = 5,
        Transfer = 6,
        FreeBall = 7,
        OpponentError = 8, 
        OpponentPoint = 9,
        JudgeMistakeWon = 10,
        JudgeMistakeLost = 11,
        DisputableBall = 12,
        TimeOut = 13,
        Change = 14,
        Undefined = -1
    }
    public enum JudgeActionType
    {
        MistakeInOurFavor = 0,
        MistakeInFavorOfOpponent = 1,
        CardGift = 2,
        Undefined = -1
    }
}
