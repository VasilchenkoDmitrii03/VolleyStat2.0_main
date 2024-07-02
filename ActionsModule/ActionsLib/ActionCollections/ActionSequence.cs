using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionsLib
{
    internal class ActionSequence : IEnumerable<Action>
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
                    if (action.AuthorType == type) seq.Append(action);
                }
                return seq;
            }
        }
        public VolleyActionSequence getPlayersAction()
        {
            List<PlayerAction> lst = new List<PlayerAction>();
            foreach(Action act in this)
            {
                if(act.AuthorType == ActionAuthorType.Player) lst.Add((PlayerAction)act);
            }
            return new VolleyActionSequence(lst);
        }

        public IEnumerator<Action> GetEnumerator()
        {
            return _actions.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        { return (IEnumerator)GetEnumerator(); }
    }
    internal class VolleyActionSequence : IEnumerable<Action>
    {
        List<PlayerAction> _actions;

        public VolleyActionSequence()
        {
            _actions = new List<PlayerAction>();
        }
        public VolleyActionSequence(List<PlayerAction> actions)
        {
            _actions = new List<PlayerAction>(_actions);
        }
        public VolleyActionSequence(Action[] actions)
        {
            _actions = new List<PlayerAction>(_actions);
        }
        public VolleyActionSequence(VolleyActionSequence seq)
        {
            _actions = new List<PlayerAction>(seq._actions);
        }


        public IEnumerator<Action> GetEnumerator()
        {
            return _actions.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        { return (IEnumerator)GetEnumerator(); }
    }
}
