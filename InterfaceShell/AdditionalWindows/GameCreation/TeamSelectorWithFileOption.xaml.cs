using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Microsoft.Win32;

namespace InterfaceShell.AdditionalWindows.GameCreation
{
    /// <summary>
    /// Логика взаимодействия для TeamSelectorWithFileOption.xaml
    /// </summary>
    ///
    public delegate void TeamUpdated(object sender, TeamEventArgs e);
    public partial class TeamSelectorWithFileOption : UserControl
    {
        public Team _selectedTeam = null;
        List<Team> _teams = new List<Team>();
        public TeamSelectorWithFileOption()
        {
            InitializeComponent();
            TeamUpdated += (o, e) => { };
        }
        public void LoadBasicTeams(List<Team> teams)
        {
            _teams = teams;
            this.BaseTeamsComboBox.ItemsSource = _teams;
        }

        public event TeamUpdated TeamUpdated;
        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    _selectedTeam = Team.Load(sr);
                    TeamUpdated(this, new TeamEventArgs(_selectedTeam));
                }
                
            }
        }

        private void BaseTeamsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedTeam =(Team) this.BaseTeamsComboBox.SelectedItem;
            TeamUpdated(this, new TeamEventArgs(_selectedTeam));
        }
    }
    public class TeamEventArgs : EventArgs
    {
        public Team team;
        public TeamEventArgs(Team team)
        {
            this.team = team;
        }
    }
}
