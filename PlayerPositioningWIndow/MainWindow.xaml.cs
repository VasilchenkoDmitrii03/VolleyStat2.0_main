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
namespace PlayerPositioningWIndow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            setup();
        }
        PlayerPositionDataContainer container = new PlayerPositionDataContainer();
        int SetterPositon = 0;
        SegmentPhase CurrentPhase = SegmentPhase.Recep_1;
        private void setup()
        {
            PhaseComboBox.Items.Add(SegmentPhase.Recep_1);
            PhaseComboBox.Items.Add(SegmentPhase.Recep);
            PhaseComboBox.Items.Add(SegmentPhase.Break);
            PhaseComboBox.SelectedIndex = 0;
            ArrangementComboBox.SelectedIndex = 0;

            PhaseComboBox.SelectionChanged += ComboBoxSelectionChanged;
            ArrangementComboBox.SelectionChanged += ComboBoxSelectionChanged;
            FieldModule.ReDraw();
            PositionModule.PlayerPositionsChanged += FieldModule.PlayerPositionChanged;
            FieldModule.LabelPositionChanged += PositionModule.LabelPositionChanged;
            FieldModule.PlayerPositionChanged(null, new PlayerPositionEventArgs(new List<int> { 1, 2, 3, 4, 5, 6 }));
        }

        private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Point[] pts = new System.Windows.Point[6];
            FieldModule.Points.CopyTo(pts, 0);
            container[CurrentPhase].setArrangement(SetterPositon, pts);
            //copy current
            int setterPos = ArrangementComboBox.SelectedIndex;
            SegmentPhase phase = (SegmentPhase)PhaseComboBox.SelectedItem;
            if (container[phase][setterPos] == null) FieldModule.UpdatePoints();
            else FieldModule.UpdatePoints(container[phase][setterPos]);
            CurrentPhase = phase;
            SetterPositon = setterPos;


        }

    }
}