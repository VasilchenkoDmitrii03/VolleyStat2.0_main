using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ActionsLib;
using ActionsLib.TextRepresentation;
using System.Text.Json;

namespace ActionsLib
{
    public class AutomaticFillersRulesHolder
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

        public void Save(StreamWriter sw)
        {
            sw.WriteLine(JsonSerializer.Serialize(_inActionFillers.Count));
            foreach(InActionAutomaticFiller tmp in _inActionFillers)
            {
                tmp.Save(sw);
            }
            sw.WriteLine(JsonSerializer.Serialize(_sequenceFillers.Count));
            foreach(SequenceAutomaticFiller tmp in _sequenceFillers)
            {
                tmp.Save(sw);
            }
        }
        public static AutomaticFillersRulesHolder Load(StreamReader sr)
        {
            AutomaticFillersRulesHolder res = new AutomaticFillersRulesHolder();
            int len = JsonSerializer.Deserialize<int>(sr.ReadLine());
            for(int i = 0;i < len; i++)
            {
                res._inActionFillers.Add(InActionAutomaticFiller.Load(sr));
            }
            len = JsonSerializer.Deserialize<int>(sr.ReadLine());
            for (int i = 0; i < len; i++)
            {
                res._sequenceFillers.Add(SequenceAutomaticFiller.Load(sr));
            }
            return res;
        }
    }

    public class InActionAutomaticFiller
    {
        VolleyActionType _actionType;
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
            _actionType = vat;
            _leftMetricType = metricType;
            _rightMetricType = RmetricType;
            _leftValues = leftValues;
            _rightValue = rightValue;
        }
        public bool Use(PlayerActionTextRepresentation plAct) // returns true if set right name
        {
            if (plAct.ActionType != _actionType) return false;
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
            string res = $"{_actionType}: {_leftMetricType.ToString()} ({tmp}) ==> {_rightMetricType.ToString()}({_rightValue})";
            return res;
        }
        public void Save(StreamWriter sw)
        {
            sw.WriteLine(JsonSerializer.Serialize(_actionType));
            _leftMetricType.Save(sw);
            _rightMetricType.Save(sw);
            sw.WriteLine(JsonSerializer.Serialize(_leftValues));
            sw.WriteLine(JsonSerializer.Serialize(_rightValue));
        }
        public static InActionAutomaticFiller Load(StreamReader sr)
        {
            InActionAutomaticFiller res = new InActionAutomaticFiller();
            res._actionType = JsonSerializer.Deserialize<VolleyActionType>(sr.ReadLine());
            res._leftMetricType = MetricType.Load(sr.ReadLine());
            res._rightMetricType = MetricType.Load(sr.ReadLine());
            res._leftValues = JsonSerializer.Deserialize<string[]>(sr.ReadLine());
            res._rightValue = JsonSerializer.Deserialize<string>(sr.ReadLine());
            return res;
        }

    }

    public class SequenceAutomaticFiller
    {
        VolleyActionType _leftActionType;
        VolleyActionType _rightActionType;
        MetricType _leftMetricType;
        MetricType _rightMetricType;
        string[] _leftValues;
        string _rightValue;
        bool _isCopying;

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
            this._isCopying = false;
        }
        public SequenceAutomaticFiller(VolleyActionType leftActionType, VolleyActionType rightActionType, MetricType leftMetricType, MetricType rightMetricType, bool isCopying = true)
        {
            _leftActionType = leftActionType;
            _rightActionType = rightActionType;
            _leftMetricType = leftMetricType;
            _rightMetricType = rightMetricType;
            this._isCopying = true;
        }


        public bool Use(PlayerActionTextRepresentation current, VolleyActionSegment segment)
        {
            if(current.ActionType == _rightActionType && segment.ContainsActionType(_leftActionType))
            {
                Metric metrics = segment.getByActionType(_leftActionType)[_leftMetricType];
                if (_isCopying)
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
            if(_isCopying)return $"{_leftActionType}: {_leftMetricType.ToString()} ==> {_rightActionType}: {_rightMetricType.ToString()}";
            string tmp = _leftValues[0];
            for (int i = 1; i < _leftValues.Length; i++) tmp += $", {_leftValues[i]}";
            string res = $"{_leftActionType}: {_leftMetricType.ToString()} ({tmp}) ==> {_rightActionType}: {_rightMetricType.ToString()}({_rightValue})";
            return res;
        }

        public void Save(StreamWriter sw)
        {
            sw.WriteLine(JsonSerializer.Serialize(_leftActionType));
            sw.WriteLine(JsonSerializer.Serialize(_rightActionType));
            _leftMetricType.Save(sw);
            _rightMetricType.Save(sw);
            sw.WriteLine(JsonSerializer.Serialize(_leftValues));
            sw.WriteLine(JsonSerializer.Serialize(_rightValue));
            sw.WriteLine(JsonSerializer.Serialize(_isCopying));
        }
        public static SequenceAutomaticFiller Load(StreamReader sr)
        {
            SequenceAutomaticFiller res = new SequenceAutomaticFiller();
            res._leftActionType = JsonSerializer.Deserialize<VolleyActionType>(sr.ReadLine());
            res._rightActionType = JsonSerializer.Deserialize<VolleyActionType>(sr.ReadLine());
            res._leftMetricType = MetricType.Load(sr.ReadLine());
            res._rightMetricType = MetricType.Load(sr.ReadLine());
            res._leftValues = JsonSerializer.Deserialize<string[]>(sr.ReadLine());
            res._rightValue = JsonSerializer.Deserialize<string>(sr.ReadLine());
            res._isCopying = JsonSerializer.Deserialize<bool>(sr.ReadLine());
            return res;
        }
    }
}
