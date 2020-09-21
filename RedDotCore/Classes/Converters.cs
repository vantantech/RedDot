using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;


namespace RedDot
{
    public class BooleanToColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value is bool)
            {
                if ((bool)value == true)
                    return "Red";
                else
                    return "Transparent";
            }
            return "Red";



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


    public class CombineGiftCardConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ContentToPathConverter : IValueConverter
    {
        readonly static ContentToPathConverter value = new ContentToPathConverter();
        public static ContentToPathConverter Value
        {
            get { return value; }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ContentPresenter cp = (ContentPresenter)value;
            double h = cp.ActualHeight > 10 ? 1.4 * cp.ActualHeight : 10;
            double w = cp.ActualWidth > 10 ? 1.25 * cp.ActualWidth : 10;
            PathSegmentCollection ps = new PathSegmentCollection(4);
            ps.Add(new LineSegment(new Point(1, 0.7 * h), true));
            ps.Add(new BezierSegment(new Point(1, 0.9 * h), new Point(0.1 * h, h), new Point(0.3 * h, h), true));
            ps.Add(new LineSegment(new Point(w, h), true));
            ps.Add(new BezierSegment(new Point(w + 0.6 * h, h), new Point(w + h, 0), new Point(w + h * 1.3, 0), true));
            //return ps; // Fix

            // Fix
            PathFigure figure = new PathFigure(new Point(1, 0), ps, false);
            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            return geometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class ContentToMarginConverter : IValueConverter
    {
        readonly static ContentToMarginConverter value = new ContentToMarginConverter();
        public static ContentToMarginConverter Value
        {
            get { return value; }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Thickness(0, 0, -((ContentPresenter)value).ActualHeight, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class StringToColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null) return "Red";

            if (value.ToString() == "") return "Red";
            if (value.ToString() == "0") return "Red";
            if (value.ToString() == "YES") return "Green";

            if (value.ToString().Contains("[N/A]")) return "Yellow";
            if (value.ToString().Contains("[empty]")) return "Yellow";

            if (value.ToString().Length > 3)
            {
                switch (value.ToString().Substring(0, 4))
                {
                    case "Attn":
                        return "Yellow";
                    case "Clos":
                        return "White";
                    case "Open":
                        return "Red";
                    case "Reve":
                        return "Orange";
                    case "Void":
                        return "LightGray";
                    case "Inac":
                        return "Red";
                    case "AUTH":
                        return "Orange";

                    default:
                        return "Transparent";
                    // return Binding.DoNothing;
                }

            }
            else return "";

        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
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


    public class BalanceToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null) return "Red";

            if (value.ToString() == "") return "Red";


            if (decimal.Parse(value.ToString()) == 0) return "Black";

            if (decimal.Parse(value.ToString()) != 0) return "Red";

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

    public class NumberToColorConverter : IValueConverter
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

    public class InverseConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (decimal)value * (-1);
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (decimal)value * (-1);
        }
    }


    public class EmptyToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null) return Visibility.Collapsed;
            if (value.ToString() == "") return Visibility.Collapsed;
            if (value.ToString() == "none") return Visibility.Collapsed;


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


    public class BooleanToYellowConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value is bool)
            {
                if ((bool)value == true)
                    return "Yellow";
                else
                    return "AliceBlue";
            }
            return "AliceBlue";



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





    public class ReversedToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null) return Visibility.Collapsed;
            if (value.ToString() == "") return Visibility.Collapsed;
            if (value.ToString() == "none") return Visibility.Collapsed;


            if (value.ToString().ToUpper() == "REVERSED")
            {
                
                    return Visibility.Visible;
               
            }else
            {
                return Visibility.Collapsed;
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


    public class ClosedToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null) return Visibility.Collapsed;
            if (value.ToString() == "") return Visibility.Collapsed;
            if (value.ToString() == "none") return Visibility.Collapsed;


            if (value.ToString().ToUpper() == "CLOSED")
            {

                return Visibility.Visible;

            }
            else
            {
                return Visibility.Collapsed;
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
