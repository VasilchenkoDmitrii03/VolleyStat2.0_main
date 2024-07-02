using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
using ActionsLib;
using static System.Net.Mime.MediaTypeNames;
using MetricTypesWindow.Converters;
namespace MetricTypesWindow
{
    /// <summary>
    /// Interaction logic for MetricTypeCreator.xaml
    /// </summary>
    /// 
    public delegate void MetricTypeCreated(MetricType mt);
    public delegate void MetricTypeUpdated(MetricType mt, int index);
    public partial class MetricTypeCreator : Window
    {
        public event MetricTypeCreated OnMetricTypeCreated;
        public event MetricTypeUpdated OnMetricTypeUpdated;
        MetricType _currentMetricType;
        bool _editorMode;
        int _editorIndex;
        public string MetricName
        {
            get;set;
        }
        public string MetricDescription
        {
            get;set;
        }
        public string MetricShortName
        {
            get;set;
        }
        public Dictionary<object, string> Dict
        {
            get;set;
        }
        public List<string> ShortNamesList
        {
            get;set;
        }
        public MetricTypeCreator(MetricType metricType, int index)
        {
            _editorMode = true;
            _editorIndex = index;
            _currentMetricType = metricType;
            MetricShortName = metricType.ShortName;
            MetricName=metricType.Name;
            MetricDescription = metricType.Description;
           Dict = metricType.AcceptableValuesNames;
            ShortNamesList = metricType.ShortValuesNames;
            this.DataContext = this;
            InitializeComponent();
        }
        public MetricTypeCreator()
        {
            _editorMode = false;
            MetricName = "unknown";
            MetricDescription = "unknown";
            MetricShortName = "unknown";
            Dict = new Dictionary<object, string>();
            ShortNamesList = new List<string>();
            this.DataContext = this;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try { checkForCorrectness(); }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
                return;
            }
            IntegerMetricType tmp = new IntegerMetricType(MetricName, MetricDescription, MetricShortName, Dict, ShortNamesList);
            if (_editorMode) OnMetricTypeUpdated(tmp, _editorIndex);
            else OnMetricTypeCreated(tmp);
            this.Close();
        }
        private void checkForCorrectness()
        {
            //lists
            if(Dict.Count != ShortNamesList.Count)
            {
                throw new Exception("Not equal number of values and their short names");
            }
            var query = ShortNamesList.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();
            if(query.Count > 0)
            {
                throw new Exception("Short names must be different");
            }
            query = Dict.Values.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();
            if (query.Count > 0)
            {
                throw new Exception("Values must be different");
            }
        }
    }


}
