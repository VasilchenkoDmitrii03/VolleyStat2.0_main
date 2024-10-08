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

namespace StatisticsCreatorModule
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
            this.MainPanel.Children.Add(new MetricTypeStatSelector(m.Name, strs));
        }
        public StatisticsMetricsController(MetricTypeList mtl)
        {
            foreach(MetricType mt in mtl)
            {
                AddMetric(mt);
            }
        }
    }
}
