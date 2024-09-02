using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionsLib;

namespace StatisticsCreatorModule.Logic
{
    public class AvaibleActionTypes
    {
        Dictionary<ActionAuthorType, List<VolleyActionType>> _data;
        public AvaibleActionTypes(Dictionary<ActionAuthorType, List<VolleyActionType>> data)
        {
            _data = data;
        }
        public AvaibleActionTypes()
        {
            _data = new Dictionary<ActionAuthorType, List<VolleyActionType>>();
        }
        public void Ad(ActionAuthorType type, List<VolleyActionType> lst) 
        {
            _data.Add(type, lst);
        }
        public void SetDefault()
        {
            _data.Clear();
            _data.Add(ActionAuthorType.Player, ActionTypeConverter.getActionTypesByAuthor(ActionAuthorType.Player));
            _data.Add(ActionAuthorType.OpponentTeam, ActionTypeConverter.getActionTypesByAuthor(ActionAuthorType.OpponentTeam));
            _data.Add(ActionAuthorType.Coach, ActionTypeConverter.getActionTypesByAuthor(ActionAuthorType.Coach));
            _data.Add(ActionAuthorType.Judge, ActionTypeConverter.getActionTypesByAuthor(ActionAuthorType.Judge));
        }
        public void GameBegining()
        {
            _data.Clear();
            _data.Add(ActionAuthorType.Coach, new List<VolleyActionType>() { VolleyActionType.StartArrangment });
        }
        public void ArrangementSet()
        {
            _data.Clear();
            _data.Add(ActionAuthorType.Coach, new List<VolleyActionType>() { VolleyActionType.SetStartParams});
        }
        public void BetweenRallies(List<VolleyActionType> PlayersActions) 
        {
            _data.Clear();
            _data.Add(ActionAuthorType.Player, PlayersActions);
            _data.Add(ActionAuthorType.OpponentTeam, ActionTypeConverter.getActionTypesByAuthor(ActionAuthorType.OpponentTeam));
            _data.Add(ActionAuthorType.Coach, new List<VolleyActionType>() { VolleyActionType.Change, VolleyActionType.TimeOut });
            _data.Add(ActionAuthorType.Judge, ActionTypeConverter.getActionTypesByAuthor(ActionAuthorType.Judge));
        }
        public void InRally(List<VolleyActionType> PlayersActions)
        {
            _data.Clear();
            _data.Add(ActionAuthorType.Player, PlayersActions);
            _data.Add(ActionAuthorType.OpponentTeam, ActionTypeConverter.getActionTypesByAuthor(ActionAuthorType.OpponentTeam));
            _data.Add(ActionAuthorType.Judge, ActionTypeConverter.getActionTypesByAuthor(ActionAuthorType.Judge));
        }
        public void Clear()
        {
            _data.Clear();
        }
        public List<VolleyActionType> this[ActionAuthorType aat]
        {
            get { return _data[aat]; }
        }
        public List<ActionAuthorType> Authors()
        {
            return _data.Keys.ToList();
        }
    }
}
