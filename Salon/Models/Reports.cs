using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Collections.ObjectModel;
using System.Data;
//using System.Windows.Forms;
using RedDot.DataManager;
using Microsoft.Win32;

namespace RedDot
{
    public class Reports
    {
        DBReports _dbreports;
       
        public Reports()
        {
            _dbreports = new DBReports();
         
        }



         public DataTable GetWorkedEmployees( DateTime startdate, DateTime enddate)
        {

           return  _dbreports.GetWorkedEmployees( startdate, enddate);
        }

        public DataTable GetSalesSummary(DateTime reportdate)
         {
              return _dbreports.GetSalesSummary(reportdate,reportdate);
         }

        public DataTable GetSalesDetail(DateTime reportdate)
        {
            return _dbreports.GetSalesTickets(reportdate, reportdate);
        }


        public DataTable GetDailySales(DateTime reportdate, int employeeid)
        {
            return _dbreports.GetDailySales(reportdate, reportdate, employeeid);
        }

        public DataTable GetDailySalesSummary(DateTime reportdate)
        {
            return _dbreports.GetDailySalesSummary(reportdate, reportdate);
        }

        public DataTable GetDailyPayments(DateTime reportdate, int employeeid)
        {
            return _dbreports.GetDailyPayments(reportdate, reportdate, employeeid);
        }

        public DataTable GetDailyPaymentSummary(DateTime reportdate, int employeeid)
        {
            return _dbreports.GetDailyPaymentSummary(reportdate, reportdate, employeeid);
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
                dt = _dbreports.GetSalesRevenue(startdate, enddate);

                totalrevenue = 0;
     
                if(dt != null)
                foreach (DataRow row in dt.Rows) //parse through each category of items sold
                {
                    reportcat = new ReportCat();
                    reportcat.CatName = row["reportcategory"].ToString();
                    if (row["revenue"].ToString() != "")
                    {
                        reportcat.CatValue = decimal.Parse(row["revenue"].ToString());
                       
                    }
                    else reportcat.CatValue = 0;

                    totalrevenue = totalrevenue + reportcat.CatValue;
                    dailyreport.SalesCat.Add(reportcat);
                }


            dailyreport.SalesTax = _dbreports.GetDailySalesTax(startdate, enddate);  //added 06/15/18
            dailyreport.TipsWitheld = _dbreports.GetDailyTips(startdate, enddate);
            dailyreport.TotalRevenue = totalrevenue;
            dailyreport.TotalAdjustment =  _dbreports.GetDailySalesAdjustments(startdate, enddate);  //this discount is for COMPs / ticket discount , not item discount.  Adjustment could be negative or positive
            dailyreport.TotalDiscount = (-1) * _dbreports.GetDailySalesDiscounts(startdate, enddate);
               
    

                return dailyreport;
        }
        public DailySettlement GetDailySettlement(DateTime startdate, DateTime enddate)
        {
          
            DailySettlement dailyreport;
            DataTable dt;
            ReportCat reportcat;
            decimal totalpayment;
            decimal tipswithheld;



                dailyreport = new DailySettlement();
                dailyreport.PaymentCat = new ObservableCollection<ReportCat>();

                dailyreport.DOW = startdate.DayOfWeek.ToString().Substring(0, 3).ToUpper();
                dailyreport.ReportDate = startdate.ToString("M/d");

                //get the daily service/product sold
                dt = _dbreports.GetSalesSettlement(startdate, enddate);
                tipswithheld = _dbreports.GetDailyTips(startdate, enddate);
                totalpayment = 0;
                foreach (DataRow row in dt.Rows) //parse through each category of items sold
                {
                    reportcat = new ReportCat();
                    reportcat.CatName = row["reportcategory"].ToString();
                    if (row["netamount"].ToString() != "") reportcat.CatValue = decimal.Parse(row["netamount"].ToString());
                    else reportcat.CatValue = 0;
                    //add tips to credit charge
                 
                    totalpayment = totalpayment + reportcat.CatValue;
                    dailyreport.PaymentCat.Add(reportcat);
                }

              

                dailyreport.TotalPayment = totalpayment;



            return  dailyreport;
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



        public ObservableCollection<DailyRevenue> GetSemiMonthlyRevenue(DateTime startdate)
        {
            ObservableCollection<DailyRevenue> weeklyreport;
            weeklyreport = new ObservableCollection<DailyRevenue>();
            DailyRevenue dailyreport;
            DateTime reportdate;
            reportdate = startdate;

            int numOfDays;

            if (startdate.Day ==1 )
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
            dailyreport = GetDailyRevenue(startdate, startdate.AddDays(numOfDays - 1));
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






        public ObservableCollection<DailyRevenue> GetCustomRevenue(DateTime startdate,DateTime enddate)
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

















        //Gets All tickets for ONE employee
        public ObservableCollection<SalesData> GetEmployeeCommission(Employee CurrentEmployee, DateTime startdate, DateTime enddate)
        {

            ObservableCollection<SalonLineItem> itemdata;
            SalonLineItem lineitem;
            int lastid = 0;
            int lastemployeeid=0;
            int currentid = 0;
            decimal totalsales = 0;
            decimal totalcommission = 0;
            decimal totalsupplyfee = 0;


            bool DeductDiscountFromCommission = GlobalSettings.Instance.DeductDiscountFromCommission;

            SalesData salesrecord;
            ObservableCollection<SalesData> employeesales;



            //Actual list of tickets in above object
            employeesales = new ObservableCollection<SalesData>();
            itemdata = new ObservableCollection<SalonLineItem>();
            salesrecord = new SalesData(GlobalSettings.Instance.CreditCardFeePercent);
            int i = 0;


            DataTable dt = _dbreports.GetSalonSalesCommissionByID(CurrentEmployee.ID, startdate, enddate);
            foreach (DataRow row in dt.Rows)
            {
                currentid = (int)row["SalesID"];
                if (currentid != lastid)
                {
                    //add record from previous line into collection
                    if (i > 0)
                    {

                        salesrecord.TotalSales = totalsales;  //total sales for each ticket
                        salesrecord.TotalCommission = totalcommission;
                        salesrecord.TotalSupplyFee = totalsupplyfee;
                        salesrecord.SalesItem = itemdata;
                        if (CurrentEmployee.ID == 999) salesrecord.Gratuity = _dbreports.GetGratuitybySalesID(lastid);
                        else salesrecord.Gratuity = _dbreports.GetEmployeeGratuitybySalesID(lastemployeeid, lastid);
                        employeesales.Add(salesrecord);  //Add ticket to employee list
                    }


                    i++;
                    //create new order record
                    totalsales = 0;
                    totalcommission = 0;
                    totalsupplyfee = 0;
                    salesrecord = new SalesData(GlobalSettings.Instance.CreditCardFeePercent);
                    salesrecord.SaleDate =(DateTime) row["saledate"];
                
                   salesrecord.PaymentType = row["paymenttype"].ToString();  //this is optional for commission report .. it is to display on the ticket details in the commission report
                    salesrecord.SalesID = (int)row["SalesID"];

                    itemdata = new ObservableCollection<SalonLineItem>();
                   
                  
                    lineitem = new SalonLineItem(row, CurrentEmployee.CommissionPercent,DeductDiscountFromCommission);
              
                    





                    itemdata.Add(lineitem);

                  
                    totalsales = totalsales + lineitem.CommissionPrice;  //CommissionPrice = Price + Surcharge 

                    totalcommission = totalcommission + lineitem.Commission;

                    totalsupplyfee = totalsupplyfee + lineitem.SupplyFee;
                  

                }
                else
                {

                 
                    lineitem = new SalonLineItem(row, CurrentEmployee.CommissionPercent,DeductDiscountFromCommission);
                

                    itemdata.Add(lineitem);
                    totalsales = totalsales + lineitem.CommissionPrice;  //CommissionPrice = Price + Surcharge 
                    totalcommission = totalcommission + lineitem.Commission;
                    totalsupplyfee = totalsupplyfee + lineitem.SupplyFee;
                }


                lastid = currentid;
                lastemployeeid = (int)row["employeeid"];
            }

            //need to add last record

            salesrecord.TotalSales = totalsales;  //total sales for each ticket
            salesrecord.TotalCommission = totalcommission;
            salesrecord.TotalSupplyFee = totalsupplyfee;
            salesrecord.SalesItem = itemdata;


            if (CurrentEmployee.ID == 999) salesrecord.Gratuity = _dbreports.GetGratuitybySalesID(lastid);
            else salesrecord.Gratuity = _dbreports.GetEmployeeGratuitybySalesID(lastemployeeid, lastid);

            employeesales.Add(salesrecord);  //Add ticket to employee list


            return employeesales;
        }



        public DataTable GetRewardSummary(DateTime startdate, DateTime enddate)
        {

           return  _dbreports.GetRewardSummary(startdate,enddate);
        }



//-----------------------------------------------------------------------------------EXPORT Commission ------------------------------------------------------------------------------------
        public void ExportCommissionCSV(int employeeid, DateTime startdate, DateTime enddate,string type)
        {

            try
            {
                DataTable dt = _dbreports.GetSalonSalesCommissionExportByID(employeeid, startdate, enddate, type);
                SaveFileDialog ofd = new SaveFileDialog();
                ofd.DefaultExt = "csv";
                ofd.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                Nullable<bool> result = ofd.ShowDialog();

                if (result == true )
                {
                    CSVWriter.WriteDataTable(dt, ofd.FileName, true);


                }
            }
            catch(Exception ex)
            {
                TouchMessageBox.Show(ex.Message);
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
                Nullable<bool> result = ofd.ShowDialog();

                if (result == true)
                {
                        CSVWriter.WriteDataTable(dt, ofd.FileName, true);

                    }
                }catch(Exception ex)
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
                Nullable<bool> result = ofd.ShowDialog();

                if (result == true)
                {
                        CSVWriter.WriteDataTable(dt, ofd.FileName, true);

                    }

                }
                catch (Exception ex)
                {
                    TouchMessageBox.Show(ex.Message);
                }
            }





        /*
        //Export employee commission report
        public void ExportCommission(Employee employee, ObservableCollection<SalesData> list, string daterange)
        {

            Microsoft.Office.Interop.Excel.Application excel = null;
            Microsoft.Office.Interop.Excel.Workbook wb = null;

            object missing = Type.Missing;
            Microsoft.Office.Interop.Excel.Worksheet ws = null;
           // Microsoft.Office.Interop.Excel.Range rng = null;    


            try
            {

                decimal Total = 0;
                decimal Gratuity = 0;
                int Idx = 0;

                excel = new Microsoft.Office.Interop.Excel.Application();
                wb = excel.Workbooks.Add();
                ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.ActiveSheet;


                ws.Range["A1"].Offset[0, 0].Value = "Sales Date";
                ws.Range["A1"].Offset[0, 1].Value = "Sales Date";


 
                foreach (SalesData dat in list)
                {
                    ws.Range["A2"].Offset[Idx, 0].Value = dat.SalesID.ToString();
                    ws.Range["A2"].Offset[Idx, 1].Value = dat.saledate.ToShortDateString();
                    ws.Range["A2"].Offset[Idx, 2].Value = dat.TotalSales.ToString();
                    ws.Range["A2"].Offset[Idx, 3].Value = dat.TotalSales.ToString();
                    ws.Range["A2"].Offset[Idx, 4].Value = dat.TotalCommission.ToString();
                    ws.Range["A2"].Offset[Idx,5].Value =  dat.Gratuity.ToString();

                  
                    
                    Gratuity = Gratuity + dat.Gratuity;
                    Total = Total + dat.TotalSales;
                    Idx++;

                }

                excel.Visible = true;
                wb.Activate();

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Export Commission:" + e.Message);
            }
        }

        */
        //prints employee commission report
        public void PrintCommission(ObservableCollection<EmployeeCommissionVM> reportlist, string daterange, bool cut, string reporttype)
        {
            try
            {
                if(reportlist == null)
                {
                    TouchMessageBox.Show("Report is empty.");
                    return;
                }

                int receiptwidth = GlobalSettings.Instance.ReceiptWidth;

                //defaults to receipt width since some customer had the character width setup.
                int receiptchars = receiptwidth;
                //translate to chars if value is in millimeters
                //58mm printer = 32 chars  , 80mm printer = 48 chars
                if (receiptwidth == 58) receiptchars = 32;
                if (receiptwidth == 80) receiptchars = 48;



                string printername = GlobalSettings.Instance.ReceiptPrinter;
                Location store = GlobalSettings.Instance.Shop;
            
          
               // decimal commissionpercent = GlobalSettings.Instance.CommissionPercent;
                decimal creditfee = GlobalSettings.Instance.CreditCardFeePercent;
                decimal Total = 0;
                decimal Commission = 0;
                decimal Gratuity = 0;
                decimal NetGratuity = 0;
                decimal SupplyFee = 0;
                int counter = 0;

                decimal GrandTotal = 0;
                decimal GrandGratuity = 0;
                decimal GrandNetGratuity = 0;
                decimal GrandSupplyFee = 0;
                decimal GrandCommission = 0;

                decimal Custom1 = 0;
                decimal Custom2 = 0;


                if (printername == "none") return;


                //58mm printer = 32 chars  , 80mm printer = 48 chars
                ReceiptPrinter printer = new ReceiptPrinter(printername);

                if(!cut)
                {
                    printer.Center();
                    printer.LineFeed();
                    printer.PrintLF(store.Name);

                    printer.PrintLF(store.Address1);
                    if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                    printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                    printer.PrintLF(store.Phone);
                    printer.LineFeed();
                    printer.PrintLF(new String('=', receiptchars));

                    printer.PrintLF("Employee Commission Report");
                    printer.LineFeed();
                    printer.Left();
                }
                







                foreach(EmployeeCommissionVM report in reportlist)
                {
                    if (cut)
                    {
                        printer.Center();
                        printer.LineFeed();
                        printer.PrintLF(store.Name);

                        printer.PrintLF(store.Address1);
                        if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                        printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                        printer.PrintLF(store.Phone);
                        printer.LineFeed();
                        printer.PrintLF(new String('=', receiptchars));

                        printer.PrintLF("Employee Commission Report");
                        printer.LineFeed();
                        printer.Left();
                    }


                    printer.DoubleHeight();

                    printer.PrintLF(report.CurrentEmployee.FirstName + " " + report.CurrentEmployee.LastName);
                    printer.DoubleHeightOFF();

                    printer.PrintLF(daterange);
                    printer.LineFeed();
                    printer.PrintLF("Service Count:" + report.GrandServiceCount);
                    printer.PrintLF("Ticket Count:" + report.GrandTicketCount);

                    bool PrintComm = GlobalSettings.Instance.ShowCommissionOnReport;
                    switch (reporttype)
                    {

                        case "extradetail":
                     
                            if (creditfee > 0) printer.PrintLF("___#___|__Date___|Total_|Supp| " + (PrintComm ? report.CurrentEmployee.CommissionPercent.ToString() : "XX") + "% |_Tip_|NetTip");
                            else printer.PrintLF("___#___|__Date___|Total_|Supp| " + (PrintComm ? report.CurrentEmployee.CommissionPercent.ToString() : "XX") + "% |_Tip_");
                    
                    

                            string desc = "";
                            foreach (SalesData dat in report.EmployeeSales)
                            {
                                foreach (SalonLineItem line in dat.SalesItem)
                                {
                                    if (line.Description.Length > 15)
                                        desc = line.Description.Substring(0, 15) + " ";
                                    else
                                        desc = line.Description.PadRight(16, ' ');

                                    printer.PrintLF(desc + String.Format("{0:0.00}",line.CommissionPrice).PadLeft(7,' ') + String.Format("{0:0.00}", line.SupplyFee).PadLeft(6, ' ') + String.Format("{0:0.00}", line.Commission).PadLeft(6, ' '));
                                }
                                printer.PrintLF("-------------------------------------------"); //48 characters
                                if (creditfee > 0) printer.PrintLF(dat.SalesID.ToString().PadLeft(7, ' ') + String.Format("{0:MM/dd/yy}", dat.SaleDate).PadLeft(9, ' ') + dat.TotalSales.ToString().PadLeft(7, ' ') + dat.TotalSupplyFee.ToString().PadLeft(6, ' ') + Math.Round(dat.TotalCommission, 2).ToString().PadLeft(7, ' ') + dat.Gratuity.ToString().PadLeft(6, ' ') + dat.NetGratuity.ToString().PadLeft(6, ' '));
                                else printer.PrintLF(dat.SalesID.ToString().PadLeft(7, ' ') + String.Format("{0:MM/dd/yy}", dat.SaleDate).PadLeft(9, ' ') + dat.TotalSales.ToString().PadLeft(7, ' ') + dat.TotalSupplyFee.ToString().PadLeft(6, ' ') + Math.Round(dat.TotalCommission, 2).ToString().PadLeft(7, ' ') + dat.Gratuity.ToString().PadLeft(6, ' '));
                                printer.LineFeed();
                         
                                Gratuity = Gratuity + dat.Gratuity;
                                NetGratuity = NetGratuity + dat.NetGratuity;
                                Total = Total + dat.TotalSales;
                                SupplyFee = SupplyFee + dat.TotalSupplyFee;
                                Commission = Commission + dat.TotalCommission;

                            }
                            //printer.PrintLF("================================"); 32 characters 
                            printer.PrintLF("================================================"); //48 characters
                            if (creditfee > 0) printer.PrintLF(Total.ToString().PadLeft(23, ' ') + SupplyFee.ToString().PadLeft(6, ' ') + Math.Round(Commission, 2).ToString().PadLeft(7, ' ') + Gratuity.ToString().PadLeft(6, ' ') + NetGratuity.ToString().PadLeft(6, ' '));
                            else printer.PrintLF(Total.ToString().PadLeft(23, ' ') + SupplyFee.ToString().PadLeft(6, ' ') + Math.Round(Commission, 2).ToString().PadLeft(7, ' ') + Gratuity.ToString().PadLeft(6, ' '));

                            break;


                        case "detail":

                            if (creditfee > 0) printer.PrintLF("___#___|__Date___|Total_|Supp| " + (PrintComm ? report.CurrentEmployee.CommissionPercent.ToString() : "XX") + "% |_Tip_|NetTip");
                            else printer.PrintLF("___#___|__Date___|Total_|Supp| " + (PrintComm ? report.CurrentEmployee.CommissionPercent.ToString() : "XX") + "% |_Tip_");


                            foreach (SalesData dat in report.EmployeeSales)
                            {
                                if (creditfee > 0) printer.PrintLF(dat.SalesID.ToString().PadLeft(7, ' ') + String.Format("{0:MM/dd/yy}", dat.SaleDate).PadLeft(9, ' ') + dat.TotalSales.ToString().PadLeft(7, ' ') + dat.TotalSupplyFee.ToString().PadLeft(6, ' ') + Math.Round(dat.TotalCommission, 2).ToString().PadLeft(7, ' ') + dat.Gratuity.ToString().PadLeft(6, ' ') + dat.NetGratuity.ToString().PadLeft(6, ' '));
                                else printer.PrintLF(dat.SalesID.ToString().PadLeft(7, ' ') + String.Format("{0:MM/dd/yy}", dat.SaleDate).PadLeft(9, ' ') + dat.TotalSales.ToString().PadLeft(7, ' ') + dat.TotalSupplyFee.ToString().PadLeft(6, ' ') + Math.Round(dat.TotalCommission, 2).ToString().PadLeft(7, ' ') + dat.Gratuity.ToString().PadLeft(6, ' '));

                                Gratuity = Gratuity + dat.Gratuity;
                                NetGratuity = NetGratuity + dat.NetGratuity;
                                Total = Total + dat.TotalSales;
                                SupplyFee = SupplyFee + dat.TotalSupplyFee;
                                Commission = Commission + dat.TotalCommission;

                            }
                            //printer.PrintLF("================================"); 32 characters 
                            printer.PrintLF("================================================"); //48 characters
                            if (creditfee > 0) printer.PrintLF(Total.ToString().PadLeft(23, ' ') + SupplyFee.ToString().PadLeft(6, ' ') + Math.Round(Commission, 2).ToString().PadLeft(7, ' ') + Gratuity.ToString().PadLeft(6, ' ') + NetGratuity.ToString().PadLeft(6, ' '));
                            else printer.PrintLF(Total.ToString().PadLeft(23, ' ') + SupplyFee.ToString().PadLeft(6, ' ') + Math.Round(Commission, 2).ToString().PadLeft(7, ' ') + Gratuity.ToString().PadLeft(6, ' '));

                            break;


                        case "daily":
                            decimal DailyTotal = 0;
                            decimal DailyGratuity = 0;
                            decimal DailyNetGratuity = 0;
                            decimal DailySupplyFee = 0;
                            decimal DailyCommission = 0;
                            string datestring = "";
                            if (creditfee > 0) printer.PrintLF("__Date___|_Total_|Supply|  " + (PrintComm ? report.CurrentEmployee.CommissionPercent.ToString() : "XX") + "%  |__Tip_|Net_Tip");
                            else printer.PrintLF("__Date___|_Total_|Supply|  " + (PrintComm ? report.CurrentEmployee.CommissionPercent.ToString() : "XX") + "%  |__Tip_");


                            foreach (SalesData dat in report.EmployeeSales.OrderBy(x => x.SaleDate))
                            {
                                if (datestring != String.Format("{0:MM/dd/yy}", dat.SaleDate))
                                {
                                    if (datestring != "")
                                    {
                                        if (creditfee > 0) printer.PrintLF(datestring.PadLeft(10, ' ') + DailyTotal.ToString().PadLeft(8, ' ') + DailySupplyFee.ToString().PadLeft(6, ' ') + Math.Round(DailyCommission, 2).ToString().PadLeft(8, ' ') + DailyGratuity.ToString().PadLeft(7, ' ') + DailyNetGratuity.ToString().PadLeft(7, ' '));
                                        else printer.PrintLF(datestring.PadLeft(10, ' ') + DailyTotal.ToString().PadLeft(8, ' ') + DailySupplyFee.ToString().PadLeft(6, ' ') + Math.Round(DailyCommission, 2).ToString().PadLeft(8, ' ') + DailyGratuity.ToString().PadLeft(7, ' '));
                                    }

                                    DailyTotal = 0;
                                    DailyGratuity = 0;
                                    DailyNetGratuity = 0;
                                    DailySupplyFee = 0;
                                    DailyCommission = 0;
                                    datestring = String.Format("{0:MM/dd/yy}", dat.SaleDate);
                                }


                                DailyGratuity = DailyGratuity + dat.Gratuity;
                                DailyNetGratuity = DailyNetGratuity + dat.NetGratuity;
                                DailyTotal = DailyTotal + dat.TotalSales;
                                DailySupplyFee = DailySupplyFee + dat.TotalSupplyFee;
                                DailyCommission = DailyCommission + dat.TotalCommission;


                                Gratuity = Gratuity + dat.Gratuity;
                                NetGratuity = NetGratuity + dat.NetGratuity;
                                Total = Total + dat.TotalSales;
                                SupplyFee = SupplyFee + dat.TotalSupplyFee;
                                Commission = Commission + dat.TotalCommission;



                            }
                            if (creditfee > 0) printer.PrintLF(datestring.PadLeft(10, ' ') + DailyTotal.ToString().PadLeft(8, ' ') + DailySupplyFee.ToString().PadLeft(6, ' ') + Math.Round(DailyCommission, 2).ToString().PadLeft(8, ' ') + DailyGratuity.ToString().PadLeft(7, ' ') + DailyNetGratuity.ToString().PadLeft(7, ' '));
                            else printer.PrintLF(datestring.PadLeft(10, ' ') + DailyTotal.ToString().PadLeft(8, ' ') + DailySupplyFee.ToString().PadLeft(6, ' ') + Math.Round(DailyCommission, 2).ToString().PadLeft(8, ' ') + DailyGratuity.ToString().PadLeft(7, ' '));



                            break;


                        case "summary":
                            //only add totals .. no printing of details for summary printout

                            foreach (SalesData dat in report.EmployeeSales)
                            {

                                Gratuity = Gratuity + dat.Gratuity;
                                NetGratuity = NetGratuity + dat.NetGratuity;
                                Total = Total + dat.TotalSales;
                                SupplyFee = SupplyFee + dat.TotalSupplyFee;
                                Commission = Commission + dat.TotalCommission;

                            }
                            break;

                    }



                    printer.PrintLF("");
                    printer.PrintLF("============== COMMISSION SUMMARY =============="); //48 characters

                    printer.PrintLF("Sales            :" + Total.ToString().PadLeft(10, ' '));
                    printer.PrintLF("Supplies         :" + ("-" + SupplyFee.ToString()).PadLeft(10, ' '));
                    printer.PrintLF("Net Sales        :" + Math.Round(Total - SupplyFee, 2).ToString().PadLeft(10, ' '));
                    printer.PrintLF("Commission (" + (PrintComm ? report.CurrentEmployee.CommissionPercent.ToString() : "XX") + "%) :" + Math.Round(Commission, 2).ToString().PadLeft(10, ' '));
                    printer.PrintLF("Tips             :" + Math.Round(Gratuity, 2).ToString().PadLeft(10, ' '));

                    if (creditfee > 0) printer.PrintLF("Net Tips (-" + GlobalSettings.Instance.CreditCardFeePercent + "%) :" + Math.Round(NetGratuity, 2).ToString().PadLeft(10, ' '));
                    printer.PrintLF("------------------------------------------------"); //48 characters
                    printer.PrintLF("Total Pay        :" + Math.Round(Commission + NetGratuity, 2).ToString().PadLeft(10, ' '));
                    printer.PrintLF("=================== PAY CHECK =================="); //48 characters





                    printer.PrintLF("Pay   1          :" + report.Custom1.ToString().PadLeft(10, ' '));
                    printer.PrintLF("Pay   2          :" + report.Custom2.ToString().PadLeft(10, ' '));
                    printer.LineFeed();

                    counter++;
                    GrandTotal = GrandTotal + Total;
                    GrandGratuity = GrandGratuity + Gratuity;
                    GrandNetGratuity = GrandNetGratuity + NetGratuity;
                    GrandSupplyFee = GrandSupplyFee + SupplyFee;
                    GrandCommission = GrandCommission + Commission;
                    Total = 0;
                    Gratuity = 0;
                    NetGratuity = 0;
                    SupplyFee = 0;
                    Commission = 0;

                    //cut each employee separately

                    if (cut)
                    {
                        printer.Send();
                        printer.Cut();
                    }

                }
 
                printer.LineFeed();
                printer.LineFeed();

                if(counter > 1)
                {
                    if (cut)
                    {
                        printer.Center();
                        printer.LineFeed();
                        printer.PrintLF(store.Name);

                        printer.PrintLF(store.Address1);
                        if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                        printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                        printer.PrintLF(store.Phone);
                        printer.LineFeed();
                        printer.PrintLF(new String('=', receiptchars));

                        printer.PrintLF("Employee Commission Report");
                        printer.LineFeed();
                        printer.Left();
                    }
                        printer.PrintLF("================================================"); //48 characters
                        printer.DoubleHeight();

                        printer.PrintLF("Store Summary - " + daterange);
                        printer.LineFeed();
                        printer.DoubleHeightOFF();
                        printer.PrintLF("Sales       :" + GrandTotal.ToString().PadLeft(10, ' '));
                        printer.PrintLF("Supplies    :" + ("-" + GrandSupplyFee.ToString()).PadLeft(10, ' '));
                        printer.PrintLF("Net Sales   :" + Math.Round(GrandTotal - GrandSupplyFee, 2).ToString().PadLeft(10, ' '));
                        printer.PrintLF("Commission  :" + Math.Round(GrandCommission, 2).ToString().PadLeft(10, ' '));
                        printer.PrintLF("Tips        :" + GrandGratuity.ToString().PadLeft(10, ' '));
                        printer.PrintLF("Net Tips    :" + GrandNetGratuity.ToString().PadLeft(10, ' '));
                     
                
                }

                printer.LineFeed();
                printer.LineFeed();
                printer.Send(); //sends buffer to printer

                printer.Cut();


            }catch(Exception e)
            {

                TouchMessageBox.Show("Print Commission:" + e.Message);
            }
        }


        public void PrintSales(DateTime startdate, DateTime enddate, string ReportTitle)
        {
            try
            {

                int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
                string printername = GlobalSettings.Instance.ReceiptPrinter;
                Location store = GlobalSettings.Instance.Shop;


                //defaults to receipt width since some customer had the character width setup.
                int receiptchars = receiptwidth;
                //translate to chars if value is in millimeters
                //58mm printer = 32 chars  , 80mm printer = 48 chars
                if (receiptwidth == 58) receiptchars = 32;
                if (receiptwidth == 80) receiptchars = 48;



                if (printername == "none") return;


                //58mm printer = 32 chars  , 80mm printer = 48 chars
                ReceiptPrinter printer = new ReceiptPrinter(printername);

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
                if(startdate == enddate)
                {
                    printer.PrintLF(startdate.ToShortDateString());
                }else
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
                    printer.PrintLF(Utility.FormatPrintRow(rp.CatName, rp.CatValue.ToString(), receiptchars));

                }
                printer.PrintLF(Utility.FormatPrintRow("Tips Withheld:", dailyrevenue.TipsWitheld.ToString(), receiptchars));

                printer.PrintLF(Utility.FormatPrintRow("Total Discount:", "-" + dailyrevenue.TotalDiscount.ToString(), receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("Total Adjustment:", "-" + dailyrevenue.TotalAdjustment.ToString(), receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("Total Tax:", dailyrevenue.SalesTax.ToString(), receiptchars));
                printer.PrintLF(new String('-', receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("Total Revenue:", dailyrevenue.NetRevenue.ToString(), receiptchars));


                printer.LineFeed();
                printer.LineFeed();
                printer.DoubleHeight();
                printer.PrintLF("Settlement");
                printer.DoubleHeightOFF();

                printer.LineFeed();
                DailySettlement dailypayment;
                dailypayment = GetDailySettlement(startdate, enddate);

                decimal totalreceived = 0;
                foreach (ReportCat rp in dailypayment.PaymentCat)
                {
                    if (rp.CatName.ToUpper().Substring(0, 4) != "GIFT" && rp.CatName.ToUpper().Substring(0, 4) != "REWA" && rp.CatName.ToUpper().Substring(0,4) != "STAM")
                    {
                        printer.PrintLF(Utility.FormatPrintRow(rp.CatName, rp.CatValue.ToString(), receiptchars));
                        totalreceived += rp.CatValue;
                    }

                 
                    

                }
                printer.PrintLF(new String('-', receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("Total Payments:", totalreceived.ToString(), receiptchars));
                printer.LineFeed();

                //------------------------------------GIFT CARD-------------------------
                totalreceived = 0;
                foreach (ReportCat rp in dailypayment.PaymentCat)
                {
                    if (rp.CatName.ToUpper().Substring(0, 4) == "GIFT")
                    {
                        printer.PrintLF(Utility.FormatPrintRow(rp.CatName, rp.CatValue.ToString(), receiptchars));
                        totalreceived += rp.CatValue;
                    }

                }
                printer.PrintLF(new String('-', receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("Total GiftCard:", totalreceived.ToString(), receiptchars));
                printer.LineFeed();

                //-----------------------------------REWARDS--------------------------
                totalreceived = 0;
                foreach (ReportCat rp in dailypayment.PaymentCat)
                {
                    if (rp.CatName.ToUpper().Substring(0, 4) == "REWA" || rp.CatName.ToUpper().Substring(0, 4) == "STAM")
                    {
                        printer.PrintLF(Utility.FormatPrintRow(rp.CatName, rp.CatValue.ToString(), receiptchars));
                        totalreceived += rp.CatValue;
                    }

                }
                printer.PrintLF(new String('-', receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("Total Reward:", totalreceived.ToString(), receiptchars));
                printer.LineFeed();



                printer.PrintLF(Utility.FormatPrintRow("Total Settlements:", dailypayment.TotalPayment.ToString(), receiptchars));
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



        public void PrintPayments(DateTime startdate, DateTime enddate, string ReportTitle, int employeeid)
        {
            try
            {

                int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
                string printername = GlobalSettings.Instance.ReceiptPrinter;
                Location store = GlobalSettings.Instance.Shop;


                //defaults to receipt width since some customer had the character width setup.
                int receiptchars = receiptwidth;
                //translate to chars if value is in millimeters
                //58mm printer = 32 chars  , 80mm printer = 48 chars
                if (receiptwidth == 58) receiptchars = 32;
                if (receiptwidth == 80) receiptchars = 48;



                if (printername == "none") return;


                //58mm printer = 32 chars  , 80mm printer = 48 chars
                ReceiptPrinter printer = new ReceiptPrinter(printername);

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



                DataTable dat = _dbreports.GetDailyPaymentSummary(startdate, enddate,employeeid);

                foreach (DataRow row in dat.Rows)
                {

                    printer.DoubleHeight();
                    printer.PrintLF(row["cashier"].ToString());
                    printer.DoubleHeightOFF();
                   

                    printer.PrintLF(Utility.FormatPrintRow("Cash:", row["cash"].ToString(), receiptchars));

                    printer.PrintLF(Utility.FormatPrintRow("Credit:", row["credit"].ToString(), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Debit:", row["debit"].ToString(), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Gift:", ((decimal)row["giftcard"] + (decimal)row["giftcertificate"]).ToString(), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Reward:", row["reward"].ToString(), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("StampCard:", row["stampcard"].ToString(), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Tips:", row["tips"].ToString(), receiptchars));
                    printer.PrintLF(new String('-', receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Total Payments:", row["allpayments"].ToString(), receiptchars));

                    printer.LineFeed();
                }

                //whole store .. not just one employee  0 or -1
                if(employeeid <= 0)
                {
                    printer.DoubleHeight();
                    printer.PrintLF("Grand Total");
                    printer.DoubleHeightOFF();


                    printer.PrintLF(Utility.FormatPrintRow("Cash:", dat.AsEnumerable().Sum(x => x.Field<Decimal>("cash")).ToString(), receiptchars));

                    printer.PrintLF(Utility.FormatPrintRow("Credit:", dat.AsEnumerable().Sum(x => x.Field<Decimal>("credit")).ToString(), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Debit:", dat.AsEnumerable().Sum(x => x.Field<Decimal>("debit")).ToString(), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Gift:", (dat.AsEnumerable().Sum(x => x.Field<Decimal>("giftcard")) + dat.AsEnumerable().Sum(x => x.Field<Decimal>("giftcertificate"))).ToString(), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Reward:", dat.AsEnumerable().Sum(x => x.Field<Decimal>("reward")).ToString(), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("StampCard:", dat.AsEnumerable().Sum(x => x.Field<Decimal>("stampcard")).ToString(), receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Tips:", dat.AsEnumerable().Sum(x => x.Field<Decimal>("tips")).ToString(), receiptchars));
                    printer.PrintLF(new String('-', receiptchars));
                    printer.PrintLF(Utility.FormatPrintRow("Total Payments:", dat.AsEnumerable().Sum(x => x.Field<Decimal>("allpayments")).ToString(), receiptchars));

                    printer.LineFeed();
                }
               




                printer.Send(); //sends buffer to printer

                printer.Cut();

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Print Sales Report:" + e.Message);
            }



        }



        public void PrintDailyPayments(DateTime startdate,int employeeid)
        {
            try
            {

                PrintPayments(startdate, startdate, "Daily Payment Report",employeeid);

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Print Daily Payment Report:" + e.Message);
            }



        }



    }









}
