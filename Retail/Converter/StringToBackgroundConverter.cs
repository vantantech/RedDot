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
            if (value.ToString() == "") return "DarkGray";
            if (value.ToString() == "Closed")  return "AliceBlue";

            if(value.ToString() == "Open")  return "Lightgray";
            if(value.ToString() == "Reversed")   return "Orange";
            if (value.ToString() == "Pending") return "LightBlue";
            if (value.ToString() == "Sales Rep") return "LightBlue";
            if (value.ToString() == "admin") return "Yellow";
            if (value.ToString() == "Voided") return "DarkGray";
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
