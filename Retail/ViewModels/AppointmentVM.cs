using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class AppointmentVM:INPCBase
    {

        private Security m_security;
        private ObservableCollection<AppointmentGroup> m_appointmentlist;
        private ObservableCollection<TimeSlot> m_timeslots;
        private AppointmentModel m_appointmentmodel;
        private DateTime m_appointdate;


        public ICommand AppointmentClicked { get; set; }
        public ICommand NewAppointmentClicked { get; set; }

        public ICommand PreviousEmployeeClicked { get; set; }
        public ICommand NextEmployeeClicked { get; set; }

        public ICommand PreviousDayClicked { get; set; }
        public ICommand NextDayClicked { get; set; }
        public ICommand CustomDayClicked { get; set; }

        private bool CanExecute = true;
        private Window m_parent;

        private int m_interval;
        private int numofcolumn=9;
        private int m_columnwidth ;

 

        public AppointmentVM(Window parent, Security security)
        {
            AppointmentClicked = new RelayCommand(ExecuteAppointmentClicked, param => true);
            NewAppointmentClicked = new RelayCommand(ExecuteNewAppointmentClicked, param => true);
            PreviousDayClicked = new RelayCommand(ExecutePreviousDayClicked, param => this.CanExecute);
            NextDayClicked = new RelayCommand(ExecuteNextDayClicked, param => this.CanExecute);
            CustomDayClicked = new RelayCommand(ExecuteCustomDayClicked, param => this.CanExecute);

            PreviousEmployeeClicked = new RelayCommand(ExecutePreviousEmployeeClicked, param => this.CanPreviousEmployee);
            NextEmployeeClicked = new RelayCommand(ExecuteNextEmployeeClicked, param => this.CanNextEmployee);

            ColumnWidth = 850 / numofcolumn;
           m_interval = GlobalSettings.Instance.AppointmentInterval;

            m_parent = parent;
            m_security = security;
            m_appointmentmodel = new AppointmentModel(parent,security);
            m_appointdate = DateTime.Now;
           
            LoadTimeSlots();

            
            LoadAppointments(m_appointdate);
           
           
        }

        public void LoadAppointments(DateTime apptdate)
        {
          // var watch = Stopwatch.StartNew();
            AppointmentList = m_appointmentmodel.GetDailyAppointments(apptdate,numofcolumn);
           // watch.Stop();
           // var elapsedMs = watch.ElapsedMilliseconds;
           // MessageBox.Show(elapsedMs.ToString());
        }

        public void LoadTimeSlots()
        {
            ObservableCollection<TimeSlot> timelist = new ObservableCollection<TimeSlot>();
            DateTime currenttime = new DateTime(2016,01,01,GlobalSettings.Instance.DayStartHour,00,00);  //Jan 1, 2016 08:00
            TimeSlot timeslot;
            int daylength = GlobalSettings.Instance.DayLength;

            for(int i=0;i< daylength;i++)
            {
                timeslot = new TimeSlot(currenttime,m_interval);
                timelist.Add(timeslot);
                currenttime = currenttime.AddMinutes(60);
            }

            TimeSlots = timelist;
        }

        public int ColumnWidth
        {
            get { return m_columnwidth; }
            set { m_columnwidth = value;
            NotifyPropertyChanged("ColumnWidth");
            }
        }

        public DateTime AppointDate
        {
            get { return m_appointdate; }
            set { m_appointdate = value;
            NotifyPropertyChanged("AppointDate");
            }
        }

        public ObservableCollection<AppointmentGroup> AppointmentList
        {
            get { return m_appointmentlist; }
            set
            {
                m_appointmentlist = value;
                NotifyPropertyChanged("AppointmentList");
            }
        }


        public ObservableCollection<TimeSlot> TimeSlots
        {
            get { return m_timeslots; }
            set
            {
                m_timeslots = value;
                NotifyPropertyChanged("TimeSlots");
            }
        }


        public void ExecuteAppointmentClicked(object obj_id)
        {
            int apptid = 0;


            if (obj_id != null)
            {
                if (obj_id.ToString() != "") apptid = int.Parse(obj_id.ToString());

            }

            Appointment appt = new Appointment(apptid,m_interval);
            ApptDetails dlg = new ApptDetails(appt,m_security,false);
            Utility.OpenModal(m_parent, dlg);
  
            LoadAppointments(m_appointdate);

        }
        public void ExecuteNewAppointmentClicked(object obj_id)
        {
            int employeeid = 0;


            if (obj_id != null)
            {
                if (obj_id.ToString() != "") employeeid = int.Parse(obj_id.ToString());

            }

      

            //create a blank appointment object
            Appointment appt = new Appointment(m_interval);
            DateTime currenttime = new DateTime(m_appointdate.Year, m_appointdate.Month, m_appointdate.Day, 01, 00, 00);
            appt.ApptDate = currenttime;
            appt.Length = m_interval;
            appt.UpdateEmployee(employeeid);
            ApptDetails dlg = new ApptDetails(appt, m_security,true);
            Utility.OpenModal(m_parent, dlg);

            //check if user hit ok or cancel
            if(dlg.Action == "OK")
            {
              
                int apptid = m_appointmentmodel.CreateNewAppt(appt);

            }

            LoadAppointments(m_appointdate);

        }



/*
        public void ExecuteNewAppointmentClicked(object obj_id)
        {
            int employeeid = 0;


            if (obj_id != null)
            {
                if (obj_id.ToString() != "") employeeid = int.Parse(obj_id.ToString());

            }

    
            //Ask cashier to lookup or create new
            int customerid = 0;

 

                //ask for appt time
                DateTime currenttime = new DateTime(m_appointdate.Year, m_appointdate.Month, m_appointdate.Day, 08, 00, 00);
                ApptDateTime apt = new ApptDateTime(currenttime,m_security);
               // apt.CurrentCustomer = new Customer(customerid);
                apt.CurrentEmployee = new Employee(employeeid);
                apt.SelectedLength = 0;

                if (apt.CurrentCustomer != null) customerid = apt.CurrentCustomer.ID;


                Utility.OpenModal(m_parent, apt);

                if(apt.Action == "OK")
                {

                    //check if space fits
                    if (m_appointmentmodel.SpaceAvailable(0,apt.AppointmentDate, employeeid, apt.SelectedLength + 1))
                    {
                        //create appt

                        m_appointmentmodel.CreateNewAppt(apt.AppointmentDate, employeeid, customerid, apt.SelectedLength ,apt.Note,apt.CatID);
                        LoadAppointments(m_appointdate);
                    }
                    else
                    {
                        MessageBox.Show("Time Slot is not available");
                    }
                }


       

        }
*/
        public void ExecutePreviousDayClicked(object tagstr)
        {

            AppointDate = AppointDate.AddDays(-1);
            LoadAppointments(AppointDate);

        }
        public void ExecuteNextDayClicked(object tagstr)
        {
            AppointDate = AppointDate.AddDays(1);
            LoadAppointments(AppointDate);
        }

        public void ExecuteCustomDayClicked(object tagstr)
        {
            CustomDate cd = new CustomDate(Visibility.Hidden,DateTime.Now);
              Utility.OpenModal(m_parent,cd);
            if(AppointDate != cd.StartDate)
            {
                AppointDate = cd.StartDate;
                LoadAppointments(AppointDate);
            }

        }

        public bool CanPreviousEmployee
        {
            get { return (m_appointmentmodel.EmployeeSet > 1); }
        }
        public bool CanNextEmployee
        {
            get { return (m_appointmentmodel.EmployeeSet < m_appointmentmodel.EmployeeCount); }
        }
        public void ExecutePreviousEmployeeClicked(object tagstr)
        {

            if (m_appointmentmodel.EmployeeSet > 1) m_appointmentmodel.EmployeeSet--;
            LoadAppointments(AppointDate);

        }
        public void ExecuteNextEmployeeClicked(object tagstr)
        {


            if (m_appointmentmodel.EmployeeSet < m_appointmentmodel.EmployeeCount) m_appointmentmodel.EmployeeSet++;
            LoadAppointments(AppointDate);
        }








    }



}
