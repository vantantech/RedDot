using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RedDot
{
    public class NumberToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null) return 1;

            if (value.ToString() == "") return 1;


            if (decimal.Parse(value.ToString()) > 0) return 0.2;
            if (decimal.Parse(value.ToString()) == 0) return 1;

            if (decimal.Parse(value.ToString()) < 0) return 1;

            return 1;


        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((string)value == "error")
            {
                if ((bool)value == true)
                    return 5;
                else
                    return 0;
            }
            return 0;
        }
    }
}
