using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
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

namespace ArrangmentUserControls
{
    /// <summary>
    /// Логика взаимодействия для test.xaml
    /// </summary>
    public partial class test : UserControl
    {
        ListBox dragSource = null;
        ObservableCollection<string> zoneList = new ObservableCollection<string>();
        public test()
        {
            InitializeComponent();
            foreach (TimeZoneInfo tzi in TimeZoneInfo.GetSystemTimeZones())

            {

                zoneList.Add(tzi.ToString());
            }

            bOne.ItemsSource = zoneList;

        }
    
        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            dragSource = parent;
            object data = GetDataFromListBox(dragSource, e.GetPosition(parent));
            if(data != null)
            {
                DragDrop.DoDragDrop(parent, data, DragDropEffects.Move);
            }
        }
        private static object GetDataFromListBox(ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if(element != null)
            {
                object data = DependencyProperty.UnsetValue;


                while (data == DependencyProperty.UnsetValue)
                {

                    data = source.ItemContainerGenerator.ItemFromContainer(element);
                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;

                    }
                    if (element == source)
                    {
                        return null;
                    }

                }
                if (data != DependencyProperty.UnsetValue)

                {
                    return data;
                }
            }
            return null;
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            object data = e.Data.GetData(typeof(string));
            ((IList<string>)dragSource.ItemsSource).Remove((string)data);
            parent.Items.Add(data);
        }
    }
    
}
