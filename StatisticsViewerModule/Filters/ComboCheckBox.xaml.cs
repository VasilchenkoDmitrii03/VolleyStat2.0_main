using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace StatisticsViewerModule
{
    /// <summary>
    /// Логика взаимодействия для ComboCheckBox.xaml
    /// </summary>
    public partial class ComboCheckBox : UserControl
    {

        public ObservableCollection<Item> Items { get; set; }
        public event EventHandler ElementsChanged;
        public ComboCheckBox()
        {
            InitializeComponent();
            SelectionChangedEvent += (o, e) => { };
             Items = new ObservableCollection<Item>();
        }
        public void UpdateOptions(string[] shortString)
        {
            //Items.Clear();
            foreach (string s in shortString)
            {
                Item item = new Item() { Name = s, IsChecked = true };
                item.PropertyChanged += SelectionChanged;
                Items.Add(item);

            }
            ComboBoxWithCheckBox.ItemsSource = Items;
            UpdateText();
        }
        public void UpdateOptions(VolleyActionType[] shortString)
        {
            //Items.Clear();
            foreach (VolleyActionType s in shortString)
            {
                Item item = new Item() { Name = s.ToString(), ActionType = s, IsChecked = true };
                item.PropertyChanged += SelectionChanged;
                Items.Add(item);

            }
            ComboBoxWithCheckBox.ItemsSource = Items;
            UpdateText();
        }
        public List<string> SelectedOptions()
        {
            List<string> res = new List<string>();
            foreach (Item item in Items)
            {
                if (item.IsChecked == true) res.Add(item.Name);
            }
            return res;
        }
        public void SelectAll()
        {
            foreach (Item item in Items)
                item.IsChecked = true;
        }
        public void DeselectAll()
        {
            foreach (Item item in Items)
                item.IsChecked = false;
        }
        public void UpdateText()
        {
            List<string> strings = SelectedOptions();
            if (strings.Count == Items.Count) ComboBoxWithCheckBox.Text = "ANY";
            else if (strings.Count == 0) ComboBoxWithCheckBox.Text = "NONE";
            else
            {
                string text = strings[0];
                for(int i= 1;i < strings.Count; i++)
                {
                    text += $",{strings[i]}";
                }
                ComboBoxWithCheckBox.Text = text;
            }
        }
        public object SelectedValue
        {
            get { return ComboBoxWithCheckBox.SelectedValue; } }
        public event EventHandler SelectionChangedEvent;
        public void SelectionChanged(object sender, EventArgs e)
        {
            UpdateText();
        }

        private void ComboBoxWithCheckBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateText();
            SelectionChangedEvent(ComboBoxWithCheckBox, e);
        }

        private void CheckBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UpdateText();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            UpdateText();
        }
    }
    public class Item : INotifyPropertyChanged
    {
        private bool _isChecked;
        public string Name { get; set; }
        public VolleyActionType ActionType { get; set; } = VolleyActionType.Undefined;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void OnPropertyChanged(string propertyName, VolleyActionType aType)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
