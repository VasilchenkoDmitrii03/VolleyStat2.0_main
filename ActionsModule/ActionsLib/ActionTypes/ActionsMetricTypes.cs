using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ActionsLib.ActionTypes
{
    public class ActionsMetricTypes
    {
        Dictionary<VolleyActionType, MetricTypeList> _data;
        string _name;
        public ActionsMetricTypes(string name)
        {
            _data = new Dictionary<VolleyActionType, MetricTypeList>();
            _name = name;
            VolleyActionType[] keys = new VolleyActionType[] { VolleyActionType.Serve, VolleyActionType.Reception, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.Block, VolleyActionType.Defence };
            foreach(VolleyActionType key in keys)
            {
                _data.Add(key, new MetricTypeList(key.ToString()));
            }
        }
        public void updateList(VolleyActionType type, MetricTypeList mtypelist)
        {
            _data[type] = new MetricTypeList(type.ToString());
            foreach(MetricType mt in  mtypelist)
            {
                _data[type].Add(mt);
            }
        }
        public VolleyActionType[] Keys
        {
            get { return _data.Keys.ToArray(); }
        }
        public MetricTypeList this[VolleyActionType type]
        {
            get
            {
                return _data[type];
            }
        }

        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine(JsonSerializer.Serialize(_name));
                foreach(VolleyActionType vt in _data.Keys)
                {
                    _data[vt].Save(sw);
                }
            }
        }
        public static ActionsMetricTypes Load(string filename)
        {
            string name;
            ActionsMetricTypes res;
            using (StreamReader sr = new StreamReader(filename))
            {
                name = JsonSerializer.Deserialize<string>(sr.ReadLine());
                res = new ActionsMetricTypes(name);
                VolleyActionType[] keys = new VolleyActionType[] { VolleyActionType.Serve, VolleyActionType.Reception, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.Block, VolleyActionType.Defence };
                for (int i = 0; i < keys.Length; i++)
                {
                    res.updateList(keys[i], MetricTypeList.Load(sr));
                }
            }
            return res;
        }
    }
}
