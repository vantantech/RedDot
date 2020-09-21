using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace RedDot
{

    public class ExternalCreditCardVM : INPCBase
    {

     

        public ICommand CreditDebitClicked { get; set; }
        public ICommand CardClicked { get; set; }


     


        public ICommand VoidClicked { get; set; }

        public ICommand RefundClicked { get; set; }

        public ICommand ExitClicked { get; set; }

        public ICommand LineItemClicked { get; set; }
      

        private VFD vfd;

        private decimal m_giftcardbalance;
        private decimal m_quickamount;





        private List<string> m_creditcardchoices;

        public Ticket CurrentTicket { get; set; }


        //private bool canExecute = true;
        private Window m_parent;

        private string m_transtype;
      
        private string m_creditcardname = "";



        public ExternalCreditCardVM(Window parent, Ticket m_ticket,  string transtype)
        {

            m_parent = parent;
            m_transtype = transtype;
        

            CurrentTicket = m_ticket;

         


            if (transtype.ToUpper() == "VOID") NumPadVisibility = Visibility.Hidden; else NumPadVisibility = Visibility.Visible;



            NotifyPropertyChanged("BalanceStr");




            vfd = new VFD(GlobalSettings.Instance.DisplayComPort);

            CardClicked = new RelayCommand(ExecuteCardClicked, param => true);

            CreditDebitClicked = new RelayCommand(ExecuteCreditDebitClicked, param => this.CanExecuteCreditDebitClicked);

    

      


            if (transtype.ToUpper() == "REFUND")
            {
                CreditCardChoices = GlobalSettings.Instance.RefundChoices.Split(',').ToList();
                //set default option for people with stand alone machines or semi-integrated
                if (GlobalSettings.Instance.CreditCardProcessor == "External")
                    m_creditcardname = CreditCardChoices[0];//pics default if user doesn't pick
                else
                    m_creditcardname = GlobalSettings.Instance.RefundChoices;

            }

            else
            {
                CreditCardChoices = GlobalSettings.Instance.CreditCardChoices.Split(',').ToList();
                //set default option for people with stand alone machines or semi-integrated
                if (GlobalSettings.Instance.CreditCardProcessor == "External")
                    m_creditcardname = CreditCardChoices[0];//pics default if user doesn't pick
                else
                    m_creditcardname = GlobalSettings.Instance.CreditCardChoices;

            }



            if (CreditCardChoices.Count == 1)
            {
                m_creditcardname = GlobalSettings.Instance.CreditCardChoices;
                CreditCardChoices.Clear();
            }




        }

        public string BalanceStr
        {
            get
            {
                return String.Format("{0:0.00}",Math.Abs( CurrentTicket.Balance));
            }
        }




        public Visibility NumPadVisibility { get; set; }

   




        public List<string> CreditCardChoices
        {
            get { return m_creditcardchoices; }
            set
            {
                m_creditcardchoices = value;
                NotifyPropertyChanged("CreditCardChoices");
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







        //-------------------Methods ------------------------------




        public bool AddPayment(string paytype, decimal amount, decimal netamount, string authorizeCode, string cardtype, string maskedpan, decimal tip, DateTime paymentdate, string transtype)
        {

            try
            {

               

                CurrentTicket.InsertPayment( paytype, amount, netamount, authorizeCode, cardtype, maskedpan, tip, paymentdate, transtype);
                CurrentTicket.LoadPayment();

                vfd.WriteDisplay(paytype + ":", amount, "Balance:", CurrentTicket.Balance);

                return true;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("AddPayment:" + e.Message);
                return false;
            }
        }




   
        public void ExecuteCardClicked(object cardname)
        {
            string temp = cardname as string;
            m_creditcardname = temp;




        }

        //this is for external users
        public void ExecuteCreditDebitClicked(object amount)
        {
            string temp = amount as string;
            decimal amt;

            try
            {
                if (temp != "")
                {
                    amt = decimal.Parse(temp);

                    if (m_transtype.ToUpper() == "SALE")
                        if (amt > CurrentTicket.Balance)
                        {
                            TouchMessageBox.Show("Amount is greater than balance!!!");
                            return;
                        }



                    AddPayment(m_creditcardname, amt, amt, "n/a", "", "", 0, DateTime.Now, m_transtype);

                    m_parent.Close();

                }
                else TouchMessageBox.Show("Please Enter Amount");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Execute Credit Debit Clicked: " + e.Message);
            }
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




        public void ExitForm()
        {
            m_parent.Close();
        }


    }
}
