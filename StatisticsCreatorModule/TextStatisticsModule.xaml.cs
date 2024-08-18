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
            BeginProcess(5);
            AddButton.TabIndex = 40;
            LineRepresentationControl.DataFilled += AddButton_Click;
        }

        public event ArrangmentChanged ArrangementChanged;
        public event ActionAdded ActionAdded;
        public event ScoreUpdated ScoreUpdated;

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
        private VolleyActionSegment _currentSegment;
        private ActionsLib.Rally _currentRally;
        private ActionsLib.Set _currentSet;
        private void BeginProcess(int setScore)
        {
            _currentRally = new Rally();
            _currentSegment = new VolleyActionSegment();
            _currentSet = new Set(setScore);
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
            LineRepresentationControl.UpdateAvaibleActionTypes(GetAvaibleActionTypes(act));
            ProcessCoachActions(act);
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
                        break;
                    case VolleyActionType.Change:
                        _teamControl.ChangePlayer(cact.Players[0], cact.Players[1]);
                        ArrangementChanged(this, new TeamControlEventArgs(_teamControl));
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
                }
                _currentSegment = new VolleyActionSegment();
                _currentRally = new Rally();
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
            ScoreUpdated(this, new ScoreEventArgs(_currentSet.CurrentScore));
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
}
