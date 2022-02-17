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
using RedDot.DataManager;

namespace RedDot
{
    public class Appointment:INPCBase
    {
        
        Employee m_currentemployee;
        Customer m_currentcustomer;
        IDataInterface m_dbappointment;
        private int m_length;
        private int m_intervallength;

        private Visibility m_visible;

        DataTable m_selectedcategories;
        public DataTable SelectedCategories
        {
            get { return m_selectedcategories; }

            set
            {
                m_selectedcategories = value;
                NotifyPropertyChanged("SelectedCategories");
            }
        }


        public Appointment(DataRow row, int intervallength)
        {

            m_dbappointment = GlobalSettings.Instance.RedDotData;

            m_intervallength = intervallength;
            InitData(row);

        }

        public Appointment(int id, int intervallength)
        {

            m_dbappointment = GlobalSettings.Instance.RedDotData;

            DataRow row = m_dbappointment.GetAppointment(id);
            m_intervallength = intervallength;
            InitData(row);
        }

        public Appointment(int intervallength)
        {
            m_dbappointment = GlobalSettings.Instance.RedDotData;

            m_intervallength = intervallength;
            m_id = 0;
           
            m_length = 1;
            m_currentcustomer = null;
            m_currentemployee = null;
            m_note = "";
            m_status = "";
    

            
        }

        private void InitData(DataRow m_row)
        {
            if (m_row["id"].ToString() != "0") m_id                     = (int)m_row["id"]; else m_id= 0;
      
            m_apptdate                                                  = (DateTime)m_row["apptdate"];
            m_status                                                    = m_row["status"].ToString();
            if (m_row["length"].ToString() != "") m_length              = int.Parse(m_row["length"].ToString());
            m_note                                                      = m_row["note"].ToString();
            if (m_row["employeeid"].ToString() != "") m_currentemployee = new Employee(int.Parse(m_row["employeeid"].ToString()));

            if (m_row["customerid"].ToString() != "" && m_row["customerid"].ToString() != "0") m_currentcustomer = new Customer(int.Parse(m_row["customerid"].ToString()), false);

            m_flexible = (m_row["flexible"].ToString() == "1");

            SelectedCategories = m_dbappointment.GetApptCategories(m_id);
        }

       public  void SaveData()
        {

            if (ID > 0)
            {
                if (m_dbappointment == null) return;

                m_dbappointment.UpdateNumeric(ID,"flexible", m_flexible?1:0);
   
                m_dbappointment.UpdateApptDate(m_apptdate, ID);
                m_dbappointment.UpdateString(ID, "status",m_status);
                m_dbappointment.UpdateNumeric(ID, "Length", m_length);
                m_dbappointment.UpdateString(ID, "note", m_note);
                m_dbappointment.UpdateEmployee(m_currentemployee.ID, ID);
                if (m_currentcustomer == null) m_dbappointment.UpdateCustomer(0, ID);
                else m_dbappointment.UpdateCustomer(m_currentcustomer.ID, ID);
            }
        }



        private int m_id = 0;
        public int ID
        {
            get{return m_id; }
        }

        private bool m_flexible = false;

        public bool Flexible
        {
            get { return m_flexible; }
            set
            {
                m_flexible = value;
                NotifyPropertyChanged("Flexible");
            }
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
                    if(CurrentEmployee != null)
                    {
                       // if (m_dbappointment == null) m_dbappointment = new MYSQLDBAppointment();

                        if (m_dbappointment.SpaceAvailable(ID, value, CurrentEmployee.ID, Length))
                        {
                            m_apptdate = value;
                        }
                        else
                        {
                            TouchMessageBox.Show("Time Slot not available!!");
                        }
                    }else
                    {
                        m_apptdate = value;
                    }


                NotifyPropertyChanged("ApptDate");
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
                if (CurrentEmployee != null)
                {
                   // if (m_dbappointment == null) m_dbappointment = new MYSQLDBAppointment();
                    if (m_dbappointment.CheckLength(ApptDate, CurrentEmployee.ID, value))
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


        public string ApptText
        {
            get
            {
                if (m_currentcustomer != null)
                {
                    string _text = "";
                    if (m_currentcustomer.FirstName != "" || m_currentcustomer.LastName != "") _text = m_currentcustomer.FirstName + " " + m_currentcustomer.LastName + (char)13+(char)10;
                    if(GlobalSettings.Instance.DisplayCustomerPhone)
                    {
                        if (m_currentcustomer.Phone1 != "") _text = _text + m_currentcustomer.Phone1 + (char)13 + (char)10;
                    }
                 

                    return  _text + m_note;
                }
                else return m_note;
                
            }
        }

      
        public string ApptColor
        {
            get
            {
                switch(m_status)
                {
                    case "Complete":
                        return "Gray";
                      
                     default:
                        return "AliceBlue";
                }
            }

          
        }


        public Employee CurrentEmployee
        {
            get
            {
                return m_currentemployee;
            }
            set
            {
               // if (m_dbappointment == null) m_dbappointment = new MYSQLDBAppointment();
                if (m_dbappointment.SpaceAvailable(ID, ApptDate, value.ID, Length))
                {
                    m_currentemployee = value;

                }
                else TouchMessageBox.Show("Space not available!");

                NotifyPropertyChanged("CurrentEmployee");

            }
        }

        public Customer CurrentCustomer
        {
            get
            {
                 return m_currentcustomer;
            }
            set
            {
                m_currentcustomer = value;



                NotifyPropertyChanged("CurrentCustomer");

            }
        }

    }

  


}
