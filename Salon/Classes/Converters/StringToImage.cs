using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace RedDot
{
    public class StringToImage : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            string path = "";
            if (value.ToString() != "")
            {
                switch (value.ToString())
                {
                   


                    case "Application":
                        path = "parameter.png";
                        break;
                    case "SMS":
                        path = "sendsms.png";
                        break;
                    case "Credit Card":
                        path = "credit.png";
                        break;
                    case "CreditCard":
                        path = "credit.png";
                        break;
                    case "Order Entry":
                        path = "notes.png";
                        break;
                    case "Receipt":
                        path = "receipt.png";
                        break;

                    case "Ticket":
                        path = "ticket.png";
                        break;
                    case "System":
                        path = "system.png";
                        break;
                    case "Store":
                        path = "store.png";
                        break;

                    case "Reward":
                        path = "rewardpoints.png";
                        break;
                    case "Screens":
                        path = "screens.png";
                        break;

                    case "Payment":
                        path = "cash.png";
                        break;

                    default:
                        path = "sphere.png";
                        break;
                }

            }
            else path = "add.png";


            var uri = string.Format("pack://application:,,,/media/{0}", path);
            return new BitmapImage(new Uri(uri));
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            return "no";
        }
    }
}

