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
using System.Windows.Shapes;
using ActionsLib;
using MetricTypesWindow;
using StatisticsCreatorModule;
namespace InterfaceShell.AdditionalWindows.Settings
{
    /// <summary>
    /// Логика взаимодействия для SettingsRedactorWindow.xaml
    /// </summary>
    public partial class SettingsRedactorWindow : Window
    {
        public SettingsRedactorWindow()
        {
            InitializeComponent();
        }

        bool isWindowOpened = false;
        Window currentWindow = null;
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow tmp = new MainWindow();
            if(currentWindow != null) { currentWindow.Close(); }
            tmp.Show();
            
            this.Close();
        }
        private void setEvent(Window window)
        {
            window.Closed += (o, e) => { isWindowOpened = false; currentWindow = null; };
        }
        private void MetricsButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentWindow != null) return;
            MetricTypeListEditor tmp = new MetricTypeListEditor();
            currentWindow = tmp;
            isWindowOpened = true;
            setEvent(tmp);
            tmp.Show();
        }
        private void MetricsActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentWindow != null) return;
            MetricTypesWindow.MainWindow tmp = new MetricTypesWindow.MainWindow();
            currentWindow = tmp;
            isWindowOpened = true;
            setEvent(tmp);
            tmp.Show();
        }
        private void LiberoButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentWindow != null) return;
            StatisticsCreatorModule.LiberoModeSetter.LiberoModeSetter tmp = new StatisticsCreatorModule.LiberoModeSetter.LiberoModeSetter();
            currentWindow = tmp;
            isWindowOpened = true;
            setEvent(tmp);
            tmp.Show();
        }
        private void ArrangementButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentWindow != null) return;
            StatisticsCreatorModule.PlayerPositioning.PlayerPositionWindow tmp = new StatisticsCreatorModule.PlayerPositioning.PlayerPositionWindow();
            currentWindow = tmp;
            isWindowOpened = true;
            setEvent(tmp);
            tmp.Show();
        }
    }
}
