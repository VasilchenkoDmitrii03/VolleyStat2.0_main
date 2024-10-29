using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ActionsLib;
using Microsoft.Win32;

namespace MetricTypesWindow
{
    /// <summary>
    /// Interaction logic for MetricTypeListEditor.xaml
    /// </summary>
    ///
    public partial class MetricTypeListEditor : Window, INotifyPropertyChanged
    {
        public List<MetricType> ListMetricTypes { get; set; }
        public string ListName { get; set; }
        MetricTypeList _currentList;
        bool _isMetricCreaterOpened = false;
        bool _isSaved = true;
        public MetricTypeListEditor()
        {
            this.PropertyChanged += unsaved;
            _currentList = new MetricTypeList("");
            ListName = _currentList.Name;
            ListMetricTypes = _currentList.MetricTypes;
            
            IntegerMetricType position = IntegerMetricType.createIntegerMetricType("position", "position on the playground", "pos", new int[] { 1, 2, 3, 4, 5, 6 });
            IntegerMetricType quality = IntegerMetricType.createIntegerMetricType("quality", "quality of any action from 1-6", "qual", new int[] { 1, 2, 3, 4, 5, 6 }, new string[] { "=", "-", "/", "!", "+", "#" }, new string[] { "=", "-", "/", "!", "+", "#" });
            ListMetricTypes.Add(position);
            ListMetricTypes.Add(quality);
            this.DataContext = this;
            InitializeComponent();
        }

        private void unsaved(object sender, PropertyChangedEventArgs e)
        {
            _isSaved = false;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = CurrentMetricTypes.SelectedIndex;
                ListMetricTypes.RemoveAt(index);
            }
            catch
            {
                MessageBox.Show("No item selected");
            }
            updateListBox();
        }
        private void updateListBox()
        {
            CurrentMetricTypes.SetBinding(ListBox.ItemsSourceProperty, "null");

            CurrentMetricTypes.SetBinding(ListBox.ItemsSourceProperty, "ListMetricTypes");
        }

        MetricType _editedMetricType;
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isMetricCreaterOpened)
                {
                    MessageBox.Show("One metric editor window is opened");
                    return;
                }
                _editedMetricType = (MetricType)CurrentMetricTypes.SelectedItem;
                if (CurrentMetricTypes.SelectedIndex == -1)
                {
                    throw new Exception("No item selected");
                }
                MetricTypeCreator t = new MetricTypeCreator(_editedMetricType, CurrentMetricTypes.SelectedIndex);
                t.Closed += (sender, e) => { _isMetricCreaterOpened = false; };
                t.OnMetricTypeUpdated += MetricTypeUpdated;
                t.Show();
                _isMetricCreaterOpened = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isMetricCreaterOpened)
            {
                MessageBox.Show("One metric editor window is opened");
                return;
            }
            MetricTypeCreator t = new MetricTypeCreator();
            t.Closed += (sender, e) => { _isMetricCreaterOpened = false; };
            t.OnMetricTypeCreated += MetricTypeCreated;
            t.Show();
            _isMetricCreaterOpened = true;
        }

        private void MetricTypeCreated(MetricType mt)
        {
            ListMetricTypes.Add(mt);
            updateListBox();
            this.Show();
            _isMetricCreaterOpened = false;
            _isSaved = false;
        }
        private void MetricTypeUpdated(MetricType mt, int index)
        {
            ListMetricTypes[index] = mt;
            updateListBox();
            this.Show();
            _isMetricCreaterOpened = false;
            _isSaved = false;
        }

        private bool CheckFill()
        {
            try
            {
                if (ListName == "")
                {
                    throw new Exception("Fill the name");
                }
                if (ListMetricTypes.Count == 0)
                {
                    throw new Exception("Fill metrics list");
                }
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        #region File
        string _currentFileName;

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CheckFill()) return;
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = @"Metric type list|*.metl";
                if (sfd.ShowDialog() == true)
                {
                    _currentFileName = sfd.FileName;
                    MetricTypeList mtl = new MetricTypeList(ListName);
                    mtl.Add(ListMetricTypes.ToArray());
                    mtl.Save(_currentFileName);
                }
                _isSaved = true;
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (!isSaved()) return;
            try
            {
                _currentFileName = "";
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = @"Metric type list|*.metl";
                if (ofd.ShowDialog() == true)
                {
                    _currentFileName = ofd.FileName;
                    MetricTypeList tmp = MetricTypeList.Load(_currentFileName);
                    ListName = tmp.Name;
                    ListMetricTypes = tmp.MetricTypes;
                    updateListBox();
                }
                _isSaved = true;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentFileName == "")
                {
                    SaveAs_Click(sender, e);
                }
                else
                {
                    MetricTypeList mtl = new MetricTypeList(ListName);
                    mtl.Add(ListMetricTypes.ToArray());
                    mtl.Save(_currentFileName);
                }
                _isSaved = true;
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            
        }
        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (!isSaved()) return;
            ListMetricTypes = new List<MetricType>();
            ListName = "";
            updateListBox();
            _isSaved = true;
        }

        private bool isSaved()
        {
            if (!_isSaved)
            {
                MessageBoxResult result = MessageBox.Show("Your file is not saved. Do you want to save it?", "Warning", MessageBoxButton.YesNoCancel);
                switch(result)
                {
                    case MessageBoxResult.Yes:
                        SaveAs_Click(null, null); return true;
                    case MessageBoxResult.No:
                        return true;
                    case MessageBoxResult.Cancel:
                        return false;

                }
            }
            return true;
        }
        #endregion


        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
