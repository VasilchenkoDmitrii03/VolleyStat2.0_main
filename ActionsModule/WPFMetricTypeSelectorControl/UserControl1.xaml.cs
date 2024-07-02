using System.ComponentModel;
using System.Runtime.CompilerServices;
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

namespace WPFMetricTypeSelectorControl
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    /// 
    public delegate void CheckBoxUpdated();
    public partial class MetricTypeSelector : UserControl, INotifyPropertyChanged
    {
        public event CheckBoxUpdated OnCheckBoxUpdated;
        public MetricTypeList _totalList { get; set; }
        public MetricTypeList _displayedList { get; set; }
        public MetricTypeList _selectedList { get; set; }
        public List<CheckBox> _checkBoxes { get; set; }
        public MetricTypeSelector()
        {
            _checkBoxes = new List<CheckBox>();
            _totalList = new MetricTypeList("total");
            _displayedList = _totalList;
            _selectedList = new MetricTypeList("seleted");
            this.DataContext = this;
            InitializeComponent();
        }

        public void StackPanelUpdate()
        {
            Items.Children.Clear();
            int ind = 0;
            foreach(CheckBox checkBox in _checkBoxes)
            {
                if (DisplayedContains(checkBox))
                {
                    Items.Children.Add(checkBox);
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txt = ((TextBox)sender).Text;
            createDisplayList(txt);
            StackPanelUpdate();
        }
        private void createDisplayList(string txt = "")
        {
            if (txt == "") _displayedList = _totalList;
            else
            {
                _displayedList = new MetricTypeList("displayed");
                foreach(MetricType tmp in _totalList)
                {
                    if (tmp.ToString().Contains(txt))
                    {
                        _displayedList.Add(tmp);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public void updateTotalList(MetricTypeList lst)
        {
            _totalList = lst;
            int ind = 0;
            while (ind < _totalList.Count()) { 
                MetricType tmp = _totalList[ind];
                CheckBox chbox = new CheckBox() { Content = tmp.ToString(), IsChecked = false };
                chbox.Checked += (o, e) => {
                    _selectedList.Add(tmp);
                    OnPropertyChanged("_selectedList");
                    OnCheckBoxUpdated();
                };
                chbox.Unchecked+= (o, e) => {
                    if(_selectedList.Contains(tmp))_selectedList.Remove(tmp);
                    OnPropertyChanged("_selectedList");
                    OnCheckBoxUpdated();
                };
                _checkBoxes.Add(chbox);
                ind++;
            }
            SearchTextBox_TextChanged(SearchTextBox, null);
    }
        private void rebuildSelected()
        {
            _selectedList.Clear();
            for(int i= 0;i < _checkBoxes.Count;i++)
            {
                if (_checkBoxes[i].IsChecked == true)
                {
                    _selectedList.Add(_totalList[i]);
                }
            }
        }
        private bool DisplayedContains(CheckBox chbox)
        {
            foreach(MetricType mt in _displayedList)
            {
                if (mt.ToString() == (string)chbox.Content) return true;
            }
            return false;
        }
        public  void updateSelected(MetricTypeList mtl)
        {
            _selectedList.Clear();
            foreach (CheckBox c in _checkBoxes) c.IsChecked = false;
            foreach(MetricType mt in mtl)
            {
                foreach(CheckBox chb in _checkBoxes)
                {
                    if(mt.ToString() == (string)chb.Content)
                    {
                        chb.IsChecked = true;
                    }
                }
            }
        }
    }

}
