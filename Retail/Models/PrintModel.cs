using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedDot.Models
{
    public class PrintModel
    {


        public static void PrintEmployeeHours(Employee employee)
        {

            string printername = GlobalSettings.Instance.ReceiptPrinter;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
            Location store = GlobalSettings.Instance.Shop;

 

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
                printer.PrintLF("Employee Time");

                printer.DoubleHeightOFF();
                printer.LineFeed();
                printer.Left();

                printer.PrintLF(new String('=', receiptchars));
                // printer.PrintLF(Utility.FormatPrintRow(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), receiptchars));
                printer.PrintLF("Time In           TimeOut          Hours");
                decimal totalhours = 0;
                decimal time = 0;
                foreach (DataRow row in employee.Hours.Rows)
                {
                    //initialize with empty data
                    DateTime timein = DateTime.MinValue;
                    DateTime timeout = DateTime.MinValue;
                    time = 0;


                    //just incase there is a blank record .. we don't want it to error out
                    if (row["hours"].ToString() != "") time = decimal.Parse(row["hours"].ToString());

                    if (row["timein"].ToString() != "") timein = DateTime.Parse(row["timein"].ToString());

                    if (row["timeout"].ToString() != "") timeout = DateTime.Parse(row["timeout"].ToString());

                    printer.PrintLF(timein.ToString("MM/dd hh:mm tt") + " => " + (timeout == DateTime.MinValue ? "              " : timeout.ToString("MM/dd hh:mm tt")) + " : " + String.Format("{0:0.00}", time).PadLeft(5));


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
                MessageBox.Show("Print Employee Hours:" + ex.Message);
            }
        }


        public static void PrintReceiptPDF(Ticket ticket,bool display)
        {
            XFont fontsmallitalic = new XFont("Courier New", 6, XFontStyle.Italic | XFontStyle.Bold);

            XFont font = new XFont("Courier New",7, XFontStyle.Bold);
           // Font font = new Font("Courier New", 8, System.Drawing.FontStyle.Bold);
            // XFont font = new XFont("Times New Roman", 10, XFontStyle.Bold);

            XFont fontitalic = new XFont("Courier New", 7, XFontStyle.Italic | XFontStyle.Bold);
            XFont fontbold = new XFont("Courier New", 8, XFontStyle.Bold);
            // Create pen.
            Pen blackPen = new Pen(Color.Black, 2);
            Brush blackBrush = new SolidBrush(Color.Black);
            Location store = GlobalSettings.Instance.Shop;

            int fontHeight = (int)font.GetHeight();
            int fontBoldHeight = (int)fontbold.GetHeight();

            int startX = 50;
            int startY = 20;
            int XOffset = 0;
            int YOffset = 0;
            int rightLimit = 550;
            string receiptstr;

            int savedY = 0;




            PdfDocument document = new PdfDocument();

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XTextFormatter tf = new XTextFormatter(gfx);
            XRect rect = new XRect(0, 0, page.Width, page.Height);

            // Draw the text

            // gfx.DrawString("Hello, World!", font, XBrushes.Black,rect,XStringFormats.Center);












            if (store == null)
            {

                MessageBox.Show("Shop/store info missing");
                return;
            }


            string bitmapfile = GlobalSettings.Instance.StoreLogo;
            if (File.Exists(bitmapfile))
                if (GlobalSettings.Instance.StoreLogo != "") gfx.DrawImage(new Bitmap(bitmapfile), startX, startY, 200, 40);



            XOffset = 500;
            YOffset = 40;
            //customer invoide #
            PrintRightAlign(gfx, "Invoice #" + ticket.TicketNo, fontbold, rightLimit, startY + YOffset);

            YOffset = YOffset + fontBoldHeight;
            //Ticket Date and Time
            PrintRightAlign(gfx, ticket.SaleDate.ToString(), font, rightLimit, startY + YOffset);

            YOffset = YOffset + fontHeight;
            gfx.DrawLine(blackPen, startX, startY + YOffset, rightLimit, startY + YOffset);

            YOffset = YOffset + fontHeight;
            savedY = YOffset;
            PrintRightAlign(gfx, store.Address1, font, rightLimit, startY + YOffset);

            YOffset = YOffset + fontHeight;
            PrintRightAlign(gfx, store.City + "," + store.State + " " + store.Zip, font, rightLimit, startY + YOffset);

            YOffset = YOffset + fontHeight;
            PrintRightAlign(gfx, store.Phone, font, rightLimit, startY + YOffset);

            //customer info
            if (ticket.CurrentCustomer != null)
            {
                //YOffset = YOffset + fontHeight;
                YOffset = savedY;
                gfx.DrawString(ticket.CurrentCustomer.FirstName + " " + ticket.CurrentCustomer.LastName, font, new SolidBrush(Color.Black), startX, startY + YOffset);
                YOffset = YOffset + fontHeight;
                gfx.DrawString(ticket.CurrentCustomer.Address1, font, new SolidBrush(Color.Black), startX, startY + YOffset);
                YOffset = YOffset + fontHeight;
                if (ticket.CurrentCustomer.Address2.Trim() != "")
                {
                    gfx.DrawString(ticket.CurrentCustomer.Address2, font, new SolidBrush(Color.Black), startX, startY + YOffset);
                    YOffset = YOffset + fontHeight;
                }
                gfx.DrawString(ticket.CurrentCustomer.City + "," + ticket.CurrentCustomer.State + " " + ticket.CurrentCustomer.ZipCode, font, new SolidBrush(Color.Black), startX, startY + YOffset);
                YOffset = YOffset + fontHeight;
                gfx.DrawString(ticket.CurrentCustomer.Phone1, font, new SolidBrush(Color.Black), startX, startY + YOffset);

            }
            else
            {
                //YOffset = YOffset + fontHeight * 3;
                YOffset = savedY;
                gfx.DrawString("[Customer info ...]", fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);


            }

            //custom info
            YOffset = YOffset + fontHeight;
            gfx.DrawString(GlobalSettings.Instance.SalesCustomName1 + ticket.Custom1, font, new SolidBrush(Color.Black), startX, startY + YOffset);

            // YOffset = YOffset + fontHeight;
            gfx.DrawString(GlobalSettings.Instance.SalesCustomName2 + ticket.Custom2, font, new SolidBrush(Color.Black), startX + 200, startY + YOffset);

            // YOffset = YOffset + fontHeight;
            gfx.DrawString(GlobalSettings.Instance.SalesCustomName3 + ticket.Custom3, font, new SolidBrush(Color.Black), startX + 400, startY + YOffset);

            YOffset = YOffset + fontHeight;

            if (ticket.IsRefund)
            {
                YOffset = YOffset + fontHeight;
                gfx.DrawString("****************************************************************************", fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);
                YOffset = YOffset + fontHeight;
                gfx.DrawString("***************************      REFUND       ******************************", fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);
                YOffset = YOffset + fontHeight;
                gfx.DrawString("****************************************************************************", fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);
            }

            //Title for ticket items
            YOffset = YOffset + fontHeight;
            gfx.DrawString("Description", fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);
            PrintRightAlign(gfx, "Qty", fontbold, rightLimit - 160, startY + YOffset);
            PrintRightAlign(gfx, "Price", fontbold, rightLimit - 80, startY + YOffset);
            PrintRightAlign(gfx, "Total", fontbold, rightLimit, startY + YOffset);
            // line beneath title line
            YOffset = YOffset + fontHeight / 2;
            gfx.DrawLine(blackPen, startX, startY + YOffset, rightLimit, startY + YOffset);

            YOffset = YOffset + fontHeight;
            //loop through items
            foreach (LineItem line in ticket.LineItems)
            {

                if (YOffset > 700)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    YOffset = 50;
                }

                if (line.Quantity != 0)
                {
                    //reduce description if it's too long so doesn't overwrite the quantity and price
                    receiptstr = line.ModelNumber;
                    if (receiptstr.Length > 70) receiptstr = receiptstr.Substring(0, 70) + "...";
                    gfx.DrawString(receiptstr, font, new SolidBrush(Color.Black), startX, startY + YOffset);


                    PrintRightAlign(gfx, line.Quantity.ToString(), font, rightLimit - 160, startY + YOffset);
                    PrintRightAlign(gfx, line.PriceSurcharge.ToString(), font, rightLimit - 80, startY + YOffset);
                    PrintRightAlign(gfx, (line.PriceSurcharge * line.Quantity).ToString(), font, rightLimit, startY + YOffset);


                    //description is printed on second line
                    YOffset = YOffset + fontHeight;
                    receiptstr = line.Description;
                    if (receiptstr.Length > 140) receiptstr = receiptstr.Substring(0, 140) + "...";
                    gfx.DrawString(receiptstr, fontitalic, new SolidBrush(Color.Gray), startX + 5, startY + YOffset);

                    //prints discount
                    if (line.Discount > 0)
                    {
                        YOffset = YOffset + fontHeight;
                        gfx.DrawString("     **Promo Discount**", fontitalic, new SolidBrush(Color.Gray), startX + 5, startY + YOffset);
                        PrintRightAlign(gfx, String.Format("-{0:0.00}", line.Discount * line.Quantity), font, rightLimit, startY + YOffset);
                    }

                    if (line.Note.Length > 0)
                    {
                        YOffset = YOffset + fontHeight;
                        PrintLeftAlign(gfx, line.Note, fontitalic, new SolidBrush(Color.Gray), startX + 5, startY + YOffset, fontHeight, 345);

                        int rtncount = line.Note.Count(x => x == (char)13);
                        YOffset = YOffset + fontHeight * rtncount;
                    }




                }
                else
                {
                    // =====Service===== line

                    gfx.DrawString(line.Description, fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);

                }

                YOffset = YOffset + fontHeight;
            }

            XOffset = 350;

            YOffset = YOffset + fontHeight / 2;

            int totalstart = YOffset;


            //Payment notice
         
            rect = new XRect(startX, startY + YOffset, 300, 150);
            //  gfx.DrawRectangle(XBrushes.SeaShell, rect);


            tf.Alignment = XParagraphAlignment.Justify;
            tf.DrawString(GlobalSettings.Instance.PaymentNotice, fontsmallitalic, new SolidBrush(Color.Black), rect, XStringFormats.TopLeft);

           // PrintLeftAlign(gfx, GlobalSettings.Instance.PaymentNotice, fontitalic, new SolidBrush(Color.Gray), startX, startY + YOffset, fontHeight, 300);


            //Receipt notice
            YOffset = YOffset + fontHeight * 4;
            rect = new XRect(startX, startY + YOffset, 300, 150);
            //  gfx.DrawRectangle(XBrushes.SeaShell, rect);
            tf.Alignment = XParagraphAlignment.Justify;
            tf.DrawString(GlobalSettings.Instance.ReceiptNotice, fontsmallitalic, new SolidBrush(Color.Black), rect, XStringFormats.TopLeft);




            //resets Y position back
            YOffset = totalstart;


            gfx.DrawLine(blackPen, startX + XOffset, startY + YOffset, rightLimit, startY + YOffset); //line below items



            rect = new XRect(startX, startY + YOffset, 220, 100);
            //  gfx.DrawRectangle(XBrushes.SeaShell, rect);
            tf.Alignment = XParagraphAlignment.Justify;



            tf.DrawString(ticket.Note, fontitalic, new SolidBrush(Color.Black), rect, XStringFormats.TopLeft);

            YOffset = YOffset + fontHeight;
            gfx.DrawString("Parts:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
            PrintRightAlign(gfx, ticket.ProductTotal.ToString(), font, rightLimit, startY + YOffset);

            YOffset = YOffset + fontHeight;
            gfx.DrawString("Labor:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
            PrintRightAlign(gfx, ticket.LaborTotal.ToString(), font, rightLimit, startY + YOffset);

            YOffset = YOffset + fontHeight;
            gfx.DrawString("Shop Fee:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
            PrintRightAlign(gfx, ticket.ShopFee.ToString(), font, rightLimit, startY + YOffset);


            YOffset = YOffset + fontHeight;
            gfx.DrawString("Shipping:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
            PrintRightAlign(gfx, ticket.ShippingTotal.ToString(), font, rightLimit, startY + YOffset);


            YOffset = YOffset + fontHeight;
            gfx.DrawString("Adjustments:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
            PrintRightAlign(gfx, "-" + ticket.Discount.ToString(), font, rightLimit, startY + YOffset);

            YOffset = YOffset + fontHeight / 2;
            gfx.DrawLine(blackPen, startX + XOffset, startY + YOffset, rightLimit, startY + YOffset); //line below Discount


            YOffset = YOffset + fontHeight;
            gfx.DrawString("Sub-Total:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
            PrintRightAlign(gfx, ticket.SubTotal.ToString(), font, rightLimit, startY + YOffset);



            YOffset = YOffset + fontHeight;
            gfx.DrawString("Sales Tax:" + (ticket.TaxExempt ? "  (Tax Exempt)" : ""), fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
            PrintRightAlign(gfx, ticket.SalesTax.ToString(), font, rightLimit, startY + YOffset);

            YOffset = YOffset + fontHeight;
            gfx.DrawString("Total:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
            PrintRightAlign(gfx, ticket.Total.ToString(), font, rightLimit, startY + YOffset);



            //Payments
            YOffset = YOffset + fontHeight * 2;
            gfx.DrawString("Payments:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
            YOffset = YOffset + fontHeight;



            //loop through payments
       

            foreach (Payment pay in ticket.Payments)
            {
                YOffset = YOffset + fontHeight;

                gfx.DrawString(pay.PaymentDate.ToShortDateString() + " " + pay.Description, font, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                PrintRightAlign(gfx, pay.AmountStr, font, rightLimit, startY + YOffset);

            }



            YOffset = YOffset + fontHeight / 2;
            gfx.DrawLine(blackPen, startX + XOffset, startY + YOffset, rightLimit, startY + YOffset); //line below payments


            YOffset = YOffset + fontHeight;
            gfx.DrawString("Total Payments:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
            PrintRightAlign(gfx, ticket.TotalPayment.ToString(), font, rightLimit, startY + YOffset);

            YOffset = YOffset + fontHeight / 2;
            gfx.DrawLine(blackPen, startX + XOffset, startY + YOffset, rightLimit, startY + YOffset);
            gfx.DrawLine(blackPen, startX + XOffset, startY + YOffset + 5, rightLimit, startY + YOffset + 5); //double lines

            YOffset = YOffset + fontHeight * 2;
            gfx.DrawString("Balance:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
            PrintRightAlign(gfx, ticket.BalanceStr, font, rightLimit, startY + YOffset);

            if (ticket.CreditCardSurcharge > 0)
            {
                YOffset = YOffset + fontHeight * 3;
                gfx.DrawString("CC Surcharge:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                PrintRightAlign(gfx, ticket.CreditCardSurcharge.ToString(), font, rightLimit, startY + YOffset);

                YOffset = YOffset + fontHeight;
                gfx.DrawString("Adj. Payment:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                PrintRightAlign(gfx, ticket.AdjustedPayment.ToString(), font, rightLimit, startY + YOffset);

            }

           


            // Save the document...
            string filename = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\pdf\\Ticket" + ticket.TicketNo + ".pdf";
            document.Save(filename);


           // RawPrinterHelper.SendFileToPrinter(GlobalSettings.Instance.LargeFormatPrinter, filename);

            // ...and start a viewer.
            if (display)
                Process.Start(filename);
        }

        private static void PrintRightAlign(Graphics graphics, string receiptline, Font font, int rightlimit, int Y)
        {
            graphics.DrawString(receiptline, font, new SolidBrush(Color.Black), rightlimit - graphics.MeasureString(receiptline, font).Width, Y);
        }


        private static void PrintRightAlign(XGraphics graphics, string receiptline, XFont font, int rightlimit, int Y)
        {
            graphics.DrawString(receiptline, font, new SolidBrush(Color.Black), rightlimit - graphics.MeasureString(receiptline, font).Width, Y);
        }

        private static int PrintLeftAlign(XGraphics graphics, string receiptline, XFont font, Brush brush, int X, int Y, int height, int width)
        {

            List<string> lines = WrapText(receiptline.Trim(), width, font.Name, (float)font.Size);
            int YOffset = Y;


            foreach (var item in lines)
            {
                graphics.DrawString(item, font, brush, X, YOffset);
                YOffset = YOffset + height;
            }

            return YOffset;
        }

        private static int PrintLeftAlign(Graphics graphics, string receiptline, Font font, Brush brush, int X, int Y, int height, int width)
        {

            List<string> lines = WrapText(receiptline, width, font.Name, font.Size);
            int YOffset = Y;



            foreach (var item in lines)
            {
                graphics.DrawString(item, font, brush, X, YOffset);
                YOffset = YOffset + height;
            }

            return YOffset;
        }

        private static List<string> WrapText(string text, double pixels, string fontFamily, float emSize)
        {
            string[] originalLines = text.Split(new string[] { " " }, StringSplitOptions.None);

            List<string> wrappedLines = new List<string>();

            StringBuilder actualLine = new StringBuilder();
            double actualWidth = 0;

            foreach (var item in originalLines)
            {
                System.Windows.Media.FormattedText formatted = new System.Windows.Media.FormattedText(item,
                    CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight,
                    new System.Windows.Media.Typeface(fontFamily), emSize, System.Windows.Media.Brushes.Black);

                actualLine.Append(item + " ");
                actualWidth += formatted.Width;

                if (actualWidth > pixels)
                {
                    wrappedLines.Add(actualLine.ToString());
                    actualLine.Clear();
                    actualWidth = 0;
                }
            }

            if (actualLine.Length > 0)
                wrappedLines.Add(actualLine.ToString());

            return wrappedLines;
        }
    }
}
