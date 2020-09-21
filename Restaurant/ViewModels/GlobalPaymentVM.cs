using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using NLog;
//using POSLink;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class GlobalPaymentVM: CCPBaseVM
    {

        IDeviceInterface _device;  //PAX 300
        private string _reason;

        public GlobalPaymentVM(Window parent, SecurityModel security, Ticket m_ticket, string transtype, Payment payment, string reason) 
            : base(parent,security, m_ticket, transtype, payment)
        {
            _reason = reason;


            CreditSaleClicked = new RelayCommand(ExecuteCreditSaleClicked, param => this.CanExecuteCreditSaleClicked);
            CreditAuthClicked = new RelayCommand(ExecuteCreditAuthClicked, param => this.CanExecuteAuthCaptureClicked);
            VoidClicked = new RelayCommand(ExecuteVoidClicked, param => this.CanExecuteVoidClicked);
            FullCreditRefundClicked = new RelayCommand(ExecuteFullCreditRefundClicked, param => true);
            CommandClicked = new RelayCommand(ExecuteCommandClicked, param => true);
            PartialCreditRefundClicked = new RelayCommand(ExecutePartialCreditRefundClicked, param => true);
            ManualRefundClicked = new RelayCommand(ExecuteManualRefund, param => true);

            //  DebitRefundClicked = new RelayCommand(ExecuteDebitRefundClicked, param => true);

            m_salesmodel = new SalesModel(m_security);

           // vfd = new VFD(GlobalSettings.Instance.DisplayComPort);

             _device = DeviceService.Create(new ConnectionConfig
                {
                    DeviceType = DeviceType.PAX_S300,
                    ConnectionMode = ConnectionModes.TCP_IP,
                    IpAddress = GlobalSettings.Instance.SIPDefaultIPAddress,
                    Port = GlobalSettings.Instance.SIPPort,
                    Timeout = 30000
                });

                //initialize logging event
                _device.OnMessageSent += (message) =>
                {
                    logger.Info("SENT:" + message);
                };
                _device.Initialize();
               _device.Reset();
        

        
          
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
                        TouchMessageBox.Show("Amount is greater than balance!!!");
                        return;
                    }



                    int nextpayment = CurrentTicket.Payments.Where(x => x.CardGroup.ToUpper() == "CREDIT" || x.CardGroup.ToUpper() == "DEBIT").Count() + 1;
                    string requestid = CurrentTicket.SalesID.ToString() + nextpayment.ToString();

                    //calls credit card processor "Sale" command



                    bool requesttip = GlobalSettings.Instance.RequestTip;

                    logger.Info("COMMAND:Credit Sale , Amount=" + amt);

                    TerminalResponse resp = _device.CreditSale(int.Parse(requestid), amt).WithSignatureCapture(signatureonscreen).WithAllowDuplicates(allowduplicates).WithRequestTip(requesttip).Execute();



                    logger.Info("Credit Sale:RECEIVED:" + resp.ToString());
                    logger.Info("Credit Sale:HRef=" + resp.TransactionId);


                    if (resp.DeviceResponseText.ToUpper() == "OK")
                    {
                        decimal authamt = (decimal)resp.TransactionAmount;

                        if (authamt >= amt)
                        {
                            //full approval 
                            bool result = AddCreditPayment(amt, resp, DateTime.Now, "CREDIT","","SALE");

                            if (result == false)
                            {
                                TouchMessageBox.Show("Payment record insert failed.");
                                return;
                            }


                        }
                        else
                        {
                            //partial approval
                            bool result = AddCreditPayment(authamt, resp, DateTime.Now, "CREDIT","","SALE");

                            if (result == false)
                            {
                                TouchMessageBox.Show("Payment record insert failed.");
                                return;
                            }
                        }


                        //print credit slip
                  
                            Payment payment = m_salesmodel.GetPayment(resp.TransactionId);
                            ReceiptPrinterModel.AutoPrintCreditSlip(payment);
                       


                        m_parent.Close();

                    }
                    else
                    {
                        logger.Error(resp.DeviceResponseText);
                        TouchMessageBox.Show("SALE TRANSACTION FAILED !!!! ERROR:  " + resp.DeviceResponseText);

                    }


                }
                else TouchMessageBox.Show("Please Enter $ Amt to Process.");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Charge Clicked: " + e.Message);
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
                    if(m_transtype != "PREAUTH")
                        if (amt > CurrentTicket.Balance)
                        {
                            TouchMessageBox.Show("Amount is greater than balance!!!");
                            return;
                        }



                    int nextpayment = CurrentTicket.Payments.Where(x => x.CardGroup.ToUpper() == "CREDIT" || x.CardGroup.ToUpper() == "DEBIT").Count() + 1;
                    string requestid = CurrentTicket.SalesID.ToString() + nextpayment.ToString();

                    //calls credit card processor "Sale" command


                 


                    logger.Info("COMMAND:Credit Authorize , Amount=" + amt);
                    TerminalResponse resp = _device.CreditAuth(int.Parse(requestid), amt).WithSignatureCapture(signatureonscreen).WithAllowDuplicates(allowduplicates).Execute();

                    logger.Info( "Credit Authorize:RECEIVED:" + resp.ToString());
                    logger.Info( "Credit Authorize:HRef=" + resp.TransactionId);

                    if (resp.DeviceResponseText.ToUpper() == "OK")
                    {
                        decimal authamt = (decimal)resp.TransactionAmount;

                        if (authamt >= amt)
                        {
                            //full approval 
                            bool result = AddCreditPayment(amt, resp, DateTime.Now,"CREDIT","",m_transtype=="SALE"?"AUTH":"PREAUTH");

                            if (result == false)
                            {
                                TouchMessageBox.Show("Payment record insert failed.");
                                return;
                            }


                        }
                        else
                        {
                            //partial approval
                            bool result = AddCreditPayment(authamt, resp, DateTime.Now, "CREDIT","", m_transtype == "SALE" ? "AUTH" : "PREAUTH");

                            if (result == false)
                            {
                                TouchMessageBox.Show("Payment record insert failed.");
                                return;
                            }
                        }


                        //print credit slip
                        if (m_transtype != "PREAUTH")
                        {
                            Payment payment = m_salesmodel.GetPayment(resp.TransactionId);
                            ReceiptPrinterModel.AutoPrintCreditSlip(payment);
                        }
                           

                        m_parent.Close();

                    }
                    else
                    {
                        logger.Error(resp.DeviceResponseText);
                        TouchMessageBox.Show("SALE TRANSACTION FAILED !!!! ERROR:  " + resp.DeviceResponseText);

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
            if (m_payment.ResponseId == "")
            {
                TouchMessageBox.Show("No Transaction #  to Void  with !!!");
                return;
            }

  

            //  Payment payment = CurrentTicket.Payments.Where(x => x.ResponseId == m_transactionid).First();

            logger.Info("COMMAND:Credit Void , Transaction ID=" + m_payment.ResponseId);
            TerminalResponse resp = _device.CreditVoid(1).WithTransactionId(m_payment.ResponseId).Execute();
            logger.Info("Credit Void:RECEIVED:" + resp.ToString());
            logger.Info("Credit Void:HRef=" + resp.TransactionId);


            if (resp.DeviceResponseText.ToUpper() == "OK")
            {

                //update payment

                CurrentTicket.VoidPayment(m_payment.ID,_reason);

                Payment payment = m_salesmodel.GetPayment(m_payment.ResponseId);
               //print debit slip
                ReceiptPrinterModel.AutoPrintCreditSlip(payment);
    


                m_parent.Close();

            }
            else
            {



                logger.Error(resp.DeviceResponseText);
                TouchMessageBox.Show("VOID TRANSACTION FAILED !!!! ERROR:  " + resp.DeviceResponseText);


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

                int nextpayment = CurrentTicket.Payments.Where(x => x.TransType.ToUpper() == "RETURN").Count() + 50;
                string requestid = CurrentTicket.SalesID.ToString() + nextpayment.ToString();


                TerminalResponse resp = _device.CreditRefund(int.Parse(requestid), originalCreditAmount + originalTipAmount).Execute();
                logger.Info("Credit Refund:RECEIVED:" + resp.ToString());
                logger.Info("Credit Refund:HRef=" + resp.TransactionId);

               
                   

                if (resp.DeviceResponseText.ToUpper() == "OK")
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



                   // decimal authamt = (decimal)resp.TransactionAmount;
                    bool result = AddCreditPayment(originalCreditAmount, resp, DateTime.Now, "CREDIT",tp.ReturnText, m_transtype);

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

                        logger.Error(resp.DeviceResponseText);
                        TouchMessageBox.Show("REFUND TRANSACTION FAILED !!!! ERROR:  " + resp.DeviceResponseText);
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
                decimal originalTipAmount = CurrentTicket.Payments.Sum(x => x.TipAmount);

                if (RefundAmount.TotalWithTip > (originalTicketAmount + originalTipAmount))
                {
                    TouchMessageBox.Show("Amount selected is greater than credit payment amount!!!");
                    return;
                }

                TextPad tp = new TextPad("Enter Refund Reason", "");
                Utility.OpenModal(m_parent, tp);

                if (tp.ReturnText == "") return;


                logger.Info("COMMAND:Credit Refund , Amount=" + RefundAmount.TotalWithTip);

                int nextpayment = CurrentTicket.Payments.Where(x => x.TransType.ToUpper() == "RETURN").Count() + 50;
                string requestid = CurrentTicket.SalesID.ToString() + nextpayment.ToString();
                TerminalResponse resp = _device.CreditRefund(int.Parse(requestid), RefundAmount.TotalWithTip).Execute();

                logger.Info("Credit Refund:RECEIVED:" + resp.ToString());
                // logger.Info("Credit Refund:HRef=" + resp.TransactionId);




                if (resp.DeviceResponseText.ToUpper() == "OK")
                {
                   
                    //full approval 
                    CurrentTicket.VoidSelectedItem(tp.ReturnText);

                    //decimal authamt = (decimal)resp.TransactionAmount;
                    bool result = AddCreditPayment(RefundAmount.SubTotal, resp, DateTime.Now, "CREDIT",tp.ReturnText, m_transtype);

                    if (result == false)
                    {
                        TouchMessageBox.Show("Error adding payment to ticket");
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

                    logger.Error(resp.DeviceResponseText);
                    TouchMessageBox.Show("REFUND TRANSACTION FAILED !!!! ERROR:  " + resp.DeviceResponseText);

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


            int nextpayment = CurrentTicket.Payments.Where(x => x.TransType.ToUpper() == "RETURN").Count() + 50;
            string requestid = CurrentTicket.SalesID.ToString() + nextpayment.ToString();
            TerminalResponse resp = _device.CreditRefund(int.Parse(requestid), amount).Execute();


            logger.Info("Credit Refund:RECEIVED:" + resp.ToString());
            if (resp.DeviceResponseText.ToUpper() == "OK")
            {
                decimal authamt = (decimal)resp.TransactionAmount;

                bool result = AddCreditPayment(authamt, resp, DateTime.Now, "CREDIT",tp.ReturnText, m_transtype);

                if (result == false)
                {
                    TouchMessageBox.Show("Error adding payment to ticket");
                    return;
                }


                //print refund receipt

                Payment payment = m_salesmodel.GetPayment(resp.TransactionId);
                ReceiptPrinterModel.AutoPrintCreditSlip(payment);



                m_parent.Close();

            }
            else
            {

                logger.Error(resp.DeviceResponseText);
                TouchMessageBox.Show("REFUND TRANSACTION FAILED !!!! ERROR:  " + resp.DeviceResponseText);

            }


        }
        /*
                public void ExecuteDebitRefundClicked(object amount)
                {
                    string temp = amount as string;
                    decimal amt;

                    try
                    {



                        if (temp != "")
                        {
                            amt = decimal.Parse(temp);

                            decimal originalTicketAmount = CurrentTicket.Payments.Sum(x => x.NetAmount);


                            if (amt > originalTicketAmount)
                            {
                                TouchMessageBox.Show("Amount is greater than original ticket amount!!!");
                                return;
                            }


                            int nextpayment = CurrentTicket.Payments.Where(x => x.TransType.ToUpper() == "RETURN").Count() + 50;

                            string requestid = CurrentTicket.SalesID.ToString() + nextpayment.ToString();


                            logger.Info("COMMAND:Debit Refund , Amount=" + amt);
                            TerminalResponse resp = _device.DebitRefund(int.Parse(requestid), amt).Execute();
                            logger.Info("Debit Refund:RECEIVED:" + resp.ToString());
                            logger.Info("Debit Refund:HRef=" + resp.TransactionId);


                            POSMessage = "Response:" + resp.DeviceResponseText + (char)13 + (char)10;
                            POSXML = "RAW:" + resp.ToString() + (char)13 + (char)10;



                            if (resp.DeviceResponseText.ToUpper() == "OK")
                            {
                                decimal authamt = (decimal)resp.TransactionAmount;

                                if (authamt >= amt)
                                {
                                    //full approval 
                                    bool result = AddCreditPayment(amt, resp, DateTime.Now, "DEBIT");

                                    if (result == false)
                                    {
                                        TouchMessageBox.Show("Payment record insert failed.");
                                        return;
                                    }


                                }
                                else
                                {
                                    //partial approval
                                    bool result = AddCreditPayment(authamt, resp, DateTime.Now, "DEBIT");

                                    if (result == false)
                                    {
                                        TouchMessageBox.Show("Payment record insert failed.");
                                        return;
                                    }
                                }


                                //print credit slip
                                if (GlobalSettings.Instance.PrintCreditSlipOnClose)
                                {
                                    Payment payment = m_salesmodel.GetPayment(resp.TransactionId);
                                    ReceiptPrinterModel.AutoPrintCreditSlip( payment);
                                }



                                m_parent.Close();

                            }
                            else
                            {


                                logger.Error(resp.DeviceResponseText);
                                TouchMessageBox.Show("REFUND TRANSACTION FAILED !!!! ERROR:  " + resp.DeviceResponseText);


                            }



                        }
                        else TouchMessageBox.Show("Please Enter Refund Amount");

                    }
                    catch (Exception e)
                    {

                        TouchMessageBox.Show("Refund Clicked: " + e.Message);
                    }
                }
                */

        /*
        public void ExecuteDebitSaleClicked(object amount)
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



                    int nextpayment = CurrentTicket.Payments.Where(x => x.CardGroup.ToUpper() == "CREDIT" || x.CardGroup.ToUpper() == "DEBIT").Count() + 1;
                    string requestid = CurrentTicket.SalesID.ToString() + nextpayment.ToString();

                    //calls credit card processor "Sale" command

                    logger.Info("COMMAND:Debit Sale , Amount=" + amt );
                    TerminalResponse resp = _device.DebitSale(int.Parse(requestid), amt).Execute();
                    logger.Info("Debit Sale:RECEIVED:" + resp.ToString());
                    logger.Info("Debit Sale:HRef=" + resp.TransactionId);

                    POSMessage = "Response:" + resp.DeviceResponseText + (char)13 + (char)10;
                    POSXML = "RAW:" + resp.ToString() + (char)13 + (char)10;
                 


                    if (resp.DeviceResponseText.ToUpper() == "OK")
                    {
                        decimal authamt = (decimal)resp.TransactionAmount;

                        if (authamt >= amt)
                        {
                            //full approval 
                            bool result = AddCreditPayment(amt, resp, DateTime.Now,"DEBIT");

                            if (result == false)
                            {
                                TouchMessageBox.Show("Payment record insert failed.");
                                return;
                            }


                        }
                        else
                        {
                            //partial approval
                            bool result = AddCreditPayment(authamt, resp, DateTime.Now,"DEBIT");

                            if (result == false)
                            {
                                TouchMessageBox.Show("Payment record insert failed.");
                                return;
                            }
                        }


                        //print credit slip
                        if (GlobalSettings.Instance.PrintCreditSlipOnClose)
                        {
                            Payment payment = m_salesmodel.GetPayment(resp.TransactionId);
                            ReceiptPrinterModel.AutoPrintCreditSlip( payment);
                        }

                        m_parent.Close();

                    }
                    else
                    {



                        logger.Error(resp.DeviceResponseText);
                        TouchMessageBox.Show("SALE TRANSACTION FAILED !!!! ERROR:  " + resp.DeviceResponseText);








                    }


                }
                else TouchMessageBox.Show("Please Enter $ Amt to Process.");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Charge Clicked: " + e.Message);
            }


        }

*/

    

        public void ExecuteCommandClicked(object obj)
        {
            logger.Info("COMMAND:Reset" );
            var resp1 = _device.Reset();

   
            logger.Debug( "Reset:" + resp1.ToString());
        }


        public bool AddCreditPayment(decimal requested_amount, TerminalResponse response, DateTime paymentdate, string cardgroup,string reason,string transtype)
        {

            try
            {



                if (CurrentTicket == null)
                    return SalesModel.InsertCreditPayment(0, requested_amount, response, paymentdate,cardgroup,reason,transtype);
                else
                {
                    GlobalSettings.CustomerDisplay.WriteDisplay(response.PaymentType + ":", requested_amount, "Balance:", CurrentTicket.Balance);
                    return SalesModel.InsertCreditPayment(CurrentTicket.SalesID, requested_amount, response, paymentdate,cardgroup,reason,transtype);
                }

                

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("AddPayment:" + e.Message);
                return false;
            }
        }


   

    }
}
