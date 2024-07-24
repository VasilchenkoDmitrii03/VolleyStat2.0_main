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
using System.Printing;

namespace StatisticsCreatorModule
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    /// 
    
    public delegate void ActionTypeChanged(object sender, ActionTypeEventArgs e);
    public delegate void MetricTypeChanged(object sender, MetricTypeEventArgs e);
    public partial class ActionTextRepresentationControl : UserControl
    {
        ActionsMetricTypes _actionsMetricTypes;
        List<VolleyActionType> _volleyActionTypes;
        PlayerActionTextRepresentation _playerActionTextRepresentation;


        #region Events
        public event ActionTypeChanged ActionTypeChangedInTextModule;
        public event MetricTypeChanged MetricTypeChangedInTextModule;
        
        public void MetricValueChangedInButtonModule(object sender, MetricValueEventArgs e)
        {
           Metric m =  e.Metric;
            ComboBox comb = _currentComboBoxes[e.MetricTypeIndex];
            comb.SelectedValue = m.getShortString();
        }
        #endregion

        public ActionTextRepresentationControl()
        {
            InitializeComponent();
        }

        public PlayerActionTextRepresentation PlayerActionTextRepresentation
        {
            get
            {
                return _playerActionTextRepresentation;
            }
        }

        public void setActionMetricsTypes(ActionsMetricTypes actionsMetricTypes)
        {
            _actionsMetricTypes = actionsMetricTypes;
            _volleyActionTypes = new List<VolleyActionType>() { VolleyActionType.Serve, VolleyActionType.Reception, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.Block, VolleyActionType.Defence, VolleyActionType.FreeBall, VolleyActionType.OpponentError, VolleyActionType.OpponentPoint };
            ActionTypeComboBox.Items.Clear();
            foreach (VolleyActionType a in _volleyActionTypes)
            {
                ActionTypeComboBox.Items.Add(a);
            }
        }
        public void clear()
        {
            this.ActionTypeComboBox.Text = "";
            MainGrid.ColumnDefinitions.RemoveRange(2, MainGrid.ColumnDefinitions.Count - 2);
            MainGrid.Children.RemoveRange(4, MainGrid.Children.Count - 4);
        }

        #region ComboBoxes
        private List<ComboBox> _currentComboBoxes  = new List<ComboBox>();
        private void ActionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!MyValidationForActTypeComboBox((ComboBox)sender, true)) return;
            VolleyActionType aType = (VolleyActionType)ActionTypeComboBox.SelectedItem;
            _playerActionTextRepresentation = new PlayerActionTextRepresentation(aType, _actionsMetricTypes[aType]);
            updateComboBoxes(aType);
            ActionTypeChangedInTextModule?.Invoke(this, new ActionTypeEventArgs(aType));
        }
        private void ActionTypeComboBox_SelectionChanged(object sender, KeyEventArgs e)
        {
            if (!MyValidationForActTypeComboBox((ComboBox)sender, false)) return;
            VolleyActionType aType = (VolleyActionType)ActionTypeComboBox.SelectedItem;
            _playerActionTextRepresentation = new PlayerActionTextRepresentation(aType, _actionsMetricTypes[aType]);
            updateComboBoxes(aType);
            ActionTypeChangedInTextModule?.Invoke(this, new ActionTypeEventArgs(aType));
        }
        public void updateComboBoxes(VolleyActionType aType)
        {
            MainGrid.ColumnDefinitions.RemoveRange(2, MainGrid.ColumnDefinitions.Count - 2);
            MainGrid.Children.RemoveRange(4, MainGrid.Children.Count - 4);
            MetricTypeList mtl = _actionsMetricTypes[aType];
            _currentComboBoxes.Clear();
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

                comb.GotFocus += (o, e) =>
                {
                    MetricType mt = a;
                    int ind = mtl.IndexOf(mt);
                    MetricTypeChangedInTextModule(this, new MetricTypeEventArgs(mt, ind));
                };
                comb.IsEditable = true;
                fillComboBoxItems(comb, a.ShortValuesNames);
                _currentComboBoxes.Add(comb);
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
        #endregion
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
        private bool MyValidationForActTypeComboBox(ComboBox comb, bool isChanged) 
        {
            bool result = false;

            string selected = (string)comb.Text;
            //if(selected == "" && comb.SelectedItem != null) selected = comb.SelectedItem.ToString();
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

        public bool Ready()
        {
            bool actType = MyValidationForActTypeComboBox(ActionTypeComboBox, false) || MyValidationForActTypeComboBox(ActionTypeComboBox, true);
            bool other = true;
            foreach(ComboBox comb in _currentComboBoxes)
            {
                other = other && MyValidationForMetricComboBoxes(comb);
            }
            return actType && other;
        }
        #endregion
    }
    public class ActionTypeEventArgs : EventArgs
    {
        public VolleyActionType VolleyActionType { get; set; }

        public ActionTypeEventArgs(VolleyActionType vat)
        {
            VolleyActionType = vat;
        }
    }
    public class MetricTypeEventArgs : EventArgs
    {
        public MetricType MetricType { get; set; }
        public int MetricIndex {  get; set; }
        public MetricTypeEventArgs(MetricType vat, int index)
        {
            MetricType = vat;
            MetricIndex = index;
        }
    }
}
