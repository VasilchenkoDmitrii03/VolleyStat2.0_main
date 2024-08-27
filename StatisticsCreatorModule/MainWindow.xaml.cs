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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ActionsLib;
using ActionsLib.ActionTypes;
using ActionsLib.TextRepresentation;
using Microsoft.Win32;

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
        public MainWindow()
        {
            InitializeComponent();
            LiberoModeSetter.LiberoModeSetter tmp = new LiberoModeSetter.LiberoModeSetter();
            tmp.Show();
            _actionMetricTypes = ActionsMetricTypes.Load(@"C:\Dmitrii\Programming\VolleyStat2.0_main\BasicActionsMetrics");
            
            _team = new Team();
            for(int  i = 1; i < 20; i++)
            {
                _team.AddPlayer(new Player($"{i}", $"{i}", 1, i, Amplua.OutsideHitter));
            }
            InitializeModules();
            TextModule.BeginNewSet(25);
        }
        private void InitializeModules()
        {
            

            TextModule.setActionMetricsTypes(_actionMetricTypes);
            TextModule.setTeam(_team);
            TextModule.LineRepresentationControl.ComboBoxSelectionChanged += ButtonModule.ComboBoxUpdated;
            ButtonModule.ButtonSelectionChanged += TextModule.LineRepresentationControl.GetButtonIndex;
            TextModule.ArrangementChanged += GraphicsModule.TeamControlChanged;
            VideoModule.TimeCodeChanged += TextModule.GetCurrentTimeCodeEventHandler;
            GraphicsModule.PointsChanged += TextModule.PointsUpdated;
            TextModule.ActionAdded += GraphicsModule.ActionAdded;
            TextModule.ScoreUpdated += ScoreModule.ScoreUpdated;
            //TextModule.LineRepresentationControl.ActionTypeChangedInTextModule += test;

            CreateLists();
        }
        private void CreateLists()
        {
            _userControls = new List<Control>();
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

        List<Control> _userControls;
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


        #region Save\Load
        private Set Load(string path)
        {
            ActionLoader.currentTeam = _team;
            ActionLoader.ActionsMetricTypes = _actionMetricTypes;
            using (StreamReader sr = new StreamReader(path))
            {
                return Set.Load(sr);
            }
        }
        private void Save(string path)
        {
            using(StreamWriter sw = new StreamWriter(path)) 
            {
                TextModule.CurrentSet.Save(sw);
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
                Set set = Load(sfd.FileName);
                TextModule.LoadSet(set);
            }
        }

        #endregion
    }
    public delegate void themeUpdater();
}