using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionsLib;

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
        public PositionHolder this[SegmentPhase phase]
        {
            get { return _data[phase]; }
            set { _data[phase] = value; }
        }
    }
    public class PositionHolder
    {
        public System.Windows.Point[][] _points;
        public PositionHolder(System.Windows.Point[][] points) { _points = points; }
        public PositionHolder() { _points = new System.Windows.Point[6][]; }
        public void setArrangement(int index, System.Windows.Point[] points)
        {
            _points[index] = points;
        }
        public void setForEvery(System.Windows.Point[] points)
        {
            for(int i = 0;i < 6; i++)
            {
                setArrangement(i, points);
            }
        }
        public System.Windows.Point[] this[int index]
        {
            get { return _points[index]; }
            set { _points[index] = value; }
        }
    }
}
