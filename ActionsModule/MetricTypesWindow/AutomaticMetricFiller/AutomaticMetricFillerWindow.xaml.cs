using ActionsLib;
using ActionsLib.ActionTypes;
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

namespace MetricTypesWindow
{
    /// <summary>
    /// Логика взаимодействия для AutomaticMetricFillerWindow.xaml
    /// </summary>
    public partial class AutomaticMetricFillerWindow : Window
    {
        ActionsMetricTypes AMT;
        public AutomaticMetricFillerWindow()
        {
            InitializeComponent();
        }
        public AutomaticMetricFillerWindow(ActionsMetricTypes AMT, VolleyActionType[] types) : this()
        {
            this.AMT = AMT;
            this.ActionTypeComboBox.ItemsSource = types;
           
        }

        private void ActionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateOptionsComboBox((VolleyActionType)ActionTypeComboBox.SelectedItem);
        }
        private void updateOptionsComboBox(VolleyActionType type)
        {
            this.LeftValuesSelector.ClearOptions();
            this.LeftValuesSelector.UpdateOptions(AMT[type][0].AcceptableValuesNames.Values.ToArray());
            this.LeftValuesSelector.DeselectAll();
        }
    }
}
