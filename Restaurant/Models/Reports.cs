using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;

namespace RedDot
{
    public class QSReports
    {
        DBReports _dbreports;
        public QSReports()
        {
            _dbreports = new DBReports();

        }



   

        public DataTable GetDailySales(DateTime reportdate)
         {
              return _dbreports.GetDailySales(reportdate);
         }


        public DataTable GetSalesByOrderType(DateTime startdate, DateTime enddate)
        {

            return _dbreports.GetSalesByOrderType(startdate, enddate);
        }

        public DataTable GetSalesByItem(DateTime startdate, DateTime enddate)
        {

            return _dbreports.GetSalesByItem(startdate, enddate);
        }

        public DataTable GetSalesByModifier(DateTime startdate, DateTime enddate)
        {

            return _dbreports.GetSalesByModifier(startdate, enddate);
        }

        public DataTable GetTicketVoids(DateTime startdate, DateTime enddate)
        {

            return _dbreports.GetTicketVoids(startdate, enddate);
        }

        public DataTable GetTicketItemVoids(DateTime startdate, DateTime enddate)
        {

            return _dbreports.GetTicketItemVoids(startdate, enddate);
        }


        public DataTable GetTicketPaymentVoids(DateTime startdate, DateTime enddate)
        {

            return _dbreports.GetTicketPaymentVoids(startdate, enddate);
        }


        public DataTable GetTicketPaymentRefunds(DateTime startdate, DateTime enddate)
        {

            return _dbreports.GetTicketPaymentRefunds(startdate, enddate);
        }

        public DataTable GetTicketDiscounts(DateTime startdate, DateTime enddate)
        {

            return _dbreports.GetTicketDiscounts(startdate, enddate);
        }


        public DataTable GetTicketItemDiscounts(DateTime startdate, DateTime enddate)
        {

            return _dbreports.GetTicketItemDiscounts(startdate, enddate);
        }

        public ObservableCollection<ReportGroup> GetReportGroupList(string grouptype)
        {
            ObservableCollection<ReportGroup> catlist = new ObservableCollection<ReportGroup>();
            DataTable dt;
            ReportGroup reportcat;

            dt = _dbreports.GetReportGroupList(grouptype);
            foreach (DataRow row in dt.Rows) //parse through each category of items sold
            {
                reportcat = new ReportGroup(row);
                catlist.Add(reportcat);
            }

            return catlist;
        }


        public int AddNewGroupList(string grouptype, string reportgroup)
        {
            return _dbreports.AddNewGroupList(grouptype, reportgroup);
        }

        public bool AddNewCatList(int reportgroupid, string reportcategory)
        {
            return _dbreports.AddNewCatList(reportgroupid, reportcategory);
        }
        public bool DeleteGroupList(int id)
        {
            return _dbreports.DeleteGroupList(id);
        }

        public bool DeleteCatList(int id)
        {
            return _dbreports.DeleteCatList(id);
        }

        public ObservableCollection<ReportCat> GetRevenueList()
         {
             ObservableCollection<ReportCat> revenuelist = new ObservableCollection<ReportCat>();
             DataTable dt;
             ReportCat reportcat;

             dt = _dbreports.GetRevenueList();
             foreach (DataRow row in dt.Rows) //parse through each category of items sold
             {
                 reportcat = new ReportCat();
                 reportcat.CatName = row["reportgroup"].ToString();
                 revenuelist.Add(reportcat);
             }

             return revenuelist;
         }


        public ObservableCollection<ReportCat> GetSettlementList()
        {
            ObservableCollection<ReportCat> settlementlist = new ObservableCollection<ReportCat>();
            DataTable dt;
            ReportCat reportcat;

            dt = _dbreports.GetSettlementList();
            foreach (DataRow row in dt.Rows) //parse through each category of items sold
            {
                reportcat = new ReportCat();
                reportcat.CatName = row["reportgroup"].ToString();
                settlementlist.Add(reportcat);
            }

            return settlementlist;
        }



        public DailyRevenue GetDailyRevenue(DateTime startdate, DateTime enddate)
        {

            DailyRevenue dailyreport;
            DataTable dt;
            DateTime reportdate;
            reportdate = startdate;
            ReportCat reportcat;
            decimal totalrevenue;



            Stopwatch stopwatch = Utility.GetStopwatch();

            dailyreport = new DailyRevenue();




            dailyreport.SalesCat = new ObservableCollection<ReportCat>();

            dailyreport.DOW = reportdate.DayOfWeek.ToString().Substring(0, 3).ToUpper();
            dailyreport.ReportDate = reportdate.ToString("M/d");

            //get the daily service/product sold
            dt = _dbreports.GetSalesRevenue(startdate, enddate);

            TouchMessageBox.Show((stopwatch.ElapsedMilliseconds / 1000).ToString() + " seconds to run");


            totalrevenue = 0;


            foreach (DataRow row in dt.Rows) //parse through each category of items sold
            {
                reportcat = new ReportCat();
                reportcat.CatName = row["reportgroup"].ToString();
                if (row["grossrevenue"].ToString() != "")
                {
                    reportcat.CatValue = decimal.Parse(row["grossrevenue"].ToString()); //revenue includes any item discounts already
                    reportcat.CatCount = decimal.Parse(row["qty"].ToString());

                }
                else reportcat.CatValue = 0;

                totalrevenue = totalrevenue + reportcat.CatValue;
                dailyreport.SalesCat.Add(reportcat);
            }


            dailyreport.TotalRevenue = totalrevenue;   
            dailyreport.TotalDiscount =(-1) * _dbreports.GetDailySalesDiscounts(startdate, enddate) + _dbreports.GetDailySalesAdjustments(startdate, enddate);
       
            dailyreport.SalesTax = _dbreports.GetDailySalesTax(startdate, enddate);
            dailyreport.TipsWithHeld = _dbreports.GetTipsWithHeld(startdate, enddate);
            dailyreport.AutoTips = _dbreports.GetAutoTips(startdate, enddate);


            return dailyreport;
        }
        public DailySettlement GetDailySettlement(DateTime startdate, DateTime enddate)
        {

            DailySettlement dailyreport;
            DataTable dt;


            ReportCat reportcat;
            decimal totalpayment;




            dailyreport = new DailySettlement();
            dailyreport.PaymentCat = new ObservableCollection<ReportCat>();

            dailyreport.DOW = startdate.DayOfWeek.ToString().Substring(0, 3).ToUpper();
            dailyreport.ReportDate = startdate.ToString("M/d");

            //get the daily service/product sold
            dt = _dbreports.GetSalesSettlement(startdate, enddate);

            totalpayment = 0;
            foreach (DataRow row in dt.Rows) //parse through each category of items sold
            {
                reportcat = new ReportCat();
                reportcat.CatName = row["reportgroup"].ToString();
                if (row["netamount"].ToString() != "") reportcat.CatValue = decimal.Parse(row["netamount"].ToString());
                else reportcat.CatValue = 0;
                totalpayment = totalpayment + reportcat.CatValue;
                dailyreport.PaymentCat.Add(reportcat);
            }



            dailyreport.TotalPayment = totalpayment;



            return dailyreport;
        }



        public ObservableCollection<DailyRevenue> GetWeeklyRevenue(DateTime startdate)
        {
            ObservableCollection<DailyRevenue> weeklyreport;
            weeklyreport = new ObservableCollection<DailyRevenue>();
            DailyRevenue dailyreport;
            DateTime reportdate;
            reportdate = startdate;



            for (int i = 0; i < 7; i++) //go through each day
            {
                dailyreport = GetDailyRevenue(reportdate, reportdate);
                weeklyreport.Add(dailyreport); //add the daily report to weekly report
                reportdate = reportdate.AddDays(1); //increment by one day
            }

            //add 8th column which is total of entire week
            dailyreport = GetDailyRevenue(startdate, startdate.AddDays(6));
            dailyreport.DOW = "";
            dailyreport.ReportDate = "W.T.D";
            weeklyreport.Add(dailyreport); //add the daily report to weekly report

            return weeklyreport;
        }


        public ObservableCollection<DailySettlement> GetWeeklySettlement(DateTime startdate)
        {
            ObservableCollection<DailySettlement> weeklyreport;
            weeklyreport = new ObservableCollection<DailySettlement>();
            DailySettlement dailyreport;

            DateTime reportdate;
            reportdate = startdate;


            for (int i = 0; i < 7; i++) //go through each day
            {
                dailyreport = GetDailySettlement(reportdate, reportdate);
                weeklyreport.Add(dailyreport); //add the daily report to weekly report
                reportdate = reportdate.AddDays(1);
            }

            //add 8th column which is total of entire week
            dailyreport = GetDailySettlement(startdate, startdate.AddDays(6));
            dailyreport.DOW = "";
            dailyreport.ReportDate = "W.T.D";
            weeklyreport.Add(dailyreport); //add the daily report to weekly report
            return weeklyreport;
        }


        public ObservableCollection<DailyRevenue> GetMonthlyRevenue(DateTime startdate)
        {
            ObservableCollection<DailyRevenue> monthlyreport;
            monthlyreport = new ObservableCollection<DailyRevenue>();
            DailyRevenue dailyreport;
            DateTime reportdate;
            reportdate = startdate;
            int days = DateTime.DaysInMonth(startdate.Year, startdate.Month);


           
            for (int i = 0; i < days; i++) //go through each day
            {
                dailyreport = GetDailyRevenue(reportdate, reportdate);
                monthlyreport.Add(dailyreport); //add the daily report to weekly report
                reportdate = reportdate.AddDays(1); //increment by one day
            }

           

            //add last column which is total of entire month
            dailyreport = GetDailyRevenue(startdate, startdate.AddDays(days - 1));
            dailyreport.DOW = "";
            dailyreport.ReportDate = "M.T.D";
            monthlyreport.Add(dailyreport); //add the daily report to weekly report

            return monthlyreport;
        }


        public ObservableCollection<DailySettlement> GetMonthlySettlement(DateTime startdate)
        {
            ObservableCollection<DailySettlement> monthlyreport;
            monthlyreport = new ObservableCollection<DailySettlement>();
            DailySettlement dailyreport;
            DateTime reportdate;
            reportdate = startdate;
            int days = DateTime.DaysInMonth(startdate.Year, startdate.Month);
            for (int i = 0; i < days; i++) //go through each day
            {
                dailyreport = GetDailySettlement(reportdate, reportdate);
                monthlyreport.Add(dailyreport); //add the daily report to weekly report
                reportdate = reportdate.AddDays(1);
            }

            //add 8th column which is total of entire week
            dailyreport = GetDailySettlement(startdate, startdate.AddDays(days - 1));
            dailyreport.DOW = "";
            dailyreport.ReportDate = "M.T.D";
            monthlyreport.Add(dailyreport); //add the daily report to weekly report
            return monthlyreport;
        }


        public ObservableCollection<DailyRevenue> GetCustomRevenue(DateTime startdate, DateTime enddate)
        {
            ObservableCollection<DailyRevenue> customreport;
            customreport = new ObservableCollection<DailyRevenue>();
            DailyRevenue dailyreport;
            DateTime reportdate;
            reportdate = startdate;
            TimeSpan ts = enddate - startdate;
            int days = ts.Days;

            for (int i = 0; i <= days; i++) //go through each day
            {
                dailyreport = GetDailyRevenue(reportdate, reportdate);
                customreport.Add(dailyreport); //add the daily report to weekly report
                reportdate = reportdate.AddDays(1); //increment by one day
            }

            //add 8th column which is total of entire week
            dailyreport = GetDailyRevenue(startdate, enddate);
            dailyreport.DOW = "";
            dailyreport.ReportDate = "Total";
            customreport.Add(dailyreport); //add the daily report to weekly report

            return customreport;
        }


        public ObservableCollection<DailySettlement> GetCustomSettlement(DateTime startdate, DateTime enddate)
        {
            ObservableCollection<DailySettlement> customreport;
            customreport = new ObservableCollection<DailySettlement>();
            DailySettlement dailyreport;
            DateTime reportdate;
            reportdate = startdate;
            TimeSpan ts = enddate - startdate;
            int days = ts.Days;


            for (int i = 0; i <= days; i++) //go through each day
            {
                dailyreport = GetDailySettlement(reportdate, reportdate);
                customreport.Add(dailyreport); //add the daily report to weekly report
                reportdate = reportdate.AddDays(1);
            }

            //add 8th column which is total of entire week
            dailyreport = GetDailySettlement(startdate, enddate);
            dailyreport.DOW = "";
            dailyreport.ReportDate = "Total";
            customreport.Add(dailyreport); //add the daily report to weekly report
            return customreport;
        }



        public void PrintWeeklySales(DateTime startdate)
        {
            try
            {

                PrintSales(startdate, startdate.AddDays(6), "Weekly Sales Report");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Print Weekly Sales Report:" + e.Message);
            }



        }


        public void PrintDailySales(DateTime startdate)
        {
            try
            {

                PrintSales(startdate, startdate, "Daily Sales Report");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Print Daily Sales Report:" + e.Message);
            }



        }

        public void PrintMonthlySales(DateTime startdate, DateTime enddate)
        {
            try
            {

                PrintSales(startdate, enddate, "Monthly Sales Report");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Print Monthly Sales Report:" + e.Message);
            }



        }


        public void PrintCustomSales(DateTime startdate, DateTime enddate)
        {
            try
            {

                PrintSales(startdate, enddate, "Custom Sales Report");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Print Custom Sales Report:" + e.Message);
            }



        }

        public void PrintSales(DateTime startdate, DateTime enddate, string ReportTitle)
        {
            try
            {

                int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
                string printername = GlobalSettings.Instance.ReceiptPrinter;
                Location store = GlobalSettings.Instance.Shop;
                string m_mode = GlobalSettings.Instance.ReceiptPrinterMode;

                //defaults to receipt width since some customer had the character width setup.
                int receiptchars = receiptwidth;

                if (receiptwidth == 58) receiptchars = 32;
                if (receiptwidth == 80) receiptchars = 48;



                if (printername == "none") return;


                //58mm printer = 32 chars  , 80mm printer = 48 chars
                ReceiptPrinter printer = new ReceiptPrinter(printername, m_mode);

                printer.Center();
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.DoubleHeight();
                printer.PrintLF(ReportTitle);
                printer.LineFeed();
                if (startdate == enddate)
                {
                    printer.PrintLF(startdate.ToShortDateString());
                }
                else
                {
                    printer.PrintLF(startdate.ToShortDateString() + "-" + enddate.ToShortDateString());
                }

                printer.DoubleHeightOFF();
                printer.LineFeed();
                printer.Left();

                printer.PrintLF(new String('=', receiptchars));
                printer.LineFeed();
                printer.LineFeed();
                printer.DoubleHeight();
                printer.PrintLF("Revenue");
                printer.DoubleHeightOFF();

                printer.LineFeed();






                DailyRevenue dailyrevenue;
                dailyrevenue = GetDailyRevenue(startdate, enddate);

                foreach (ReportCat rp in dailyrevenue.SalesCat)
                {
                    printer.PrintLF(Utility.FormatPrintRow(rp.CatName, string.Format("{0:0.00}", rp.CatValue), receiptchars));

                }
                printer.PrintLF(Utility.FormatPrintRow("Total Tax:", string.Format("{0:0.00}", dailyrevenue.SalesTax), receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("Tips Withheld:", string.Format("{0:0.00}", dailyrevenue.TipsWithHeld), receiptchars));

                printer.PrintLF(Utility.FormatPrintRow("Total Discount:", string.Format("{0:0.00}", dailyrevenue.TotalDiscount), receiptchars));

                printer.PrintLF(new String('-', receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("Total Revenue:", string.Format("{0:0.00}", dailyrevenue.NetRevenue), receiptchars));


                printer.LineFeed();
                printer.LineFeed();
                printer.DoubleHeight();
                printer.PrintLF("Settlement");
                printer.DoubleHeightOFF();

                printer.LineFeed();
                DailySettlement dailypayment;
                dailypayment = GetDailySettlement(startdate, enddate);

                //decimal totalreceived = 0;
                foreach (ReportCat rp in dailypayment.PaymentCat)
                {
                    printer.PrintLF(Utility.FormatPrintRow(rp.CatName, rp.CatValue.ToString(), receiptchars));
                }


                printer.PrintLF(new String('-', receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("Total Settlements:", string.Format("{0:0.00}", dailypayment.TotalPayment), receiptchars));
                printer.LineFeed();
                printer.LineFeed();


                printer.Send(); //sends buffer to printer

                printer.Cut();

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Print Sales Report:" + e.Message);
            }



        }

        public void PrintCashierOut(CashDrawer drawer, decimal diff)
        {
            try
            {

                int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
                string printername = GlobalSettings.Instance.ReceiptPrinter;
                Location store = GlobalSettings.Instance.Shop;
                string m_mode = GlobalSettings.Instance.ReceiptPrinterMode;

                //defaults to receipt width since some customer had the character width setup.
                int receiptchars = receiptwidth;

                if (receiptwidth == 58) receiptchars = 32;
                if (receiptwidth == 80) receiptchars = 48;



                if (printername == "none") return;


                //58mm printer = 32 chars  , 80mm printer = 48 chars
                ReceiptPrinter printer = new ReceiptPrinter(printername,m_mode);

                printer.Center();
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.DoubleHeight();
                printer.PrintLF("Cashier In/Out");
                printer.DoubleHeightOFF();
                printer.LineFeed();
           
                printer.PrintLF(drawer.CashIn.CashDate.ToString() + "-" + drawer.CashOut.CashDate.ToString());
               

             
                printer.LineFeed();
                printer.Left();

                printer.PrintLF(new String('=', receiptchars));
                printer.LineFeed();
       
                printer.PrintLF(Utility.FormatPrintRow("Cash In:", string.Format("{0:0.00}", drawer.CashIn.CashTotal), receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("Cash Out:", string.Format("{0:0.00}", drawer.CashOut.CashTotal), receiptchars));

          

                if(diff > 0)
                {
                    printer.PrintLF(Utility.FormatPrintRow("Cash Over:", string.Format("{0:0.00}", diff), receiptchars));
                    printer.PrintLF("Note:" + drawer.Note);
                }


                if (diff < 0)
                {
                    printer.PrintLF(Utility.FormatPrintRow("Cash Short:", string.Format("{0:0.00}", diff), receiptchars));
                    printer.PrintLF("Note:" + drawer.Note);
                }


                printer.LineFeed();


                printer.Send(); //sends buffer to printer

                printer.Cut();

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Print Cashier In/Out:" + e.Message);
            }



        }


        //-----------------------------------------------------------------------------EXPORT Sales -------------------------

        public void ExportSalesCSV(DateTime startdate, DateTime enddate)
        {
            try
            {
                DataTable dt = _dbreports.GetSalesSummary(startdate, enddate);
                SaveFileDialog ofd = new SaveFileDialog();
                ofd.DefaultExt = "csv";
                ofd.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
               
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    CSVWriter.WriteDataTable(dt, ofd.FileName, true);

                }
            }
            catch (Exception ex)
            {
                TouchMessageBox.Show(ex.Message);
            }



        }


        public void ExportSalesDetailCSV(DateTime startdate, DateTime enddate)
        {
            try
            {
                DataTable dt = _dbreports.GetSalesTickets(startdate, enddate);
                SaveFileDialog ofd = new SaveFileDialog();
                ofd.DefaultExt = "csv";
                ofd.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    CSVWriter.WriteDataTable(dt, ofd.FileName, true);

                }

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show(ex.Message);
            }
        }




        //-----------------------Print By Order Type --------------------------------------------------------
        public void PrintByOrderType(DateTime startdate, DateTime enddate)
        {


            try
            {

                int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
                string printername = GlobalSettings.Instance.ReceiptPrinter;
                Location store = GlobalSettings.Instance.Shop;
                string m_mode = GlobalSettings.Instance.ReceiptPrinterMode;

                //defaults to receipt width since some customer had the character width setup.
                int receiptchars = receiptwidth;
                //translate to chars if value is in millimeters
                //58mm printer = 32 chars  , 80mm printer = 48 chars
                if (receiptwidth == 58) receiptchars = 32;
                if (receiptwidth == 80) receiptchars = 48;



                if (printername == "none") return;


                //58mm printer = 32 chars  , 80mm printer = 48 chars
                ReceiptPrinter printer = new ReceiptPrinter(printername, m_mode);

                printer.Center();
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.DoubleHeight();
                printer.PrintLF("Sales By Order Type");
                if (startdate == enddate)
                {
                    printer.PrintLF(startdate.ToShortDateString());
                }
                else
                {
                    printer.PrintLF(startdate.ToShortDateString() + "-" + enddate.ToShortDateString());
                }

                printer.DoubleHeightOFF();
                printer.PrintLF(new String('=', receiptchars));

                printer.LineFeed();


                DataTable dt = _dbreports.GetSalesByOrderType(startdate, enddate);

                printer.PrintLF("OrderType    Total    Count  Average  Percent");
                printer.PrintLF(new String('-', receiptchars));

                decimal total = 0;
                decimal ticketcount = 0;
                decimal averageticket = 0;
                decimal percenttotal = 0;

                foreach (DataRow row in dt.Rows)
                {
                    printer.PrintLF(row["ordertype"].ToString().PadRight(10) + row["total"].ToString().PadLeft(8) + row["ticketcount"].ToString().PadLeft(8) + row["avgticketamount"].ToString().PadLeft(8) + row["percentage"].ToString().PadLeft(8));

                    total += decimal.Parse(row["total"].ToString());
                    ticketcount += decimal.Parse(row["ticketcount"].ToString());
                   // averageticket += decimal.Parse(row["avgticketamount"].ToString());
                    percenttotal += decimal.Parse(row["percentage"].ToString());
                }

                averageticket = Math.Round(total / ticketcount, 2);
                /*
                foreach (DataRow row in dt.Rows)
                {
                    printer.PrintLF(Utility.FormatPrintRow("Type:", row["ordertype"].ToString(), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Total:", row["total"].ToString(), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Ticket Count:", row["ticketcount"].ToString(), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Avg Amount:", row["avgticketamount"].ToString(), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Percent:", row["percentage"].ToString(), receiptchars));
                }
                */
      
               // printer.LineFeed();

                printer.PrintLF(new String('-', receiptchars));

                printer.PrintLF("          " + total.ToString().PadLeft(8) + ticketcount.ToString().PadLeft(8) + averageticket.ToString().PadLeft(8) + percenttotal.ToString().PadLeft(8));

                printer.LineFeed();
                printer.LineFeed();
                printer.LineFeed();

                printer.Send(); //sends buffer to printer

                printer.Cut();

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Print Sales Report:" + e.Message);
            }


        }

        public void ExportByOrderType(DateTime startdate, DateTime enddate)
        {
            try
            {
                DataTable dt = _dbreports.GetSalesByOrderType(startdate, enddate);
                SaveFileDialog ofd = new SaveFileDialog();
                ofd.DefaultExt = "csv";
                ofd.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    CSVWriter.WriteDataTable(dt, ofd.FileName, true);

                }
            }
            catch (Exception ex)
            {
                TouchMessageBox.Show(ex.Message);
            }
        }


        //------------------------Print by Item type ---------------------
        public void PrintByItem(DateTime startdate, DateTime enddate)
        {


            try
            {

                int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
                string printername = GlobalSettings.Instance.ReceiptPrinter;
                Location store = GlobalSettings.Instance.Shop;
                string m_mode = GlobalSettings.Instance.ReceiptPrinterMode;

                //defaults to receipt width since some customer had the character width setup.
                int receiptchars = receiptwidth;
                //translate to chars if value is in millimeters
                //58mm printer = 32 chars  , 80mm printer = 48 chars
                if (receiptwidth == 58) receiptchars = 32;
                if (receiptwidth == 80) receiptchars = 48;



                if (printername == "none") return;


                //58mm printer = 32 chars  , 80mm printer = 48 chars
                ReceiptPrinter printer = new ReceiptPrinter(printername, m_mode);

                printer.Center();
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.DoubleHeight();
                printer.PrintLF("Sales By Item");
                if (startdate == enddate)
                {
                    printer.PrintLF(startdate.ToShortDateString());
                }
                else
                {
                    printer.PrintLF(startdate.ToShortDateString() + "-" + enddate.ToShortDateString());
                }

                printer.DoubleHeightOFF();
                printer.PrintLF(new String('=', receiptchars));

                printer.LineFeed();


                DataTable dt = _dbreports.GetSalesByItem(startdate, enddate);

                printer.PrintLF("Item Description".PadRight(25) + "Qty/Weight".PadLeft(10) + "Total Amt".PadLeft(10));
                printer.PrintLF(new String('-', receiptchars));

                decimal totalamount = 0;
                decimal totalquantity = 0;

                foreach (DataRow row in dt.Rows)
                {
                    printer.PrintLF(row["description"].ToString().PadRight(25).Substring(0, 25) + String.Format("{0:0.00}", row["totalquantity"]).PadLeft(10) + String.Format("{0:0.00}", row["totalamount"]).PadLeft(10));

                    totalquantity += decimal.Parse(row["totalquantity"].ToString());
                    totalamount += decimal.Parse(row["totalamount"].ToString());
                  
                }


                printer.PrintLF(new String('-', receiptchars));

                printer.PrintLF(String.Empty.PadRight(25) + String.Format("{0:0.00}", totalquantity).PadLeft(10) + String.Format("{0:0.00}", totalamount).PadLeft(10));

                printer.LineFeed();
                printer.LineFeed();
                printer.LineFeed();

                printer.Send(); //sends buffer to printer

                printer.Cut();

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Print Item Report:" + e.Message);
            }


        }


        public void ExportByItem(DateTime startdate, DateTime enddate)
        {
            try
            {
                DataTable dt = _dbreports.GetSalesByItem(startdate, enddate);
                SaveFileDialog ofd = new SaveFileDialog();
                ofd.DefaultExt = "csv";
                ofd.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    CSVWriter.WriteDataTable(dt, ofd.FileName, true);

                }
            }
            catch (Exception ex)
            {
                TouchMessageBox.Show(ex.Message);
            }
        }
    }


    public class DailyRevenue
    {
        public string DOW { get; set; }
        public string ReportDate { get; set; }
        public ObservableCollection<ReportCat> SalesCat { get; set; }

        public decimal SalesTax { get; set; }
        public decimal TipsWithHeld { get; set; }
        public decimal AutoTips { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal ShopFee { get; set; }
        public decimal TotalDiscount { get; set; }
    


        public decimal NetRevenue { get { return TotalRevenue + AutoTips + TipsWithHeld + SalesTax + TotalDiscount ; } }

    }


    public class DailySettlement
    {
        public string DOW { get; set; }
        public string ReportDate { get; set; }
        public ObservableCollection<ReportCat> PaymentCat { get; set; }
     
        public decimal TotalPayment { get; set; }

    }

    public class Difference
    {
        public decimal NetRevenue { get; set; }
        public decimal NetPayment { get; set; }
        public decimal NetDifference { get { return NetPayment- NetRevenue ; } }
    }
    public class ReportCat
    {
        public string CatName {get;set;}
        public decimal CatCount { get; set; }
        public decimal CatValue {get;set;}
    }
}
