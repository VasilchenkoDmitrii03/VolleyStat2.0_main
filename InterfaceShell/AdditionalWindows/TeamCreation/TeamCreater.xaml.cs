using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ActionsLib;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;

namespace InterfaceShell.AdditionalWindows
{
    /// <summary>
    /// Логика взаимодействия для TeamCreater.xaml
    /// </summary>
    public partial class TeamCreater : Window
    {
        List<ExtendedPlayerControl> players = new List<ExtendedPlayerControl>();
        public TeamCreater()
        {
            InitializeComponent();
            wasChanged = false;
        }

        string currentPath = "";
        bool wasChanged = false;

        private void AddPlayer_Click(object sender, RoutedEventArgs e)
        {

            AddPlayer();
        }
        private void AddPlayer()
        {
            ExtendedPlayerControl playerControl = new ExtendedPlayerControl(players.Count);
            playerControl.Click += Remove_Click;
            playerControl.Click += WasChanged;
            playerControl.PlayerControlForEvent.InfoChanged += WasChanged;
            PlayersPanel.Children.Add(playerControl);  // Добавляем новый элемент в StackPanel
            players.Add(playerControl);  // Добавляем его в список для последующей обработки
            renumeratePlayers();
        }
        private void AddPlayer(Player p)
        {
            ExtendedPlayerControl playerControl = new ExtendedPlayerControl(players.Count, p);
            playerControl.Click += Remove_Click;
            playerControl.Click += WasChanged;
            playerControl.PlayerControlForEvent.InfoChanged += WasChanged;
            PlayersPanel.Children.Add(playerControl);  // Добавляем новый элемент в StackPanel
            players.Add(playerControl);  // Добавляем его в список для последующей обработки
            renumeratePlayers();
        }
        private void renumeratePlayers()
        {
            for(int i= 0;i <players.Count; i++)
            {
                players[i].index = i;
            }
        }
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            int index = ((ExtendedPlayerControl)sender).index;
            players.RemoveAt(index);
            PlayersPanel.Children.RemoveAt(index);
            renumeratePlayers();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if(sfd.ShowDialog() == true)
            {
                System.Drawing.Color libero = Convert((System.Windows.Media.Color)LiberocolorPicker.SelectedColor);
                System.Drawing.Color main = Convert((System.Windows.Media.Color)MaincolorPicker.SelectedColor);
                Team team = new Team(TeamNameTextBox.Text, TeamDescriptionTextBox.Text, main, libero, getPlayersList());
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    team.Save(sw);
                }
                currentPath = sfd.FileName;
                wasChanged = false;
            }
        }
        private System.Drawing.Color Convert(System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(currentPath == "") SaveAs_Click(sender, e);
            else
            {
                System.Drawing.Color libero = Convert((System.Windows.Media.Color)LiberocolorPicker.SelectedColor);
                System.Drawing.Color main = Convert((System.Windows.Media.Color)MaincolorPicker.SelectedColor);
                Team team = new Team(TeamNameTextBox.Text, TeamDescriptionTextBox.Text, main, libero, getPlayersList());
                using (StreamWriter sw = new StreamWriter(currentPath))
                {
                    team.Save(sw);
                    wasChanged = false;
                }
            }
        }
        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (isThereUnsavedInfo() != MessageBoxResult.No) return;
            Team t = new Team("", "", System.Drawing.Color.Black, System.Drawing.Color.White, new List<Player>());
            LoadTeam(t);
            currentPath = "";
            wasChanged = false;
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            if (isThereUnsavedInfo() != MessageBoxResult.No) return;
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    Team t = Team.Load(sr);
                    LoadTeam(t);
                    currentPath = ofd.FileName;
                    wasChanged = false;
                }
            }
        }
        private void LoadTeam(Team team)
        {
            TeamNameTextBox.Text = team.Name;
            TeamDescriptionTextBox.Text = team.Description;
            LiberocolorPicker.SelectedColor = System.Windows.Media.Color.FromRgb(team.LiberoColor.R, team.LiberoColor.G, team.LiberoColor.B);
            MaincolorPicker.SelectedColor = System.Windows.Media.Color.FromRgb(team.MainColor.R, team.MainColor.G, team.MainColor.B);
            PlayersPanel.Children.Clear();
            foreach(Player player in team.Players)
            {
                AddPlayer(player);
            }
        }
        private List<Player> getPlayersList()
        {
            List<Player> res = new List<Player>();
            foreach(ExtendedPlayerControl con in PlayersPanel.Children)
            {
                res.Add(con.GetPlayer());
            }
            return res;
        }

        private MessageBoxResult isThereUnsavedInfo()
        {
            if (wasChanged)
            {
               return  System.Windows.MessageBox.Show(
            "Save changes?",     // Сообщение
            "There are unsaved data",                      // Заголовок
            MessageBoxButton.YesNoCancel,          // Кнопки
            MessageBoxImage.Question);
            }
            return MessageBoxResult.No;
        }
        private void WasChanged(object sender, EventArgs e)
        {
            wasChanged = true;
        }
    }
}
