using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RedDot;



namespace RedDot
{
    public class Employee : INPCBase
    {

        private string _imgurl;

        private DBEmployee _dbEmployee;
   


        private string _pin;
        private string _msrcard;
        private bool _active;
        private bool _onschedule;
        private bool _fingerprintbypass;
        private bool _sales;
        private string _lastname;
        private string _firstname;
        private string _middlename;
        private string _displayname;
        private string m_phone1;
        private string m_phone2;
        private string m_phone3;
        private string m_address1;
        private string m_address2;
        private string m_city;
        private string m_state;
        private string m_zipcode;
        private string m_email;
        private string _title;
        private int _securitylevel;
        private string _role;
        public int ID { get; set; }
        private string _ssn;
        private int _badgeid;
        private decimal _paynormal;
        private decimal _payovertime;

        private decimal _commissionproduct;
        private decimal _commissionlabor;

        private string _paytype;
        private int _sortorder;

        private DateTime _timein;
        private DateTime _timeout;
        private decimal _hoursworked; //hours worked during current day
        private decimal _totalhours; //total hours worked for the current pay period


        private string m_clockintime = "";
        private string m_clockouttime = "";

        private DataTable _hours;

        private DataRow m_datarow;

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
            m_datarow = employeerow;
            InitEmployee(employeerow);
            if (_dbEmployee == null) _dbEmployee = new DBEmployee();
        }


        public void Refresh()
        {
            InitEmployee(m_datarow);

        }

        public void InitEmployee(DataRow employeerow)
        {
            if (employeerow["id"].ToString() != "") ID                           = int.Parse(employeerow["id"].ToString()); else ID = 0;

            if (employeerow["securitylevel"].ToString() != "") _securitylevel    = int.Parse(employeerow["securitylevel"].ToString()); else _securitylevel = 0;

            if (employeerow["active"].ToString() != "") _active                  = int.Parse(employeerow["active"].ToString()) == 1 ? true : false;
            if (employeerow["onschedule"].ToString() != "") _onschedule = int.Parse(employeerow["onschedule"].ToString()) == 1 ? true : false;
            if (employeerow["pin"].ToString() != "") _pin                        = employeerow["pin"].ToString(); else _pin = "";
            if (employeerow["badgeid"].ToString() != "") _badgeid                = int.Parse(employeerow["badgeid"].ToString()); else _badgeid = 0;
            if (employeerow["paynormal"].ToString() != "") _paynormal            = decimal.Parse(employeerow["paynormal"].ToString()); else _paynormal = 0;
            if (employeerow["payovertime"].ToString() != "") _payovertime        = decimal.Parse(employeerow["payovertime"].ToString()); else _payovertime = 0;
            if (employeerow["sortorder"].ToString() != "") _sortorder            = int.Parse(employeerow["sortorder"].ToString()); else _sortorder = 0;
            if (employeerow["fingerprintbypass"].ToString() != "") _fingerprintbypass = int.Parse(employeerow["fingerprintbypass"].ToString()) == 1 ? true : false;
            if (employeerow["commissionproduct"].ToString() != "") _commissionproduct = decimal.Parse(employeerow["commissionproduct"].ToString()); else _commissionproduct = 0;
            if (employeerow["commissionlabor"].ToString() != "") _commissionlabor = decimal.Parse(employeerow["commissionlabor"].ToString()); else _commissionlabor = 0;
            if (employeerow["sales"].ToString() != "") _sales = int.Parse(employeerow["sales"].ToString()) == 1 ? true : false;

            _lastname = employeerow["lastname"].ToString();
            _middlename                                                          = employeerow["middlename"].ToString();
            _msrcard                                                             = employeerow["msrcard"].ToString();
            _ssn                                                                 = employeerow["ssn"].ToString();
            _firstname                                                           = employeerow["firstname"].ToString();
            _displayname                                                         = employeerow["displayname"].ToString();
            _paytype                                                             = employeerow["paytype"].ToString();
            _title                                                               = employeerow["title"].ToString();
            _role                                                                = employeerow["role"].ToString();
            _imgurl                                                              = employeerow["imagesrc"].ToString().Replace("\\","\\\\");

            m_address1                                                           = employeerow["address1"].ToString();
            m_address2                                                           = employeerow["address2"].ToString();
            m_city                                                               = employeerow["city"].ToString();
            m_state                                                              = employeerow["state"].ToString();
            m_zipcode                                                            = employeerow["zipcode"].ToString();
            m_phone1                                                             = employeerow["phone1"].ToString();
            m_phone2                                                             = employeerow["phone2"].ToString();
            m_phone3                                                             = employeerow["phone3"].ToString();
            m_email                                                              = employeerow["email"].ToString();


            if (!_active) _securitylevel = 0;
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
            _role          = "Sales Person";
                _imgurl    = "Images\\\\exclamation.jpg";

         

        }

 

        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                _dbEmployee.UpdateActive(ID, _active);
                NotifyPropertyChanged("Active");
            }
        }

        public bool OnSchedule
        {
            get { return _onschedule; }
            set
            {
                _onschedule = value;
                _dbEmployee.UpdateOnSchedule(ID, _onschedule);
                NotifyPropertyChanged("OnSchedule");
            }
        }

        public bool FingerPrintBypass
        {
            get { return _fingerprintbypass; }
            set
            {
                _fingerprintbypass = value;
                _dbEmployee.UpdateBoolean(ID, "fingerprintbypass", _fingerprintbypass);
                NotifyPropertyChanged("FingerPrintBypass");
            }
        }


        public bool Sales
        {
            get { return _sales; }
            set
            {
                _sales = value;
                _dbEmployee.UpdateBoolean(ID, "sales", _sales);
                NotifyPropertyChanged("Sales");
            }
        }


        public string MSRCard {get { return _msrcard; }
            set
            {
                _msrcard = value;
                _dbEmployee.UpdateString(ID, "msrcard", _msrcard);
                NotifyPropertyChanged("MSRCard");
               
            }
        }

        public string SSN
        {
            get { return _ssn; }
            set
            {
                _ssn = value;
                _dbEmployee.UpdateString(ID, "ssn", _ssn);
                NotifyPropertyChanged("SSN");
               
            }
        }


        public string LastName
        {
            get { return _lastname; }
            set
            {
                _lastname = value;
                _dbEmployee.UpdateString(ID, "lastname", _lastname);
                NotifyPropertyChanged("LastName");
             
            }
        }
        public string MiddleName
        {
            get { return _middlename; }
            set
            {
                _middlename = value;
                _dbEmployee.UpdateString(ID, "MiddleName", _middlename);
                NotifyPropertyChanged("MiddleName");
               
            }
        }

        public string FirstName
        {
            get { return _firstname; }
            set
            {
                _firstname = value;
                _dbEmployee.UpdateString(ID, "FirstName", _firstname);
                NotifyPropertyChanged("FirstName");
                
            }
        }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        public string DisplayName
        {
            get { return _displayname; }
            set
            {
                _displayname = value;
                _dbEmployee.UpdateString(ID, "DisplayName", _displayname);
                NotifyPropertyChanged("DisplayName");
               
            }
        }

        public string Phone1
        {
            get { return m_phone1; }
            set
            {
                m_phone1 = value;
                _dbEmployee.UpdateString(ID, "Phone1", m_phone1);
                NotifyPropertyChanged("Phone1");
            }
        }

        public string Phone2
        {
            get { return m_phone2; }
            set
            {
                m_phone2 = value;
                _dbEmployee.UpdateString(ID, "Phone2", m_phone2);
                NotifyPropertyChanged("Phone2");
            }
        }

        public string Phone3
        {
            get { return m_phone3; }
            set
            {
                m_phone3 = value;
                _dbEmployee.UpdateString(ID, "Phone3", m_phone3);
                NotifyPropertyChanged("Phone3");
            }
        }

        public string Address1
        {
            get { return m_address1; }
            set
            {
                m_address1 = value;
                _dbEmployee.UpdateString(ID, "Address1", m_address1);
                NotifyPropertyChanged("Address1");
            }
        }

        public string Address2
        {
            get { return m_address2; }
            set
            {
                m_address2 = value;
                _dbEmployee.UpdateString(ID, "Address2", m_address2);
                NotifyPropertyChanged("Address2");
            }
        }

        public string City
        {
            get { return m_city; }
            set
            {
                m_city = value;
                _dbEmployee.UpdateString(ID, "City", m_city);
                NotifyPropertyChanged("City");
            }
        }


        public string State
        {
            get { return m_state; }
            set
            {
                m_state = value;
                _dbEmployee.UpdateString(ID, "State", m_state);
                NotifyPropertyChanged("State");
            }
        }

        public string ZipCode
        {
            get { return m_zipcode; }
            set
            {
                m_zipcode = value;
                _dbEmployee.UpdateString(ID, "ZipCode", m_zipcode);
                NotifyPropertyChanged("ZipCode");
            }
        }

        public string Email
        {
            get { return m_email; }
            set
            {
                m_email = value;
                _dbEmployee.UpdateString(ID, "Email", m_email);
                NotifyPropertyChanged("Email");
            }
        }
        public string Role
        {
            get { return _role; }
            set
            {
                _role = value;
                _dbEmployee.UpdateString(ID, "Role", _role);
                NotifyPropertyChanged("Role");
                
            }
        }
        public string Title 
        {
            get { return _title; }
            set
            {
                _title = value;
                _dbEmployee.UpdateString(ID, "Title", _title);
                NotifyPropertyChanged("Title");
                
            }
        }


        public string PayType
        {
            get { return _paytype; }
            set
            {
                _paytype = value;
                _dbEmployee.UpdateString(ID, "paytype", _paytype);
                NotifyPropertyChanged("PayType");

            }
        }


        public int SecurityLevel
        {
            get { return _securitylevel; }
            set
            {
                _securitylevel = value;
                _dbEmployee.UpdateNumeric(ID, "SecurityLevel", _securitylevel);
               NotifyPropertyChanged("SecurityLevel");
            }
        }
        public string PIN
        {
            get { return _pin; }
            set
            {
              
                _pin = value;
                _dbEmployee.UpdateString(ID, "pin", _pin);
                NotifyPropertyChanged("PIN");
            }
        }

        public int BadgeID
        {
            get { return _badgeid; }
            set
            {
                _badgeid = value;
                _dbEmployee.UpdateNumeric(ID, "badgeid", _badgeid);
                NotifyPropertyChanged("BadgeID");
            }
        }

        public int SortOrder
        {
            get { return _sortorder; }
            set
            {
                _sortorder = value;
                _dbEmployee.UpdateNumeric(ID, "sortorder", _sortorder);
                NotifyPropertyChanged("SortOrder");
            }
        }


        public decimal PayNormal
        {
            get { return _paynormal; }
            set
            {
                _paynormal = value;
                _dbEmployee.UpdateNumeric(ID, "paynormal", _paynormal);
                NotifyPropertyChanged("PayNormal");
            }
        }


        public decimal CommissionProduct
        {
            get { return _commissionproduct; }
            set
            {
                _commissionproduct = value;
                _dbEmployee.UpdateNumeric(ID, "commissionproduct", _commissionproduct);
                NotifyPropertyChanged("CommissionProduct");
            }
        }


        public decimal CommissionLabor
        {
            get { return _commissionlabor; }
            set
            {
                _commissionlabor = value;
                _dbEmployee.UpdateNumeric(ID, "commissionlabor", _commissionlabor);
                NotifyPropertyChanged("CommissionLabor");
            }
        }



        public decimal PayOvertime
        {
            get { return _payovertime; }
            set
            {
                _payovertime = value;
                _dbEmployee.UpdateNumeric(ID, "payovertime", _payovertime);
                NotifyPropertyChanged("PayOvertime");
            }
        }
        public string ImageSrc
        {
            get
            {
                return "pack://siteoforigin:,,,/" + _imgurl;
                // return "pack://application:,,,/" + _imgurl;

            }
            set
            {
                if (value != null)
                {
                    _imgurl = value;
                    _dbEmployee.UpdateString(ID, "imagesrc", _imgurl);
                    NotifyPropertyChanged("ImageSrc");
                }

            }
        }

        protected string fmd1;
        public string FMD1
        {
            get { return fmd1; }
            set
            {
                fmd1 = value;
                _dbEmployee.UpdateString(ID, "fmd1", fmd1);
                NotifyPropertyChanged("FMD1");
            }
        }

        protected string fmd2;
        public string FMD2
        {
            get { return fmd2; }
            set
            {
                fmd2 = value;
                _dbEmployee.UpdateString(ID, "fmd2", fmd2);
                NotifyPropertyChanged("FMD2");
            }
        }


        public string ReceiptStr
        {
            get
            {
                return Role + ":" + DisplayName;
            }

        }

        public DateTime TimeIn
        {
            get { return _timein; }
            set { _timein = value;
            NotifyPropertyChanged("TimeIn");
            }
        }
        public DateTime TimeOut
        {
            get { return _timeout; }
            set
            {
                _timeout = value;
                NotifyPropertyChanged("TimeOut");
            }
        }

        public decimal HoursWorked {
            get { return _hoursworked; }
            set { _hoursworked = value;
            NotifyPropertyChanged("HoursWorked");
            }
        }

        public decimal TotalHours
        {
            get { return _totalhours; }
            set
            {
                _totalhours = value;
                NotifyPropertyChanged("TotalHours");
            }
        }
        public DataTable Hours
        {
            get { return _hours; }
            set { _hours = value;
            NotifyPropertyChanged("Hours");
            }
        }

        public int TimeID { get; set; }


        public string ClockinTime
        {
            get { return m_clockintime; }
            set
            {
                m_clockintime = value;
                NotifyPropertyChanged("ClockinTime");
            }
        }


        public string ClockoutTime
        {
            get { return m_clockouttime; }
            set
            {
                m_clockouttime = value;
                NotifyPropertyChanged("ClockoutTime");
            }
        }
        ///----------------------------------------------------- public methods


        public bool DeleteEmployee()
        {

            return _dbEmployee.DeleteEmployee(ID);
        }

        public void ClockIn(DateTime time)
        {

            _dbEmployee.InsertTimeIn(ID,time);
            GetLastTimeRecord();
        }

        public void ClockOut(DateTime time)
        {
            _dbEmployee.UpdateTimeOut(TimeID, time);
            GetLastTimeRecord();
        }

        public void GetLastTimeRecord()
        {
 
            TimeID = 0;
            HoursWorked = 0;
            ClockinTime = "";
            ClockoutTime = "";
            HasTimeRecord = false;
            if (_dbEmployee == null) _dbEmployee = new DBEmployee();
            DataTable dt = _dbEmployee.GetLastTimeRecord(ID);
            if(dt.Rows.Count > 0)
            {
                HasTimeRecord = true;
                TimeID = int.Parse( dt.Rows[0]["id"].ToString());
                TimeIn = Convert.ToDateTime(dt.Rows[0]["timein"].ToString());
                ClockinTime = TimeIn.ToString();
                if (dt.Rows[0]["timeout"].ToString() != "")
                {
                    TimeOut = Convert.ToDateTime(dt.Rows[0]["timeout"].ToString());
                    ClockoutTime = TimeOut.ToString();
                    TimeSpan total = TimeOut.Subtract(TimeIn);
                    HoursWorked = (decimal) total.TotalHours;
                }
            }
            
        }

        public bool HasTimeRecord { get; set; }

          public void LoadHours(DateTime startdate, DateTime enddate)
            {
                if (_dbEmployee == null) _dbEmployee = new DBEmployee();
                Hours = _dbEmployee.GetEmployeeHours(ID,startdate,enddate);
                TotalHours = _dbEmployee.GetTotalHours(ID, startdate, enddate);
            }
    }
}
