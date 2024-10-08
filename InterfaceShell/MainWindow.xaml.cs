using ActionsLib.ActionTypes;
using ActionsLib;
using InterfaceShell.AdditionalWindows;
using Microsoft.Win32;
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
using System.IO;
using StatisticsCreatorModule;
using InterfaceShell.AdditionalWindows.Settings;

namespace InterfaceShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public void CreateGame_Click(object sender, RoutedEventArgs e)
        {
            AdditionalWindows.GameCreation.GameCreateWindow tmp = new AdditionalWindows.GameCreation.GameCreateWindow();
            tmp.Show();
            this.Close();
        }
        public void OpenGame_Click(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            if (sfd.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(sfd.FileName))
                {
                    Game game = Game.Load(sr);
                    StatisticsCreatorModule.MainWindow tmp = new StatisticsCreatorModule.MainWindow(game.Team, game, game.ActionsMetricTypes);
                    tmp.Show();
                    this.Close();
                }  
            }
        }
        private void Link_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                textBlock.Foreground = Brushes.Red;  // Меняем цвет на красный при наведении
            }
        }

        // Возврат цвета при уходе мыши
        private void Link_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                textBlock.Foreground = Brushes.Blue;  // Возвращаем синий цвет
            }
        }

        // Действие при клике на "ссылку"
        private void Link_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StatisticsCreatorModule.MainWindow tmp = new StatisticsCreatorModule.MainWindow();
            tmp.Show();
            this.Close();
        }
        private void LinkSettings_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SettingsRedactorWindow tmp = new SettingsRedactorWindow();
            tmp.Show();
            this.Close();
        }
        private void LinkTeam_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AdditionalWindows.TeamCreater tmp = new AdditionalWindows.TeamCreater();
            tmp.Show();
            this.Close();
        }
    }
}