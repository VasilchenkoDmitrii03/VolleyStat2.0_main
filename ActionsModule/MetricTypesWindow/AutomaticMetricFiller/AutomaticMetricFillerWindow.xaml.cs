using ActionsLib;
using ActionsLib.ActionTypes;
using MetricTypesWindow.AutomaticMetricFiller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
           this.CondtionActionType_ComboBox.ItemsSource = types;
            this.ValueActionType_ComboBox.ItemsSource = types;
        }
        #region Action Tab
        private void ActionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateMetricTypeComboBoxes((VolleyActionType)ActionTypeComboBox.SelectedItem);
            //updateOptionsComboBox((VolleyActionType)ActionTypeComboBox.SelectedItem);
        }
        private void updateMetricTypeComboBoxes(VolleyActionType type)
        {
            List<MetricType> mtl = AMT[type].MetricTypes;
            this.AvaibleConditionMetricTypes_ComboBox.ItemsSource = null;
            this.AvaibleConditionMetricTypes_ComboBox.ItemsSource = mtl;
            this.RightSelectedMetricType_ComboBox.ItemsSource = null;
            this.RightSelectedMetricType_ComboBox.ItemsSource = mtl;
        }
        private void updateLeftOptionsComboBox(VolleyActionType type, int index)
        {
            this.LeftValuesSelector.ClearOptions();
            this.LeftValuesSelector.UpdateOptions(AMT[type][index].AcceptableValuesNames.Values.ToArray());
            this.LeftValuesSelector.DeselectAll();
        }
        private void updateRightOptionsComboBox(VolleyActionType type, int index)
        {
            RightValueComboBox.ItemsSource = null;
            RightValueComboBox.ItemsSource = AMT[type][index].AcceptableValuesNames.Values.ToArray();
        }

        private void RightSelectedMetricType_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateRightOptionsComboBox((VolleyActionType)ActionTypeComboBox.SelectedItem, RightSelectedMetricType_ComboBox.SelectedIndex);
        }

        private void AvaibleConditionMetricTypes_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateLeftOptionsComboBox((VolleyActionType)ActionTypeComboBox.SelectedItem, AvaibleConditionMetricTypes_ComboBox.SelectedIndex);
        }
        private void InAction_Click(object sender, RoutedEventArgs e)
        {
            VolleyActionType type = (VolleyActionType)ActionTypeComboBox.SelectedItem;
            MetricType right = AMT[type][RightSelectedMetricType_ComboBox.SelectedIndex];
            MetricType left = AMT[type][AvaibleConditionMetricTypes_ComboBox.SelectedIndex];
            string[] strs = LeftValuesSelector.SelectedOptions().ToArray();

            InActionAutomaticFiller inActionAutomaticFiller = new InActionAutomaticFiller(type, left, right, strs, RightValueComboBox.SelectedItem.ToString());


        }

        #endregion

        #region Sequence Tab
        private void CondtionActionType_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateMetricComboBox((VolleyActionType)CondtionActionType_ComboBox.SelectedItem, SequenceConditionMetricType_ComboBox);
        }
        private void updateMetricComboBox(VolleyActionType type, ComboBox comboBox)
        {
            List<MetricType> mtl = AMT[type].MetricTypes;

            comboBox.ItemsSource = null;
            comboBox.ItemsSource = mtl;
        }
        private void ValueActionType_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateMetricComboBox((VolleyActionType)ValueActionType_ComboBox.SelectedItem, SequenceValueMetricType_ComboBox);
        }

        private void SequenceConditionMetricType_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SequenceConditionValues_ComboBox.ClearOptions();
            this.SequenceConditionValues_ComboBox.UpdateOptions(AMT[(VolleyActionType)CondtionActionType_ComboBox.SelectedItem][SequenceConditionMetricType_ComboBox.SelectedIndex].AcceptableValuesNames.Values.ToArray());
            this.SequenceConditionValues_ComboBox.DeselectAll();
        }

        private void SequenceValueMetricType_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SequenceValue_ComboBox.ItemsSource = null;
            this.SequenceValue_ComboBox.ItemsSource = AMT[(VolleyActionType)ValueActionType_ComboBox.SelectedItem][SequenceValueMetricType_ComboBox.SelectedIndex].AcceptableValuesNames.Values.ToArray();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.SequenceConditionValues_ComboBox.IsEnabled = !(bool)((CheckBox)sender).IsChecked;
            this.SequenceValue_ComboBox.IsEnabled = !(bool)((CheckBox)sender).IsChecked;
        }
        #endregion
    }
}
