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
using System.Windows.Shapes;
using ActionsLib;
using StatisticsCreatorModule;

namespace InterfaceShell.AdditionalWindows.GameCreation
{
    /// <summary>
    /// Логика взаимодействия для GameCreateWindow.xaml
    /// </summary>
    public partial class GameCreateWindow : Window
    {
        public GameCreateWindow()
        {
            InitializeComponent();
            this.OurTeamControl.TeamUpdated += OurTeamUpdated;
            this.OpponentTeamControl.TeamUpdated += (o, e) => { };

        }
        private void CreateMatch_Click(object sender, RoutedEventArgs e)
        {
            Game game = new Game(this.GameLenControl.getLengthes());
            StatisticsCreatorModule.MainWindow tmp = new StatisticsCreatorModule.MainWindow(this.OurTeamControl._selectedTeam, game, this.ATMSelector._selectedAMT);
            //StatisticsCreatorModule.MainWindow tmp = new StatisticsCreatorModule.MainWindow();
            tmp.Show();
        }
        public void OurTeamUpdated(object sender, TeamEventArgs e) 
        {
            PlayersListBox.ItemsSource = OurTeamControl._selectedTeam.Players;
        }
    }
}
