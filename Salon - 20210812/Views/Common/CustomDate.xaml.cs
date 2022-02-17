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
    /// Interaction logic for CustomDate.xaml
    /// </summary>
    public partial class CustomDate : Window
    {

        public DateTime StartDate;
        public DateTime EndDate;
        public Visibility VisibleEndDate { get; set; }
        public CustomDate(Visibility visibleenddate)
        {
            VisibleEndDate = visibleenddate;
            InitializeComponent();
            this.DataContext = this;
        }

        private void startdatepicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get DatePicker reference.
            var picker = sender as DatePicker;

            // ... Get nullable DateTime from SelectedDate.
            StartDate = (DateTime) picker.SelectedDate;

        }

        private void enddatepicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get DatePicker reference.
            var picker = sender as DatePicker;

            // ... Get nullable DateTime from SelectedDate.
            EndDate = (DateTime)picker.SelectedDate;

        }

        private void Button_OK(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Same(object sender, RoutedEventArgs e)
        {
            enddatepicker.SelectedDate = startdatepicker.SelectedDate;
        }


    }
}
