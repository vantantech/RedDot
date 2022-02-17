using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using NLog;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class GlobalPaymentVM:CCPBaseVM
    {

       

        IDeviceInterface _device;  //PAX 300
    

        public GlobalPaymentVM(Window parent, Ticket m_ticket, string transtype, Payment payment)
            :base(parent,m_ticket,transtype,payment)
        {

       



            CreditAuthClicked = new RelayCommand(ExecuteCreditAuthClicked, param => this.CanExecuteAuthCaptureClicked);
            CreditSaleClicked = new RelayCommand(ExecuteCreditSaleClicked, param => this.CanExecuteCreditSaleClicked);
            DebitSaleClicked = new RelayCommand(ExecuteDebitSaleClicked, param => this.CanExecuteDebitSaleClicked);




            CreditRefundClicked = new RelayCommand(ExecuteCreditRefundClicked, param => true);
            DebitRefundClicked = new RelayCommand(ExecuteDebitRefundClicked, param => true);

            VoidClicked = new RelayCommand(ExecuteVoidClicked, param => this.CanExecuteVoidClicked);
            CommandClicked = new RelayCommand(ExecuteCommandClicked, param => true);


            if (GlobalSettings.Instance.CreditCardProcessor == "PAX_S300")
            {
                _device = DeviceService.Create(new ConnectionConfig
                {
                    DeviceType = DeviceType.PAX_S300,
                    ConnectionMode = ConnectionModes.TCP_IP,
                    IpAddress = GlobalSettings.Instance.SIPDefaultIPAddress,
                    Port = "10009",
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


          
        }





        string m_posmessage;
        public string POSMessage
        {
            get { return m_posmessage; }
            set
            {
                m_posmessage = value;
                NotifyPropertyChanged("POSMessage");

            }
        }

        string m_posxml;
        public string POSXML
        {
            get { return m_posxml; }
            set
            {
                m_posxml = value;
                NotifyPropertyChanged("POSXML");

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



                    int nextpayment = CurrentTicket.Payments.Where(x => x.CardGroup.ToUpper() == "CREDIT" || x.CardGroup.ToUpper() == "DEBIT").Count() + 1;
                    string requestid = CurrentTicket.SalesID.ToString() + nextpayment.ToString();

                    //calls credit card processor "Sale" command


                 


                    logger.Info("COMMAND:Credit Authorize , Amount=" + amt);
                    TerminalResponse resp = _device.CreditAuth(int.Parse(requestid), amt).WithSignatureCapture(signatureonscreen).WithAllowDuplicates(allowduplicates).Execute();
                  
                    logger.Info( "Credit Authorize:RECEIVED:" + resp.ToString());
                    logger.Info( "Credit Authorize:HRef=" + resp.TransactionId);
                    logger.Info("Response:" + resp.DeviceResponseText);

                    POSMessage = "Response:" + resp.DeviceResponseText + (char)13 + (char)10;
                    POSXML = "RAW:" + resp.ToString() + (char)13 + (char)10;
                  


                    if (resp.DeviceResponseText.ToUpper() == "OK")
                    {
                        decimal authamt = (decimal)resp.TransactionAmount;

                        if (authamt >= amt)
                        {
                            //full approval 
                            bool result = AddCreditPayment(amt, resp, DateTime.Now,"CREDIT");

                            if (result == false)
                            {
                                TouchMessageBox.Show("Payment record insert failed.");
                                logger.Error("Payment record insert failed.");
                                return;
                            }


                        }
                        else
                        {
                            //partial approval
                            logger.Info("Partial Approval:" + authamt);
                            bool result = AddCreditPayment(authamt, resp, DateTime.Now, "CREDIT");

                            if (result == false)
                            {
                                TouchMessageBox.Show("Payment record insert failed.");
                                logger.Error("Payment record insert failed.");
                                return;
                            }
                        }


                        //print credit slip
                        if (GlobalSettings.Instance.PrintCreditSlipOnClose)
                        {
                            Payment payment = m_salesmodel.GetPayment(resp.TransactionId);
                            ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket, payment);
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


                    POSMessage = "Response:" + resp.DeviceResponseText + (char)13 + (char)10;
                    POSXML = "RAW:" + resp.ToString() + (char)13 + (char)10;



                    if (resp.DeviceResponseText.ToUpper() == "OK")
                    {
                        decimal authamt = (decimal)resp.TransactionAmount;

                        if(resp.TipAmount > GlobalSettings.Instance.TipAlertAmount)
                        {
                            TouchMessageBox.Show("Alert!!! TIP AMOUNT: " + resp.TipAmount.ToString() + (char)13 + (char)10 +  " Please verify with customer!!");
                        }

                        if (authamt >= amt)
                        {
                            //full approval 
                            bool result = AddCreditPayment(amt, resp, DateTime.Now, "CREDIT");

                            if (result == false)
                            {
                                TouchMessageBox.Show("Payment record insert failed.");
                                return;
                            }


                        }
                        else
                        {
                            //partial approval
                            bool result = AddCreditPayment(authamt, resp, DateTime.Now, "CREDIT");

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
                            ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket, payment);
                        }



                        //this will run the auto tip logic or bring up tip screen if manual mode
                        if (CurrentTicket != null)    CurrentTicket.SplitTips();

                       





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
                            ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket, payment);
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


        public void ExecuteCreditRefundClicked(object amount)
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


                    logger.Info("COMMAND:Credit Refund , Amount=" + amt);
                    TerminalResponse resp = _device.CreditRefund(int.Parse(requestid), amt).Execute();
                    logger.Info("Credit Refund:RECEIVED:" + resp.ToString());
                    logger.Info("Credit Refund:HRef=" + resp.TransactionId);

                    POSMessage = "Response:" + resp.DeviceResponseText + (char)13 + (char)10;
                    POSXML = "RAW:" + resp.ToString() + (char)13 + (char)10;
                   

                    if (resp.DeviceResponseText.ToUpper() == "OK")
                    {
                        decimal authamt = (decimal)resp.TransactionAmount;

                   
                           
                            bool result = AddCreditPayment(authamt, resp, DateTime.Now, "CREDIT");

                            if (result == false)
                            {
                                TouchMessageBox.Show("Payment record insert failed.");
                                return;
                            }
                      


                        //print credit slip
                        if (GlobalSettings.Instance.PrintCreditSlipOnClose)
                        {
                            Payment payment = m_salesmodel.GetPayment(resp.TransactionId);
                            ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket, payment);
                        }


                        if (resp.PaymentType.ToUpper() == "CREDIT" || resp.PaymentType.ToUpper() == "GIFT")
                        {
                            //this will run the auto tip logic or bring up tip screen if manual mode
                            if (CurrentTicket != null)
                                CurrentTicket.SplitTips();

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

                       
                            //partial approval
                            bool result = AddCreditPayment(authamt, resp, DateTime.Now, "DEBIT");

                            if (result == false)
                            {
                                TouchMessageBox.Show("Payment record insert failed.");
                                return;
                            }
                      


                        //print credit slip
                        if (GlobalSettings.Instance.PrintCreditSlipOnClose)
                        {
                            Payment payment = m_salesmodel.GetPayment(resp.TransactionId);
                            ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket, payment);
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

            POSMessage = "Response:" + resp.DeviceResponseText + (char)13 + (char)10;
            POSXML = "RAW:" + resp.ToString() + (char)13 + (char)10;
         

            if (resp.DeviceResponseText.ToUpper() == "OK")
            {

                //update payment

                SalesModel.VoidCreditPayment(m_payment.ResponseId);
              
                if (CurrentTicket == null)
                {
                    Payment payment = m_salesmodel.GetPayment(m_payment.ResponseId);
                    //print debit slip
                    ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket, payment);

                }
                else
                {
                    //force ticket to load payment into object so it will contain the tip
                    CurrentTicket.LoadGratuity();
                    CurrentTicket.LoadPayment();


                    Payment payment = CurrentTicket.Payments.Where(x => x.ResponseId == m_payment.ResponseId).First();
                    if (payment.TipAmount > 0)
                    {
                        //if tip is already assigned to current employee, then need to remove tip first for current employee
                        //passing 0 for employee ID will delete all gratuity for that ticket
                        //this only affects nail salons
                        CurrentTicket.DeleteGratuity();
                      
                    }

                    //print credit slip
                    if (GlobalSettings.Instance.PrintCreditSlipOnClose)
                    {
                        //print debit slip
                        ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket, payment);
                    }


                }


                m_parent.Close();

            }
            else
            {

                logger.Error(resp.DeviceResponseText);
                TouchMessageBox.Show("VOID TRANSACTION FAILED !!!! ERROR:  " + resp.DeviceResponseText);
            }
        }

        public void ExecuteCommandClicked(object obj)
        {
            logger.Info("COMMAND:Reset" );
            var resp1 = _device.Reset();

            POSMessage = "Response:" + resp1.DeviceResponseText + (char)13 + (char)10;
            POSXML = "RAW:" + resp1.ToString() + (char)13 + (char)10;
            logger.Debug( "Reset:" + resp1.ToString());
        }


        public bool AddCreditPayment(decimal requested_amount, TerminalResponse response, DateTime paymentdate, string cardgroup)
        {

            try
            {



                if (CurrentTicket == null)
                    return SalesModel.InsertCreditPayment(0, requested_amount, response, paymentdate,cardgroup);
                else
                {
                  var result =  CurrentTicket.InsertCreditPayment( requested_amount, response, paymentdate,cardgroup);

                    vfd.WriteDisplay(response.PaymentType + ":", requested_amount, "Balance:", CurrentTicket.Balance);

                    return result;
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
