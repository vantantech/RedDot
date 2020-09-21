using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using RedDot;
using RedDot.Class;

namespace RedDot
{
    [DataContract]
    public class Employee : EmployeeBase
    {

       
       

        public  Employee(int id)
        {

            try
            {
                _dbEmployee = new DBEmployee();
               LastName = "";
               ID = id;

               if (id > 0 && id < 999)
               {
                   DataTable table = _dbEmployee.GetEmployee(id);

                   if (table.Rows.Count >= 1)
                   {
                       m_datarow = table.Rows[0];
                       InitEmployee(table.Rows[0]);
                   }
               }


               else
               {
                    if(id==0)  InitDefaultEmployee();


                   if (id == 999)
                   {
                       DisplayName = "All Employees";
                    
                   }

               }
                
            }
            catch (Exception e)
            {

               MessageBox.Show(e.Message);
               
            }

        }

        public Employee(DataRow employeerow)
        {
            if (_dbEmployee == null) _dbEmployee = new DBEmployee();
            m_datarow = employeerow;
            InitEmployee(employeerow);
           
        }


        public void Refresh()
        {
            InitEmployee(m_datarow);

        }

        public void InitEmployee(DataRow employeerow)
        {
            if (employeerow["id"].ToString() != "") ID                           = int.Parse(employeerow["id"].ToString()); else ID = 0;

            if (employeerow["securitylevel"].ToString() != "") _securitylevel    = int.Parse(employeerow["securitylevel"].ToString()); else _securitylevel = 0;

            if (employeerow["active"].ToString() != "")             _active      = int.Parse(employeerow["active"].ToString()) == 1 ? true : false;
            if (employeerow["intippool"].ToString() != "")      _intippool       = int.Parse(employeerow["intippool"].ToString()) == 1 ? true : false;
            if (employeerow["fingerprintbypass"].ToString() != "") _fingerprintbypass = int.Parse(employeerow["fingerprintbypass"].ToString()) == 1 ? true : false;
            if (employeerow["pin"].ToString() != "") _pin                        = employeerow["pin"].ToString(); else _pin = "";
            if (employeerow["badgeid"].ToString() != "") _badgeid                = int.Parse(employeerow["badgeid"].ToString()); else _badgeid = 0;
            if (employeerow["paynormal"].ToString() != "") _paynormal            = decimal.Parse(employeerow["paynormal"].ToString()); else _paynormal = 0;
            if (employeerow["payovertime"].ToString() != "") _payovertime        = decimal.Parse(employeerow["payovertime"].ToString()); else _payovertime = 0;
            if (employeerow["sortorder"].ToString() != "") _sortorder            = int.Parse(employeerow["sortorder"].ToString()); else _sortorder = 0;

            _lastname                                                            = employeerow["lastname"].ToString().Replace('\r', ' ').Replace('\n', ' '); 
            _middlename                                                          = employeerow["middlename"].ToString().Replace('\r', ' ').Replace('\n', ' ');
            _msrcard                                                             = employeerow["msrcard"].ToString();
            _ssn                                                                 = employeerow["ssn"].ToString();
            _firstname                                                           = employeerow["firstname"].ToString().Replace('\r', ' ').Replace('\n', ' ');
            _displayname                                                         = employeerow["displayname"].ToString().Replace('\r', ' ').Replace('\n', ' ');
            _paytype                                                             = employeerow["paytype"].ToString();
            _title                                                               = employeerow["title"].ToString().Replace('\r', ' ').Replace('\n', ' ');
            _role                                                                = employeerow["role"].ToString();
            _imgurl                                                              = employeerow["imagesrc"].ToString().Replace("\\","\\\\");

            m_address1                                                           = employeerow["address1"].ToString().Replace('\r', ' ').Replace('\n', ' ');
            m_address2                                                           = employeerow["address2"].ToString().Replace('\r', ' ').Replace('\n', ' ');
            m_city                                                               = employeerow["city"].ToString().Replace('\r', ' ').Replace('\n', ' ');
            m_state                                                              = employeerow["state"].ToString().Replace('\r', ' ').Replace('\n', ' ');
            m_zipcode                                                            = employeerow["zipcode"].ToString().Replace('\r', ' ').Replace('\n', ' ');
            m_phone1                                                             = employeerow["phone1"].ToString().Replace('\r', ' ').Replace('\n', ' ');
            m_email                                                              = employeerow["email"].ToString().Replace('\r', ' ').Replace('\n', ' ');

            xmlschedule = employeerow["schedule"].ToString();
            WorkSchedule = new EmployeeSchedule();
            if (xmlschedule != null && xmlschedule.ToString() != "")
            {
             
                var ser = new XmlSerializer(typeof(EmployeeSchedule));
                var reader = new StringReader(xmlschedule);
                WorkSchedule = (EmployeeSchedule)ser.Deserialize(reader);
               
            }

            EmployeeSales = new SalesReport();
            EmployeeSalesSummary = new SalesReport();

            if (!_active) _securitylevel = 0;

    

            GetLastTimeRecord();
          

        }

  

        private void InitDefaultEmployee()
        {

            _securitylevel = 0;
            _lastname      = "";
            _firstname     = "";
            _displayname   = "[None]";
            _securitylevel = 1;
                ID         = 0;
            _title         = "";
            _role          = "Staff";
                _imgurl    = "Images\\\\exclamation.jpg";

            EmployeeSales = new SalesReport();
            EmployeeSalesSummary = new SalesReport();

        }

     




        public DataTable GetEmployeeMeals(DateTime startdate, DateTime enddate)
        {
          return  _dbEmployee.GetEmployeeMeals(ID, startdate, enddate);
        }


        public DataTable Hours
        {
            get { return _hours; }
            set { _hours = value;
            NotifyPropertyChanged("Hours");
            }
        }

        private SalesReport employeesales;
        public SalesReport EmployeeSales
        {
            get { return employeesales; }
            set
            {
                employeesales = value;
                NotifyPropertyChanged("EmployeeSales");
            }
        }


        private SalesReport employeesalessummary;
        public SalesReport EmployeeSalesSummary
        {
            get { return employeesalessummary; }
            set
            {
                employeesalessummary = value;
                NotifyPropertyChanged("EmployeeSalesSummary");
            }
        }




       
        ///----------------------------------------------------- public methods


        public bool DeleteEmployee()
        {

            return _dbEmployee.DeleteEmployee(ID);
        }

        public bool ClockIn(DateTime time, bool forced)
        {
            int hour=0;
            int minute=0;

            //skip time checking code and just checkin
            if(forced)
            {
                _dbEmployee.InsertTimeIn(ID, time);
                GetLastTimeRecord();
                return true;
            }


            DateTime shift1start = new DateTime(time.Year, time.Month, time.Day, GlobalSettings.Instance.Shift1Start.Hours, GlobalSettings.Instance.Shift1Start.Minutes, 0);
            DateTime shift1end = new DateTime(time.Year, time.Month, time.Day, GlobalSettings.Instance.Shift1End.Hours, GlobalSettings.Instance.Shift1End.Minutes, 0);

            DateTime shift2start = new DateTime(time.Year, time.Month, time.Day, GlobalSettings.Instance.Shift2Start.Hours, GlobalSettings.Instance.Shift2Start.Minutes, 0);
            DateTime shift2end = new DateTime(time.Year, time.Month, time.Day, GlobalSettings.Instance.Shift2End.Hours, GlobalSettings.Instance.Shift2End.Minutes, 0);

            DateTime shift3start = new DateTime(time.Year, time.Month, time.Day, GlobalSettings.Instance.Shift3Start.Hours, GlobalSettings.Instance.Shift3Start.Minutes, 0);
            DateTime shift3end = new DateTime(time.Year, time.Month, time.Day, GlobalSettings.Instance.Shift3End.Hours, GlobalSettings.Instance.Shift3End.Minutes, 0);


            //check to see if allowed based on time schedule
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    if(shift1start <= DateTime.Now && DateTime.Now <= shift1end)
                    {
                        hour = WorkSchedule.Monday.Shift1.StartTime.Hour;
                        minute = WorkSchedule.Monday.Shift1.StartTime.Minute;
                        break;
                    }
                    if (shift2start <= DateTime.Now && DateTime.Now <= shift2end)
                    {
                        hour = WorkSchedule.Monday.Shift2.StartTime.Hour;
                        minute = WorkSchedule.Monday.Shift2.StartTime.Minute;
                        break;
                    }
                    if (shift3start <= DateTime.Now && DateTime.Now <= shift3end)
                    {
                        hour = WorkSchedule.Monday.Shift3.StartTime.Hour;
                        minute = WorkSchedule.Monday.Shift3.StartTime.Minute;
                        break;
                    }
                    hour = 23;
                    minute = 59;
                    break;

                case DayOfWeek.Tuesday:
                    if (shift1start <= DateTime.Now && DateTime.Now <= shift1end)
                    {
                        hour = WorkSchedule.Tuesday.Shift1.StartTime.Hour;
                        minute = WorkSchedule.Tuesday.Shift1.StartTime.Minute;
                        break;
                    }
                    if (shift2start <= DateTime.Now && DateTime.Now <= shift2end)
                    {
                        hour = WorkSchedule.Tuesday.Shift2.StartTime.Hour;
                        minute = WorkSchedule.Tuesday.Shift2.StartTime.Minute;
                        break;
                    }
                    if (shift3start <= DateTime.Now && DateTime.Now <= shift3end)
                    {
                        hour = WorkSchedule.Tuesday.Shift3.StartTime.Hour;
                        minute = WorkSchedule.Tuesday.Shift3.StartTime.Minute;
                        break;
                    }
                    hour = 23;
                    minute = 59;
                    break;

                case DayOfWeek.Wednesday:
                    if (shift1start <= DateTime.Now && DateTime.Now <= shift1end)
                    {
                        hour = WorkSchedule.Wednesday.Shift1.StartTime.Hour;
                        minute = WorkSchedule.Wednesday.Shift1.StartTime.Minute;
                        break;
                    }
                    if (shift2start <= DateTime.Now && DateTime.Now <= shift2end)
                    {
                        hour = WorkSchedule.Wednesday.Shift2.StartTime.Hour;
                        minute = WorkSchedule.Wednesday.Shift2.StartTime.Minute;
                        break;
                    }
                    if (shift3start <= DateTime.Now && DateTime.Now <= shift3end)
                    {
                        hour = WorkSchedule.Wednesday.Shift3.StartTime.Hour;
                        minute = WorkSchedule.Wednesday.Shift3.StartTime.Minute;
                        break;
                    }
                    hour = 23;
                    minute = 59;
                    break;

                case DayOfWeek.Thursday:
                    if (shift1start <= DateTime.Now && DateTime.Now <= shift1end)
                    {
                        hour = WorkSchedule.Thursday.Shift1.StartTime.Hour;
                        minute = WorkSchedule.Thursday.Shift1.StartTime.Minute;
                        break;
                    }
                    if (shift2start <= DateTime.Now && DateTime.Now <= shift2end)
                    {
                        hour = WorkSchedule.Thursday.Shift2.StartTime.Hour;
                        minute = WorkSchedule.Thursday.Shift2.StartTime.Minute;
                        break;
                    }
                    if (shift3start <= DateTime.Now && DateTime.Now <= shift3end)
                    {
                        hour = WorkSchedule.Thursday.Shift3.StartTime.Hour;
                        minute = WorkSchedule.Thursday.Shift3.StartTime.Minute;
                        break;
                    }
                    hour = 23;
                    minute = 59;
                    break;
                case DayOfWeek.Friday:
                    if (shift1start <= DateTime.Now && DateTime.Now <= shift1end)
                    {
                        hour = WorkSchedule.Friday.Shift1.StartTime.Hour;
                        minute = WorkSchedule.Friday.Shift1.StartTime.Minute;
                        break;
                    }
                    if (shift2start <= DateTime.Now && DateTime.Now <= shift2end)
                    {
                        hour = WorkSchedule.Friday.Shift2.StartTime.Hour;
                        minute = WorkSchedule.Friday.Shift2.StartTime.Minute;
                        break;
                    }
                    if (shift3start <= DateTime.Now && DateTime.Now <= shift3end)
                    {
                        hour = WorkSchedule.Friday.Shift3.StartTime.Hour;
                        minute = WorkSchedule.Friday.Shift3.StartTime.Minute;
                        break;
                    }
                    hour = 23;
                    minute = 59;
                    break;

                case DayOfWeek.Saturday:
                    if (shift1start <= DateTime.Now && DateTime.Now <= shift1end)
                    {
                        hour = WorkSchedule.Saturday.Shift1.StartTime.Hour;
                        minute = WorkSchedule.Saturday.Shift1.StartTime.Minute;
                        break;
                    }
                    if (shift2start <= DateTime.Now && DateTime.Now <= shift2end)
                    {
                        hour = WorkSchedule.Saturday.Shift2.StartTime.Hour;
                        minute = WorkSchedule.Saturday.Shift2.StartTime.Minute;
                        break;
                    }
                    if (shift3start <= DateTime.Now && DateTime.Now <= shift3end)
                    {
                        hour = WorkSchedule.Saturday.Shift3.StartTime.Hour;
                        minute = WorkSchedule.Saturday.Shift3.StartTime.Minute;
                        break;
                    }
                    hour = 23;
                    minute = 59;
                    break;
                case DayOfWeek.Sunday:
                    if (shift1start <= DateTime.Now && DateTime.Now <= shift1end)
                    {
                        hour = WorkSchedule.Sunday.Shift1.StartTime.Hour;
                        minute = WorkSchedule.Sunday.Shift1.StartTime.Minute;
                        break;
                    }
                    if (shift2start <= DateTime.Now && DateTime.Now <= shift2end)
                    {
                        hour = WorkSchedule.Sunday.Shift2.StartTime.Hour;
                        minute = WorkSchedule.Sunday.Shift2.StartTime.Minute;
                        break;
                    }
                    if (shift3start <= DateTime.Now && DateTime.Now <= shift3end)
                    {
                        hour = WorkSchedule.Sunday.Shift3.StartTime.Hour;
                        minute = WorkSchedule.Sunday.Shift3.StartTime.Minute;
                        break;
                    }
                    hour = 23;
                    minute = 59;
                    break;


            }

            DateTime scheduletime = new DateTime(time.Year, time.Month, time.Day, hour, minute, 0);

            //?? minutes before starttime is allowed
            int earlyclockinallowance = GlobalSettings.Instance.EarlyClockInAllowance;
            if (time >= scheduletime.AddMinutes(-10) )
            {

                _dbEmployee.InsertTimeIn(ID, time);
                GetLastTimeRecord();
                return true;
            }

            bool response = Override.Ask("Clock In NOT Allowed until after " + scheduletime.AddMinutes((-1) * earlyclockinallowance).ToShortTimeString(),GlobalSettings.Instance.AutoAskTimeout);
            if(response)
            {
               
                if(SecurityModel.ManagerOverride("TimeCard","Enter Manager Override!!"))
                {
                    _dbEmployee.InsertTimeIn(ID, time);
                    GetLastTimeRecord();
                    return true;
                }
            }
            return false;
        }

        public void ClockOut(DateTime time)
        {
            //before clockin out .. employee needs to close all tickets
            DataTable dt =  _dbEmployee.GetEmployeeOpenOrders(ID);

            if(dt.Rows.Count > 0)
            {
                TouchMessageBox.Show("Must Close Out or Transfer all Open Orders before Clock Out");
                return;
            }

            _dbEmployee.UpdateTimeOut(TimeID, time);
            GetLastTimeRecord();


            TouchMessageBox.Show(FullName + "\n\rClock Out Time:" + ClockoutTime);

        }

        public void GetLastTimeRecord()
        {
 
            TimeID = 0;
            ClockedIn = false;
            ClockinTime = "";
            ClockoutTime = "";
            if (_dbEmployee == null) _dbEmployee = new DBEmployee();
            DataTable dt = _dbEmployee.GetLastTimeRecord(ID);
            if(dt.Rows.Count > 0)
            {
                TimeID = int.Parse( dt.Rows[0]["id"].ToString());
                TimeIn = Convert.ToDateTime(dt.Rows[0]["timein"].ToString());
                ClockinTime = TimeIn.ToString();
                ClockedIn = true;
                if (dt.Rows[0]["timeout"].ToString() != "")
                {
                    TimeOut = Convert.ToDateTime(dt.Rows[0]["timeout"].ToString());
                    ClockoutTime = TimeOut.ToString();
                    //  TimeSpan total = TimeOut.Subtract(TimeIn);
                    // HoursWorked = (decimal) total.TotalHours;
                    ClockedIn = false;
                }
            }
            
        }

       
        public bool ClockedIn
        {
            get { return m_clockedin; }
            set
            {
                m_clockedin = value;
                NotifyPropertyChanged("ClockedIn");
            }
        }
        public void RunReports(DateTime startdate, DateTime enddate)
        {
            LoadHours(startdate, enddate);
            GetSales(startdate, enddate);
            GetSalesSummary(startdate, enddate);
            GetHoursWorkedTips(startdate, enddate);
        }

        public void LoadHours(DateTime startdate, DateTime enddate)
        {
            if (_dbEmployee == null) _dbEmployee = new DBEmployee();
            Hours = _dbEmployee.GetEmployeeHours(ID, startdate, enddate);

            decimal total = 0;
            foreach (DataRow row in Hours.Rows)
            {
                string hourstr = row["hours"].ToString();
                if(hourstr != "")
                    total += decimal.Parse(hourstr);
            }
            TotalHours = total;
        }

        public void GetSales(DateTime startdate, DateTime enddate)
        {
            if (_dbEmployee == null) _dbEmployee = new DBEmployee();
            EmployeeSales.Sales = _dbEmployee.GetEmployeeSales(ID, startdate, enddate);

           
        }

        public void GetSalesSummary(DateTime startdate, DateTime enddate)
        {
            if (_dbEmployee == null) _dbEmployee = new DBEmployee();
            EmployeeSalesSummary.Sales = _dbEmployee.GetEmployeeSales(ID, startdate, enddate,true);

        }

     
        public ObservableCollection<PayDay> EmployeeHoursTips
        {
            get { return m_employeehourstips; }
            set
            {
                m_employeehourstips = value;
                NotifyPropertyChanged("EmployeeHoursTips");
            }
        }

        public void GetHoursWorkedTips(DateTime startdate, DateTime enddate)
        {

   


            ObservableCollection<PayDay> employeehourstips = new ObservableCollection<PayDay>();
            TimeSpan numofdays = enddate - startdate;
            DateTime currentdate = startdate;
            decimal totalpooltips = 0;
            decimal totalpooltiphour = 0;
            for(int i=0; i <= numofdays.TotalDays;i++)
            {
                DateTime shift1start = new DateTime(currentdate.Year, currentdate.Month, currentdate.Day, GlobalSettings.Instance.Shift1Start.Hours, GlobalSettings.Instance.Shift1Start.Minutes, 0);
                DateTime shift1end = new DateTime(currentdate.Year, currentdate.Month, currentdate.Day, GlobalSettings.Instance.Shift1End.Hours, GlobalSettings.Instance.Shift1End.Minutes, 0);

                DateTime shift2start = new DateTime(currentdate.Year, currentdate.Month, currentdate.Day, GlobalSettings.Instance.Shift2Start.Hours, GlobalSettings.Instance.Shift2Start.Minutes, 0);
                DateTime shift2end = new DateTime(currentdate.Year, currentdate.Month, currentdate.Day, GlobalSettings.Instance.Shift2End.Hours, GlobalSettings.Instance.Shift2End.Minutes, 0);

                DateTime shift3start = new DateTime(currentdate.Year, currentdate.Month, currentdate.Day, GlobalSettings.Instance.Shift3Start.Hours, GlobalSettings.Instance.Shift3Start.Minutes, 0);
                DateTime shift3end = new DateTime(currentdate.Year, currentdate.Month, currentdate.Day, GlobalSettings.Instance.Shift3End.Hours, GlobalSettings.Instance.Shift3End.Minutes, 0);

                PayDay pay = new PayDay();
                pay.PayDate = currentdate;
                pay.Shift1Hours =Math.Round( _dbEmployee.GetTotalShiftHours(ID, shift1start, shift1end),2);
                pay.Shift2Hours = Math.Round(_dbEmployee.GetTotalShiftHours(ID, shift2start, shift2end), 2);
                pay.Shift3Hours = Math.Round(_dbEmployee.GetTotalShiftHours(ID, shift3start, shift3end), 2);

                if(InTipPool)
                {
                    if (pay.Shift1Hours >= 1)
                        pay.Shift1Tip = GetPoolTip(shift1start, shift1end);
                    else pay.Shift1Tip = 0;

                    if (pay.Shift2Hours >= 1)
                        pay.Shift2Tip = GetPoolTip(shift2start, shift2end);
                    else pay.Shift2Tip = 0;

                    if (pay.Shift3Hours >= 1)
                        pay.Shift3Tip = GetPoolTip(shift3start, shift3end);
                    else pay.Shift3Tip = 0;
                }
         


                currentdate = currentdate.AddDays(1);
                employeehourstips.Add(pay);
                totalpooltips += pay.TipAmount;
                totalpooltiphour += pay.HoursWorked;
            }

            EmployeeHoursTips = employeehourstips;
            TotalPoolTips = totalpooltips;
            TotalPoolTipHours = totalpooltiphour;


        }
        public decimal GetPoolTip(DateTime shiftstart, DateTime shiftend )
        {
    

            if (shiftstart != shiftend)
            {
                decimal shifttip = _dbEmployee.GetShiftTips(shiftstart, shiftend);
                decimal numofworker = _dbEmployee.GetTotalWorkers(shiftstart, shiftend);

                if (numofworker == 0) return 0;

                return shifttip / numofworker;
            }
            else return 0;
        
        }


  
        public void SaveSchedule()
        {
            var stringBuilderXml = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(EmployeeSchedule));
            var xmlWriter = XmlWriter.Create(stringBuilderXml, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            xmlSerializer.Serialize(xmlWriter, WorkSchedule);
            xmlschedule = stringBuilderXml.ToString();
            _dbEmployee.UpdateString(ID, "schedule", xmlschedule);
        }


     
    }
}
