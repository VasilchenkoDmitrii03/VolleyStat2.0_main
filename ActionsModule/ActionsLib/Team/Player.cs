using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ActionsLib
{
    public class Player
    {
        string _name;
        string _surname;
        int _height;
        int _number;
        Amplua _amplua;

        public Player(string name, string surname, int height, int number, Amplua amplua)
        {
            _name = name;
            _surname = surname;
            _height = height;
            _number = number;
            _amplua = amplua;
        }

        public int Number
        {
            get { return _number; }
        }
        public Amplua Amplua
        {
            get { return _amplua; }
            set { _amplua = value; }
        }
        public string Surname
        {
            get { return _surname; }
        }
        public int Height
        {
            get { return _height; }
        }
        public string Name
        {
            get { return _name; }
        }
        public static bool operator ==(Player left, Player right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;
            return left.Number == right.Number;
        }
        public static bool operator !=(Player left, Player right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
                return false;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return true;
            return left.Number != right.Number;
        }

        public string ToTableString()
        {
            return $"{_number} {_surname} {_name}";
        }
    }

    public enum Amplua
    {
        Libero = 0,
        Setter = 1,
        OutsideHitter = 2,
        MiddleBlocker = 3,
        Opposite = 4,
        Undefined = -1
    }
}
