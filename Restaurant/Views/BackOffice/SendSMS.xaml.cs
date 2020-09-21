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
    /// Interaction logic for SendSMS.xaml
    /// </summary>
    public partial class SendSMS : Window
    {

        SendSMSVM sendsmsvm;

        public SendSMS()
        {
            InitializeComponent();
            sendsmsvm = new SendSMSVM(this);

            this.DataContext = sendsmsvm;
        }





        private void Exit_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbMessageLength.Text = tbMessage.Text.Length.ToString() + "/160";
        }



    }
}
