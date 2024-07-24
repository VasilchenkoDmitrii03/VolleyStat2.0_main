using ActionsLib.ActionTypes;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ActionsLib.TextRepresentation;
using System.Globalization;

namespace StatisticsCreatorModule
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        ActionsMetricTypes _actionsMetricTypes;
        List<VolleyActionType> _volleyActionTypes;
        PlayerActionTextRepresentation _playerActionTextRepresentation;
        public UserControl1()
        {
            InitializeComponent();
        }

        public PlayerActionTextRepresentation PlayerActionTextRepresentation
        {
            get;set;
        }

        public void LoadActionMetricsTypes(ActionsMetricTypes actionsMetricTypes)
        {
            _actionsMetricTypes = actionsMetricTypes;
            _volleyActionTypes = new List<VolleyActionType>() { VolleyActionType.Serve, VolleyActionType.Reception, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.Block, VolleyActionType.Defence, VolleyActionType.FreeBall, VolleyActionType.OpponentError, VolleyActionType.OpponentPoint };
            ActionTypeComboBox.Items.Clear();
            foreach (VolleyActionType a in _volleyActionTypes)
            {
                ActionTypeComboBox.Items.Add(a);
            }
        }

        private void ActionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!MyValidationForActTypeComboBox((ComboBox)sender)) return;
            VolleyActionType aType = (VolleyActionType)ActionTypeComboBox.SelectedItem;
            _playerActionTextRepresentation = new PlayerActionTextRepresentation(aType, _actionsMetricTypes[aType]);
            updateComboBoxes(aType);
        }
        private void ActionTypeComboBox_SelectionChanged(object sender, KeyEventArgs e)
        {
            if (!MyValidationForActTypeComboBox((ComboBox)sender)) return;
            VolleyActionType aType = (VolleyActionType)ActionTypeComboBox.SelectedItem;
            _playerActionTextRepresentation = new PlayerActionTextRepresentation(aType, _actionsMetricTypes[aType]);
            updateComboBoxes(aType);
        }
        public void updateComboBoxes(VolleyActionType aType)
        {
            MainGrid.ColumnDefinitions.RemoveRange(2, MainGrid.ColumnDefinitions.Count - 2);
            MainGrid.Children.RemoveRange(4, MainGrid.Children.Count - 4);
            MetricTypeList mtl = _actionsMetricTypes[aType];
            int index = 2;
            foreach (MetricType a in mtl)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                Label lab = new Label() { Content = a.Name };
                ComboBox comb = new ComboBox() { Margin = new Thickness(1)};
                comb.Background = new SolidColorBrush(Colors.Purple);
                comb.KeyDown += (o, e) =>
                {
                    if (MyValidationForMetricComboBoxes((comb)))
                    {
                        MetricType type = a;
                        string shrtString = (string)((ComboBox)o).SelectedItem;
                        _playerActionTextRepresentation.SetMetricByShortString(type, shrtString);
                    }
                };
                comb.SelectionChanged +=
                    (o, e) =>
                    {
                        if (MyValidationForMetricComboBoxes((comb)))
                        {
                            MetricType type = a;
                            string shrtString = (string)((ComboBox)o).SelectedItem;
                            _playerActionTextRepresentation.SetMetricByShortString(type, shrtString);
                        }
                    };
                comb.IsEditable = true;
                fillComboBoxItems(comb, a.ShortValuesNames);
                MainGrid.Children.Add(lab);
                MainGrid.Children.Add(comb);
                Grid.SetColumn(lab, index);
                Grid.SetColumn(comb, index);
                Grid.SetRow(comb, 1);
                Grid.SetRow(lab, 0);
                index++;
            }

        }
        private void MetricComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void fillComboBoxItems(ComboBox comboBox, List<string> strs)
        {
            foreach (string str in strs)
            {
                comboBox.Items.Add(str);
            }
        }
        #region Validation
        private bool MyValidationForMetricComboBoxes(ComboBox comb)
        {
            bool result = false;
            string selected = (string)comb.SelectedItem;
            foreach (string str in comb.Items)
            {
                if (str == selected) { result = true; break; }
            }

            if (result)
            {
                comb.BorderThickness = new Thickness(1);
                comb.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                comb.BorderThickness = new Thickness(1);
                comb.Foreground = new SolidColorBrush(Colors.Red);
            }
            return result;
        }
        private bool MyValidationForActTypeComboBox(ComboBox comb) 
        {
            bool result = false;
            string selected = (string)comb.Text;
            foreach (VolleyActionType at in comb.Items)
            {
                if (selected == at.ToString()) { result = true; break; }
            }
            if (result)
            {
                comb.BorderThickness = new Thickness(1);
                comb.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                comb.BorderThickness = new Thickness(1);
                comb.Foreground = new SolidColorBrush(Colors.Red);
            }
            return result;
        }
        #endregion
    }
}
