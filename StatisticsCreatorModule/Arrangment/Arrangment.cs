using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionsLib;

namespace StatisticsCreatorModule.Arrangment
{
    internal class Arrangment
    {
        Player[] _fieldPlayers; 
        public Arrangment()
        {
            _fieldPlayers = new Player[6];
        }
        public Arrangment(Player[] pl)
        {
            _fieldPlayers = pl.ToArray();
        }
        public void Rotate()
        {
            Player p = _fieldPlayers[0];
            for (int i = 0; i < _fieldPlayers.Length - 1; i++)
            {
                _fieldPlayers[i] = _fieldPlayers[i + 1];
            }
            _fieldPlayers[5] = p;
        }

        public Player this[int index]
        {
            get { return _fieldPlayers[index]; }
            set { _fieldPlayers[index] = value; }
        }
        public Amplua getAmpluaByZone(int zone)
        {
            return _fieldPlayers[zone - 1].Amplua;
        }
        public bool Contains(Player p)
        {
            for(int i = 0;i < 6; i++)
            {
                if(_fieldPlayers[i] == p) return true;
            }
            return false;
        }
        public int IndexOf(Player p)
        {
            for(int i = 0; i < 6; i++)
            {
                if (_fieldPlayers[i] == p) return i;
            }
            return -1;
        }
    }
}
