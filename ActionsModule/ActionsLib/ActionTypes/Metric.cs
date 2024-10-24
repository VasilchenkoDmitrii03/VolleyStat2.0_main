using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ActionsLib
{
    public class Metric
    {
        MetricType _metricType;
        object _value;
        public Metric()
        {
            _metricType = new MetricType();
            _value = -1;
        }
        public Metric(MetricType mType, object value)
        {
            //if (mType == null || value == null) throw new ArgumentNullException();
            if (value.GetType() != mType.ValueType) throw new Exception($"Wrong value type {value.GetType()} in metric type {mType.Name}");
            _metricType = mType;
            if (!mType.isAcceptableValue(value)) throw new Exception($"Non acceptable value {value.ToString()} in metric type {_metricType.Name}");
            _value = value;
        }

        public MetricType MetricType
        {
            get { return _metricType; }
        }
        public object Value
        {
            get { return _value; }
        }

        public string getShortString()
        {
            return _metricType.getShortString(_value);
        }

        public static bool operator ==(Metric lhs, Metric rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                return true;

            // Если один из объектов null, они не равны
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                return false;
            if (lhs._metricType != rhs._metricType) return false;
            return lhs._value == rhs._value;
        }
        
        public static bool operator !=(Metric lhs, Metric rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                return false;

            // Если один из объектов null, они не равны
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                return true;
            if (lhs._metricType != rhs._metricType) return true;
            return lhs._value != rhs._value;
        }
    }
}
