using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class CCPBaseVM:INPCBase
    {



        public ICommand CreditAuthClicked { get; set; }
        public ICommand CreditSaleClicked { get; set; }
        public ICommand DebitSaleClicked { get; set; }
        public ICommand LineItemClicked { get; set; }

        public ICommand FullCreditRefundClicked { get; set; }
        public ICommand FullStoreCreditClicked { get; set; }
        public ICommand PartialStoreCreditClicked { get; set; }
        public ICommand PartialCreditRefundClicked { get; set; }
        public ICommand DebitRefundClicked { get; set; }
        public ICommand VoidClicked { get; set; }
        public ICommand ReversalClicked { get; set; }
        public ICommand CommandClicked { get; set; }
        public ICommand ManualRefundClicked { get; set; }

        protected bool enabledebitsale = GlobalSettings.Instance.EnableDebitSale;
        protected bool enablecreditsale = GlobalSettings.Instance.EnableCreditSale;
        protected bool enableauthcapture = GlobalSettings.Instance.EnableAuthCapture;
        protected bool allowduplicates = GlobalSettings.Instance.AllowDuplicates;
        protected bool signatureonscreen = GlobalSettings.Instance.SignatureOnScreen;

        protected string m_transtype = "";
        protected Payment m_payment;



        protected SalesModel m_salesmodel { get; set; }
        
        protected VFD vfd;

        protected Window m_parent;
        protected SecurityModel m_security;
        protected CCPBaseVM(Window parent,SecurityModel security, Ticket m_ticket, string transtype, Payment payment)
        {

            CurrentTicket = m_ticket;
            m_parent = parent;
            m_transtype = transtype;
            m_payment = payment;
            m_security = security;

            switch(transtype.ToUpper())
            {
                case "REFUND":
                    if (m_ticket.Balance < 0) NumPadVisibility = Visibility.Hidden; else NumPadVisibility = Visibility.Visible;

                    break;
                case "VOID":
                case "REVERSAL":
                    NumPadVisibility = Visibility.Hidden;
                    break;
                case "PREAUTH":
                    if (GlobalSettings.Instance.AllowServerChangeOpenTabAmount)
                    {
                        NumPadVisibility = Visibility.Visible;
                    }
                    else
                    {
                        NumPadVisibility = Visibility.Hidden;
                    }
                    break;
                default:
                    NumPadVisibility = Visibility.Visible;
                    break;

            }
     
                



            m_salesmodel = new SalesModel(m_security);
            vfd = new VFD(GlobalSettings.Instance.DisplayComPort);

            // NotifyPropertyChanged("BalanceStr");

            FullStoreCreditClicked = new RelayCommand(ExecuteFullStoreCredit, param => true);
            PartialStoreCreditClicked = new RelayCommand(ExecutePartialStoreCredit, param => true);

            LineItemClicked = new RelayCommand(ExecuteLineItemClicked, param => true);

         
        }


        public bool TipSelected { get; set; }
        public Ticket CurrentTicket { get; set; }
        public string BalanceStr
        {
            get
            {
                if (m_transtype == "PREAUTH")
                    return String.Format("{0:0.00}", GlobalSettings.Instance.OpenTabPreAuthAmount);
                else
                    return String.Format("{0:0.00}", CurrentTicket.Balance);
            }
        }

        public Visibility NumPadVisibility { get; set; }

        private TicketTotals m_refundamount;
        public TicketTotals RefundAmount
        {
            get { return m_refundamount; }
            set
            {
                m_refundamount = value;
                NotifyPropertyChanged("RefundAmount");
            }
        }

        public bool CanExecuteChargeClicked
        {
            get
            {
                return (m_transtype.ToUpper() == "SALE");
            }

        }

        public bool CanExecuteDebitSaleClicked
        {
            get
            {
                if (CanExecuteChargeClicked)
                {
                    return enabledebitsale;
                }
                else
                    return false;
            }

        }




        public bool CanExecuteCreditSaleClicked
        {
            get
            {
                if (CanExecuteChargeClicked && m_transtype != "PREAUTH")
                {
                    return enablecreditsale;
                }
                else
                    return false;
            }

        }


        public bool CanExecuteAuthCaptureClicked
        {
            get
            {
                if (CanExecuteChargeClicked || m_transtype == "PREAUTH")
                {
                    return enableauthcapture;
                }
                else
                    return false;
            }

        }



        public bool CanExecuteVoidClicked
        {
            get
            {
                return (m_transtype.ToUpper() == "VOID");
            }

        }

        public bool CanExecuteReversalClicked
        {
            get
            {
                return ( m_transtype.ToUpper() == "REVERSAL");
            }

        }

        public void ExecuteFullStoreCredit(object obj)
        {
            try
            {

                if (CurrentTicket == null) return;

                if (CurrentTicket.CurrentCustomer == null)
                {
                    TouchMessageBox.Show("Please add customer to ticket first before giving store credit.");
                    return;
                }

                if (CurrentTicket.Balance != 0)
                {
                    TouchMessageBox.Show("Balance not ZERO .. does not qualify for Full Refund.");
                    return;
                }

                int refundedcount = CurrentTicket.Payments.Where(x => x.CardGroup == "CREDIT" && x.TransType == "REFUND").Count();
                if (refundedcount > 0)
                {
                    TouchMessageBox.Show("Already has a refund .. does not qualify for Full Refund.");
                    return;
                }

      

                decimal originalCreditAmount = CurrentTicket.Payments.Where(x => x.CardGroup == "CREDIT").Sum(x => x.NetAmount + x.TipAmount); //do not use the charged amount because there might be cash back and exclude refunds ( negative)
                decimal originalCashAmount = CurrentTicket.Payments.Where(x => x.CardGroup == "CASH").Sum(x => x.NetAmount);

                TextPad tp = new TextPad("Enter Credit Reason", "");
                Utility.OpenModal(m_parent, tp);

                if (tp.ReturnText == "") return;


                logger.Info("COMMAND:Store Credit Refund , Amount=" + originalCreditAmount + "  Reason=" + tp.ReturnText);
                CurrentTicket.CurrentCustomer.AddCredit(originalCreditAmount, tp.ReturnText);


                //void items
                logger.Info("Void all line items on ticket:" + CurrentTicket.SalesID);
                CurrentTicket.VoidAllLineItem(tp.ReturnText);

                if (originalCashAmount > 0)
                {
                    logger.Info("Void all cash payment on ticket:" + CurrentTicket.SalesID);
                    CurrentTicket.VoidAllCashPayment(tp.ReturnText);
                    TouchMessageBox.Show("Refund Cash To Customer : " + originalCashAmount);
                }

                bool result = PaymentModel.InsertPayment(CurrentTicket.SalesID, "STORECREDIT", originalCreditAmount, originalCreditAmount, "", "", "", 0, DateTime.Now, "REFUND");

                if (result == false)
                {
                    TouchMessageBox.Show("Payment record insert failed.");
                    return;
                }


                m_parent.Close();


            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Store Credit Full Refund Clicked: " + e.Message);
            }
        }


        public void ExecutePartialStoreCredit(object obj)
        {
            try
            {
                if (CurrentTicket == null) return;

                if (RefundAmount.TotalWithTip == 0)
                {
                    TouchMessageBox.Show("Select items to do partial refund");
                    return;
                }

                if (CurrentTicket.CurrentCustomer == null)
                {
                    TouchMessageBox.Show("Please add customer to ticket first before giving store credit.");
                    return;
                }


                decimal originalTicketAmount = CurrentTicket.Payments.Sum(x => x.NetAmount);


                if (RefundAmount.TotalWithTip > originalTicketAmount)
                {
                    TouchMessageBox.Show("Amount selected is greater than credit payment amount!!!");
                    return;
                }

                decimal total = RefundAmount.TotalWithTip;

                TextPad tp = new TextPad("Enter Credit Reason", "");
                Utility.OpenModal(m_parent, tp);

                if (tp.ReturnText == "") return;

                logger.Info("COMMAND:Store Credit Refund, Amount=" + total);

                CurrentTicket.CurrentCustomer.AddCredit(total, tp.ReturnText);


                CurrentTicket.VoidSelectedItem(tp.ReturnText);

                bool result = PaymentModel.InsertPayment(CurrentTicket.SalesID, "STORECREDIT", total, total, "", "", "", 0, DateTime.Now, "REFUND");

                if (result == false)
                {
                    TouchMessageBox.Show("Payment record insert failed.");
                    return;
                }



                m_parent.Close();

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Store Credit Partial Refund Clicked: " + e.Message);
            }
        }


        public void ExecuteLineItemClicked(object iditemtype)
        {
            try
            {
                RefundAmount = CurrentTicket.GetSelectedItemTotal(TipSelected);
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Execute Line Item Clicked: " + e.Message);
            }
        }


        protected void LinkCreditCardToCustomer(Payment payment)
        {
            if (CurrentTicket.CurrentCustomer != null)
            {
                //customer already linked .. add last 4 digit if not already there
                if (CurrentTicket.CurrentCustomer.CreditCardNumber == "")
                {
                    CurrentTicket.CurrentCustomer.CreditCardNumber = payment.MaskedPAN.Substring(payment.MaskedPAN.Length - 4);
                }
            }
            else
            {
                CustomerModelCore cust = new CustomerModelCore();
                //try to lookup and link customer
                DataTable dt;
                int customerid;

                dt = cust.GetCustomerbyCreditCard(payment.MaskedPAN.Substring(payment.MaskedPAN.Length - 4));
                if (dt.Rows.Count == 1)
                {
                    customerid = int.Parse(dt.Rows[0]["id"].ToString());
                    CurrentTicket.UpdateCustomerID(customerid);
                }
            }
        }
    }
}
