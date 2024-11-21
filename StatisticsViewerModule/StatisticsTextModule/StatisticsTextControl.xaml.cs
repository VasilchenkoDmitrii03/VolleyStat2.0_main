using ActionsLib;
using StatisticsViewerModule;
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

namespace StatisticsViewerModule
{
    /// <summary>
    /// Interaction logic for StatisticsTextControl.xaml
    /// </summary>
    public partial class StatisticsTextControl : UserControl
    {
        public StatisticsTextControl()
        {
            InitializeComponent();
        }
        public VolleyActionSequence Sequence { get; set; } = new VolleyActionSequence();
        public AllActionMetricsController AllActionMetricsController { get; private set; }
        public StatisticsPlayersFilters PlayerFilters { get; private set; }
        public VideoModule VideoModule { get; private set; }
        public void SetFiltersController(AllActionMetricsController allActionMetricsController)
        {
            this.AllActionMetricsController = allActionMetricsController;
        }
        public void SetPlayersFilters(StatisticsPlayersFilters tmp)
        {
            PlayerFilters = tmp;
        }
        public void SetVideoModule(VideoModule videoModule)
        {
            VideoModule = videoModule;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        { 
            updateListBox();
        }
        private void updateListBox()
        {
            PlayerFilters.Update();
            MainListBox.ItemsSource = null;
            currentSeq = AllActionMetricsController.UseFilters(Sequence);
            currentSeq = PlayerFilters.PlayersFiltersHolder.ProcessSequence(currentSeq);
            MainListBox.ItemsSource = currentSeq;
        }
        private VolleyActionSequence currentSeq;
        private void MainListBox_Selected(object sender, RoutedEventArgs e)
        {
            int index = MainListBox.SelectedIndex;
            VideoModule.SetYouTubeTimeCode(((PlayerAction)currentSeq[index]).TimeCode - Convert.ToInt32(TimeShiftComboBox.Text));
        }
        public void SetActions(VolleyActionSequence seq)
        {
            Sequence = seq;
            updateListBox();
        }
    }
}
