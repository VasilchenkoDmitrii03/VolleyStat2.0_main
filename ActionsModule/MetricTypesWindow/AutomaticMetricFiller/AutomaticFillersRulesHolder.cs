using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionsLib;
using ActionsLib.TextRepresentation;

namespace MetricTypesWindow.AutomaticMetricFiller
{
    class AutomaticFillersRulesHolder
    {
        List<InActionAutomaticFiller> _inActionFillers;
        List<SequenceAutomaticFiller> _sequenceFillers;

        public AutomaticFillersRulesHolder()
        {
            _inActionFillers = new List<InActionAutomaticFiller>();
            _sequenceFillers = new List<SequenceAutomaticFiller>();
        }

        public void Add(InActionAutomaticFiller tmp)
        {
            _inActionFillers.Add(tmp);
        }
        public void Add(SequenceAutomaticFiller tmp)
        {
            _sequenceFillers.Add(tmp);
        }

        public List<InActionAutomaticFiller> InActionsFillers
        {
            get { return _inActionFillers; }
        }
        public List<SequenceAutomaticFiller> SequenceFillers
        {
            get { return _sequenceFillers; }
        }
    }

    class InActionAutomaticFiller
    {
        VolleyActionType actionType;
        MetricType _leftMetricType;
        MetricType _rightMetricType;
        string[] _leftValues;
        string _rightValue;

        public InActionAutomaticFiller()
        {
            _leftValues = new string[0];
        }
        public InActionAutomaticFiller(VolleyActionType vat, MetricType metricType, MetricType RmetricType, string[] leftValues, string rightValue)
        {
            actionType = vat;
            _leftMetricType = metricType;
            _rightMetricType = RmetricType;
            _leftValues = leftValues;
            _rightValue = rightValue;
        }
        public bool Use(PlayerActionTextRepresentation plAct) // returns true if set right name
        {
            if (plAct.ActionType != actionType) return false;
            Metric RightMetric = plAct.GetMetric(_rightMetricType);
            Metric LeftMetric = plAct.GetMetric(_leftMetricType);
            if (RightMetric != null) return false;
            else {
                if (LeftMetric == null) return false;

                if (_leftValues.Contains(_leftMetricType.AcceptableValuesNames[LeftMetric.Value]))
                {
                    plAct.SetMetricByObject(_rightMetricType, _rightMetricType.getObjectByLargeName(_rightValue));
                    return true;
                }

            }
            return false;
        }

        public override string ToString()
        {
            string tmp = _leftValues[0];
            for (int i = 1; i < _leftValues.Length; i++) tmp += $", {_leftValues[i]}";
            string res = $"{actionType}: {_leftMetricType.ToString()} ({tmp}) ==> {_rightMetricType.ToString()}({_rightValue})";
            return res;
        }
    }

    class SequenceAutomaticFiller
    {
        VolleyActionType _leftActionType;
        VolleyActionType _rightActionType;
        MetricType _leftMetricType;
        MetricType _rightMetricType;
        string[] _leftValues;
        string _rightValue;
        bool isCopying;

        public SequenceAutomaticFiller()
        {

        }
        public SequenceAutomaticFiller(VolleyActionType leftActionType, VolleyActionType rightActionType, MetricType leftMetricType, MetricType rightMetricType, string[] leftValues, string rightValue)
        {
            _leftActionType = leftActionType;
            _rightActionType = rightActionType;
            _leftMetricType = leftMetricType;
            _rightMetricType = rightMetricType;
            _leftValues = leftValues;
            _rightValue = rightValue;
            this.isCopying = false;
        }
        public SequenceAutomaticFiller(VolleyActionType leftActionType, VolleyActionType rightActionType, MetricType leftMetricType, MetricType rightMetricType, bool isCopying = true)
        {
            _leftActionType = leftActionType;
            _rightActionType = rightActionType;
            _leftMetricType = leftMetricType;
            _rightMetricType = rightMetricType;
            this.isCopying = true;
        }


        public bool Use(PlayerActionTextRepresentation current, VolleyActionSegment segment)
        {
            if(current.ActionType == _rightActionType && segment.ContainsActionType(_leftActionType))
            {
                Metric metrics = segment.getByActionType(_leftActionType)[_leftMetricType];
                if (isCopying)
                {
                    current.SetMetricByObject(_rightMetricType, metrics.Value);
                    return true;
                }
                else if (_leftValues.Contains(_leftMetricType.AcceptableValuesNames[metrics.Value])) 
                {
                    current.SetMetricByObject(_rightMetricType, _rightValue); return true;
                }
            }
            return false;
        }
        public override string ToString()
        {
            if(isCopying)return $"{_leftActionType}: {_leftMetricType.ToString()} ==> {_rightActionType}: {_rightMetricType.ToString()}";
            string tmp = _leftValues[0];
            for (int i = 1; i < _leftValues.Length; i++) tmp += $", {_leftValues[i]}";
            string res = $"{_leftActionType}: {_leftMetricType.ToString()} ({tmp}) ==> {_rightActionType}: {_rightMetricType.ToString()}({_rightValue})";
            return res;
        }
    }
}
