using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
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

    public class VolleyActionSequence : ObservableCollection<PlayerAction>
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
            foreach (PlayerAction act in seq)
            {
                this.Add(act);
            }
        }

        public static VolleyActionSequence operator+(VolleyActionSequence left,  VolleyActionSequence right)
        {
            VolleyActionSequence res = new VolleyActionSequence(left);
            foreach(PlayerAction act in right)
            {
                res.Add(act);
            }
            return res;
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
            foreach (PlayerAction action in a)
            {
                Actions.Add(action);
            }
        }
        public void Add(PlayerAction action)
        {
            Actions.Add(action);
        }
        public bool ContainsActionType(VolleyActionType type)
        {
            var req = Actions.Where(act => act.VolleyActionType == type);
            return req.Count() != 0;
        }
        public List<VolleyActionType> VolleyActionTypes()
        {
            var req = from act in Actions select act.VolleyActionType;
            return req.ToList();
        }
        public VolleyActionSequence ConvertToSequence()
        {
            return Actions;
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
    }
    public enum RallyResult
    {
        Undefined = -1,
        Won = 1,
        Lost = 2,
        Disputable = 3
    }
    public class Rally
    {
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
    }
    public enum SetResult
    {
        Undefined = -1,
        NotFinished = 0,
        Won = 1,
        Lost = 2
    }
    public class Set
    {
        public RallySequence Rallies { get; set; }
        int _setLength;
        SetResult _setResult;
        Score _currentScore;

        public Set(int setLength)
        {
            _setLength = setLength;
            _setResult = SetResult.Undefined;
        }

        public int SetLength { get { return _setLength; } }
        public SetResult SetResult
        {
            get { return _setResult; }
        }
        public Score CurrentScore
        {
            get { return _currentScore; }
        }

        public void Add(Rally rally)
        {
            Rallies.Add(rally);
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
    }
    public class Score
    {
        int _left;
        int _right;
        int _setLength;

        public Score(int setLength = 25)
        {
            _left = setLength;
            _right = _left = 0;
        }

        public bool isFinished()
        {
            if (Math.Abs(_left - _right) > 1 && (_left > _setLength || _right > _setLength))
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
    }
}