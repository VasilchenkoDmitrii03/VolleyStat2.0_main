using ActionsLib;
using ActionsLib.ActionTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        List<ActionsLib.Action> actions = new List<ActionsLib.Action>();
        Random rand =new Random();
        public TextStatisticsModule()
        {
            InitializeComponent();
            updateListVisual();
        }
        private void updateListVisual()
        {
            MainListBox.ItemsSource = null;
            MainListBox.ItemsSource = actions;
        }
        public void setActionMetricsTypes(ActionsMetricTypes amt)
        {
            Test.setActionMetricsTypes(amt);
        }
        public void setTeam(Team team)
        {
            this.LineRepresentationControl.setTeam(team);
        }
        public ActionTextRepresentationControl LineRepresentationControl
        {
            get { return Test; }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LineRepresentationControl.Ready())
            {
                MessageBox.Show("Fill Data correctly");
                return;
            }
            ActionsLib.Action act = LineRepresentationControl.ActionTextRepresentation.GenerateAction();
            actions.Add(act);
            updateListVisual();
            LineRepresentationControl.clear();
        }
    }
}
