using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MetricTypesWindow.Converters
{
    class ShortNamesArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string res = "";
            string[] values = ((List<string>)value).ToArray();
            foreach (string str in values)
            {
                res += str + ",";
            }
            if (res.Length > 0) res = res.Remove(res.Length - 1, 1);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] values = ((string)value).Split(',');
            List<string> list  = new List<string>();
            int ind = 1;
            foreach (string str in values)
            {
                list.Add(str);
            }
            return list;
        }
    }
}
