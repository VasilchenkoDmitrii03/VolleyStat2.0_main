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
            this.OpponentTeamControl.TeamUpdated += (o, e) => { };

        }
        private void CreateMatch_Click(object sender, RoutedEventArgs e)
        {
            Game game = new Game(this.GameLenControl.getLengthes(),this.ATMSelector._selectedAMT, this.OurTeamControl._selectedTeam);
            StatisticsCreatorModule.MainWindow tmp = new StatisticsCreatorModule.MainWindow(this.OurTeamControl._selectedTeam, game, this.ATMSelector._selectedAMT);
            //StatisticsCreatorModule.MainWindow tmp = new StatisticsCreatorModule.MainWindow();
            tmp.Show();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow tmp = new MainWindow();
            tmp.Show();
            this.Close();
        }
    }
}
