using ActionsLib;
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
using PlayerPositioningWIndow;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics.CodeAnalysis;
namespace StatisticsCreatorModule.PlayerPositioning
{
    /// <summary>
    /// Логика взаимодействия для PlayerPositionWindow.xaml
    /// </summary>

    public partial class PlayerPositionWindow : Window
    {
        public PlayerPositionWindow()
        {
            InitializeComponent();
            setup();
        }
        PlayerPositionDataContainer container = new PlayerPositionDataContainer();
        int SetterPosition = 0;
        SegmentPhase CurrentPhase = SegmentPhase.Recep_1;
        private void setup()
        {
            PhaseComboBox.Items.Add(SegmentPhase.Recep_1);
            PhaseComboBox.Items.Add(SegmentPhase.Recep);
            PhaseComboBox.Items.Add(SegmentPhase.Break);
            PhaseComboBox.SelectedIndex = 0;
            ArrangementComboBox.SelectedIndex = 0;

            PhaseComboBox.SelectionChanged += ComboBoxSelectionChanged;
            ArrangementComboBox.SelectionChanged += ComboBoxSelectionChanged;
            FieldModule.ReDraw();
            PositionModule.PlayerPositionsChanged += FieldModule.PlayerPositionChanged;
            FieldModule.LabelPositionChanged += PositionModule.LabelPositionChanged;
            FieldModule.PlayerPositionChanged(null, new PlayerPositionEventArgs(new List<int> { 1, 2, 3, 4, 5, 6 }));

            
        }

        private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Point[] pts = new System.Windows.Point[6];
            FieldModule.Points.CopyTo(pts, 0);
            int[] arrangementValues = new int[6];
            PositionModule.GetArrangementPoints().CopyTo(arrangementValues, 0);
            container[CurrentPhase].setArrangement(SetterPosition, pts, arrangementValues);
            //copy current

            int setterPos = ArrangementComboBox.SelectedIndex;
            SegmentPhase phase = (SegmentPhase)PhaseComboBox.SelectedItem;
            if (container[phase][setterPos] == null || isNullPoints(pts))
            {
                FieldModule.UpdatePoints();
                PositionModule.SetDefaultArrangementPoints();
            }
            else
            {
                FieldModule.UpdatePoints(container[phase][setterPos]);
                PositionModule.SetArrangementPoints(container[phase]._arrangementPositions[setterPos]);
            }

            CurrentPhase = phase;
            SetterPosition = setterPos;
        }
        private bool isNullPoints(System.Windows.Point[] pts)
        {
            for(int i  = 0; i < pts.Length; i++)
            {
                if (pts[i].X != 0 || pts[i].Y != 0) return false;
            }
            return true;
        }
        private void updateCurrentVisual()
        {
            if (container[CurrentPhase][SetterPosition] == null)
            {
                FieldModule.UpdatePoints();
                PositionModule.SetDefaultArrangementPoints();
            }
            else
            {
                FieldModule.UpdatePoints(container[CurrentPhase][SetterPosition]);
                PositionModule.SetArrangementPoints(container[CurrentPhase]._arrangementPositions[SetterPosition]);
            }
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxSelectionChanged(null, null);
            container.setForNull(FieldModule.GetBaseConvertedPoints());
           SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    container.Save(sw);
                }
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == true)
            {
                using(StreamReader sr =  new StreamReader(ofd.FileName))
                {
                    container = PlayerPositionDataContainer.Load(sr);
                    updateCurrentVisual();
                }
            }
        }
    }

}
