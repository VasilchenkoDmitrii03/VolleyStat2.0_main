using ActionsLib;
using ActionsLib.ActionTypes;
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
using ActionsLib;

namespace MetricTypesWindow
{
    /// <summary>
    /// Логика взаимодействия для AutomaticMetricFillerWindow.xaml
    /// </summary>
    public partial class AutomaticMetricFillerWindow : Window
    {
        ActionsMetricTypes AMT;
        public AutomaticFillersRulesHolder fillersHolder;
        public AutomaticMetricFillerWindow()
        {
            InitializeComponent();
            fillersHolder = new AutomaticFillersRulesHolder();
        }
        public AutomaticMetricFillerWindow(ActionsMetricTypes AMT, VolleyActionType[] types) : this()
        {
            this.AMT = AMT;
            this.ActionTypeComboBox.ItemsSource = types;
           this.CondtionActionType_ComboBox.ItemsSource = types;
            this.ValueActionType_ComboBox.ItemsSource = types;
            if (AMT.FillersRules == null) this.fillersHolder = new AutomaticFillersRulesHolder();
             else this.fillersHolder = AMT.FillersRules;
            CurrentSequenceActionRules_ListBox.ItemsSource = null;
            CurrentSequenceActionRules_ListBox.ItemsSource = fillersHolder.SequenceFillers;
            CurrentInActionRules_ListBox.ItemsSource = null;
            CurrentInActionRules_ListBox.ItemsSource = fillersHolder.InActionsFillers;
        }
        #region Action Tab
        private void clearInActionComboBoxes()
        {
            this.AvaibleConditionMetricTypes_ComboBox.Text = "" ;
            this.RightSelectedMetricType_ComboBox.Text = "";
            this.RightValueComboBox.Text = "";
            this.LeftValuesSelector.ClearOptions();
        }
        private void ActionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clearInActionComboBoxes();
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
            try
            {
                updateRightOptionsComboBox((VolleyActionType)ActionTypeComboBox.SelectedItem, RightSelectedMetricType_ComboBox.SelectedIndex);
            }
            catch { }
            
        }

        private void AvaibleConditionMetricTypes_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                updateLeftOptionsComboBox((VolleyActionType)ActionTypeComboBox.SelectedItem, AvaibleConditionMetricTypes_ComboBox.SelectedIndex);
            }
            catch { }
        }
        private void InAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VolleyActionType type = (VolleyActionType)ActionTypeComboBox.SelectedItem;
                MetricType right = AMT[type][RightSelectedMetricType_ComboBox.SelectedIndex];
                MetricType left = AMT[type][AvaibleConditionMetricTypes_ComboBox.SelectedIndex];
                string[] strs = LeftValuesSelector.SelectedOptions().ToArray();

                InActionAutomaticFiller inActionAutomaticFiller = new InActionAutomaticFiller(type, left, right, strs, RightValueComboBox.SelectedItem.ToString());
                fillersHolder.Add(inActionAutomaticFiller);
                CurrentInActionRules_ListBox.ItemsSource = null;
                CurrentInActionRules_ListBox.ItemsSource = fillersHolder.InActionsFillers;
            }
            catch
            {
                MessageBox.Show("Something went wrong, check input data");
            }


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
            try
            {
                updateMetricComboBox((VolleyActionType)ValueActionType_ComboBox.SelectedItem, SequenceValueMetricType_ComboBox);
            }
            catch { }
        }

        private void SequenceConditionMetricType_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.SequenceConditionValues_ComboBox.ClearOptions();
                this.SequenceConditionValues_ComboBox.UpdateOptions(AMT[(VolleyActionType)CondtionActionType_ComboBox.SelectedItem][SequenceConditionMetricType_ComboBox.SelectedIndex].AcceptableValuesNames.Values.ToArray());
                this.SequenceConditionValues_ComboBox.DeselectAll();
            }
            catch { }
        }

        private void SequenceValueMetricType_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.SequenceValue_ComboBox.ItemsSource = null;
                this.SequenceValue_ComboBox.ItemsSource = AMT[(VolleyActionType)ValueActionType_ComboBox.SelectedItem][SequenceValueMetricType_ComboBox.SelectedIndex].AcceptableValuesNames.Values.ToArray();
            }
            catch { }
           
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.SequenceConditionValues_ComboBox.IsEnabled = !(bool)((CheckBox)sender).IsChecked;
            this.SequenceValue_ComboBox.IsEnabled = !(bool)((CheckBox)sender).IsChecked;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VolleyActionType leftAct = (VolleyActionType)CondtionActionType_ComboBox.SelectedItem;
            VolleyActionType rightAct = (VolleyActionType)ValueActionType_ComboBox.SelectedItem;
            MetricType left = AMT[leftAct][SequenceConditionMetricType_ComboBox.SelectedIndex];
            MetricType right = AMT[rightAct][SequenceValueMetricType_ComboBox.SelectedIndex];
            string[] leftValues = SequenceConditionValues_ComboBox.SelectedOptions().ToArray();
            string rightValue = (string)SequenceValue_ComboBox.SelectedItem;
            bool isCopying = (bool)CopyingChecker.IsChecked;
            if(isCopying)
            {
                SequenceAutomaticFiller sequenceAutomaticFiller = new SequenceAutomaticFiller(leftAct, rightAct, left, right, isCopying);
                fillersHolder.Add(sequenceAutomaticFiller);
            }
            else
            {
                SequenceAutomaticFiller sequenceAutomaticFiller = new SequenceAutomaticFiller(leftAct, rightAct, left, right, leftValues, rightValue);
                fillersHolder.Add(sequenceAutomaticFiller);
            }
            
            CurrentSequenceActionRules_ListBox.ItemsSource = null;
            CurrentSequenceActionRules_ListBox.ItemsSource = fillersHolder.SequenceFillers;
        }
        #endregion
       public AutomaticFillersRulesHolder AutomaticFillersRulesHolder
        {
            get { return fillersHolder; }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //AMT.FillersRules = fillersHolder;
            this.Close();
        }
        
    }
}
