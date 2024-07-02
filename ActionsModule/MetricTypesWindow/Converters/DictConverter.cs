using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MetricTypesWindow.Converters
{
    class MyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string res = "";
            string[] values = ((Dictionary<object, string>)value).Values.ToArray();
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
            Dictionary<object, string> res = new Dictionary<object, string>();
            int ind = 1;
            foreach (string str in values)
            {
                res.Add(ind++, str);
            }
            return res;
        }
    }
}
