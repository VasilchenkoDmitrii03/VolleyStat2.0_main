using System.Security.Cryptography.X509Certificates;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       /* public MainWindow()
        {
            InitializeComponent();
            ActionsMetricTypes atl = ActionsMetricTypes.Load(@"C:\Dmitrii\Programming\VolleyStat2.0_main\BasicActionsMetrics");
            MetricTypeList mtl = atl[VolleyActionType.Attack];
            //ButtonController.getPlayerAction(mtl);
            ButtonController.updateButtons(new List<Object> { VolleyActionType.Serve, JudgeActionType.CardGift });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //List<Metric> tmp = ButtonController.MetricListResult;
            Object o = ButtonController.SelectedObject;
            Type t = o.GetType();
        }*/

        public MainWindow()
        {
            InitializeComponent();
           
            
        }
    }

}