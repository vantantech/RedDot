using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RedDot
{
    /// <summary>
    /// Interaction logic for ApptDetails.xaml
    /// </summary>
    public partial class ApptDetails : Window
    {
        private ApptDetailsVM m_apptdetailsvm;
        public string Action { get; set; }
        public ApptDetails(Appointment appt, Security security, bool NewAppointment)
        {
            InitializeComponent();

            if (NewAppointment)
            {
                btnDelete.Visibility = Visibility.Collapsed;


            }else
            {
                btnDelete.Visibility = Visibility.Visible;


            }
            m_apptdetailsvm = new ApptDetailsVM(this,appt,security);
            this.DataContext = m_apptdetailsvm;

            List<string> data = new List<string>();

            DateTime currenttime = new DateTime(2016, 01, 01, GlobalSettings.Instance.DayStartHour, 00, 00);  //Jan 1, 2016 08:00
            int daylength = GlobalSettings.Instance.DayLength;
            int interval = GlobalSettings.Instance.AppointmentInterval;

            for (int i = 0; i < daylength * 60/interval; i++)
            {
                data.Add(currenttime.ToShortTimeString());
                currenttime = currenttime.AddMinutes(interval);
            }
            this.combo1.ItemsSource = data;
            this.combo1.SelectedIndex = 0;

            //this.startdatepicker.SelectedDate = appttime;
        }

        private void Button_OK(object sender, RoutedEventArgs e)
        {
            Action = "OK";
          
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            Action = "CANCEL";
            this.Close();
        }

        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            Action = "DELETE";
           
        }
    }
}
