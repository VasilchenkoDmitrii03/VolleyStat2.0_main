using ActionsLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

namespace PlayerPositioningWIndow
{
    /// <summary>
    /// Логика взаимодействия для Field.xaml
    /// </summary>
    public partial class Field : UserControl
    {
        public LabelPositionChanged LabelPositionChanged;
        public Field()
        {
            InitializeComponent();
            PlayerLabels = new Label[6];
            for (int i = 0; i < PlayerLabels.Length; i++)
            {
                PlayerLabels[i] = new Label() { Width = 20, Height = 30 };
                setLabelEvents(PlayerLabels[i]);
                PlayerLabels[i].Content = (i + 1).ToString();
            }
            DrawField();
            Points = null;

        }
        Label[] PlayerLabels;
        public System.Windows.Point[] Points;
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

                for (int i = 0; i < PlayerLabels.Length; i++)
                {
                    MainCanvas.Children.Add(PlayerLabels[i]);
                }
                if (Points == null && _currentRectangle.Width > 0)
                {
                    Points = NormalizedBasePositions(_currentRectangle);
                    RelocateLabels(Points);
                }
            }
            catch (Exception ex) { }

        }
        private System.Windows.Point[] BasePositions(Rectangle Field)
        {
            System.Windows.Point[] points = new System.Windows.Point[6];
            for (int i = 3; i >= 1; i--) // first line
            {
                points[i] = new System.Windows.Point((this.ActualWidth - Field.Width) / 2 + (3 - i) * (Field.Width / 3) + insideBorder, this.ActualHeight / 2 + insideBorder);
                // Canvas.SetLeft(PlayerLabels[i], (this.ActualWidth - Field.Width) / 2 + (3 - i) * (Field.Width / 3) + insideBorder);
                //Canvas.SetTop(PlayerLabels[i], this.ActualHeight / 2 + insideBorder);
            }

            for (int i = 4; i <= 6; i++)
            {
                points[i % 6] = new System.Windows.Point((this.ActualWidth - Field.Width) / 2 + (i - 4) * (Field.Width / 3) + insideBorder, this.ActualHeight / 2 + Field.Height / 6 + insideBorder);
                //Canvas.SetLeft(PlayerLabels[i % 6], (this.ActualWidth - Field.Width) / 2 + (i - 4) * (Field.Width / 3) + insideBorder);
                //Canvas.SetTop(PlayerLabels[i % 6], this.ActualHeight / 2 + Field.Height / 6 + insideBorder);
            }
            return points;
        }
        private System.Windows.Point[] NormalizedBasePositions(Rectangle Field)
        {
            System.Windows.Point[] res = BasePositions(Field);
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = ConvertToPercentFormat(res[i]);
            }
            return res;
        }
        private void setBaseLocation()
        {
            System.Windows.Point[] points = BasePositions(_currentRectangle);
            for (int i = 0; i < 6; i++)
            {
                Canvas.SetLeft(PlayerLabels[i], points[i].X);
                Canvas.SetTop(PlayerLabels[i], points[i].Y);
            }
        }
        private void setLabelEvents(Label lab)
        {
            lab.MouseDown += Label_MouseDown;
            lab.MouseUp += Label_MouseUp;
            lab.MouseMove += Label_MouseMove;
        }
        private int[] getFieldSizes(int W, int H)
        {
            if (H < 2 * W)
            {
                return new int[] { H - 2 * border, (H - 2 * border) / 2 };
            }
            else
            {
                return new int[] { (W - 2 * border) * 2, W - 2 * border };
            }
        }


        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ReDraw();
        }
        public void ReDraw()
        {
            MainCanvas.Children.Clear();
            DrawField();
            if (Points != null) RelocateLabels(Points);
        }
        private void RelocateLabels(System.Windows.Point[] points)
        {
            for(int i = 0; i < 6; i++)
            {
                Canvas.SetLeft(PlayerLabels[i], points[i].X * _currentRectangle.Width + (this.ActualWidth - _currentRectangle.Width)/2);
                Canvas.SetTop(PlayerLabels[i], points[i].Y * _currentRectangle.Height + (this.ActualHeight - _currentRectangle.Height)/2);
            }
        }
        public void PlayerPositionChanged(object o, PlayerPositionEventArgs e)
        {
            List<int> ints = e.positions;
            System.Windows.Point[] points = NormalizedBasePositions(_currentRectangle);
            for (int i = 0; i < ints.Count; i++)
            {
                if (ints[i] > 0)
                {
                    if (Points != null)
                    {

                        Points[i] = points[i];
                    }
                }
            }
            if(Points != null)RelocateLabels(Points);
        }

        public void UpdatePoints(System.Windows.Point[] newPoints)
        {
            Points = newPoints;
            RelocateLabels(Points);
        }
        public void UpdatePoints()
        {
            Points = NormalizedBasePositions(_currentRectangle);
            RelocateLabels(Points);
        }

        public System.Windows.Point ConvertToPercentFormat(System.Windows.Point p)
        {
            double y = Canvas.GetTop(_currentRectangle);
            double x = Canvas.GetLeft(_currentRectangle);
            y = p.Y - y;
            x = p.X - x;
            return new System.Windows.Point(x / _currentRectangle.Width, y / _currentRectangle.Height);
        }
        private System.Windows.Point[] GetBaseConvertedPoints()
        {
            System.Windows.Point[] res = BasePositions(_currentRectangle);
            for(int i= 0;i < res.Length; i++)
            {
                res[i] = ConvertToPercentFormat(res[i]);
            }
            return res;
            
        }
        #region Drag&Drop
        private bool isDragging = false;
        private System.Windows.Point clickPosition;
        private UIElement draggedElement;

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            draggedElement = sender as UIElement;

            if (draggedElement != null)
            {
                isDragging = true;
                clickPosition = e.GetPosition(draggedElement);
                draggedElement.CaptureMouse();
                int index = Convert.ToInt32(((Label)draggedElement).Content);
                LabelPositionChanged(sender, new LabelPositionChangedEventArgs(index-1));
            }
        }

        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && draggedElement != null)
            {
                System.Windows.Point currentPosition = e.GetPosition(MainCanvas);

                double newLeft = currentPosition.X - clickPosition.X;
                double newTop = currentPosition.Y - clickPosition.Y;

                // Обновление позиции на Canvas
                Canvas.SetLeft(draggedElement, newLeft);
                Canvas.SetTop(draggedElement, newTop);

            }
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging && draggedElement != null)
            {
                isDragging = false;
                draggedElement.ReleaseMouseCapture();
                double x = Canvas.GetLeft(draggedElement);
                double y = Canvas.GetTop(draggedElement);
                int index = Convert.ToInt32(((Label)draggedElement).Content);
                Points[index-1] = ConvertToPercentFormat(new System.Windows.Point(x, y)); //adding to structure
                RelocateLabels(Points);
            }
        }
        #endregion
    }
    public delegate void LabelPositionChanged(object sender, LabelPositionChangedEventArgs e);
    public class LabelPositionChangedEventArgs : EventArgs
    {
        public int index = -1;
        public LabelPositionChangedEventArgs(int index)
        {
            this.index = index;
        }
    }
}
