using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RedDot
{
    public class PaymentModel
    {
        SecurityModel m_security;
        public PaymentModel(SecurityModel security)
        {
            m_security = security;
        }

     
        public void PreAuthDelete(Ticket CurrentTicket, int paymentid, Window m_parent)
        {
          
            try
            {
                if (CurrentTicket.Status == "Closed")
                {
                    TouchMessageBox.Show("Ticket is closed.  Can not modify");
                    return;
                }

          


                // can't delete voided payments...
                if (CurrentTicket.PreAuth.Voided) return;


                ConfirmDelete dlg;
                Confirm dlg1 = new Confirm("Void Pre-Auth Card?");
                Utility.OpenModal(m_parent, dlg1);

                if (dlg1.Action == Confirm.OK)
                {

                    switch (GlobalSettings.Instance.CreditCardProcessor)
                    {


                        case "External":

                            CurrentTicket.VoidPayment((int)paymentid, "Pre Auth Removal");
                            //if tip is already assigned to current employee, then need to remove tip first for current employee
                            //passing 0 for employee ID will delete all gratuity for that ticket
                            //this only affects nail salons

                            break;

                        case "PAX_S300":
                        case "HSIP_ISC250":
                        case "VANTIV":



                            CCPPayment tri = new CCPPayment(CurrentTicket, m_security, "VOID",CurrentTicket.PreAuth,""); //all credit card
                            Utility.OpenModal(m_parent, tri);

                            break;
                    }






                }
            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Error deleting line item: " + e.Message);
            }
        }

        public void PaymentDelete(Ticket CurrentTicket, int paymentid, Window m_parent)
        {
            ConfirmDelete dlg;

            try
            {
                if (CurrentTicket.Status == "Closed")
                {
                    TouchMessageBox.Show("Ticket is closed.  Can not modify");
                    return;
                }

                Payment pay = CurrentTicket.GetPaymentLine(paymentid);


                // can't delete voided payments...
                if (pay.Voided) return;
                if (!m_security.WindowNewAccess("VoidPayment")) return;

                TextPad tp = new TextPad("Enter Void Reason", "");
                Utility.OpenModal(m_parent, tp);

                if (tp.ReturnText == "") return;


                switch (pay.CardGroup.ToUpper())
                {
                    case "GIFT CARD":
                        dlg = new ConfirmDelete("Deleting Gift Card from payment will refund charged amount back to gift card. Proceed?");
                        Utility.OpenModal(m_parent, dlg);

                        if (dlg.Action == "Delete")
                            CurrentTicket.VoidPayment((int)paymentid,tp.ReturnText);

                        break;

                    case "CREDIT":


                        Confirm dlg1 = new Confirm("Void " + pay.CardType + " Payment of " + pay.Amount + "?");
                        Utility.OpenModal(m_parent, dlg1);

                        if (dlg1.Action == Confirm.OK)
                        {

                            switch (GlobalSettings.Instance.CreditCardProcessor)
                            {
                                case "HeartSIP":
                                   // CreditPayment ccp = new CreditPayment(m_parent, CurrentTicket, m_ccp, "VOID", pay.ResponseId);
                                  //  Utility.OpenModal(m_parent, ccp);
                                    break;

                                case "External":

                                    CurrentTicket.VoidPayment((int)paymentid,tp.ReturnText);
                                    //if tip is already assigned to current employee, then need to remove tip first for current employee
                                    //passing 0 for employee ID will delete all gratuity for that ticket
                                    //this only affects nail salons

                                    break;

                                case "PAX_S300":
                                case "HSIP_ISC250":
                                case "VANTIV":

                                 
                                 
                                        CCPPayment tri = new CCPPayment(CurrentTicket,m_security, "VOID", pay,tp.ReturnText); //all credit card
                                        Utility.OpenModal(m_parent, tri);

                                    break;


                                case "Clover":

                                    CloverPayment clover = new CloverPayment(CurrentTicket, m_security, "VOID", pay,tp.ReturnText);
                                    Utility.OpenModal(m_parent, clover);

                                    break;
                            }






                        }




                        break;



                    default:

                        dlg = new ConfirmDelete("Delete " + pay.CardGroup + " Payment?");
                        Utility.OpenModal(m_parent, dlg);

                        if (dlg.Action == "Delete")
                            CurrentTicket.VoidPayment((int)paymentid,tp.ReturnText);
                        break;

                }



            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Error deleting line item: " + e.Message);
            }
        }

        public static bool InsertPayment(int salesid, string paytype, decimal amount, decimal netamount, string authorizecode, string cardtype, string maskedpan, decimal tip, DateTime paymentdate, string transtype)
        {
            DBTicket dbticket = new DBTicket();
            int stationno = GlobalSettings.Instance.StationNo;

            return dbticket.DBInsertPayment(stationno,salesid, paytype.ToUpper(), amount, netamount, authorizecode, cardtype.ToUpper(), maskedpan, tip, paymentdate, transtype.ToUpper());
        }


















    }


  
}
