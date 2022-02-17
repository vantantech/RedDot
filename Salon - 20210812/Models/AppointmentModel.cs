using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using RedDot.DataManager;


namespace RedDot
{
    public class AppointmentModel
    {
        DBEmployee m_dbemployee;
        IDataInterface m_dbappointment;
        Window m_parent;
 
        public AppointmentModel(Window parent)
        {


            m_parent = parent;

            EmployeeSet = 1;

            m_dbemployee = new DBEmployee();
            m_dbappointment = GlobalSettings.Instance.RedDotData;
        }

        public bool UpdateEmployee(int employeeid, int appointmentid)
        {
           return m_dbappointment.UpdateEmployee(employeeid, appointmentid);
        }

        public bool UpdateCatID( int appointmentid, int catid)
        {
            return m_dbappointment.UpdateNumeric(appointmentid, "catid", catid);
        }

        public int EmployeeSet { get; set; }

        public int EmployeeCount { get; set; }

   

        public ObservableCollection<AppointmentGroup>  GetDailyAppointments(DateTime appointmentdate, int NumOfColumn, int width)
        {
            ObservableCollection<AppointmentGroup> apptgroupcollection;
            AppointmentGroup apptgroup;
            ObservableCollection<Appointment> appointments;
            Appointment appt;
            Appointment spacer;
            DataTable employeelist;
            DataTable appttable;
            DateTime starttime;
            DateTime previousendtime;
            TimeSpan datediff;
            int totalspacerminutes;

            int startcount;
            int endcount;

            

            int daystarthour = GlobalSettings.Instance.DayStartHour;


            if (NumOfColumn < 1) NumOfColumn = 1;

            int ColumnWidth = width / NumOfColumn -2;

            apptgroupcollection = new ObservableCollection<AppointmentGroup>();

            //Get tables of employees 
            // 1st call to database
            employeelist = m_dbemployee.GetApptEmployeeList();

            //Load all appointment for the day
            //second call to database 
            appttable = m_dbappointment.GetAppointments( appointmentdate);

          

            DataRow[] employee_array;
            DataRow[] appt_array;

            DataColumnCollection columns = employeelist.Columns;

            if (columns.Contains("sortorder")) employee_array = employeelist.Select("", "sortorder asc");
            else employee_array = employeelist.Select("", "displayname asc");
            
            EmployeeCount = employee_array.Length / NumOfColumn + (((employee_array.Length % NumOfColumn) > 0) ? 1 : 0);

            startcount = (EmployeeSet - 1) * NumOfColumn + 1;
            endcount = startcount + NumOfColumn - 1;
            int employeecounter = 0;
            int intervallength = GlobalSettings.Instance.AppointmentInterval;


            
            
            foreach(DataRow row in employee_array)  //loop thru list of employees
            {

                employeecounter++;
                if(employeecounter >= startcount && employeecounter <= endcount)
                {




                        previousendtime = new DateTime(appointmentdate.Year, appointmentdate.Month, appointmentdate.Day, daystarthour, 00, 00); //resets to ?:00 am each time
                        apptgroup = new AppointmentGroup();  //class to hold all appointment for this employee
                      apptgroup.CurrentEmployee = new EmployeeSimple(row);  //Load employee info to display on top of column
                        //appttable = m_dbappointment.GetAppointmentsByEmployee((int)row["id"], appointmentdate);

                        appointments = new ObservableCollection<Appointment>();
                 

                        //load appointments from above table with filter set to only current employee
                        
                       appt_array = appttable.Select("employeeid=" + apptgroup.CurrentEmployee.ID,"apptdate asc");

                        //appt_array = appttable.Select("employeeid=1", "apptdate asc");

                     
                        foreach (DataRow row2 in appt_array)
                        {

                            //var watch = Stopwatch.StartNew();
                                    appt = new Appointment(row2, intervallength);

                                    //watch.Stop();
                                    //var elapsedMs = watch.ElapsedMilliseconds;
                                   // TouchMessageBox.Show(elapsedMs.ToString());
                                  
                                    appt.Width = ColumnWidth-15;
                                    appt.Visible = Visibility.Visible;
  
                                    starttime = appt.ApptDate;
                                    datediff = starttime - previousendtime;
                                    totalspacerminutes = datediff.Minutes + datediff.Hours * 60;

                                    if (totalspacerminutes < 0) continue;//time starts before start time so is invalid

                                    if(totalspacerminutes> 0) //add a blank spacer 
                                    {
                                            spacer = new Appointment( intervallength);
                                            spacer.Length = totalspacerminutes;
                                                spacer.Visible = Visibility.Hidden;
                                            appointments.Add(spacer);
                                    }

                                    previousendtime = starttime.AddMinutes(appt.Length) ;
                                    appointments.Add(appt);

                                    
                        }

                        apptgroup.Appointments = appointments;
                        apptgroupcollection.Add(apptgroup);

                       


                        
                }//employee sets



            }

            


            return apptgroupcollection;
        }



        public static void DeleteAppt(int id)
        {
          
            GlobalSettings.Instance.RedDotData.DeleteAppt(id);

        }

        public  bool SpaceAvailable(int currentapptid,DateTime apptdate, int employeeid, int length)
        {
            
            return m_dbappointment.SpaceAvailable(currentapptid,apptdate, employeeid, length);
        }

        public int CreateNewAppt(Appointment appt)
        {
            
            int customerid = 0;
            if (appt.CurrentCustomer != null) customerid = appt.CurrentCustomer.ID;
            return m_dbappointment.CreateNewAppointment(appt.ApptDate, appt.CurrentEmployee.ID, customerid, appt.Length, appt.Note);

        }


        public DataTable GetApptCategories(int apptid)
        {
            return m_dbappointment.GetApptCategories(apptid);
        }


        public void AddApptCategory(int apptid, int catid)
        {
             m_dbappointment.AddApptCategory(apptid, catid);
        }
        public void RemoveApptCategory( int catid)
        {
            m_dbappointment.RemoveApptCategory( catid);
        }
    }


    public class AppointmentGroup : INPCBase
    {


        public EmployeeSimple CurrentEmployee { get; set; }
        public ObservableCollection<Appointment> Appointments { get; set; }
    }
}
