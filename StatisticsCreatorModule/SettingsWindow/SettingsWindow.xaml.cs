using StatisticsCreatorModule.Arrangment;
using StatisticsCreatorModule.LiberoModeSetter;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;

namespace StatisticsCreatorModule.SettingsWindow
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        string ArrangementPath = "C:\\Dmitrii\\Programming\\VolleyStat2.0_main\\AdditionalFiles\\ArrangementSystems";
        string LiberoSystemPath = "C:\\Dmitrii\\Programming\\VolleyStat2.0_main\\AdditionalFiles\\LiberoSystems";

        string[] ArrangementSystems = new string[0];
        string[] LiberoSystems = new string[0];

        TeamControl _teamControl;
        public SettingsWindow( TeamControl tc)
        {
            _teamControl = tc;
            InitializeComponent();
            LiberoSettingsBlock.setTeamControl(tc);
            LoadFiles();
        }
        public void LoadFiles()
        {
            LoadArrangementSystems();
            LoadLiberoSystems();
        }
        private void LoadLiberoSystems()
        {
            LiberoSystems = Directory.GetFiles(LiberoSystemPath);
            List<string> LiberoSystemNames = new List<string>();
            for (int i = 0; i < LiberoSystems.Length; i++) LiberoSystemNames.Add(getFileName(LiberoSystems[i]));
            LiberoSystemComboBoxes.ItemsSource = LiberoSystemNames;
        }
        private void LoadArrangementSystems()
        {
            ArrangementSystems = Directory.GetFiles(ArrangementPath);
            List<string> ArrangementNames = new List<string>();
            for(int i = 0; i < ArrangementSystems.Length; i ++) ArrangementNames.Add(getFileName(ArrangementSystems[i]));
            ArrangementSystemComboBoxes.ItemsSource = ArrangementNames;
        }

        private string getFileName(string fileName)
        {
            int index = fileName.LastIndexOf("\\");
            return fileName.Substring(index + 1);
        }

        private void LiberoSystemComboBoxes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string path = LiberoSystems[LiberoSystemComboBoxes.SelectedIndex];
            using(StreamReader sr =  new StreamReader(path))
            {
                LiberoArrangementDataContainer tmp = LiberoArrangementDataContainer.Load(sr);
                LiberoSettingsBlock.updateComboBoxesNumber(tmp.getLiberoCount());
            }
            
        }

        StatisticsCreatorModule.PlayerPositioning.PlayerPositionWindow ArrangementWindow = null;
        StatisticsCreatorModule.LiberoModeSetter.LiberoModeSetter LiberoWindow = null;
        private void ArrangementMenu_Click(object sender, RoutedEventArgs e)
        {
            if(ArrangementWindow == null)ArrangementWindow  = new PlayerPositioning.PlayerPositionWindow();
            ArrangementWindow.Closed += (o, e) => { ArrangementWindow = null; };
            ArrangementWindow.Show();
        }

        private void LiberoMenu_Click(object sender, RoutedEventArgs e)
        {
            if (LiberoWindow == null) LiberoWindow = new LiberoModeSetter.LiberoModeSetter();
            LiberoWindow.Closed += (o, e) => { LiberoWindow = null; };
            LiberoWindow.Show();
        }
    }
}
