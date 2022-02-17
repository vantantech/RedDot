using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TriPOS.ResponseModels;

namespace RedDot
{
    public class TriPOSVM:CCPBaseVM
    {
        private TriPOSModel triposmodel = new TriPOSModel(GlobalSettings.Instance.LaneId);
        public TriPOSVM(Window parent, Ticket m_ticket, string transtype, Payment payment)
             : base(parent, m_ticket, transtype, payment)
        {
            CreditSaleClicked = new RelayCommand(ExecuteCreditSaleClicked, param => this.CanExecuteCreditSaleClicked);
            CreditAuthClicked = new RelayCommand(ExecuteCreditAuthClicked, param => this.CanExecuteAuthCaptureClicked);
            VoidClicked = new RelayCommand(ExecuteVoidClicked, param => this.CanExecuteVoidClicked);

            CreditRefundClicked = new RelayCommand(ExecuteCreditRefundClicked, param => true);

            //future implementation if needed
            // FullCreditRefundClicked = new RelayCommand(ExecuteFullCreditRefundClicked, param => true);
            // PartialCreditRefundClicked = new RelayCommand(ExecutePartialCreditRefundClicked, param => true);
            // ManualRefundClicked = new RelayCommand(ExecuteManualRefund, param => true);

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

                    string referencenumber = Utility.GenerateReferenceNumber();
                    SaleResponse resp = triposmodel.ExecuteCreditSale(CurrentTicket.SalesID, amt, referencenumber);
                    logger.Info("CREDIT SALE => reference number:" + referencenumber + ", ticket=" + CurrentTicket.SalesID + ", amount=" + amt);


                    if (resp != null)
                    {
                        switch (resp.StatusCode.ToUpper())
                        {

                            case "APPROVED":
                                bool result = AddCreditPayment("SALE", amt, resp, DateTime.Now);
                                if (result == false)
                                {
                                    TouchMessageBox.Show("Payment record insert failed.");
                                    return;
                                }


                                Payment payment = m_salesmodel.GetPayment(resp.TransactionId);
                          


                                //print credit slip
                                if (GlobalSettings.Instance.PrintCreditSlipOnClose)
                                    ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket,payment);

                                //this will run the auto tip logic or bring up tip screen if manual mode
                                if (CurrentTicket != null) CurrentTicket.SplitTips();



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
                        TouchMessageBox.Show("No Response from Credit Card Pin Pad");

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
                    if (amt > CurrentTicket.Balance)
                    {
                        TouchMessageBox.Show("Amount is greater than balance!!!");
                        return;
                    }

                    string referencenumber = Utility.GenerateReferenceNumber();
                    AuthorizationResponse resp = triposmodel.ExecuteCreditAuthorize(CurrentTicket.SalesID, amt, referencenumber);
                    logger.Info("CREDIT AUTH => reference number:" + referencenumber + ", ticket=" + CurrentTicket.SalesID + ", amount=" + amt);



                    if (resp != null)
                    {
                        switch (resp.StatusCode.ToUpper())
                        {
                            case "APPROVED":
                                bool result = AddCreditPayment(m_transtype == "SALE" ? "AUTH" : "PREAUTH", amt, TriPOSModel.ConvertToSale(resp), DateTime.Now);
                                if (result == false)
                                {
                                    TouchMessageBox.Show("Payment record insert failed.");
                                    return;
                                }

                                Payment payment = m_salesmodel.GetPayment(resp.TransactionId);


                                //print credit slip
                                if (m_transtype != "PREAUTH")
                                    if (GlobalSettings.Instance.PrintCreditSlipOnClose)
                                        ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket,payment);



                                m_parent.Close();
                                break;

                            case "NONE":
                                TouchMessageBox.Show("No Response from PIN PAD, Please try again in 20 seconds",20);
                                break;

                            case "CANCELLED":
                                TouchMessageBox.Show("Transaction Cancelled.");
                                break;

                            default:
                                TouchMessageBox.Show("Unknown Error!!");
                                logger.Error("Unknown Error!:" + resp.StatusCode);
                               // TouchMessageBox.Show(resp.StatusCode);
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

            string referencenumber = Utility.GenerateReferenceNumber();

            string status = "";
            if (m_payment.TransType.ToUpper() == "AUTH")
            {
                //AUTH => REVERSAL   , SALE => REVERSAL  .. reversal can be used for both .. weird

                ReversalResponse resp = triposmodel.ExecuteReversal(m_payment.ResponseId, m_payment.CardGroup, m_payment.Amount, referencenumber);
                logger.Info("CREDIT REVERSAL => reference number:" + referencenumber + ", ticket=" + CurrentTicket.SalesID + ",transid=" + m_payment.ResponseId + ", amount=" + m_payment.Amount);
                status = resp.StatusCode;
            }
            else
            {
                //SALE => VOID
                VoidResponse resp = triposmodel.ExecuteVoid(m_payment.ResponseId, referencenumber);
                logger.Info("CREDIT VOID => reference number:" + referencenumber + ", ticket=" + CurrentTicket.SalesID + ", transid=" + m_payment.ResponseId);
                status = resp.StatusCode;
            }

            

            if (status == "Approved")
            {
                //update payment
                CurrentTicket.VoidPayment(m_payment.ID);
                CurrentTicket.DeleteGratuity();

                Payment payment = m_salesmodel.GetPayment(m_payment.ResponseId);
                //print debit slip
                ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket,payment);
          
                m_parent.Close();
            }
            else
            {
                logger.Error("VOID TRANSACTION FAILED !!!! ERROR:  " + status);
                TouchMessageBox.Show("VOID TRANSACTION FAILED !!!! ERROR:  " + status);
            }
        }


        public void ExecuteCreditRefundClicked(object amountobj)
        {
            string temp = amountobj as string;
            decimal amt;

            try
            {



                if (temp != "")
                {
                    amt = decimal.Parse(temp);

                    if(amt == 0) return;

                    decimal originalTicketAmount = CurrentTicket.Payments.Sum(x => x.NetAmount);


                    if (amt > originalTicketAmount)
                    {
                        TouchMessageBox.Show("Amount is greater than original ticket amount!!!");
                        return;
                    }


                    logger.Info("COMMAND:Credit Refund , Amount=" + amountobj.ToString());
                    string referencenumber = Utility.GenerateReferenceNumber();
                    RefundResponse resp = triposmodel.ExecuteRefund(amt, referencenumber);
                    logger.Info("CREDIT REFUND => reference number:" + referencenumber + ", ticket=" + CurrentTicket.SalesID + ", amount=" + amountobj);

                    logger.Info("Credit Refund:RECEIVED:" + resp.ToString());




                    if (resp.StatusCode == "Approved")
                    {
                        decimal authamt = (decimal)resp.TotalAmount;



                        bool result = AddCreditPayment("REFUND", authamt, TriPOSModel.ConvertToSale(resp), DateTime.Now);

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
                        logger.Error(resp.Errors.ToString());
                        TouchMessageBox.Show("REFUND TRANSACTION FAILED !!!! ERROR:  " + resp.Errors.ToString());

                    }



                }
                else TouchMessageBox.Show("Please Enter Refund Amount");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Refund Clicked: " + e.Message);
            }
        }



        private bool AddCreditPayment(string transtype,decimal requested_amount, SaleResponse response, DateTime paymentdate)
        {
            if (CurrentTicket == null)
               return  SalesModel.InsertCreditPayment(transtype,0, requested_amount, response,paymentdate);
            else
            {
                var result = CurrentTicket.InsertCreditPayment(transtype, requested_amount, response,paymentdate);



                vfd.WriteDisplay(response.PaymentType + ":", requested_amount, "Balance:", CurrentTicket.Balance);

                return result;
              
            }
        }
    }
}
