using System;
using System.Collections.Generic;
using System.Data;
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
        private string _amount = "";
        private decimal _price;
        PromotionsModel m_promomodel;
        public Promotion ReturnPromotion;
        private int m_productid;

        public DiscountPad(int productid,decimal price)
        {
            InitializeComponent();

            m_promomodel = new PromotionsModel();
            m_productid = productid;

            tbPrice.Text = "Price: $" + price;
            _price = price;
            this.Left = 0;
            this.Top = 0;
            PromotionList = m_promomodel.GetPromotionsToday(m_productid,"DISCOUNT");
           
            this.DataContext = this;
        }

        DataTable m_promotions;
        public DataTable PromotionList
        {
            get { return m_promotions; }

            set
            {
                m_promotions = value;
             
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

   

        }
        private void Button_OK(object sender, RoutedEventArgs e)
        {

            Validate();
        }


        private void Button_Coupon(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            string id = button.Tag.ToString();
           DataRow promo = m_promomodel.GetPromotionbyID(int.Parse(id));

           ReturnPromotion = new Promotion(promo, false);
            this.Close();
            
        }

        private void Validate()
        {

            //remove all dashes and parentheses and emptyspaces
            _amount = tbAmount.Text.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");

            ReturnPromotion = new Promotion();
            ReturnPromotion.Description = "Manual";
            if(_amount != "")
                ReturnPromotion.DiscountAmount = decimal.Parse(_amount);

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
       

            //BAck space
            //tbAmount.Text = tbAmount.Text.Substring(0, tbAmount.Text.Trim().Length - 1);
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            _amount = "";

            this.Close();
        }

        private void Button_Remove(object sender, RoutedEventArgs e)
        {
            _amount = "0";
            ReturnPromotion = new Promotion();
            ReturnPromotion.Description = "Remove";
            this.Close();
        }




        private void Button_Back(object sender, RoutedEventArgs e)
        {
            if (tbAmount.Text != "") tbAmount.Text = tbAmount.Text.Substring(0, tbAmount.Text.Length - 1);

        }

        private void Percent_Click(object sender, RoutedEventArgs e)
        {


            Button but = (Button)sender;

            switch (but.Content.ToString())
            {
                case "%":
                    if (tbAmount.Text != "")
                    {
                        tbAmount.Text = Math.Round(decimal.Parse(tbAmount.Text) / 100 * _price, 2).ToString();
                        Validate();
                    }
                    break;
                case "5%":
                    tbAmount.Text = Math.Round(0.05m * _price, 2).ToString();
                    Validate();
                    break;

                case "10%":
                    tbAmount.Text = Math.Round(0.10m * _price, 2).ToString();
                    Validate();
                    break;
                case "15%":
                    tbAmount.Text = Math.Round(0.15m * _price, 2).ToString();
                    Validate();
                    break;
                case "20%":
                    tbAmount.Text = Math.Round(0.20m * _price, 2).ToString();
                    Validate();
                    break;
            }






        }



    }
}
