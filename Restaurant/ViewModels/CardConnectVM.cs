using RedDot.Models;
using RedDot.Models.CardConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RedDot.ViewModels
{
    public class CardConnectVM : CCPBaseVM
    {

        private string _reason;

        public CardConnectVM(Window parent, SecurityModel security,Ticket m_ticket, string transtype, Payment payment,string reason)
             : base(parent,security, m_ticket, transtype, payment)
        {
            CreditSaleClicked = new RelayCommand(ExecuteCreditSaleClicked, param => this.CanExecuteCreditSaleClicked);
            CreditAuthClicked = new RelayCommand(ExecuteCreditAuthClicked, param => this.CanExecuteAuthCaptureClicked);
  
            FullCreditRefundClicked = new RelayCommand(ExecuteFullCreditRefundClicked, param => (transtype == "REFUND"));
           // PartialCreditRefundClicked = new RelayCommand(ExecutePartialCreditRefundClicked, param => (transtype == "REFUND"));
            ManualRefundClicked = new RelayCommand(ExecuteManualRefund, param => (transtype == "REFUND"));
            _reason = reason;

        }

    
        public void ExecuteManualRefund(object obj)
        {

        }
        public void ExecuteCreditSaleClicked(object obj)
        {
            string receiptline = "";

            try
            {
                foreach (Seat seat in CurrentTicket.Seats)
                {
                    foreach (LineItem line in seat.LineItems)
                    {
                        receiptline += line.PinPadStr + "\n";
                    }
                }
 
                receiptline += "-----------------------------\n";
                receiptline += "Tax:           " + CurrentTicket.SalesTax + "\n";
                receiptline += "Total :        " + CurrentTicket.Balance + "\n";

            
                if (CardConnectModel.ReadConfirmation(receiptline))
                {
                    bool success = CardConnectModel.ProcessCredit(CurrentTicket.SalesID, CurrentTicket.Balance, true);

                  
                }
                else
                {
                   
                    CardConnectModel.DisplayWelcome();
                   
                }



                m_parent.Close();
            }catch(Exception ex)
            {
                TouchMessageBox.Show("Credit Auth Error:" + ex.Message);
            }
       
        }

        public void ExecuteCreditAuthClicked(object obj)
        {
            bool success = CardConnectModel.ProcessCredit(CurrentTicket.SalesID, CurrentTicket.Balance, false);

            //always print customer receipt for AUTH
            if (GlobalSettings.Instance.PrintCustomerAuthReceipt)
            {
                ReceiptPrinterModel.PrintReceipt(CurrentTicket, GlobalSettings.Instance.ReceiptPrinter);
            }
                   

           

            m_parent.Close();
        }

 
        public void ExecuteFullCreditRefundClicked(object amountobj)
        {
     

            try
            {


                    Payment pay = CurrentTicket.Payments.First();

                    string resp = CardConnectModel.ProcessRefund(pay.ResponseId);

                    if (resp == "Approval")
                    {

                        //update payment
                        //   CurrentTicket.VoidPayment(m_payment.ID);


                        CurrentTicket.LoadPayment();
                        Payment payment = m_salesmodel.GetPayment(pay.ResponseId);


                        //print credit slip

                        if (payment != null)
                        {
                            ReceiptPrinterModel.AutoPrintCreditSlip(payment);

                        }


                    }


                    m_parent.Close();



            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                TouchMessageBox.Show("Refund Clicked: " + e.Message);
            }
        }
    }
}
