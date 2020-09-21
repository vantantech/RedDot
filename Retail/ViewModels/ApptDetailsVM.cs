using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class ApptDetailsVM:INPCBase
    {

        private Appointment m_appt;
        public ICommand EmployeeClicked { get; set; }
        public ICommand CustomerClicked { get; set; }
        public ICommand SaveClicked { get; set; }
        public ICommand DeleteClicked { get; set; }
        public ICommand NoteClicked { get; set; }

        private Window m_parent;
        private Security m_security;
        private CustomerModel m_customermodel;
        private InventoryModel m_inventorymodel;
        private int m_selectedtimeindex;
        private int m_interval;
        private int m_daystarthour;
        public List<AppointmentTime> ApptLengths { get; set; }
       // public DataTable Categories { get; set; }
        
        public ApptDetailsVM(Window parent,Appointment appt, Security security)
        {
            m_appt = appt;
            m_parent = parent;
            m_security = security;
            EmployeeClicked = new RelayCommand(ExecuteEmployeeClicked, param => true);
            CustomerClicked = new RelayCommand(ExecuteCustomerClicked, param => true);
            DeleteClicked = new RelayCommand(ExecuteDeleteClicked, param => true);
            SaveClicked = new RelayCommand(ExecuteSaveClicked, param => this.CanSave);
             NoteClicked = new RelayCommand(ExecuteNoteClicked, param => true);
             m_customermodel = new CustomerModel(m_parent);
             m_inventorymodel = new InventoryModel();

            ApptLengths = new List<AppointmentTime>();
            m_interval = GlobalSettings.Instance.AppointmentInterval;
            int numinterval = GlobalSettings.Instance.DayLength * 60 / m_interval;
            m_daystarthour = GlobalSettings.Instance.DayStartHour;
     
            AppointmentTime time;
            for(int i=1; i <= numinterval;i++)
            {
                time = new AppointmentTime();
                time.Length = i * m_interval;
                ApptLengths.Add(time);
            }
            //Categories = m_inventorymodel.GetCategoryList("Cat1",0);

        }


        public Appointment CurrentAppt
        {
            get { return m_appt; }
            set { m_appt = value;
            NotifyPropertyChanged("CurrentAppt");
            }
        }

        public DateTime AppointmentDate
        {
            get { return CurrentAppt.ApptDate; }
            set
            {
                DateTime newtime= new DateTime(value.Year, value.Month,value.Day, m_appt.ApptDate.Hour, m_appt.ApptDate.Minute, 00);

                m_appt.ApptDate = newtime;

                NotifyPropertyChanged("AppointmentDate");
                NotifyPropertyChanged("CurrentAppt");

            }
        }

        public int SelectedTimeIndex
        {
            get {

                DateTime starttime = new DateTime(m_appt.ApptDate.Year, m_appt.ApptDate.Month, m_appt.ApptDate.Day, GlobalSettings.Instance.DayStartHour, 00, 00);
                        TimeSpan diff = CurrentAppt.ApptDate - starttime;
                        m_selectedtimeindex =  (int)diff.TotalMinutes / m_interval;
                        return m_selectedtimeindex;
         }


            set
            {
                
                DateTime currenttime = new DateTime(m_appt.ApptDate.Year, m_appt.ApptDate.Month, m_appt.ApptDate.Day, GlobalSettings.Instance.DayStartHour, 00, 00);
                DateTime newtime = currenttime.AddMinutes(m_interval * value);
                m_appt.ApptDate = newtime;

                   NotifyPropertyChanged("SelectedTimeIndex");
                NotifyPropertyChanged("CurrentAppt");
            }
        }
        public int SelectedLength
        {
            get { return CurrentAppt.Length ; }
            set {
                int oldlength = CurrentAppt.Length;
                int newLength = value;
                CurrentAppt.Length = newLength;
                if (CurrentAppt.Length != newLength)
                {
                    MessageBox.Show("Invalid Length!!");
                   
                }


                NotifyPropertyChanged("SelectedLength");
                NotifyPropertyChanged("CurrentAppt");
            } 
        }

        public int SelectedStatus
        {
            get {
                
                switch( CurrentAppt.Status)
                {
                    case "Active":
                        return 0;
                
                    case "Confirmed":
                        return 1;
                   
                    case "Cancelled":
                        return 2;
                   
                    case "Complete":
                        return 3;
                  
                    case "No Show":
                        return 4;
                     
                    default:
                        return 0;

                }
            
            
            
            }
            set
            {
                switch (value)
                {
                    case 0:
                        CurrentAppt.Status = "Active";
                        break;
                    case 1:
                    CurrentAppt.Status = "Confirmed";
 
                        break;
                    case 2:
                    CurrentAppt.Status = "Cancelled";
    
                        break;
                    case 3:
                    CurrentAppt.Status = "Complete";
    
                        break;
                    case 4:
                        CurrentAppt.Status = "No Show";
            
                        break;
  
                }
                NotifyPropertyChanged("SelectedStatus");
                NotifyPropertyChanged("CurrentAppt");
            }
        }



        public bool CanSave
        {
            get
            {
                if (CurrentAppt.ApptDate.Hour < m_daystarthour) return false;
                if (CurrentAppt.Length < m_interval) return false;

                return true;
            }
        }



        public void ExecuteEmployeeClicked(object obj_id)
        {


            EmployeeList empl = new EmployeeList(null,m_security);
            Utility.OpenModal(m_parent, empl);
            if (empl.EmployeeID > 0 )
            {
                CurrentAppt.UpdateEmployee(empl.EmployeeID);
         
                if (CurrentAppt.EmployeeID != empl.EmployeeID) MessageBox.Show("Space not available!!");
               NotifyPropertyChanged("CurrentAppt");
            }


        }



        public void ExecuteCustomerClicked(object obj_id)
        {

            //if ticket already has customer number, then bring up edit screen
            if (CurrentAppt.CustomerID != 0)
            {

                if (m_security.WindowAccess("CustomerView") == false)
                {
                    MessageBox.Show("Access Denied");
                    return; //jump out of routine
                }

               CurrentAppt.CustomerID= m_customermodel.EditViewCustomer(CurrentAppt.CustomerID, m_security);

            }
            else
            {


                //if no customers then code continues below
                int custid = m_customermodel.GetCreateCustomer();
                if(custid > 0)  CurrentAppt.CustomerID = custid;

            }



        }

        public void ExecuteDeleteClicked(object obj_id)
        {
            Confirm conf = new Confirm("Delete Appointment?");
            Utility.OpenModal(m_parent, conf);

            if(conf.Action == "Yes")  AppointmentModel.DeleteAppt(CurrentAppt.ID);
            m_parent.Close();
        }

        public void ExecuteSaveClicked(object obj_id)
        {

            CurrentAppt.SaveData();
            m_parent.Close();
        }

        public void ExecuteNoteClicked(object obj_id)
        {

            TextPad tp = new TextPad("Appointment Notes:",CurrentAppt.Note);
            Utility.OpenModal(m_parent, tp);
            CurrentAppt.Note = tp.ReturnText;
            NotifyPropertyChanged("CurrentAppt");
        }
    }


    public class AppointmentTime
    {
        public string Description {
            get
            {
                int timehour;

               timehour = Length / 60;

               return timehour.ToString() + " hr " + (Length - (timehour * 60)).ToString() + " min";
               
            }
            
            
            }
        public int Length { get; set; }
    }


}
