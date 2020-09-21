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
    /// Interaction logic for CustomerPhone.xaml
    /// </summary>
    public partial class CustomerPhone : Window
    {
        private string _amount = "";
        private string _original;
        public bool FullNumberRequired = false;
        private VFD vfd;
        public CustomerPhone()
        {
            InitializeComponent();
            vfd = new VFD(GlobalSettings.Instance.DisplayComPort);
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

            if (e.Text == "\r")
            {
                Validate();
                return;
            }

            if (e.Text == "\b")
            {
                //backspace
                if (tbAmount.Text != "")
                    tbAmount.Text = tbAmount.Text.Substring(0, tbAmount.Text.Length - 1);
            }
            else
            {
                tbAmount.Text = tbAmount.Text + e.Text;
            }

            vfd.WriteRaw("Please enter number",tbAmount.Text);

        }
        private void Button_OK(object sender, RoutedEventArgs e)
        {

            Validate();
        }


        private void Validate()
        {

            //remove all dashes and parentheses and emptyspaces
            _amount = tbAmount.Text.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");

            if(FullNumberRequired)
            {
                if (_amount.Length == 10)
                {
                    this.Close();
                    vfd.Clear();
                }
                else TouchMessageBox.Show("Phone Number is INVALID!, Please enter 10 digit number");
            }else
            {
                if (_amount.Length <= 10)
                {
                    this.Close();
                    vfd.Clear();
                }
                else TouchMessageBox.Show("Phone Number is INVALID!, Please enter 10 digit number or LESS");

            }



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
            vfd.Clear();
            //BAck space
            //tbAmount.Text = tbAmount.Text.Substring(0, tbAmount.Text.Trim().Length - 1);
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            _amount = _original;
            vfd.Clear();
            this.Close();
        }

        private void tbTextChanged(object sender, TextChangedEventArgs e)
        {
            vfd.WriteRaw("Please Enter Phone",tbAmount.Text);
        }

        private void Button_Back(object sender, RoutedEventArgs e)
        {
            if (tbAmount.Text != "") tbAmount.Text = tbAmount.Text.Substring(0, tbAmount.Text.Length - 1);
        }
    }
}
