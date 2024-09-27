using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
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
        public Player GetPlayerByNumber(int number)
        {
            foreach (Player player in _players)
            {
                if(player.Number == number) return player;
            }
            return null;
        }
        public void Save(StreamWriter sw)
        {
            sw.WriteLine(_name);
            sw.WriteLine(_description);
            sw.WriteLine(SaveColor(MainColor));
            sw.WriteLine(SaveColor(LiberoColor));
            sw.WriteLine(JsonSerializer.Serialize(Players));

        }
        private string SaveColor(Color c)
        {
            string res = $"{c.R},{c.G},{c.B}";
            return res;
        }
        private static Color LoadColor(string color)
        {
            string[] strs = color.Split(',');
            int[] values = new int[strs.Length];
            for (int i = 0; i < values.Length; i++) { values[i] = Convert.ToInt32(strs[i]); }
            return Color.FromArgb(values[0], values[1], values[2]);
        }
        public static Team Load(StreamReader sr)
        {
            string name = sr.ReadLine();
            string description = sr.ReadLine();
            Color color = LoadColor(sr.ReadLine());
            Color lcolor = LoadColor(sr.ReadLine());
            List<Player> players = JsonSerializer.Deserialize<List<Player>>(sr.ReadLine());

            return new Team(name, description, color, lcolor, players);
        }
    }
}
