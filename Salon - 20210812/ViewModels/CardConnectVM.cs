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
    public class CardConnectVM:CCPBaseVM
    {
   
        public CardConnectVM(Window parent, Ticket m_ticket, string transtype, Payment payment)
             : base(parent, m_ticket, transtype, payment)
        {
            CreditSaleClicked = new RelayCommand(ExecuteCreditSaleClicked, param => this.CanExecuteCreditSaleClicked);
            CreditAuthClicked = new RelayCommand(ExecuteCreditAuthClicked, param => this.CanExecuteAuthCaptureClicked);
            VoidClicked = new RelayCommand(ExecuteVoidClicked, param => this.CanExecuteVoidClicked);
            CreditRefundClicked = new RelayCommand(ExecuteCreditRefundClicked, param => true);
        }

        public void ExecuteCreditSaleClicked(object amount)
        {
            string temp = (string)amount;
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

                
                    string referencenumber = CardConnectModel.ProcessCredit(CurrentTicket.SalesID,amt,true);

                    if(referencenumber != "")
                    {
                        //this will run the auto tip logic or bring up tip screen if manual mode
                        if (CurrentTicket != null) CurrentTicket.SplitTips();

                        Payment payment = m_salesmodel.GetPayment(referencenumber);

                        //print credit slip
                        if (GlobalSettings.Instance.PrintCreditSlipOnClose)
                            ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket, payment);

                        m_parent.Close();
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
            string temp = (string)amount;
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


                    string referencenumber = CardConnectModel.ProcessCredit(CurrentTicket.SalesID, amt, false);

                    if (referencenumber != "")
                    {
                        //this will run the auto tip logic or bring up tip screen if manual mode
                        if (CurrentTicket != null) CurrentTicket.SplitTips();

                        Payment payment = m_salesmodel.GetPayment(referencenumber);

                        //print credit slip
                        if (GlobalSettings.Instance.PrintCreditSlipOnClose)
                            ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket, payment);

                        m_parent.Close();
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
            if (m_payment.TransType.ToUpper() != "SALE")
            {
                //AUTH => REVERSAL   , SALE => REVERSAL  .. reversal can be used for both .. weird

                var resp = CardConnectModel.Void(m_payment.ResponseId, m_payment.AuthorCode, m_payment.Amount);
                logger.Info("CREDIT VOID =>ticket=" + CurrentTicket.SalesID + ",transid=" + m_payment.ResponseId + ", amount=" + m_payment.Amount);
                status = resp.respstat;
            }




            if (status == "A")
            {
                //update payment
                CurrentTicket.VoidPayment(m_payment.ID);
                CurrentTicket.DeleteGratuity();

                Payment payment = m_salesmodel.GetPayment(m_payment.ResponseId);
                //print debit slip
                ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket, payment);

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

                    if (amt == 0) return;

                    decimal originalTicketAmount = CurrentTicket.Payments.Sum(x => x.NetAmount);


                    if (amt > originalTicketAmount)
                    {
                        TouchMessageBox.Show("Amount is greater than original ticket amount!!!");
                        return;
                    }


                    logger.Info("COMMAND:Credit Refund , Amount=" + amountobj.ToString());

                    Payment pay = CurrentTicket.Payments.First();

                    string resp = CardConnectModel.ProcessRefund(pay.ResponseId,pay.Amount);

                    if(resp == "Approval")
                    {

                        //update payment
                        CurrentTicket.VoidPayment(m_payment.ID);
                        CurrentTicket.DeleteGratuity();


                        //print credit slip
                        if (GlobalSettings.Instance.PrintCreditSlipOnClose)
                        {
                            Payment payment = m_salesmodel.GetPayment(pay.ResponseId);
                            ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket, payment);
                        }


               
                            //this will run the auto tip logic or bring up tip screen if manual mode
                            if (CurrentTicket != null)
                                CurrentTicket.SplitTips();

                    

                    }


                    m_parent.Close();


                }
                else TouchMessageBox.Show("Please Enter Refund Amount");

            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                TouchMessageBox.Show("Refund Clicked: " + e.Message);
            }
        }
    }
}
