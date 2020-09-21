using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RedDot
{
    public class FalseToVisibilityConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null) return Visibility.Collapsed;
            if (value.ToString() == "") return Visibility.Collapsed;
            if (value.ToString() == "none") return Visibility.Collapsed;

            if (value is bool)
            {
                if ((bool)value == true) return Visibility.Hidden; else return Visibility.Visible;
            }
            else return Visibility.Hidden;
            
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
