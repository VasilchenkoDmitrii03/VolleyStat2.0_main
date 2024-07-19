using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ActionsLib;

namespace StatisticsCreatorModule.Logic
{
    //this class contains rules for segments, for example: which action can be after +serve
    class VolleyActionSegmetRules
    {
        Dictionary<VolleyActionType, List<VolleyActionType>[]> _sequenceLogic;
        Dictionary<VolleyActionType, ActionSegmentResult[]> _winLossLogic;

        public VolleyActionSegmetRules(bool defaultSettings = true)
        {
            _sequenceLogic = new Dictionary<VolleyActionType, List<VolleyActionType>[]>();
            _winLossLogic = new Dictionary<VolleyActionType, ActionSegmentResult[]>();
            if (defaultSettings) setDefaultRules();
        }

        //sequence Logic
        public void setRuleForActionAndQuality(VolleyActionType actType, int quality, params VolleyActionType[] types)
        {
            if (_sequenceLogic.ContainsKey(actType))
            {
                _sequenceLogic[actType][quality - 1] = new List<VolleyActionType>(types);
            }
            else
            {
                _sequenceLogic.Add(actType, new List<VolleyActionType>[6]);
            }
        }
        public void setRuleForActionAndQuality(VolleyActionType actType, int[] qualities, params VolleyActionType[] types)
        {
            foreach (int qual in qualities)
            {
                setRuleForActionAndQuality(actType, qual, types);
            }
        }
        public void setDefaultRules()
        {
            //serve
            setRuleForActionAndQuality(VolleyActionType.Serve, 6, VolleyActionType.Serve);
            setRuleForActionAndQuality(VolleyActionType.Serve, new int[] { 5, 4, 3, 2 }, VolleyActionType.Block, VolleyActionType.Defence, VolleyActionType.FreeBall, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.OpponentError, VolleyActionType.OpponentPoint);
            setRuleForActionAndQuality(VolleyActionType.Serve, 1, VolleyActionType.Reception);
            //reception
            setRuleForActionAndQuality(VolleyActionType.Reception, new int[] {6, 5, 4, 3, 2 },VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.Block, VolleyActionType.Defence, VolleyActionType.FreeBall, VolleyActionType.OpponentError, VolleyActionType.OpponentPoint);
            setRuleForActionAndQuality(VolleyActionType.Reception, 1, VolleyActionType.Reception);
            //set
            setRuleForActionAndQuality(VolleyActionType.Reception, new int[] { 6, 5, 4, 3, 2 }, VolleyActionType.Attack, VolleyActionType.Transfer, VolleyActionType.Block, VolleyActionType.Defence, VolleyActionType.FreeBall, VolleyActionType.OpponentError, VolleyActionType.OpponentPoint);
            setRuleForActionAndQuality(VolleyActionType.Set, 1, VolleyActionType.Reception);
            //attack
            setRuleForActionAndQuality(VolleyActionType.Attack, 6, VolleyActionType.Serve);
            setRuleForActionAndQuality(VolleyActionType.Attack, new int[] { 5, 4, 3 }, VolleyActionType.Block, VolleyActionType.Defence, VolleyActionType.FreeBall, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.OpponentError, VolleyActionType.OpponentPoint);
            setRuleForActionAndQuality(VolleyActionType.Set, new int[] { 2, 1 }, VolleyActionType.Reception);
            //block
            setRuleForActionAndQuality(VolleyActionType.Block, 6, VolleyActionType.Serve);
            setRuleForActionAndQuality(VolleyActionType.Block, new int[] { 5, 4, 3, 2 }, VolleyActionType.Defence, VolleyActionType.FreeBall, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.OpponentError, VolleyActionType.OpponentPoint);
            setRuleForActionAndQuality(VolleyActionType.Block, 1, VolleyActionType.Reception);
            //defence
            setRuleForActionAndQuality(VolleyActionType.Defence, new int[] {6, 5, 4, 3, 2 }, VolleyActionType.Block, VolleyActionType.Defence, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.Transfer, VolleyActionType.OpponentError, VolleyActionType.OpponentPoint);
            setRuleForActionAndQuality(VolleyActionType.Defence, 1, VolleyActionType.Reception);
            //freeBall
            setRuleForActionAndQuality(VolleyActionType.FreeBall, new int[] { 6, 5, 4, 3, 2 }, VolleyActionType.Block, VolleyActionType.Defence, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.Transfer, VolleyActionType.OpponentError, VolleyActionType.OpponentPoint);
            setRuleForActionAndQuality(VolleyActionType.FreeBall, 1, VolleyActionType.Reception);
            //Transfer
            setRuleForActionAndQuality(VolleyActionType.Transfer, new int[] { 6, 5, 4, 3, 2 }, VolleyActionType.Block, VolleyActionType.Defence, VolleyActionType.FreeBall, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.OpponentError, VolleyActionType.OpponentPoint);
            setRuleForActionAndQuality(VolleyActionType.Transfer, 1, VolleyActionType.Reception);


            //opponent
            setRuleForActionAndQuality(VolleyActionType.OpponentPoint, new int[] { 1, 2, 3, 4, 5, 6 }, VolleyActionType.Reception);
            setRuleForActionAndQuality(VolleyActionType.OpponentError, new int[] { 1, 2, 3, 4, 5, 6 }, VolleyActionType.Serve);

            //winlossRules
            setDefaultWinLoseLogic();
        }
        
        //winlose Logic
        public void setWinLoseRule(VolleyActionType actType, ActionSegmentResult[] results)
        {
            if (_winLossLogic.ContainsKey(actType))
            {
                _winLossLogic[actType] = results;
            }
            else
            {
                _winLossLogic.Add(actType, results);
            }
        }
        public void setDefaultWinLoseLogic()
        {
            setWinLoseRule(VolleyActionType.Serve, new ActionSegmentResult[] { ActionSegmentResult.Lost, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.Won });
            setWinLoseRule(VolleyActionType.Reception, new ActionSegmentResult[] { ActionSegmentResult.Lost, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded });
            setWinLoseRule(VolleyActionType.Set, new ActionSegmentResult[] { ActionSegmentResult.Lost, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded });
            setWinLoseRule(VolleyActionType.Attack, new ActionSegmentResult[] { ActionSegmentResult.Lost, ActionSegmentResult.Lost, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.Won });
            setWinLoseRule(VolleyActionType.Block, new ActionSegmentResult[] { ActionSegmentResult.Lost, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.Won });
            setWinLoseRule(VolleyActionType.Defence, new ActionSegmentResult[] { ActionSegmentResult.Lost, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded });
            setWinLoseRule(VolleyActionType.FreeBall, new ActionSegmentResult[] { ActionSegmentResult.Lost, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded });
            setWinLoseRule(VolleyActionType.Transfer, new ActionSegmentResult[] { ActionSegmentResult.Lost, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded, ActionSegmentResult.NotEnded });
            setWinLoseRule(VolleyActionType.OpponentError, new ActionSegmentResult[] { ActionSegmentResult.Won, ActionSegmentResult.Won, ActionSegmentResult.Won, ActionSegmentResult.Won, ActionSegmentResult.Won, ActionSegmentResult.Won });
            setWinLoseRule(VolleyActionType.OpponentPoint, new ActionSegmentResult[] { ActionSegmentResult.Lost, ActionSegmentResult.Lost, ActionSegmentResult.Lost, ActionSegmentResult.Lost, ActionSegmentResult.Lost, ActionSegmentResult.Lost });
        }

        //interface
        public List<VolleyActionType> getPossibleActions(VolleyActionType type, int quality)
        {
            return _sequenceLogic[type][quality];
        }
        public ActionSegmentResult getActionResult(VolleyActionType type, int quality)
        {
            return _winLossLogic[type][quality];
        }


    }
    //this class contains logic about when you can make changes take timeouts and etc
    class SetLogic 
    {

    }
}
