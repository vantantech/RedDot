using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RedDot
{
    public class StringToBackgroundConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null) return "Red";


            if (value.ToString() == "Reversed") return "Orange";
            if (value.ToString() == "Voided") return "LightGray";

            if (value.ToString() == "0") return "White";

            if (value.ToString() == "10") return "Yellow";

            if (value.ToString() == "50") return "Orange";

            if (value.ToString() == "100") return "Red";



            return "Transparent";
            // return Binding.DoNothing;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == true)
                    return "yes";
                else
                    return "no";
            }
            return "no";
        }
    }

}
