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
    /// Interaction logic for NumPad.xaml
    /// </summary>
    public partial class NumPad : Window
    {
        private string _amount="";
        private string _original="";
  
      
        public NumPad(string title, bool IntegerOnly, bool AllowNegatives, string defaultamount="")
        {
            InitializeComponent();
            this.tbTicketAmount.Text = defaultamount;
            if (defaultamount == "") btnQuick.Visibility = Visibility.Hidden;
       
            if(title != "")  tbTitle.Text = title;
            if (IntegerOnly)
            {
                btndot.Visibility = Visibility.Hidden;
                btn14.Visibility = Visibility.Hidden;
                btn12.Visibility = Visibility.Hidden;
                btn34.Visibility = Visibility.Hidden;
            }

            if(AllowNegatives == false)
                this.btnneg.Visibility = Visibility.Hidden;

        }

        public string Amount
        {
            get { return _amount; }

            set
            {
                _amount = value;
                _original = value;
                tbAmount.Text = value;
            }
        }

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {

            if(e.Text =="\r")
            {
                Validate();
                return;
            }

            if(e.Text =="\b")
            {
                //backspace
                if(tbAmount.Text != "")
                tbAmount.Text = tbAmount.Text.Substring(0, tbAmount.Text.Length - 1);
            }else
            {
                tbAmount.Text = tbAmount.Text + e.Text;
            }
           
            GlobalSettings.CustomerDisplay.WriteRaw("Please Enter Number",tbAmount.Text);

        }
        private void Button_OK(object sender, RoutedEventArgs e)
        {

            Validate();
        }


        private void Validate()
        {

            //remove all  parentheses and emptyspaces
            _amount = tbAmount.Text.Replace("(", "").Replace(")", "").Replace(" ", "");

            decimal amt = decimal.Parse(_amount);
            if(amt < 0)
            {
                SecurityModel security = new SecurityModel();
                if(!security.ManagerOverrideAccess("Refund","Refund require Override!!"))
                {
                    TouchMessageBox.Show("Refund access not allowed, please remove negative quantity!!!");
                    return;
                }

            }

            GlobalSettings.CustomerDisplay.Clear();
            this.Close();
        }
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            tbAmount.Text = tbAmount.Text + button.Content.ToString();
       
        }




        private void Button_Clear(object sender, RoutedEventArgs e)
        {
            //Clear 
            tbAmount.Text = "";
            GlobalSettings.CustomerDisplay.Clear();
 
            //BAck space
            //tbAmount.Text = tbAmount.Text.Substring(0, tbAmount.Text.Trim().Length - 1);
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            _amount = _original;
            GlobalSettings.CustomerDisplay.Clear();
            this.Close();
        }

        private void tbTextChanged(object sender, TextChangedEventArgs e)
        {
            GlobalSettings.CustomerDisplay.WriteRaw("Data:",tbAmount.Text);
        }

        private void Quick_Click(object sender, RoutedEventArgs e)
        {
            _amount = this.tbTicketAmount.Text;
            this.Close();
        }

        private void Quick_Click14(object sender, RoutedEventArgs e)
        {
            _amount = ".25";
            this.Close();
        }

        private void Quick_Click12(object sender, RoutedEventArgs e)
        {
            _amount = ".5";
            this.Close();
        }

        private void Quick_Click34(object sender, RoutedEventArgs e)
        {
            _amount = ".75";
            this.Close();
        }
        private void Button_Back(object sender, RoutedEventArgs e)
        {
            if (tbAmount.Text != "") tbAmount.Text = tbAmount.Text.Substring(0, tbAmount.Text.Length - 1);
   
        }



    }
}
