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
    public partial class FIeldVisualisationControl : UserControl
    {
        public FIeldVisualisationControl()
        {
            InitializeComponent();
            PlayerLabels = new PlayerLabel[6];
        }


        PlayerLabel[] PlayerLabels;
        const int border = 20;
        const int insideBorder = 10;
        private void DrawField()
        {
            MainCanvas.Children.Clear();
            try
            {
                Rectangle Field = new Rectangle() { Fill = Brushes.Orange };
                
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
            for(int i = 3; i >= 1; i--) // first line
            {
                PlayerLabels[i] = new PlayerLabel(new ActionsLib.Player("", "", 2, i, ActionsLib.Amplua.MiddleBlocker), null);
                PlayerLabels[i].Height = 50;
                PlayerLabels[i].Width = 50;
                MainCanvas.Children.Add(PlayerLabels[i]);
                Canvas.SetLeft(PlayerLabels[i], (this.ActualWidth - Field.Width) / 2 + (3 - i) * (Field.Width / 3) + insideBorder);
                Canvas.SetTop(PlayerLabels[i], this.ActualHeight / 2 + insideBorder);
            }

            for(int i = 4; i <=6; i++)
            {
                PlayerLabels[i%6] = new PlayerLabel(new ActionsLib.Player("", "", 2, i%6, ActionsLib.Amplua.MiddleBlocker), null);
                PlayerLabels[i%6].Height = 50;
                PlayerLabels[i%6].Width = 50;
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
            DrawField();
        }
    }
}
