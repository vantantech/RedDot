using RedDot.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace RedDot
{
    public class ReceiptPrinterModel
    {

        private DBSettings m_dbsettings;
    

        public ReceiptPrinterModel()
        {

            m_dbsettings = new DBSettings();
       
        }

        public DataTable GetPrinterLocations()
        {
            return m_dbsettings.GetPrinterLocations();
        }

        public DataRow GetPrinter(int id)
        {
            return m_dbsettings.GetPrinter(id);
        }

        public static RedDotPrinter GetPrinterObject(int id)
        {
            RedDotPrinter prntr = new RedDotPrinter();
            DBSettings m_dbsettings = new DBSettings();
            DataRow selected =  m_dbsettings.GetPrinter(id);
            if (selected != null)
            {
                prntr.Description = selected["description"].ToString();
                prntr.AssignedPrinter = selected["assignedprinter"].ToString();
                prntr.id = (int)selected["id"];
                prntr.PrintMode = selected["printermode"].ToString();
                prntr.IsLabel = selected["islabel"].ToString() == "1" ? true : false;
                prntr.LandScape= selected["landscape"].ToString() == "1" ? true : false;


                if (selected["receiptwidth"].ToString() != "")
                    prntr.ReceiptWidth= int.Parse(selected["receiptwidth"].ToString());
                else prntr.ReceiptWidth= 80;
            }

            return prntr;

        }
        public bool AddPrinterLocations(string description, string assignedprinter)
        {
            return m_dbsettings.AddPrinterLocation(description, assignedprinter);
        }


        public bool UpdatePrinterLocation_AssignedPrinter(RedDotPrinter selected)
        {
            return m_dbsettings.UpdatePrinterLocation_AssignedPrinter(selected.id,selected.AssignedPrinter, selected.Description, selected.PrintMode, selected.ReceiptWidth, selected.IsLabel, selected.LandScape);
        }

      

        public bool DeletePrinterLocation(int id)
        {
            return m_dbsettings.DeletePrinterLocation(id);

        }




        public static void PrintReceipt(Ticket m_currentticket, string printername)
        {


            string receiptline;
 
            string m_mode = GlobalSettings.Instance.ReceiptPrinterMode;
            Location store = GlobalSettings.Instance.Shop;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;

            //defaults to receipt width since some customer had the character width setup.
            int receiptchars = receiptwidth;
        


            //translate to chars if value is in millimeters
            //58mm printer = 32 chars  , 80mm printer = 48 chars
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;

            int doubleheightwidth = (int)Math.Round(receiptchars * 0.65, 0);


            if (printername == "none") return;


            //58mm printer = 32 chars  , 80mm printer = 48 chars
            ReceiptPrinter printer = new ReceiptPrinter(printername,m_mode);

            try
            {

                if (store == null)
                {

                    MessageBox.Show("Shop/store info missing");
                    return;
                }



                printer.Center();
                if (GlobalSettings.Instance.Demo)
                    printer.PrintLF("*** DEMO SOFTWARE ***");
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.PrintLF(new String('=', receiptchars));
                printer.Center();
                printer.DoubleHeight();
                printer.PrintLF("*** " + m_currentticket.OrderTypeStr + " ****");
                printer.DoubleHeightOFF();

                printer.PrintLF(new String('=', receiptchars));


                printer.DoubleHeight();
                printer.Center();
                if (m_currentticket.TableNumber > 0)
                    printer.PrintLF(Utility.FormatPrintRow("Order #" + String.Format("{0:D3}", m_currentticket.OrderNumber), "Table:" + m_currentticket.TableNumber.ToString(), receiptchars / 2));
                else
                    printer.PrintLF("Order #" + String.Format("{0:D3}", m_currentticket.OrderNumber));

                printer.DoubleHeightOFF();

                printer.PrintLF(new String('=', receiptchars));

                printer.Left();

                printer.PrintLF(Utility.FormatPrintRow("Server: " + m_currentticket.CurrentEmployee.DisplayName, m_currentticket.SaleDate.ToShortDateString() + " " + m_currentticket.SaleDate.ToShortTimeString(), receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("Station: " + m_currentticket.StationNo,  "No of Guest: " + m_currentticket.CustomerCount, receiptchars));

                printer.Left();

           
                printer.PrintLF(new String('-', receiptchars));




                //starts to print each item on ticket

   
                bool showdetails = GlobalSettings.Instance.ReceiptShowModifierPrice;

                foreach (Seat seat in m_currentticket.Seats)
                    foreach (LineItem line in seat.LineItems)
                    {
                        //wether or not to include the modifier prices in the lineitem pricing
                       
                         receiptline = Utility.FormatPrintRow(DoFormat(line.Quantity) + "x " + line.Description + " " + line.WeightStr, line.Voided ? "(VOIDED)" : String.Format("{0:0.00}", (line.PriceWithModifiers) * line.Quantity), receiptchars);
                        printer.PrintLF(receiptline);

                        if (line.Discount > 0 && !line.Voided)
                        {

                            receiptline = Utility.FormatPrintRow(" **" + line.DiscountName + "**", String.Format("-{0:0.00}", line.Discount), receiptchars);
                            printer.PrintLF(receiptline);
                        }

                        if (line.Note != "")
                        {
                            printer.PrintLF(" ***" + line.Note + "***");
                        }

                        foreach (SalesModifier mod in line.Modifiers)
                        {
                            if(mod.Quantifiable)
                            {
                                if (showdetails) receiptline = Utility.FormatPrintRow(" * " + DoFormat(mod.Quantity) + "x " + mod.Description, String.Format("+({0:0.00})", mod.TotalPrice), receiptchars - 10);
                                else receiptline = " * " + DoFormat(mod.Quantity) + "x " + mod.Description;
                            }else
                            {
                               receiptline = " * "  + mod.Description;
                            }
                          

                            printer.PrintLF(receiptline);


                        }


                        //if it's a combo .. then need to list item under combo
                        foreach(LineItem comboline in line.LineItems)
                        {
                            if (showdetails)  receiptline = Utility.FormatPrintRow(" + " + DoFormat(comboline.Quantity) + "x " + comboline.Description + " " + line.WeightStr, " " + String.Format("+({0:0.00})", comboline.ComboPrice), receiptchars-10);
                            else receiptline = " + " + DoFormat(comboline.Quantity) + "x " + comboline.Description;
                            printer.PrintLF(receiptline);

                            foreach (SalesModifier mod in comboline.Modifiers)
                            {
                                if (showdetails) receiptline = Utility.FormatPrintRow(" *" + DoFormat(mod.Quantity) + "x " + mod.Description, String.Format("+({0:0.00})", mod.TotalPrice), receiptchars-10);
                                else receiptline = "      *" + DoFormat(mod.Quantity) + "x "+ mod.Description;

                                printer.PrintLF(receiptline);


                            }
                        }


                    }

                printer.PrintLF(new String('-', receiptchars));
        

                printer.PrintLF(Utility.FormatPrintRow("SUBTOTAL:", String.Format("{0:0.00}", m_currentticket.SubTotal), receiptchars));


                if (m_currentticket.Discount != 0) printer.PrintLF(Utility.FormatPrintRow("DISCOUNT:", String.Format("{0:0.00}", m_currentticket.Discount), receiptchars));
               
                if (m_currentticket.SalesTax > 0) printer.PrintLF(Utility.FormatPrintRow("SALES TAX:", String.Format("{0:0.00}", m_currentticket.SalesTax), receiptchars));

              
                if (m_currentticket.AutoTipAmount > 0) printer.PrintLF(Utility.FormatPrintRow(GlobalSettings.Instance.AutoTipPercent.ToString() +  "% GRATUITY:", String.Format("{0:0.00}", m_currentticket.AutoTipAmount), receiptchars));
                // printer.PrintLF(Utility.FormatPrintRow("SERVICE:", String.Format("{0:0.00}", LaborTotal), receiptwidth));



                printer.PrintLF(Utility.FormatPrintRow("", "============", receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("TOTAL:", String.Format("{0:0.00}", m_currentticket.Total), receiptchars));

                printer.LineFeed();


                bool settled = true;
                foreach (Payment line in m_currentticket.Payments)
                {
                  switch(line.CardGroup.ToUpper())
                    {
                        case "CREDIT":
                            receiptline = Utility.FormatPrintRow(line.CardType + (line.Voided ? " (VOIDED)" : " " + line.AuthorCode), line.AmountStr, receiptchars);
                            printer.PrintLF(receiptline);
                            break;

                        case "GIFT CARD":
                            receiptline = Utility.FormatPrintRow(line.CardGroup + (line.Voided ? " (VOIDED)" : " " + line.AuthorCode), line.AmountStr, receiptchars);
                            printer.PrintLF(receiptline);
                            printer.PrintLF("< Card Balance: " + GiftCardModel.GetGiftCardBalance(line.AuthorCode) + " >");
                            break;
                        case "GIFT CERTIFICATE":
                            receiptline = Utility.FormatPrintRow(line.CardGroup + (line.Voided ? " (VOIDED)" : " " + line.AuthorCode), line.AmountStr, receiptchars);
                            printer.PrintLF(receiptline);
                            printer.PrintLF("< Certifcate Balance: " + GiftCardModel.GetGiftCardBalance(line.AuthorCode) + " >");
                            break;

                        default:

                            receiptline = Utility.FormatPrintRow(line.CardGroup + (line.Voided ? " (VOIDED)" : " "), line.AmountStr, receiptchars);
                            printer.PrintLF(receiptline);
                            break;
                    }
           
                    

                 
                    if (line.TransType.ToUpper() == "AUTH") settled = false;

                }



                printer.PrintLF(Utility.FormatPrintRow("", "============", receiptchars));

                printer.DoubleHeight();
              
                    printer.PrintLF(Utility.FormatPrintRow("BALANCE:", String.Format("{0:0.00}", Math.Round(m_currentticket.Balance, 2)), doubleheightwidth));
               
               if(m_currentticket.ChangeDue > 0)
                {
                    printer.PrintLF(Utility.FormatPrintRow("CASH TENDERED:", String.Format("{0:0.00}", Math.Round(m_currentticket.CashPayment, 2)), doubleheightwidth));
                    printer.PrintLF(Utility.FormatPrintRow("CHANGE:", String.Format("{0:0.00}", Math.Round(m_currentticket.ChangeDue , 2)), doubleheightwidth));
                }

                printer.DoubleHeightOFF();

                printer.LineFeed();

                if (GlobalSettings.Instance.PrintTipOnReceipt)
                    if (m_currentticket.TipAmount > 0)
                        printer.PrintLF(Utility.FormatPrintRow("TIP:", String.Format("{0:0.00}", m_currentticket.TipAmount), receiptchars));


                if (m_currentticket.CreditCardSurcharge > 0)
                {

                    printer.PrintLF(Utility.FormatPrintRow("CC Surcharge:", String.Format("{0:0.00}", m_currentticket.CreditCardSurcharge), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Adj. Payment:", String.Format("{0:0.00}", m_currentticket.AdjustedPayment), receiptchars));

                }

                //Payment notice

                printer.PrintLF(GlobalSettings.Instance.PaymentNotice);

                //Receipt notice
                printer.PrintLF(GlobalSettings.Instance.ReceiptNotice);


                if (GlobalSettings.Instance.ReceiptPrintReward)
                {
                    if (m_currentticket.CurrentCustomer != null)
                    {
                        if (m_currentticket.CurrentCustomer.ID > 0)
                        {
                            printer.LineFeed();
                            if (m_currentticket.CurrentCustomer.Phone1.Length > 4) printer.PrintLF(Utility.FormatPrintRow("Customer:", m_currentticket.CurrentCustomer.ID.ToString() + ":" + m_currentticket.CurrentCustomer.Phone1.Substring(m_currentticket.CurrentCustomer.Phone1.Length - 4, 4), receiptchars));
                            printer.PrintLF(Utility.FormatPrintRow("REWARD BALANCE:", String.Format("{0:0.00}", Math.Round(m_currentticket.CurrentCustomer.RewardBalance, 2)), receiptchars));
                        }


                    }


                }

                printer.Center();

                if (m_currentticket.Status == "Closed" && settled)
                {
                  
                    printer.PrintLF("** ORDER SETTLED  **");
                    printer.PrintLF( m_currentticket.CloseDate.ToShortDateString() + " " + m_currentticket.CloseDate.ToShortTimeString());

                }

                printer.PrintLF("THANK YOU!");
            

                if(GlobalSettings.Instance.Demo)
                {
                    printer.PrintLF("*** DEMO SOFTWARE ***");
                    printer.PrintLF("This software is for DEMO purpose ONLY.  It is illegal to use in a commercial environment. Please visit http:///www.reddotpos.com for more information on how to purchase a license.");
                }
                   
              
           
        
            

                printer.PrintLF("Receipt #:" + String.Format("{0:D5}", m_currentticket.SalesID));
                if (GlobalSettings.Instance.PrintBarCode) printer.PrintBarCode("TKT" + m_currentticket.SalesID.ToString());

                printer.LineFeed();
                printer.Send(); //sends buffer to printer

                printer.Cut();

            }
            catch (Exception ex)
            {
                MessageBox.Show("PrintReceipt:" + ex.Message);
            }

        }

 

        public static void AutoPrintCreditSlip( Payment m_payment)
        {
            ReceiptPrinterModel.PrintCreditSlip( m_payment, "**Merchant Copy**");
            if (GlobalSettings.Instance.PrintCustomerCopy) ReceiptPrinterModel.PrintCreditSlip( m_payment, "**Customer Copy**");
        }

        public static void PrintCreditSlip( Payment payment, string CopyString)
        {



            if (payment == null) return;




            string printername = GlobalSettings.Instance.ReceiptPrinter;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
            string m_mode = GlobalSettings.Instance.ReceiptPrinterMode;
            Location store = GlobalSettings.Instance.Shop;
            bool printemployee = GlobalSettings.Instance.ReceiptPrintEmployee;




            if (printername == "none") return;

            //defaults to receipt width since some customer had the character width setup.
            int receiptchars = receiptwidth;
    
            //translate to chars if value is in millimeters
            //58mm printer = 32 chars  , 80mm printer = 48 chars
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;

            int doubleheightwidth = (int)Math.Round(receiptchars * 0.65, 0);


            ReceiptPrinter printer = new ReceiptPrinter(printername,m_mode);

            try
            {

                if (store == null)
                {

                    TouchMessageBox.Show("Shop/store info missing");
                    return;
                }



                printer.Center();
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();



                //  printer.PrintLF(new String('=', receiptchars));


                printer.DoubleHeight();

                printer.PrintLF(payment.CardGroup.ToUpper() + " " + payment.TransType.ToUpper());
                printer.PrintLF("Ticket: " + payment.SalesID.ToString());
                printer.DoubleHeightOFF();


                printer.Left();

                if (payment.Voided)
                    printer.PrintLF(Utility.FormatPrintRow(payment.VoidDate.ToShortDateString(), payment.VoidDate.ToShortTimeString(), receiptchars));
                else
                    printer.PrintLF(Utility.FormatPrintRow(payment.PaymentDate.ToShortDateString(), payment.PaymentDate.ToShortTimeString(), receiptchars));




                printer.LineFeed();

                printer.PrintLF(new String('-', receiptchars));
                if (payment.CardAcquisition == "INSERT" || payment.CardAcquisition == "EMV TAP" || payment.CardAcquisition.ToUpper() == "CHIP" || payment.CardAcquisition == "CONTACTICC")
                {
                    printer.PrintLF(Utility.FormatPrintRow("Acct:", payment.MaskedPAN, receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("App Name:", payment.EMV_ApplicationName, receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("AID:", payment.EMV_AID, receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow(payment.EMV_CryptogramType + ":", payment.EMV_Cryptogram, receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Entry:", payment.CardAcquisition, receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Approval:", payment.AuthorCode, receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Card Holder:", payment.CardHolderName, receiptchars));


                    if (payment.PinVerified == false) printer.PrintLF(Utility.FormatPrintRow("PIN:", "PIN not provided", receiptchars));
                    else printer.PrintLF(Utility.FormatPrintRow("PIN:", "Verified", receiptchars));
                }
                else
                {
                    printer.PrintLF(Utility.FormatPrintRow("Card Type:", payment.CardType, receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Acct:", payment.MaskedPAN, receiptchars));

                    printer.PrintLF(Utility.FormatPrintRow("Entry:", payment.CardAcquisition, receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Approval:", payment.AuthorCode, receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Card Holder:", payment.CardHolderName, receiptchars));

                 

                }

                printer.PrintLF(Utility.FormatPrintRow("Transaction ID:", payment.ResponseId, receiptchars));

                printer.LineFeed();

                printer.PrintLF(Utility.FormatPrintRow("Ticket Total:", String.Format("{0:0.00}", payment.NetAmount), receiptchars));


                if ( payment.TransType.ToUpper() == "POSTAUTH" || payment.TransType.ToUpper() == "SALE")
                {
                   
                        printer.PrintLF(Utility.FormatPrintRow("Tip:", String.Format("{0:0.00}", payment.TipAmount), receiptchars));
                        printer.DoubleHeight();
                        printer.PrintLF(Utility.FormatPrintRow("Total w/Tip:", String.Format("{0:0.00}", payment.Amount), doubleheightwidth));
                        printer.DoubleHeightOFF();
                 
                }

                if (payment.TransType.ToUpper() == "AUTH" )
                {
                    printer.LineFeed();
                    printer.PrintLF(Utility.FormatPrintRow("Tip   :", " _________________", receiptchars));
                    printer.LineFeed();
                    printer.PrintLF(Utility.FormatPrintRow("Total :", " _________________", receiptchars));

                    printer.LineFeed();
                    printer.PrintLF(new String('-', receiptchars));

                    if (GlobalSettings.Instance.PrintTipGuide && payment.CardGroup.ToUpper() == "CREDIT")
                    {
                        printer.PrintLF("20%  = " + String.Format("{0:0.00}", payment.NetAmount * 0.2m) + "    " +
                                        "18%  = " + String.Format("{0:0.00}", payment.NetAmount * 0.18m) + "    " +
                                        "15%  = " + String.Format("{0:0.00}", payment.NetAmount * 0.15m));
                    }

                }







                printer.Center();


                if (payment.Voided)
                {
                    printer.DoubleHeight();

                    printer.PrintLF("****VOIDED****");
                    printer.DoubleHeightOFF();
                }
                else
                {


                    if (payment.SignatureLine)
                    {
                        printer.LineFeed();
                        printer.PrintLF("I agree to pay above total amount according to  card issuer agreement.");

                        printer.Center();
                        printer.LineFeed();
                        printer.LineFeed();
                        printer.PrintLF("X" + new String('_', receiptchars - 5));
                        printer.PrintLF("SIGNATURE");

                    }

                    printer.LineFeed();
                    printer.DoubleHeight();

                    printer.PrintLF("APPROVED");
                    printer.DoubleHeightOFF();

                }



                printer.Center();
                printer.DoubleHeight();
                printer.PrintLF(CopyString);
                printer.DoubleHeightOFF();

                printer.Center();
                printer.LineFeed();
                printer.PrintLF("THANK YOU! COME BACK AGAIN");
                printer.LineFeed();
                printer.Send(); //sends buffer to printer

                if (GlobalSettings.Instance.PrintBarCode) printer.PrintBarCode("PAY" + payment.ID.ToString());


                printer.Cut();

            }
            catch (Exception ex)
            {
                MessageBox.Show("PrintCreditSlip:" + ex.Message);
            }

        }


        public static void PrintPackagingReceipt(Ticket m_currentticket, bool printcopy = false )
        {

            //check to see if there is new items to print or it's a copy
            if (printcopy == false && m_currentticket.AllFired) return;


            string printername = GlobalSettings.Instance.PackagingPrinter;
            if (printername == "none") return;


            string receiptline;
            string m_mode = GlobalSettings.Instance.ReceiptPrinterMode;
            Location store = GlobalSettings.Instance.Shop;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;

   


            //defaults to receipt width since some customer had the character width setup.
            int receiptchars = receiptwidth;
            //translate to chars if value is in millimeters
            //58mm printer = 32 chars  , 80mm printer = 48 chars
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;

       

            int doubleheightwidth = (int)Math.Round(receiptchars * 0.65, 0);

            //58mm printer = 32 chars  , 80mm printer = 48 chars
            ReceiptPrinter printer = new ReceiptPrinter(printername, m_mode);

            try
            {

                if (store == null)
                {

                    MessageBox.Show("Shop/store info missing");
                    return;
                }


                printer.Center();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.Center();
                if (GlobalSettings.Instance.Demo)
                    printer.PrintLF("*** DEMO SOFTWARE ***");
                printer.LineFeed();
                printer.DoubleHeight();
                printer.PrintLF("*** PACKAGER RECEIPT ***");

                if (printcopy)
                {
                    printer.PrintLF("*** COPY COPY COPY ***");

                }else
                {
                    //if it has been previously sent
                    if (m_currentticket.SentToKitchen)
                        printer.PrintLF("*** (MODIFIED TICKET) ***");
                }
              

                printer.DoubleHeightOFF();
                printer.Center();
                printer.PrintLF(new String('=', receiptchars));


                printer.DoubleHeight();
                printer.PrintLF("*** " + m_currentticket.OrderTypeStr + " ****");
                printer.DoubleHeightOFF();
                printer.PrintLF(new String('=', receiptchars));

                if(m_currentticket.CustomerName != "")
                {
                    printer.Center();
                    printer.DoubleHeight();
                    printer.PrintLF("*** " + m_currentticket.CustomerName+ " ****");
                    printer.DoubleHeightOFF();
                    printer.PrintLF(new String('=', receiptchars));
                }

                if (m_currentticket.CurrentCustomer != null)
                    if (m_currentticket.CurrentCustomer.FullName != "")
                    {
                        printer.Center();
                        printer.DoubleHeight();
                        printer.PrintLF("*** " + m_currentticket.CurrentCustomer.FullName + " ****");
                       if(m_currentticket.CurrentCustomer.Phone1 != "") printer.PrintLF("*** " + m_currentticket.CurrentCustomer.Phone1 + " ****");
                        printer.DoubleHeightOFF();
                        printer.PrintLF(new String('=', receiptchars));
                    }

                printer.DoubleHeight();

                printer.Center();
                if (m_currentticket.TableNumber > 0)
                    printer.PrintLF(Utility.FormatPrintRow("Order #" + String.Format("{0:D3}", m_currentticket.OrderNumber), "Table:" + m_currentticket.TableNumber.ToString(), doubleheightwidth));
                else
                    printer.PrintLF("Order #" + String.Format("{0:D3}", m_currentticket.OrderNumber));

                printer.DoubleHeightOFF();

                printer.PrintLF(new String('=', receiptchars));

                printer.Left();

                printer.PrintLF(Utility.FormatPrintRow("Server: " + m_currentticket.CurrentEmployee.DisplayName, m_currentticket.SaleDate.ToShortDateString() + " " + m_currentticket.SaleDate.ToShortTimeString(), receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("Station: " + m_currentticket.StationNo, "No of Guest: " + m_currentticket.CustomerCount, receiptchars));

                printer.Left();

                printer.PrintLF(new String('-', receiptchars));

                //this section is for new items so double height


           
                int sentcount = 0;
                foreach (Seat seat in m_currentticket.Seats)
                    foreach (LineItem line in seat.LineItems)
                    {
                        if(line.Sent)
                        {
                            //counts items that already sent for printing in next section
                            sentcount++;
                        }else
                        {
                            //only print unsent items

                            receiptline = Utility.FormatPrintRow(DoFormat(line.Quantity) + "x " + line.WeightStrDescription , String.Format("{0:0.00}", (line.PriceWithModifiers) * line.Quantity), receiptchars );
                            printer.PrintLF(receiptline);

                            if (line.Note != "") printer.PrintLF("**" +line.Note);

                            foreach (SalesModifier mod in line.Modifiers)
                            {
                                if (mod.Quantifiable)
                                    receiptline = "    ** " + DoFormat(mod.Quantity) + "x " + mod.Description;
                                else
                                    receiptline = "    ** " + mod.Description;

                                if (mod.ColorCode == "Red") printer.ColorRed();

                                printer.PrintLF(receiptline);

                                printer.ColorBlack();
                            }
                              

                            //if it's a combo .. then need to list item under combo
                            foreach (LineItem comboline in line.LineItems)
                            {
                                receiptline = Utility.FormatPrintRow(" + " + DoFormat(comboline.Quantity) + "x " + comboline.WeightStrDescription, " " + String.Format("{0:0.00}", comboline.ComboPrice), (int)(receiptchars / 1.5));
                                printer.PrintLF(receiptline);

                                foreach (SalesModifier mod in comboline.Modifiers)
                                {
                                    if (mod.Quantifiable)
                                        receiptline = "    ** " + DoFormat(mod.Quantity) + "x " + mod.Description;
                                    else
                                        receiptline = "    ** " + mod.Description;

                                    if (mod.ColorCode == "Red") printer.ColorRed();

                                    printer.PrintLF(receiptline);

                                    printer.ColorBlack();
                                }
                            }
                        }
                  
                    }



                printer.PrintLF(new String('-', receiptchars));




                //this section will print items that was already on ticket
                if(sentcount > 0)
                {
                    printer.PrintLF("Previous items from same order:");
                    printer.PrintLF(new String('-', receiptchars));

                    foreach (Seat seat in m_currentticket.Seats)
                        foreach (LineItem line in seat.LineItems)
                        {
                            //only print previously sent items
                            if(line.Sent)
                            {
                                printer.PrintLF(DoFormat(line.Quantity) + "x " + line.WeightStrDescription);
                                foreach (SalesModifier mod in line.Modifiers)
                                {
                                    if (mod.Quantifiable)
                                        receiptline = "    ** " + DoFormat(mod.Quantity) + "x " + mod.Description;
                                    else
                                        receiptline = "    ** " + mod.Description;

                                    if (mod.ColorCode == "Red") printer.ColorRed();

                                    printer.PrintLF(receiptline);

                                    printer.ColorBlack();
                                }


                                //if it's a combo .. then need to list item under combo
                                foreach (LineItem comboline in line.LineItems)
                                {
                                    printer.PrintLF(" + " + DoFormat(comboline.Quantity) + "x " + comboline.WeightStrDescription);

                                    foreach (SalesModifier mod in comboline.Modifiers)
                                    {
                                        if (mod.Quantifiable)
                                            receiptline = "    ** " + DoFormat(mod.Quantity) + "x " + mod.Description;
                                        else
                                            receiptline = "    ** " + mod.Description;

                                        if (mod.ColorCode == "Red") printer.ColorRed();

                                        printer.PrintLF(receiptline);

                                        printer.ColorBlack();
                                    }
                                }
                            }
                          
                        }

                    printer.PrintLF(new String('-', receiptchars));

                }


                    //TOTALS 
                printer.PrintLF(Utility.FormatPrintRow("SUBTOTAL:", String.Format("{0:0.00}", m_currentticket.SubTotal), receiptchars));


                if (m_currentticket.Discount != 0) printer.PrintLF(Utility.FormatPrintRow("DISCOUNT:", String.Format("{0:0.00}", m_currentticket.Discount), receiptchars));

                if (m_currentticket.SalesTax > 0) printer.PrintLF(Utility.FormatPrintRow("SALES TAX:", String.Format("{0:0.00}", m_currentticket.SalesTax), receiptchars));

              
                printer.PrintLF(Utility.FormatPrintRow("", "============", receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("TOTAL:", String.Format("{0:0.00}", m_currentticket.Total), receiptchars));

                printer.LineFeed();




                printer.Center();

                if (m_currentticket.Status == "Closed" )
                {
                    printer.PrintLF("** ORDER SETTLED  **");
                    printer.PrintLF(m_currentticket.CloseDate.ToShortDateString() + " " + m_currentticket.CloseDate.ToShortTimeString());
                }



                if (GlobalSettings.Instance.Demo)
                {
                    printer.PrintLF("*** DEMO SOFTWARE ***");
                    printer.PrintLF("This software is for DEMO purpose ONLY.  It is illegal to use in a commercial environment. Please visit http:///www.reddotpos.com for more information on how to purchase a license.");
                }


                printer.PrintLF("Receipt #:" + String.Format("{0:D5}", m_currentticket.SalesID));
                if (GlobalSettings.Instance.PrintBarCode) printer.PrintBarCode("TKT" + m_currentticket.SalesID.ToString());

                printer.LineFeed();
                printer.Send(); //sends buffer to printer

                printer.Cut();

            }
            catch (Exception ex)
            {
                MessageBox.Show("PrintReceipt:" + ex.Message);
            }

        }



        public static void PrintGiftCard(DataTable dt)
        {

            string printername = GlobalSettings.Instance.ReceiptPrinter;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
            Location store = GlobalSettings.Instance.Shop;
            bool printemployee = GlobalSettings.Instance.ReceiptPrintEmployee;
            string m_mode = GlobalSettings.Instance.ReceiptPrinterMode;

            decimal amount = 0;
            decimal spent = 0;
            decimal balance = 0;

            if (printername == "none") return;

            //defaults to receipt width since some customer had the character width setup.
            int receiptchars = receiptwidth;
            //translate to chars if value is in millimeters
            //58mm printer = 32 chars  , 80mm printer = 48 chars
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;


            ReceiptPrinter printer = new ReceiptPrinter(printername,m_mode);


            try
            {

                if (store == null)
                {

                    MessageBox.Show("Shop/store info missing");
                    return;
                }



                printer.Center();
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.DoubleHeight();
                printer.PrintLF("Gift Card Summary");

                printer.DoubleHeightOFF();
                printer.LineFeed();
                printer.Left();

                printer.PrintLF(new String('=', receiptchars));

                printer.PrintLF("___Type__|__Account No___|_Amount|_Spent|Balance");


                foreach (DataRow row in dt.Rows)
                {
                    string cardtype;
                    if(row["cardtype"].ToString().Length >=9)
                        cardtype= row["cardtype"].ToString().Substring(0, 9);
                    else cardtype = row["cardtype"].ToString();

                    printer.PrintLF(cardtype.PadLeft(9, ' ') + row["accountnumber"].ToString().PadLeft(16, ' ') + Math.Round(decimal.Parse(row["amount"].ToString()), 2).ToString().PadLeft(7, ' ') + Math.Round(decimal.Parse(row["spent"].ToString()), 2).ToString().PadLeft(7, ' ') + Math.Round(decimal.Parse(row["balance"].ToString()), 2).ToString().PadLeft(7, ' '));
                    amount = amount + Math.Round(decimal.Parse(row["amount"].ToString()), 2);
                    spent = spent + Math.Round(decimal.Parse(row["spent"].ToString()), 2);
                    balance = balance + Math.Round(decimal.Parse(row["balance"].ToString()), 2);

                }

                printer.LineFeed();
                printer.PrintLF(new String('=', receiptchars));

                printer.LineFeed();
                printer.PrintLF(" Total Amount:" + Math.Round(decimal.Parse(amount.ToString()), 2).ToString().PadLeft(15, ' '));
                printer.PrintLF("  Total Spent:" + Math.Round(decimal.Parse(spent.ToString()), 2).ToString().PadLeft(15, ' '));
                printer.PrintLF("Total Balance:" + Math.Round(decimal.Parse(balance.ToString()), 2).ToString().PadLeft(15, ' '));

                printer.LineFeed();

                printer.Send();
                printer.Cut();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Print Gift Card Summary:" + ex.Message);
            }
        }




        public static void PrintEOD(string title, string printstring)
        {

            string printername = GlobalSettings.Instance.ReceiptPrinter;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
            Location store = GlobalSettings.Instance.Shop;
            bool printemployee = GlobalSettings.Instance.ReceiptPrintEmployee;
            string m_mode = GlobalSettings.Instance.ReceiptPrinterMode;



            if (printername == "none") return;

            //defaults to receipt width since some customer had the character width setup.
            int receiptchars = receiptwidth;
            //translate to chars if value is in millimeters
            //58mm printer = 32 chars  , 80mm printer = 48 chars
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;


            ReceiptPrinter printer = new ReceiptPrinter(printername,m_mode);


            try
            {

                if (store == null)
                {

                    MessageBox.Show("Shop/store info missing");
                    return;
                }



                printer.Center();
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.DoubleHeight();
                printer.PrintLF(title);

                printer.DoubleHeightOFF();
                printer.LineFeed();
                printer.Left();

                printer.PrintLF(new String('=', receiptchars));



                var stringarray = printstring.Split((char)13);

                foreach (String str in stringarray)
                {
                    printer.PrintLF(str);

                }

                printer.LineFeed();
                printer.PrintLF(new String('=', receiptchars));



                printer.LineFeed();

                printer.Send();
                printer.Cut();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Print Gift Card Summary:" + ex.Message);
            }
        }

        public static void TestPrint(RedDotPrinter Selected)
        {
            if (Selected.PrintMode.ToUpper() == "LABEL" || Selected.PrintMode.ToUpper() == "ZEBRA")
                TestPrintLabel(Selected);
            else
                TestPrintReceipt(Selected.Description, Selected.PrintMode, Selected.ReceiptWidth);
        }


        public static void TestPrintLabel(RedDotPrinter selected)
        {
            PrintDocument pdoc = null;
            //System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();

        
            pdoc = new PrintDocument();

            try
            {
                pdoc.PrinterSettings.PrinterName = selected.AssignedPrinter;

                if (selected.PrintMode == "LABEL")
                    pdoc.DefaultPageSettings.Landscape = selected.LandScape;


                // pdoc.DefaultPageSettings.Landscape = false;
                pdoc.PrintPage += (sender, e) => pdoc_TestPrintLabel(e);  //this method allows you to pass parameters
                pdoc.Print();

            }catch(Exception ex)
            {
                TouchMessageBox.Show("TestPrintLabel:" + ex.Message);
            }



        }

        public static void pdoc_TestPrintLabel(PrintPageEventArgs e)
        {
       
            

            int largefont = 15;
            int normalfont = 12;
            int smallfont = 10;

            Graphics graphics = e.Graphics;
          
            Font font = new Font("Courier New", smallfont, System.Drawing.FontStyle.Bold);
            Font fontbold = new Font("Courier New", normalfont, System.Drawing.FontStyle.Bold);
            Font fontgiant = new Font("Courier New", largefont, System.Drawing.FontStyle.Bold);
            // Create pen.
            Pen blackPen = new Pen(Color.Black, 2);
            Brush blackBrush = new SolidBrush(Color.Black);
            int fontHeight = (int)font.GetHeight();
            int fontBoldHeight = (int)fontbold.GetHeight();
            int fontGiantHeight = (int)fontgiant.GetHeight();
            int y =-5;





            graphics.DrawString("ORDER #001", fontgiant, blackBrush, 0, 0);

            y += fontGiantHeight;
   
            graphics.DrawString("MILK TEA 24 oz", fontbold, blackBrush, 0, y);

            y +=  fontBoldHeight;

         
     
            graphics.DrawString("+ " + "Passion Fruit Flavor", font, blackBrush, 0, y);
            y += fontHeight;
            graphics.DrawString("+ " + "Banana Flavor", font, blackBrush, 0, y);
            y += fontHeight;
            graphics.DrawString("+ " + "Honey Boba", font, blackBrush, 0, y);
            y += fontHeight;
            graphics.DrawString("+ " + "Coffee Jelly", font, blackBrush, 0, y);

        }
        public static void TestPrintReceipt(string printername, string m_mode, int receiptwidth)
        {
           
           // int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
            Location store = GlobalSettings.Instance.Shop;
          //  string m_mode = GlobalSettings.Instance.ReceiptPrinterMode;




            if (printername == "none") return;

            //defaults to receipt width since some customer had the character width setup.
            int receiptchars = receiptwidth;
            //translate to chars if value is in millimeters
            //58mm printer = 32 chars  , 80mm printer = 48 chars
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;
            if (receiptwidth == 70) receiptchars = 42;

            ReceiptPrinter printer = new ReceiptPrinter(printername,m_mode);


            try
            {

                if (store == null)
                {

                    TouchMessageBox.Show("Shop/store info missing");
                    return;
                }



                printer.Center();
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.DoubleHeight();
                printer.PrintLF("Printer Test");

                printer.DoubleHeightOFF();
                printer.LineFeed();
                printer.Left();

                printer.PrintLF(new String('=', receiptchars));
                printer.PrintLF(Utility.FormatPrintRow(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), receiptchars));


                printer.ColorBlack();
                    printer.PrintLF("This is a test print - Black");

                printer.ColorRed();
                    printer.PrintLF("This is a test print - Red/inverted");

                printer.ColorBlack();
                printer.LineFeed();
                printer.PrintLF(new String('=', receiptchars));



                printer.LineFeed();

                printer.Send();
                printer.Cut();


            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("Print Test:" + ex.Message);
            }
        }


        public static void PrintResponse(string title, string printstring)
        {

            string printername = GlobalSettings.Instance.ReceiptPrinter;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
            Location store = GlobalSettings.Instance.Shop;
            bool printemployee = GlobalSettings.Instance.ReceiptPrintEmployee;
            string m_mode = GlobalSettings.Instance.ReceiptPrinterMode;



            if (printername == "none") return;

            //defaults to receipt width since some customer had the character width setup.
            int receiptchars = receiptwidth;
            //translate to chars if value is in millimeters
            //58mm printer = 32 chars  , 80mm printer = 48 chars
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;


            ReceiptPrinter printer = new ReceiptPrinter(printername,m_mode);


            try
            {

                if (store == null)
                {

                    TouchMessageBox.Show("Shop/store info missing");
                    return;
                }



                printer.Center();
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.DoubleHeight();
                printer.PrintLF(title);

                printer.DoubleHeightOFF();
                printer.LineFeed();
                printer.Left();

                printer.PrintLF(new String('=', receiptchars));
                printer.PrintLF(Utility.FormatPrintRow(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), receiptchars));


                var stringarray = printstring.Split((char)13);

                foreach (String str in stringarray)
                {
                    printer.PrintLF(str);

                }

                printer.LineFeed();
                printer.PrintLF(new String('=', receiptchars));



                printer.LineFeed();

                printer.Send();
                printer.Cut();


            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("Print REsponse:" + ex.Message);
            }
        }


        public static void SendKitchen(Ticket ticket,bool silent=false)
        {

            //Need to assign Order number before printing to packaging
            //first thing .. need to assign order number before printing

            ticket.AssignOrderNumber();  //will not assign if already has a number

            if (GlobalSettings.Instance.AutoPackagingReceiptPrint) ReceiptPrinterModel.PrintPackagingReceipt(ticket);

            bool success =   PrintToKitchen(ticket);
            if (success && !silent) TouchMessageBox.ShowSmall("Ticket Sent", 2);

            ticket.MarkAsSent(); //Mark all item on ticket as being sent no matter what
         
          
        }


        public static  bool PrintToKitchen(Ticket ticket)
        {
           



            DBTicket m_dbTicket;
            m_dbTicket = new DBTicket();
            bool doubleheight = GlobalSettings.Instance.KitchenPrintDoubleHeight;
            bool doublewidth = GlobalSettings.Instance.KitchenPrintDoubleWidth;
            bool doubleimpact = GlobalSettings.Instance.KitchenPrintDoubleImpact;
      

            try
            {


                bool printed = false;

                DataTable printers = m_dbTicket.GetPrinters();

                ObservableCollection<LineItem> lineitems;
                ObservableCollection<LineItem> collaboration_lineitems;
                LineItem line;

                foreach(DataRow prn in printers.Rows)
                {
                    int printerid = (int)prn["id"];
                    RedDotPrinter SelectedPrinter = GetPrinterObject(printerid);
                

                    lineitems = new ObservableCollection<LineItem>();


                    //---------------Print Unsent Items to correct Kitchen printer -------------------------------------
                    DataTable kitchenitems = m_dbTicket.GetUnsentKitchenItems(ticket.SalesID,printerid);
                    foreach (DataRow row in kitchenitems.Rows)
                    {
                        line = new LineItem(row);

                        //there should not be any voided item that is unsent but because of tablet orders, this can happen
                        if(line.Voided == false)
                            lineitems.Add(line);
                    }


                    if (lineitems.Count > 0)
                    {
                    
                        if (SelectedPrinter.PrintMode.ToUpper() == "LABEL" || SelectedPrinter.PrintMode.ToUpper() == "ZEBRA")
                            PrintLabelPrinter(ticket.OrderNumber, lineitems, SelectedPrinter);
                        else
                        {
                            collaboration_lineitems = new ObservableCollection<LineItem>(); //reset collection
                            //ticket collaboration section     -- check to see if it's a label printer.  DO NOT Print to label printer
                            if (GlobalSettings.Instance.EnableTicketCollaberation)
                            {

                                DataTable otheritems = m_dbTicket.GetCollaborationKitchenItems(ticket.SalesID, printerid);
                                foreach (DataRow row in otheritems.Rows)
                                {
                                    line = new LineItem(row);
                                    collaboration_lineitems.Add(line);
                                }
                            }

                            PrintItem(ticket, lineitems, collaboration_lineitems, SelectedPrinter.Description, SelectedPrinter.AssignedPrinter, SelectedPrinter.ReceiptWidth, SelectedPrinter.PrintMode, doubleheight, doublewidth, doubleimpact, SelectedPrinter.LandScape);
                        }
                        printed = true;
                    }

                   

                 

                }


                return printed;


            }
            catch (Exception ex)
            {
               
                TouchMessageBox.Show("PrintKitchen:" + ex.Message);
                return false;
            }

        }

        private static void PrintLabelPrinter(int ordernumber,ObservableCollection<LineItem> lineitems,RedDotPrinter selected)
        {
            PrintDocument pdoc = null;
            //System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();

            pdoc = new PrintDocument();
            pdoc.PrinterSettings.PrinterName = selected.AssignedPrinter;
            if (selected.PrintMode == "LABEL")
                pdoc.DefaultPageSettings.Landscape = selected.LandScape;
         


            int count = 0;
            int totalcount = (int)lineitems.Sum(x => x.Quantity);

            foreach (LineItem line in lineitems)
            {
               
                for (int i = 0; i < line.Quantity; i++)
                {
                    count++;
                    pdoc = new PrintDocument();
            
                   
                    pdoc.PrinterSettings.PrinterName = selected.AssignedPrinter;
                    if (selected.PrintMode == "LABEL")
                        pdoc.DefaultPageSettings.Landscape = selected.LandScape;

                    pdoc.PrintPage += (sender, e) => pdoc_PrintLabel(ordernumber, line, count, totalcount, e);
                   
                    pdoc.Print();
                }


               

            }
          
        }

     

        public static void pdoc_PrintLabel(int ordernumber, LineItem line, int number, int totalcount, PrintPageEventArgs e)
        {
            int largefont = 15;
            int normalfont = 12;
            int smallfont = 10;

            Graphics graphics = e.Graphics;

            Font font = new Font("Courier New", smallfont, System.Drawing.FontStyle.Bold);
            Font fontbold = new Font("Courier New", normalfont, System.Drawing.FontStyle.Bold);
            Font fontgiant = new Font("Courier New", largefont, System.Drawing.FontStyle.Bold);
            // Create pen.
            Pen blackPen = new Pen(Color.Black, 2);
            Brush blackBrush = new SolidBrush(Color.Black);
            int fontHeight = (int)font.GetHeight();
            int fontBoldHeight = (int)fontbold.GetHeight();
            int fontGiantHeight = (int)fontgiant.GetHeight();
            int y = 0;





            graphics.DrawString("ORDER #" + String.Format("{0:D3}", ordernumber) + " " + number + "/" + totalcount , fontgiant, blackBrush, 0, 0);

            y += fontGiantHeight;

            if(line.Description.Length > 15)
                graphics.DrawString(line.Description, font, blackBrush, 0, y);
            else
                graphics.DrawString(line.Description, fontbold, blackBrush, 0, y);

            y += fontBoldHeight;


           foreach (SalesModifier mod in line.Modifiers)
            {
              
                graphics.DrawString("+ " + mod.Description, font, blackBrush, 0, y);
                y += fontHeight;
            }

        }

        private static void PrintItem(Ticket ticket,ObservableCollection<LineItem> lineitems, ObservableCollection<LineItem> collaboration_lineitems, string printerlocation, string printername, int receiptwidth, string mode, bool doubleheight, bool doublewidth, bool doubleimpact, bool islabel)
        {
            string receiptline;
            //defaults to receipt width since some customer had the character width setup.
            int receiptchars = receiptwidth;
            //translate to chars if value is in millimeters
            //58mm printer = 32 chars  , 80mm printer = 48 chars
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;

            Location store = GlobalSettings.Instance.Shop;
          

            DateTime currenttime = DateTime.Now;

            DBTicket m_dbTicket;
            m_dbTicket = new DBTicket();


            //58mm printer = 32 chars  , 80mm printer = 48 chars
            ReceiptPrinter printer = new ReceiptPrinter(printername,mode);

            //if not labelformat ,then printer header
            if(!islabel)
            {
                printer.Center();
                printer.LineFeed();
                printer.PrintLF("*** " + printerlocation.ToUpper() + " ***");


                printer.PrintLF(new String('=', receiptchars));


                printer.DoubleHeight();

                if (ticket.TableNumber > 0)
                    printer.PrintLF(Utility.FormatPrintRow("Order #" + String.Format("{0:D3}", ticket.OrderNumber), "Table:" + ticket.TableNumber.ToString(), receiptchars / 2));
                else
                    printer.PrintLF("Order #" + String.Format("{0:D3}", ticket.OrderNumber));


                printer.PrintLF(ticket.OrderTypeStr);

                printer.DoubleHeightOFF();

                printer.PrintLF(new String('=', receiptchars));

                printer.Left();

                printer.PrintLF(Utility.FormatPrintRow("Server:" + ticket.CurrentEmployee.DisplayName, currenttime.ToShortDateString() + " " + currenttime.ToShortTimeString(), receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("Station: " + ticket.StationNo, "No of Guest: " + ticket.CustomerCount, receiptchars));

                printer.Left();

                printer.PrintLF(new String('-', receiptchars));
            }
     


            bool showdetails = GlobalSettings.Instance.ReceiptShowModifierPrice;

            string printchoice = GlobalSettings.Instance.KitchenPrintDescription;


      
           //regular print items for this printer
            foreach (LineItem line in lineitems)
            {
                if(islabel) //if label then print short header
                {
                   
                    printer.DoubleHeightOFF();
                    printer.PrintLF(new String('=', receiptchars));
                    printer.DoubleHeight();
                    printer.Center();
                    printer.PrintLF("Order #" + String.Format("{0:D3}", ticket.OrderNumber));
                    printer.Left();
                    printer.DoubleHeightOFF();
                    printer.PrintLF(new String('=', receiptchars));
                  
                }

                if (doubleheight) printer.DoubleHeight();
                if (doublewidth) printer.DoubleWidth();
                if (doubleimpact) printer.Emphasize();

                switch (printchoice)
                {
                    case "description 1":
                        receiptline = " " + DoFormat(line.Quantity) + "x " + line.Description + " " +  line.WeightStr;
                        printer.PrintLF(receiptline);
                        break;
                    case "description 2":
                        receiptline = " " + DoFormat(line.Quantity) + "x " + line.Description2 + " " + line.WeightStr;
                        printer.PrintLF(receiptline);
                        break;
                    case "description 3":
                        receiptline = " " + DoFormat(line.Quantity) + "x " + line.Description3 + " " + line.WeightStr;
                        printer.PrintLF(receiptline);
                        break;
                }


                foreach (SalesModifier mod in line.Modifiers)
                    {
                    if (mod.Quantifiable)
                        receiptline = "    ** " + DoFormat(mod.Quantity) + "x " + mod.Description;
                    else
                        receiptline = "    ** " + mod.Description;

                    if(mod.ColorCode == "Red") printer.ColorRed();

                    printer.PrintLF(receiptline);

                    printer.ColorBlack();
                }


                printer.ColorBlack();

                if (line.Note != "")
                {
                   printer.PrintLF("**" + line.Note + "**");
                }


                //if it's a combo .. then need to list item under combo
                foreach (LineItem comboline in line.LineItems)
                {
                    receiptline =" + " + DoFormat(comboline.Quantity) + "x " + comboline.Description + " " + line.WeightStr;
                    printer.PrintLF(receiptline);

                    foreach (SalesModifier mod in comboline.Modifiers)
                    {
                        if (mod.Quantifiable)
                            receiptline = "    ** " + DoFormat(mod.Quantity) + "x " + mod.Description;
                        else
                            receiptline = "    ** " + mod.Description;

                        if (mod.ColorCode == "Red") printer.ColorRed();

                        printer.PrintLF(receiptline);

                        printer.ColorBlack();
                    }
                }


                if (islabel)
                {
                    printer.Send(); //sends buffer to printer
                    printer.Cut();
                }
             
            }


            //Collaboration ticket items .. only print if enable
            if(GlobalSettings.Instance.EnableTicketCollaberation)
            {
                 printer.DoubleHeightOFF();
                printer.DoubleWidthOFF();
                printer.EmphasizeOFF();

                //need to print header for collaboration
                printer.PrintLF("             ******   BY OTHERS *********");
                foreach (LineItem line in collaboration_lineitems)
                {
               
                    switch (printchoice)
                    {
                        case "description 1":
                            receiptline =  DoFormat(line.Quantity) + "x " + line.Description + " " + line.WeightStr;
                            printer.PrintLF(receiptline);
                            break;
                        case "description 2":
                            receiptline =  DoFormat(line.Quantity) + "x " + line.Description2 + " " + line.WeightStr;
                            printer.PrintLF(receiptline);
                            break;
                        case "description 3":
                            receiptline =  DoFormat(line.Quantity) + "x " + line.Description3 + " " + line.WeightStr;
                            printer.PrintLF(receiptline);
                            break;
                    }


                    //--------Do we need modifiers???
                    foreach (SalesModifier mod in line.Modifiers)
                    {
                     
                        if (mod.Quantifiable)
                            receiptline = "    ** " + DoFormat(mod.Quantity) + "x " + mod.Description;
                        else
                            receiptline = "    ** " + mod.Description;


                        if (mod.ColorCode == "Red") printer.ColorRed();
                        printer.PrintLF(receiptline);

                        printer.ColorBlack();
                    }


                    printer.ColorBlack();

                    if (line.Note != "")
                    {
                       printer.PrintLF(line.Note);  
                    }

              
                    printer.LineFeed();
                }
            }
     


            if (!islabel)
            {
                printer.DoubleHeightOFF();
                printer.DoubleWidthOFF();
                printer.EmphasizeOFF();
                printer.PrintLF(new String('-', receiptchars));
                printer.LineFeed();
                printer.Send(); //sends buffer to printer

                printer.Cut();
            }
 


        }

        public static string DoFormat(decimal myNumber)
        {
            var s = string.Format("{0:0.00}", myNumber);

            if (s.EndsWith("00"))
            {
                return ((int)myNumber).ToString();
            }
            else
            {
                return s;
            }
        }

        public static void PrintEmployeeHours(Employee employee)
        {

            string printername = GlobalSettings.Instance.ReportPrinter;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
            Location store = GlobalSettings.Instance.Shop;
     
            string m_mode = GlobalSettings.Instance.ReportPrinterMode;



            if (printername == "none") return;

            //defaults to receipt width since some customer had the character width setup.
            int receiptchars = receiptwidth;
            //translate to chars if value is in millimeters
            //58mm printer = 32 chars  , 80mm printer = 48 chars
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;


            ReceiptPrinter printer = new ReceiptPrinter(printername,m_mode);


            try
            {

                if (store == null)
                {

                    TouchMessageBox.Show("Shop/store info missing");
                    return;
                }



                printer.Center();
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.DoubleHeight();
                printer.PrintLF("Employee Time");

                printer.DoubleHeightOFF();
                printer.LineFeed();
                printer.Left();

                printer.PrintLF(new String('=', receiptchars));
                // printer.PrintLF(Utility.FormatPrintRow(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), receiptchars));
                printer.PrintLF("Time In           TimeOut          Hours");
                decimal totalhours = 0;
                decimal time = 0;
               foreach(DataRow row in employee.Hours.Rows)
                {
                    //initialize with empty data
                    DateTime timein=DateTime.MinValue;
                    DateTime timeout=DateTime.MinValue;
                    time = 0;


                    //just incase there is a blank record .. we don't want it to error out
                    if(row["hours"].ToString() != "") time = decimal.Parse(row["hours"].ToString());
                   
                    if(row["timein"].ToString() != "") timein = DateTime.Parse(row["timein"].ToString());
          
                    if (row["timeout"].ToString() != "") timeout = DateTime.Parse(row["timeout"].ToString());

                    printer.PrintLF(timein.ToString("MM/dd hh:mm tt") + " => " + (timeout== DateTime.MinValue ? "              " : timeout.ToString("MM/dd hh:mm tt")) +  " : " + String.Format("{0:0.00}", time).PadLeft(5));


                    if (row["note"].ToString() != "") printer.PrintLF(row["note"].ToString());

                    totalhours += time;

                }

               
                printer.LineFeed();
                printer.PrintLF(new String('=', receiptchars));

                printer.PrintLF("Total Hours: " + String.Format("{0:0.00}", totalhours).PadLeft(23));



                printer.LineFeed();

                printer.Send();
                printer.Cut();


            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("Print Employee Hours:" + ex.Message);
            }
        }


        public static void PrintEmployeeSales(Employee employee, DateTime startdate, DateTime enddate, bool summary=false)
        {

            string printername = GlobalSettings.Instance.ReportPrinter;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
            Location store = GlobalSettings.Instance.Shop;
            string m_mode = GlobalSettings.Instance.ReportPrinterMode;



            if (printername == "none") return;

            //defaults to receipt width since some customer had the character width setup.
            int receiptchars = receiptwidth;
            //translate to chars if value is in millimeters
            //58mm printer = 32 chars  , 80mm printer = 48 chars
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;


            ReceiptPrinter printer = new ReceiptPrinter(printername, m_mode);


            try
            {

                if (store == null)
                {

                    TouchMessageBox.Show("Shop/store info missing");
                    return;
                }



                printer.Center();
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.DoubleHeight();
                if(summary)
                    printer.PrintLF("Employee Sales Summary");
                else
                    printer.PrintLF("Employee Sales Detail");

                printer.PrintLF(employee.FullName);

                printer.PrintLF(startdate.ToShortDateString() + " - " + enddate.ToShortDateString());

                printer.DoubleHeightOFF();
                printer.LineFeed();
                printer.Left();

                printer.PrintLF(new String('=', receiptchars));
                // printer.PrintLF(Utility.FormatPrintRow(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), receiptchars));
                if(summary)
                    printer.PrintLF("Count   Date Total   Cash  Credit Other  Tip");
                else
                    printer.PrintLF("Ticket# Date Total   Cash  Credit Other  Tip");

                int count = 0;
                decimal totalamount = 0;
                decimal tipamount = 0;
                decimal totalcash = 0;
                decimal totalcredit = 0;
                decimal totalother = 0;

                DBEmployee _dbEmployee = new DBEmployee();
                DataTable Sales;

                Sales = _dbEmployee.GetEmployeeSales(employee.ID, startdate, enddate, summary);
         

                foreach (DataRow row in Sales.Rows)
                {

                    //initialize with empty data
                    DateTime saledate = DateTime.Parse(row["saledate"].ToString());
                    decimal total = decimal.Parse(row["total"].ToString());
                    decimal tip = decimal.Parse(row["tipamount"].ToString());
                    decimal cash = decimal.Parse(row["cash"].ToString());
                    decimal credit = decimal.Parse(row["credit"].ToString());
                    decimal other = decimal.Parse(row["other"].ToString());
                    string firstcolumn = "";
                    if (summary)
                    {
                        firstcolumn = row["totalticket"].ToString().PadLeft(6);
                        count += int.Parse(row["totalticket"].ToString());
                    }
                    else
                    {
                        firstcolumn = row["id"].ToString().PadLeft(6);
                        count++;
                    }



                    printer.PrintLF(firstcolumn + " " + saledate.Month + "/" + saledate.Day + " " +  String.Format("{0:0.00}", total).PadLeft(6) + " " + String.Format("{0:0.00}", cash).PadLeft(6) + " " + String.Format("{0:0.00}", credit).PadLeft(6) + " " + String.Format("{0:0.00}", other).PadLeft(5) + " " + String.Format("{0:0.00}",tip).PadLeft(5) );

                    totalamount += total;
                    tipamount += tip;
                    totalcash += cash;
                    totalcredit += credit;
                    totalother += other;

                }


                printer.LineFeed();
                printer.PrintLF(new String('=', receiptchars));

                printer.PrintLF("Ticket Count: " + String.Format("{0}", count).PadLeft(23));
                printer.PrintLF("Total Amount: " + String.Format("{0:0.00}", totalamount).PadLeft(23));
                printer.PrintLF("  Total Cash: " + String.Format("{0:0.00}", totalcash).PadLeft(23));
                printer.PrintLF("Total Credit: " + String.Format("{0:0.00}", totalcredit).PadLeft(23));
                printer.PrintLF(" Total Other: " + String.Format("{0:0.00}", totalother).PadLeft(23));
                printer.PrintLF("   Total Tip: " + String.Format("{0:0.00}", tipamount).PadLeft(23));


                printer.LineFeed();

                printer.Send();
                printer.Cut();


            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("Print Employee Sales:" + ex.Message);
            }
        }


    }
}
