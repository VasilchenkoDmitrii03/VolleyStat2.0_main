using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml.Linq;

namespace ActionsLib
{
    public class MetricTypeList :IEnumerable<MetricType>
    {
        string _name;
        List<MetricType> _metricTypes;
        public MetricTypeList(string name)
        {
            _name = name;
            _metricTypes = new List<MetricType>();
        }
        
        public string Name
        {
            get { return _name; }
        }
        public List<MetricType> MetricTypes
        {
            get { return _metricTypes; }
            set { _metricTypes = value; }
        }

        public IEnumerator<MetricType> GetEnumerator()
        {
            return _metricTypes.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        { return (IEnumerator)GetEnumerator(); }

        public MetricType this[int index]
        {
            get { return _metricTypes[index]; }
            set { _metricTypes[index] = value; }
        }

        public void Add(MetricType metricType)
        {
            if(!_metricTypes.Contains(metricType)) { _metricTypes.Add(metricType); }
        }
        public void Add(MetricType[] lst)
        {
            foreach(MetricType metricType in lst)
            {
                Add(metricType);
            }
        }
        public void Remove(MetricType metricType)
        {
            if(_metricTypes.Contains(metricType))
            {
                _metricTypes.Remove(metricType);
            }
            else
            {
                throw new Exception($"Metric types list {_name} does not contains metric type {metricType.Name}");
            }
        }
        public void Clear()
        {
            _metricTypes.Clear();
        }
        public int IndexOf(MetricType metricType)
        {
            for(int i= 0;i < _metricTypes.Count; ++i)
            {
                if(metricType == _metricTypes[i]) { return i; }
            }
            return -1;
        }
        public MetricTypeList Copy()
        {
            MetricTypeList res = new MetricTypeList(this._name);
            res.Add(this._metricTypes.ToArray());
            return res;
        }

        public void Save(string filename)
        {
            using(StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine(JsonSerializer.Serialize(_name));
                sw.WriteLine(JsonSerializer.Serialize(_metricTypes.Count));
                foreach(MetricType metricType in _metricTypes)
                {
                    metricType.Save(sw);
                }
            }
        }
        public void Save(StreamWriter sw)
        {
            sw.WriteLine(JsonSerializer.Serialize(_name));
            sw.WriteLine(JsonSerializer.Serialize(_metricTypes.Count));
            foreach (MetricType metricType in _metricTypes)
            {
                metricType.Save(sw);
            }
        }
        public static MetricTypeList Load(string filename)
        {
            int count;
            string name;
            MetricTypeList res;
            using(StreamReader sr = new StreamReader(filename))
            {
                name = JsonSerializer.Deserialize<string>(sr.ReadLine());
                res = new MetricTypeList(name);
                count = JsonSerializer.Deserialize<int>(sr.ReadLine());
                for(int i = 0;i < count; i++)
                {
                    res.Add(MetricType.Load(sr));
                }
            }
            return res;
        }
        public static MetricTypeList Load(StreamReader sr)
        {
            int count;
            string name;
            MetricTypeList res;
            name = JsonSerializer.Deserialize<string>(sr.ReadLine());
            res = new MetricTypeList(name);
            count = JsonSerializer.Deserialize<int>(sr.ReadLine());
            for (int i = 0; i < count; i++)
            {
                res.Add(MetricType.Load(sr));
            }
            return res;
        }
        
    }
}
