using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RedDot
{
    public class EmptyToVisibilityConverter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null) return Visibility.Collapsed;
            if (value.ToString() == "") return Visibility.Collapsed;
            if (value.ToString() == "none") return Visibility.Collapsed;
            if (value.ToString() == "pack://siteoforigin:,,,/") return Visibility.Collapsed;
            if (value.ToString() == "0") return Visibility.Collapsed;

            switch (value.ToString().Trim().Length)
            {
                case 0:
                    return Visibility.Collapsed;
                default:
                    return Visibility.Visible;
                   // return Binding.DoNothing;
            }
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
