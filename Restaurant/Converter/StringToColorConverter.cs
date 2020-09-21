using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RedDot
{
    public class StringToColorConverter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null) return "Red";

            if (value.ToString()=="") return "Red";
            if (value.ToString() == "0") return "Red";
            if (value.ToString() == "0.00") return "Red";
            if (value.ToString() == "YES") return "Green";

            if (value.ToString().Contains("[N/A]")) return "Yellow";
            if (value.ToString().Contains("[empty]")) return "Yellow";

            if (value.ToString().Length > 3)
            {
                switch (value.ToString().Substring(0, 4))
                {
                    case "Attn":
                    case "Need":
                        return "Yellow";
                    case "AllF":
                        return "White";
                    case "Clos":
            
                        return "White";
                    case "Aler":
                    case "Open":
                        return "Red";
                    case "Reve":
                        return "Orange";

                    case "Dine":
                        return "Beige";
                    case "ToGo":
                        return "AliceWhite";
                    case "Void":
                        return "LightGray";
                    case "Inac":
                        return "Red";
                    case "AUTH":
                        return "GreenYellow";
                    default:
                        return "Transparent";
                    // return Binding.DoNothing;
                }

            }
            else return "";

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
