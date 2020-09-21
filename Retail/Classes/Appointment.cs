using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class Appointment:INPCBase
    {
        
       // Employee m_currentemployee;
        //Customer m_currentcustomer;
        DBAppointment m_dbappointment;
        private int m_length;
        private int m_intervallength;

        private Visibility m_visible;



  
        public Appointment(DataRow row, int intervallength)
        {
            
           // m_dbappointment = new DBAppointment();
            m_intervallength = intervallength;
            InitData(row);

        }

        public Appointment(int id, int intervallength)
        {
            if (m_dbappointment == null) m_dbappointment = new DBAppointment();

            DataRow row = m_dbappointment.GetAppointmentByID(id);
            m_intervallength = intervallength;
            InitData(row);
        }

        public Appointment(int intervallength)
        {
            m_intervallength = intervallength;
            m_id = 0;
            m_catcolorcode = "black";
            m_length = 1;
           // m_currentcustomer = null;
          //  m_currentemployee = null;
            m_note = "";
            m_status = "";
            m_catid = 0;

            
        }

        private void InitData(DataRow m_row)
        {
            if (m_row["id"].ToString() != "0") m_id                     = (int)m_row["id"]; else m_id= 0;
            if (m_row["catid"].ToString() != "0") m_catid               = (int)m_row["catid"];  else m_catid= 0;
            m_catcolorcode                                              =  m_row["catcolorcode"].ToString();
            m_apptdate                                                  = (DateTime)m_row["apptdate"];
            m_status                                                    = m_row["status"].ToString();
            if (m_row["length"].ToString() != "") m_length              = int.Parse(m_row["length"].ToString());
            m_note                                                      = m_row["note"].ToString();

            m_employeeid = int.Parse(m_row["employeeid"].ToString());
            m_customerid = int.Parse(m_row["customerid"].ToString());

            m_customerfirstname = m_row["customerfirstname"].ToString();
            m_customerlastname = m_row["customerlastname"].ToString();
            m_phonenumber = m_row["customerphonenumber"].ToString();
            m_employeename = m_row["displayname"].ToString();


            //  if (m_row["employeeid"].ToString() != "") m_currentemployee = new Employee(int.Parse(m_row["employeeid"].ToString()));

            //  if (m_row["customerid"].ToString() != "" && m_row["customerid"].ToString() != "0") m_currentcustomer = new Customer(int.Parse(m_row["customerid"].ToString()),  false);

        }

       public  void SaveData()
        {

            if (ID > 0)
            {
                if (m_dbappointment == null) m_dbappointment = new DBAppointment();
                m_dbappointment.UpdateNumeric(ID, "catid", m_catid);
                m_dbappointment.UpdateApptDate(m_apptdate, ID);
                m_dbappointment.UpdateString(ID, "status",m_status);
                m_dbappointment.UpdateNumeric(ID, "Length", m_length);
                m_dbappointment.UpdateString(ID, "note", m_note);
                m_dbappointment.UpdateEmployee(m_employeeid, ID);
                m_dbappointment.UpdateCustomer(m_customerid, ID);
            }
        }



        private int m_id = 0;
        public int ID
        {
            get{return m_id; }
        }


        private int m_customerid;
        public int CustomerID
        {
            get { return m_customerid; }
            set
            {
                m_customerid = value;
                NotifyPropertyChanged("CustomerID");
            }
        }

        private int m_employeeid;
        public int EmployeeID
        {
            get { return m_employeeid; }
   

        }
        private int m_catid;
        public int CatID
        {
            get { return m_catid; }
            set
            {
                m_catid = value;
                NotifyPropertyChanged("CatID");
                NotifyPropertyChanged("CatColorCode");
            }
        }

        private string m_catcolorcode;
        public string CatColorCode
        {
            get  { return m_catcolorcode; }
        }

        public Visibility Visible
        {
            get { return m_visible; }
            set { m_visible = value;
            NotifyPropertyChanged("Visible");
            }
        }
        private DateTime m_apptdate;
        public DateTime ApptDate
        {
            get
            {
                return m_apptdate;
            }
            set
            {
                    if(EmployeeID != 0)
                    {
                        if (m_dbappointment == null) m_dbappointment = new DBAppointment();
                        if (m_dbappointment.SpaceAvailable(ID, value, EmployeeID, Length))
                        {
                            m_apptdate = value;
                        }
                        else
                        {
                            MessageBox.Show("Time Slot not available!!");
                        }
                    }else
                    {
                        m_apptdate = value;
                    }


                NotifyPropertyChanged("ApptDate");
            }
        }

        public void UpdateEmployee(int employeeid)
        {
            if (m_dbappointment == null) m_dbappointment = new DBAppointment();
            if (m_dbappointment.SpaceAvailable(ID, ApptDate, employeeid, Length))
            {
                m_employeeid = employeeid;
                EmployeeName = (new Employee(employeeid)).DisplayName;

            }
           

        }
        private string m_status;
        public string Status
        {
            get
            {
                return m_status;
            }
            set {
                m_status = value;
                NotifyPropertyChanged("Status");
            }
        }
    
        public  int Length //( of # minutes)
        {
            get
            {
                return m_length;
            }
            set
            {
                if (EmployeeID != 0)
                {
                    if (m_dbappointment == null) m_dbappointment = new DBAppointment();
                    if (m_dbappointment.CheckLength(ApptDate, EmployeeID, value))
                    {
                        m_length = value;
                    }
                    
                }
                else m_length = value;


                NotifyPropertyChanged("Length");
            }
        }

        
        public int Height //number of slot ( of # minutes)
        {
            get
            {
                return 20 *  Length/m_intervallength;
            }

        }

        public int Width
        {
            get;
            set;
        }
        private string m_note;
        public string Note
        {
            get
            {
                return m_note;
            }
            set
            {
                m_note = value;
                NotifyPropertyChanged("Note");
            }
        }


        private string m_employeename;
        public string EmployeeName
        {
            get { return m_employeename; }
            set
            {
                m_employeename = value;
                NotifyPropertyChanged("EmployeeName");
            }
        }
        private string m_customerfirstname;
        public string CustomerFirstName
        {
            get { return m_customerfirstname; }
            set
            {
                m_customerfirstname = value;
                NotifyPropertyChanged("CustomerFirstName");
            }
        }

        private string m_customerlastname;
        public string CustomerLastName
        {
            get { return m_customerlastname; }
            set
            {
                m_customerlastname = value;
                NotifyPropertyChanged("CustomerLastName");
            }
        }


        private string m_phonenumber;
        public string CustomerPhoneNumber
        {
            get { return m_phonenumber; }
            set
            {
                m_phonenumber = value;
                NotifyPropertyChanged("CustomerPhoneNumber");
            }
        }
    }

  


}
