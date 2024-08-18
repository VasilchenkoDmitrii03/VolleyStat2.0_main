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

namespace StatisticsCreatorModule
{
    public partial class ScoreDisplayer : UserControl
    {
        public ScoreDisplayer()
        {
            InitializeComponent();
            _last.Left = 0;
            _last.Right = 0;
        }
        Score _last = new Score(25);
        public void UpdateScore(Score score)
        {
            if(score.Right != _last.Right || score.Left != score.Right)
            {
                this.ScoreLabel.Content = $"{score.Left} : {score.Right}";
                this.ScoreListBox.Items.Add(new Label() { Content = $"{score.Left}:{score.Right}" });
                _last.Left = score.Left;
                _last.Right = score.Right;
            }
        }
        public void ScoreUpdated(object sender, ScoreEventArgs e)
        {
            UpdateScore(e.score);
        }
    }
}
