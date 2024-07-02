using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionsLib
{
    public class Team
    {
        List<Player> _players;
        string _name;
        string _description;
        Color _mainColor;
        Color _liberoColor;
        public Team(string name, string description, Color mainColor, Color liberColor, List<Player> players = null)
        {
            if (players == null) _players = new List<Player>();
            else _players = new List<Player>(players);
            _name = name;
            _description = description;
            _mainColor = mainColor;
            _liberoColor = liberColor;
        }
        public Team(string name = "unknown", string description = "unknown", List<Player> players = null)
        {
            if (players == null) _players = new List<Player>();
            else _players = new List<Player>(players);
            _name = name;
            _description = description;
            _mainColor = Color.DarkGreen;
            _liberoColor = Color.Orange;
        }

        public string Name
        {
            get { return _name; }
        }
        public List<Player> Players
        {
            get { return _players; }
        }
        public string Description
        {
            get { return _description; }
        }
        public Color MainColor
        {
            get { return _mainColor; }
        }
        public Color LiberoColor
        {
            get { return _liberoColor; }
        }

        public void AddPlayer(Player player)
        {
            if (!_players.Contains(player)) _players.Add(player);
            else throw new Exception($"Team {_name} also contains Player {player.ToTableString()}");
        }
    }
}
