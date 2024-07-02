using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ActionsLib;
using PlayerEditMessageBox;

namespace ArrangmentUserControls
{
    /// <summary>
    /// Логика взаимодействия для TeamRepresentationControl.xaml
    /// </summary>
    public partial class TeamRepresentationControl : UserControl
    {
        public Team Team { get; set; }
        public TeamRepresentationControl()
        {
            InitializeComponent();
        }
        public void Update()
        {
            this.DataContext = Team;
        }


        #region Editing Player
        Player changedPlayer;
        bool isEditingWindowOpened = false;
        public void PlayerChanging_ButtonClick(object sender, RoutedEventArgs e)
        {
            Player selected = null;
            int playerNumber = Convert.ToInt32(((TextBlock)((StackPanel)sender).Children[0]).Text);
            foreach (Player p in PlayersListBox.ItemsSource)
            {
                if (p.Number == playerNumber) { selected = p; break; }
            }
            changedPlayer = selected;
            PlayerEditMsgBox msg = new PlayerEditMsgBox(selected,OkPressed);
            msg.ShowDialog();
            isEditingWindowOpened = true;
        }
        private void OkPressed(Player p)
        {
            //НУЖНА ПРОВЕРКА
            
            List<Player> plr = (List<Player>)(PlayersListBox.ItemsSource);
            if (p.Number != changedPlayer.Number )
            {
                foreach(Player player in plr)
                {
                    if(player.Number == p.Number)
                    {
                        MessageBox.Show("Player with such number already exists");
                        PlayerEditMsgBox msg = new PlayerEditMsgBox(p, OkPressed);
                        msg.ShowDialog();
                        isEditingWindowOpened = true;
                        return;
                    }
                }
                
            }
            plr.Remove(changedPlayer);
            plr.Add(p);
            plr.Sort((x, y) => x.Number - y.Number);
            PlayersListBox.ItemsSource = null;
            PlayersListBox.ItemsSource = plr;
            isEditingWindowOpened = false;

        }
        #endregion

        #region DragDrop
        public ListBox DragSource { get { return PlayersListBox; } set { PlayersListBox = value; } }
        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            DragSource = parent;
            object data = GetDataFromListBox(DragSource, e.GetPosition(parent));
            if (data != null)
            {
                DragDrop.DoDragDrop(parent, data, DragDropEffects.Move);
            }
            Player a = new Player("", " ", 1, 1, Amplua.Setter);
            
        }
        private static object GetDataFromListBox(ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                object data = DependencyProperty.UnsetValue;


                while (data == DependencyProperty.UnsetValue)
                {

                    data = source.ItemContainerGenerator.ItemFromContainer(element);
                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;

                    }
                    if (element == source)
                    {
                        return null;
                    }

                }
                if (data != DependencyProperty.UnsetValue)

                {
                    return data;
                }
            }
            return null;
        }

        #endregion
    }
}
