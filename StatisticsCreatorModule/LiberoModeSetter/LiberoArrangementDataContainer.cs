using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionsLib;
using System.Text.Json;
namespace StatisticsCreatorModule.LiberoModeSetter
{
    public class LiberoArrangementDataContainer
    {
        Player[] Players;
        Dictionary<SegmentPhase, LiberoPhaseDataContainer> _data;
        int arrangementNumberForChange = -1;
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
        public int ArrangementNumberForChange
        {
            get { return arrangementNumberForChange; }
            set {  arrangementNumberForChange = value; }
        }
        public void fillDefault()
        {
            foreach(SegmentPhase phase in _data.Keys)
            {
                _data[phase].fillDefault();
            }
        }
        public int getLiberoCount()
        {
            int max = -1;
            foreach(SegmentPhase phase in _data.Keys)
            {
                if (_data[phase].getLiberoCount() > max) max = _data[phase].getLiberoCount();
            }
            return max;
        }
        public void setLiberos(Player[] players)
        {
            players.CopyTo(Players, 1);
        }
        public void setFieldPlayer(Player player)
        {
            Players[0] = player;
        }
        public Player getCurrentPlayer(SegmentPhase currentPhase, int currentArrangement)
        {
            return Players[_data[currentPhase].getData(currentArrangement)];
        }

        public void Save(StreamWriter sw)
        {
            sw.WriteLine(JsonSerializer.Serialize(arrangementNumberForChange));
            for(int i = 0;i < 2; i++)
            {
                sw.WriteLine(JsonSerializer.Serialize(_data.Keys.ToArray()[i]));
                _data[_data.Keys.ToArray()[i]].Save(sw);
            }
        }
        public static LiberoArrangementDataContainer Load(StreamReader sr)
        {
            int arrang = JsonSerializer.Deserialize<int>(sr.ReadLine());
            LiberoArrangementDataContainer res = new LiberoArrangementDataContainer();
            for (int i = 0; i < 2; i++)
            {
                SegmentPhase ph = JsonSerializer.Deserialize<SegmentPhase>(sr.ReadLine());
                LiberoPhaseDataContainer tmp = LiberoPhaseDataContainer.Load(sr);
                res._data[ph] = tmp;
            }
            return res;
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
        public LiberoPhaseDataContainer(int[] data)
        {
            this.data = new int[6];
            data.CopyTo(this.data, 0);
        }
        public void fillData(int i, int value)
        {
            data[i] = value;
        }
        public int getData(int i)
        {
            return data[i];
        }
        public void fillDefault()
        {
            for (int i = 0; i < 6; i++) data[i] = 0;
        }
        public int getLiberoCount()
        {
            int max = data.Max();
            return max;
        }
        public void Save(StreamWriter sw)
        {
            sw.WriteLine(JsonSerializer.Serialize(data));
        }
        public static LiberoPhaseDataContainer Load(StreamReader sr)
        {
            int[] res = JsonSerializer.Deserialize<int[]>(sr.ReadLine());
            return new LiberoPhaseDataContainer(res);
        }
    }
}
