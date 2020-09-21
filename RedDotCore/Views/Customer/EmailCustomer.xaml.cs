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
    /// Interaction logic for SearchbyName.xaml
    /// </summary>
    public partial class EmailCustomer : Window
    {
        public string To;
        public string Subject;
        public string Message;
        public string Action;
        public EmailCustomer(string to, string subject, string message, string attachment)
        {
            InitializeComponent();
            To = to;
            Subject = subject;
            Message = message;

            this.tbTo.Text = to;
            this.tbSubject.Text = subject;
            this.tbMessage.Text = message;
            this.tbAttachment.Text = attachment;
        }

        private void Button_OK(object sender, RoutedEventArgs e)
        {
            To = tbTo.Text;
            Subject = tbSubject.Text;
            Message = tbMessage.Text;
            Action = "send";

            this.Close();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            
            Action = "cancel";
            this.Close();
        }
    }
}
