using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using RedDotBase;

namespace RedDot
{

    public class PaymentViewModel:INPCBase
    {

        public ICommand CreditSaleClicked { get; set; }
        public ICommand DebitSaleClicked { get; set; }

        public ICommand CreditDebitClicked { get; set; }
        public ICommand CardClicked { get; set; }
   
    
        public ICommand GiftCardClicked { get; set; }

        public ICommand CommandClicked { get; set; }
     

        public ICommand VoidClicked { get; set; }

        public ICommand RefundClicked { get; set; }

        public ICommand ExitClicked { get; set; }

        public ICommand LineItemClicked { get; set; }
        public ICommand AddGiftCardClicked { get; set; }

        private VFD vfd;
       
        private decimal m_giftcardbalance;
        private decimal m_quickamount;
     
       
      
      
      
        private List<string> m_creditcardchoices;

        private HeartPOS m_ccp;

        public Ticket CurrentTicket { get; set; }
       

        //private bool canExecute = true;
        private Window m_parent;

        private string m_transtype;
        private string m_responseid;
        private string m_creditcardname = "";
    


        // private bool canExecute = true;

        public PaymentViewModel(Window parent,Ticket m_ticket, HeartPOS ccp, string transtype, string responseid)
        {

            m_parent = parent;
            m_transtype = transtype;
            m_responseid = responseid;
      
            CurrentTicket = m_ticket;

            CCP = ccp;

            if(ccp != null)
             ccp.closeform = ExitForm;
          

            if (transtype.ToUpper() == "VOID") NumPadVisibility = Visibility.Hidden; else NumPadVisibility = Visibility.Visible;

          

            NotifyPropertyChanged("BalanceStr");




            vfd = new VFD(GlobalSettings.Instance.DisplayComPort);

            CardClicked = new RelayCommand(ExecuteCardClicked, param => true);
         
           CreditDebitClicked = new RelayCommand(ExecuteCreditDebitClicked, param => this.CanExecuteCreditDebitClicked);
         
           GiftCardClicked = new RelayCommand(ExecuteGiftCardClicked, param => this.CanExecuteGiftCardClicked);

            CommandClicked = new RelayCommand(ExecuteCommandClicked, param => true);
       

            CreditSaleClicked = new RelayCommand(ExecuteCreditSaleClicked, param => this.CanExecuteChargeClicked);
            DebitSaleClicked = new RelayCommand(ExecuteDebitSaleClicked, param => this.CanExecuteChargeClicked);

            VoidClicked = new RelayCommand(ExecuteVoidClicked, param => this.CanExecuteVoidClicked);
            RefundClicked = new RelayCommand(ExecuteRefundClicked, param => this.CanExecuteRefundClicked);
            LineItemClicked = new RelayCommand(ExecuteLineItemClicked, param=> true);
            AddGiftCardClicked = new RelayCommand(ExecuteAddGiftCardClicked, param => true);


            if (transtype.ToUpper() == "REFUND")
            {
                CreditCardChoices = GlobalSettings.Instance.RefundChoices.Split(',').ToList();
                //set default option for people with stand alone machines or semi-integrated
                if (GlobalSettings.Instance.CreditCardProcessor == "External")
                    m_creditcardname = CreditCardChoices[0];
                else
                    m_creditcardname = GlobalSettings.Instance.RefundChoices;

            }

            else
            {
                CreditCardChoices = GlobalSettings.Instance.CreditCardChoices.Split(',').ToList();
                //set default option for people with stand alone machines or semi-integrated
                if (GlobalSettings.Instance.CreditCardProcessor == "External")
                    m_creditcardname = CreditCardChoices[0];
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
                return String.Format("{0:0.00}", CurrentTicket.Balance);
            }
        }




        public Visibility NumPadVisibility { get; set; }

        public HeartPOS CCP
        {
            get { return m_ccp; }
            set { m_ccp = value;
                NotifyPropertyChanged("CCP");
            }
        }


    

        public List<string> CreditCardChoices
        {
            get { return m_creditcardchoices; }
            set { m_creditcardchoices = value;
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

        public bool CanExecuteChargeClicked
        {
            get
            {
                return m_ccp.SIPIsLaneOpen && (m_transtype.ToUpper() == "SALE");
            }

        }

        public bool CanExecuteVoidClicked
        {
            get
            {
                return m_ccp.SIPIsLaneOpen && (m_transtype.ToUpper() == "VOID");
            }

        }

        public bool CanExecuteRefundClicked
        {
            get
            {
                return m_ccp.SIPIsLaneOpen && (m_transtype.ToUpper() == "REFUND");
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


    

        public bool CanExecuteGiftCardClicked
        {
            get
            {

                if (CurrentTicket.SalesID == 0) return false;
                else return true;
            }

        }



        //-------------------Methods ------------------------------

      
     

        public bool AddPayment(string paytype, decimal amount,decimal netamount, string authorizeCode, string cardtype, string maskedpan, decimal tip,DateTime paymentdate,string transtype)
        {

            try
            {
                
                vfd.WriteDisplay(paytype + ":", amount, "Balance:", CurrentTicket.Balance);

                SalesModel.InsertPayment(CurrentTicket.SalesID, paytype, amount, netamount, authorizeCode,cardtype, maskedpan, tip,paymentdate,transtype);

                return true;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("AddPayment:" + e.Message);
                return false;
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
                        NumPad np = new NumPad("Enter Gift Card Charge Amount:", false);
                        Utility.OpenModal(m_parent, np);
                        if (np.Amount != "")
                            amt = decimal.Parse(np.Amount);
                        else amt = 0;


                    }


                    if (amt > 0)
                    {
                        if (!GlobalSettings.Instance.AllowCashBackGiftCard)
                        {
                            //check to see if amount is greater than balance
                            if (amt > CurrentTicket.Balance)
                            {
                                TouchMessageBox.Show("Amount greater than balance due is not allowed!!");
                                return ;
                            }
                        }

                        giftcardnumber = values[1].ToString();

                        decimal giftcardbal = GiftCardModel.GetGiftCardBalance(giftcardnumber);
                        if (giftcardbal != -99)
                        {
                            if (giftcardbal >= amt)
                            {
                                //need to check giftcard number again!!! before adding to ticket
                                AddPayment("Gift Card", amt, amt, giftcardnumber, "giftcard", "***" + giftcardnumber.Substring(giftcardnumber.Length - 4, 4), 0, DateTime.Now,"Sale");
                              
                                SalesModel.RedeemGiftCard(CurrentTicket.SalesID, "Gift Card", amt,  giftcardnumber, DateTime.Now);
                                NotifyPropertyChanged("CurrentTicket");
                                m_parent.Close();

                            }
                            else
                            {
                                TouchMessageBox.Show("Insufficient Balance!! ");
                            }


                        }
                        else
                        {
                            TouchMessageBox.Show("Invalid gift card number!! ");
                        }


                    }

                }

                if (GlobalSettings.Instance.CreditCardProcessor == "HeartSIP")
                    if (m_ccp != null) m_ccp.ExecuteCommand("Reset");

                m_parent.Close();

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Gift Card Clicked: " + e.Message);
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

                    if(m_transtype.ToUpper() == "SALE")
                    if(amt > CurrentTicket.Balance)
                    {
                        TouchMessageBox.Show("Amount is greater than balance!!!");
                        return;
                    }

             

                    AddPayment(m_creditcardname, amt,amt, "standalone","","",0, DateTime.Now,m_transtype);

                     m_parent.Close();
                   
                }
                else TouchMessageBox.Show("Please Enter Amount");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Execute Credit Debit Clicked: " + e.Message);
            }
        }


        public void ExecuteCommandClicked(object obj)
        {
            string command = "";

            if (obj != null) command = obj.ToString();

            //calls credit card processor command
            m_ccp.ExecuteCommand(command);

          
        }


        public void ExecuteCreditSaleClicked(object amount)
        {
            string temp = amount as string;
            decimal amt;
        

            try
            {


                if (temp != "")
                {
                    amt = decimal.Parse(temp);


                        HeartSIP_Charge(amt, "Credit");
                   



                }
                else TouchMessageBox.Show("Please Enter $ Amt to Process.");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Charge Clicked: " + e.Message);
            }


        }

        public void ExecuteDebitSaleClicked(object amount)
        {
            string temp = amount as string;
            decimal amt;
     

            try
            {



                if (temp != "")
                {
                    amt = decimal.Parse(temp);


                   
                        HeartSIP_Charge(amt, "Debit");
                  




                }
                else TouchMessageBox.Show("Please Enter $ Amt to Process.");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Charge Clicked: " + e.Message);
            }


        }

        public void HeartSIP_Charge(decimal amt,string creditcardname)
        {
         

            try
            {
              
            

                if (amt == 0)
                {
                   
                    if (amt > CurrentTicket.Balance)
                    {
                        TouchMessageBox.Show("Amount is greater than balance!!!");
                        return;
                    }



                    //calls credit card processor "Sale" command

                    int nextpayment = CurrentTicket.Payments.Where(x => x.CardGroup.ToUpper() == "CREDIT" || x.CardGroup.ToUpper() == "DEBIT").Count() + 1;
                    string requestid = CurrentTicket.SalesID.ToString() + nextpayment.ToString();
                    m_ccp.ExecuteSaleCommand(amt,requestid, creditcardname);




                }
                else TouchMessageBox.Show("Please Enter Amount");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Charge Clicked: " + e.Message);
            }

       
        }

        public void ExecuteVoidClicked(object obj)
        {
            if(m_responseid == "")
            {
                TouchMessageBox.Show("No Transaction #  to Void  with !!!");
                return;
            }
       

            //send the responseid ( transaction #) as transaction # and request id
            m_ccp.ExecuteVoidCommand(m_responseid, m_responseid);
        

        }

        public void ExecuteRefundClicked(object amount)
        {
            string temp = amount as string;
            decimal amt;
          
            try
            {

             

                if (temp != "")
                {
                    amt = decimal.Parse(temp);

                    decimal originalTicketAmount = CurrentTicket.Payments.Sum(x=>x.NetAmount);


                    if (amt > originalTicketAmount)
                    {
                        TouchMessageBox.Show("Amount is greater than original ticket amount!!!");
                        return;
                    }


                    int nextpayment = CurrentTicket.Payments.Where(x => x.TransType.ToUpper() == "REFUND").Count() + 50;

                    string requestid = CurrentTicket.SalesID.ToString() + nextpayment.ToString();

                    m_ccp.ExecuteRefundCommand(amt, requestid, m_creditcardname);




                }
                else TouchMessageBox.Show("Please Enter Refund Amount");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Refund Clicked: " + e.Message);
            }
        }


        public void ExecuteAddGiftCardClicked(object amount)
        {
            string temp = amount as string;
            decimal amt;

            try
            {



                if (temp != "")
                {
                    amt = decimal.Parse(temp);

                   


                    int nextpayment = CurrentTicket.Payments.Where(x => x.TransType.ToUpper() == "REFUND").Count() + 50;

                    string requestid = CurrentTicket.SalesID.ToString() + nextpayment.ToString();

                    m_ccp.ExecuteRefundCommand(amt, requestid, m_creditcardname);




                }
                else TouchMessageBox.Show("Please Enter Amount to Add to Gift Card");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Refund Clicked: " + e.Message);
            }
        }

        public void ExecuteLineItemClicked(object iditemtype)
        {

           // string temp = iditemtype as string;
          //  string[] portion = temp.Split(',');

          //  int id = 0;
           // string itemtype = portion[1]; // type = service, product , giftcard .. etc..
  

            try
            {

                // id = int.Parse(portion[0]);

                // if (id == 0) return;

                decimal total = 0;
                decimal producttotal = 0;

                decimal labortotal = 0;
                decimal taxtotal = 0;

                decimal taxabletotal = 0;

                foreach (TicketEmployee emp in CurrentTicket.TicketEmployees)
                    foreach (LineItem line in emp.LineItems)
                    {
                        if(line.Selected)
                        {
                            if (line.ItemEnumType == ProductType.Service)
                            {
                                labortotal = labortotal + line.AdjustedPrice * line.Quantity;
                            }
                            else
                            {
                                producttotal = producttotal + line.AdjustedPrice * line.Quantity;
                            }


                            //doesn't matter if its a service or product or giftcard , only cares if it's marked as taxable
                            if (line.Taxable)
                                taxabletotal = taxabletotal + line.AdjustedPrice * line.Quantity;
                        }
                           
                    }

          

                taxtotal = Math.Round(taxabletotal * GlobalSettings.Instance.SalesTaxPercent / 100, 2);

           

                total = labortotal + producttotal + taxtotal ; 



                RefundAmount = total.ToString();

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Execute Line Item Clicked: " + e.Message);
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

      
     



        public decimal CheckBalance(string giftcardnumber)
        {
            GiftCardBalance = GiftCardModel.GetGiftCardBalance(giftcardnumber);

       

            if(CurrentTicket.Balance> GiftCardBalance)
            {
                QuickAmount = GiftCardBalance;
            }else
            {
                QuickAmount = CurrentTicket.Balance;
            }


            return GiftCardBalance;
        }

   }
}
