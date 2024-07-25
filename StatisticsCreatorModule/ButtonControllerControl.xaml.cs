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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StatisticsCreatorModule
{
    /// <summary>
    /// Interaction logic for ButtonControllerControl.xaml
    /// </summary>

    public delegate void MetricValueChanged(object sender, MetricValueEventArgs e);
    public partial class ButtonControllerControl : UserControl
    {

        public ActionsMetricTypes _actionMetricTypes;
        public ButtonControllerControl()
        {
            InitializeComponent();
            _currentObjectsList = new List<object>();
        }
        public void setActionMetricTypes(ActionsMetricTypes actionMetricTypes)
        {
            _actionMetricTypes = actionMetricTypes;
        }

        object _selectedObject = null;
        int _selectedIndex = -1;
        List<object> _selectedObjectsList = new List<Object>();
        List<Object> _currentObjectsList;

        public void updateButtons(List<Object> list)
        {
            MainPanel.Children.Clear();
            _currentObjectsList = list;
            bool _isClicked = false;
            foreach (Object obj in list)
            {
                Button button = new Button() { Margin = new Thickness(1), Width = 100, Height = 33, Content = obj.ToString() };
                button.Click += (o, e) =>
                { _isClicked = true;
                    _selectedObject = obj;
                    _currentObjectsList.Add(obj);
                    resultUpdated();
                    return;
                };
                MainPanel.Children.Add(button);
                if (_isClicked) return;
            }
        }
        private object resultUpdated()
        {
            //MainPanel.Children.Clear();
            MainLabel.Content = _selectedObject.ToString();
            return _selectedObject;
        }

        public Object SelectedObject
        {
            get { return _selectedObject; }
        }

        #region Event module
        public event MetricValueChanged MetricValueChangedInButtonModule;
        public void ActionTypeChangedInTextModule(object sender, ActionTypeEventArgs e)
        {
            getPlayerAction(_actionMetricTypes[e.VolleyActionType]);
        }
        public void MetricTypeChangedInTextModule(object sender, MetricTypeEventArgs e) 
        {
            MetricTypeComboBox.SelectedIndex = e.MetricIndex;
            _metricResultArray = e.AllMetrics;
        }
        #endregion


        /*
                #region MetricTypeList 
                //without combobox
                List<MetricType> _metricTypeList;
                List<Metric> _metricResultList;
                int _currentMetricIndex = -1;
                public void getPlayerAction(MetricTypeList mtl)
                {
                    _metricTypeList = mtl.MetricTypes;
                    _metricResultList = new List<Metric>();
                    _currentMetricIndex = -1;
                    nextStep();
                }
                private void nextStep()
                {
                    MainPanel.Children.Clear();
                    _currentMetricIndex++;

                    if (_currentMetricIndex < _metricTypeList.Count)
                    {
                        SupportLabel.Content = _metricTypeList[_currentMetricIndex].Name;
                        List<string> strings = _metricTypeList[_currentMetricIndex].AcceptableValuesNames.Values.ToList();
                        updateButtons(strings);
                    }
                    else
                    {
                        return;
                    }


                }
                public void updateButtons(List<string> list)
                {
                    MainPanel.Children.Clear();
                    int i = -1;
                    bool _isClicked = false;
                    foreach (string obj in list)
                    {
                        i++;
                        Button button = new Button() { Margin = new Thickness(1), Width = 100, Height = 33, Content = obj.ToString() };
                        button.Click += (o, e) =>
                        {
                            _isClicked = true;
                            addNewMetric(obj);
                            nextStep();
                        };
                        MainPanel.Children.Add(button);
                        if (_isClicked) return;
                    }
                }
                private void addNewMetric(string valueName)
                {
                    MetricType metricType = _metricTypeList[_currentMetricIndex];
                    object val = metricType.getObjectByLargeName(valueName);
                    _metricResultList.Add(new Metric(metricType, val));
                }
                public List<Metric> MetricListResult{
                    get { return _metricResultList; }
                    }
                #endregion
        */
        #region MetricTypeList 
        //with combobox
        ComboBox _metricsComboBox;
        List<MetricType> _metricTypeList;
        Metric[] _metricResultArray;
        int _currentMetricIndex = -1;
        public void getPlayerAction(MetricTypeList mtl)
        {
            _metricTypeList = mtl.MetricTypes;
            _metricResultArray = new Metric[_metricTypeList.Count];
            for (int i = 0; i < _metricTypeList.Count; i++) { _metricResultArray[i] = null; } //filling with nulls
            _metricsComboBox = MetricTypeComboBox;
            _metricsComboBox.Items.Clear();
            //_metricsComboBox.SelectionChanged += ComboBoxUpdated;
            foreach (MetricType metric in _metricTypeList)
            {
                _metricsComboBox.Items.Add(metric.Name);
            }
            //MainStackPanel.Children.Add(_metricsComboBox);
            _currentMetricIndex = 0;
            _metricsComboBox.SelectedIndex = _currentMetricIndex;
        }
        /*private void nextStep()
        {
            MainPanel.Children.Clear();
            MainPanel.Children.Add(_metricsComboBox);
            if (_metricsComboBox.SelectedIndex < _metricTypeList.Count)
            {
                SupportLabel.Content = _metricTypeList[_metricsComboBox.SelectedIndex].Name;
                List<string> strings = _metricTypeList[_metricsComboBox.SelectedIndex].AcceptableValuesNames.Values.ToList();
                updateButtons(strings);
            }
            else
            {
                return;
            }
        }*/
        private void ComboBoxUpdated(object sender, EventArgs e)
        {
           MainPanel.Children.Clear();

            if (_metricsComboBox.SelectedIndex < _metricTypeList.Count && _metricsComboBox.SelectedIndex >= 0)
            {
                SupportLabel.Content = _metricTypeList[_metricsComboBox.SelectedIndex].Name;
                List<string> strings = _metricTypeList[_metricsComboBox.SelectedIndex].AcceptableValuesNames.Values.ToList();
                updateButtons(strings);
            }
            else
            {
                return;
            }
        }
        public void updateButtons(List<string> list)
        {
            MainPanel.Children.Clear();
            int i = -1;
            bool _isClicked = false;
            foreach (string obj in list)
            {
                i++;
                Button button = new Button() { Margin = new Thickness(1), Width = 100, Height = 33, Content = obj.ToString() };
                button.Click += (o, e) =>
                {
                    _isClicked = true;
                    MetricValueChangedInButtonModule(this, new MetricValueEventArgs(getMetricByString(obj), MetricTypeComboBox.SelectedIndex, _metricResultArray));
                    addNewMetric(obj);
                    _currentMetricIndex = nextNullIndex();
                    if(_currentMetricIndex == -1)
                    {
                        //exit and job is done
                    }
                   
                    this._metricsComboBox.SelectedIndex = _currentMetricIndex;

                };
                MainPanel.Children.Add(button);
                if (_isClicked) return;
            }
        }
        private void addNewMetric(string valueName)
        {
            _metricResultArray[_metricsComboBox.SelectedIndex]  = getMetricByString(valueName);
        }
        private Metric getMetricByString(string valueName)
        {
            MetricType metricType = _metricTypeList[_metricsComboBox.SelectedIndex];
            object val = metricType.getObjectByLargeName(valueName);
            return new Metric(metricType, val);
        }
        private int nextNullIndex()
        {
            for (int i = 0; i < _metricResultArray.Length; i++)
            {
                if ((Object)_metricResultArray[i] == null) return i;
            }
            return -1;
        }
        public List<Metric> MetricListResult
        {
            get { return new List<Metric>(_metricResultArray); }
        }
        #endregion
    }
    public class MetricValueEventArgs : EventArgs
    {
        public Metric Metric { get; set; }
        public int MetricTypeIndex { get; set; }
        public Metric[] AllMetrics { get; set; }
        public MetricValueEventArgs(Metric vat, int metricTypeIndex, Metric[] allMetrics)
        {
            Metric = vat;
            MetricTypeIndex = metricTypeIndex;
            AllMetrics = allMetrics;
        }
    }
}

