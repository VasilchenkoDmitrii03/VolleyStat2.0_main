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
    /// Логика взаимодействия для AllActionMetricsController.xaml
    /// </summary>
    public partial class AllActionMetricsController : UserControl
    {
        ActionsMetricTypes ActionMetricTypes;
        public AllActionMetricsController()
        {
            InitializeComponent();
        }
        public void UpdateAMT(ActionsMetricTypes actionMetricTypes)
        {
            this.ActionMetricTypes = actionMetricTypes;
            UpdateAllActionMetrics();
        }
        private void UpdateAllActionMetrics()
        {
            VolleyActionType[] array = { VolleyActionType.Serve, VolleyActionType.Reception, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.Block, VolleyActionType.Defence/*, VolleyActionType.FreeBall*/ };
            foreach(VolleyActionType actionType in array)
            {
                this.MainPanel.Children.Add(new ActionMetricTypeStatSelector(actionType, ActionMetricTypes));
            }
        }
    }
}
