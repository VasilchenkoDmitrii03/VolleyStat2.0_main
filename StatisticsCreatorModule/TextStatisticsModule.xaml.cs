using ActionsLib;
using ActionsLib.ActionTypes;
using DocumentFormat.OpenXml.Office2013.Excel;
using StatisticsCreatorModule.Arrangment;
using StatisticsCreatorModule.Logic;
using StatisticsCreatorModule.SettingsWindow;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StatisticsCreatorModule
{
    /// <summary>
    /// Логика взаимодействия для TextStatisticsModule.xaml
    /// </summary>
    /// 
    public delegate void ArrangmentChanged(object sender, TeamControlEventArgs e);
    public delegate void ActionAdded(object sender, ActionSequenceEventArgs e);
    public delegate void ScoreUpdated(object sender, ScoreEventArgs e);
    public delegate void GamePhaseForGraphicsChanged(object sender, PhaseEventArgs e);
    public delegate void SetFinished(object sender, SetEventArgs e);

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
        public void Clear()
        {
            this._actions.Clear();
            this.MainListBox.ItemsSource = _actions;
            this.LineRepresentationControl.clear();
            this.LineRepresentationControl.setDefaultfocus();
        }
        public event ArrangmentChanged ArrangementChanged;
        public event ActionAdded ActionAdded;
        public event ScoreUpdated ScoreUpdated;
        public event GamePhaseForGraphicsChanged GamePhaseForGraphicsChanged;
        public event SetFinished SetFinished;
        public TeamControl TeamControl
        {
            get { return _teamControl; }
        }

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

        PositionSettingsMode PositionSettingsMode;
        public void SetPositionSettingsMode(PositionSettingsArgs stm)
        {
            PositionSettingsMode = stm.PositionSettingsMode;
            this.CurrentSet.CurrentPhase = stm.startingPhase;
            currentArrangementNumber = PositionSettingsMode.CurrentArrangementIndex;
            LineRepresentationControl.setPositionHolder(stm.PositionSettingsMode.PlayerPositionDataContainer);
            GamePhaseForGraphicsChanged(this, new PhaseEventArgs(SegmentPhase.Recep_1, currentArrangementNumber));
        }
        private bool isLiberoChangingSituation(int currentArrangement)
        {
            int changingPos = PositionSettingsMode.LiberoArrangementDataContainer.ArrangementNumberForChange;
            if (changingPos == currentArrangement || (changingPos + 3)%6 == currentArrangement)
            {
                return CurrentSet.CurrentPhase == SegmentPhase.Break;
            }
            return false;
        }
        private Player CurrentBackRowLibero(int currentArrangement)
        {
            //new version
            int difference = 7 - PositionSettingsMode.LiberoArrangementDataContainer.ArrangementNumberForChange;
            int first = (currentArrangement + difference) % 6;
            int second =(currentArrangement + difference - 3)%6 ;
            int index = -1;
            if (first == 6 || first == 5 || first == 1) index = first;
            if(second == 6 || second == 5 || second == 1) index = second; ;
            if (first == 0 || second == 0) index = 6;
            return _teamControl.CurrentArrangement[index - 1];

        }

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
        public void BeginNewSet(int setScore, VolleyActionSequence sequence)
        {
            BeginNewSet(setScore);
            foreach(ActionsLib.Action act in sequence)
            {
                buttonClickActionAdded(act);
            }
            LineRepresentationControl.updateCurrentSegment(_currentSegment);
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
            buttonClickActionAdded(act);
        }
        private void buttonClickActionAdded(ActionsLib.Action act)
        {
            
            _actions.Add(act);
            if (actionAdded(act)) return;
            if (currentArrangementNumber != -1)
            {
                if (isRallyEnded) { avaibleActionTypes.BetweenRallies(GetAvaibleActionTypes(act)); }
                else { avaibleActionTypes.InRally(GetAvaibleActionTypes(act)); }
            }
            if (!isSetStart) return;
            LineRepresentationControl.UpdateAvaibleActionTypes(avaibleActionTypes);
            updateListVisual();
            LineRepresentationControl.setDefaultfocus();
            ActionAdded(this, new ActionSequenceEventArgs(_actions));
            LineRepresentationControl.updateCurrentSegment(_currentSegment);
        }
        private void ProcessCoachActions(ActionsLib.Action act)
        {
            if(act.AuthorType == ActionAuthorType.Coach)
            {
                CoachAction cact = (CoachAction)act;
                switch (cact.ActionType)
                {
                    case VolleyActionType.StartArrangment:
                        isSetStart = true;
                        _teamControl.SetArrangement(cact.Players.ToArray());
                        ArrangementChanged(this, new TeamControlEventArgs(_teamControl));
                        if(currentArrangementNumber == -1)currentArrangementNumber = 1; //Default arrangement selected;
                        
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
        private bool ProcessJudgeActions(ActionsLib.Action act)
        {
            switch (act.ActionType)
            {
                case VolleyActionType.DisputableBall:
                    RallyResult res = this.CurrentSet.Rallies.Last().RallyResult;
                    if(res == RallyResult.Lost) this.CurrentSet.CurrentScore.Right -= 1;
                    if (res == RallyResult.Won) this.CurrentSet.CurrentScore.Left -= 1;
                    if (isRotateBackNeeded(_currentSet.Rallies))
                    {
                        _teamControl.RotateBack();
                        currentArrangementNumber = (currentArrangementNumber + 1) % 6;
                       
                    }
                    ActionsLib.Action firstAction = this.CurrentSet.Rallies.Last().ConvertToActionSequence()[0];
                    _currentSet.CurrentPhase = SegmentPhase.Recep_1;
                    if (firstAction.ActionType == VolleyActionType.Serve) _currentSet.CurrentPhase = SegmentPhase.Break;
                    ArrangementChanged(this, new TeamControlEventArgs(_teamControl));
                    GamePhaseForGraphicsChanged(this, new PhaseEventArgs(_currentSet.CurrentPhase, currentArrangementNumber));
                    return true;
                case VolleyActionType.JudgeMistakeLost:
                    this.CurrentSet.CurrentScore.Left -= 1;
                    this.CurrentSet.CurrentScore.Right += 1;
                    return true;
                case VolleyActionType.JudgeMistakeWon:
                    this.CurrentSet.CurrentScore.Left += 1;
                    this.CurrentSet.CurrentScore.Right -= 1;
                    return true;
            }
            return false;
           
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
        private bool actionAdded(ActionsLib.Action act)
        {
           phaseChanged(act);
           ProcessCoachActions(act);
            ProcessJudgeActions(act);
            if (act.ActionType == VolleyActionType.Reception) LineRepresentationControl.setCurrentArrangementAndPhase(SegmentPhase.Recep, currentArrangementNumber);
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
                if(rallyAdded(_currentRally)) return true;
                if(isRotateNeeded(_currentRally, _currentSet.CurrentPhase))
                {
                    _teamControl.Rotate();
                    currentArrangementNumber -= 1;
                    if (currentArrangementNumber < 0) currentArrangementNumber = 5;
                    if (isLiberoChangingSituation(currentArrangementNumber))
                    {
                        _teamControl.AutomaticFirstLineLiberoChange();
                    }
                    ArrangementChanged(this, new TeamControlEventArgs(_teamControl));
                                    
                }

                 if(PositionSettingsMode != null && !isLiberoChangingSituation(currentArrangementNumber))
                 {

                         Player currentPlayer = CurrentBackRowLibero(currentArrangementNumber);
                         Player newPlayer = PositionSettingsMode.LiberoArrangementDataContainer.UpdatePlayer(currentArrangementNumber, _currentSet.CurrentPhase);
                         if (_teamControl.isLiberoOnFirstLine() && (currentArrangementNumber == PositionSettingsMode.CurrentArrangementIndex || (currentArrangementNumber + 3) % 6 == PositionSettingsMode.CurrentArrangementIndex) && CurrentSet.CurrentPhase == SegmentPhase.Break)//случай когда выполняется подача центральным блокирующим
                         {
                             Player notLibero = _teamControl.GetNonLiberoFromFastChangePlayers();
                             if (notLibero != null)
                             {
                                 _teamControl.ChangeFastPlayer(currentPlayer, notLibero);
                             }
                         }
                         else
                         {
                             if (newPlayer == null)
                             {
                                 newPlayer = _teamControl.GetNonLiberoFromFastChangePlayers();
                                 if (newPlayer == null) newPlayer = currentPlayer;
                             }
                             if (currentPlayer.Number != newPlayer.Number)
                             {
                                 _teamControl.ChangeFastPlayer(currentPlayer, newPlayer);
                             }
                         }



                     ArrangementChanged(this, new TeamControlEventArgs(_teamControl));
                     LineRepresentationControl.setCurrentArrangementAndPhase(_currentSet.CurrentPhase, currentArrangementNumber);
                     GamePhaseForGraphicsChanged(this, new PhaseEventArgs(_currentSet.CurrentPhase, currentArrangementNumber));
                 }
                
                ArrangementChanged(this, new TeamControlEventArgs(_teamControl));
                LineRepresentationControl.setCurrentArrangementAndPhase(_currentSet.CurrentPhase, currentArrangementNumber);
                GamePhaseForGraphicsChanged(this, new PhaseEventArgs(_currentSet.CurrentPhase, currentArrangementNumber));

                graphicsPhaseChanges(_currentRally);
                _currentSegment = new VolleyActionSegment();
                _currentRally = new Rally();
            }
            
            return false;
        }
        private void phaseChanged(ActionsLib.Action act)
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
            if (seq.ContainsActionType(act.ActionType) && act.ActionType != VolleyActionType.Defence) return true;
            if (act.ActionType == VolleyActionType.Block && ((PlayerAction)act).GetQuality() == 5) return true;
            if(seq.Last().ActionType == VolleyActionType.Attack) return true;
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
        private bool rallyAdded(Rally rally)
        {
            rally.UpdateRallyResult();
            _currentSet.Add(rally);
            isRallyEnded = true;
            ScoreUpdated(this, new ScoreEventArgs(_currentSet.CurrentScore));
            if (_currentSet.isFinished() == SetResult.Lost || _currentSet.isFinished() == SetResult.Won)
            {
                SetFinished(this, new SetEventArgs(_currentSet));
                return true;
            }
            return false;
        }
        private void graphicsPhaseChanges(Rally rally)
        {
            if (rally.RallyResult == RallyResult.Won) GamePhaseForGraphicsChanged(this, new PhaseEventArgs(SegmentPhase.Break, currentArrangementNumber));
            else if (rally.RallyResult == RallyResult.Lost) GamePhaseForGraphicsChanged(this, new PhaseEventArgs(SegmentPhase.Recep_1, currentArrangementNumber));
        }

        private bool isRotateBackNeeded(RallySequence rallySequence)
        {
            if (rallySequence.Count == 0) return false;
            Rally last = rallySequence.Last();
            if (last.RallyResult == RallyResult.Lost) return false;
            if (rallySequence.Count == 2) return true;
            int index = rallySequence.Count - 2;
            if (rallySequence[index].RallyResult == RallyResult.Lost) return true;
            return false;
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
    public class SetEventArgs : EventArgs
    {
        public Set Set { get; set; }
        public SetEventArgs(Set set)
        {
            Set = set;
        }
    }
    public class ActionSequenceEventArgs : EventArgs
    {
        public VolleyActionSequence seq;
        public ActionSequenceEventArgs(List<ActionsLib.Action> actions)
        {
            this.seq = new VolleyActionSequence();
            foreach (ActionsLib.Action action in actions)
            {
                if (action.AuthorType == ActionAuthorType.Player) seq.Add(action);
            }
        }
    }
}
