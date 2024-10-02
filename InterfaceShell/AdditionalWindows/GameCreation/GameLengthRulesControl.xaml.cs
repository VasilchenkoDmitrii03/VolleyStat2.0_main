using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

namespace InterfaceShell.AdditionalWindows.GameCreation
{
    /// <summary>
    /// Логика взаимодействия для GameLengthRulesControl.xaml
    /// </summary>
    public partial class GameLengthRulesControl : UserControl
    {
        public GameLengthRulesControl()
        {
            InitializeComponent();
            this.SetNumberComboBox.ItemsSource = new List<int>() { 1, 3, 5, 7, 9,11 };
        }
        List<TextBox> textBoxes = new List<TextBox>();
        private void updateSetCount(int newCount)
        {
            this.MainGrid.Children.Clear();
            textBoxes.Clear();
            this.MainGrid.RowDefinitions.Clear();
            
            for(int i= 0;i < newCount; i++)
            {
                RowDefinition newRows = new RowDefinition();
                this.MainGrid.RowDefinitions.Add(newRows);
                TextBox textBox = new TextBox();
                Label label = new Label() { Content = $"Set #{i+1}"};
                this.textBoxes.Add(textBox);
                this.MainGrid.Children.Add(label);
                this.MainGrid.Children.Add(textBox);
                Grid.SetColumn(label, 0);
                Grid.SetColumn(textBox, 1);
                Grid.SetRow(label, i);
                Grid.SetRow(textBox, i);

            }
        }
        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int value = (int)SetNumberComboBox.SelectedValue;
            updateSetCount(value);
        }
        public List<int> getLengthes()
        {
            List<int> res = new List<int>();
            foreach(TextBox textBox in textBoxes)
            {
                try
                {
                    res.Add(Convert.ToInt32(textBox.Text));
                }
                catch
                {
                    MessageBox.Show("You can only input numbers in set leghts textboxes");
                }
            }
            return res;
        }
    }
}
