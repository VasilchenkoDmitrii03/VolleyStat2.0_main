using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionsLib;

namespace StatisticsCreatorModule.Arrangment
{
    public class TeamControl
    {
        Team _team;
        Arrangement _currentArrangment;
        List<Player> _reservePlayers;
        List<Player> _fastChangePlayers;
        
        public TeamControl(Team team)
        {
            _team = team;
            _currentArrangment = new Arrangement();
            _reservePlayers = new List<Player>();
            _fastChangePlayers = new List<Player>();


            UpdateReservePlayers();
            SetFastChangePlayers();
        }

        public Arrangement CurrentArrangement
        {
            get { return _currentArrangment; }
        }
        public Team Team
        {
            get { return _team; }
        }
        public List<Player> ReservePlayers
        {
            get { return _reservePlayers; }

        }

        public void SetArrangement(Arrangement arrangement)
        {
            for (int i = 0; i < 6; i++)
            {
                _currentArrangment[i] = arrangement[i];
            }
            UpdateReservePlayers();
        }
        public void SetArrangement(Player[] players)
        {
            for(int i = 0; i < 6; i++)
            {
                _currentArrangment[i] = players[i];
            }
            UpdateReservePlayers();
        }
        public void ChangePlayer(Player onField, Player Reserve)
        {
            if (_reservePlayers.Contains(Reserve)) ChangeBasicPlayer(onField, Reserve);
            else if (_fastChangePlayers.Contains(Reserve)) ChangeFastPlayer(onField, Reserve);
            else throw new Exception("No such reserve player");
        }
        public void Rotate() // bug fixes needed
        {
            _currentArrangment.Rotate();
            if(_currentArrangment.getAmpluaByZone(4) == Amplua.Libero)
            {
                //Change To MiddleBlocker
                Player changer = GetNonLiberoFromFastChangePlayers();
                ChangeFastPlayer(_currentArrangment[3], changer);
            }
        }
        public int getPlayerZone(Player player)
        {
            for(int i =0;i < 6; i++)
            {
                if(player == _currentArrangment[i]) return i;
            }
            return -1;
        }
        public List<Player> getLiberoPlayers()
        {
            List<Player> res = new List<Player>();
            foreach(Player player in Team.Players)
            {
                if(player.Amplua == Amplua.Libero) res.Add(player);
            }
            return res;
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

        public bool isLiberoOnFirstLine()
        {
            for(int i = 1; i <=3; i++)
            {
                if (_currentArrangment[i].Amplua == Amplua.Libero) return true;
            }
            return false;
        }
        public void AutomaticFirstLineLiberoChange()
        {
            if (! isLiberoOnFirstLine()) return;
            ChangeLiberoOnMiddleBlockerAutomatically(_currentArrangment[3]);
        }
        
        private void ChangeBasicPlayer(Player onField, Player Reserve) 
        {
            int index = _currentArrangment.IndexOf(onField);
            if (index == -1) { throw new Exception("No such player on field"); }
            _currentArrangment[index] = Reserve;
            UpdateReservePlayers();
        }
        public void ChangeFastPlayer(Player onField, Player Reserve) // to change fast players correctly
        {
            int index = _currentArrangment.IndexOf(onField);
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
            int index = -1;
            for(int i= 0;i < _fastChangePlayers.Count;i++)
            {
                if (_fastChangePlayers[i].Amplua != Amplua.Libero)
                {
                    index = i;
                    break;
                }
            }
            ChangeFastPlayer(_fastChangePlayers[index], Libero);
        }
        public Player GetNonLiberoFromFastChangePlayers()
        {
            foreach(Player m in _fastChangePlayers)
            {
                if (m.Amplua != Amplua.Libero) return m;
            }
            return null;
        }
    }
}
