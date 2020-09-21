using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RedDot
{

    public class PaymentViewModel:INPCBase
    {
       
 
      /// <summary>
      /// In house payments
      /// </summary>
        public ICommand CreditDebitClicked { get; set; }
        public ICommand CardClicked { get; set; }
       // public ICommand RewardClicked { get; set; }
       // public ICommand GiftCertificateClicked { get; set; }
        public ICommand GiftCardClicked { get; set; }

        public ICommand LineItemClicked { get; set; }

        private decimal m_giftcardbalance;
        private decimal m_quickamount;
        private string m_creditcardname="";
   
 


        private List<string> m_creditcardchoices;

      

        public Ticket CurrentTicket { get; set; }


        //private bool canExecute = true;
        private Window m_parent;

        private string m_transtype;
        private string m_responseid;

        PaymentModel m_paymentmodel;
        SecurityModel m_security;

        // private bool canExecute = true;
        public PaymentViewModel(Window parent,SecurityModel security, Ticket m_ticket,  string transtype, string responseid)
        {
            m_parent = parent;
            m_transtype = transtype;
            m_responseid = responseid;
            m_security = security;

            CurrentTicket = m_ticket;
            m_paymentmodel = new PaymentModel(m_security);




            if (transtype.ToUpper() == "VOID") NumPadVisibility = Visibility.Hidden; else NumPadVisibility = Visibility.Visible;



            NotifyPropertyChanged("BalanceStr");


            CardClicked = new RelayCommand(ExecuteCardClicked, param => true);
       
           CreditDebitClicked = new RelayCommand(ExecuteCreditDebitClicked, param => this.CanExecuteCreditDebitClicked);
        //   RewardClicked = new RelayCommand(ExecuteRewardClicked, param => this.CanExecuteRewardClicked);
           GiftCardClicked = new RelayCommand(ExecuteGiftCardClicked, param => this.CanExecuteGiftCardClicked);

          //  LineItemClicked = new RelayCommand(ExecuteLineItemClicked, param => true);


            CreditCardChoices = GlobalSettings.Instance.ExternalPayChoices.Split(',').ToList();

           if (CreditCardChoices.Count == 1)
           {
               m_creditcardname = GlobalSettings.Instance.ExternalPayChoices;
               CreditCardChoices.Clear();
           }

        }





    

        public Visibility NumPadVisibility { get; set; }

        public List<string> CreditCardChoices
        {
            get { return m_creditcardchoices; }
            set { m_creditcardchoices = value;
            NotifyPropertyChanged("CreditCardChoices");
            }
        }

        public string BalanceStr
        {
            get
            {
                return String.Format("{0:0.00}", CurrentTicket.Balance);
            }
        }

        public decimal GiftCardBalance
        {
            get
            {
                return m_giftcardbalance;
            }

            set
            {
                m_giftcardbalance = value;
                NotifyPropertyChanged("GiftCardBalance");
            }
        }
        public decimal QuickAmount
        {
            get
            {
                return m_quickamount;
            }

            set
            {
                m_quickamount = value;
                NotifyPropertyChanged("QuickAmount");
            }
        }







        //------------------------------------------------------------------------------------------
        //  ____        _   _                ____  _           _     _      
        // | __ ) _   _| |_| |_ ___  _ __   |  _ \(_)___  __ _| |__ | | ___ 
        // |  _ \| | | | __| __/ _ \| '_ \  | | | | / __|/ _` | '_ \| |/ _ \
        // | |_) | |_| | |_| || (_) | | | | | |_| | \__ \ (_| | |_) | |  __/
        // |____/ \__,_|\__|\__\___/|_| |_| |____/|_|___/\__,_|_.__/|_|\___|                                                                 
        //
        //------------------------------------------------------------------------------------------ 




        public bool CanExecuteChargeClicked
        {
            get
            {
                return  (m_transtype.ToUpper() == "SALE");
            }

        }

        public bool CanExecuteVoidClicked
        {
            get
            {
                return  (m_transtype.ToUpper() == "VOID");
            }

        }


        public bool CanExecuteCashTenderClicked
        {
            get
            {

                if (CurrentTicket.SalesID == 0) return false;
               // if (_currentticket.HasBeenPaid("Cash")) return false;
                return true;
            }

        }


        public bool CanExecuteCreditDebitClicked
        {
            get
            {

                if (CurrentTicket.SalesID == 0) return false;
                if (m_creditcardname == "") return false;
               // if (_currentticket.HasBeenPaid("Credit/Debit")) return false;
                return true;
            }

        }


        public bool CanExecuteRewardClicked
        {
            get
            {

                if (CurrentTicket.SalesID == 0) return false;
                if (SalesModel.HasBeenPaid(CurrentTicket.SalesID, "Reward")) return false; //aslo need to check if has any reward
                return true;
            }

        }

        public bool CanExecuteGiftCardClicked
        {
            get
            {

                if (CurrentTicket.SalesID == 0) return false;
                if (m_giftcardbalance > 0) return true;
                else return false;
            }

        }



        //-------------------Methods ------------------------------



        public void ExitForm()
        {
            m_parent.Close();
        }






        public bool AddPayment(string paytype, decimal amount, decimal netamount, string authorizeCode, string cardtype, string maskedpan, decimal tip, DateTime paymentdate, string transtype)
        {

            try
            {



                PaymentModel.InsertPayment(CurrentTicket.SalesID, paytype, amount, netamount, authorizeCode, cardtype, maskedpan, tip, paymentdate, transtype);
                CurrentTicket.Reload();
                GlobalSettings.CustomerDisplay.WriteDisplay(paytype + ":", amount, "Balance:", CurrentTicket.Balance);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("AddPayment:" + e.Message);
                return false;
            }
        }







        //-----------------------------------------------------CREDIT CARD Processing ------------------------------------------

        public void ExecuteCommandClicked(object obj)
        {
            string command = "";

            if (obj != null) command = obj.ToString();

          


        }





        public void ExecuteCardClicked(object cardname)
        {
            string temp = cardname as string;
            m_creditcardname = temp;

        }
        public void ExecuteCreditDebitClicked(object amount)
        {
            string temp = amount as string;
            decimal amt;

            try
            {
                if (temp != "")
                {
                    amt = decimal.Parse(temp);
                    if (amt > CurrentTicket.Balance)
                    {
                        MessageBox.Show("Amount is greater than balance!!!");
                        return;
                    }


                    CurrentTicket.AddPayment(m_creditcardname, amt, "External Pay", "SALE");
                   // AddPayment(m_creditcardname, amt, amt, "External Pay", "", "", 0, DateTime.Now, "SALE");

                    m_parent.Close();

                }
                else MessageBox.Show("Please Enter Amount");

            }
            catch (Exception e)
            {

                MessageBox.Show("Execute Credit Debit Clicked: " + e.Message);
            }
        }


        public void ExecuteGiftCardClicked(object parameters)
        {
            var values = (object[])parameters;

            decimal amt;
            string giftcardnumber;

            try
            {
                if (values != null)
                {



                    amt = decimal.Parse(values[0].ToString());

                    if (amt == -99)
                    {
                        NumPad np = new NumPad("Enter Gift Card Charge Amount:", false, false);
                        Utility.OpenModal(m_parent, np);
                        if (np.Amount != "")
                            amt = decimal.Parse(np.Amount);
                        else amt = 0;


                    }


                    if (amt > 0)
                    {
                        giftcardnumber = values[1].ToString();

                        decimal giftcardbal = GiftCardModel.GetGiftCardBalance(giftcardnumber);
                        if (giftcardbal != -99)
                        {
                            if (giftcardbal >= amt)
                            {
                                //need to check giftcard number again!!! before adding to ticket
                                AddPayment("Gift Card", amt, amt, giftcardnumber, "giftcard", "***" + giftcardnumber.Substring(giftcardnumber.Length - 4, 4), 0, DateTime.Now, "SALE");
                                SalesModel.RedeemGiftCard(CurrentTicket.SalesID, "Gift Card", amt, giftcardnumber, DateTime.Now);

                                NotifyPropertyChanged("CurrentTicket");
                                m_parent.Close();

                            }
                            else
                            {
                                MessageBox.Show("Insufficient Balance!! ");
                            }


                        }
                        else
                        {
                            MessageBox.Show("Invalid gift card number!! ");
                        }


                    }

                }

               

                m_parent.Close();

            }
            catch (Exception e)
            {

                MessageBox.Show("Gift Card Clicked: " + e.Message);
            }
        }

        public decimal CheckBalance(string giftcardnumber)
        {
            GiftCardBalance = GiftCardModel.GetGiftCardBalance(giftcardnumber);

            //gift card is already on payment list, then return ZERO balance since it can not be used
            if(SalesModel.GiftCardOnPayment(CurrentTicket.SalesID,giftcardnumber))
            {
                GiftCardBalance = -100;  //-100 is code for "already used on ticket"
            }

            if(CurrentTicket.Balance > GiftCardBalance)
            {
                QuickAmount = GiftCardBalance;
            }else
            {
                QuickAmount = CurrentTicket.Balance;
            }


            return GiftCardBalance;
        }

        private string m_refundamount;

        public string RefundAmount
        {
            get { return m_refundamount; }
            set
            {
                m_refundamount = value;
                NotifyPropertyChanged("RefundAmount");
            }
        }




    }
}
