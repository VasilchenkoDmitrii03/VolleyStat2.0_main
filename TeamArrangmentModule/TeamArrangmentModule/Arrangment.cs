using ActionsLib;
using System.Security.Cryptography.X509Certificates;

namespace TeamArrangmentModule
{
    public class Arrangment
    {
        Player[] _players;
        public Arrangment(Player[] _player) 
        {
            _players = new Player[6];
            for(int i = 0;i < _player.Length;i++) { _players[i] = _player[i]; }
        }
        public void rotate()
        {
            Player p = _players[0];
            for(int i =0; i < _players.Length-1; i++)
            {
                _players[i] = _players[i+1];
            }
            _players[5] = p;
        }
    }
}
