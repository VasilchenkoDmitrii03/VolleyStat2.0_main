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
using ActionsLib;
using StatisticsViewerModule;


namespace StatisticsViewerModule
{
    /// <summary>
    /// Interaction logic for StatisticsPlayersFilters.xaml
    /// </summary>
    public partial class StatisticsPlayersFilters : UserControl
    {
        List<string> _players;
        Team _team;
        public PlayersFiltersHolder PlayersFiltersHolder { get; set; }
        public StatisticsPlayersFilters()
        {
            InitializeComponent();
            _players = new List<string>();
            PlayersFiltersHolder = new PlayersFiltersHolder(_team);
            PlayerCheckBox.SelectionChangedEvent += (o, e) => { PlayersFiltersHolder.update(getSelected()); };
        }
        public void Setup(Team team)
        {
            _team = team;
            _players.Clear();
            foreach (Player p in _team.Players) {
                _players.Add(p.Number.ToString());
            }
            PlayerCheckBox.UpdateOptions(_players.ToArray());
            PlayerCheckBox.SelectAll();
        }
        public List<string> getSelected()
        {
            return PlayerCheckBox.SelectedOptions();
        }
        public void Update()
        {
            PlayersFiltersHolder.update(getSelected());
        }
        public void SelectAll()
        {
            PlayerCheckBox.SelectAll();
        }
        public void DeselectAll()
        {
            PlayerCheckBox.DeselectAll();
        }

        
    }
}
