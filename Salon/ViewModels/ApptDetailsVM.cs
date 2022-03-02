using RedDotBase;
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
        public ICommand TicketClicked { get; set; }
        public ICommand CategoryClicked1 { get; set; }
        public ICommand CategoryClicked2 { get; set; }


        private Window m_parent;
        private SecurityModel m_security;
          private CustomerModel m_customermodel;
          private MenuSetupModel m_inventorymodel;
        private int m_selectedtimeindex;
        private int m_interval;
        private int m_daystarthour;
        public List<AppointmentTime> ApptLengths { get; set; }
        private DataTable m_categories;
        public DataTable Categories
        {
            get { return m_categories; }
            set
            {
                m_categories = value;
                NotifyPropertyChanged("Categories");
            }
        }

        private AppointmentModel m_appointmentmodel;

        public ApptDetailsVM(Window parent,Appointment appt, SecurityModel security)
        {
            m_appt = appt;
            m_parent = parent;
            m_security = security;
            EmployeeClicked = new RelayCommand(ExecuteEmployeeClicked, param => true);
            CustomerClicked = new RelayCommand(ExecuteCustomerClicked, param => true);
            DeleteClicked = new RelayCommand(ExecuteDeleteClicked, param => true);
            SaveClicked = new RelayCommand(ExecuteSaveClicked, param => true);
             NoteClicked = new RelayCommand(ExecuteNoteClicked, param => true);
            TicketClicked = new RelayCommand(ExecuteTicketClicked, param => true);
            CategoryClicked1 = new RelayCommand(ExecuteCategoryClicked1, param => true);


            CategoryClicked2 = new RelayCommand(ExecuteCategoryClicked2, param => true);

            m_customermodel = new CustomerModel();
             m_inventorymodel = new MenuSetupModel();
            m_appointmentmodel = new AppointmentModel(parent);

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
            Categories = m_inventorymodel.GetCategories(appt.ID);
            CurrentAppt.SelectedCategories = m_appointmentmodel.GetApptCategories(appt.ID);
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
                    TouchMessageBox.Show("Invalid Length!!");
                   
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


        public void ExecuteTicketClicked(object obj)
        {
            if(CurrentAppt.CurrentCustomer != null)
            {
               

                Employee CurrentEmployee = m_security.CurrentEmployee;
                Ticket CurrentTicket = new Ticket(CurrentEmployee);
                CurrentTicket.CreateTicket();
                CurrentTicket.UpdateCustomerID(CurrentAppt.CurrentCustomer.ID);

                if(Confirm.Ask("Mark Appointment Complete?"))
                {
                    CurrentAppt.Status = "Complete";
                    CurrentAppt.SaveData();
                }
            


                SalonSales sales = new SalonSales(CurrentTicket.SalesID);
                Utility.OpenModal(m_parent,sales);


                m_parent.Close();

            }
            else
            {
                TouchMessageBox.Show("Customer info is missing");
            }
      
        }





        public void ExecuteEmployeeClicked(object obj_id)
        {


            PickEmployee empl = new PickEmployee(m_parent,m_security);
            Utility.OpenModal(m_parent, empl);
            if (empl.EmployeeID > 0 )
            {
                CurrentAppt.CurrentEmployee = new Employee(empl.EmployeeID);
                if (CurrentAppt.CurrentEmployee.ID != empl.EmployeeID) TouchMessageBox.Show("Employee Already Has Appointment");
               NotifyPropertyChanged("CurrentAppt");
            }


        }



        public void ExecuteCustomerClicked(object obj_id)
        {

            //if ticket already has customer number, then bring up edit screen
            if (CurrentAppt.CurrentCustomer != null)
            {

                if (m_security.WindowNewAccess("CustomerView") == false)
                {
                 
                    return; //jump out of routine
                }

               CurrentAppt.CurrentCustomer = m_customermodel.EditViewCustomer(CurrentAppt.CurrentCustomer, m_security);

            }
            else
            {


                //if no customers then code continues below
                int custid = m_customermodel.GetCreateCustomer(m_security);
                if(custid > 0)  CurrentAppt.CurrentCustomer = new Customer(custid,false);

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

           if(CanSave) CurrentAppt.SaveData();
            m_parent.Close();
        }

        public void ExecuteNoteClicked(object obj_id)
        {

            TextPad tp = new TextPad("Appointment Notes:",CurrentAppt.Note);
            Utility.OpenModal(m_parent, tp);
            CurrentAppt.Note = tp.ReturnText;
            NotifyPropertyChanged("CurrentAppt");
        }

        public void ExecuteCategoryClicked1(object obj)
        {
            int catid = 0;
            if (obj.ToString() != "") catid = int.Parse(obj.ToString());

            m_appointmentmodel.RemoveApptCategory(catid);
            CurrentAppt.SelectedCategories = m_appointmentmodel.GetApptCategories(m_appt.ID);
            Categories = m_inventorymodel.GetCategories(m_appt.ID);
        }

        public void ExecuteCategoryClicked2(object obj)
        {
            int catid = 0;
            if (obj.ToString() != "") catid = int.Parse(obj.ToString());

            m_appointmentmodel.AddApptCategory(m_appt.ID, catid);
            CurrentAppt.SelectedCategories = m_appointmentmodel.GetApptCategories(m_appt.ID);
            Categories = m_inventorymodel.GetCategories(m_appt.ID);
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
