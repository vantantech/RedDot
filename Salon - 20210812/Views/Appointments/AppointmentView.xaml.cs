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
        protected Point SwipeStart;
        bool ready = true;

        public AppointmentView(SecurityModel security)
        {
            InitializeComponent();
            m_appointmentvm = new AppointmentVM(this,security,1024);
            this.DataContext = m_appointmentvm;
                
        }

  

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
          
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (ready)
                    {
                        if (SwipeStart.X == 0) SwipeStart = e.GetPosition(this);

                        var Swipe = e.GetPosition(this);

                        //Swipe right
                        if (Swipe.X > (SwipeStart.X + 200))
                        {
                            ready = false;
                            m_appointmentvm.ExecutePreviousEmployeeClicked(null);


                        }

                        //Swipe left
                        if (Swipe.X < (SwipeStart.X - 200))
                        {
                            ready = false;
                            m_appointmentvm.ExecuteNextEmployeeClicked(null);


                        }
                    }
                
                }
                else
                {
                    SwipeStart = new Point();
                    ready = true;
                }
         
           
               

      
            e.Handled = true;
        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Size_Changed(object sender, SizeChangedEventArgs e)
        {
            bool changed = e.WidthChanged;
            double width = e.NewSize.Width;
            if(changed)
                m_appointmentvm.WindowWidth =(int) (width - 100);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.calendar.Visibility = Visibility.Visible;
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            this.calendar.Visibility = Visibility.Hidden;
        }

     
    }
}
