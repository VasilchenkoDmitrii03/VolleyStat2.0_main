using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionsLib;

namespace StatisticsCreatorModule.LiberoModeSetter
{
    public class LiberoArrangementDataContainer
    {
        Dictionary<SegmentPhase, LiberoPhaseDataContainer> _data;
        public LiberoArrangementDataContainer()
        {
            _data = new Dictionary<SegmentPhase, LiberoPhaseDataContainer>();
            _data.Add(SegmentPhase.Recep_1, new LiberoPhaseDataContainer());
            _data.Add(SegmentPhase.Break, new LiberoPhaseDataContainer());
        }
        public void fillData(SegmentPhase phase, int index, int value)
        {
            _data[phase].fillData(index, value);
        }
        public int getData(SegmentPhase phase, int index)
        {
            return _data[phase].getData(index);
        }
    }
    public class LiberoPhaseDataContainer
    {
        int[] data;
        public LiberoPhaseDataContainer()
        {
            data = new int[6];
            for (int i = 0; i < 6; i++) data[i] = -1;
        }
        public void fillData(int i, int value)
        {
            data[i] = value;
        }
        public int getData(int i)
        {
            return data[i];
        }
    }
}
