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
    /// Interaction logic for GCVerify.xaml
    /// </summary>
    public partial class GCVerify : Window
    {

        public string RawString = "";
        public string CardNumber = "";
        private string m_cardnumber = "";
        int trackcount = 0;
        public GCVerify()
        {
            InitializeComponent();
        }


        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {


            if(e.Text == "%")
            {
                this.tbAmount.Foreground = Brushes.White;
                this.tbAmount.Text = "";
                RawString = "";
                this.txtGiftCard.Text = "Reading....";
                trackcount = 0;
            }


            RawString = RawString + e.Text;

            if (e.Text == "?") trackcount++;

            if (e.Text == "\r") ProcessInput(RawString);

            this.tbTest.Text = RawString;

        }



        private void ProcessInput(string inputstr)
        {
           decimal result;
           string msrcardprefix;

           msrcardprefix = GlobalSettings.Instance.MSRCardPrefix;

           try
           {
               if (trackcount >= 1)
               {
                   string[] tracks = RawString.Split(';');

                   string[] data = tracks[0].Split('^');

                   //string strID = data[0];
                   //string name = data[1];
                   //string expiration = data[2];

                   string data1 = data[0];
                   this.tbTest.Text = data1;
                   RawString = "";

                   if (data1.Length >= 1)
                       if (data1.Contains(msrcardprefix))
                       {
                           m_cardnumber = data1.Replace(msrcardprefix, "").Replace("?", "").Replace("\r","");
                           txtGiftCard.Text = m_cardnumber;
                           this.tbTest.Text = "";
                           //get value
                           result = GiftCardModel.GetGiftCardBalance(m_cardnumber);
                           if (result == -99)
                           {
                               this.tbAmount.Foreground = Brushes.Red;
                               this.tbAmount.Text = "Card not activated...";
                           }
                           else
                           {
                               this.tbAmount.Foreground = Brushes.White;
                               this.tbAmount.Text = result.ToString();
                           }
                       }
                       else
                       {
                           MessageBox.Show("Card Prefix is invalid");
                       }

                   trackcount = 0;

               }
           }catch(Exception e)
           {
               this.tbAmount.Foreground = Brushes.Red;
               this.tbAmount.Text = "Error reading card..." + e.Message;
               RawString = "";
               trackcount = 0;

           }

           

        }


        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
