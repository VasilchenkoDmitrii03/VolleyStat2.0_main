using ActionsLib;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
namespace InterfaceShell.AdditionalWindows.GameCreation
{
    /// <summary>
    /// Логика взаимодействия для ActionMetricTypesSelector.xaml
    /// </summary>
    /// 
    public partial class ActionMetricTypesSelector : UserControl
    {
        public ActionsMetricTypes _selectedAMT;
        List<ActionsMetricTypes> _AMTs;
        public ActionMetricTypesSelector()
        {
            InitializeComponent();
        }
        public void LoadBasicTeams(List<ActionsMetricTypes> amts)
        {
            this._AMTs = amts;
            this.BaseAMTComboBox.ItemsSource = _AMTs;
        }

        public event TeamUpdated TeamUpdated;
        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                    _selectedAMT = ActionsMetricTypes.Load(ofd.FileName);
            }
        }

        private void BaseTeamsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedAMT = (ActionsMetricTypes)this.BaseAMTComboBox.SelectedItem;
        }
    }
}
