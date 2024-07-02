using System.Security.Cryptography.X509Certificates;
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
using ArrangmentUserControls;
namespace ArrangmentCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartArrangmentSetter.dragSource = TeamRepr.DragSource;
            Team tmp = new Team("abc");
            for(int i= 0;i < 10; ++i)
            {
                Player p = new Player(((char)i).ToString(), "Vais", 0, i, Amplua.Opposite);
                tmp.AddPlayer(p);
            }
            TeamRepr.Team = tmp;
            TeamRepr.Update();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartArrangmentSetter.RotateBackward();
        }
    }
}