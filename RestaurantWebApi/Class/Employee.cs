using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RedDot;



namespace RedDot.OrderService.Class
{
    public class Employee 
    {

        private string _imgurl;

        private DBEmployee _dbEmployee;
   
        private string _pin;
        private string _msrcard;
        private bool _active;
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
        private bool _appointment;
        private bool _sales;
        private decimal _paynormal;
      

        private string _paytype;
        private int _sortorder;
        private int _commission;
     

        private DataRow m_datarow;

        public  Employee(int id)
        {

            try
            {
                _dbEmployee = new DBEmployee();
               _lastname = "";
               ID = id;

               if (id > 0 )
               {
                   
                   DataTable table = _dbEmployee.GetEmployee(id);

                   if (table.Rows.Count >= 1)
                   {

                       m_datarow = table.Rows[0];
                       InitEmployee(table.Rows[0]);

                   }
               }


    
                
            }
            catch (Exception e)
            {

             //  TouchMessageBox.Show(e.Message);
               
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
        
            if (employeerow["pin"].ToString() != "") _pin                        = employeerow["pin"].ToString(); else _pin = "";
            if (employeerow["paynormal"].ToString() != "") _paynormal            = decimal.Parse(employeerow["paynormal"].ToString()); else _paynormal = 0;
             if (employeerow["sortorder"].ToString() != "") _sortorder            = int.Parse(employeerow["sortorder"].ToString()); else _sortorder = 0;
           
            _lastname                                                            = employeerow["lastname"].ToString();
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

            m_email                                                              = employeerow["email"].ToString();


            if (!_active) _securitylevel = 0;
        }



 

        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;

            }
        }


        public string MSRCard {get { return _msrcard; }
            set
            {
                _msrcard = value;

               
            }
        }

        public string SSN
        {
            get { return _ssn; }
            set
            {
                _ssn = value;

               
            }
        }


        public string LastName
        {
            get { return _lastname; }
            set
            {
                _lastname = value;

             
            }
        }
        public string MiddleName
        {
            get { return _middlename; }
            set
            {
                _middlename = value;

               
            }
        }

        public string FirstName
        {
            get { return _firstname; }
            set
            {
                _firstname = value;

                
            }
        }

        public string DisplayName
        {
            get { return _displayname; }
            set
            {
                _displayname = value;

               
            }
        }

        public string Phone1
        {
            get { return m_phone1; }
            set
            {
                m_phone1 = value;

            }
        }



        public string Address1
        {
            get { return m_address1; }
            set
            {
                m_address1 = value;
       
            }
        }

        public string Address2
        {
            get { return m_address2; }
            set
            {
                m_address2 = value;

            }
        }

        public string City
        {
            get { return m_city; }
            set
            {
                m_city = value;

            }
        }


        public string State
        {
            get { return m_state; }
            set
            {
                m_state = value;
  
            }
        }

        public string ZipCode
        {
            get { return m_zipcode; }
            set
            {
                m_zipcode = value;

            }
        }

        public string Email
        {
            get { return m_email; }
            set
            {
                m_email = value;

            }
        }
        public string Role
        {
            get { return _role; }
            set
            {
                _role = value;

                
            }
        }
        public string Title 
        {
            get { return _title; }
            set
            {
                _title = value;

                
            }
        }


        public string PayType
        {
            get { return _paytype; }
            set
            {
                _paytype = value;


            }
        }


        public int SecurityLevel
        {
            get { return _securitylevel; }
            set
            {
                _securitylevel = value;

            }
        }
        public string PIN
        {
            get { return _pin; }
            set
            {
              
                _pin = value;
 
            }
        }






        public int SortOrder
        {
            get { return _sortorder; }
            set
            {
                _sortorder = value;

            }
        }



        public decimal PayNormal
        {
            get { return _paynormal; }
            set
            {
                _paynormal = value;
      
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

                }

            }
        }
        public string ReceiptStr
        {
            get
            {
                return Role + ":" + DisplayName;
            }

        }

         

      
    }
}
