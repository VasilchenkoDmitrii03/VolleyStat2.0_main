
using ActionsLib;
using ActionsLib.ActionTypes;
using System.Xml.Serialization;
using ActionsLib.TextRepresentation;
using System.IO;
class Program
{
    List<MetricType> _metricTypes = new List<MetricType>();

    public static void Main(string[] args)
    {
        /*ActionSequence tmp = new ActionsLib.ActionSequence();
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
        Console.WriteLine("end");*/
        OpenTest();
    }
    private static void OpenTest()
    {
        Team _team = new Team();
        for (int i = 1; i < 20; i++)
        {
            _team.AddPlayer(new Player($"{i}", $"{i}", 1, i, Amplua.OutsideHitter));
        }
            ActionsMetricTypes actionMetricTYpes = ActionsMetricTypes.Load(@"C:\Dmitrii\Programming\VolleyStat2.0_main\BasicActionsMetrics");
            ActionLoader.currentTeam = _team;
            ActionLoader.ActionsMetricTypes = actionMetricTYpes;
            using (StreamReader sr = new StreamReader(@"C:\Dmitrii\newTestExt.txt"))
            {
                    Set set = Set.Load(sr);
                    foreach(ActionsLib.Action act in set.ConvertToSequence())
            {
                if (act == null) Console.WriteLine();
                else Console.WriteLine(act.ExtendedString);
            }
            }
        }
    }

