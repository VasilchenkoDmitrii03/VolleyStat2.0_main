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

namespace PlayerEditMessageBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    
    public delegate void WindowClosing(Player p);
    public partial class PlayerEditMsgBox : Window
    {

        WindowClosing  ok;
        public string PName { get; set; }
        public string Surname { get; set; }
        public int PHeight { get; set; }
        public int Number {  get; set; }
        public Amplua Amplua { get; set; }

        public PlayerEditMsgBox()
        {
            InitializeComponent();
            this.DataContext = this;
            AmpluaCombobox.ItemsSource = new List<Amplua>() {Amplua.OutsideHitter, Amplua.MiddleBlocker, Amplua.Setter, Amplua.Opposite, Amplua.Libero };
        }
        public PlayerEditMsgBox(Player p,  WindowClosing ok)
        {
            InitializeComponent();
            this.DataContext = this;
            AmpluaCombobox.ItemsSource = new List<Amplua>() { Amplua.OutsideHitter, Amplua.MiddleBlocker, Amplua.Setter, Amplua.Opposite, Amplua.Libero };
            PName = p.Name;
            Surname = p.Surname;
            PHeight = p.Height;
            Number  = p.Number;
            Amplua = p.Amplua;
            this.ok = ok;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            ok(new Player(PName, Surname, PHeight, Number, Amplua));
            
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}