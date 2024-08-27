using ActionsLib;
using ActionsLib.ActionTypes;
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
    /// Interaction logic for ButtonControllerControl.xaml
    /// </summary>

    public delegate void ButtonSelectionChanged(object sender, ButtonSelectedChangedEventArgs e);
    public partial class ButtonControllerControl : UserControl
    {
        public ButtonControllerControl()
        {
            InitializeComponent();
        }

        public ButtonSelectionChanged ButtonSelectionChanged;
        public void ComboBoxUpdated(object o, ComboBoxEventArgs e)
        {
            ClearButtons();
            CreateNewButtons(e._comboBox);
            MainLabel.Content = e._actionType;
            SupportLabel.Content = e._metric;
        }

        #region Buttons
        List<Button> _currentButtons = new List<Button>();
        public void CreateNewButtons(ComboBox comb)
        {
            List<object> lst = new List<object>();
            foreach (object o in comb.Items)
            {
                lst.Add(o);
            }
            CreateNewButtons(lst);
        }
        public void CreateNewButtons(List<object> list)
        {
            _currentButtons = new List<Button>();
            int index = 0;
            foreach(object o in list)
            {
                Button button = new Button() {Width=100, Height=30 };
                button.Content = o.ToString();
                MainPanel.Children.Add(button);
                _currentButtons.Add(button);
                button.Click += CreateButtonClickEvent(index);
                index++;
            }
        }
        public void ClearButtons()
        {
            this.MainPanel.Children.Clear();
            this._currentButtons.Clear();
        }
        private RoutedEventHandler CreateButtonClickEvent(int index)
        {
            return (o, e) => {
                ButtonSelectionChanged(o, new ButtonSelectedChangedEventArgs(index));
            };
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
    public class ButtonSelectedChangedEventArgs : EventArgs
    {
        public int index = -1;
        public ButtonSelectedChangedEventArgs(int index = -1)
        {
            this.index = index;
        }
    }

}

