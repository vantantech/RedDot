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
    public partial class DiscountPad : Window
    {
        private string _amount="";
        private decimal _price;

        public DiscountPad( decimal price)
        {
            InitializeComponent();

            tbPrice.Text = "Price: $" + price;
            _price = price;

        }


  

        public string Amount
        {
            get { return _amount; }

            set
            {
                _amount = value;
             
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
           
            VFD.WriteRaw("Please Enter Number",tbAmount.Text);

        }
        private void Button_OK(object sender, RoutedEventArgs e)
        {

            Validate();
        }


        private void Validate()
        {

            //remove all dashes and parentheses and emptyspaces
            _amount = tbAmount.Text.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");

            VFD.Clear();
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
            VFD.Clear();
 
            //BAck space
            //tbAmount.Text = tbAmount.Text.Substring(0, tbAmount.Text.Trim().Length - 1);
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            _amount ="";
            VFD.Clear();
            this.Close();
        }

        private void tbTextChanged(object sender, TextChangedEventArgs e)
        {
            VFD.WriteRaw("Data:",tbAmount.Text);
        }

   

        private void Button_Back(object sender, RoutedEventArgs e)
        {
            if (tbAmount.Text != "") tbAmount.Text = tbAmount.Text.Substring(0, tbAmount.Text.Length - 1);
   
        }

        private void Percent_Click(object sender, RoutedEventArgs e)
        {
            tbAmount.Text =Math.Round(decimal.Parse(tbAmount.Text) / 100 * _price,2).ToString();
        }



    }
}
