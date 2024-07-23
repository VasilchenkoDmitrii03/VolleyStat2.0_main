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
    /// Логика взаимодействия для TextStatisticsModule.xaml
    /// </summary>
    public partial class TextStatisticsModule : UserControl
    {
        public TextStatisticsModule()
        {
            InitializeComponent();
            ActionsMetricTypes amt = ActionsMetricTypes.Load(@"C:\Dmitrii\Programming\VolleyStat2.0_main\BasicActionsMetrics");
            Test.LoadActionMetricsTypes(amt);
        }
    }
}
