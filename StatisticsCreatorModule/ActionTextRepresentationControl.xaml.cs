using ActionsLib.ActionTypes;
using ActionsLib;
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
using ActionsLib.TextRepresentation;
using System.Globalization;
using System.Printing;
using System.Diagnostics;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace StatisticsCreatorModule
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    /// 
    
    public delegate void ActionTypeChanged(object sender, ActionTypeEventArgs e);
    public delegate void MetricTypeChanged(object sender, MetricTypeEventArgs e);
    public partial class ActionTextRepresentationControl : UserControl
    {
        ActionsMetricTypes _actionsMetricTypes;
        List<VolleyActionType> _volleyActionTypes;
        ActionTextRepresentation _actionTextRepresentation;
        Team _team;


        #region Events
        public event ActionTypeChanged ActionTypeChangedInTextModule;
        public event MetricTypeChanged MetricTypeChangedInTextModule;
        
        public void MetricValueChangedInButtonModule(object sender, MetricValueEventArgs e)
        {
           Metric m =  e.Metric;
            ComboBox comb = _currentComboBoxes[e.MetricTypeIndex];
            comb.SelectedValue = m.getShortString();
        }
        #endregion

        public ActionTextRepresentationControl()
        {
            InitializeComponent();
            ActionTypeComboBox.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                      new System.Windows.Controls.TextChangedEventHandler(ActionTypeComboBox_SelectionChanged));
        }

        public ActionTextRepresentation ActionTextRepresentation
        {
            get
            {
                return _actionTextRepresentation;
            }
        }

        public void setActionMetricsTypes(ActionsMetricTypes actionsMetricTypes)
        {
            _actionsMetricTypes = actionsMetricTypes;
            _volleyActionTypes = new List<VolleyActionType>() { VolleyActionType.Serve, VolleyActionType.Reception, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.Block, VolleyActionType.Defence, VolleyActionType.FreeBall, VolleyActionType.OpponentError, VolleyActionType.OpponentPoint };
            ActionTypeComboBox.Items.Clear();
            
        }
        public void setTeam(Team team)
        {
            _team = team;
            PlayerComboBox.Items.Clear();
            foreach (Player p in _team.Players)
            {
                PlayerComboBox.Items.Add($"#{p.Number}");
            }
            PlayerComboBox.Items.Add(ActionAuthorType.OpponentTeam);
            PlayerComboBox.Items.Add(ActionAuthorType.Coach);
            PlayerComboBox.Items.Add(ActionAuthorType.Judge);
        }
        public void clear()
        {
            this.ActionTypeComboBox.Text = "";
            if(MainGrid.ColumnDefinitions.Count > 2)MainGrid.ColumnDefinitions.RemoveRange(2, MainGrid.ColumnDefinitions.Count - 2);
            if(MainGrid.Children.Count > 4) MainGrid.Children.RemoveRange(4, MainGrid.Children.Count - 4);
        }

        //Module for statistics creation

        ActionAuthorType _currentAuthorType;
        private void PlayerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!MyValidationForPlayerTypeComboBox(PlayerComboBox, true) || PlayerComboBox.SelectedItem == null) return;
            ActionAuthorType aat = ActionAuthorType.Undefined;
            if (PlayerComboBox.SelectedItem != null  && PlayerComboBox.SelectedItem.ToString().Length > 0 && PlayerComboBox.SelectedItem.ToString().StartsWith("#")) aat = ActionAuthorType.Player;
            foreach (ActionAuthorType a in new List<ActionAuthorType>() { ActionAuthorType.Judge, ActionAuthorType.OpponentTeam, ActionAuthorType.Coach })
            {
                if (a.ToString() == PlayerComboBox.SelectedItem.ToString()) aat = a;
            }
            _currentAuthorType = aat;
            UpdateActionTypeComboBoxItems(ActionTypeConverter.getActionTypesByAuthor(aat));
        }

        private void PlayerComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!MyValidationForPlayerTypeComboBox(PlayerComboBox, false)) return;
            ActionAuthorType aat = ActionAuthorType.Undefined;
            if (PlayerComboBox.SelectedItem.ToString().StartsWith("#")) aat = ActionAuthorType.Player;
            foreach (ActionAuthorType a in new List<ActionAuthorType>() { ActionAuthorType.Judge, ActionAuthorType.OpponentTeam, ActionAuthorType.Coach })
            {
                if (a.ToString() == PlayerComboBox.SelectedItem.ToString()) aat = a;
            }
            _currentAuthorType = aat;
            UpdateActionTypeComboBoxItems(ActionTypeConverter.getActionTypesByAuthor(aat));
        }

        private void UpdateActionTypeComboBoxItems(List<VolleyActionType> vats)
        {
            ActionTypeComboBox.Items.Clear();
            foreach (VolleyActionType a in vats)
            {
                ActionTypeComboBox.Items.Add(a);
            }
        }
        #region ComboBoxes for Players Action
        private List<ComboBox> _currentComboBoxes  = new List<ComboBox>();
        private void ActionTypeComboBox_SelectionChanged(object sender, EventArgs e)
        {
            if (!MyValidationForActTypeComboBox((ComboBox)sender, true)) return;
            VolleyActionType aType = (VolleyActionType)ActionTypeComboBox.SelectedItem;
            _actionTextRepresentation = createActionTextRepresentation(_currentAuthorType, aType);
            //
            //if (_currentAuthorType == ActionAuthorType.Player) _actionTextRepresentation = new PlayerActionTextRepresentation(aType, _actionsMetricTypes[aType]);
            updateComboBoxes(aType);
            //if(_currentAuthorType == ActionAuthorType.Player)ActionTypeChangedInTextModule?.Invoke(this, new ActionTypeEventArgs(aType));
        }
        private void ActionTypeComboBox_SelectionChanged(object sender, KeyEventArgs e)
        {
            if (!MyValidationForActTypeComboBox((ComboBox)sender, false)) return;
            VolleyActionType aType = (VolleyActionType)ActionTypeComboBox.SelectedItem;
            _actionTextRepresentation = createActionTextRepresentation(_currentAuthorType, aType);
            updateComboBoxes(aType);
            //if (_currentAuthorType == ActionAuthorType.Player) ActionTypeChangedInTextModule?.Invoke(this, new ActionTypeEventArgs(aType));
        }

        private ActionTextRepresentation createActionTextRepresentation(ActionAuthorType aat, VolleyActionType vat)
        {
            switch (aat)
            {
                case ActionAuthorType.Player:
                    PlayerActionTextRepresentation patr =  new PlayerActionTextRepresentation(vat, _actionsMetricTypes[vat]);
                    patr.SetPlayer(_team.Players[PlayerComboBox.SelectedIndex]);
                    return patr;
                case ActionAuthorType.Coach:
                    return new CoachActionTextRepresentation(vat);
                case ActionAuthorType.Judge:
                    return new JudgeActionTextRepresentation(vat);
                case ActionAuthorType.OpponentTeam:
                    return new OpponentActionTextRepresentation(vat);
            }
            return null;
        }

        public void updateComboBoxes(VolleyActionType aType)
        {
            switch (_currentAuthorType)
            {
                case ActionAuthorType.Player:
                    updateComboBoxesPlayerMode(aType);break;
                case ActionAuthorType.OpponentTeam:
                    updateComboBoxesOpponentMode(aType);break;
                case ActionAuthorType.Judge:
                    updateComboBoxesJudgeMode(aType);break;
                case ActionAuthorType.Coach:
                    updateComboBoxesCoachMode(aType); break;
            }
        }
        public void updateComboBoxesPlayerMode(VolleyActionType aType)
        {
            if (_currentAuthorType != ActionAuthorType.Player) return;
            if(MainGrid.ColumnDefinitions.Count > 2)MainGrid.ColumnDefinitions.RemoveRange(2, MainGrid.ColumnDefinitions.Count - 2);
            if (MainGrid.Children.Count > 4)  MainGrid.Children.RemoveRange(4, MainGrid.Children.Count - 4);
            MetricTypeList mtl = _actionsMetricTypes[aType];
            _currentComboBoxes.Clear();
            int index = 2;
            foreach (MetricType a in mtl)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                Label lab = new Label() { Content = a.Name };
                ComboBox comb = new ComboBox() {};
                comb.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                      new System.Windows.Controls.TextChangedEventHandler((o, e) =>
                      {
                          if (MyValidationForMetricComboBoxes((comb)))
                          {
                              MetricType type = a;
                              string shrtString = (string)((ComboBox)o).SelectedItem;
                              ((PlayerActionTextRepresentation)_actionTextRepresentation).SetMetricByShortString(type, shrtString);
                          }
                      }));
                comb.GotFocus += (o, e) =>
                {
                    MetricType mt = a;
                    int ind = mtl.IndexOf(mt);
                    MetricTypeChangedInTextModule(this, new MetricTypeEventArgs(mt, ind, ((PlayerActionTextRepresentation)_actionTextRepresentation)));
                };
                comb.IsEditable = true;
                fillComboBoxItems(comb, a.ShortValuesNames);
                _currentComboBoxes.Add(comb);
                MainGrid.Children.Add(lab);
                MainGrid.Children.Add(comb);
                Grid.SetColumn(lab, index);
                Grid.SetColumn(comb, index);
                Grid.SetRow(comb, 1);
                Grid.SetRow(lab, 0);
                index++;
            }

        }
        public void updateComboBoxesJudgeMode(VolleyActionType aType) { }
        public void updateComboBoxesCoachMode(VolleyActionType aType) 
        {
            switch (aType)
            {
                case VolleyActionType.TimeOut:
                    break;
                case VolleyActionType.Change:
                    _actionTextRepresentation = new CoachActionTextRepresentation(aType, new List<Player>(new Player[2]));
                    createPlayerNumberComboBoxes(2, _team);
                    break;
                case VolleyActionType.StartArrangment:
                    _actionTextRepresentation = new CoachActionTextRepresentation(aType, new List<Player>(new Player[6]));
                    createPlayerNumberComboBoxes(6, _team);
                    break;
            }
        }
        public void updateComboBoxesOpponentMode(VolleyActionType aType) { }
        private void MetricComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void fillComboBoxItems(ComboBox comboBox, List<string> strs)
        {
            foreach (string str in strs)
            {
                comboBox.Items.Add(str);
            }
        }

        private void createPlayerNumberComboBoxes(int count = 2, Team team = null)
        {
            _currentComboBoxes.Clear();
            if (MainGrid.ColumnDefinitions.Count > 2) MainGrid.ColumnDefinitions.RemoveRange(2, MainGrid.ColumnDefinitions.Count - 2);
            if (MainGrid.Children.Count > 4) MainGrid.Children.RemoveRange(4, MainGrid.Children.Count - 4);
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                Label lab = new Label();
                lab.Content = $"Pl#{i + 1}";
                ComboBox comboBox = new ComboBox();
                comboBox.IsEditable = true;
                comboBox.IsTextSearchCaseSensitive = true;
                comboBox.TabIndex = i;
                foreach (Player p in team.Players)
                {
                    comboBox.Items.Add($"#{p.Number}");
                }
                comboBox.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                      new System.Windows.Controls.TextChangedEventHandler((o,e) => {
                          if (!MyValidationForPlayerTypeComboBox((ComboBox)o, true)) return;
                          string txt = ((ComboBox)o).Text;
                          txt = txt.Remove(0, 1);
                          ((CoachActionTextRepresentation)_actionTextRepresentation).Players[((ComboBox)o).TabIndex] = team.GetPlayerByNumber(Convert.ToInt32(txt));
                      } ));

                MainGrid.Children.Add(lab);
                MainGrid.Children.Add(comboBox);
                Grid.SetColumn(lab, i+2);
                Grid.SetColumn(comboBox, i+2);
                Grid.SetRow(comboBox, 1);
                Grid.SetRow(lab, 0);
                index++;
            }
        }
        #endregion
        #region Validation
        private bool MyValidationForMetricComboBoxes(ComboBox comb)
        {
            bool result = false;
            string selected = (string)comb.SelectedItem;
            foreach (string str in comb.Items)
            {
                if (str == selected) { result = true; break; }
            }

            if (result)
            {
                comb.BorderThickness = new Thickness(1);
                comb.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                comb.BorderThickness = new Thickness(1);
                comb.Foreground = new SolidColorBrush(Colors.Red);
            }
            return result;
        }
        private bool MyValidationForActTypeComboBox(ComboBox comb, bool isChanged) 
        {
            bool result = false;

            string selected = (string)comb.Text;
            //if(selected == "" && comb.SelectedItem != null) selected = comb.SelectedItem.ToString();
            foreach (VolleyActionType at in comb.Items)
            {
                if (selected == at.ToString()) { result = true; break; }
            }
            if (result)
            {
                comb.BorderThickness = new Thickness(1);
                comb.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                comb.BorderThickness = new Thickness(1);
                comb.Foreground = new SolidColorBrush(Colors.Red);
            }
            return result;
        }
        private bool MyValidationForPlayerTypeComboBox(ComboBox comb, bool isChanged)
        {
             bool result = false;

            string selected = (string)comb.Text;
            if (selected == "" && comb.SelectedItem != null) selected = comb.SelectedItem.ToString();
            //if(selected == "" && comb.SelectedItem != null) selected = comb.SelectedItem.ToString();
            foreach (object at in comb.Items)
            {
                if (selected == at.ToString()) { result = true; break; }
            }
            if (result)
            {
                comb.BorderThickness = new Thickness(1);
                comb.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                comb.BorderThickness = new Thickness(1);
                comb.Foreground = new SolidColorBrush(Colors.Red);
            }
            return result;
        }

        public bool Ready()
        {
            bool actType = MyValidationForActTypeComboBox(ActionTypeComboBox, false) || MyValidationForActTypeComboBox(ActionTypeComboBox, true);
            bool other = true;
            foreach(ComboBox comb in _currentComboBoxes)
            {
                other = other && MyValidationForMetricComboBoxes(comb);
            }
            return actType && other;
        }
        #endregion

        #region Not Players Actions


        #endregion


    }
    public class ActionTypeEventArgs : EventArgs
    {
        public VolleyActionType VolleyActionType { get; set; }

        public ActionTypeEventArgs(VolleyActionType vat)
        {
            VolleyActionType = vat;
        }
    }
    public class MetricTypeEventArgs : EventArgs
    {
        public MetricType MetricType { get; set; }
        public int MetricIndex {  get; set; }
        public Metric[] AllMetrics { get; set; }
        public MetricTypeEventArgs(MetricType vat, int index, PlayerActionTextRepresentation patr)
        {
            MetricType = vat;
            MetricIndex = index;
            convert(patr);
        }
        private void convert(PlayerActionTextRepresentation patr)
        {
            AllMetrics = patr.Metrics;
        }

    }
}
