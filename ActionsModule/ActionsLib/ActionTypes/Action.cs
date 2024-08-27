using ActionsLib.ActionTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;

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
        public VolleyActionType ActionType
        {
            get { return _actionType; }
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
        public virtual string Save()
        {
            string res = _authorType.ToString();
            foreach (Metric metric in _metrics)
            {
                res += ";" + metric.getShortString();
            }
            return res;
        }
        public virtual void Save(StreamWriter sw)
        {
            sw.WriteLine(JsonSerializer.Serialize(_authorType));
            sw.WriteLine(JsonSerializer.Serialize(_metrics));
        }
        
    }
    public class Point
    {
        double x, y;
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public double X
        {
            get => this.x;
            set => this.x = value;
        }
        public double Y
        {
            get => this.y;
            set => this.y = value;
        }
        public string Save()
        {
            return $"{x}|{y}";
        }
        public void Save(StreamWriter sw)
        {
            sw.WriteLine(Save());
        }
        public static Point Load(string str)
        {
            string[] strs = str.Split('|');
            double x = double.Parse(strs[0]);
            double y = double.Parse(strs[1]);
            return new Point(x, y);
        }
        public static Point Load(StreamReader sr)
        {
            return Load(sr.ReadLine());
        }
    }
    public class PlayerAction : Action
    {

        Player _player;
        double _timeCode = 0;
        Point[] _points;
        public PlayerAction(Player player, VolleyActionType actType, Metric[] metrics = null) : base(actType.ToString(), ActionAuthorType.Player, metrics)
        {
            _actionType = actType;
            _player = player;
        }
        public double TimeCode
        {
            get { return _timeCode; }
            set { if (value >= 0) _timeCode = value;
                else _timeCode = 0;
            }
        }
        public Point[] Points
        {
            get { return _points; }
            set { _points = value; }
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
                    return res + $" {TimeCode}";
            }
        }
        public int GetQuality()
        {
            return (int)_metrics[0].Value;
        }
        public override string Save()
        {
            string res =  $"#{_player.Number};{_actionType.ToString()}";
            foreach(Metric m in _metrics)
            {
                res += $";{m.getShortString()}";
            }
            res += $";{_timeCode}";
            foreach (Point p in _points)
            {
                res += ";" + p.Save();
            }
            return res;
        }
        public override void Save(StreamWriter sw)
        {
            sw.WriteLine(Save());
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
        public override string Save()
        {
            return "Opponent;" + _actionType.ToString();
        }
        public override void Save(StreamWriter sw)
        {
            sw.WriteLine(Save());
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
        public override string Save()
        {
            return "Judge;" + _actionType.ToString();
        }
        public override void Save(StreamWriter sw)
        {
            sw.WriteLine(Save());
        }
    }
    public class CoachAction : Action
    {
        List<Player> _players;
        public CoachAction(VolleyActionType actionType, List<Player> players = null) : base(actionType.ToString(), ActionAuthorType.Coach)
        {
            _actionType = actionType;
            if(players == null) _players = new List<Player>();
            else _players = players; ;
        }
        public List<Player> Players
        {
            get { return _players; }
        }
        public override string ExtendedString
        {
            get
            {
                string res = "Coach " + _actionType.ToString();
                for(int i = 0;i < _players.Count; i++)
                {
                    res += $" #{_players[i].Number}";
                }
                return res;
            }
        }
        public override string Save()
        {
            string res = "Coach;" + _actionType.ToString();
            for (int i = 0; i < _players.Count; i++)
            {
                res += $";#{_players[i].Number}";
            }
            return res;
        }
        public override void Save(StreamWriter sw)
        {
            sw.WriteLine(Save());
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
                    return new List<VolleyActionType>() {VolleyActionType.TimeOut, VolleyActionType.Change, VolleyActionType.StartArrangment };
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
    public static class ActionLoader
    {
        public static Team currentTeam;
        public static ActionsMetricTypes ActionsMetricTypes;
        public static Action Load(string str)
        {
            string[] stst = str.Split(';');
            switch (stst[0])
            {
                case "Coach":
                    return LoadCoac(stst);
                    break;
                case "Judge":
                    return LoadJud(stst);
                    break;
                case "Opponent":
                    return LoadOpp(stst);
                    break;
                default:
                    if (stst[0][0] == '#')
                    {
                        return LoadPl(stst);
                    }
                    return null;
            }
            throw new Exception($"Can't load {str}");

        }
        public static Action Load(StreamReader sr)
        {
            return Load(sr.ReadLine());
        }
        public static CoachAction LoadCoac(string[] strs)
        {
            try
            {
                VolleyActionType actType = VolleyActionType.Undefined;
                foreach (VolleyActionType act in ActionTypeConverter.getActionTypesByAuthor(ActionAuthorType.Coach))
                {
                    if (strs[1] == act.ToString())
                    {
                        actType = act;
                        break;
                    }
                }
                if (actType == VolleyActionType.Undefined) throw new Exception($"No such action Type for coach {strs[1]}");
                int additionalParamsCount = 0;
                switch (actType)
                {
                    case VolleyActionType.Change:
                        additionalParamsCount = 2; break;
                    case VolleyActionType.StartArrangment:
                        additionalParamsCount = 6; break;
                } //argrument counts
                Player[] players = new Player[additionalParamsCount];
                for (int i = 2; i < 2 + additionalParamsCount; i++)
                {
                    int number = Convert.ToInt32(strs[i].Substring(1));
                    players[i - 2] = currentTeam.GetPlayerByNumber(number);
                }
                return new CoachAction(actType, new List<Player>(players));
            }
            catch { }
            return null;
        }
        public static OpponentAction LoadOpp(string[] strs)
        {
            try
            {
                foreach (VolleyActionType act in ActionTypeConverter.getActionTypesByAuthor(ActionAuthorType.OpponentTeam))
                {
                    if (strs[1] == act.ToString()) return new OpponentAction(act);
                }
                throw new Exception($"Can't load Opponent Action {strs[1]}");
            }
            catch { }
            return null;
        }
        public static JudgeAction LoadJud(string[] strs)
        {
            try
            {
                foreach (VolleyActionType act in ActionTypeConverter.getActionTypesByAuthor(ActionAuthorType.Judge))
                {
                    if (strs[1] == act.ToString()) return new JudgeAction(act);
                }
                throw new Exception($"Can't load Jude Action {strs[1]}");
            }
            catch { }
            return null;
        }
        public static PlayerAction LoadPl(string[] strs)
        {
            try
            {
                Player player = currentTeam.GetPlayerByNumber(Convert.ToInt32(strs[0].Substring(1)));
                VolleyActionType actType = VolleyActionType.Undefined;
                foreach (VolleyActionType act in ActionTypeConverter.getActionTypesByAuthor(ActionAuthorType.Player))
                {
                    if (strs[1] == act.ToString())
                    {
                        actType = act;
                        break;
                    }
                }
                if (actType == VolleyActionType.Undefined) throw new Exception($"No such action Type for player {strs[1]}");
                int countOfMetrics = ActionsMetricTypes[actType].Count();
                Metric[] metrics = new Metric[countOfMetrics];
                int index = 2;
                for (int i = 0; i < countOfMetrics; i++, index++)
                {
                    metrics[i] = new Metric(ActionsMetricTypes[actType][i], ActionsMetricTypes[actType][i].getObjectByShortString(strs[index]));
                }
                double timeCode = Convert.ToDouble(strs[index++]);
                List<Point> points = new List<Point>();
                if (strs.Length > index)
                {
                    for (; index < strs.Length; index++)
                    {
                        points.Add(Point.Load(strs[index]));
                    }
                }
                return new PlayerAction(player, actType, metrics) { TimeCode = timeCode, Points = points.ToArray() };

            }
            catch { }
            return null;
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
        StartArrangment = 15,
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
