using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ActionsLib;
using TeamArrangmentModule;
namespace ArrangmentUserControls
{
    /// <summary>
    /// Логика взаимодействия для StartArrangmentSetterControl.xaml
    /// </summary>
    public partial class StartArrangmentSetterControl : UserControl
    {
        public StartArrangmentSetterControl()
        {
            InitializeComponent();
            createPlayersListBoxes();
            reDrawField();
        }
        public Arrangment Arrangment
        {
            get
            {
                Player[] players = new Player[6];
                for(int i= 0;i < players.Length; i++)
                {
                    if (_listBoxes[i].Items.Count > 0)
                    {
                        players[i] = ((PlayerTextBlock)_listBoxes[i].Items[0]).Player;
                    }
                }
                return new Arrangment(players);
            }
        }

        #region Creating ListBoxex
        ListBox[] _listBoxes = new ListBox[6];
        private void createPlayersListBoxes()
        {
            for(int i = 1;i < 3; i++)
            {
                for(int j = 0;j < 3; ++j)
                {
                    ListBox lstb = new ListBox() {Margin= new Thickness(5,5,5,5), Background = Brushes.LightBlue,AllowDrop= true };
                    lstb.Drop += ListBox_Drop;
                    MainGrid.Children.Add(lstb);
                    Grid.SetRow(lstb, i);
                    Grid.SetColumn(lstb, j);
                    _listBoxes[convertIndex(i - 1, j)] = lstb;
                }
            }
        }
        public void RotateForward()
        {
            ListBox p = _listBoxes[0];
            for (int i = 0; i < _listBoxes.Length-1; i++)
            {
                _listBoxes[i] = _listBoxes[i + 1];
            }
            _listBoxes[5] = p;
            updateGrid();
        }
        public void RotateBackward()
        {
            for (int i = 0; i < 5; ++i) RotateForward();
        }
        private void updateGrid()
        {
            MainGrid.Children.Clear();
            for (int i = 0; i < _listBoxes.Length; ++i)
            {
                int j =0, k = 0;
                convertIndexBack(i, ref j, ref k);
                MainGrid.Children.Add(_listBoxes[i]);
                Grid.SetColumn(_listBoxes[i], k);
                Grid.SetRow(_listBoxes[i], j + 1);
            }
        }
        private int convertIndex(int i, int j)
        {
            if (i == 0 && j == 0) return 3;
            if (i == 0 && j == 1) return 2;
            if (i == 0 && j == 2) return 1;
            if (i == 1 && j == 0) return 4;
            if(i == 1 && j == 1) return 5;
            if (i == 1 && j == 2) return 0;
            return 0;
        }
        private void convertIndexBack(int k, ref int i, ref int j)
        {
            if (k == 0) { i = 1; j = 2; }
            if (k == 1) { i = 0; j = 2; }
            if (k == 2) { i = 0; j = 1; }
            if (k == 3) { i = 0; j = 0; }
            if (k == 4) { i = 1; j = 0; }
            if (k == 5) { i = 1; j = 1; }
        }
        #endregion

        #region Drawing 

        const int BORDERDISTANCE = 10;
        private void reDrawField()
        {
            try
            {
                this.MainCanvas.Height = this.ActualHeight;
                this.MainCanvas.Width = this.ActualWidth;

                MainGrid.Height = this.Height - 2 * BORDERDISTANCE;

                MainGrid.Width = this.Width - 2 * BORDERDISTANCE;
                Rectangle rect = new Rectangle() { Fill = Brushes.Gray, Width = this.ActualWidth - 2 * BORDERDISTANCE, Height = this.ActualHeight - 2 * BORDERDISTANCE };
                Canvas.SetLeft(rect, BORDERDISTANCE);
                Canvas.SetTop(rect, BORDERDISTANCE);
                MainCanvas.Children.Add(rect);
                //draw midle line
                MainCanvas.Children.Add(createHorizontalLine((int)Height / 2, Brushes.White, 3, false));
                //3-rd meter lines
                MainCanvas.Children.Add(createHorizontalLine((int)rect.Height / 3 + BORDERDISTANCE, Brushes.White, 2, true));
                //
                MainCanvas.Children.Add(createHorizontalLine((int)rect.Height * 2 / 3 + BORDERDISTANCE, Brushes.White, 2, true));
            }
            catch
            {

            }
            

        }
        private Line createHorizontalLine(int y, Brush b , int width, bool dash = false)
        {
            try
            {
                Line midLine = new Line();
                midLine.X1 = BORDERDISTANCE;
                midLine.X2 = this.Width - BORDERDISTANCE;
                midLine.Y1 = y;
                midLine.Y2 = y;
                midLine.Stroke = b;
                midLine.StrokeThickness = width;
                if (dash) midLine.StrokeDashArray = new DoubleCollection(new List<double>() { 4, 2 });
                return midLine;
            }
            catch 
            {
                return new Line();
            }
        }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            reDrawField();
        }

        #endregion

        #region DragDrop
        public ListBox dragSource
        {
            get;set;
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            if(parent.Items.Count == 0) // if no player selected
            {
                object data = e.Data.GetData(typeof(Player));
                List<Player> updatedList = (List<Player>)dragSource.ItemsSource;
                updatedList.Remove((Player)data);
                updatedList.Sort((x, y) => x.Number - y.Number);
                dragSource.ItemsSource = null;
                dragSource.ItemsSource = updatedList;
                dragSource.UpdateLayout();
                parent.Items.Add(new PlayerTextBlock() { Width = 30, Height = 30, Text = $"{((Player)data).Number}", Foreground = Brushes.Blue, Player = (Player)data });
            }
            else // if player were selected earlier
            {
                Player currentPlayer = ((PlayerTextBlock)parent.Items[0]).Player;
                Player data = (Player)e.Data.GetData(typeof(Player));
                List<Player> updatedList = (List<Player>)dragSource.ItemsSource;
                updatedList.Remove(data);
                updatedList.Add(currentPlayer);
                updatedList.Sort((x, y) => x.Number - y.Number);
                dragSource.ItemsSource = null;
                dragSource.ItemsSource = updatedList;

                parent.Items.Clear();
                parent.Items.Add(new PlayerTextBlock() { Width = 30, Height = 30, Text = $"{((Player)data).Number}", Foreground = Brushes.Blue, Player = (Player)data });

            }
        }

        #endregion
    }
}
