using Microsoft.Win32;
using RedDot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace RedDot
{
    public class EmployeeViewModel : EmployeeListViewModel
    {

        private Employee _employee;
        private int _currentemployeeid;
        public ICommand DeleteEmployeeClicked { get; set; }
        public ICommand PictureClicked { get; set; }
   
        public ICommand MSRSetClicked { get; set; }
        public ICommand PreviousPeriodClicked { get; set; }
        public ICommand NextPeriodClicked { get; set; }
        public ICommand PrintHoursClicked { get; set; }
        public ICommand ExportHoursClicked { get; set; }
        public ICommand EditHoursClicked { get; set; }
        public ICommand DeleteHoursClicked { get; set; }
        public ICommand AddHoursClicked { get; set; }

        public ICommand EmployeeClicked { get; set; }

        public ICommand EnrollFingerPrintClicked { get; set; }

        private bool CanExecute = true;
        private bool CanDelete = false;
        private Window _parent;

        private DateTime _startdate;
        private DateTime _enddate;
        private int m_selectedindex;

        private List<ReportDate> m_reportdates;
        private ReportDate m_currentdate;
        private int m_selecteddateid;

        public Visibility VisibleAdmin { get; set; }
        public ReportDate CurrentDate
        {
            get { return m_currentdate; }
            set
            {
                m_currentdate = value;
                NotifyPropertyChanged("CurrentDate");
            }
        }
        public List<ReportDate> ReportDates
        {
            get { return m_reportdates; }

            set
            {
                m_reportdates = value;
                NotifyPropertyChanged("ReportDates");
            }
        }

        private List<CustomList> newlist;
        public List<CustomList> Roles
        {
            get { return newlist; }
            set
            {
                newlist = value;
                NotifyPropertyChanged("Roles");
            }
        }
        public bool IsAdmin { get; set; }

        private Security _security;

        public EmployeeViewModel(Window parent, Security security, Employee employee, bool candelete, bool admin)
        {
            _employee = employee;
            _parent = parent;
            _security = security;
            CanDelete = candelete;


            EmployeeClicked = new RelayCommand(ExecuteEmployeeClicked, param => this.CanExecute);
            DeleteEmployeeClicked = new RelayCommand(ExecuteDeleteEmployeeClicked, param => this.CanDeleteEmployee);
            PictureClicked = new RelayCommand(ExecutePictureClicked, param => this.CanExecute);
         
            MSRSetClicked = new RelayCommand(ExecuteMSRSetClicked, param => this.CanExecute);
            PreviousPeriodClicked = new RelayCommand(ExecutePreviousPeriodClicked, param => this.CanExecute);
            NextPeriodClicked = new RelayCommand(ExecuteNextPeriodClicked, param => this.CanExecute);
            PrintHoursClicked = new RelayCommand(ExecutePrintHoursClicked, param => this.CanExecute);
            ExportHoursClicked = new RelayCommand(ExecuteExportHoursClicked, param => this.CanExecute);
            EditHoursClicked = new RelayCommand(ExecuteEditHoursClicked, param => IsAdmin);
            DeleteHoursClicked = new RelayCommand(ExecuteDeleteHoursClicked, param => IsAdmin);
            AddHoursClicked = new RelayCommand(ExecuteAddHoursClicked, param => IsAdmin);
            EnrollFingerPrintClicked = new RelayCommand(ExecuteEnrollFingerPrintClicked, param => this.CanExecute);


            DateTime startdate = DateTime.Today;


            ReportDates = new List<ReportDate>();
            m_currentdate = new ReportDate();


            List<CustomList> newlist = new List<CustomList>();
            var listarray = GlobalSettings.Instance.EmployeeRoles.Split(',');
            foreach (var item in listarray)
            {
                newlist.Add(new CustomList { returnstring = item, description = item });
            }

            Roles = newlist;

            FillDateList();
            SelectedDateID = 1;  //this will set selected date to today

            if (admin) VisibleAdmin = Visibility.Visible;
            else VisibleAdmin = Visibility.Collapsed;

            IsAdmin = admin;

            if(CurrentEmployee != null)
                CurrentEmployee.GetLastTimeRecord();


        }

        public bool CanDeleteEmployee
        {
            get
            {
                if (_employee == null) return false;

                if (this.CanDelete)
                {

                   
                    if (_employee.HasTimeRecord) return false;
                    else
                    {
                        if (SalesModel.GetSalesCount(_employee.ID) > 0)   return false;
                        else return true;
                    }
                       

                }
                else return false;




            }

        }

        private void FillDateList()
        {
            DateTime startdate;
            ReportDate newdate;
            startdate = DateTime.Today;
            int j = 1;


            int diff = (int)DateTime.Now.DayOfWeek - GlobalSettings.Instance.PayPeriodStartDay;

            switch (GlobalSettings.Instance.PayPeriodType.ToUpper())
            {

                case "SEMIMONTHLY":

                    for (int i = 1; i <= 36; i++)
                    {


                        newdate = new ReportDate();
                        newdate.ReportDateID = j;
                        newdate.StartDate = new DateTime(startdate.Year, startdate.Month, 16);
                        newdate.EndDate = newdate.StartDate.AddMonths(1).AddDays(-16);
                        ReportDates.Add(newdate);
                        j++;


                        newdate = new ReportDate();
                        newdate.ReportDateID = j;
                        newdate.StartDate = new DateTime(startdate.Year, startdate.Month, 1);
                        newdate.EndDate = new DateTime(startdate.Year, startdate.Month, 15);
                        ReportDates.Add(newdate);
                        j++;


                        startdate = startdate.AddMonths(-1);
                    }

                    break;

                case "WEEKLY":

                    //difference between today and the first day of pay week
                    if (diff >= 0)
                    {
                        //positive or equal ..that means the start
                        startdate = DateTime.Now.AddDays(-diff);

                    }
                    else
                    {
                        startdate = DateTime.Now.AddDays((diff * -1) - 7);
                    }

                    for (int i = 1; i <= 36; i++)
                    {


                        newdate = new ReportDate();
                        newdate.ReportDateID = j;
                        newdate.StartDate = startdate;
                        newdate.EndDate = startdate.AddDays(6);
                        ReportDates.Add(newdate);
                        j++;

                        startdate = startdate.AddDays(-7);
                    }


                    break;

                case "BIWEEKLY":


                    startdate = GlobalSettings.Instance.PayPeriodStartDate;
                    TimeSpan datediff = DateTime.Now.Subtract(startdate);
                    int totalcount = (int)datediff.TotalDays / 14;
                    startdate = startdate.AddDays(totalcount * 14);

                    for (int i = 1; i <= 36; i++)
                    {


                        newdate = new ReportDate();
                        newdate.ReportDateID = j;
                        newdate.StartDate = startdate;
                        newdate.EndDate = startdate.AddDays(13);
                        ReportDates.Add(newdate);
                        j++;

                        startdate = startdate.AddDays(-14);
                    }
                    break;


            }


        }


        public int SelectedDateID
        {
            get { return m_selecteddateid; }
            set
            {
                m_selecteddateid = value;
                foreach (ReportDate rptdate in ReportDates)
                {
                    if (rptdate.ReportDateID == m_selecteddateid)
                    {
                        CurrentDate = rptdate;
                        if(CurrentEmployee != null)  CurrentEmployee.LoadHours(CurrentDate.StartDate, CurrentDate.EndDate);
                    }
                }


                NotifyPropertyChanged("SelectedDateID");
            }
        }


        public Employee CurrentEmployee
        {
            get { return _employee; }
            set
            {
                _employee = value;
                NotifyPropertyChanged("CurrentEmployee");
            }
        }
        public DateTime StartDate
        {
            get { return _startdate; }
            set
            {
                _startdate = value;
                NotifyPropertyChanged("StartDate");
            }
        }

        public DateTime EndDate
        {
            get { return _enddate; }
            set
            {
                _enddate = value;
                NotifyPropertyChanged("EndDate");
            }
        }

        public void ExecutePreviousPeriodClicked(object button)
        {
            StartDate = StartDate.AddDays(-7);
            EndDate = StartDate.AddDays(6);
            CurrentEmployee.LoadHours(StartDate, EndDate);
        }

        public void ExecuteNextPeriodClicked(object button)
        {
            StartDate = StartDate.AddDays(7);
            EndDate = StartDate.AddDays(6);
            CurrentEmployee.LoadHours(StartDate, EndDate);
        }


        public void ExecuteEmployeeClicked(object employeeid)
        {
            if (employeeid != null)
            {


                _currentemployeeid = int.Parse(employeeid.ToString());
                CurrentEmployee = new Employee(_currentemployeeid);
                if (CurrentEmployee != null) CurrentEmployee.LoadHours(CurrentDate.StartDate, CurrentDate.EndDate);
            }
        }

        public void ExecutePrintHoursClicked(object button)
        {
            try
            {
                PrintModel.PrintEmployeeHours(CurrentEmployee);
            }
            catch (Exception e)
            {
                MessageBox.Show("Print Hours:" + e.Message);
            }

        }

        public void ExecuteExportHoursClicked(object button)
        {
            try
            {
                SaveFileDialog ofd = new SaveFileDialog();
                ofd.DefaultExt = "csv";
                ofd.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                var result = ofd.ShowDialog();

                if (ofd.FileName != "") CSVWriter.WriteDataTable(CurrentEmployee.Hours, ofd.FileName, true);
            }catch(Exception e)
            {
                MessageBox.Show("Hours Export:" + e.Message);
            }
   



        }


        public void ExecuteAddHoursClicked(object employeeid)
        {
            try
            {
                int id;
                if (employeeid == null) return;

                if (employeeid.ToString() != "") id = int.Parse(employeeid.ToString());
                else id = 0;

                var dbemployee = new DBEmployee();
                dbemployee.InsertTimeRecord(id, CurrentDate.StartDate.ToString());


                CurrentEmployee.LoadHours(CurrentDate.StartDate, CurrentDate.EndDate);

               
            }
            catch (Exception e)
            {
                MessageBox.Show("Delete Time Record:" + e.Message);
            }

        }

        public void ExecuteEnrollFingerPrintClicked(object obj)
        {

            if (_security.WindowAccess("BackOffice"))
            {
                EnrollFingerPrint ep = new EnrollFingerPrint(CurrentEmployee);
                Utility.OpenModal(_parent, ep);

                //Loads finger print database into memory
                GlobalSettings.Instance.LoadAllFmdsUserIDs();
            }


        }


        public void ExecuteDeleteHoursClicked(object timeid)
        {
            try
            {
                int id;
                if (timeid == null) return;

                if (timeid.ToString() != "") id = int.Parse(timeid.ToString());
                else id = 0;

                var dbemployee = new DBEmployee();
                Confirm popup = new Confirm("Delete Time Record?");
                Utility.OpenModal(_parent, popup);


                if(popup.Action == "Yes")
                {
                    dbemployee.DeleteTimeRecord(id);
                    CurrentEmployee.LoadHours(CurrentDate.StartDate, CurrentDate.EndDate);
                   
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Delete Time Record:" + e.Message);
            }

        }
        public void ExecuteEditHoursClicked(object timeid)
        {
            try
            {
                int id;
                if (timeid == null) return;

                if (timeid.ToString() != "") id = int.Parse(timeid.ToString());
                else id = 0;

                var dbemployee = new DBEmployee();
                var timedata = dbemployee.GetTimeRecord(id);
                if(timedata != null)
                {
                    //open edit pop up
                    EditHours popup = new EditHours(timedata["Timein"].ToString(),timedata["timeout"].ToString(), timedata["note"].ToString());
                    Utility.OpenModal(_parent, popup);

                    // save hours
                    if(popup.Action == "Save")
                    {
                        dbemployee.SaveTimeRecord(id,popup.InTime, popup.OutTime, popup.Note);
                    }

                }


                CurrentEmployee.LoadHours(CurrentDate.StartDate, CurrentDate.EndDate);
            }
            catch (Exception e)
            {
                MessageBox.Show("Edit Time Record:" + e.Message);
            }




        }
        public void ExecuteMSRSetClicked(object button)
        {
            CardScanner crd = new CardScanner();
            Utility.OpenModal(_parent, crd);
            if (crd.CardNumber != "")
            {
                CurrentEmployee.MSRCard = crd.CardNumber;

            }

        }
        public void ExecutePictureClicked(object button)
        {
            OpenFileDialog picfile = new OpenFileDialog();
            picfile.DefaultExt = "jpg";
            picfile.Filter = "PNG Files (*.png)|*.png|JPG files (*.jpg)|*.jpg|BMP files (*.bmp)|*.bmp|All files (*.*)|*.*";
            string AppPath;
            string selectedPath;

            AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            AppPath = AppPath.ToUpper();

            if (picfile.ShowDialog() == true)
            {
                selectedPath = picfile.FileName;
                selectedPath = selectedPath.ToUpper();
                selectedPath = selectedPath.Replace(AppPath, "");

                CurrentEmployee.ImageSrc = selectedPath.Replace("\\", "\\\\");
            }
            // Utility.PlaySound();

        }
        public void ExecuteDeleteEmployeeClicked(object button)
        {
            Confirm con = new Confirm("Delete Employee from System ???!!!!!!!");
            con.ShowDialog();

            if(con.Action.ToUpper() == "YES")
                if (_employee.DeleteEmployee())
                {
                    MessageBox.Show("Deleted Successfully");

                    CurrentEmployee = null;
                    _parent.Close();
                }
                else MessageBox.Show("Delete Fail");


        }

   

        public int SelectedIndex
        {
            get { return m_selectedindex; }
            set
            {
                m_selectedindex = value;
                switch (m_selectedindex)
                {
                    case 0: //daily
                        break;
                    case 1: //weekly
                        CurrentEmployee.LoadHours(_startdate, _enddate);


                        break;

                }
                NotifyPropertyChanged("SelectedIndex");
            }
        }

    }
}
