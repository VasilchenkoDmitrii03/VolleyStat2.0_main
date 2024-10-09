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
        public VideoModule VideoModule { get; private set; }
        public void SetFiltersController(AllActionMetricsController allActionMetricsController)
        {
            this.AllActionMetricsController = allActionMetricsController;
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
            MainListBox.ItemsSource = null;
            currentSeq = AllActionMetricsController.UseFilters(Sequence);
            MainListBox.ItemsSource = currentSeq;
        }
        private VolleyActionSequence currentSeq;
        private void MainListBox_Selected(object sender, RoutedEventArgs e)
        {
            
            int index = MainListBox.SelectedIndex;
            VideoModule.SetYouTubeTimeCode(((PlayerAction)currentSeq[index]).TimeCode - 2);
        }
    }
}
