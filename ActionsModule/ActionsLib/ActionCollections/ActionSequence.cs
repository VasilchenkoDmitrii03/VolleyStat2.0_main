using ActionsLib.ActionTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ActionsLib
{
    // for filters use LINQ requests
    public class ActionSequence : ObservableCollection<Action>
    {
        List<Action> _actions;

        public ActionSequence()
        {
            _actions = new List<Action>();
        }
        public ActionSequence(List<Action> actions)
        {
            _actions = new List<Action>(_actions);
        }
        public ActionSequence(Action[] actions)
        {
            _actions = new List<Action>(_actions);
        }
        public ActionSequence(ActionSequence seq)
        {
            _actions = new List<Action>(seq._actions);
        }

        public ActionSequence this[ActionAuthorType type]
        {
            get
            {
                ActionSequence seq = new ActionSequence();
                foreach (Action action in this)
                {
                    if (action.AuthorType == type) seq.Add(action);
                }
                return seq;
            }
        }
        public IDisposable Subscribe(IObserver<Action> observer)
        {
            throw new NotImplementedException();
        }

    }

    public class VolleyActionSequence : ObservableCollection<Action>
    {

        public VolleyActionSequence()
        {
        }
        public VolleyActionSequence(VolleyActionSequence seq)
        {
            Add(seq);
        }
        public IDisposable Subscribe(IObserver<Action> observer)
        {
            throw new NotImplementedException();
        }
        public void Add(VolleyActionSequence seq)
        {
            foreach (Action act in seq)
            {
                this.Add(act);
            }
        }
       
        public static VolleyActionSequence operator+(VolleyActionSequence left,  VolleyActionSequence right)
        {
            VolleyActionSequence res = new VolleyActionSequence(left);
            foreach(Action act in right)
            {
                res.Add(act);
            }
            return res;
        }
    
        public int[] CountActionsByCondition(params Func<PlayerAction, bool>[]  function)
        {
            int[] res = new int[function.Length];
            for (int i = 0; i < res.Length; i++) res[i] = 0;
            foreach(Action action in this)
            {
                if(action.AuthorType == ActionAuthorType.Player)
                {
                    PlayerAction pact = (PlayerAction)action;
                    for(int i = 0;i < function.Length; i++)
                    {
                        if (function[i](pact)) res[i]++;
                    }
                }
            }
            return res;
        }
        public VolleyActionSequence SelectActionsByCondition(Func<PlayerAction, bool> function)
        {
            VolleyActionSequence res = new VolleyActionSequence();
            foreach(Action act in this)
            {
                if (act.AuthorType != ActionAuthorType.Player) continue;
                if (function((PlayerAction)act)) res.Add(act);
            }
            return res;
        }


        public string Save()
        {
            string res = $"{this.Count()}";
            foreach(Action act in this)
            {
                res += "\n" + act.Save();
            }
            return res;
        }
        public static VolleyActionSequence Load(string str)
        {
            string[] strs = str.Split('\n');
            int count = Convert.ToInt32(strs[0]);
            VolleyActionSequence seq = new VolleyActionSequence();
            for(int i = 0; i < count; i++)
            {
                seq.Add(ActionLoader.Load(strs[i+1]));
            }
            return seq;
        }
        public static VolleyActionSequence Load(string[] strs)
        {
            int count = Convert.ToInt32(strs[0]);
            VolleyActionSequence seq = new VolleyActionSequence();
            for (int i = 0; i < count; i++)
            {
                seq.Add(ActionLoader.Load(strs[i + 1]));
            }
            return seq;
        }
        public void Save (StreamWriter sw)
        {
            sw.WriteLine(JsonSerializer.Serialize(this.Count()));
            foreach(Action act in this)
            {
                act.Save(sw);
            }
        }
        public static VolleyActionSequence Load(StreamReader sr)
        {
            int count = JsonSerializer.Deserialize<int>(sr.ReadLine());
            VolleyActionSequence actions = new VolleyActionSequence();
            for(int i = 0;i < count; i++)
            {
                actions.Add(ActionLoader.Load(sr.ReadLine()));
            }
            return actions;
        }
    }
    public enum ActionSegmentResult //УТОЧНИТЬ
    {
        Undefined = -1,
        NotEnded = 0,
        Won = 1,
        Lost = 2,
        Disputable = 3
    }
    public class VolleyActionSegment
    {
        public VolleyActionSequence Actions
        {
            get; set;
        }
        public ActionSegmentResult SegmentResult
        {
            get; set;
        }

        public VolleyActionSegment()
        {
            Actions = new VolleyActionSequence();
        }
        public VolleyActionSegment(VolleyActionSequence a)
        {
            Actions = new VolleyActionSequence();
            foreach (Action action in a)
            {
                Actions.Add(action);
            }
        }
        public void Add(Action action)
        {
            Actions.Add(action);
        }
        public bool ContainsActionType(VolleyActionType type)
        {
            var req = Actions.Where(act => act.ActionType == type);
            return req.Count() != 0;
        }
        public List<VolleyActionType> VolleyActionTypes()
        {
            var req = from act in Actions select act.ActionType;
            return req.ToList();
        }
        public PlayerAction getByActionType(VolleyActionType type)
        {
            if (!ContainsActionType(type)) return null;
            foreach(var act in Actions)
            {
                if (act.ActionType == type) return (PlayerAction)act;
            }
            return null;
        }
        public VolleyActionSequence ConvertToSequence()
        {
            return Actions;
        }
        public Action Last()
        {
            return Actions.Last();
        }

        public int Count
        {
            get { return Actions.Count; }
        }

        public string Save()
        {
            string res = $"{SegmentResult}";
            res += "\n" + Actions.Save();
            return res;
        }

        public static VolleyActionSegment Load(string str)
        {
            string[] strs = str.Split('\n');
            ActionSegmentResult result = ActionSegmentResult.Undefined;
            for(int i = -1; i < 4; i++)
            {
                if (strs[0] == ((ActionSegmentResult)i).ToString()) result = ((ActionSegmentResult)i);
            }
            if (result == ActionSegmentResult.Undefined) throw new Exception($"Can't load action segment result {strs[0]}");
            string newstr = "";
            for(int i= 1; i <  strs.Length; i++)
            {
                newstr += strs[i].ToString() + "\n";
            }
            VolleyActionSegment seg = new VolleyActionSegment(VolleyActionSequence.Load(newstr));
            seg.SegmentResult = result;
            return seg;
        }
        public void Save(StreamWriter sw)
        {
            sw.WriteLine(JsonSerializer.Serialize(SegmentResult));
            Actions.Save(sw);
        }
        public static VolleyActionSegment Load(StreamReader sr)
        {
            ActionSegmentResult res = JsonSerializer.Deserialize<ActionSegmentResult>(sr.ReadLine());
            VolleyActionSegment seg =  new VolleyActionSegment(VolleyActionSequence.Load(sr));
            seg.SegmentResult = res;
            return seg;
        }
    }

    public class VolleyActionSegmentSequence : ObservableCollection<VolleyActionSegment>
    {
        public VolleyActionSegmentSequence() { }
        public VolleyActionSegmentSequence(VolleyActionSegmentSequence seq) 
        {
            this.Add(seq);        
        }
        public IDisposable Subscribe(IObserver<Action> observer)
        {
            throw new NotImplementedException();
        }
        public int Length
        {
            get { return this.Count; }
        }
        public void Add(VolleyActionSegmentSequence seq)
        {
            foreach(VolleyActionSegment seg in seq)
            {
                this.Add(seg);
            }
        }
        public VolleyActionSequence ConvertToActionSequence()
        {
            VolleyActionSequence res = new VolleyActionSequence();
            foreach(VolleyActionSegment seg in this)
            {
                res.Add(seg.ConvertToSequence());
            }
            return res;
        }
        public VolleyActionSegmentSequence SelectByCondition(Func<VolleyActionSegment, bool> func)
        {
            VolleyActionSegmentSequence result = new VolleyActionSegmentSequence();
            foreach(var seg in this)
            {
                if (func(seg)) result.Add(seg);
            }
            return result;
        }

        public string Save()
        {
            string res = $"{this.Count}";
            foreach(VolleyActionSegment seg in this)
            {
                res += "\n" + seg.Save();
            }
            return res;
        }
        public static VolleyActionSegmentSequence Load(string str)
        {
            string[] strs = str.Split('\n');
            int index = 0;
            int segmentsCount = Convert.ToInt32(strs[0]);
            index++;
            VolleyActionSegmentSequence res = new VolleyActionSegmentSequence();
            for(int i= 0;i <  segmentsCount;i++)
            {
                string newstr = "";
                for(int j= index; j < strs.Length; j++)
                {
                    newstr = strs[j] + "\n";
                }
                VolleyActionSegment seg = VolleyActionSegment.Load(newstr);
                res.Add(seg);
                index += seg.Count;
            }
            return res;
        }
        public void Save(StreamWriter sw)
        {
            sw.WriteLine(JsonSerializer.Serialize(Count));
            foreach(VolleyActionSegment seg in this)
            {
                seg.Save(sw);
            }
        }
        public static VolleyActionSegmentSequence Load(StreamReader sr)
        {
            int count = JsonSerializer.Deserialize<int>(sr.ReadLine());
            VolleyActionSegmentSequence res = new VolleyActionSegmentSequence();
            for(int i = 0;i < count; i++)
            {
                res.Add(VolleyActionSegment.Load(sr));
            }
            return res;
        }
    }
    public enum RallyResult
    {
        Undefined = -1,
        Won = 0,
        Lost = 1,
        Disputable = 2
    }
    public class Rally
    {
        public RallyResult RallyResult { get; set; }
        public VolleyActionSegmentSequence Segments
        {
            get; set;
        }
        public int Length
        {
            get
            {
                return Segments.Count;
            }
        }
        public Rally()
        {
            Segments = new VolleyActionSegmentSequence();   

        }
        public Rally(VolleyActionSegmentSequence seg)
        {
            Segments = new VolleyActionSegmentSequence();
            foreach(VolleyActionSegment s in seg)
            {
                Add(s);
            }
        }
        public void Add(VolleyActionSegment seg)
        {
            Segments.Add(seg);
        }
        public void Add(VolleyActionSegmentSequence seq)
        {
            foreach(VolleyActionSegment seg in Segments)
            Segments.Add(seg);
        }
        public VolleyActionSegmentSequence ConvertToSegmentSequence()
        {
            VolleyActionSegmentSequence res = new VolleyActionSegmentSequence();
            foreach(VolleyActionSegment r in Segments)
            {
                res.Add(r);
            }
            return res;
        }
        public VolleyActionSequence ConvertToActionSequence()
        {
            return ConvertToSegmentSequence().ConvertToActionSequence();
        }
        public void UpdateRallyResult()
        {
           switch(Segments.Last().SegmentResult)
            {
                case ActionSegmentResult.Lost:
                    RallyResult = RallyResult.Lost; break;
                case ActionSegmentResult.Won:
                    RallyResult = RallyResult.Won;  break;
                case ActionSegmentResult.Undefined:
                    RallyResult = RallyResult.Undefined; break;
                case ActionSegmentResult.Disputable:
                    RallyResult = RallyResult.Disputable; break;
            }

        }
        public SegmentPhase GetRallyPhase()
        {
            Action act = Segments[0].Actions[0];
            if (act.ActionType == VolleyActionType.Serve) return SegmentPhase.Break;
            if (act.ActionType == VolleyActionType.Reception) return SegmentPhase.Recep_1;
            return SegmentPhase.Recep;
        }

        public string Save()
        {
            string res = $"{RallyResult}";
            res += "\n" + Segments.Save();
            return res;
        }
        public static Rally Load(string str)
        {
            string[] strs = str.Split('\n');
            RallyResult result = RallyResult.Undefined;
            for (int i = -1; i < 3; i++)
            {
                if (strs[0] == ((RallyResult)i).ToString()) result = ((RallyResult)i);
            }
            //if (result == RallyResult.Undefined) throw new Exception($"Can't load rally result {strs[0]}");
            string newstr = "";
            for (int i = 1; i < strs.Length; i++)
            {
                newstr += strs[i].ToString() + "\n";
            }
            VolleyActionSegmentSequence seq = VolleyActionSegmentSequence.Load(newstr);
            Rally ral = new Rally(seq);
            ral.RallyResult = result; return ral;

        }
        public void Save(StreamWriter sw)
        {
            sw.WriteLine(JsonSerializer.Serialize(RallyResult));
            Segments.Save(sw);
        }
        public static Rally Load(StreamReader sr)
        {
            RallyResult res = JsonSerializer.Deserialize<RallyResult>(sr.ReadLine());
            VolleyActionSegmentSequence seq = VolleyActionSegmentSequence.Load(sr);
            Rally ral = new Rally(seq);
            ral.RallyResult = res;
            return ral;
        }
    }
    public class RallySequence : ObservableCollection<Rally>
    {
        public RallySequence() { }
        public RallySequence(RallySequence a)
        {
            this.Add(a);
        }
        public IDisposable Subscribe(IObserver<Action> observer)
        {
            throw new NotImplementedException();
        }
        
        public void Add(RallySequence seq)
        {
            foreach(Rally r in seq)
            {
                this.Add(r);
            }
        }
        
        public VolleyActionSegmentSequence ConvertToSegmentSequence()
        {
            VolleyActionSegmentSequence res = new VolleyActionSegmentSequence();
            foreach(Rally r in this)
            {
                res.Add(r.ConvertToSegmentSequence());
            }
            return res;
        }
        public VolleyActionSequence ConvertToActionSequence()
        {
            VolleyActionSequence res = new VolleyActionSequence();
            foreach (Rally r in this)
            {
                res.Add(r.ConvertToActionSequence());
            }
            return res;
        }
        public RallySequence SelectByCondition(Func<Rally, bool> condition)
        {
            RallySequence res = new RallySequence();
            foreach (Rally r in this) if (condition(r)) { res.Add(r); }
            return res;
        }

        public string Save()
        {
            string res = $"{Count}";
            foreach(Rally r in this)
            {
                res +="\n" +  r.Save();
            }
            return res;
        }
        public static RallySequence Load(string str)
        {
            string[] strs = str.Split('\n');
            int index = 0;
            int segmentsCount = Convert.ToInt32(strs[0]);
            index++;
            RallySequence res = new RallySequence();
            for (int i = 0; i < segmentsCount; i++)
            {
                string newstr = "";
                for (int j = index; j < strs.Length; j++)
                {
                    newstr = strs[j] + "\n";
                }
                Rally seg = Rally.Load(newstr);
                res.Add(seg);
                index += seg.Length;
            }
            return res;
        }
        public void Save(StreamWriter sw)
        {
            sw.WriteLine(JsonSerializer.Serialize(Count));
            foreach(Rally r in this)
            {
                r.Save(sw);
            }
        }
        public static RallySequence Load(StreamReader sr)
        {
            RallySequence res = new RallySequence();
            int count = JsonSerializer.Deserialize<int>(sr.ReadLine());
            for(int i = 0;i < count; i++)
            {
                res.Add(Rally.Load(sr));
            }
            return res;
        }
    }
    public enum SetResult
    {
        Undefined = -1,
        NotFinished = 0,
        Won = 1,
        Lost = 2
    }
    public enum SegmentPhase
    {
        Recep_1, 
        Recep, 
        Break
    }
    public class Set
    {
        public RallySequence Rallies { get; set; }
        public SegmentPhase CurrentPhase { get; set; } = SegmentPhase.Recep;
        int _setLength;
        SetResult _setResult;
        Score _currentScore;

        public Set(int setLength)
        {
            _setLength = setLength;
            _setResult = SetResult.Undefined;
            Rallies = new RallySequence();
            _currentScore = new Score(setLength);
        }

        public int SetLength { get { return _setLength; } }
        public SetResult SetResult
        {
            get { return _setResult; }
            set {  _setResult = value; }
        }
        public Score CurrentScore
        {
            get { return _currentScore; }
            set { _currentScore = value; }
        }

        public void updateScore(Rally rally)
        {
            switch (rally.RallyResult)
            {
                case RallyResult.Won:
                    _currentScore.Left++;
                    break;
                case RallyResult.Lost:
                    _currentScore.Right++;
                    break;
                case RallyResult.Disputable:
                    break;
                case RallyResult.Undefined:
                    break;
            }
        }
        public void updatePhase(SegmentPhase phase)
        {
            CurrentPhase = phase;
        }
        private void setStartPhase()
        {
            if (CurrentPhase != SegmentPhase.Recep) return;
            int index = 0;
            for(int i = 0;i < Rallies.Count; i++)
            {
                if ((Rallies[i].Segments[0].Actions[0]).AuthorType == ActionAuthorType.Player || (Rallies[i].Segments[0].Actions[0]).AuthorType == ActionAuthorType.OpponentTeam)
                {
                    if ((Rallies[i].Segments[0].Actions[0]).AuthorType == ActionAuthorType.OpponentTeam || (Rallies[i].Segments[0].Actions[0]).ActionType == VolleyActionType.Reception)
                    {
                        CurrentPhase = SegmentPhase.Recep_1;
                    }
                    else CurrentPhase = SegmentPhase.Break;
                }
            }
        }
        public void Add(Rally rally)
        {
            Rallies.Add(rally);
            setStartPhase();
            updateScore(rally);
        }
        public SetResult isFinished()
        {
            if (_currentScore.isFinished())
            {
                _setResult = (_currentScore.Left > _currentScore.Right) ? SetResult.Won : SetResult.Lost;
            }
            return _setResult;
        }

        public VolleyActionSegmentSequence ConvertToSegmentSequence()
        {
            return Rallies.ConvertToSegmentSequence();
        }
        public VolleyActionSequence ConvertToSequence()
        {
            return Rallies.ConvertToActionSequence();
        }

        public string Save()
        {
            string res = $"{_setLength};{_setResult}";
            res += "\n" + _currentScore.Save();
            res += "\n" + Rallies.Save();
            return res;
        }
        public static Set Load(string str)
        {
            string[] strs = str.Split('\n');
            string[] tmp = strs[0].Split(';');
            int setLen = Convert.ToInt32(tmp[0]);
           SetResult result = SetResult.Undefined;
            for (int i = -1; i < 3; i++)
            {
                if (tmp[1] == ((SetResult)i).ToString()) result = ((SetResult)i);
            }
            //if (result == SetResult.Undefined) throw new Exception($"Can't load rally result {strs[1]}");
            Set set = new Set(setLen);
            string newstr = "";
            for (int i = 2; i < strs.Length; i++) newstr = strs[i] + "\n";
            RallySequence rs = RallySequence.Load(newstr);
            set.Rallies = rs;
            set._setResult = result;
            return set;
            
        }

        public void Save(StreamWriter sw)
        {
            sw.WriteLine(JsonSerializer.Serialize(_setLength));
            sw.WriteLine(JsonSerializer.Serialize(_setResult));
            sw.WriteLine(JsonSerializer.Serialize(CurrentPhase));
            CurrentScore.Save(sw);
            Rallies.Save(sw);
        }
        public static Set Load(StreamReader sr)
        {
            int len = JsonSerializer.Deserialize<int>(sr.ReadLine());
            SetResult res = JsonSerializer.Deserialize<SetResult>(sr.ReadLine());
            SegmentPhase phase= JsonSerializer.Deserialize<SegmentPhase>(sr.ReadLine());
            Score score = Score.Load(sr);
            RallySequence rs = RallySequence.Load(sr);
            Set set = new Set(len);
            set.CurrentPhase = phase;
            set.SetResult = res;
            set.CurrentScore = score;
            set.Rallies = rs;
            return set;
        }
    }
    public class Score
    {
        int _left;
        int _right;
        int _setLength;

        public Score(int setLength = 25)
        {
            _setLength = setLength;
            _right = _left = 0;
        }

        public bool isFinished()
        {
            if (Math.Abs(_left - _right) > 1 && (_left >= _setLength || _right >= _setLength))
            {
                return true;
            }
            return false;
        }
        public int Left
        {
            get { return _left; }
            set { if (value >= 0) _left = value; }
        }
        public int Right
        {
            get { return _right; }
            set { if (value >= 0) _right = value; }
        }

        public string Save()
        {
            string res = $"{_left};{_right};{_setLength}";
            return res;
        }
        public static Score Load(string str)
        {
            string[] strs = str.Split(';');
            int left = int.Parse(strs[0]);
            int right = int.Parse(strs[1]);
            int len = int.Parse(strs[2]);
            Score s = new Score(len);
            s.Left = left;
            s.Right = right;
            return s;
        }
        public void Save(StreamWriter sw)
        {
            sw.WriteLine(Save());
        }
        public static Score Load(StreamReader sr)
        {
            return Load(sr.ReadLine());
        }
    }

    public enum GameResult
    {
        Undefined = -1,
        NotFinished = 0,
        Won = 1,
        Lost = 2
    }
    public class Game
    {
        List<Set> _sets;
        int[] _setLengths;
        int _currentSetIndex;
        int _setsToWin;
        GameResult _result;
        string youtubeURL = "";
        public ActionsMetricTypes ActionsMetricTypes;
        public Team Team;

        public Game(List<int> setLengths, ActionsMetricTypes amt, Team team, int setsToWin)
        {
            _setLengths = setLengths.ToArray();
            _currentSetIndex = -1;
            _result = GameResult.NotFinished;
            _setsToWin = setsToWin;
            _sets = new List<Set>();
            ActionsMetricTypes = amt;
            Team = team;    
        }
        public Game(List<int> setLengths, ActionsMetricTypes amt, Team team, int setsToWin, string youtubeURL) : this(setLengths, amt, team, setsToWin)
        {
            this.youtubeURL = youtubeURL;
        }
        public Game(List<int> setLengths, ActionsMetricTypes amt, Team team)
        {
            _setLengths = setLengths.ToArray();
            _currentSetIndex = -1;
            _result = GameResult.NotFinished;
            _setsToWin = _setLengths.Length / 2 + 1;
            _sets = new List<Set>();
            ActionsMetricTypes = amt;
            Team = team;
        }
        public Game(List<int> setLengths, ActionsMetricTypes amt, Team team, string URL) : this(setLengths, amt, team)
        {
            youtubeURL = URL;
        }
        public GameResult AddSet(Set set)
        {
            _sets.Add(set);
            _result = isGameFinished();
            _currentSetIndex++;
            return _result;
            
        }
        public void UpdateResult()
        {
            _result = isGameFinished();
        }
        private GameResult isGameFinished()
        {
            int lost = 0, won = 0;
            foreach(Set set in _sets)
            {
                if (set.SetResult == SetResult.Lost) lost++;
                else if (set.SetResult == SetResult.Won) won++;
                
            }
            if (lost >= _setsToWin) return GameResult.Lost;
            else if (won >= _setsToWin) return GameResult.Won;
            return GameResult.NotFinished;
        }
        public Set CurrentSet
        {
            get { return _sets[_currentSetIndex]; }
            set { _sets[_currentSetIndex] = value; }
        }
        public GameResult GameResult
        {
            get { return _result; }
        }
        public List<Set> Sets
        {
            get { return _sets; }
        }
        public int NextSetLength
        {
            get {
                try
                {
                    int ind = Sets.Count;
                    return _setLengths[ind];
                    return _setLengths[_currentSetIndex];
                }
                catch {
                    return 25;
                } }
        }
        public string URL
        {
            get { return youtubeURL; }
        }

        public VolleyActionSequence getVolleyActionSequence()
        {
            VolleyActionSequence res = new VolleyActionSequence();
            foreach(Set s in Sets)
            {
                res += s.ConvertToSequence();
            }
            return res;
        }
        public VolleyActionSegmentSequence getVolleyActionSegmentSequence()
        {
            VolleyActionSegmentSequence res = new VolleyActionSegmentSequence();
            foreach(Set s in Sets)
            {
                res.Add(s.ConvertToSegmentSequence());
            }
            return res;
        }

        public void Save(StreamWriter sw)
        {
            ActionsMetricTypes.Save(sw);
            Team.Save(sw);
            sw.WriteLine(URL);
            sw.WriteLine(JsonSerializer.Serialize(_setLengths));
            sw.WriteLine(JsonSerializer.Serialize(_setsToWin));
            sw.WriteLine(JsonSerializer.Serialize(_currentSetIndex));
            sw.WriteLine(JsonSerializer.Serialize(GameResult));
            sw.WriteLine(JsonSerializer.Serialize(_sets.Count));
            foreach(Set set in _sets)
            {
                set.Save(sw);
            }

        }
        public static Game Load(StreamReader sr)
        {
            ActionsMetricTypes amt = ActionsMetricTypes.Load(sr);
            Team team = Team.Load(sr);
            ActionLoader.currentTeam = team;
            ActionLoader.ActionsMetricTypes = amt;
            string url = sr.ReadLine();
            int[] setLengths = JsonSerializer.Deserialize<int[]>(sr.ReadLine());
            int setsToWin = JsonSerializer.Deserialize<int>(sr.ReadLine());
            int currentIndexSet = JsonSerializer.Deserialize<int>(sr.ReadLine());
            GameResult gameResult = JsonSerializer.Deserialize<GameResult>(sr.ReadLine());
            int setsCount = JsonSerializer.Deserialize<int>(sr.ReadLine());
            Game res = new Game(new List<int>(setLengths), amt, team, url);
            res._setsToWin = setsToWin;
            res._currentSetIndex = currentIndexSet -1 ;
            for(int i= 0;i < setsCount; i++)
            {
                res.AddSet(Set.Load(sr));
            }
            return res;
        }
    }

}