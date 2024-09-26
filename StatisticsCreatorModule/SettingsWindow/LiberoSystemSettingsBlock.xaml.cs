using StatisticsCreatorModule.Arrangment;
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

namespace StatisticsCreatorModule.SettingsWindow
{
    /// <summary>
    /// Логика взаимодействия для LiberoSystemSettingsBlock.xaml
    /// </summary>
    public partial class LiberoSystemSettingsBlock : UserControl
    {
        public LiberoSystemSettingsBlock()
        {
            InitializeComponent();
        }
        StatisticsCreatorModule.Arrangment.TeamControl _teamControl;
        int Count = 0;
        public void setTeamControl(TeamControl teamControl)
        {
            _teamControl = teamControl;
        }
        public void updateComboBoxesNumber(int number)
        {
            Count = number;
            createComboBoxes(number);
        }
        List<ComboBox> comboBoxes = new List<ComboBox>();
        private void createComboBoxes(int number)
        {
            MainGrid.Children.RemoveRange(2, MainGrid.Children.Count - 2);
            comboBoxes.Clear();
            List<ActionsLib.Player> liberos = _teamControl.getLiberoPlayers();
            for(int i = 0;i < number; i++)
            {
                ComboBox comb = new ComboBox();
                comb.ItemsSource = liberos;
                MainGrid.Children.Add(comb);
                Label lab = new Label() { Content = $"Libero #{i+1}" };
                MainGrid.Children.Add(lab);
                Grid.SetColumn(lab, 0);
                Grid.SetColumn(comb, 1);
                Grid.SetRow(comb, 1 + i);
                Grid.SetRow(lab, 1 + i);
                comboBoxes.Add(comb);
            }
        }

        public bool isDataFilledCorrectly()
        {
            bool isFilled = ChangingArrangementNumberComboBox.SelectedItem != null;
            foreach(ComboBox comboBox in comboBoxes)
            {
                isFilled = isFilled && comboBox.SelectedItem != null;
            }
            return isFilled;
        }
        public ActionsLib.Player[] GetLiberos()
        {
            ActionsLib.Player[] result = new ActionsLib.Player[comboBoxes.Count];
            for(int i = 0;i < comboBoxes.Count; i++)
            {
                result[i] = (ActionsLib.Player)comboBoxes[i].SelectedItem;
            }
            return result;
        }
        public int getChangingArrangement()
        {
            return ChangingArrangementNumberComboBox.SelectedIndex;
        }
    }
}
