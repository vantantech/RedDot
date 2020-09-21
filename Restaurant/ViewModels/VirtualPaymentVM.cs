using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TriPOS.ResponseModels;
using TriPOS.SignatureFiles;

namespace RedDot
{
    public class VirtualPaymentVM : CCPBaseVM
    {
        private VirtualPaymentModel virtualpaymentmodel = new VirtualPaymentModel();
        private string _reason;

        public VirtualPaymentVM(Window parent, SecurityModel security, Ticket m_ticket, string transtype, Payment payment, string reason)
            : base(parent, security, m_ticket, transtype, payment)
        {
            _reason = reason;

            CreditSaleClicked = new RelayCommand(ExecuteCreditSaleClicked, param => this.CanExecuteCreditSaleClicked);
            CreditAuthClicked = new RelayCommand(ExecuteCreditAuthClicked, param => this.CanExecuteAuthCaptureClicked);
            VoidClicked = new RelayCommand(ExecuteVoidClicked, param => this.CanExecuteVoidClicked);
            FullCreditRefundClicked = new RelayCommand(ExecuteFullCreditRefundClicked, param => true);
  
            PartialCreditRefundClicked = new RelayCommand(ExecutePartialCreditRefundClicked, param => true);
            ManualRefundClicked = new RelayCommand(ExecuteManualRefund, param => true);

            m_salesmodel = new SalesModel(m_security);

        }



        public void ExecuteCreditSaleClicked(object amount)
        {
            string temp = amount as string;
            decimal amt;


            if (temp != "")
            {
                amt = decimal.Parse(temp);
                if (amt > CurrentTicket.Balance)
                {
                    TouchMessageBox.Show("Amount is greater than balance!!!");
                    return;
                }

                SaleResponse2 resp = virtualpaymentmodel.ExecuteCreditSale(CurrentTicket.SalesID, amt);


                if (resp != null)
                {
                    switch (resp.StatusCode.ToUpper())
                    {

                        case "APPROVED":
                            bool result = AddCreditPayment("SALE", amt, resp, DateTime.Now, "");
                            if (result == false)
                            {
                                TouchMessageBox.Show("Payment record insert failed.");
                                return;
                            }


                            Payment payment = m_salesmodel.GetPayment(resp.TransactionId);
                            //check to see if Ticket has customer linked.
                            LinkCreditCardToCustomer(payment);


                            //print credit slip
                      
                                ReceiptPrinterModel.AutoPrintCreditSlip(payment);


                            m_parent.Close();

                            break;


                        case "CANCELLED":
                            TouchMessageBox.Show("Transaction Cancelled.");
                            break;

                        case "NONE":
                            TouchMessageBox.Show("No Response from PIN PAD, Please try again");
                            break;

                        default:

                            TouchMessageBox.Show(resp.StatusCode);
                            break;

                    }
                }
                else
                {
                    logger.Error("No Response from Credit Card Pin Pad  ");
                    TouchMessageBox.Show("No Response from Credit Card Pin Pad  ");

                }
            }
        }


        public void ExecuteCreditAuthClicked(object amount)
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
                        TouchMessageBox.Show("Amount is greater than balance!!!");
                        return;
                    }

                    AuthorizationResponse resp = virtualpaymentmodel.ExecuteCreditAuthorize(CurrentTicket.SalesID, amt);
                    if (resp != null)
                    {
                        switch (resp.StatusCode.ToUpper())
                        {
                            case "APPROVED":
                                bool result = AddCreditPayment(m_transtype == "SALE" ? "AUTH" : "PREAUTH", amt, TriPOSModel.ConvertToSale(resp), DateTime.Now, "");
                                if (result == false)
                                {
                                    TouchMessageBox.Show("Payment record insert failed.");
                                    return;
                                }

                                Payment payment = m_salesmodel.GetPayment(resp.TransactionId);
                                //check to see if Ticket has customer linked.
                                LinkCreditCardToCustomer(payment);

                                //print credit slip
                            
                               ReceiptPrinterModel.AutoPrintCreditSlip(payment);



                                m_parent.Close();
                                break;

                            case "NONE":
                                TouchMessageBox.Show("No Response from PIN PAD, Please try again");
                                break;

                            case "CANCELLED":
                                TouchMessageBox.Show("Transaction Cancelled.");
                                break;

                            default:

                                TouchMessageBox.Show(resp.StatusCode);
                                break;

                        }
                    }
                    else
                    {
                        logger.Error(resp.Errors.ToString());
                        TouchMessageBox.Show("SALE TRANSACTION FAILED !!!! ERROR:  " + resp.Errors.ToString());

                    }

                }


                else TouchMessageBox.Show("Please Enter $ Amt to Process.");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Charge Clicked: " + e.Message);
            }


        }

        public void ExecuteVoidClicked(object obj)
        {
            if (m_payment == null)
            {
                TouchMessageBox.Show("No Transaction #  to Void  with !!!");
                return;
            }

 
            string status = "";
            if (m_payment.TransType.ToUpper() == "AUTH")
            {
                //AUTH => REVERSAL   , SALE => REVERSAL  .. reversal can be used for both .. weird
                ReversalResponse resp = virtualpaymentmodel.ExecuteReversal(m_payment.ResponseId, m_payment.CardGroup, m_payment.Amount);
                status = resp.StatusCode;
            }
            else
            {
                //SALE => VOID
                VoidResponse resp = virtualpaymentmodel.ExecuteVoid(m_payment.ResponseId);
                status = resp.StatusCode;
            }



            if (status == "Approved")
            {
                //update payment
                CurrentTicket.VoidPayment(m_payment.ID, _reason);


          
                    Payment payment = m_salesmodel.GetPayment(m_payment.ResponseId);
                    //print debit slip
                    ReceiptPrinterModel.AutoPrintCreditSlip(payment);
            
         


                m_parent.Close();
            }
            else
            {
                logger.Error("VOID TRANSACTION FAILED !!!! ERROR:  " + status);
                TouchMessageBox.Show("VOID TRANSACTION FAILED !!!! ERROR:  " + status);
            }
        }

        public void ExecuteFullCreditRefundClicked(object amount)
        {



            try
            {

                if (CurrentTicket == null) return;

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
                TextPad tp = new TextPad("Enter Refund Reason", "");
                Utility.OpenModal(m_parent, tp);

                if (tp.ReturnText == "") return;

                decimal originalCreditAmount = CurrentTicket.Payments.Where(x => x.CardGroup == "CREDIT").Sum(x => x.NetAmount); //do not use the charged amount because there might be cash back and exclude refunds ( negative)
                decimal originalTipAmount = CurrentTicket.Payments.Where(x => x.CardGroup == "CREDIT").Sum(x => x.TipAmount); //do not use the charged amount because there might be cash back and exclude refunds ( negative)

                decimal originalCashAmount = CurrentTicket.Payments.Where(x => x.CardGroup == "CASH").Sum(x => x.NetAmount);

                logger.Info("COMMAND:Credit Refund , Amount=" + originalCreditAmount);

                //credit card processor specific code

                RefundResponse resp = virtualpaymentmodel.ExecuteRefund(originalCreditAmount + originalTipAmount);


                logger.Info("Credit Refund:RECEIVED:" + resp.ToString());
                // logger.Info("Credit Refund:HRef=" + resp.TransactionId);




                if (resp.StatusCode == "Approved")
                {
                    //full approval 

                    //void items
                    logger.Info("Void all line items on ticket:" + CurrentTicket.SalesID);
                    CurrentTicket.VoidAllLineItem(tp.ReturnText);

                    if (originalCashAmount > 0)
                    {
                        logger.Info("Void all cash payment on ticket:" + CurrentTicket.SalesID);
                        CurrentTicket.VoidAllCashPayment(tp.ReturnText);
                        TouchMessageBox.Show("Refund Cash To Customer : " + originalCashAmount);
                    }





                    // decimal authamt = (decimal)resp.TotalAmount;

                    bool result = AddCreditPayment("REFUND", originalCreditAmount, TriPOSModel.ConvertToSale(resp), DateTime.Now, tp.ReturnText);

                    if (result == false)
                    {
                        TouchMessageBox.Show("Error adding payment to ticket");
                        return;
                    }

                    //print refund receipt

                    Payment payment = m_salesmodel.GetPayment(resp.TransactionId);
                    //check to see if Ticket has customer linked.
                    LinkCreditCardToCustomer(payment);

                   ReceiptPrinterModel.AutoPrintCreditSlip(payment);

                    m_parent.Close();

                }
                else
                {

                    logger.Error(resp.Errors.ToString());
                    TouchMessageBox.Show("REFUND TRANSACTION FAILED !!!! ERROR:  " + resp.Errors.ToString());
                }

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Refund Clicked: " + e.Message);
            }

        }

        public void ExecutePartialCreditRefundClicked(object amount)
        {



            try
            {
                if (RefundAmount.TotalWithTip == 0)
                {
                    TouchMessageBox.Show("Select items to do partial refund");
                    return;
                }

                decimal originalTicketAmount = CurrentTicket.Payments.Sum(x => x.NetAmount);


                if (RefundAmount.TotalWithTip > originalTicketAmount)
                {
                    TouchMessageBox.Show("Amount selected is greater than credit payment amount!!!");
                    return;
                }

                TextPad tp = new TextPad("Enter Refund Reason", "");
                Utility.OpenModal(m_parent, tp);

                if (tp.ReturnText == "") return;



                logger.Info("COMMAND:Credit Refund , Amount=" + RefundAmount.TotalWithTip);

                RefundResponse resp = virtualpaymentmodel.ExecuteRefund(RefundAmount.TotalWithTip);


                logger.Info("Credit Refund:RECEIVED:" + resp.ToString());
                // logger.Info("Credit Refund:HRef=" + resp.TransactionId);


                if (resp.StatusCode == "Approved")
                {
                    // decimal authamt = (decimal)resp.TotalAmount;


                    //full approval 
                    CurrentTicket.VoidSelectedItem(tp.ReturnText);
                    bool result = AddCreditPayment("REFUND", RefundAmount.SubTotal, TriPOSModel.ConvertToSale(resp), DateTime.Now, tp.ReturnText);

                    if (result == false)
                    {
                        TouchMessageBox.Show("Payment record insert failed.");
                        return;
                    }


                    Payment payment = m_salesmodel.GetPayment(resp.TransactionId);
                    //check to see if Ticket has customer linked.
                    LinkCreditCardToCustomer(payment);

                    //print credit slip
                    ReceiptPrinterModel.AutoPrintCreditSlip(payment);

                    m_parent.Close();

                }
                else
                {


                    logger.Error(resp.Errors.ToString());
                    TouchMessageBox.Show("REFUND TRANSACTION FAILED !!!! ERROR:  " + resp.Errors.ToString());


                }





            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Refund Clicked: " + e.Message);
            }








        }

        public void ExecuteManualRefund(object obj)
        {

            TextPad tp = new TextPad("Enter Refund Reason", "");
            Utility.OpenModal(m_parent, tp);

            if (tp.ReturnText == "") return;


            NumPad num = new NumPad("Enter Amount to Refund", false, false);
            Utility.OpenModal(m_parent, num);
            decimal amount = 0;


            if (num.Amount != "")
            {

                if (!decimal.TryParse(num.Amount, out amount))
                {
                    return;
                }
            }

            logger.Info("COMMAND:Credit Refund , Amount=" + amount.ToString());

            RefundResponse resp = virtualpaymentmodel.ExecuteRefund(amount);


            logger.Info("Credit Refund:RECEIVED:" + resp.ToString());
            if (resp.StatusCode == "Approved")
            {
                decimal authamt = (decimal)resp.TotalAmount;

                bool result = AddCreditPayment("REFUND", authamt, TriPOSModel.ConvertToSale(resp), DateTime.Now, tp.ReturnText);

                if (result == false)
                {
                    TouchMessageBox.Show("Payment record insert failed.");
                    return;
                }


                //print refund receipt

                Payment payment = m_salesmodel.GetPayment(resp.TransactionId);
                ReceiptPrinterModel.AutoPrintCreditSlip(payment);



                m_parent.Close();

            }
            else
            {


                logger.Error(resp.Errors.ToString());
                TouchMessageBox.Show("REFUND TRANSACTION FAILED !!!! ERROR:  " + resp.Errors.ToString());


            }


        }

        //--------------------Common functions ---------------------------------------------

        private bool AddCreditPayment(string transtype, decimal ticket_amount, SaleResponse2 response, DateTime paymentdate, string reason)
        {
            if (CurrentTicket == null)
                return SalesModel.InsertCreditPayment(transtype, 0, ticket_amount, response, paymentdate, reason);
            else
            {

                var result = SalesModel.InsertCreditPayment(transtype, CurrentTicket.SalesID, ticket_amount, response, paymentdate, reason);

                GlobalSettings.CustomerDisplay.WriteDisplay(response.PaymentType + ":", ticket_amount, "Balance:", CurrentTicket.Balance);

                return result;

            }
        }
    }
}
