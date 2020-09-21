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
    /// Interaction logic for GiftCardScanner.xaml
    /// </summary>
    public partial class CardScanner : Window
    {
        public string RawString = "";
        public string CardNumber = "";
        private string m_cardnumber = "";
        int trackcount = 0;

       

        public CardScanner()
        {
            InitializeComponent();

                     
        }





    
      


        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "%")
            {
                this.tbMessage.Foreground = Brushes.White;
                this.tbMessage.Text = "Reading ...";
                txtGiftCard.Text = "Reading ...";
                RawString = "";
                trackcount = 0;
            }
            RawString = RawString + e.Text;

            if (e.Text == "?") trackcount++;

            if (e.Text == "\r")
            {
                this.tbMessage.Text = "Track data: " + RawString;
                ProcessInput(RawString);
            }

            this.tbTemp.Text = RawString;

        }

        private void ProcessInput(string inputstr)
        {

           


            try
            {
               

                if (trackcount >= 1)
                {
                    string[] tracks = inputstr.Split(';');

                    //process track 1
                    string[] data = tracks[0].Split('^');

                    // string name = data[1];
                    //string expiration = data[2];
                    string data1;

                    if (data.Length >= 1)
                    {

                        data1 = data[0].ToUpper();
                        this.tbTemp.Text = data1;
                        RawString = "";

                        if (data1.Contains("%B") || data1.Contains("%R"))
                        {
                            m_cardnumber = data1.Replace("%B", "").Replace("%R", "").Replace("?", "").Replace("\r", "");
                            txtGiftCard.Text = m_cardnumber;
                            this.tbTemp.Text = "";
       
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


    
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
          

            CardNumber = m_cardnumber;
            this.Close();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
           

            this.Close();
        }


    }
}
