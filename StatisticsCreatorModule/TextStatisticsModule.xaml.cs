using ActionsLib;
using ActionsLib.ActionTypes;
using StatisticsCreatorModule.Arrangment;
using StatisticsCreatorModule.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    /// Логика взаимодействия для TextStatisticsModule.xaml
    /// </summary>
    /// 
    public delegate void ArrangmentChanged(object sender, TeamControlEventArgs e);
    public delegate void ActionAdded(object sender, EventArgs e);
    public delegate void ScoreUpdated(object sender, ScoreEventArgs e);
    public delegate void GamePhaseForGraphicsChanged(object sender, PhaseEventArgs e);
    public partial class TextStatisticsModule : UserControl
    {
        List<ActionsLib.Action> _actions = new List<ActionsLib.Action>();
        Random rand =new Random();
        TeamControl _teamControl = null;
        VolleyActionSegmetRules rules = new VolleyActionSegmetRules();
        
        public TextStatisticsModule()
        {
            InitializeComponent();
            updateListVisual();
            rules.setDefaultRules();
            BeginNewSet(5);
            AddButton.TabIndex = 40;
            LineRepresentationControl.DataFilled += AddButton_Click;
        }

        public event ArrangmentChanged ArrangementChanged;
        public event ActionAdded ActionAdded;
        public event ScoreUpdated ScoreUpdated;
        public event GamePhaseForGraphicsChanged GamePhaseForGraphicsChanged;

        private void updateListVisual()
        {
            MainListBox.ItemsSource = null;
            MainListBox.ItemsSource = _actions;
        }
        public void setActionMetricsTypes(ActionsMetricTypes amt)
        {
            Test.setActionMetricsTypes(amt);
        }
        public void setTeam(Team team)
        {
            _teamControl = new TeamControl(team);
            this.LineRepresentationControl.setTeamControl(_teamControl);
        }
        public ActionTextRepresentationControl LineRepresentationControl
        {
            get { return Test; }
        }

        #region TimeCode module
        double _currentTimeCode;
        public void GetCurrentTimeCodeEventHandler(object sender, TimeCodeEventArgs e)
        {
            _currentTimeCode = e.currentTimeCode;
        }
        #endregion
        #region Points module
        ActionsLib.Point[] _currentPoints;
        public void PointsUpdated(object sender, PointsEventArgs e)
        {
            _currentPoints = e.currentPoints;
        }
        #endregion



        #region game order creator

        int currentArrangementNumber = -1;
        public void LoadSet(Set set)
        {
            _currentSet = set;
            _actions = set.ConvertToSequence().ToList();
            updateListVisual();
        }

        private VolleyActionSegment _currentSegment;
        private ActionsLib.Rally _currentRally;
        private ActionsLib.Set _currentSet;
        public ActionsLib.Set CurrentSet
        {
            get
            {
                return _currentSet;
            }
        }
        bool isRallyEnded = true;
        bool isSetStart = true;
        AvaibleActionTypes avaibleActionTypes = new AvaibleActionTypes();
        public void BeginNewSet(int setScore)
        {
            _currentRally = new Rally();
            _currentSegment = new VolleyActionSegment();
            _currentSet = new Set(setScore);
            avaibleActionTypes.GameBegining();
            isSetStart = false;
            LineRepresentationControl.UpdateAvaibleActionTypes(avaibleActionTypes);
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LineRepresentationControl.Ready())
            {
                MessageBox.Show("Fill Data correctly");
                return;
            }
            ActionsLib.Action act = LineRepresentationControl.ActionTextRepresentation.GenerateAction();
            if (act.AuthorType == ActionAuthorType.Player)
            {
                ((PlayerAction)act).TimeCode = _currentTimeCode;
                ((PlayerAction)act).Points = _currentPoints;
            }
            _actions.Add(act);
            actionAdded(act);
            if(currentArrangementNumber != -1)
            {
                if (isRallyEnded) { avaibleActionTypes.BetweenRallies(GetAvaibleActionTypes(act)); }
                else { avaibleActionTypes.InRally(GetAvaibleActionTypes(act)); }
            }
            LineRepresentationControl.UpdateAvaibleActionTypes(avaibleActionTypes);
            updateListVisual();
            LineRepresentationControl.setDefaultfocus();
            ActionAdded(this, new EventArgs());
        }
        private void ProcessCoachActions(ActionsLib.Action act)
        {
            if(act.AuthorType == ActionAuthorType.Coach)
            {
                CoachAction cact = (CoachAction)act;
                switch (cact.ActionType)
                {
                    case VolleyActionType.StartArrangment:
                        _teamControl.SetArrangement(cact.Players.ToArray());
                        ArrangementChanged(this, new TeamControlEventArgs(_teamControl));
                        if(currentArrangementNumber == -1)
                        {
                            avaibleActionTypes.ArrangementSet();
                        }
                        break;
                    case VolleyActionType.Change:
                        _teamControl.ChangePlayer(cact.Players[0], cact.Players[1]);
                        ArrangementChanged(this, new TeamControlEventArgs(_teamControl));
                        break;
                    case VolleyActionType.SetStartParams:
                         currentArrangementNumber =  _teamControl.getPlayerZone(((CoachAction)act).Players[0]);
                        GamePhaseForGraphicsChanged(this, new PhaseEventArgs(SegmentPhase.Recep_1, currentArrangementNumber));
                        break;
                }
            }
        }
        public List<VolleyActionType> GetAvaibleActionTypes(ActionsLib.Action lastAction)
        {
            if(lastAction.AuthorType == ActionAuthorType.Player)
            {
                return rules.getPossibleActions(lastAction.ActionType, ((PlayerAction)lastAction).GetQuality());
            }
            if(lastAction.AuthorType == ActionAuthorType.OpponentTeam)
            {
                return rules.getPossibleActionsForOpponent(lastAction.ActionType);
            }
            if(lastAction.AuthorType == ActionAuthorType.Judge || lastAction.AuthorType == ActionAuthorType.Coach)
            {
                return rules.getSetStartPlayersActions(); // or reception // 
            }
            return null;
        }
        private void actionAdded(ActionsLib.Action act)
        {
           phaseChaned(act);
           ProcessCoachActions(act);
           isRallyEnded = false;
           if(isNewSequence(_currentSegment, act))
            {
                _currentRally.Add(_currentSegment);
                _currentSegment = new VolleyActionSegment();
            }
            _currentSegment.Add(act);
            if(isRallyFinished(_currentSegment) != ActionSegmentResult.NotEnded)
            {
                _currentRally.Add(_currentSegment);
                rallyAdded(_currentRally);
                if(isRotateNeeded(_currentRally, _currentSet.CurrentPhase))
                {
                    _teamControl.Rotate();
                    ArrangementChanged(this, new TeamControlEventArgs(_teamControl));
                    currentArrangementNumber -= 1;
                    if (currentArrangementNumber < 0) currentArrangementNumber = 5;                   
                }
                graphicsPhaseChanges(_currentRally);
                _currentSegment = new VolleyActionSegment();
                _currentRally = new Rally();
            }
        }
        private void phaseChaned(ActionsLib.Action act)
        {
            if(act.ActionType == VolleyActionType.Reception && ((PlayerAction)act).GetQuality() > 1)
            {
                GamePhaseForGraphicsChanged(this, new PhaseEventArgs(SegmentPhase.Recep, currentArrangementNumber));
            }
            if(_currentSet.CurrentScore.Left == 0 && _currentSet.CurrentScore.Right == 0 && _currentRally.Length == 0 && _currentSegment.Count == 0)
            {
                if (act.ActionType == VolleyActionType.Serve) GamePhaseForGraphicsChanged(this, new PhaseEventArgs(SegmentPhase.Break, currentArrangementNumber));
                else if(act.ActionType == VolleyActionType.Reception) GamePhaseForGraphicsChanged(this,new PhaseEventArgs(SegmentPhase.Recep,currentArrangementNumber));
            }
        }
        private bool isNewSequence(VolleyActionSegment seq, ActionsLib.Action act)
        {
            if (seq.Count == 0 || act.AuthorType == ActionAuthorType.Judge) return false;
            if (seq.Count > 4) return true;
            if (seq.Last().AuthorType != act.AuthorType) return true;
            if (seq.Count > 0 && (act.ActionType == VolleyActionType.Block || act.ActionType == VolleyActionType.Defence)) return true;
            return false;
        }
        private ActionSegmentResult isRallyFinished(VolleyActionSegment seq)
        {
            ActionSegmentResult res = ActionSegmentResult.Undefined;
            ActionsLib.Action last = seq.Last();
            if (last.AuthorType == ActionAuthorType.Player) res = rules.getActionResult(last.ActionType, ((PlayerAction)last).GetQuality());
            else if (last.AuthorType == ActionAuthorType.OpponentTeam) res = rules.getActionResultForOpponent(last.ActionType);
            else if (last.AuthorType == ActionAuthorType.Judge) res = rules.getActionResultForJudge(last.ActionType);
            else res = ActionSegmentResult.Undefined; // coach
            seq.SegmentResult = res;
            return res;
        }
        private bool isRotateNeeded(Rally rally, SegmentPhase phase)
        {
            SegmentPhase newPhase = phase;
            if (rally.RallyResult == RallyResult.Won) newPhase = SegmentPhase.Break;
            if(rally.RallyResult == RallyResult.Lost) newPhase = SegmentPhase.Recep_1;
            if (rally.RallyResult == RallyResult.Disputable || rally.RallyResult == RallyResult.Undefined) newPhase = phase;
            _currentSet.updatePhase(newPhase);
            if (phase == SegmentPhase.Recep_1 && newPhase == SegmentPhase.Break) return true;
            return false;
        }
        private void rallyAdded(Rally rally)
        {
            rally.UpdateRallyResult();
            _currentSet.Add(rally);
            isRallyEnded = true;
            ScoreUpdated(this, new ScoreEventArgs(_currentSet.CurrentScore));
        }
        private void graphicsPhaseChanges(Rally rally)
        {
            if (rally.RallyResult == RallyResult.Won) GamePhaseForGraphicsChanged(this, new PhaseEventArgs(SegmentPhase.Break, currentArrangementNumber));
            else if (rally.RallyResult == RallyResult.Lost) GamePhaseForGraphicsChanged(this, new PhaseEventArgs(SegmentPhase.Recep_1, currentArrangementNumber));
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
            LineRepresentationControl.UpdateTheme();
        }
        #endregion
    }
    public class TeamControlEventArgs : EventArgs
    {
        public TeamControl _teamControl;
        public TeamControlEventArgs(TeamControl tc)  : base()
        {
            _teamControl = tc;
        }
    }
    public class ScoreEventArgs : EventArgs
    {
        public Score score;
        public ScoreEventArgs(Score score)
        {
            this.score = score;
        }
    }
    public class PhaseEventArgs : EventArgs
    {
        public SegmentPhase phase;
        public int arrangement;
        public PhaseEventArgs(SegmentPhase ph, int arr)
        {
            phase = ph;
            arrangement = arr;
        }
    }
}
