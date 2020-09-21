using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;


namespace RedDot
{
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

    public class EnumMatchToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            
            string checkValue = value.ToString();
            decimal quantity = decimal.Parse(checkValue);
            string targetValue = parameter.ToString();
            if(targetValue == "-99")
            {
                if (quantity != (int)quantity || quantity > 9) return true;
                else return false;
            }else
            return checkValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true ;
        }
    }

    public class NumberToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null) return "Red";

            if (value.ToString() == "") return "Red";


            if (decimal.Parse(value.ToString()) > 0) return "Green";
            if (decimal.Parse(value.ToString()) == 0) return "Transparent";

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

    public class NumberToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null) return Visibility.Collapsed;
            if (value.ToString() == "") return Visibility.Collapsed;
            if (value.ToString() == "none") return Visibility.Collapsed;

            if (decimal.Parse(value.ToString()) > 0) return Visibility.Visible;
            if (decimal.Parse(value.ToString()) == 0) return Visibility.Collapsed;

            if (decimal.Parse(value.ToString()) < 0) return Visibility.Collapsed;

            return Visibility.Collapsed;

          
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


