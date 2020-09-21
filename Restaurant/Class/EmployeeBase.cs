using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.Class
{


    public class EmployeeBase : INPCBase
    {

        protected string _pin;
        protected string _msrcard;
        protected bool _active;
        protected bool _intippool;
        protected bool _fingerprintbypass;
        protected string _lastname;
        protected string _firstname;
        protected string _middlename;
        protected string _displayname;
        protected string m_phone1;
        protected string m_phone2;
        protected string m_phone3;
        protected string m_address1;
        protected string m_address2;
        protected string m_city;
        protected string m_state;
        protected string m_zipcode;
        protected string m_email;
        protected string _title;
        protected int _securitylevel;
        protected string _role;

        protected string _ssn;
        protected int _badgeid;
        protected decimal _paynormal;
        protected decimal _payovertime;
        protected bool m_clockedin;

        protected EmployeeSchedule m_workschedule;
        protected ObservableCollection<PayDay> m_employeehourstips;

        protected string _imgurl;

        protected DBEmployee _dbEmployee;
        protected string xmlschedule;



        protected string _paytype;
        protected int _sortorder;

        protected DateTime _timein;
        protected DateTime _timeout;

        protected decimal _totalhours; //total hours worked for the current pay period


        protected string m_clockintime = "";
        protected string m_clockouttime = "";

        protected DataTable _hours;


        protected DataRow m_datarow;

       

        [DataMember]
        public int ID { get; set; }

     
 


        [DataMember]
        public EmployeeSchedule WorkSchedule
        {
            get { return m_workschedule; }
            set
            {
                m_workschedule = value;
                NotifyPropertyChanged("WorkSchedule");
            }
        }




        [DataMember]
        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                _dbEmployee.UpdateBoolean(ID, "active", _active);

                NotifyPropertyChanged("Active");
            }
        }
        [DataMember]
        public bool InTipPool
        {
            get { return _intippool; }
            set
            {
                _intippool = value;
                _dbEmployee.UpdateBoolean(ID, "intippool", _intippool);
                NotifyPropertyChanged("InTipPool");
            }
        }

        [DataMember]
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

        [DataMember]
        public string MSRCard
        {
            get { return _msrcard; }
            set
            {
                _msrcard = value;
                _dbEmployee.UpdateString(ID, "msrcard", _msrcard);
                NotifyPropertyChanged("MSRCard");

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

        [DataMember]
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
        [DataMember]
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

        [DataMember]
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

        [DataMember]
        public string FullName
        {
            get { return FirstName + " " + LastName; }

        }

        [DataMember]
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

        [DataMember]
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
        [DataMember]
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
        [DataMember]
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

        [DataMember]
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
                if(value.Length == GlobalSettings.Instance.PinLength)
                {
                    _pin = value;

                    _dbEmployee.UpdateString(ID, "pin", _pin);
                    NotifyPropertyChanged("PIN");
                }
             
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

        [DataMember]
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

        [DataMember]
        public string ReceiptStr
        {
            get
            {
                return Role + ":" + DisplayName;
            }

        }

        [DataMember]
        public DateTime TimeIn
        {
            get { return _timein; }
            set
            {
                _timein = value;
                NotifyPropertyChanged("TimeIn");
            }
        }
        [DataMember]
        public DateTime TimeOut
        {
            get { return _timeout; }
            set
            {
                _timeout = value;
                NotifyPropertyChanged("TimeOut");
            }
        }

        protected decimal _totalpooltiphours;
        public decimal TotalPoolTipHours
        {
            get { return _totalpooltiphours; }
            set
            {
                _totalpooltiphours = value;
                NotifyPropertyChanged("TotalPoolTipHours");
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




        protected decimal _totalpooltips;
        public decimal TotalPoolTips
        {
            get { return _totalpooltips; }
            set
            {
                _totalpooltips = value;
                NotifyPropertyChanged("TotalPoolTips");
            }
        }

        public int TimeID { get; set; }

        [DataMember]
        public string ClockinTime
        {
            get { return m_clockintime; }
            set
            {
                m_clockintime = value;
                NotifyPropertyChanged("ClockinTime");
            }
        }

        [DataMember]
        public string ClockoutTime
        {
            get { return m_clockouttime; }
            set
            {
                m_clockouttime = value;
                NotifyPropertyChanged("ClockoutTime");
            }
        }
    }
}
