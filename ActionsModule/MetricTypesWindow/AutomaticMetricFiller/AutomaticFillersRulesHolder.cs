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
    }
}
