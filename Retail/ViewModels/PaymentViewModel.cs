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
       
 
        public ICommand CashClicked { get; set; }
        public ICommand CreditDebitClicked { get; set; }
        public ICommand CardClicked { get; set; }
        public ICommand RewardClicked { get; set; }
        public ICommand GiftCertificateClicked { get; set; }
        public ICommand GiftCardClicked { get; set; }

 
       
        private decimal m_giftcardbalance;
        private decimal m_quickamount;
        private string m_creditcardname="";
      
        private List<string> m_creditcardchoices;


        //private bool canExecute = true;
        private Window m_parent;
        private Ticket m_currentticket;
  
       // private bool canExecute = true;
        public PaymentViewModel(Window parent,Ticket currentticket)
        {

            m_parent = parent;
            m_currentticket = currentticket;
            NotifyPropertyChanged("BalanceStr");

            CardClicked = new RelayCommand(ExecuteCardClicked, param => true);
           CashClicked = new RelayCommand(ExecuteCashClicked, param => this.CanExecuteCashTenderClicked);
           CreditDebitClicked = new RelayCommand(ExecuteCreditDebitClicked, param => this.CanExecuteCreditDebitClicked);
           RewardClicked = new RelayCommand(ExecuteRewardClicked, param => this.CanExecuteRewardClicked);
           GiftCardClicked = new RelayCommand(ExecuteGiftCardClicked, param => this.CanExecuteGiftCardClicked);

           CreditCardChoices = GlobalSettings.Instance.CreditCardChoices.Split(',').ToList();
           if (CreditCardChoices.Count == 1)
           {
               m_creditcardname = GlobalSettings.Instance.CreditCardChoices;
               CreditCardChoices.Clear();
           }

        }

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
                return String.Format("{0:0.00}", m_currentticket.Balance);
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

        public decimal UsableBalance
        {
            get
            {
                if (m_currentticket.CustomerID > 0) return CustomerModelCore.GetUsableReward(m_currentticket.CustomerID);
                else return 0;
            }
        }

        public string MaxRewardStr
        {
            get
            {
                if (UsableBalance > m_currentticket.Balance) return BalanceStr;
                else
                    return  String.Format("{0:0.00}",UsableBalance);

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


        public bool CanExecuteCashTenderClicked
        {
            get
            {

                if (m_currentticket.SalesID == 0) return false;
               // if (_currentticket.HasBeenPaid("Cash")) return false;
                return true;
            }

        }


        public bool CanExecuteCreditDebitClicked
        {
            get
            {

                if (m_currentticket.SalesID == 0) return false;
                if (m_creditcardname == "") return false;
               // if (_currentticket.HasBeenPaid("Credit/Debit")) return false;
                return true;
            }

        }


        public bool CanExecuteRewardClicked
        {
            get
            {

                if (m_currentticket.SalesID == 0) return false;
                if (TicketCore.HasBeenPaid(m_currentticket.SalesID, "Reward")) return false; //aslo need to check if has any reward
                return true;
            }

        }

        public bool CanExecuteGiftCardClicked
        {
            get
            {

                if (m_currentticket.SalesID == 0) return false;
                if (m_giftcardbalance > 0) return true;
                else return false;
            }

        }



//-------------------Methods ------------------------------

        public bool AddPayment(string paytype, decimal amount, string authorizeCode)
        {

            try
            {
                decimal netamount;


                if (m_currentticket.IsRefund)
                {
                    if (amount > 0) amount = amount * (-1);//make sure amount is negative for refund

                    if (Math.Abs(amount) > Math.Abs(m_currentticket.Balance))
                    {
                        netamount = m_currentticket.Balance;
                        amount = m_currentticket.Balance; //can not refund higher than the balance.
                    }else
                    {
                        netamount = amount;
                    }
                   
                  
                }else
                {
                    //if change is given to customers, then record the balance, not the tender 
                    if (amount > m_currentticket.Balance) netamount = m_currentticket.Balance;
                    else netamount = amount;

                }


                VFD.WriteDisplay(paytype + ":", amount, "Balance:", m_currentticket.Balance);


                TicketCore.InsertPayment(m_currentticket.SalesID, paytype, amount, netamount, authorizeCode);

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("AddPayment:" + e.Message);
                return false;
            }
        }


        public void ExecuteCashClicked(object amount)
        {
         
            string temp = amount as string;
            decimal amt;

            try
            {
                if (temp != "")
                {

                    amt = decimal.Parse(temp);

                  
                    AddPayment("Cash", amt, "");
                    NotifyPropertyChanged("CurrentTicket");
                     m_parent.Close();
                    
                }
                else MessageBox.Show("Please Enter Amount");

            }
            catch (Exception e)
            {

                MessageBox.Show("Error Cash Clicked: " + e.Message);
            }
        }

        public void ExecuteRewardClicked(object amount)
        {

            string temp;
            decimal amt;

            try
            {
                temp = amount.ToString();
                if (temp != "")
                {

                    amt = decimal.Parse(temp);

                    //validate amount before adding to ticket
                    if(amt > UsableBalance)
                    {
                        MessageBox.Show("Not Enough Reward Credit!");
                    }
                    else
                    {
                        if (amt > m_currentticket.Balance)
                        {
                            MessageBox.Show("Reward is greater than balance!!");
                        }
                        else
                        {
                            AddPayment("Reward", amt, "");
                            NotifyPropertyChanged("CurrentTicket");
                            m_parent.Close();
                           
                        }
    
                    }


                }
                else MessageBox.Show("Please Enter Amount");

            }
            catch (Exception e)
            {

                MessageBox.Show("Payment -  Reward Clicked: " + e.Message);
            }
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
                    if (amt > m_currentticket.Balance)
                    {
                        MessageBox.Show("Amount is greater than balance!!!");
                        return;
                    }
                    
                  AddPayment(m_creditcardname, amt, "external");

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

                    if(amt == -99)
                    {
                        NumPad np = new NumPad("Enter Gift Card Charge Amount:",false);
                        Utility.OpenModal(m_parent, np);
                        if (np.Amount != "")
                            amt = decimal.Parse(np.Amount);
                        else amt = 0;


                    }


                    if(amt > 0)
                    {
                        giftcardnumber = values[1].ToString();

                        decimal giftcardbal = GiftCardModel.GetGiftCardBalance(giftcardnumber);
                        if(giftcardbal != -99)
                        {
                            if(giftcardbal >= amt)
                            {
                                //need to check giftcard number again!!! before adding to ticket
                                AddPayment("Gift Card", amt, giftcardnumber);
                                NotifyPropertyChanged("CurrentTicket");
                                 m_parent.Close();
 
                            }
                            else
                            {
                                MessageBox.Show("Insufficient Balance!! .. try again");
                            }


                        }else
                        {
                            MessageBox.Show("Invalid gift card number!! .. try again");
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
            if (TicketCore.GiftCardOnPayment(m_currentticket.SalesID, giftcardnumber))
            {
                GiftCardBalance = -100;  //-100 is code for "already used on ticket"
            }

            if (m_currentticket.Balance > GiftCardBalance)
            {
                QuickAmount = GiftCardBalance;
            }else
            {
                QuickAmount = m_currentticket.Balance;
            }


            return GiftCardBalance;
        }

   }
}
