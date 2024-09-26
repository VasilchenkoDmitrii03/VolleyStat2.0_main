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

namespace InterfaceShell.AdditionalWindows
{
    /// <summary>
    /// Логика взаимодействия для TeamCreater.xaml
    /// </summary>
    public partial class TeamCreater : Window
    {
        List<PlayerControl> players = new List<PlayerControl>();
        public TeamCreater()
        {
            InitializeComponent();
        }
        private void AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            PlayerControl playerControl = new PlayerControl();
            PlayersPanel.Children.Add(playerControl);  // Добавляем новый элемент в StackPanel
            players.Add(playerControl);  // Добавляем его в список для последующей обработки
        }
        private void RemovePlayer_Click(object sender, RoutedEventArgs e)
        {
            if (players.Count == 0) return;
            PlayersPanel.Children.RemoveAt(PlayersPanel.Children.Count - 1);  // Добавляем новый элемент в StackPanel
            players.Remove(players.Last());
        }
    }
}
