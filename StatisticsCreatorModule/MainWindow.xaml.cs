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
using ActionsLib.TextRepresentation;

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
        PlayerActionTextRepresentation _currentAction;
        ActionsMetricTypes _actionMetricTypes;
        Team _team;
        public MainWindow()
        {
            InitializeComponent();
            _actionMetricTypes = ActionsMetricTypes.Load(@"C:\Dmitrii\Programming\VolleyStat2.0_main\BasicActionsMetrics");
            
            _team = new Team();
            for(int  i = 1; i < 7; i++)
            {
                _team.AddPlayer(new Player("", "", 1, i, (Amplua)(i % 4)));
            }
            InitializeModules();
        }
        private void InitializeModules()
        {
            ButtonModule.setActionMetricTypes(_actionMetricTypes);
            TextModule.setActionMetricsTypes(_actionMetricTypes);
            TextModule.LineRepresentationControl.ActionTypeChangedInTextModule += ButtonModule.ActionTypeChangedInTextModule;
            TextModule.LineRepresentationControl.MetricTypeChangedInTextModule += ButtonModule.MetricTypeChangedInTextModule;
            ButtonModule.MetricValueChangedInButtonModule += TextModule.LineRepresentationControl.MetricValueChangedInButtonModule;
            TextModule.setTeam(_team);
            //TextModule.LineRepresentationControl.ActionTypeChangedInTextModule += test;
        }
        private void test(object sender, ActionTypeEventArgs e)
        {
            MessageBox.Show(e.VolleyActionType.ToString());
        }
    }

}