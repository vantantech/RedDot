using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class NumberToColorConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null) return "Red";

            if (value.ToString() == "") return "Red";


            if (decimal.Parse(value.ToString()) > 0) return "Green";

            if (decimal.Parse(value.ToString()) < 0) return "Red";

            return "Black";


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
