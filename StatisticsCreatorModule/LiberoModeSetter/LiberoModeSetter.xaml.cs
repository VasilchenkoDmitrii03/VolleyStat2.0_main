using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using ActionsLib;
using Microsoft.Win32;
using PlayerPositioningWIndow;

namespace StatisticsCreatorModule.LiberoModeSetter
{
    /// <summary>
    /// Логика взаимодействия для LiberoModeSetter.xaml
    /// </summary>
    public partial class LiberoModeSetter : Window
    {
        LiberoArrangementDataContainer _dataContainer;
        public LiberoModeSetter()
        {
            InitializeComponent();
            _dataContainer = new LiberoArrangementDataContainer();
            _dataContainer.fillDefault();
            FillComboBoxesItems();
           
        }

        SegmentPhase previousPhase;
        int previousArrangement;
        int previousValue;
        private void FillComboBoxesItems()
        {
            List<SegmentPhase> phases = new List<SegmentPhase>() { SegmentPhase.Recep_1, SegmentPhase.Break };
            List<int> countsofLiberos = new List<int>() { 1, 2, 3, 4, 5, 6 };
            List<string> arrangements = new List<string>() { "P1", "P2", "P3", "P4", "P5", "P6" };
            PhaseComboBox.ItemsSource = phases;
            PhaseComboBox.SelectionChanged += SelectionChanged;
            PhaseComboBox.SelectedIndex = 0;
            previousPhase = SegmentPhase.Recep_1;
            LiberosCountComboBox.ItemsSource = countsofLiberos;
            LiberosCountComboBox.SelectedIndex = 0;
            LiberosCountComboBox.SelectionChanged += LiberosCountIndexChanged;
            ArrangementComboBox.ItemsSource = arrangements;
            ArrangementComboBox.SelectedIndex = 0;
            previousArrangement = 0;
            ArrangementComboBox.SelectionChanged += SelectionChanged;
            PlayerComboBox.SelectionChanged += (o, e) => { previousValue = PlayerComboBox.SelectedIndex; };
            UpdatePlayersComboBox();
            

        }
        private void UpdatePlayersComboBox()
        {
            PlayerComboBox.Items.Clear();
            PlayerComboBox.Items.Add("Player");
            int count = (int)LiberosCountComboBox.SelectedItem;
            for(int i = 0;i < count; i++)
            {
                PlayerComboBox.Items.Add($"Libero #{i + 1}");
            }
            previousValue = -1;
        }
        private void LiberosCountIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePlayersComboBox();
        }
        private void SelectionChanged(object sender, SelectionChangedEventArgs e) 
        {
            try
            {
                int value = PlayerComboBox.SelectedIndex;
                int arrangement = ArrangementComboBox.SelectedIndex;
                SegmentPhase ph = (SegmentPhase)PhaseComboBox.SelectedItem;
                _dataContainer.fillData(previousPhase, previousArrangement, previousValue);
                previousValue = value;
                previousPhase = ph;
                previousArrangement = arrangement;
                int getData = _dataContainer.getData(ph, arrangement);
                if(getData != -1)PlayerComboBox.SelectedIndex = getData;
            }
            catch { }
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    _dataContainer.Save(sw);
                }
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    _dataContainer = LiberoArrangementDataContainer.Load(sr);
                }
            }
        }
    }
}
