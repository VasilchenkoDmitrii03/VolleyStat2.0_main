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
using ActionsLib.ActionTypes;

namespace StatisticsCreatorModule.Arrangment
{
    public class PlayerLabel :  Label
    {
        Player Main, Change;
        public Player MainPlayer
        {
            get { return Main; }
            set
            {
                Main = value;
                updateVisual();
            }
        }
        public Player ChangePlayer
        {
            get { return Change; }
            set
            {
                Change = value;
                updateVisual();
            }
        }
        public PlayerLabel(Player main, Player fch) : base()
        {
            Main = main;
            ChangePlayer = fch;
        }
        private void updateVisual()
        {
            string cont = "";
            if (Main != null) cont = $"#{Main.Number}";
            else cont = "-";
            if (Change != null) cont += $" (#{Change.Number})";
            this.Content = cont; ;
            
        }
    }
}
