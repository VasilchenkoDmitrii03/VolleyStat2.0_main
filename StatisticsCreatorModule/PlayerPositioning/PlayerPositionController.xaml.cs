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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StatisticsCreatorModule.PlayerPositioning
{
    /// <summary>
    /// Логика взаимодействия для PlayerPositionController.xaml
    /// </summary>
    public delegate void PlayerPositionsChanged(object sender, PlayerPositionEventArgs e);
    public partial class PlayerPositionController : UserControl
    {
        public PlayerPositionsChanged PlayerPositionsChanged;
        public PlayerPositionController()
        {
            InitializeComponent();
            CreateComboBoxes();
            setDefault();
        }
        public void setDefault()
        {
            for (int i = 0; i < zoneComboBoxes.Length; i++)
            {
                zoneComboBoxes[i].SelectedIndex = i;
            }
        }
        ComboBox[] zoneComboBoxes;
        ComboBox[] arrangementComboBoxes;
        public void CreateComboBoxes()
        {
            List<string> zoneKeys = new List<string>() { "1", "2", "3", "4", "5", "6", "Other" };
            List<string> arrangementKeys = new List<string>() { "1", "2", "3", "4", "5", "6" };
            zoneComboBoxes = new ComboBox[6];
            arrangementComboBoxes = new ComboBox[6];
            for (int i = 0; i < zoneComboBoxes.Length; i++)
            {
                zoneComboBoxes[i] = new ComboBox();
                MainGrid.Children.Add(zoneComboBoxes[i]);
                Grid.SetColumn(zoneComboBoxes[i], 1);
                Grid.SetRow(zoneComboBoxes[i], i + 1);
                zoneComboBoxes[i].ItemsSource = zoneKeys;
                zoneComboBoxes[i].SelectedIndex = i;
                zoneComboBoxes[i].SelectionChanged += ComboBoxSelectionChanged;
                //
                //
                arrangementComboBoxes[i] = new ComboBox();
                MainGrid.Children.Add(arrangementComboBoxes[i]);
                Grid.SetColumn(arrangementComboBoxes[i], 2);
                Grid.SetRow(arrangementComboBoxes[i], i + 1);
                arrangementComboBoxes[i].ItemsSource = arrangementKeys;
                arrangementComboBoxes[i].SelectedIndex = i;

            }

        }
        public int[] GetArrangementPoints()
        {
            int[] res = new int[6];
            for(int i = 0; i <  arrangementComboBoxes.Length; i++) res[i] = arrangementComboBoxes[i].SelectedIndex;
            return res;
        }
        public void SetArrangementPoints(int[] arrangementPoints)
        {
            for(int i= 0;i < 6; i++)
            {
                arrangementComboBoxes[i].SelectedIndex = arrangementPoints[i];
            }
        }
        public void SetDefaultArrangementPoints()
        {
            for (int i = 0; i < 6; i++)
            {
                arrangementComboBoxes[i].SelectedIndex = i;
            }
        }

        private void updateArrangementComboBoxSelection(ComboBox changedBox)
        {
            for(int i= 0;i < 6; i++)
            {
                if (changedBox == zoneComboBoxes[i])
                {
                    if(changedBox.SelectedIndex < 6)
                    {
                        arrangementComboBoxes[i].SelectedIndex = changedBox.SelectedIndex;
                    }
                    return;
                }
            }
            
        }

        private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateArrangementComboBoxSelection((ComboBox)sender);
            List<string> strs = new List<string>();
            foreach (ComboBox comboBox in zoneComboBoxes)
            {
                if (comboBox.SelectedValue == null) continue;
                strs.Add(comboBox.SelectedValue.ToString());
            }
            PlayerPositionsChanged(this, new PlayerPositionEventArgs(strs));
        }
        public void LabelPositionChanged(object sender, LabelPositionChangedEventArgs e)
        {
            zoneComboBoxes[e.index].SelectedIndex = 6;
            arrangementComboBoxes[e.index].SelectedIndex = e.ClosestArrangementLocation;
        }
    }
    public class PlayerPositionEventArgs : EventArgs
    {
        public List<int> positions;
        public PlayerPositionEventArgs(List<int> positions)
        {
            this.positions = positions;
        }
        public PlayerPositionEventArgs(List<string> strs)
        {
            positions = new List<int>(new int[6]);
            for (int i = 0; i < strs.Count; i++)
            {
                if (strs[i] == "Other")
                {
                    positions[i] = -1;
                }
                else
                {
                    positions[i] = Convert.ToInt32(strs[i]);
                }
            }
        }
    }
}
