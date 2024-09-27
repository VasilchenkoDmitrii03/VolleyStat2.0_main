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
using ActionsLib;

namespace InterfaceShell.AdditionalWindows
{
    /// <summary>
    /// Логика взаимодействия для PlayerControl.xaml
    /// </summary>
    /// 
    public delegate void InfoWasChanged (object sender, EventArgs e);
    public partial class PlayerControl : UserControl
    {
        static List<Amplua> ampluas = new List<Amplua>() { Amplua.OutsideHitter, Amplua.MiddleBlocker, Amplua.Setter, Amplua.Opposite, Amplua.Libero };
        public PlayerControl()
        {
            InitializeComponent();
            RoleComboBox.ItemsSource = ampluas;
            InfoChanged += (o, e) => { };
        }
        public Player GetPlayer()
        {

            Player p = new Player(FirstNameTextBox.Text, LastNameTextBox.Text, Convert.ToInt32(HeightTextBox.Text), Convert.ToInt32(PlayerNumberTextBox.Text), (Amplua)RoleComboBox.SelectedItem);
            return p;
        }
        public void SetPlayer(Player p)
        {
            this.FirstNameTextBox.Text = p.Name;
            this.LastNameTextBox.Text = p.Surname;
            this.HeightTextBox.Text = p.Number.ToString();
            this.PlayerNumberTextBox.Text = p.Number.ToString();
            RoleComboBox.SelectedItem = p.Amplua;
        }
        public InfoWasChanged InfoChanged;
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            InfoChanged(sender, e);
        }
    }
}
