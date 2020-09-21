using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Interaction logic for Credit.xaml
    /// </summary>
    public partial class Credit : Window
    {

        public decimal Amount { get { return _amount; } set { _amount = value; txtAmount.Text = " " + value.ToString(); } }
        public string ReturnCode { get; set; }
        private string RawString;
        private decimal _amount;
        private string _expiration;
        private string _creditcardnumber;

        public Credit()
        {
            InitializeComponent();
 
            
        }

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            

            RawString = RawString + e.Text;
       

            if (e.Text == "/r")
            {
                string[] tracks = RawString.Split(';');
                string[] data = tracks[0].Split('^');

                string creditcardnumber = data[0];
                string name = data[1];
                string expiration = data[2];


                _expiration = expiration.Substring(0, 4);
                _creditcardnumber = creditcardnumber.Substring(2);


                txtCredit.Text = "  " + _creditcardnumber;
                txtYear.Text = "  20" + expiration.Substring(0, 2);
                txtMonth.Text = "  " + expiration.Substring(2, 2);
                txtName.Text = "  " + name;


                
            }
            
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            ReturnCode = "Error";
            this.Close();
        }
        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            ReturnCode = ProcessCard();
            this.Close();
        }
 
        private string ProcessCard()
        {
            bool rtn;

            _creditcardnumber = "4012888818888";
            
          //  var creditCard = new creditCardType
          //  {
           //    cardNumber = _creditcardnumber,
            //   expirationDate = _expiration
           // };


           // String apiLoginId = "3neZ9NC53"; VTT id
           // String transactionKey = "34GMxY5fu8kx4U79";

            String apiLoginId = "2CnQc364E86";
            String transactionKey = "96U55wNp6dvs3Qtn";
            //api login id:   2CnQc364E86
//transaction key: 8uG45W5mt8p7Uexn
//secret key: Simon

            WaitView wait = new WaitView();
            wait.Show();

            AuthorizeDotNet proc = new AuthorizeDotNet(apiLoginId, transactionKey);
            rtn = proc.Authorize("4111111111111111", "0718", _amount);
            wait.Close();
           

            if (rtn == true)
            {
                return proc.ReturnCode;
            }
            else return "Error";

           

        }

    }
}
