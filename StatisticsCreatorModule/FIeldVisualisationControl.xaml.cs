using Microsoft.Windows.Themes;
using StatisticsCreatorModule.Arrangment;
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

namespace StatisticsCreatorModule
{
    /// <summary>
    /// Логика взаимодействия для FIeldVisualisationControl.xaml
    /// </summary>
    /// 
    public delegate void PointsChanged(object sender, PointsEventArgs e);
    public partial class FIeldVisualisationControl : UserControl
    {
        public FIeldVisualisationControl()
        {
            InitializeComponent();
            PlayerLabels = new PlayerLabel[6];
            createPlayerLabels();
        }

        private void createPlayerLabels()
        {
            for(int i= 0; i<PlayerLabels.Length; i++)
            {
                PlayerLabels[i] = new PlayerLabel(null, null);
            }
        }

        List<ActionsLib.Point> _currentPoints = new List<ActionsLib.Point>();


        #region drawing

        PlayerLabel[] PlayerLabels;
        const int border = 20;
        const int insideBorder = 10;
        Rectangle _currentRectangle;
        private void DrawField()
        {
            MainCanvas.Children.Clear();
            try
            {
                Rectangle Field = new Rectangle() { Fill = Brushes.Orange };
                _currentRectangle = Field;
                int[] sizes = getFieldSizes(Convert.ToInt32(this.ActualWidth), Convert.ToInt32(this.ActualHeight));
                Field.Height = sizes[0];
                Field.Width = sizes[1];

                MainCanvas.Children.Add(Field);
                Canvas.SetLeft(Field, (this.ActualWidth - Field.Width) / 2);
                Canvas.SetTop(Field, (this.ActualHeight - Field.Height) / 2);

                //lines

                Line MiddleLine = new Line();
                MiddleLine.Stroke = new SolidColorBrush(Colors.White);
                MiddleLine.StrokeThickness = 3;
                MiddleLine.X1 = (this.ActualWidth - Field.Width) / 2;
                MiddleLine.X2 = MiddleLine.X1 + Field.Width;
                MiddleLine.Y1 = MiddleLine.Y2 = this.ActualHeight / 2;
                MainCanvas.Children.Add(MiddleLine);   
                //thirdMeterlines
                Line upper = new Line();
                upper.Stroke = new SolidColorBrush(Colors.White);
                upper.StrokeThickness = 1;
                upper.X1 = MiddleLine.X1; upper.X2 = MiddleLine.X2;
                upper.Y1 = upper.Y2 = MiddleLine.Y1 - Field.Height / 6; 
                MainCanvas.Children.Add(upper);

                Line lower = new Line();
                lower.Stroke = new SolidColorBrush(Colors.White);
                lower.StrokeThickness = 1;
                lower.X1 = MiddleLine.X1; lower.X2 = MiddleLine.X2;
                lower.Y1 = lower.Y2 = MiddleLine.Y1 + Field.Height / 6;
                MainCanvas.Children.Add(lower);

                RelocateLabels(Field);
            }
            catch (Exception ex) { }
            
        }
        private void RelocateLabels(Rectangle Field)
        {
            int LabelWidth =(int) Field.Width / 3 - insideBorder;
            int LabelHeight = (int) Field.Height / 6 -  insideBorder;
            for(int i = 3; i >= 1; i--) // first line
            {
                PlayerLabels[i].Height = LabelHeight;
                PlayerLabels[i].Width = LabelWidth;
                MainCanvas.Children.Add(PlayerLabels[i]);
                Canvas.SetLeft(PlayerLabels[i], (this.ActualWidth - Field.Width) / 2 + (3 - i) * (Field.Width / 3) + insideBorder);
                Canvas.SetTop(PlayerLabels[i], this.ActualHeight / 2 + insideBorder);
            }

            for(int i = 4; i <=6; i++)
            {
                PlayerLabels[i%6].Height = LabelHeight;
                PlayerLabels[i%6].Width = LabelWidth;
                MainCanvas.Children.Add(PlayerLabels[i % 6]);
                Canvas.SetLeft(PlayerLabels[i%6], (this.ActualWidth - Field.Width) / 2 + (i - 4) * (Field.Width / 3) + insideBorder);
                Canvas.SetTop(PlayerLabels[i%6], this.ActualHeight / 2 + Field.Height/ 6 + insideBorder);
            }
        }
        private int[] getFieldSizes(int W, int H)
        {
            if( H < 2 * W)
            {
                return new int[] { H - 2 * border, (H - 2 * border) / 2 };
            }
            else
            {
                return new int[] {( W - 2 * border) * 2, W - 2 * border};
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ReDraw();
        }

        private void DrawPoints()
        {
            foreach(ActionsLib.Point p in _currentPoints)
            {
                Rectangle point = new Rectangle() { Width = 5, Height = 5, Fill = Brushes.Black };
                MainCanvas.Children.Add(point);
                double x = (this.ActualWidth - _currentRectangle.Width) / 2 + p.X * _currentRectangle.Width;
                double y = (this.ActualHeight - _currentRectangle.Height) / 2 + p.Y * _currentRectangle.Height;
                Canvas.SetLeft(point, x);
                Canvas.SetTop(point, y);
            }
        }
        private void ReDraw()
        {
            MainCanvas.Children.Clear();
            DrawField();
            DrawPoints();
        }

        #endregion
        public void updateLabels(Arrangment.Arrangement arr)
        {
            for(int i= 0;i < 6; i++)
            {
                PlayerLabels[i].MainPlayer = arr[i];
            }
        }
        public void TeamControlChanged(object sender, TeamControlEventArgs e)
        {
            updateLabels(e._teamControl.CurrentArrangement);
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                double x = e.GetPosition(_currentRectangle).X;
                double y = e.GetPosition(_currentRectangle).Y;
                _currentPoints.Add(new ActionsLib.Point(x/ _currentRectangle.Width, y / _currentRectangle.Height));
                ReDraw();
                PointsChanged(this, new PointsEventArgs(_currentPoints));
            }
            catch { }
        }

        private void MainCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _currentPoints.Clear();
            ReDraw();
            PointsChanged(this, new PointsEventArgs(_currentPoints));
        }

        public void ClearPoints()
        {
            _currentPoints.Clear();
            ReDraw();
            PointsChanged(this, new PointsEventArgs(_currentPoints));
        }

        #region Events
        public PointsChanged PointsChanged;

        public void ActionAdded(object sender, EventArgs e)
        {
            ClearPoints();
        }
        #endregion

        #region Themese module
        private void LoadTheme()
        {
            ResourceDictionary themeDict = new ResourceDictionary();
            // Определяем, какая тема загружена в приложении
            if (Application.Current.Resources.MergedDictionaries[0].Source.ToString().Contains("LightTheme"))
            {
                themeDict.Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative);
            }
            else
            {
                themeDict.Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative);
            }

            this.Resources.MergedDictionaries.Add(themeDict);
        }
        public void UpdateTheme()
        {
            this.Resources.MergedDictionaries.Clear();
            LoadTheme();
        }
        #endregion


    }
    public class PointsEventArgs: EventArgs
    {
        public ActionsLib.Point[] currentPoints;
        public PointsEventArgs(List<ActionsLib.Point> points)
        {
            currentPoints = points.ToArray();
        }
    }
}
