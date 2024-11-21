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
using ActionsLib;

namespace StatisticsViewerModule
{
    /// <summary>
    /// Логика взаимодействия для StatisticsMetricsController.xaml
    /// </summary>
    public partial class StatisticsMetricsController : UserControl
    {
        public StatisticsMetricsController()
        {
            InitializeComponent();
        }
        public void AddMetric(MetricType m)
        {
            string[] strs= m.AcceptableValuesNames.Values.ToArray();
            MetricTypeStatSelector tmp = new MetricTypeStatSelector(m, strs);
            this.MainPanel.Children.Add(tmp);
            this.Selectors.Add(tmp);
        }
        public StatisticsMetricsController(MetricTypeList mtl)
        {
            foreach(MetricType mt in mtl)
            {
                AddMetric(mt);
            }
        }
        List<MetricTypeStatSelector> Selectors = new List<MetricTypeStatSelector>();
        public List<MetricTypeStatSelector> GetMetricTypeList()
        {
            return Selectors;
        }
    }
}
