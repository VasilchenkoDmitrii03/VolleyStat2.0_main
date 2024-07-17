
using ActionsLib;
using ActionsLib.ActionTypes;
using System.Xml.Serialization;
using ActionsLib.TextRepresentation;
class Program
{
    List<MetricType> _metricTypes = new List<MetricType>();

    public static void Main(string[] args)
    {
        ActionSequence tmp = new ActionsLib.ActionSequence();
        tmp.Add(new ActionsLib.Action("0"));
        tmp.Add(new ActionsLib.Action("1"));


        ActionsMetricTypes actionMetricTYpes = ActionsMetricTypes.Load(@"C:\Dmitrii\Programming\VolleyStat2.0\BasicActionsMetrics");
        foreach(var mt in actionMetricTYpes[VolleyActionType.Serve].MetricTypes)
        {
            Console.WriteLine(mt.ToString());
        }
        PlayerActionTextRepresentation serveTmp = new PlayerActionTextRepresentation(VolleyActionType.Serve, actionMetricTYpes[VolleyActionType.Serve]);
        serveTmp.SetPlayer(new Player("", "", 1, 1, Amplua.Setter));
        foreach (var mt in actionMetricTYpes[VolleyActionType.Serve].MetricTypes)
        {
            serveTmp.SetMetricByObject(mt, 2);
        }
        serveTmp.SetMetricByShortString(actionMetricTYpes[VolleyActionType.Serve].MetricTypes[0], "#");
        serveTmp.SetMetricByShortString(actionMetricTYpes[VolleyActionType.Serve].MetricTypes[1], "short");
        Console.WriteLine(serveTmp.LongStringFormat());
        Console.WriteLine("end");
    }

}