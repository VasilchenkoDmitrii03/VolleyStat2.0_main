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
                ComboBox comb = new ComboBox();
                comb.SelectionChanged += (o, e) =>
                {
                    MetricType type = a;
                    string shrtString =(string) ((ComboBox)o).SelectedItem;
                    _playerActionTextRepresentation.SetMetricByShortString(type, shrtString);
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
    }
}
