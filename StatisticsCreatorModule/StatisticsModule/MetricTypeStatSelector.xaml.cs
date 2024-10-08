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
    /// Логика взаимодействия для MetricTypeStatSelector.xaml
    /// </summary>
    public partial class MetricTypeStatSelector : UserControl
    {
        public MetricTypeStatSelector()
        {
            InitializeComponent();
        }
        public MetricTypeStatSelector(string name, string[] values)
        {
            InitializeComponent();
            NameLabel.Content = name;
            ComboCheckBox.UpdateOptions(values);
        }
        public List<string> SelectedOptions()
        {
            return ComboCheckBox.SelectedOptions();
        }
        public void SelectAll()
        {
            ComboCheckBox.SelectAll();
        }
        public void DeselectAll()
        {
            ComboCheckBox.DeselectAll();
        }
    }
}
