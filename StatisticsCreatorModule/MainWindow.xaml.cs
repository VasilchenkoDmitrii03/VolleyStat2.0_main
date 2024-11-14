using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ActionsLib;
using ActionsLib.ActionTypes;
using ActionsLib.TextRepresentation;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.Win32;
using PlayerPositioningWIndow;
using StatisticsCreatorModule.Arrangment;
using StatisticsCreatorModule.LiberoModeSetter;
using StatisticsCreatorModule.PlayerPositioning;
using StatisticsCreatorModule.SettingsWindow;
using StatisticsCreatorModule.TableTextStatsModule;
namespace StatisticsCreatorModule
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /* public MainWindow()
         {
             InitializeComponent();
             ActionsMetricTypes atl = ActionsMetricTypes.Load(@"C:\Dmitrii\Programming\VolleyStat2.0_main\BasicActionsMetrics");
             MetricTypeList mtl = atl[VolleyActionType.Attack];
             //ButtonController.getPlayerAction(mtl);
             ButtonController.updateButtons(new List<Object> { VolleyActionType.Serve, JudgeActionType.CardGift });
         }

         private void Button_Click(object sender, RoutedEventArgs e)
         {
             //List<Metric> tmp = ButtonController.MetricListResult;
             Object o = ButtonController.SelectedObject;
             Type t = o.GetType();
         }*/
        PlayerActionTextRepresentation _currentAction;
        ActionsMetricTypes _actionMetricTypes;
        Team _team;
        Game _game;
        public MainWindow()
        {
            InitializeComponent();
            _actionMetricTypes = ActionsMetricTypes.Load(@"C:\Dmitrii\Programming\VolleyStat2.0_main\AdditionalFiles\ActionMetricTypes\Kvadratiki");
            
            _team = new Team();
            for(int  i = 1; i < 20; i++)
            {
                if(i > 16)
                {
                    _team.AddPlayer(new Player($"{i}", $"{i}", 1, i, Amplua.Libero));
                }
                else _team.AddPlayer(new Player($"{i}", $"{i}", 1, i, Amplua.OutsideHitter));
            }

            InitializeModules();
            _game = new Game(new List<int>() { 1, 2, 2 }, _actionMetricTypes, _team);
            BeginSet();
        }
        public MainWindow(Team team, Game game, ActionsMetricTypes amt)
        {
            InitializeComponent();
            
            _team = team;
            _game = game;
            _actionMetricTypes = amt;
            UpdateURL(game.URL);
            InitializeModules();
            updateStatisticsData(_game);
            BeginSet();
        }

        private void updateStatisticsData(Game game)
        {
            VolleyActionSequence seq = new VolleyActionSequence();
            foreach(Set set in game.Sets)
            {
                seq.Add(set.ConvertToSequence());
            }
            StatisticsListBox.SetActions(seq);
        }
        private void InitializeModules()
        {
            SetTeam_AMT(_team, _actionMetricTypes);
            TextModule.LineRepresentationControl.ComboBoxSelectionChanged += ButtonModule.ComboBoxUpdated;
            ButtonModule.ButtonSelectionChanged += TextModule.LineRepresentationControl.GetButtonIndex;
            TextModule.ArrangementChanged += GraphicsModule.TeamControlChanged;
            VideoModule.OnTimeCodeChanged += TextModule.GetCurrentTimeCodeEventHandler;
            GraphicsModule.PointsChanged += TextModule.PointsUpdated;
            TextModule.ActionAdded += GraphicsModule.ActionAdded;
            TextModule.ActionAdded += (o, e) => { StatisticsListBox.Sequence = e.seq; };
            TextModule.ScoreUpdated += ScoreModule.ScoreUpdated;
            TextModule.GamePhaseForGraphicsChanged += GraphicsModule.PhaseChanged;
            TextModule.SetFinished += SetFinishedHandler;
            TextModule.SetPositionSettingsMode(new PositionSettingsArgs(new PositionSettingsMode(1, LiberoArrangementDataContainer.GetDefault(), PlayerPositionDataContainer.GetDefault()), SegmentPhase.Recep_1));
            GraphicsModule._playersPositions = PlayerPositionDataContainer.GetDefault();
            FiltersModule.UpdateAMT(_actionMetricTypes);
            this.PlayersFiltersModule.SetupCombobox(_team);
            StatisticsListBox.SetFiltersController(this.FiltersModule);
            StatisticsListBox.SetVideoModule(this.StatisticsVideoModule);
            StatisticsListBox.SetPlayersFilters(this.PlayersFiltersModule);
            PlayerLabel.LiberoColor = System.Drawing.Color.Red;
            PlayerLabel.MainColor = System.Drawing.Color.Black;
            //TextModule.LineRepresentationControl.ActionTypeChangedInTextModule += test;

            CreateLists();
        }
        private void CreateLists()
        {
            _userControls = new List<System.Windows.Controls.Control>();
            _userControls.Add(VideoModule);
            _userControls.Add(TextModule);
            _userControls.Add(ButtonModule);
            _userControls.Add(GraphicsModule);
            _userControls.Add(ScoreModule);
            _userControlsThemeUpdaters = new List<themeUpdater>();
            _userControlsThemeUpdaters.Add(VideoModule.UpdateTheme);
            _userControlsThemeUpdaters.Add(TextModule.UpdateTheme);
            _userControlsThemeUpdaters.Add(ButtonModule.UpdateTheme);
            _userControlsThemeUpdaters.Add(GraphicsModule.UpdateTheme);
            _userControlsThemeUpdaters.Add(ScoreModule.UpdateTheme);
        }
        private void SetFinishedHandler(object sender, SetEventArgs e) 
        {
            MessageBox.Show("Game finished");
            if (!currentSetAdded)
            {
                this._game.AddSet(e.Set);
            }
            currentSetAdded = false;
            _game.UpdateResult();
            if (_game.GameResult == GameResult.Lost || _game.GameResult == GameResult.Won)
            {
                MessageBox.Show("Game finished");
                SaveAs_Click(null, null);

                return;
            }
            else
            {
                TextModule.Clear();
                ScoreModule.Clear();
                GraphicsModule.ClearPoints();
                ButtonModule.ClearButtons();
                BeginSet();
            }
        }

        private void UpdateURL(string url)
        {
            this.VideoModule.StartVideo(url);
            this.StatisticsVideoModule.StartVideo(url);
        }
        private void BeginSet()
        {

            
            if(_game.GameResult == GameResult.NotFinished)
            {
                
                if(_game.Sets.Count > 0 && (_game.Sets.Last().SetResult == SetResult.NotFinished || _game.Sets.Last().SetResult == SetResult.Undefined))
                {
                    ScoreModule.UpdateSetNumber(_game.Sets.Count);
                    VolleyActionSequence seq = _game.Sets.Last().ConvertToSequence();
                    TextModule.BeginNewSet(this._game.Sets.Last().SetLength, seq);
                    currentSetAdded = true;
                    _game.Sets[_game.Sets.Count-1] = TextModule.CurrentSet;
                }
                else
                {
                    ScoreModule.UpdateSetNumber(_game.Sets.Count + 1);
                    TextModule.BeginNewSet(this._game.NextSetLength);
                    PlayerPositionUpdate(null, null);
                }
                
            }
            
        }
        private void SetTeam_AMT(Team team, ActionsMetricTypes amt)
        {
            this.TextModule.setTeam(team);
            this.TextModule.setActionMetricsTypes(amt);
           
        }
        List<System.Windows.Controls.Control> _userControls;
        List<themeUpdater> _userControlsThemeUpdaters;

        #region Themes
        string _currentTheme = "Light";
        private void switchTheme(string theme)
        {
            if (theme == _currentTheme) return;
            ResourceDictionary newTheme = new ResourceDictionary();
            switch (theme)
            {
                case "Dark":
                    newTheme.Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative);
                    break;
                case "Light":
                    newTheme.Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative);
                    break;
            }
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(newTheme);
            
            _currentTheme = theme;
            foreach(themeUpdater updater in _userControlsThemeUpdaters)
            {
                updater();
            }
            // Обновляем темы во всех UserControl
            //myUserControl.UpdateTheme();
        }
        private void ThemeSelector_Click(object sender, RoutedEventArgs e)
        {
            string theme = ((MenuItem)sender).Header.ToString();
            switchTheme(theme);
        }
        #endregion


        private void CreateBaseTableClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == true)
            {
                using (WordprocessingDocument document = WordprocessingDocument.Create(sfd.FileName, WordprocessingDocumentType.Document))
                {
                    DocumentCreator documentCreator = new DocumentCreator();
                    documentCreator.CreateBaseStatTable(_game, document);
                    documentCreator.ImageTestDocument(_game, document);

                }
            }
        }
        private void ImageTableClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == true)
            {
                using (WordprocessingDocument document = WordprocessingDocument.Create(sfd.FileName, WordprocessingDocumentType.Document))
                {
                    DocumentCreator documentCreator = new DocumentCreator();
                    documentCreator.CreateFullReceptionStats(_game, document);

                }
            }
        }

        private void CreateSetterTableClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == true)
            {
                using (WordprocessingDocument document = WordprocessingDocument.Create(sfd.FileName, WordprocessingDocumentType.Document))
                {
                    DocumentCreator documentCreator = new DocumentCreator();
                    documentCreator.CreateBaseSetterTable(_game, document);
                }
            }
        }

        private void CreateReceptionTableClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == true)
            {
                using (WordprocessingDocument document = WordprocessingDocument.Create(sfd.FileName, WordprocessingDocumentType.Document))
                {
                   DocumentCreator documentCreator = new DocumentCreator();
                    documentCreator.CreateBaseReceptionTable(_game, document);
                }
            }
        }

        SettingsWindow.SettingsWindow PositionSettingsWindow;
        bool currentSetAdded = false;
        public void PlayerPositionUpdate (object sender, RoutedEventArgs e)
        {
            if(PositionSettingsWindow == null || PositionSettingsWindow.IsVisible == false)
            {
                PositionSettingsWindow = new SettingsWindow.SettingsWindow(TextModule.TeamControl);
                PositionSettingsWindow.Closed += (s, args) => PositionSettingsWindow = null;
                PositionSettingsWindow.SettingsUpdated += PositionSettingsUpdated;
                PositionSettingsWindow.Show();
            }
            else
            {
                PositionSettingsWindow.Activate();
            }
        }
        private void PositionSettingsUpdated(object sender, PositionSettingsArgs e)
        {
            GraphicsModule.SetPositionSettingsMode(e);
            TextModule.SetPositionSettingsMode(e);
            
        }
        #region Save\Load
        private Game Load(string path)
        {
            ActionLoader.currentTeam = _team;
            ActionLoader.ActionsMetricTypes = _actionMetricTypes;
            using (StreamReader sr = new StreamReader(path))
            {
                return Game.Load(sr);
            }

        }
        private void Save(string path)
        {
            using(StreamWriter sw = new StreamWriter(path)) 
            {

                if(_game.GameResult == GameResult.Undefined || _game.GameResult== GameResult.NotFinished)
                {
                    if (!currentSetAdded)
                    {
                        _game.AddSet(TextModule.CurrentSet);
                    }        
                    currentSetAdded = true;
                }
                _game.Save(sw);
                //sw.Write(TextModule.CurrentSet.Save());
            }
        }
        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if(sfd.ShowDialog() == true )
            {
                Save(sfd.FileName);
            }
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            if (sfd.ShowDialog() == true)
            {
                Game game = Load(sfd.FileName);
                _game = game;
                this._actionMetricTypes = _game.ActionsMetricTypes;
                this._team = _game.Team;
                StatisticsListBox.Sequence = _game.Sets[0].ConvertToSequence();
                SetTeam_AMT(_team, _actionMetricTypes);
                ScoreModule.UpdateSetNumber(_game.Sets.Count);
                TextModule.LoadSet(_game.Sets.Last());
            }
        }

        #endregion
    }
    public delegate void themeUpdater();
}