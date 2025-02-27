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
        VolleyActionSegmentSequence _actionSegmentSequence;
        RallySequence _rallySequence;
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == true)
            {
                using(StreamReader sr =  new StreamReader(ofd.FileName))
                {
                    Game game = Game.Load(sr);
                    updateGame(game);
                    _actionSegmentSequence = null;
                }
                
            }
        }

        private void OpenFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Multiselect = true };
            if (ofd.ShowDialog() == true)
            {
                string[] selectedFiles = ofd.FileNames;
                VolleyActionSegmentSequence sequence = new VolleyActionSegmentSequence();
                RallySequence rallySequence = new RallySequence();
                foreach (string file in selectedFiles)
                {
                    
                    
                    using (StreamReader sr = new StreamReader(file))
                    {
                        Game game = Game.Load(sr);
                        sequence.Add(game.getVolleyActionSegmentSequence());
                        rallySequence.Add(game.getRallySequence());
                        _game = game;
                    }
                    
                }
                _actionSegmentSequence = sequence;
                _rallySequence = rallySequence;
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

        #region testFuncs

        private void test(RallySequence seq, Team team)
        {
            Dictionary<int, int> dict = new Dictionary<int,  int>();
            foreach(Player p in team.Players)
            {
                dict.Add(p.Number, 0);
            }
            foreach(Rally r in seq)
            {
                if(r.RallyTimeLength() >= 15)
                {
                    ActionsLib.Action act = r.ConvertToActionSequence().Last();
                    if(act.AuthorType == ActionAuthorType.Player)
                    {
                        if (((PlayerAction)act).GetQuality() == 6) dict[((PlayerAction)act).Player.Number]++;
                    }
                }
            }

            int a = 0;
        }
        #endregion

        #region pdf stats

        private void FullMathStat_Click(object sender, RoutedEventArgs e)
        {
            //test(_rallySequence, _game.Team);


            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == true)
            {
                DocumentCreator docCreat = new DocumentCreator();
                if(_actionSegmentSequence == null)
                         docCreat.CreateTotalStatisticsFilePDF(_game, sfd.FileName);
                else
                    docCreat.CreateTotalStatisticsFilePDF(_game, _actionSegmentSequence,  sfd.FileName);
            }
        }


        #endregion
    }
}