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
        public EditHours(string intime, string outtime, string note)
        {
            InitializeComponent();

            InTime = intime;
            OutTime = outtime;
            Note = note;
            Action = "none";

            this.tbTimeIn.Text = intime;
            this.tbTimeOut.Text = outtime;

        }

        public string InTime { get; set; }
        public string OutTime { get; set; }

        public string Note { get; set; }

        public string Action { get; set; }


        private void Button_OK(object sender, RoutedEventArgs e)
        {

            Validate();
        }

        private void Validate()
        {

            Action = "Save";
            InTime = this.tbTimeIn.Text;
            OutTime = this.tbTimeOut.Text;
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
