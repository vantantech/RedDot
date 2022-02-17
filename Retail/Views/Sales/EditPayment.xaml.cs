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
    /// Interaction logic for EditPayment.xaml
    /// </summary>
    public partial class EditPayment : Window
    {
        public DateTime StartDate;
        public string PayType;
        public decimal Amount;
        public EditPayment(DateTime date,string paytype, decimal amount)
        {
            InitializeComponent();
            this.DataContext = this;
            this.startdatepicker.SelectedDate = date;
            StartDate = date;


            List<string> m_creditcardchoices = GlobalSettings.Instance.CreditCardChoices.Split(',').ToList();

            foreach(string pay in m_creditcardchoices)
            {
               int index =  this.cmbPaytype.Items.Add(pay);
                if (paytype == pay) this.cmbPaytype.SelectedIndex= index;
            }

            Amount = amount;
            PayType = paytype;

            this.tbAmount.Text = amount.ToString();
        }

        private void startdatepicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get DatePicker reference.
            var picker = sender as DatePicker;

            // ... Get nullable DateTime from SelectedDate.
            StartDate = (DateTime)picker.SelectedDate;

        }

        private void Button_OK(object sender, RoutedEventArgs e)
        {
            try
            {
                PayType = this.cmbPaytype.SelectedItem.ToString();
                Amount = decimal.Parse(tbAmount.Text);
                this.Close();
            }catch(Exception ex)
            {
                TouchMessageBox.Show("Error:" + ex.Message);
            }

        }
    }
}
