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
        AutomaticFillersRulesHolder _rulesHolder;
        string _name;
        static ActionsMetricTypes()
        {
            staticMetrics= createDefaultMetrics();
        }
        public ActionsMetricTypes(string name)
        {
            _data = new Dictionary<VolleyActionType, MetricTypeList>();
            _name = name;
            VolleyActionType[] keys = new VolleyActionType[] { VolleyActionType.Serve, VolleyActionType.Reception, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.Block, VolleyActionType.Defence, VolleyActionType.FreeBall, VolleyActionType.Transfer };
            foreach(VolleyActionType key in keys)
            {
                _data.Add(key, new MetricTypeList(key.ToString()));
            }
            _rulesHolder = new AutomaticFillersRulesHolder();
        }
        public static MetricType[] staticMetrics = new MetricType[0];
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

        static private MetricType[] createDefaultMetrics()
        {
            MetricType Quality = new MetricType("Quality", "Quality of any action", "qual", typeof(int), new Dictionary<object, string> { { 1, "=" }, { 2, "-" }, { 3, "/" }, { 4, "!" }, { 5, "+" }, { 6, "#" } }, new List<string>() { "=", "-", "\\", "!", "+", "#" });
            MetricType PlaygroundPosition = new MetricType("FieldPosition", "Position on field", "fpos", typeof(int), new Dictionary<object, string> { { 1, "1" }, { 2, "2" }, { 3, "3" }, { 4, "4" }, { 5, "5" }, { 6, "6" } }, new List<string>() { "1", "2", "3", "4", "5", "6" });
            MetricType ArrangementPosition = new MetricType("ArrangementPosition", "Position in arrangement", "apos", typeof(int), new Dictionary<object, string> { { 1, "1" }, { 2, "2" }, { 3, "3" }, { 4, "4" }, { 5, "5" }, { 6, "6" } }, new List<string>() { "1", "2", "3", "4", "5", "6" });
            return new MetricType[] { Quality, PlaygroundPosition, ArrangementPosition };
        }

        public AutomaticFillersRulesHolder FillersRules
        {
            get { return _rulesHolder; }
            set { _rulesHolder = value; }
        }

        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                Save(sw);
            }
        }
        public void Save(StreamWriter sw)
        {
            sw.WriteLine(JsonSerializer.Serialize(_name));
            foreach (VolleyActionType vt in _data.Keys)
            {
                _data[vt].Save(sw);
            }
            
            _rulesHolder.Save(sw);
        }
        public static ActionsMetricTypes Load(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                return Load(sr);
            }
        }
        public static ActionsMetricTypes Load(StreamReader sr)
        {
            string name;
            ActionsMetricTypes res;
            name = JsonSerializer.Deserialize<string>(sr.ReadLine());
            res = new ActionsMetricTypes(name);
            VolleyActionType[] keys = new VolleyActionType[] { VolleyActionType.Serve, VolleyActionType.Reception, VolleyActionType.Set, VolleyActionType.Attack, VolleyActionType.Block, VolleyActionType.Defence, VolleyActionType.FreeBall, VolleyActionType.Transfer };
            for (int i = 0; i < keys.Length; i++)
            {
                res.updateList(keys[i], MetricTypeList.Load(sr));
            }
            res._rulesHolder = AutomaticFillersRulesHolder.Load(sr);
            return res;
        }
    }
}
