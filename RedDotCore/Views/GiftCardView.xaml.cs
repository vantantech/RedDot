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
using System.Windows.Threading;

namespace RedDot
{
    /// <summary>
    /// Interaction logic for GiftCardView.xaml
    /// </summary>
    public partial class GiftCardView : Window
    {
        private PaymentViewModel paymentviewmodel;
        public string RawString = "";
        public string CardNumber = "";
        private string m_cardnumber = "";
        int trackcount = 0;


        public GiftCardView(int salesid, int customerid, decimal balance)
        {
            InitializeComponent();

            paymentviewmodel = new PaymentViewModel(this, salesid,customerid,balance);
            this.DataContext = paymentviewmodel;
    

        }




        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "%")
            {
                this.tbMessage.Foreground = Brushes.White;
                this.tbMessage.Text = "";
                tbGiftCard.Text = "Reading ...";
                RawString = "";
                trackcount = 0;
            }
            RawString = RawString + e.Text;

            if (e.Text == "?")
            {
                trackcount++;

            }
            if (e.Text == "\r") ProcessInput(RawString);


            this.tbTemp.Text = RawString;

        }

        private void ProcessInput(string inputstr)
        {

            string msrcardprefix;

            msrcardprefix = GlobalSettings.Instance.MSRCardPrefix;


            try
            {
                if (trackcount >= 1)
                {
                    string[] tracks = RawString.Split(';');

                    string[] data = tracks[0].Split('^');

                    string data1 = data[0];
                   // this.tbTemp.Text = data1;
                    decimal balance;
                   // RawString = "";

                    if (data1.Length >= 1)



                        if (data1.Contains(msrcardprefix))
                        {
                            m_cardnumber = data1.Replace(msrcardprefix, "").Replace("?", "").Replace("\r", "");
                            tbGiftCard.Text = m_cardnumber;

                           // this.tbTemp.Text = "";
                            balance = paymentviewmodel.CheckBalance(m_cardnumber);

                            //verify if card has been activated (0 or greater)
                            if (balance == -99)
                            {
                                tbMessage.Foreground = Brushes.Red;
                                tbMessage.Text = "Gift Card NOT Activated";
                               // RawString = "";
                            }
                            //verify if card has already been used on this ticket
                            if (balance == -100)
                            {
                                tbMessage.Foreground = Brushes.Red;
                                tbMessage.Text = "Gift Card already used on this ticket";
                                // RawString = "";
                            }
                                trackcount = 0;
                        }
                }
            }
            catch (Exception e)
            {
                tbMessage.Foreground = Brushes.Red;
                tbMessage.Text = "Error reading card .. please try again" + e.Message;
               RawString = "";
                trackcount = 0;
            }
        }

    }
}
