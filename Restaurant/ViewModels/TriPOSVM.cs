using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TriPOS.ResponseModels;

namespace RedDot
{
    public class TriPOSVM : CCPBaseVM
    {
        private TriPOSModel triposmodel = new TriPOSModel(GlobalSettings.Instance.LaneId);
        private string _reason;

       

        public TriPOSVM(Window parent, SecurityModel security, Ticket m_ticket, string transtype, Payment payment,string reason)
             : base(parent,security, m_ticket, transtype, payment)
        {
      
            _reason = reason;

            CreditSaleClicked = new RelayCommand(ExecuteCreditSaleClicked, param => this.CanExecuteCreditSaleClicked);
            CreditAuthClicked = new RelayCommand(ExecuteCreditAuthClicked, param => this.CanExecuteAuthCaptureClicked);
            VoidClicked = new RelayCommand(ExecuteVoidClicked, param => this.CanExecuteVoidClicked);
            FullCreditRefundClicked = new RelayCommand(ExecuteFullCreditRefundClicked, param => (transtype == "REFUND"));
            PartialCreditRefundClicked = new RelayCommand(ExecutePartialCreditRefundClicked, param => (transtype == "REFUND"));
            ManualRefundClicked = new RelayCommand(ExecuteManualRefund, param => (transtype == "REFUND"));
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
                    if (amt > CurrentTicket.Balance)
                    {
                        TouchMessageBox.Show("Amount is greater than balance!!!",10);
                        return;
                    }

                      

                    string referencenumber = Utility.GenerateReferenceNumber();
                    SaleResponse2 resp = triposmodel.ExecuteCreditSale(CurrentTicket.SalesID,amt,referencenumber);
                    logger.Info("CREDIT SALE => reference number:" + referencenumber + ", ticket=" + CurrentTicket.SalesID + ", amount=" + amt);

          

                    if (resp != null)
                    {
                        ProcessResponse("SALE",resp,amt);
                    }
                    else
                    {
                        logger.Error("Transaction Response is NULL .. Running Work Around Code ...");
                        TransactionQueryResponse response = triposmodel.ExecuteTransactionQuery(referencenumber);

                        if (response != null)
                        {
                            if (response.ExpResponse.ExpReportingData.Items.Count > 0)
                            {

                                SaleResponse2 converted = TriPOSModel.ConvertToSale(response.ExpResponse.ExpReportingData.Items[0]);
                                ProcessResponse("SALE",converted , amt);
                            }

                        }
                        else
                        {
                            logger.Error(resp.Errors.ToString());
                            TouchMessageBox.Show("SALE TRANSACTION FAILED !!!! ERROR:  " + resp.Errors.ToString());
                        }

                    }

               

                }
                else TouchMessageBox.Show("Please Enter $ Amt to Process.");

             
            }
            catch (Exception e)
            {
                logger.Error("Credit Sale Clicked: " + e.Message);
                TouchMessageBox.Show("Credit Sale Clicked: " + e.Message);
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

                    string referencenumber = Utility.GenerateReferenceNumber();
                    AuthorizationResponse resp = triposmodel.ExecuteCreditAuthorize(CurrentTicket.SalesID, amt,referencenumber);
                    logger.Info("CREDIT AUTH => reference number:" + referencenumber + ", ticket=" + CurrentTicket.SalesID + ", amount=" + amt);


                    if (resp != null)
                    {
                        ProcessResponse(m_transtype == "SALE" ? "AUTH" : "PREAUTH",TriPOSModel.ConvertToSale(resp),amt);
                    }
                    else
                    {
                        logger.Error("Transaction Response is NULL .. Running Work Around Code ...");
                        TransactionQueryResponse response = triposmodel.ExecuteTransactionQuery(referencenumber);

                        if (response != null)
                        {
                            if (response.ExpResponse.ExpReportingData.Items.Count > 0)
                            {
                                SaleResponse2 converted =  TriPOSModel.ConvertToSale(response.ExpResponse.ExpReportingData.Items[0]);
                                ProcessResponse(m_transtype == "SALE" ? "AUTH" : "PREAUTH",converted , amt);
                            }

                        }else
                        {
                            logger.Error(resp.Errors.ToString());
                            TouchMessageBox.Show("SALE TRANSACTION FAILED !!!! ERROR:  " + resp.Errors.ToString());
                        }
                           
                    }

                }
                       

                else TouchMessageBox.Show("Please Enter $ Amt to Process.");

            }
            catch (Exception e)
            {
                logger.Error("Credit Auth Clicked: " + e.Message);
                TouchMessageBox.Show("Credit Auth Clicked: " + e.Message);
            }


        }

   

         
         
      

        public void ProcessResponse(string transtype,SaleResponse2 resp,decimal requestedamt)
        {
            switch (resp.StatusCode.ToUpper())
            {
                case "AUTHORIZED":
                case "APPROVED":
 
                    logger.Info("Ticket:" + CurrentTicket.SalesID + " : Credit Payment Approved.");

                    bool result = AddCreditPayment(transtype, requestedamt, resp, DateTime.Now, "");
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
                    logger.Error("unknown response status:" + resp.StatusCode);
                    break;

            }
        }

        public void ExecuteVoidClicked(object obj)
        {
            if (m_payment == null)
            {
                TouchMessageBox.Show("No Transaction #  to Void  with !!!");
                return;
            }

            string referencenumber = Utility.GenerateReferenceNumber();

            string status = "";
            if(m_payment.TransType.ToUpper() == "AUTH")
            {
                //AUTH => REVERSAL   , SALE => REVERSAL  .. reversal can be used for both .. weird
              
                ReversalResponse resp = triposmodel.ExecuteReversal(m_payment.ResponseId, m_payment.CardGroup, m_payment.Amount,referencenumber);
                logger.Info("CREDIT REVERSAL => reference number:" + referencenumber + ", ticket=" + CurrentTicket.SalesID + ",transid=" + m_payment.ResponseId +  ", amount=" + m_payment.Amount);
                status = resp.StatusCode;
            }
            else
            { 
                //SALE => VOID
                VoidResponse resp = triposmodel.ExecuteVoid(m_payment.ResponseId,referencenumber);
                logger.Info("CREDIT VOID => reference number:" + referencenumber + ", ticket=" + CurrentTicket.SalesID + ", transid=" + m_payment.ResponseId);
                status = resp.StatusCode;
            }



            if (status == "Approved")
            {
                //update payment
                CurrentTicket.VoidPayment(m_payment.ID,_reason);
         
                Payment payment = m_salesmodel.GetPayment(m_payment.ResponseId);
                //print debit slip
                ReceiptPrinterModel.AutoPrintCreditSlip( payment);
         
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

                if(CurrentTicket.Balance != 0)
                {
                    TouchMessageBox.Show("Balance not ZERO .. does not qualify for Full Refund.");
                    return;
                }

                int refundedcount = CurrentTicket.Payments.Where(x => x.CardGroup == "CREDIT" && x.TransType == "REFUND" && x.Voided == false).Count();
                if(refundedcount > 0)
                {
                    TouchMessageBox.Show("Already has a refund .. does not qualify for Full Refund.");
                    return;
                }
                TextPad tp = new TextPad("Enter Refund Reason", "");
                Utility.OpenModal(m_parent, tp);

                if (tp.ReturnText == "") return;

                decimal originalCreditAmount = CurrentTicket.Payments.Where(x=>x.CardGroup == "CREDIT" ).Sum(x => x.NetAmount ); //do not use the charged amount because there might be cash back and exclude refunds ( negative)
                decimal originalTipAmount = CurrentTicket.Payments.Where(x => x.CardGroup == "CREDIT").Sum(x => x.TipAmount); //do not use the charged amount because there might be cash back and exclude refunds ( negative)

                decimal originalCashAmount = CurrentTicket.Payments.Where(x => x.CardGroup == "CASH").Sum(x => x.NetAmount);

                logger.Info("COMMAND:Credit Refund , Amount=" + originalCreditAmount);

                //credit card processor specific code
                string referencenumber = Utility.GenerateReferenceNumber();
                RefundResponse resp = triposmodel.ExecuteRefund(originalCreditAmount + originalTipAmount,referencenumber);
                logger.Info("CREDIT REFUND => reference number:" + referencenumber + ", ticket=" + CurrentTicket.SalesID + ", amount=" + originalCreditAmount + originalTipAmount);

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

                    bool result = AddCreditPayment("REFUND", originalCreditAmount, TriPOSModel.ConvertToSale(resp), DateTime.Now,tp.ReturnText);

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
                string referencenumber = Utility.GenerateReferenceNumber();
                RefundResponse resp = triposmodel.ExecuteRefund(RefundAmount.TotalWithTip,referencenumber);
                logger.Info("CREDIT REFUND => reference number:" + referencenumber + ", ticket=" + CurrentTicket.SalesID + ", amount=" + RefundAmount.TotalWithTip);

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
            decimal amount = 0;

            if (CurrentTicket.Balance < 0) amount = (-1) * CurrentTicket.Balance;
          

            TextPad tp = new TextPad("Enter Refund Reason", "");
            Utility.OpenModal(m_parent, tp);

            if (tp.ReturnText == "") return;


            if(amount ==0)
            {
                NumPad num = new NumPad("Enter Amount to Refund", false, false);
                Utility.OpenModal(m_parent, num);



                if (num.Amount != "")
                {

                    if (!decimal.TryParse(num.Amount, out amount))
                    {
                        return;
                    }
                }
                else
                {
                    //user cancels so return
                    return;

                }
            }
  

            logger.Info("COMMAND:Credit Refund , Amount=" + amount.ToString());
            string referencenumber = Utility.GenerateReferenceNumber();
            RefundResponse resp = triposmodel.ExecuteRefund(amount,referencenumber);
            logger.Info("CREDIT REFUND => reference number:" + referencenumber + ", ticket=" + CurrentTicket.SalesID + ", amount=" + amount);

            logger.Info("Credit Refund:RECEIVED:" + resp.ToString());


            if (resp.StatusCode == "Approved")
            {
                decimal authamt = (decimal)resp.TotalAmount;

                bool result = AddCreditPayment("REFUND", authamt, TriPOSModel.ConvertToSale(resp), DateTime.Now,tp.ReturnText);

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
                return SalesModel.InsertCreditPayment(transtype, 0, ticket_amount, response, paymentdate,  reason);
            else
            {
               
                var result = SalesModel.InsertCreditPayment(transtype, CurrentTicket.SalesID, ticket_amount, response, paymentdate,reason);

                GlobalSettings.CustomerDisplay.WriteDisplay(response.PaymentType + ":", ticket_amount, "Balance:", CurrentTicket.Balance);

                return result;

            }
        }

    
    

    }
}
