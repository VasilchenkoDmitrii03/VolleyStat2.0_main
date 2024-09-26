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
    public partial class PlayerControl : UserControl
    {
        Player Player { get; set; }
        static List<Amplua> ampluas = new List<Amplua>() {Amplua.OutsideHitter, Amplua.MiddleBlocker, Amplua.Setter, Amplua.Opposite, Amplua.Libero };
        public PlayerControl()
        {
            InitializeComponent();
            this.DataContext = Player;
            RoleComboBox.ItemsSource = ampluas;
        }
    }
}
