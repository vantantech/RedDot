using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace RedDot
{
    public class Reports
    {
        DBReports _dbreports;
        public Reports()
        {
            _dbreports = new DBReports();

        }

        public DataTable GetInventoryReport(DateTime startdate, DateTime enddate)
        {
            return _dbreports.GetInventory(startdate,enddate);
        }

        public DataTable GetShippingReport(DateTime startdate, DateTime enddate)
        {
            return _dbreports.GetShipping(startdate, enddate);
        }

        public DataTable GetShippingReportDelayed()
        {
            return _dbreports.GetShippingDelayed();
        }

        public DataTable GetProductSalesReport(DateTime startdate, DateTime enddate)
        {
            return _dbreports.GetProductSales(startdate,enddate);
        }

        public DataTable GetWorkedEmployees(DateTime startdate, DateTime enddate)
        {

            return _dbreports.GetRetailWorkedEmployees(startdate, enddate);
        }

        public DataTable GetDailySales(DateTime reportdate)
        {
             return _dbreports.GetRetailSales(reportdate,reportdate);
           
        }

        public DataTable GetMonthlySales(DateTime reportdate)
        {

            int days = DateTime.DaysInMonth(reportdate.Year, reportdate.Month);
            DateTime enddate = reportdate.AddDays(days - 1);

            return _dbreports.GetRetailSales(reportdate, enddate);

        }

        public DataTable GetMonthlySummary(DateTime reportdate)
        {

            return _dbreports.GetMonthlySummary(reportdate);
        }


        public DataTable GetPayments(DateTime startdate, DateTime enddate)
        {
            return _dbreports.GetPayments(startdate, enddate);
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
                reportcat.CatName = row["revenuecategory"].ToString();
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
                reportcat.CatName = row["settlementcategory"].ToString();
                settlementlist.Add(reportcat);
            }

            return settlementlist;
        }

        public DailyRevenue GetDailyRevenue(DateTime startdate, DateTime enddate)
        {

            DailyRevenue dailyreport;
            DataTable dt;
            ReportCat reportcat;
            decimal totalrevenue;





            dailyreport = new DailyRevenue();
            dailyreport.SalesCat = new ObservableCollection<ReportCat>();

            dailyreport.DOW = startdate.DayOfWeek.ToString().Substring(0, 3).ToUpper();
            dailyreport.ReportDate = startdate.ToString("M/d");

            //get the daily service/product sold
            dt = _dbreports.GetRetailDailyRevenue(startdate, enddate);


            totalrevenue = 0;


            foreach (DataRow row in dt.Rows) //parse through each category of items sold
            {
                reportcat = new ReportCat();
                reportcat.CatName = row["reportcategory"].ToString();
                if (row["revenue"].ToString() != "")
                {
                    reportcat.CatValue = decimal.Parse(row["revenue"].ToString()); //revenue includes any item discounts already

                }
                else reportcat.CatValue = 0;

                totalrevenue = totalrevenue + reportcat.CatValue;
                dailyreport.SalesCat.Add(reportcat);
            }
            dailyreport.ItemTotal = totalrevenue;


            dailyreport.ShopFee = _dbreports.GetDailyShopFee(startdate, enddate);
            dailyreport.Discount = _dbreports.GetDailySalesDiscount(startdate, enddate);  //this discount is for COMPs / ticket discount , not item discount
            dailyreport.SalesTax = _dbreports.GetDailySalesTax(startdate, enddate);


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
            dt = _dbreports.GetDailySettlement(startdate, enddate);
 
            totalpayment = 0;
            foreach (DataRow row in dt.Rows) //parse through each category of items sold
            {
                reportcat = new ReportCat();
                reportcat.CatName = row["reportcategory"].ToString();
                if (row["netamount"].ToString() != "") reportcat.CatValue = decimal.Parse(row["netamount"].ToString());
                else reportcat.CatValue = 0;
                totalpayment = totalpayment + reportcat.CatValue;
                dailyreport.PaymentCat.Add(reportcat);
            }



            dailyreport.TotalPayment = totalpayment;



            return dailyreport;
        }

        public DailySettlement GetDailyPayment(DateTime startdate, DateTime enddate)
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
            dt = _dbreports.GetDailyPayments(startdate, enddate);

            totalpayment = 0;
            foreach (DataRow row in dt.Rows) //parse through each category of items sold
            {
                reportcat = new ReportCat();
                reportcat.CatName = row["reportcategory"].ToString();
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
                dailyreport = GetDailySettlement(reportdate,reportdate);
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



        public ObservableCollection<DailySettlement> GetWeeklyPayment(DateTime startdate)
        {
            ObservableCollection<DailySettlement> weeklyreport;
            weeklyreport = new ObservableCollection<DailySettlement>();
            DailySettlement dailyreport;
            DateTime reportdate;
            reportdate = startdate;

            for (int i = 0; i < 7; i++) //go through each day
            {
                dailyreport = GetDailyPayment(reportdate, reportdate);
                weeklyreport.Add(dailyreport); //add the daily report to weekly report
                reportdate = reportdate.AddDays(1); //increment by one day
            }

            //add 8th column which is total of entire week
            dailyreport = GetDailyPayment(startdate, startdate.AddDays(6));
            dailyreport.DOW = "";
            dailyreport.ReportDate = "W.T.D";
            weeklyreport.Add(dailyreport); //add the daily report to weekly report

            return weeklyreport;
        }




        public ObservableCollection<DailyRevenue> GetSemiMonthlyRevenue(DateTime startdate)
        {
            ObservableCollection<DailyRevenue> weeklyreport;
            weeklyreport = new ObservableCollection<DailyRevenue>();
            DailyRevenue dailyreport;
            DateTime reportdate;
            reportdate = startdate;
 
            int numOfDays;

            if (startdate.Day == 1)
            {
                numOfDays = 15;
            }
            else
            {
                //second half of month
                DateTime temp;
                temp = startdate.AddMonths(1).AddDays(-16);  //find last day of month
                numOfDays = temp.Day - 15;
            }


            for (int i = 0; i < numOfDays; i++) //go through each day
            {

                dailyreport = GetDailyRevenue(reportdate, reportdate);
                weeklyreport.Add(dailyreport); //add the daily report to weekly report
                reportdate = reportdate.AddDays(1);
            }

            //add last column which is total of entire month
            dailyreport = GetDailyRevenue(startdate, startdate.AddDays(numOfDays-1));
            dailyreport.DOW = "";
            dailyreport.ReportDate = "M.T.D";
            weeklyreport.Add(dailyreport); //add the daily report to weekly report  


            return weeklyreport;
        }


        public ObservableCollection<DailySettlement> GetSemiMonthlySettlement(DateTime startdate)
        {
            ObservableCollection<DailySettlement> weeklyreport;
            weeklyreport = new ObservableCollection<DailySettlement>();
            DailySettlement dailyreport;
            DateTime reportdate;
            reportdate = startdate;
            int numOfDays;

            if (startdate.Day == 1)
            {
                numOfDays = 15;
            }
            else
            {
                //second half of month
                DateTime temp;
                temp = startdate.AddMonths(1).AddDays(-16);  //find last day of month
                numOfDays = temp.Day - 15;
            }


            for (int i = 0; i < numOfDays; i++) //go through each day
            {

                dailyreport = GetDailySettlement(reportdate, reportdate);
                weeklyreport.Add(dailyreport); //add the daily report to weekly report
                reportdate = reportdate.AddDays(1);
            }

            //add 8th column which is total of entire week
            dailyreport = GetDailySettlement(startdate, startdate.AddDays(numOfDays));
            dailyreport.DOW = "";
            dailyreport.ReportDate = "M.T.D";
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

            //add 8th column which is total of entire week
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

        public ObservableCollection<DailySettlement> GetMonthlyPayment(DateTime startdate)
        {
            ObservableCollection<DailySettlement> monthlyreport;
            monthlyreport = new ObservableCollection<DailySettlement>();
            DailySettlement dailyreport;
            DateTime reportdate;
            reportdate = startdate;
            int days = DateTime.DaysInMonth(startdate.Year, startdate.Month);
            for (int i = 0; i < days; i++) //go through each day
            {
                dailyreport = GetDailyPayment(reportdate, reportdate);
                monthlyreport.Add(dailyreport); //add the daily report to weekly report
                reportdate = reportdate.AddDays(1);
            }

            //add 8th column which is total of entire week
            dailyreport = GetDailyPayment(startdate, startdate.AddDays(days - 1));
            dailyreport.DOW = "";
            dailyreport.ReportDate = "M.T.D";
            monthlyreport.Add(dailyreport); //add the daily report to weekly report
            return monthlyreport;
        }
        //Gets All tickets for ONE employee
        public ObservableCollection<SalesData> GetEmployeeCommission(Employee employee, DateTime startdate, DateTime enddate)
        {

            ObservableCollection<RetailLineItem> itemdata;
            int lastid = 0;
            int lastemployeeid = 0;
            int currentid = 0;
            decimal totalsales = 0;
            decimal totalcost = 0;
            decimal totalmargin = 0;
            decimal totalcommission = 0;
            RetailLineItem line;
            SalesData salesrecord;
            ObservableCollection<SalesData> employeesales;



            //Actual list of tickets in above object
            employeesales = new ObservableCollection<SalesData>();
            itemdata = new ObservableCollection<RetailLineItem>();
            salesrecord = new SalesData();
            salesrecord.CustomerName = "";
            salesrecord.PaymentType = "";


            int i = 0;
            decimal adjustment = 0;

            DataTable dt = _dbreports.GetEmployeeSalesCommissionByID(employee.ID, startdate, enddate);


            foreach (DataRow row in dt.Rows)
            {
                currentid = (int)row["SalesID"];
                if (currentid != lastid)
                {
                    //add record from previous line into collection
                    if (i > 0)
                    {

                        salesrecord.TotalSales = totalsales;  //total sales for each ticket
                        salesrecord.TotalCost = totalcost;
                        salesrecord.TotalMargin = totalmargin;
                        adjustment = salesrecord.TotalAdjustments * employee.CommissionProduct / 100;
                        salesrecord.TotalCommissionAdjustment = adjustment;
                        salesrecord.TotalCommission = totalcommission;
                        salesrecord.SalesItem = itemdata;

                        employeesales.Add(salesrecord);  //Add ticket to employee list
                    }


                    i++;
                    //first line item in the ticket
                    totalsales = 0;
                    totalcost = 0;
                    totalmargin = 0;
                    totalcommission = 0;
                    salesrecord = new SalesData();
                    salesrecord.CloseDate = (DateTime)row["closedate"];
                    salesrecord.PaymentType = row["paymenttype"].ToString();
                    salesrecord.SalesID = (int)row["SalesID"];
                    salesrecord.CustomerName = row["customername"].ToString();

                    if (row["adjustment"].ToString() != "") salesrecord.TotalAdjustments = (decimal)row["adjustment"];
                    else salesrecord.TotalAdjustments = 0;

                    itemdata = new ObservableCollection<RetailLineItem>();
                    line = new RetailLineItem(row,employee.CommissionProduct,employee.CommissionLabor,employee.Role == "Sales Rep");
                    itemdata.Add(line);
                    totalsales = totalsales + line.TotalAdjustedPrice;
                     totalcost = totalcost + line.TotalCost;
                     totalcommission = totalcommission + line.TotalCommission;
                     totalmargin = totalmargin + line.TotalMargin;


                }
                else
                {
                    //additional items on the ticket
                    line = new RetailLineItem(row, employee.CommissionProduct, employee.CommissionLabor, employee.Role == "Sales Rep"); 
                    itemdata.Add(line);
                    totalsales = totalsales + line.TotalAdjustedPrice;
                     totalcost = totalcost + line.TotalCost;
                     totalcommission = totalcommission + line.TotalCommission;
                     totalmargin = totalmargin + line.TotalMargin;
                }


                lastid = currentid;
                lastemployeeid = (int)row["salesemployeeid"];
            }

            //need to add last record

            salesrecord.TotalSales = totalsales;  //total sales for each ticket
            salesrecord.TotalCost = totalcost;
            salesrecord.TotalMargin = totalmargin;
            adjustment = salesrecord.TotalAdjustments * employee.CommissionProduct / 100;
            salesrecord.TotalCommissionAdjustment = adjustment;
            salesrecord.TotalCommission = totalcommission;
            salesrecord.SalesItem = itemdata;


            employeesales.Add(salesrecord);  //Add ticket to employee list


            return employeesales;
        }



        public void ExportCommissionCSV(int employeeid, DateTime startdate, DateTime enddate)
        {


            DataTable dt = _dbreports.GetEmployeeSalesCommissionByID(employeeid, startdate, enddate);
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.DefaultExt = "csv";
            ofd.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CSVWriter.WriteDataTable(dt, ofd.FileName, true);


            }


        }

        public void ExportDailySalesCSV(DateTime startdate)
        {

            DataTable dt = _dbreports.GetRetailSales(startdate, startdate);
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.DefaultExt = "csv";
            ofd.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CSVWriter.WriteDataTable(dt, ofd.FileName, true);

            }


        }


        public void ExportMonthlySalesCSV(DateTime startdate)
        {
            int days = DateTime.DaysInMonth(startdate.Year, startdate.Month);
            DataTable dt = _dbreports.GetRetailSales(startdate, startdate.AddDays(days));
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.DefaultExt = "csv";
            ofd.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CSVWriter.WriteDataTable(dt, ofd.FileName, true);

            }


        }


        public void PrintCommissionPDF(ObservableCollection<EmployeeSalesData> reportlist, ReportDate reportdate, bool display= true)
        {


            XFont fontsmallitalic = new XFont("Courier New", 6, XFontStyle.Italic | XFontStyle.Bold);

            XFont font = new XFont("Courier New", 10, XFontStyle.Bold);
            // Font font = new Font("Courier New", 8, System.Drawing.FontStyle.Bold);
            // XFont font = new XFont("Times New Roman", 10, XFontStyle.Bold);

            XFont fontitalic = new XFont("Courier New",10, XFontStyle.Italic | XFontStyle.Bold);
            XFont fontbold = new XFont("Courier New", 12, XFontStyle.Bold);
            XFont fonttitle = new XFont("Courier New", 15, XFontStyle.Bold);



            // Create pen.
            Pen blackPen = new Pen(Color.Black, 2);
            Brush blackBrush = new SolidBrush(Color.Black);
            Location store = GlobalSettings.Instance.Shop;

            int fontHeight = (int)font.GetHeight();
            int fontBoldHeight = (int)fontbold.GetHeight();

            int startX = 50;
            int startY = 20;
            int XOffset = 25;
            int YOffset = 0;




            PdfDocument document = new PdfDocument();

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XTextFormatter tf = new XTextFormatter(gfx);
            XRect rect = new XRect(0, 0, page.Width, page.Height);


            try
            {

                decimal Total = 0;
                decimal Cost = 0;
                decimal Commission = 0;
                int counter = 0;

                decimal GrandTotal = 0;
                decimal GrandCost = 0;
                decimal GrandCommission = 0;

                string bitmapfile = GlobalSettings.Instance.StoreLogo;
                if (File.Exists(bitmapfile))
                    if (GlobalSettings.Instance.StoreLogo != "") gfx.DrawImage(new Bitmap(bitmapfile), startX, startY, 200, 40);


                YOffset = 80;
                gfx.DrawString("Sales Commission Report",fonttitle, new SolidBrush(Color.Black), XOffset, YOffset);
               // gfx.DrawString(reportdate.ReportString, fontbold, new SolidBrush(Color.Black), XOffset, YOffset);
                PrintRightAlign(gfx, reportdate.ReportString, fontbold,500, YOffset);

                YOffset = 100;


                foreach (EmployeeSalesData report in reportlist)
                {

                    if (YOffset > 700)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        YOffset = 50;
                    }


                    gfx.DrawString(report.CurrentEmployee.FirstName + " " + report.CurrentEmployee.LastName , fontbold, new SolidBrush(Color.Black), XOffset , YOffset);
  


                    YOffset = YOffset + fontHeight * 2;



                    gfx.DrawString("Date", font, new SolidBrush(Color.Black), XOffset, YOffset);
                    gfx.DrawString("Invoice#", font, new SolidBrush(Color.Black), XOffset + 80, YOffset);
                    gfx.DrawString("Account Name ", font, new SolidBrush(Color.Black), XOffset + 150, YOffset);
                    gfx.DrawString("Sales Total", font, new SolidBrush(Color.Black), XOffset + 350, YOffset);
                    gfx.DrawString("Commission Total", font, new SolidBrush(Color.Black), XOffset + 450, YOffset);
                    gfx.DrawString("_______________________________________________________________________________________________________", font, new SolidBrush(Color.Black), XOffset, YOffset);

                    YOffset = YOffset + fontHeight;

                    foreach (SalesData dat in report.EmployeeSales)
                    {
                        gfx.DrawString( dat.CloseDate.ToShortDateString(), font, new SolidBrush(Color.Black), XOffset, YOffset);
                        gfx.DrawString(dat.SalesID.ToString(), font, new SolidBrush(Color.Black), XOffset + 80, YOffset);
                        gfx.DrawString(dat.CustomerName, font, new SolidBrush(Color.Black), XOffset + 150, YOffset);
                        gfx.DrawString(dat.TotalSales.ToString(), font, new SolidBrush(Color.Black), XOffset + 350, YOffset);
                        gfx.DrawString(dat.TotalCommission.ToString(), font, new SolidBrush(Color.Black), XOffset + 450, YOffset);


                        Total = Total + dat.TotalSales;
                        Cost = Cost + dat.TotalCost;
                        Commission += dat.TotalCommission;

                        YOffset = YOffset + fontHeight;

                    }

                    gfx.DrawString("========================================================================================================", font, new SolidBrush(Color.Black), XOffset, YOffset);

                    YOffset = YOffset + fontHeight;

                    gfx.DrawString("Total", font, new SolidBrush(Color.Black), XOffset + 150, YOffset);
                    gfx.DrawString(Total.ToString() , font, new SolidBrush(Color.Black), XOffset + 350, YOffset);
                    gfx.DrawString( Math.Round(Commission, 2).ToString(), font, new SolidBrush(Color.Black), XOffset + 450, YOffset);



                    YOffset = YOffset + fontHeight * 4;

                    counter++;
                    GrandTotal = GrandTotal + Total;
                    GrandCost = GrandCost + Cost;
                    GrandCommission += Commission;
                    Total = 0;
                    Cost = 0;
                    Commission = 0;

                    if (reportlist.Count == 1)
                    {
                        // Save the document...
                        string filename_1 = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\pdf\\CommissionReport_" + report.CurrentEmployee.FirstName + "_" + report.CurrentEmployee.LastName + "_" + reportdate.DateString + ".pdf";
                        document.Save(filename_1);


                        // ...and start a viewer.
                        if (display)
                            Process.Start(filename_1);
                        return;
                    }
                }

                YOffset = YOffset + fontHeight;

                if (counter > 1)
                {
                    gfx.DrawString("========================================================================================================", font, new SolidBrush(Color.Black), XOffset, YOffset);
                    YOffset = YOffset + fontHeight;

                  
                    gfx.DrawString("Grand Total", font, new SolidBrush(Color.Black), XOffset + 150, YOffset);
                    gfx.DrawString(GrandTotal.ToString(), font, new SolidBrush(Color.Black), XOffset + 350, YOffset);
                    gfx.DrawString(Math.Round(GrandCommission, 2).ToString(), font, new SolidBrush(Color.Black), XOffset + 450, YOffset);
                }



                // Save the document...
                string filename = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\pdf\\CommissionReport_" + reportdate.DateString + ".pdf";
                document.Save(filename);


                // ...and start a viewer.
                if (display)
                    Process.Start(filename);

            }
            catch (Exception e)
            {

                MessageBox.Show("Print Commission:" + e.Message);
            }
        }




        //prints employee commission report
        public void PrintCommission(ObservableCollection<EmployeeSalesData> reportlist, string daterange)
        {





            try
            {
                decimal receiptwidth = GlobalSettings.Instance.ReceiptWidth;
                string printername = GlobalSettings.Instance.ReceiptPrinter;
                decimal Total = 0;
                decimal Cost = 0;
                decimal Commission = 0;
                int counter = 0;

                decimal GrandTotal = 0;
                decimal GrandCost = 0;
                decimal GrandCommission = 0;


                if (printername == "none") return;


                //58mm printer = 32 chars  , 80mm printer = 48 chars
                ReceiptPrinter printer = new ReceiptPrinter(printername);

                printer.Center();
                printer.PrintLF("Employee Commission Report");
                printer.LineFeed();

                printer.Left();

                foreach (EmployeeSalesData report in reportlist)
                {

                    printer.PrintLF(report.CurrentEmployee.FirstName + " " + report.CurrentEmployee.LastName);
                    printer.PrintLF(daterange);
                    printer.LineFeed();

                    printer.PrintLF("_____#____|__Date___|__Total__|___Cost__|__Comm %__");


                    foreach (SalesData dat in report.EmployeeSales)
                    {

                        printer.PrintLF(dat.SalesID.ToString().PadLeft(10, ' ') + dat.CloseDate.ToShortDateString().PadLeft(10, ' ') + dat.TotalSales.ToString().PadLeft(10, ' ') + dat.TotalCost.ToString().PadLeft(10, ' ') + dat.TotalCommission.ToString().PadLeft(8, ' '));
                
                        Total = Total + dat.TotalSales;
                        Cost = Cost + dat.TotalCost;
                        Commission += dat.TotalCommission;

                    }
                    //printer.PrintLF("================================"); 32 characters 
                    printer.PrintLF("================================================"); //48 characters

                    printer.PrintLF(Total.ToString().PadLeft(30, ' ') + Cost.ToString().PadLeft(10, ' ') + Math.Round(Commission, 2).ToString().PadLeft(8, ' '));
                    counter++;
                    GrandTotal = GrandTotal + Total;
                    GrandCost = GrandCost + Cost;
                    GrandCommission += Commission;
                    Total = 0;
                    Cost = 0;
                    Commission = 0;
                }

                printer.LineFeed();

                if (counter > 1)
                {
                    printer.PrintLF("================================================"); //48 characters
                    printer.PrintLF("Grand Total: " + GrandTotal.ToString().PadLeft(17, ' ') + GrandCost.ToString().PadLeft(10, ' ') + Math.Round(GrandCommission, 2).ToString().PadLeft(8, ' '));
                }


                printer.Send(); //sends buffer to printer

                printer.Cut();


            }
            catch (Exception e)
            {

                MessageBox.Show("Print Commission:" + e.Message);
            }
        }



        public void PrintSalesSummary(DateTime startdate, DateTime enddate, ObservableCollection<DailyRevenue> revenuereport, ObservableCollection<DailySettlement> settlementreport, ObservableCollection<ReportCat> revenuelist, ObservableCollection<ReportCat> settlementlist, bool PrintPreview)
        {

            PrintDocument pdoc = null;
            System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();
            pdoc = new PrintDocument();

            pd.UseEXDialog = true;


            pd.Document = pdoc;


            pdoc.PrintPage += (sender, e) => pdoc_PrintSalesSummary(startdate,enddate,revenuereport,settlementreport,revenuelist,settlementlist, e);  //this method allows you to pass parameters


            if (GlobalSettings.Instance.LargeFormatPrinter == "")
            {
                MessageBox.Show("Large format printer name not set.");
                return;
            }
            pdoc.PrinterSettings.PrinterName = GlobalSettings.Instance.LargeFormatPrinter;
            pdoc.DefaultPageSettings.Landscape = true;

           // pd.ShowDialog();

            if(PrintPreview)
            {

   
                        System.Windows.Forms.DialogResult result;
                        System.Windows.Forms.PrintPreviewDialog pp = new System.Windows.Forms.PrintPreviewDialog();
                        pp.PrintPreviewControl.Zoom = 1;
                        pp.Document = pdoc;
                        pp.WindowState = System.Windows.Forms.FormWindowState.Maximized;

                        result = pp.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.OK)
                        {
                            pdoc.Print();
                        }

            }
            else
            {
                pdoc.Print();
            }
            

        }

        public void pdoc_PrintSalesSummary(DateTime startdate, DateTime enddate, ObservableCollection<DailyRevenue> revenuereport, ObservableCollection<DailySettlement> settlementreport, ObservableCollection<ReportCat> revenuelist, ObservableCollection<ReportCat> settlementlist, PrintPageEventArgs e)
        {
            try
            {
                Graphics graphics = e.Graphics;
                Font font = new Font("Courier New", 8, System.Drawing.FontStyle.Bold);
                Font fontitalic = new Font("Courier New", 8, System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold);
                Font fontbold = new Font("Courier New", 10, System.Drawing.FontStyle.Bold);
                // Create pen.
                Pen blackPen = new Pen(Color.Black, 2);
                Brush blackBrush = new SolidBrush(Color.Black);
                Location store = GlobalSettings.Instance.Shop;

                int fontHeight = (int)font.GetHeight();
                int fontBoldHeight = (int)fontbold.GetHeight();

                int startX = 50;
                int startY = 50;
                int XOffset = 0;
                int YOffset = 0;
                int paymentoffset = 0;

                graphics.DrawString("Week to Date Sales Report " + startdate.ToShortDateString() + "-" + enddate.ToShortDateString(), fontbold, blackBrush, startX, startY + YOffset);





                YOffset = 50;
                //left first title column
                graphics.DrawString("Revenue", fontbold, blackBrush, startX + XOffset, startY + YOffset);

                YOffset = YOffset + fontHeight * 4;
                graphics.DrawLine(blackPen, startX, startY + YOffset, startX + 1000, startY + YOffset);

                foreach(ReportCat cat in revenuelist)
                {
                    YOffset = YOffset + fontHeight * 2;
                    graphics.DrawString(cat.CatName, fontbold, blackBrush, startX + XOffset, startY + YOffset);

                    
                }

                YOffset = YOffset + fontHeight * 2;
                graphics.DrawString("Shop Fee", fontbold, blackBrush, startX + XOffset, startY + YOffset);

                YOffset = YOffset + fontHeight * 2;
                graphics.DrawString("Discount", fontbold, blackBrush, startX + XOffset, startY + YOffset);

                YOffset = YOffset + fontHeight * 2;
                graphics.DrawString("SubTotal", fontbold, blackBrush, startX + XOffset, startY + YOffset);

                YOffset = YOffset + fontHeight * 2;
                graphics.DrawString("Sales Tax", fontbold, blackBrush, startX + XOffset, startY + YOffset);

                YOffset = YOffset + fontHeight * 2;
                graphics.DrawLine(blackPen, startX, startY + YOffset, startX + 1000, startY + YOffset);

                YOffset = YOffset + fontHeight * 2;
                graphics.DrawString("Total", fontbold, blackBrush, startX + XOffset, startY + YOffset);



                //start datacolumn
                XOffset = 50;
                foreach(DailyRevenue rep in revenuereport)
                {
                    YOffset = 50;
                    XOffset = XOffset + 100;

                    graphics.DrawString(rep.DOW, fontbold, blackBrush, startX + XOffset + 50, startY + YOffset);


                    YOffset = YOffset + fontHeight * 2;
                    graphics.DrawString(rep.ReportDate, fontbold, blackBrush, startX + XOffset + 50, startY + YOffset);

                    YOffset = YOffset + fontHeight * 2;


                    foreach(ReportCat cat in rep.SalesCat)
                    {
                        YOffset = YOffset + fontHeight * 2;
                       // graphics.DrawString(cat.CatValue.ToString(), fontbold, blackBrush, startX + XOffset, startY + YOffset);
                        PrintRightAlign(graphics, cat.CatValue.ToString(), fontbold, startX + XOffset + 100, startY + YOffset);
                        
                    }

                    YOffset = YOffset + fontHeight * 2;
                   // graphics.DrawString(rep.ShopFee.ToString(), fontbold, blackBrush, startX + XOffset, startY + YOffset);
                    PrintRightAlign(graphics, rep.ShopFee.ToString(), fontbold, startX + XOffset + 100, startY + YOffset);

                    YOffset = YOffset + fontHeight * 2;
                    //graphics.DrawString(rep.Discount.ToString(), fontbold, blackBrush, startX + XOffset, startY + YOffset);
                    PrintRightAlign(graphics, rep.Discount.ToString(), fontbold, startX + XOffset + 100, startY + YOffset);

                    YOffset = YOffset + fontHeight * 2;
                    //graphics.DrawString(rep.SubTotal.ToString(), fontbold, blackBrush, startX + XOffset, startY + YOffset);
                    PrintRightAlign(graphics, rep.SubTotal.ToString(), fontbold, startX + XOffset + 100, startY + YOffset);

                    YOffset = YOffset + fontHeight * 2;
                   // graphics.DrawString(rep.SalesTax.ToString(), fontbold, blackBrush, startX + XOffset, startY + YOffset);
                    PrintRightAlign(graphics, rep.SalesTax.ToString(), fontbold, startX + XOffset + 100, startY + YOffset);

                    YOffset = YOffset + fontHeight * 4;
                    //graphics.DrawString(rep.TotalRevenue.ToString(), fontbold, blackBrush, startX + XOffset, startY + YOffset);
                    PrintRightAlign(graphics, rep.TotalRevenue.ToString(), fontbold, startX + XOffset + 100, startY + YOffset);
                }

                YOffset = YOffset + fontHeight * 4;

                //Settlement report section

                paymentoffset = YOffset;
                XOffset = 0;

                //left first title column for Settlement
                graphics.DrawString("Settlement", fontbold, blackBrush, startX + XOffset, startY + YOffset);

                YOffset = YOffset + fontHeight * 4;
                graphics.DrawLine(blackPen, startX, startY + YOffset, startX + 1000, startY + YOffset);

                foreach (ReportCat cat in settlementlist)
                {
                    YOffset = YOffset + fontHeight * 2;
                    graphics.DrawString(cat.CatName, fontbold, blackBrush, startX + XOffset, startY + YOffset);


                }

                YOffset = YOffset + fontHeight * 2;
                graphics.DrawLine(blackPen, startX, startY + YOffset, startX + 1000, startY + YOffset);

                YOffset = YOffset + fontHeight * 2;
                graphics.DrawString("Total", fontbold, blackBrush, startX + XOffset, startY + YOffset);



                //start datacolumn
                XOffset = 50;
                foreach (DailySettlement rep in settlementreport)
                {
                    YOffset = paymentoffset;
                    XOffset = XOffset + 100;

                    graphics.DrawString(rep.DOW, fontbold, blackBrush, startX + XOffset + 50, startY + YOffset);


                    YOffset = YOffset + fontHeight * 2;
                    graphics.DrawString(rep.ReportDate, fontbold, blackBrush, startX + XOffset + 50, startY + YOffset);

                    YOffset = YOffset + fontHeight * 2;


                    foreach (ReportCat cat in rep.PaymentCat)
                    {
                        YOffset = YOffset + fontHeight * 2;
                        //graphics.DrawString(cat.CatValue.ToString(), fontbold, blackBrush, startX + XOffset, startY + YOffset);
                        PrintRightAlign(graphics, cat.CatValue.ToString(), fontbold, startX + XOffset + 100, startY + YOffset);
                    }


                    YOffset = YOffset + fontHeight * 4;
                    //graphics.DrawString(rep.TotalPayment.ToString(), fontbold, blackBrush, startX + XOffset, startY + YOffset);
                    PrintRightAlign(graphics, rep.TotalPayment.ToString(), fontbold, startX + XOffset + 100, startY + YOffset);
                }


















            }catch(Exception ex)
            {
                MessageBox.Show("Print Summary:" + ex.Message);
            }
        }

        public void PrintCommissionLarge(ObservableCollection<EmployeeSalesData> reportlist, string daterange)
        {
            PrintDocument pdoc = null;
            System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();
            pdoc = new PrintDocument();

            pd.UseEXDialog = true;


            pd.Document = pdoc;


            pdoc.PrintPage += (sender,e) => pdoc_PrintCommissionLarge( reportlist,daterange, e);  //this method allows you to pass parameters

  
            if (GlobalSettings.Instance.LargeFormatPrinter == "" || GlobalSettings.Instance.LargeFormatPrinter == "none")
            {
                MessageBox.Show("Large format printer name not set.");
                return;
            }
            pdoc.PrinterSettings.PrinterName = GlobalSettings.Instance.LargeFormatPrinter;
         

    

            pdoc.Print();


        }
        public void pdoc_PrintCommissionLarge(ObservableCollection<EmployeeSalesData> reportlist, string daterange, PrintPageEventArgs e)
        {
            try
            {


                Graphics graphics = e.Graphics;
                Font font = new Font("Courier New", 8, System.Drawing.FontStyle.Bold);
                Font fontitalic = new Font("Courier New", 8, System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold);
                Font fontbold = new Font("Courier New", 10, System.Drawing.FontStyle.Bold);
                // Create pen.
                Pen blackPen = new Pen(Color.Black, 2);
                Brush blackBrush = new SolidBrush(Color.Black);
                Location store = GlobalSettings.Instance.Shop;

                int fontHeight = (int)font.GetHeight();
                int fontBoldHeight = (int)fontbold.GetHeight();

                int startX = 20;
                int startY = 50;
           
                int YOffset = 0;
                int rightLimit = 750;
                string receiptstr;



                decimal Total = 0;
                decimal Cost = 0;
                decimal Commission=0;
                decimal Adjustments = 0;
                decimal CommissionAdj = 0;

                int counter = 0;

                decimal GrandTotal = 0;
                decimal GrandCost = 0;
                decimal GrandCommission = 0;
                decimal GrandAdjustments = 0;
                decimal GrandCommissionAdj = 0;

                string description;
    
 
 
                    graphics.DrawString("Employee Commission Report", fontbold, blackBrush, startX, startY + YOffset);

                   YOffset = YOffset + fontHeight * 3;

                foreach (EmployeeSalesData report in reportlist)
                {

  
                    receiptstr=report.CurrentEmployee.FirstName + " " + report.CurrentEmployee.LastName;
                     graphics.DrawString(receiptstr, fontbold, blackBrush, startX, startY + YOffset);


                    YOffset = YOffset + fontHeight * 3;
                    graphics.DrawString(daterange, fontbold, blackBrush, startX, startY + YOffset);


                 //Title for ticket items
                YOffset = YOffset + fontHeight*2;
                graphics.DrawString("Ticket #", fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);
                graphics.DrawString("Date", fontbold, new SolidBrush(Color.Black), startX + 80, startY + YOffset);
                graphics.DrawString("Qty", fontbold, new SolidBrush(Color.Black), startX + 150, startY + YOffset);
                graphics.DrawString("Description", fontbold, new SolidBrush(Color.Black), startX + 200, startY + YOffset);


                PrintRightAlign(graphics, "Sold At", fontbold, rightLimit - 240, startY + YOffset);
                PrintRightAlign(graphics, "Cost", fontbold, rightLimit - 160, startY + YOffset);
                PrintRightAlign(graphics, "Margin", fontbold, rightLimit - 80, startY + YOffset);
                PrintRightAlign(graphics,  "Comm %", fontbold, rightLimit, startY + YOffset);


                    // line beneath title line
                YOffset = YOffset + fontBoldHeight;
                graphics.DrawLine(blackPen, startX, startY + YOffset, startX + 700, startY + YOffset);



                    foreach (SalesData dat in report.EmployeeSales)
                    {
                        YOffset = YOffset + fontBoldHeight;
                        receiptstr = dat.SalesID.ToString().PadLeft(7, ' ') + dat.CloseDate.ToShortDateString().PadLeft(10, ' ');
                        graphics.DrawString(receiptstr, fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);

                        //-- print the line items
                        foreach(RetailLineItem line in dat.SalesItem)
                        {

                            YOffset = YOffset + fontBoldHeight;

                            description = line.ModelNumber + line.Description;
                            if (description.Length > 20) description = description.Substring(0, 20);
                            receiptstr = line.Quantity.ToString().PadLeft(5, ' ') + description + line.AdjustedPrice.ToString().PadLeft(10, ' ') + line.Cost.ToString().PadLeft(10, ' ');
                            graphics.DrawString(receiptstr, fontbold, new SolidBrush(Color.Black), startX + 120, startY + YOffset);

                        }

                        YOffset = YOffset + fontBoldHeight;
                        graphics.DrawLine(blackPen, startX+300, startY + YOffset, startX + 700, startY + YOffset);
                        receiptstr = dat.TotalSales.ToString().PadLeft(10, ' ') + dat.TotalCost.ToString().PadLeft(10, ' ') + dat.TotalCommission.ToString().PadLeft(8, ' ');
                        graphics.DrawString(receiptstr, fontbold, new SolidBrush(Color.Black), startX + 300, startY + YOffset);

                        Total = Total + dat.TotalSales;
                        Cost = Cost + dat.TotalCost;
                        Commission += dat.TotalCommission;
                        Adjustments += dat.TotalAdjustments;
                        CommissionAdj += dat.TotalCommissionAdjustment;

                    }


                    // line beneath title line
                    YOffset = YOffset + fontBoldHeight;
                    graphics.DrawLine(blackPen, startX, startY + YOffset, startX + 700, startY + YOffset);


                    YOffset = YOffset + fontBoldHeight;
                    receiptstr = Total.ToString().PadLeft(30, ' ') + Cost.ToString().PadLeft(10, ' ') + Math.Round(Commission, 2).ToString().PadLeft(8, ' ');
                     graphics.DrawString(receiptstr, fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);

                    
                    counter++;
                    GrandTotal = GrandTotal + Total;
                    GrandCost = GrandCost + Cost;
                    GrandCommission += Commission;
                    GrandAdjustments += Adjustments;
                    GrandCommissionAdj += CommissionAdj;


                    Total = 0;
                    Cost = 0;
                    Commission = 0;
                }

                YOffset = YOffset + fontBoldHeight;

                if (counter > 1)
                {
                    // line beneath title line
                    YOffset = YOffset + fontBoldHeight;
                    graphics.DrawLine(blackPen, startX, startY + YOffset, startX + 700, startY + YOffset);

                    receiptstr = "Grand Total: " + GrandTotal.ToString().PadLeft(17, ' ') + GrandCost.ToString().PadLeft(10, ' ') + Math.Round(GrandCommission, 2).ToString().PadLeft(8, ' ');
                }





            }
            catch (Exception ex)
            {

                MessageBox.Show("Print Commission:" + ex.Message);
            }
        }




        private void PrintRightAlign(Graphics graphics, string receiptline, Font font, int rightlimit, int Y)
        {
            graphics.DrawString(receiptline, font, new SolidBrush(Color.Black), rightlimit - graphics.MeasureString(receiptline, font).Width, Y);
        }


 
        private static void PrintRightAlign(XGraphics graphics, string receiptline, XFont font, int rightlimit, int Y)
        {
            graphics.DrawString(receiptline, font, new SolidBrush(Color.Black), rightlimit - graphics.MeasureString(receiptline, font).Width, Y);
        }

    }


    public class DailyRevenue
    {
        public string DOW { get; set; }
        public string ReportDate { get; set; }
        public ObservableCollection<ReportCat> SalesCat { get; set; }

        public decimal SalesTax { get; set; }

        public decimal ItemTotal { get; set; }

        public decimal SubTotal { get { return ItemTotal + ShopFee - Discount; } }
        public decimal ShopFee { get; set; }
        public decimal Discount { get; set; }

        public decimal TotalRevenue { get { return SubTotal + SalesTax ; } }

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
        public decimal NetDifference { get { return NetPayment - NetRevenue; } }
    }
    public class ReportCat
    {
        public string CatName { get; set; }
        public decimal CatValue { get; set; }
    }
}
