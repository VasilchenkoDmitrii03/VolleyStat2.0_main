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

namespace StatisticsCreatorModule
{
    /// <summary>
    /// Interaction logic for ButtonControllerControl.xaml
    /// </summary>
    public partial class ButtonControllerControl : UserControl
    {
        public ButtonControllerControl()
        {
            InitializeComponent();
            _currentObjectsList = new List<object>();
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



        #region MetricTypeList
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
    }
}
