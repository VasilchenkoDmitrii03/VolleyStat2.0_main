using ActionsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StatisticsViewerModule
{
    /// <summary>
    /// Логика взаимодействия для SimpleArrangementFilter.xaml
    /// </summary>
    public partial class SimpleArrangementFilter : UserControl
    {
        List<Player> _players = new List<Player>();
        Team _team;
        TextBox[] _textBoxes;
        public SimpleArrangementFilter()
        {
            InitializeComponent();
           _textBoxes = new TextBox[] {P1_ComboBox, P2_ComboBox, P3_ComboBox, P4_ComboBox, P5_ComboBox, P6_ComboBox};
        }
        public void Setup(Team team)
        {
            _team = team;
            _players.Clear();
            foreach (Player p in _team.Players)
            {
                _players.Add(p);
            }

            //foreach (ComboBox cb in _comboBoxes) cb.ItemsSource = _players;
        }

        public VolleyActionSegmentSequence Process(List<Set> sets)
        {
            VolleyActionSegmentSequence result = new VolleyActionSegmentSequence();
            foreach(Set set in sets)
            {
                result.Add(Process(set));
            }
            return result;
        }
        public VolleyActionSegmentSequence Process(Set set)
        {
            Player[] condition = getSelectedValues();
            Player[] arrangement = new Player[6];
            VolleyActionSegmentSequence seq = set.ConvertToSegmentSequence();
            CoachAction startArrangement = (CoachAction)seq.SelectByCondition((seg) => { return seg.ContainsActionType(VolleyActionType.StartArrangment); }).ConvertToActionSequence()[0];
            for (int i = 0; i < 6; i++) arrangement[i] = startArrangement.Players[i]; // startArrangement
            RallySequence rallies = set.Rallies;
            SegmentPhase previous = SegmentPhase.Recep;
            VolleyActionSegmentSequence result = new VolleyActionSegmentSequence();
            foreach (Rally rally in rallies)
            {
                SegmentPhase current = rally.GetRallyPhase();
                if (previous == SegmentPhase.Recep_1 && current == SegmentPhase.Break)
                {
                    rotate(arrangement);
                }
                if (check(arrangement, condition))
                {
                    result.Add(rally.ConvertToSegmentSequence());
                }
                previous = current;
            }
            return result;
        }
        Player[] getSelectedValues()
        {
            Player[] res = new Player[6];
            for(int i = 0;i < 6; i++)
            {
                string str = _textBoxes[i].Text;
                if (str == "") res[i] = null;
                else res[i] = _team.GetPlayerByNumber(Convert.ToInt32(str));
            }
            return res;
        }
        void rotate(Player[] players)
        {
            Player zone1 = players[0];
            for(int i=1;i < 6; i++)
            {
                players[i - 1] = players[i];
            }
            players[5] = zone1;
        }
        bool check(Player[] arrangement, Player[] condition)
        {
            bool res = true;
            for(int i = 0;i < 6; i++)
            {
                if (condition[i] == null) continue;
                res = res & arrangement[i].Number == condition[i].Number;
            }
            return res;
        }
    }
}
