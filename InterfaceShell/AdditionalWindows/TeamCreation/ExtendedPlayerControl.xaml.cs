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
    /// Логика взаимодействия для ExtendedPlayerControl.xaml
    /// </summary>
    /// 
    public delegate void Click(object sender, RoutedEventArgs e);
    public partial class ExtendedPlayerControl : UserControl
    {
        public int index { get; set; } = 0;
        public ExtendedPlayerControl(int index, Player p)
        {
            InitializeComponent();
            this.PlayerControl.SetPlayer(p);
            this.index = index;
        }
        public ExtendedPlayerControl(int index)
        {
            InitializeComponent();
            this.index = index;
        }
        public Player GetPlayer()
        {
            return PlayerControl.GetPlayer();
        }
        private void button_Click(object sender,  RoutedEventArgs e)
        {
            Click(this, e);
        }
        public Click Click;
        public PlayerControl PlayerControlForEvent
        {
            get { return PlayerControl; }
        }
    }
}
