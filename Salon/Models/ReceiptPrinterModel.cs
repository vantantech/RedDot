using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using RedDot.DataManager;

namespace RedDot
{
    public class ReceiptPrinterModel
    {
        public static void PrintStore()
        {
            string printername = GlobalSettings.Instance.ReceiptPrinter;
            Location store = GlobalSettings.Instance.Shop;
            ReceiptPrinter printer = new ReceiptPrinter(printername);


            if (store == null)
            {

                TouchMessageBox.Show("Shop/store info missing");
                return;
            }

            printer.Center();

            printer.PrintLF(store.Name);

            printer.PrintLF(store.Address1);
            if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
            printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
            printer.PrintLF(store.Phone);
            printer.LineFeed();
            printer.Send(); //sends buffer to printer
        }

        public static void PrintReceipt(Ticket CurrentTicket)
        {


            string receiptline;
            string printername = GlobalSettings.Instance.ReceiptPrinter;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
         
            bool printemployee = GlobalSettings.Instance.ReceiptPrintEmployee;




            if (printername == "none") return;

            //defaults to receipt width since some customer had the character width setup.
            int receiptchars = receiptwidth;
            //translate to chars if value is in millimeters
            //58mm printer = 32 chars  , 80mm printer = 48 chars
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;


            ReceiptPrinter printer = new ReceiptPrinter(printername);
            int doubleheightwidth = (int)Math.Round(receiptchars * 0.65, 0);


            try
            {

  
                PrintStore();


                printer.Center();
                printer.PrintLF(new String('=', receiptchars));

                if (CurrentTicket.Status == "Voided")
                {
                    printer.DoubleHeight();

                    printer.PrintLF("****VOID****");
                    printer.DoubleHeightOFF();
                }



                printer.Left();

                if (CurrentTicket.CurrentEmployee.DisplayName == "[None]") printer.PrintLF(Utility.FormatPrintRow(" ", "Ticket #:" + CurrentTicket.SalesID, receiptchars));
                else printer.PrintLF(Utility.FormatPrintRow( "Cashier:" + CurrentTicket.CurrentEmployee.DisplayName, "Ticket #:" + CurrentTicket.SalesID, receiptchars));


                printer.Left();

                printer.PrintLF(Utility.FormatPrintRow(CurrentTicket.SaleDate.ToShortDateString(), CurrentTicket.SaleDate.ToShortTimeString(), receiptchars));
                printer.PrintLF(new String('-', receiptchars));




                //starts to print each item on ticket

                decimal itemtotal;

                foreach (TicketEmployee emp in CurrentTicket.TicketEmployees)
                    foreach (LineItem line in emp.LineItems)
                    {
                        //wether or not to include the modifier prices in the lineitem pricing
                        itemtotal = line.Price * line.Quantity;



                        if (line.Quantity > 0) receiptline = Utility.FormatPrintRow(line.Description, String.Format("{0:0.00}", itemtotal), receiptchars);
                        else receiptline = Utility.FormatPrintRow(line.Description, "", receiptchars);

                        printer.PrintLF(receiptline);
                        if (line.Surcharge > 0)
                        {

                            receiptline = Utility.FormatPrintRow("**Service Upgrade**", String.Format("+{0:0.00}", line.Surcharge), receiptchars);
                            printer.PrintLF(receiptline);
                        }
                        if (line.Discount > 0)
                        {

                            receiptline = Utility.FormatPrintRow("**" + line.DiscountName + "**", String.Format("-{0:0.00}", line.Discount), receiptchars);
                            printer.PrintLF(receiptline);
                        }
                        if (line.ItemType.ToUpper().Substring(0, 4) == "GIFT")
                        {
                            printer.PrintLF("No:" + line.Custom1);
                        }
                        if (line.Note != "")
                        {
                            printer.PrintLF(line.Note);
                        }

                        if (printemployee) printer.PrintLF("Tech:" + emp.EmployeeName);
                        printer.PrintLF(new String('-', receiptchars / 2));

                    }

                printer.PrintLF(new String('-', receiptchars));
                printer.LineFeed();



                printer.PrintLF(Utility.FormatPrintRow("SUBTOTAL:", String.Format("{0:0.00}", CurrentTicket.SubTotal), receiptchars));


                printer.PrintLF(Utility.FormatPrintRow( CurrentTicket.AdjustmentName + ":", String.Format("{0:0.00}", CurrentTicket.Adjustment), receiptchars));
              

                //Total = SubTotal + Adjustment + TaxTotal;

                printer.PrintLF(Utility.FormatPrintRow("TAX:", String.Format("{0:0.00}", CurrentTicket.TaxTotal), receiptchars));



                printer.PrintLF(Utility.FormatPrintRow("", "============", receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("TOTAL:", String.Format("{0:0.00}", CurrentTicket.Total), receiptchars));

                printer.LineFeed();


         
                foreach (Payment line in CurrentTicket.Payments)
                {
                    switch (line.CardGroup.ToUpper())
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



                }



                printer.PrintLF(Utility.FormatPrintRow("", "============", receiptchars));

                printer.DoubleHeight();
                if (CurrentTicket.Balance >= 0)
                {
                    printer.PrintLF(Utility.FormatPrintRow("BALANCE:", String.Format("{0:0.00}", Math.Round(CurrentTicket.Balance, 2)), doubleheightwidth));
                }
                else
                {
                    printer.PrintLF(Utility.FormatPrintRow("CHANGE:", String.Format("{0:0.00}", Math.Round(CurrentTicket.Balance * (decimal)(-1.0), 2)), doubleheightwidth));
                }
                printer.DoubleHeightOFF();

                printer.LineFeed();

                if(GlobalSettings.Instance.PrintTipOnReceipt)
                    if (CurrentTicket.TipAmount > 0)
                        printer.PrintLF(Utility.FormatPrintRow("TIP:", String.Format("{0:0.00}", CurrentTicket.TipAmount), receiptchars));



                if (CurrentTicket.CreditCardSurcharge > 0)
                {

                    printer.PrintLF(Utility.FormatPrintRow("CC Surcharge:", String.Format("{0:0.00}", CurrentTicket.CreditCardSurcharge), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Adj. Payment:", String.Format("{0:0.00}", CurrentTicket.AdjustedPayment), receiptchars));
                

                }


              


                //Payment notice

                printer.PrintLF(GlobalSettings.Instance.PaymentNotice);

                //Receipt notice
                printer.PrintLF(GlobalSettings.Instance.ReceiptNotice);


                if (GlobalSettings.Instance.ReceiptPrintReward)
                {
                    if (CurrentTicket.CurrentCustomer != null)
                    {
                        if (CurrentTicket.CurrentCustomer.ID > 0)
                        {
                            printer.LineFeed();
                            if (CurrentTicket.CurrentCustomer.Phone1.Length > 4) printer.PrintLF(Utility.FormatPrintRow("Customer:", CurrentTicket.CurrentCustomer.ID.ToString() + ":" + CurrentTicket.CurrentCustomer.Phone1.Substring(CurrentTicket.CurrentCustomer.Phone1.Length - 4, 4), receiptchars));
                            printer.PrintLF(Utility.FormatPrintRow("REWARD BALANCE:", String.Format("{0:0.00}", Math.Round(CurrentTicket.CurrentCustomer.RewardBalance, 2)), receiptchars));
                        }


                    }


                }
          
               
                printer.Center();

                if (CurrentTicket.Status == "Closed")
                {
                    printer.LineFeed();
                    printer.PrintLF("PAID IN FULL");
                }

                if (CurrentTicket.Status == "Voided")
                {
                    printer.DoubleHeight();

                    printer.PrintLF("****VOID****");
                    printer.DoubleHeightOFF();
                }


                printer.LineFeed();
                printer.PrintLF("     THANK YOU!");
                printer.LineFeed();
                printer.LineFeed();
                printer.Send(); //sends buffer to printer

                if (GlobalSettings.Instance.PrintBarCode) printer.PrintBarCode("TKT" + CurrentTicket.SalesID.ToString() );


                printer.Cut();

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("PrintReceipt:" + ex.Message);
            }

        }

        public static void AutoPrintCreditSlip(Ticket CurrentTicket, Payment m_payment)
        {
            ReceiptPrinterModel.PrintCreditSlip(CurrentTicket, m_payment, "**Merchant Copy**");
            if (GlobalSettings.Instance.PrintCustomerCopy) ReceiptPrinterModel.PrintCreditSlip(CurrentTicket, m_payment, "**Customer Copy**");
        }

        public static void PrintCreditSlip(Ticket CurrentTicket,Payment payment, string CopyString)
        {
          
            

            if (payment == null) return;



          
            string printername = GlobalSettings.Instance.ReceiptPrinter;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
            Location store = GlobalSettings.Instance.Shop;
            bool printemployee = GlobalSettings.Instance.ReceiptPrintEmployee;

         

            if (printername == "none") return;

            //defaults to receipt width since some customer had the character width setup.
            int receiptchars = receiptwidth;
            //translate to chars if value is in millimeters
            //58mm printer = 32 chars  , 80mm printer = 48 chars
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;


            ReceiptPrinter printer = new ReceiptPrinter(printername);


            int doubleheightwidth = (int)Math.Round(receiptchars * 0.65, 0);




            try
            {

                PrintStore();



                printer.Center();
             
                printer.PrintLF(new String('=', receiptchars));

               printer.DoubleHeight();
               printer.PrintLF(payment.CardGroup.ToUpper() + " " + payment.TransType.ToUpper());

                printer.DoubleHeightOFF();

                printer.Left();

                if (CurrentTicket.CurrentEmployee.DisplayName == "[None]") printer.PrintLF(Utility.FormatPrintRow(" ", "Ticket #:" + CurrentTicket.SalesID, receiptchars));
                else printer.PrintLF(Utility.FormatPrintRow("Cashier:" + CurrentTicket.CurrentEmployee.DisplayName, "Ticket #:" + CurrentTicket.SalesID, receiptchars));

                printer.Left();

                if(payment.Voided)
                    printer.PrintLF(Utility.FormatPrintRow(payment.VoidDate.ToShortDateString(), payment.VoidDate.ToShortTimeString(), receiptchars));
                else
                    printer.PrintLF(Utility.FormatPrintRow(payment.PaymentDate.ToShortDateString(), payment.PaymentDate.ToShortTimeString(), receiptchars));



                printer.PrintLF(new String('-', receiptchars));
                if(payment.CardAcquisition.ToUpper() == "INSERT" || payment.CardAcquisition.ToUpper() == "EMV TAP" || payment.CardAcquisition.ToUpper() == "CHIP" || payment.CardAcquisition.ToUpper() == "CONTACTICC")
                {
                    printer.PrintLF(Utility.FormatPrintRow("Acct:", payment.MaskedPAN, receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("App Name:", payment.EMV_ApplicationName, receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("AID:", payment.EMV_AID, receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow(payment.EMV_CryptogramType + ":", payment.EMV_Cryptogram, receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Entry:", payment.CardAcquisition, receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Approval:", payment.AuthorCode, receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Card Holder:", payment.CardHolderName, receiptchars));

                    switch(payment.PinVerified)
                    {
                        case 2:
                            printer.PrintLF(Utility.FormatPrintRow("PIN:", "Verified Online", receiptchars));
                            break;
                        case 1:
                            printer.PrintLF(Utility.FormatPrintRow("PIN:", "Verified Offline", receiptchars));
                            break;

                        default:
                            printer.PrintLF(Utility.FormatPrintRow("PIN:", "PIN not provided", receiptchars));
                            break;
                    }
                  
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

                printer.DoubleHeight();
                printer.PrintLF(Utility.FormatPrintRow("Ticket Total", String.Format("{0:0.00}", payment.NetAmount), doubleheightwidth));
                if(payment.CashbackAmount > 0) printer.PrintLF(Utility.FormatPrintRow("Cash Back", String.Format("{0:0.00}", payment.CashbackAmount), doubleheightwidth));
                printer.DoubleHeightOFF();
                printer.LineFeed();


                if (payment.TransType.ToUpper() == "POSTAUTH" || payment.TransType.ToUpper() == "SALE")
                {

                    printer.PrintLF(Utility.FormatPrintRow("Tip:", String.Format("{0:0.00}", payment.TipAmount), receiptchars));
                    printer.DoubleHeight();
                    printer.PrintLF(Utility.FormatPrintRow("Total w/Tip:", String.Format("{0:0.00}", payment.Amount), doubleheightwidth));
                    printer.DoubleHeightOFF();

                }

                if (payment.TransType.ToUpper() == "AUTH")
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





                printer.PrintLF(new String('-', receiptchars));

                if (GlobalSettings.Instance.PrintTipGuide && payment.CardGroup.ToUpper() == "CREDIT")
                {
                    printer.PrintLF("20%  = " + String.Format("{0:0.00}", payment.NetAmount * 0.2m) + "    " +
                                    "18%  = " + String.Format("{0:0.00}", payment.NetAmount * 0.18m) + "    " +
                                    "15%  = " + String.Format("{0:0.00}", payment.NetAmount * 0.15m));
                }
             


                printer.Center();


                if (payment.Voided)
                {
                    printer.DoubleHeight();

                    printer.PrintLF("****VOID****");
                    printer.DoubleHeightOFF();
                }else
                {
     

                 

                    if (payment.SignatureLine == 1 || (GlobalSettings.Instance.RequireSignatureOnAllCreditSale && payment.CardGroup.ToUpper() == "CREDIT" ))
                    {
                        printer.LineFeed();
                        printer.PrintLF("I agree to pay above total amount according to  card issuer agreement.");

                        printer.Center();
                        printer.LineFeed();
                        printer.LineFeed();
                        printer.PrintLF("X" + new String('_', receiptchars - 5));
                        printer.PrintLF("SIGNATURE");

                    }else
                        if (payment.SignatureLine == 2)
                        {
                            printer.LineFeed();
                            printer.PrintLF("I agree to pay above total amount according to  card issuer agreement.");
                            printer.Center();
                            printer.LineFeed();
                            printer.LineFeed();
                            printer.PrintLF("<<Signature Captured Electronically>>");
                            printer.PrintLF("SIGNATURE");
  
                        }


                    printer.LineFeed();
                    printer.DoubleHeight();

                    printer.PrintLF("APPROVED");
                 
                }

                printer.Center();

                printer.PrintLF(CopyString);

                printer.DoubleHeightOFF();

                printer.Center();
                printer.LineFeed();
                printer.PrintLF("THANK YOU! COME BACK AGAIN");
                printer.LineFeed();
                printer.Send(); //sends buffer to printer

                if (GlobalSettings.Instance.PrintBarCode) printer.PrintBarCode("PAY" +payment.ID.ToString());


                printer.Cut();

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("PrintCreditSlip:" + ex.Message);
            }

        }




        public static void PrintGiftCard(DataTable dt)
        {
           
            string printername = GlobalSettings.Instance.ReceiptPrinter;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
            Location store = GlobalSettings.Instance.Shop;
            bool printemployee = GlobalSettings.Instance.ReceiptPrintEmployee;


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


            ReceiptPrinter printer = new ReceiptPrinter(printername);
            int doubleheightwidth = (int)Math.Round(receiptchars * 0.65, 0);

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
                printer.PrintLF("Gift Card Summary");
          
                printer.DoubleHeightOFF();
                printer.LineFeed();
                printer.Left();

                printer.PrintLF(new String('=', receiptchars));

                printer.PrintLF("___Type__|__Account No___|_Amount|_Spent|Balance");


                foreach(DataRow row in dt.Rows)
                {
                    printer.PrintLF(row["cardtype"].ToString().Substring(0, 9).PadLeft(9, ' ') + row["accountnumber"].ToString().PadLeft(16, ' ') + Math.Round(decimal.Parse(row["amount"].ToString()), 2).ToString().PadLeft(7, ' ') + Math.Round(decimal.Parse(row["spent"].ToString()), 2).ToString().PadLeft(7, ' ') + Math.Round(decimal.Parse(row["balance"].ToString()), 2).ToString().PadLeft(7, ' '));
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


            }catch(Exception ex)
            {
                TouchMessageBox.Show("Print Gift Card Summary:" + ex.Message);
            }
        }




        public static void PrintResponse(string title,string printstring)
        {

            string printername = GlobalSettings.Instance.ReceiptPrinter;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
            Location store = GlobalSettings.Instance.Shop;
            bool printemployee = GlobalSettings.Instance.ReceiptPrintEmployee;


   

            if (printername == "none") return;

            //defaults to receipt width since some customer had the character width setup.
            int receiptchars = receiptwidth;
            //translate to chars if value is in millimeters
            //58mm printer = 32 chars  , 80mm printer = 48 chars
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;


            ReceiptPrinter printer = new ReceiptPrinter(printername);


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
                TouchMessageBox.Show("Print Response:" + ex.Message);
            }
        }
    }
}
