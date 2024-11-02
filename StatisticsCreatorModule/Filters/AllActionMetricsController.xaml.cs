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
using StatisticsCreatorModule.StatisticsModule;

namespace StatisticsCreatorModule
{
    /// <summary>
    /// Логика взаимодействия для AllActionMetricsController.xaml
    /// </summary>
    public partial class AllActionMetricsController : UserControl
    {
        ActionsMetricTypes ActionMetricTypes;
        Dictionary<VolleyActionType, ActionMetricTypeStatSelector> ActionMetricTypesSelectors = new Dictionary<VolleyActionType, ActionMetricTypeStatSelector>();
        FiltersHolder FiltersHolder;
        VolleyActionType[] array;
        public AllActionMetricsController()
        {
            InitializeComponent();
             array = new VolleyActionType[] { VolleyActionType.Serve, VolleyActionType.Reception, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.Block, VolleyActionType.Defence, VolleyActionType.FreeBall };
            ActionTypeComboBox.ItemsSource = array;
            ActionTypeComboBox.SelectionChanged += ActionTypeSelectionChanged;
        }
        public void UpdateAMT(ActionsMetricTypes actionMetricTypes)
        {
            this.ActionMetricTypes = actionMetricTypes;
            FiltersHolder = new FiltersHolder(actionMetricTypes, array);
            ActionMetricTypesSelectors.Clear();
            foreach (VolleyActionType actionType in ActionTypeComboBox.Items)
            {
                ActionMetricTypesSelectors.Add(actionType, new ActionMetricTypeStatSelector(actionType, ActionMetricTypes));
            }
        }
        private void ActionTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.MainPanel.Children.Clear();
            this.MainPanel.Children.Add(ActionMetricTypesSelectors[(VolleyActionType)ActionTypeComboBox.SelectedItem]);
        }
        public List<MetricTypeStatSelector> GetMetricSelectors(VolleyActionType actType)
        {
            return ActionMetricTypesSelectors[actType].Selectors;
        }
        private void UpdateFilters()
        {
            FiltersHolder.Clear();
            foreach (VolleyActionType aType in ActionTypeComboBox.Items)
            {
                if (ActionMetricTypesSelectors[aType].IsChecked)
                {
                   FiltersHolder.update(aType, GetMetricSelectors(aType));
                }
            }
        }
        public VolleyActionSequence UseFilters(VolleyActionSequence vas)
        {
            UpdateFilters();
            return FiltersHolder.ProcessSequence(vas);
        }
    }
}
