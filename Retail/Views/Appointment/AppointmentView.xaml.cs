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
    /// Interaction logic for Appointments.xaml
    /// </summary>
    public partial class AppointmentView : Window
    {
        private AppointmentVM m_appointmentvm;
        public AppointmentView(Security security)
        {
            InitializeComponent();
            m_appointmentvm = new AppointmentVM(this,security);
            this.DataContext = m_appointmentvm;
                
        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
