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
    /// Interaction logic for EditHours.xaml
    /// </summary>
    public partial class EditHours : Window
    {
        public EditHours(DateTime intime, DateTime outtime, string note)
        {
            InitializeComponent();

            InTime = intime;
            OutTime = outtime;
            Note = note;
            Action = "none";

            this.startdatepicker.SelectedDate = intime;
            this.enddatepicker.SelectedDate = outtime;
            this.tbTimeIn.Text = intime.ToShortTimeString();
            this.tbTimeOut.Text = outtime.ToShortTimeString();
            this.tbNote.Text = note;

        }

        public DateTime InTime { get; set; }
        public DateTime OutTime { get; set; }

        public string Note { get; set; }

        public string Action { get; set; }


        private void Button_OK(object sender, RoutedEventArgs e)
        {

            Validate();
        }

        private void Validate()
        {

            Action = "Save";

            if (this.startdatepicker.SelectedDate.HasValue)
            {
                DateTime startdate = (DateTime)this.startdatepicker.SelectedDate;
                InTime = DateTime.Parse(startdate.ToShortDateString() + " " + this.tbTimeIn.Text);
            }
      
            if(this.enddatepicker.SelectedDate.HasValue)
            {
                DateTime enddate = (DateTime)this.enddatepicker.SelectedDate;
                OutTime = DateTime.Parse(enddate.ToShortDateString() + " " + this.tbTimeOut.Text);
            }



            Note = this.tbNote.Text;

            this.Close();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {

            Action = "Cancel";
            this.Close();
        }


  
    }
}
