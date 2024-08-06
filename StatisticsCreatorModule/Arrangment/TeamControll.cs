using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionsLib;

namespace StatisticsCreatorModule.Arrangment
{
    internal class TeamControll
    {
        Team _team;
        Arrangment _currentArrangment;
        List<Player> _reservePlayers;
        List<Player> _fastChangePlayers;
        
        public TeamControll(Team team)
        {
            _team = team;
            _currentArrangment = new Arrangment();
            _reservePlayers = new List<Player>();
            _fastChangePlayers = new List<Player>();


            UpdateReservePlayers();
            SetFastChangePlayers();
        }

        private void UpdateReservePlayers()
        {
            _reservePlayers.Clear();
            foreach(Player p in _team.Players)
            {
                if(!_currentArrangment.Contains(p))  _reservePlayers.Add(p);
            }
        }
        private void SetFastChangePlayers()
        {
            _fastChangePlayers.Clear();
            foreach(Player p in _team.Players)
            {
                if(p.Amplua == Amplua.Libero) _fastChangePlayers.Add(p);
            }
        }

        private void ChangePlayer(Player onField, Player Reserve)
        {
            if (_reservePlayers.Contains(Reserve)) ChangeBasicPlayer(onField, Reserve);
            else if (_fastChangePlayers.Contains(Reserve)) ChangeFastPlayer(onField, Reserve);
            else throw new Exception("No such reserve player");
        }
        private void ChangeBasicPlayer(Player onField, Player Reserve) 
        {
            int index = _team.Players.IndexOf(onField);
            if (index == -1) { throw new Exception("No such player on field"); }
            _currentArrangment[index] = Reserve;
            UpdateReservePlayers();
        }
        private void ChangeFastPlayer(Player onField, Player Reserve) // to change fast players correctly
        {
            int index = _team.Players.IndexOf(onField);
            if (index == -1) { throw new Exception("No such player on field"); }
            _fastChangePlayers.Remove(Reserve);
            _fastChangePlayers.Insert(0, onField);
            _currentArrangment[index] = Reserve;
        }
        private void ChangeMiddleBlockerOnLiberoAutomatically(Player middleBlocker) // P1 reception
        {
            ChangeFastPlayer(_fastChangePlayers[0], middleBlocker);
        }
        private void ChangeLiberoOnMiddleBlockerAutomatically(Player Libero) // P4 
        {
            ChangeFastPlayer(_fastChangePlayers[0], Libero);
        }
    }
}
