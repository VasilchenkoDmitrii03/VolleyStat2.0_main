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
using ActionsLib.ActionTypes;
using ActionsLib;

namespace StatisticsCreatorModule
{
    /// <summary>
    /// Interaction logic for ActionTextRepresentationLineControl.xaml
    /// </summary>
    public partial class ActionTextRepresentationLineControl : UserControl
    {
        ActionsMetricTypes _actionsMetricTypes;
        List<VolleyActionType> _volleyActionTypes;
        public ActionTextRepresentationLineControl(ActionsMetricTypes actionsMetricTypes)
        {
            InitializeComponent();
            _actionsMetricTypes = actionsMetricTypes;
            _volleyActionTypes = new List<VolleyActionType>() {VolleyActionType.Serve, VolleyActionType.Reception, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.Block, VolleyActionType.Defence, VolleyActionType.FreeBall, VolleyActionType.OpponentError, VolleyActionType.OpponentPoint };
            ActionTypeComboBox.Items.Clear();
            foreach(VolleyActionType a in _volleyActionTypes)
            {
                ActionTypeComboBox.Items.Add(a);
            }
        }

        private void ActionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        public void updateComboBoxes()
        {
            MainGrid.ColumnDefinitions.RemoveRange(2, MainGrid.ColumnDefinitions.Count - 2);
            MetricTypeList mtl = _actionsMetricTypes[(VolleyActionType)ActionTypeComboBox.SelectedItem];
            int index = 2;
            foreach (MetricType a in mtl)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                Label lab = new Label() { Content = a.Name };
                ComboBox comb = new ComboBox();
                MainGrid.Children.Add(lab);
                MainGrid.Children.Add(comb);
                Grid.SetColumn(lab, index);
                Grid.SetColumn(comb, index);
                Grid.SetRow(comb, 1);
                Grid.SetRow(lab, 0);
            }

        }
    }
}
