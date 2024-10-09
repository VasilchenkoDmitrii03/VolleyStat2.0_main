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
using ActionsLib.ActionTypes;

namespace StatisticsCreatorModule
{
    /// <summary>
    /// Логика взаимодействия для ActionMetricTypeStatSelector.xaml
    /// </summary>
    public partial class ActionMetricTypeStatSelector : UserControl
    {
        ActionsMetricTypes ActionsMetricTypes { get; set; }
        public VolleyActionType volleyActionType { get; set; }
        public ActionMetricTypeStatSelector()
        {
            InitializeComponent();
        }
        public ActionMetricTypeStatSelector(VolleyActionType type,ActionsMetricTypes amt)
        {
            InitializeComponent();
            this.NameCheckBox.Content = type.ToString();
            foreach(MetricType mt in amt[type])
            {
                this.MetricsController.AddMetric(mt);
            }
                
        }
        public bool IsChecked
        {
            get { return (bool)NameCheckBox.IsChecked; }
        }
        public List<MetricTypeStatSelector> Selectors
        {
            get { return MetricsController.GetMetricTypeList(); }
        }
    }
}
