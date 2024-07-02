
namespace ActionsLib
{
    public class Action
    {
        string _name;
        ActionAuthorType _authorType;
        protected Metric[] _metrics;
        public Action(string name, ActionAuthorType authorType = ActionAuthorType.Undefined, Metric[] metrics = null)
        {
            _authorType = authorType;
            _name = name;
            if (metrics == null) _metrics = new Metric[0];
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
    }
    public class PlayerAction : Action
    {
        Player _player;
        VolleyActionType _volleyActionType;
        public PlayerAction(Player player, VolleyActionType actType, Metric[] metrics = null) : base(actType.ToString(), ActionAuthorType.Player, metrics)
        {
            _volleyActionType = actType;
            _player = player;
        }

        public Player Player
        {
            get { return _player; }
        }

    }
    public class OpponentAction : Action
    {
        OpponentTeamActionType _actionType;
        public OpponentAction(OpponentTeamActionType actionType) : base(actionType.ToString(), ActionAuthorType.OpponentTeam, null)
        {
            _actionType = actionType;
        }
    }
    public class JudgeAction : Action
    {
        JudgeActionType _actionType;
        public JudgeAction(JudgeActionType actionType, Metric[] metrics = null) : base(actionType.ToString(), ActionAuthorType.Judge, metrics)
        {
            _actionType = actionType;
        }
    }


    public enum ActionAuthorType
    {
        Player = 0,
        OpponentTeam = 1,
        Judge = 2,
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
        Undefined = -1
    }
    public enum OpponentTeamActionType
    {
        WonPoint = 0,
        LostPoint = 1,
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
