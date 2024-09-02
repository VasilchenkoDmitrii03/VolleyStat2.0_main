using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionsLib;
using System.Text.Json;
using System.IO;

namespace PlayerPositioningWIndow
{
    public class PlayerPositionDataContainer
    {
        Dictionary<SegmentPhase, PositionHolder> _data;
        public PlayerPositionDataContainer(Dictionary<SegmentPhase, PositionHolder> data)
        {
            _data = data;
        }
        public PlayerPositionDataContainer()
        {
            _data = new Dictionary<SegmentPhase, PositionHolder>();
            _data.Add(SegmentPhase.Recep_1, new PositionHolder());
            _data.Add(SegmentPhase.Recep, new PositionHolder());
            _data.Add(SegmentPhase.Break, new PositionHolder());
        }
        public void setForEvery(System.Windows.Point[] points)
        {
            foreach(SegmentPhase phase in _data.Keys)
            {
                _data[phase].setForEvery(points);
            }
        }
        public void setForNull(System.Windows.Point[] points)
        {
            foreach(PositionHolder positionHolder in _data.Values)
            {
                positionHolder.setForNull(points);
            }
        }
        public PositionHolder this[SegmentPhase phase]
        {
            get { return _data[phase]; }
            set { _data[phase] = value; }
        }

        public void Save(StreamWriter sw)
        {

            for(int i = 0;i < 3; i++)
            {
                sw.WriteLine(JsonSerializer.Serialize(_data.Keys.ToArray()[i] ));
                _data.Values.ToArray()[i].Save(sw);
            }
        }

        public static PlayerPositionDataContainer Load(StreamReader sr)
        {
            PlayerPositionDataContainer res = new PlayerPositionDataContainer();
            for(int i = 0; i< 3; i++)
            {
                SegmentPhase phase = JsonSerializer.Deserialize<SegmentPhase>(sr.ReadLine());
                PositionHolder holder = PositionHolder.Load(sr);
                res[phase] = holder;
            }
            return res;
        }
    }
    public class PositionHolder
    {
        public System.Windows.Point[][] _points;
        public int[][] _arrangementPositions;
        public PositionHolder(System.Windows.Point[][] points, int[][] arrangementPositions)
        {
            _points = points; _arrangementPositions = arrangementPositions;
        }
        public PositionHolder() { _points = new System.Windows.Point[6][]; _arrangementPositions = new int[6][]; setDefault(); }
        public void setArrangement(int index, System.Windows.Point[] points, int[] ints)
        {
            _points[index] = points;
            _arrangementPositions[index] = ints;  
        }
        public void setForEvery(System.Windows.Point[] points)
        {
            for(int i = 0;i < 6; i++)
            {
                setArrangement(i, points, new int[] { 0, 1, 2, 3, 4, 5 });
            }
        }
        public void setForNull(System.Windows.Point[] points)
        {
            for(int i= 0;i < 6; i++)
            {
                if (_points[i] == null) _points[i] = points;
            }
        }
        public void setDefault()
        {
            for(int i = 0;i < 6; i++)
            {
                _arrangementPositions[i] = new int[6];
                for (int j = 0; j < 6; j++)
                {
                    _arrangementPositions[i][j] = j;
                }
            }
        }
        public System.Windows.Point[] this[int index]
        {
            get { return _points[index]; }
            set { _points[index] = value; }
        }

        public void Save(System.IO.StreamWriter sw)
        {
            for(int i = 0; i < 6; i++)
            {
                SaveArray(sw, _points[i], _arrangementPositions[i]);
            }
        }
        public static PositionHolder Load(StreamReader sr)
        {
            System.Windows.Point[][] res = new System.Windows.Point[6][];
            int[][] ints = new int[6][];
            for(int i = 0; i < 6; i++)
            {
                res[i] = LoadArray(sr);
                ints[i] = JsonSerializer.Deserialize<int[]>(sr.ReadLine());
            }
            return new PositionHolder(res, ints);
        }
        public void SaveArray(System.IO.StreamWriter sw, System.Windows.Point[] pts, int[] ints)
        {
            sw.WriteLine(JsonSerializer.Serialize(pts));
            sw.WriteLine(JsonSerializer.Serialize(ints));
        }
        private static System.Windows.Point[] LoadArray(System.IO.StreamReader sr)
        {
           System.Windows.Point[] res = JsonSerializer.Deserialize<System.Windows.Point[]>(sr.ReadLine());
            return res;
        }

    }
}
