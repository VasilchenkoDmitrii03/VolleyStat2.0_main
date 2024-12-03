using ActionsLib;
using Microsoft.Win32;
using StatisticsCreatorModule.TableTextStatsModule;
using System.IO;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            baseSetup();
        }
        Game _game = null;
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == true)
            {
                using(StreamReader sr =  new StreamReader(ofd.FileName))
                {
                    Game game = Game.Load(sr);
                    updateGame(game);
                }
                
            }
        }

        private void updateGame(Game game)
        {
            _game = game;
            this.PlayersFiltersModule.Update();
            this.FiltersModule.UpdateAMT(_game.ActionsMetricTypes);
            this.StatisticsListBox.SetActions(_game.getVolleyActionSequence());
            this.PlayersFiltersModule.Setup(game.Team);
            this.StatisticsVideoModule.StartVideo(_game.URL);
            this.ArrangementFilterModule.Setup(_game.Team);
        }
        private void baseSetup()
        {
            this.StatisticsListBox.SetVideoModule(this.StatisticsVideoModule);
            this.StatisticsListBox.SetFiltersController(this.FiltersModule);
            this.StatisticsListBox.SetPlayersFilters(this.PlayersFiltersModule);
            this.StatisticsListBox.PlayerFilters.SelectAll();

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.ArrangementFilterModule.Process(_game.Sets);
        }

        #region pdf stats

        private void FullMathStat_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == true)
            {
                DocumentCreator docCreat = new DocumentCreator();
                docCreat.CreateTotalStatisticsFilePDF(_game, sfd.FileName);
            }
        }


        #endregion
    }
}