using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
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
using System.Windows;


namespace RedDot
{
    public class WorkOrder:INPCBase
    {
        private DBTicket dbTicket = new DBTicket();
        public int id { get; set; }
        public int salesid { get; set; }

        public string TicketNo
        {
            get { return GlobalSettings.Instance.Shop.StorePrefix + salesid; }
        }

        public string WorkOrderNo
        {
            get { return GlobalSettings.Instance.Shop.StorePrefix + id; }
        }
        public string workordertype { get; set; }
        public string custom1 { get; set; }
        public string custom2 { get; set; }
        public string custom3 { get; set; }
        public string custom4 { get; set; }
        public string custom5 { get; set; }
        public string custom6 { get; set; }
        public string custom7 { get; set; }
        public string custom8 { get; set; }

        public string custom9 { get; set; }
        public string custom10 { get; set; }
        public string custom11 { get; set; }

        public string custom12 { get; set; }
        public string custom13 { get; set; }
        public string custom14 { get; set; }
        public string custom15 { get; set; }
        public string custom16 { get; set; }
        public string custom17 { get; set; }
        public string custom18 { get; set; }
        public string custom19 { get; set; }
        public string custom20 { get; set; }
        public string custom21 { get; set; }


        public WorkOrder(DataRow row)
        {
             if(row["id"].ToString() != "") id = (int)row["id"]; else id = 0;
            if (row["salesid"].ToString() != "") salesid = (int)row["salesid"]; else salesid = 0;
            workordertype = row["workordertype"].ToString();
            custom1 = row["custom1"].ToString();
            custom2 = row["custom2"].ToString();
            custom3 = row["custom3"].ToString();
            custom4 = row["custom4"].ToString();
            custom5 = row["custom5"].ToString();
            custom6 = row["custom6"].ToString();
            custom7 = row["custom7"].ToString();
            custom8 = row["custom8"].ToString();

            //Integrated amp
            custom9 = row["custom9"].ToString();
            if (custom9 == "YES") custom9 = "INTEGRATED";
            if (custom9 == "NO") custom9 = "BEHIND SEAT";


            custom10 = row["custom10"].ToString();
            custom11 = row["custom11"].ToString();
            custom12 = row["custom12"].ToString();
            custom13 = row["custom13"].ToString();
            custom14 = row["custom14"].ToString();
            custom15 = row["custom15"].ToString();
            custom16 = row["custom16"].ToString();
            custom17 = row["custom17"].ToString();
            custom18 = row["custom18"].ToString();
            custom19 = row["custom19"].ToString();
            custom20 = row["custom20"].ToString();
            custom21 = row["custom21"].ToString();


     
        }

        public void Save()
        {
            dbTicket.UpdateWorkOrder(id, custom1, custom2, custom3, custom4, custom5, custom6, custom7, custom8, custom9, custom10,custom11,custom12,custom13,custom14,custom15,custom16,custom17,custom18,custom19,custom20,custom21);

        }


        /*
        public void Print()
        {
            try
            {
                Save(); //save any changes so the print will pick it up

                PrintDocument pdoc = null;
                pdoc = new PrintDocument();

                pdoc.PrintPage += new PrintPageEventHandler(pdoc_Print);
                pdoc.PrinterSettings.PrinterName = GlobalSettings.Instance.LargeFormatPrinter;
               // pdoc.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Printing Error:" + ex.Message);
            }



        }


        void pdoc_Print(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Font font = new Font("Courier New", 10, System.Drawing.FontStyle.Bold);
        
            Font fontbold = new Font("Courier New", 14, System.Drawing.FontStyle.Bold);
            // Create pen.
            Pen blackPen = new Pen(Color.Black, 1);

            Brush blackBrush = new SolidBrush(Color.Black);
            Location store = GlobalSettings.Instance.Shop;

            Ticket CurrentTicket = new Ticket(salesid);


            int fontHeight = (int)font.GetHeight();
            int fontBoldHeight = (int)fontbold.GetHeight();

            int startX = 50;
          
            int XOffset = 0;
            int YOffset = 0;
            int rightLimit = 750;
            int taboffset = 180;
         

         


            try
            {
                if (store == null)
                {

                    MessageBox.Show("Shop/store info missing");
                    return;
                }


                string bitmapfile = GlobalSettings.Instance.StoreLogo;
                if (File.Exists(bitmapfile))
                    if (GlobalSettings.Instance.StoreLogo != "") graphics.DrawImage(new Bitmap(bitmapfile), startX, 20, 300, 50);

                
                XOffset = 500;
                YOffset = 30;
        
                YOffset = YOffset + fontBoldHeight;
              

                YOffset = YOffset + fontBoldHeight;

                graphics.DrawString("Enclosure Build Log", fontbold, new SolidBrush(Color.Black), startX + 200, YOffset);

                YOffset = YOffset + fontBoldHeight * 2;
              
                graphics.DrawString("Create Date:" + CurrentTicket.SaleDate.ToString() + "                 TICKET #: " + CurrentTicket.SalesID, font, new SolidBrush(Color.Black), startX, YOffset);

                YOffset = YOffset + fontBoldHeight ;



                if(CurrentTicket.CloseDate == DateTime.MinValue)
                {
                    if(CurrentTicket.Payments.Count() > 0)
                        graphics.DrawString("Settle Date: Partial Payment", font, new SolidBrush(Color.Black), startX, YOffset);
                   else
                        graphics.DrawString("Settle Date: Not Paid", font, new SolidBrush(Color.Black), startX, YOffset);
                }
                else
                    graphics.DrawString("Settle Date:" + CurrentTicket.CloseDate.ToString(), font, new SolidBrush(Color.Black), startX, YOffset) ;



                YOffset = YOffset + fontBoldHeight;
                //customer info
                if (CurrentTicket.CurrentCustomer != null)
                {
                    YOffset = YOffset + fontBoldHeight;
                    graphics.DrawString("Name:", font, new SolidBrush(Color.Black), startX,  YOffset);
                    graphics.DrawString(CurrentTicket.CurrentCustomer.FirstName + " " + CurrentTicket.CurrentCustomer.LastName, font, new SolidBrush(Color.Black), startX + taboffset,  YOffset);


                    YOffset = YOffset + fontBoldHeight;
                    graphics.DrawString("Adress:", font, new SolidBrush(Color.Black), startX,  YOffset);
                    graphics.DrawString(CurrentTicket.CurrentCustomer.Address1, font, new SolidBrush(Color.Black), startX + taboffset,  YOffset);
                    YOffset = YOffset + fontBoldHeight;
                    graphics.DrawString(CurrentTicket.CurrentCustomer.City + "," + CurrentTicket.CurrentCustomer.State + " " + CurrentTicket.CurrentCustomer.ZipCode, font, new SolidBrush(Color.Black), startX + taboffset,  YOffset);
                    YOffset = YOffset + fontBoldHeight;

                    graphics.DrawString("Phone:", font, new SolidBrush(Color.Black), startX,  YOffset);
                    graphics.DrawString(CurrentTicket.CurrentCustomer.Phone1, font, new SolidBrush(Color.Black), startX + taboffset,  YOffset);
                    YOffset = YOffset + fontBoldHeight;

                    graphics.DrawString("Email:", font, new SolidBrush(Color.Black), startX, YOffset);
                    graphics.DrawString(CurrentTicket.CurrentCustomer.Email, font, new SolidBrush(Color.Black), startX + taboffset, YOffset);

                }
                else
                {
                    YOffset = YOffset + fontBoldHeight;
                    graphics.DrawString("[Customer info not available ...]", font, new SolidBrush(Color.Black), startX,  YOffset);
                    YOffset = YOffset + fontBoldHeight;

                }

                YOffset = YOffset + fontBoldHeight;
                graphics.DrawLine(blackPen, startX , YOffset, startX + 750, YOffset);

                YOffset = YOffset + fontBoldHeight;
                graphics.DrawString("Vehicle:", font, new SolidBrush(Color.Black), startX,  YOffset);
                graphics.DrawString("Year:" + CurrentTicket.Custom1 + "   Make:" + CurrentTicket.Custom2 + "  Model:" + CurrentTicket.Custom3, font, new SolidBrush(Color.Black), startX + taboffset, YOffset);


                YOffset = YOffset + fontBoldHeight;
                graphics.DrawLine(blackPen, startX , YOffset, startX + 750, YOffset);

                


                YOffset = YOffset + fontBoldHeight;

                PrintRightAlign(graphics, "Enclosure:", font, startX + taboffset, YOffset);
                graphics.DrawString(CurrentTicket.WorkOrder.custom1, font, new SolidBrush(Color.Black), startX + taboffset, YOffset);

                YOffset = YOffset + fontBoldHeight;
                PrintRightAlign(graphics, "Build Notes:", font, startX + taboffset, YOffset);
                
                

                YOffset = YOffset + fontBoldHeight;
                YOffset = PrintLeftAlign(graphics, CurrentTicket.WorkOrder.custom2, font, startX + taboffset, YOffset, fontBoldHeight);


                YOffset = YOffset + fontBoldHeight ;
            
                PrintRightAlign(graphics, "Subs:", font, startX + taboffset, YOffset);
                graphics.DrawString(CurrentTicket.WorkOrder.custom3, font, new SolidBrush(Color.Black), startX + taboffset, YOffset);
             
                YOffset = YOffset + fontBoldHeight;
 
                PrintRightAlign(graphics, "Wrap Color:", font, startX + taboffset, YOffset);
                YOffset = PrintLeftAlign(graphics, CurrentTicket.WorkOrder.custom4, font, startX + taboffset, YOffset, fontBoldHeight);

                YOffset = YOffset + fontBoldHeight;
 
                PrintRightAlign(graphics, "Stitch Color:", font, startX + taboffset, YOffset);
                YOffset = PrintLeftAlign(graphics, CurrentTicket.WorkOrder.custom5, font, startX + taboffset, YOffset, fontBoldHeight);


                YOffset = YOffset + fontBoldHeight;

                PrintRightAlign(graphics, "Badging:", font, startX + taboffset, YOffset);
                YOffset = PrintLeftAlign(graphics, CurrentTicket.WorkOrder.custom6, font, startX + taboffset, YOffset, fontBoldHeight);

                YOffset = YOffset + fontBoldHeight;

                PrintRightAlign(graphics, "Lighting:", font, startX + taboffset, YOffset);
                graphics.DrawString(CurrentTicket.WorkOrder.custom7, font, new SolidBrush(Color.Black), startX + taboffset, YOffset);

                YOffset = YOffset + fontBoldHeight;
  
                PrintRightAlign(graphics, "Lighting Color:", font, startX + taboffset, YOffset);
                graphics.DrawString(CurrentTicket.WorkOrder.custom8, font, new SolidBrush(Color.Black), startX + taboffset, YOffset);

                YOffset = YOffset + fontBoldHeight;

                PrintRightAlign(graphics, "Int. Amp Rack:", font, startX + taboffset, YOffset);
                graphics.DrawString(CurrentTicket.WorkOrder.custom9, font, new SolidBrush(Color.Black), startX + taboffset, YOffset);

                YOffset = YOffset + fontBoldHeight * 2;
  
                PrintRightAlign(graphics, "Ported Enclosure:", font, startX + taboffset, YOffset);
                graphics.DrawString(CurrentTicket.WorkOrder.custom10, font, new SolidBrush(Color.Black), startX + taboffset, YOffset);

                YOffset = YOffset + fontBoldHeight * 2;

                PrintRightAlign(graphics, "To Be Shipped:", font, startX + taboffset, YOffset);
                graphics.DrawString(CurrentTicket.WorkOrder.custom11, font, new SolidBrush(Color.Black), startX + taboffset, YOffset);

                YOffset = YOffset + fontBoldHeight * 2;

                PrintRightAlign(graphics, "Built By:", font, startX + taboffset, YOffset);
                graphics.DrawString(CurrentTicket.WorkOrder.custom12, font, new SolidBrush(Color.Black), startX + taboffset, YOffset);

                YOffset = YOffset + fontBoldHeight * 2;

                PrintRightAlign(graphics, "Wrapped By:", font, startX + taboffset, YOffset);
                graphics.DrawString(CurrentTicket.WorkOrder.custom13, font, new SolidBrush(Color.Black), startX + taboffset, YOffset);
                YOffset = YOffset + fontBoldHeight * 2;

                PrintRightAlign(graphics, "Finished By:", font, startX + taboffset, YOffset);
                graphics.DrawString(CurrentTicket.WorkOrder.custom14, font, new SolidBrush(Color.Black), startX + taboffset, YOffset);
            }
            catch (Exception ex)
            {
                MessageBox.Show("pdoc_PrintPage:" + ex.Message);
            }


        }


        */


        public void PrintPDF(bool display)
        {





            XFont font = new XFont("Courier New", 10, XFontStyle.Bold);

            XFont fontbold = new XFont("Courier New", 12, XFontStyle.Bold);

            XFont fontboldbold = new XFont("Courier New", 14, XFontStyle.Bold);
            // Create pen.
            Pen blackPen = new Pen(Color.Black, 1);

            Brush blackBrush = new SolidBrush(Color.Black);
            Location store = GlobalSettings.Instance.Shop;

            Ticket CurrentTicket = new Ticket(salesid);


           
            int fontHeight = (int)font.GetHeight();

            int Xoffset = 35;
            int YOffset = 40;
       
            int taboffset = 80;

            PdfDocument document = new PdfDocument();

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XTextFormatter tf = new XTextFormatter(gfx);
            XRect rect = new XRect(0, 0, page.Width, page.Height);



            try
            {
                if (store == null)
                {

                    MessageBox.Show("Shop/store info missing");
                    return;
                }


                string bitmapfile = GlobalSettings.Instance.StoreLogo;
                if (File.Exists(bitmapfile))
                    if (GlobalSettings.Instance.StoreLogo != "") gfx.DrawImage(new Bitmap(bitmapfile), Xoffset, 10, 200, 30);


            

                YOffset = YOffset + fontHeight;


                YOffset = YOffset + fontHeight;

                gfx.DrawString("Enclosure Build Log    TICKET #: " + CurrentTicket.TicketNo + ", Work Order #: " + WorkOrderNo, fontboldbold, new SolidBrush(Color.Black), Xoffset , YOffset);

                YOffset = YOffset + fontHeight * 2;

              
                gfx.DrawString("Create Date:" + CurrentTicket.SaleDate.ToString() , font, new SolidBrush(Color.Black), Xoffset, YOffset);

                YOffset = YOffset + fontHeight;



                if (CurrentTicket.CloseDate == DateTime.MinValue)
                {
                    if (CurrentTicket.Payments.Count() > 0)
                        gfx.DrawString("Settle Date: Partial Payment", font, new SolidBrush(Color.Black), Xoffset, YOffset);
                    else
                        gfx.DrawString("Settle Date: Not Paid", font, new SolidBrush(Color.Black), Xoffset, YOffset);
                }
                else
                    gfx.DrawString("Settle Date:" + CurrentTicket.CloseDate.ToString(), font, new SolidBrush(Color.Black), Xoffset, YOffset);



                YOffset = YOffset + fontHeight;
                //customer info
                if (CurrentTicket.CurrentCustomer != null)
                {
                    YOffset = YOffset + fontHeight;
                    gfx.DrawString("Name:", font, new SolidBrush(Color.Black), Xoffset, YOffset);
                    gfx.DrawString(CurrentTicket.CurrentCustomer.FirstName + " " + CurrentTicket.CurrentCustomer.LastName, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);


                    YOffset = YOffset + fontHeight;
                    gfx.DrawString("Adress:", font, new SolidBrush(Color.Black), Xoffset, YOffset);
                    gfx.DrawString(CurrentTicket.CurrentCustomer.Address1, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);
                    YOffset = YOffset + fontHeight;
                    if(CurrentTicket.CurrentCustomer.Address2.Trim() != "")
                    {
                        gfx.DrawString(CurrentTicket.CurrentCustomer.Address2, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);
                        YOffset = YOffset + fontHeight;
                    }
     
                    gfx.DrawString(CurrentTicket.CurrentCustomer.City + "," + CurrentTicket.CurrentCustomer.State + " " + CurrentTicket.CurrentCustomer.ZipCode, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);
                    YOffset = YOffset + fontHeight;

                    gfx.DrawString("Phone:", font, new SolidBrush(Color.Black), Xoffset, YOffset);
                    gfx.DrawString(CurrentTicket.CurrentCustomer.Phone1, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);
                    YOffset = YOffset + fontHeight;

                    gfx.DrawString("Email:", font, new SolidBrush(Color.Black), Xoffset, YOffset);
                    gfx.DrawString(CurrentTicket.CurrentCustomer.Email, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);

                }
                else
                {
                    YOffset = YOffset + fontHeight;
                    gfx.DrawString("[Customer info not available ...]", font, new SolidBrush(Color.Black), Xoffset, YOffset);
                    YOffset = YOffset + fontHeight;

                }

                YOffset = YOffset + fontHeight;
                gfx.DrawLine(blackPen, Xoffset, YOffset, Xoffset + 750, YOffset);

                YOffset = YOffset + fontHeight;
                gfx.DrawString("Vehicle:", font, new SolidBrush(Color.Black), Xoffset, YOffset);
                gfx.DrawString("Year:" + CurrentTicket.Custom1 + "   Make:" + CurrentTicket.Custom2 + "  Model:" + CurrentTicket.Custom3, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);


                YOffset = YOffset + fontHeight;
                gfx.DrawLine(blackPen, Xoffset, YOffset, Xoffset + 750, YOffset);

                YOffset = YOffset + fontHeight;
                PrintRightAlign(gfx, "RUSH?:  ", font, Xoffset + taboffset, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom18, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);
                PrintRightAlign(gfx, "Date Promised:  ", font, Xoffset + taboffset + 150, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom19, font, new SolidBrush(Color.Black), Xoffset + taboffset + 150, YOffset);



                YOffset += fontHeight * 2;
                PrintRightAlign(gfx, "Enclosure:  ", font, Xoffset + taboffset, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom1, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);

                YOffset += fontHeight * 2;
                PrintRightAlign(gfx, "Build Notes:  ", font, Xoffset + taboffset, YOffset);
                YOffset = PrintLeftAlign(gfx, CurrentTicket.WorkOrder.custom2, font, Xoffset + taboffset, YOffset, fontHeight);

                YOffset += fontHeight;

                PrintRightAlign(gfx, "Subs:  ", font, Xoffset + taboffset, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom20, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);
                PrintRightAlign(gfx, "Sub Model:  ", font, Xoffset + taboffset + 200, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom3, font, new SolidBrush(Color.Black), Xoffset + taboffset + 200, YOffset);

                YOffset += fontHeight * 2;

                PrintRightAlign(gfx, "Wrap Color:  ", font, Xoffset + taboffset, YOffset);
                YOffset = PrintLeftAlign(gfx, CurrentTicket.WorkOrder.custom4, font, Xoffset + taboffset, YOffset, fontHeight);

                YOffset += fontHeight;

                PrintRightAlign(gfx, "Stitch Color:  ", font, Xoffset + taboffset, YOffset);
                YOffset = PrintLeftAlign(gfx, CurrentTicket.WorkOrder.custom5, font, Xoffset + taboffset, YOffset, fontHeight);

                YOffset += fontHeight;

                PrintRightAlign(gfx, "Badging:  ", font, Xoffset + taboffset, YOffset);
                YOffset = PrintLeftAlign(gfx, CurrentTicket.WorkOrder.custom6, font, Xoffset + taboffset, YOffset, fontHeight);

                YOffset += fontHeight ;

                PrintRightAlign(gfx, "Lighting:  ", font, Xoffset + taboffset, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom7, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);

                YOffset += fontHeight * 2;

                PrintRightAlign(gfx, "Light Color:  ", font, Xoffset + taboffset, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom8, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);

                YOffset += fontHeight * 2;

                PrintRightAlign(gfx, "Int. Amp Rack:  ", font, Xoffset + taboffset, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom9, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);

                YOffset += fontHeight * 2;

                PrintRightAlign(gfx, "Ported:  ", font, Xoffset + taboffset, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom10, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);

                YOffset += fontHeight * 2;

                PrintRightAlign(gfx, "To Be Shipped:  ", font, Xoffset + taboffset, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom11, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);

                YOffset += fontHeight * 2;

                gfx.DrawString("Additional Items to be shipped", fontbold, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);

                YOffset += fontHeight * 2;

                PrintRightAlign(gfx, "Speakers:  ", font, Xoffset + taboffset, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom12, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);

                YOffset += fontHeight * 2;

                PrintRightAlign(gfx, "Amps:  ", font, Xoffset + taboffset, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom13, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);

                YOffset += fontHeight * 2;

                PrintRightAlign(gfx, "Wiring:  ", font, Xoffset + taboffset, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom14, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);

                YOffset += fontHeight * 2;

                PrintRightAlign(gfx, "Adapters:  ", font, Xoffset + taboffset, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom15, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);

                YOffset += fontHeight * 2;

                PrintRightAlign(gfx, "Seat Lift:  ", font, Xoffset + taboffset, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom16, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);

                YOffset += fontHeight * 2;

                PrintRightAlign(gfx, "Submitted By:  ", font, Xoffset + taboffset, YOffset);
                gfx.DrawString(CurrentTicket.WorkOrder.custom17, font, new SolidBrush(Color.Black), Xoffset + taboffset, YOffset);

                YOffset += fontHeight * 2;



                // Save the document...
                string filename = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\pdf\\WorkOrder" + WorkOrderNo + ".pdf";
                document.Save(filename);


                // ...and start a viewer.
                if(display)
                Process.Start(filename);


            }
            catch (Exception ex)
            {
                MessageBox.Show("pdoc_PrintPage:" + ex.Message);
            }


        }

  
        private void PrintRightAlign(Graphics graphics, string receiptline, Font font, int rightlimit, int Y)
        {
            graphics.DrawString(receiptline, font, new SolidBrush(Color.Black), rightlimit - graphics.MeasureString(receiptline, font).Width, Y);
        }


        private int PrintLeftAlign(Graphics graphics, string receiptline, Font font,int X, int Y, int height)
        {

            List<string> lines = WrapText(receiptline, 270, font.Name, font.Size);
            int YOffset = Y;
            
           

            foreach (var item in lines)
            {
                graphics.DrawString(item, font, new SolidBrush(Color.Black), X, YOffset);
                YOffset = YOffset + height;
            }

            return YOffset;
        }


        private void PrintRightAlign(XGraphics graphics, string receiptline, XFont font, int rightlimit, int Y)
        {
            graphics.DrawString(receiptline, font, new SolidBrush(Color.Black), rightlimit - graphics.MeasureString(receiptline, font).Width - 2, Y);
        }


        private int PrintLeftAlign(XGraphics graphics, string receiptline, XFont font, int X, int Y, int height)
        {

            List<string> lines = WrapText(receiptline.Trim(), 350, font.Name,(float) font.Size);
            int YOffset = Y;


            foreach (var item in lines)
            {
                graphics.DrawString(item, font, new SolidBrush(Color.Black), X, YOffset);
                YOffset = YOffset + height;
            }

            return YOffset;
        }


        static List<string> WrapText(string text, double pixels, string fontFamily, float emSize)
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
