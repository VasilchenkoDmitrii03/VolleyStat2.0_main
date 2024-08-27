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

namespace PlayerPositioningWIndow
{
    /// <summary>
    /// Логика взаимодействия для PlayerPositionController.xaml
    /// </summary>
    ///
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
            for(int i= 0; i < comboBoxes.Length; i++)
            {
                comboBoxes[i].SelectedIndex = i;
            }
        }
        ComboBox[] comboBoxes;
        public void CreateComboBoxes()
        {
            List<string> keys = new List<string>() {"1", "2", "3", "4", "5", "6", "Other" };
            comboBoxes = new ComboBox[6];
            for(int i = 0;i < comboBoxes.Length;i++)
            {
                comboBoxes[i] = new ComboBox();
                MainGrid.Children.Add(comboBoxes[i]);
                Grid.SetColumn(comboBoxes[i], 1);
                Grid.SetRow(comboBoxes[i], i + 1);
                comboBoxes[i].ItemsSource = keys;
                comboBoxes[i].SelectedIndex = i;
                comboBoxes[i].SelectionChanged += ComboBoxSelectionChanged;
            }
        }

        private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> strs = new List<string>();
            foreach (ComboBox comboBox in comboBoxes)
            {
                if (comboBox.SelectedValue == null) continue;
                strs.Add(comboBox.SelectedValue.ToString());
            }
            PlayerPositionsChanged(this, new PlayerPositionEventArgs(strs));
        }
        public void LabelPositionChanged(object sender, LabelPositionChangedEventArgs e)
        {
            comboBoxes[e.index].SelectedIndex = 6;
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
            for(int i= 0;i < strs.Count;i++)
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
