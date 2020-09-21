using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Windows;
using System.Data;
using System.Windows.Input;
using RedDot.DataManager;

namespace RedDot
{
   public class TurnsVM:INPCBase
    {



        public ICommand AdminCheckInClicked { get; set; }
        public ICommand MoveTopClicked { get; set; }
        public ICommand MoveUpClicked { get; set; }
        public ICommand MoveDownClicked { get; set; }
        public ICommand MoveBottomClicked { get; set; }
        public ICommand DeleteClicked { get; set; }

        public ICommand TurnClicked { get; set; }


        private Window     m_parent;
        private SecurityModel   m_security;
        private DBEmployee m_dbemployee;
        private DataTable  m_employeelist;
        private DataTable m_availablelist;
        private int        m_employeeid=0;

       public TurnsVM(Window parent)
       {

           AdminCheckInClicked = new RelayCommand(ExecuteAdminCheckInClicked, param => true);
           MoveTopClicked      = new RelayCommand(ExecuteMoveTopClicked, param => this.IsButtonSelected);
           MoveUpClicked       = new RelayCommand(ExecuteMoveUpClicked, param => this.IsButtonSelected);
           MoveDownClicked     = new RelayCommand(ExecuteMoveDownClicked, param => this.IsButtonSelected);
           MoveBottomClicked   = new RelayCommand(ExecuteMoveBottomClicked, param => this.IsButtonSelected);
           DeleteClicked       = new RelayCommand(ExecuteDeleteClicked, param => this.IsButtonSelected);

           TurnClicked = new RelayCommand(ExecuteTurnClicked, param => true);


           m_parent            = parent;

           m_dbemployee        = new DBEmployee();


           m_security          = new SecurityModel();


           LoadList(0);

           

       }

       private void LoadList(int selectedemployeeid)
       {
           m_employeeid = 0;
           EmployeeList = m_dbemployee.GetCheckInList(DateTime.Now);
           AvailableList = m_dbemployee.GetCheckOutList(DateTime.Now);

           foreach(DataRow row in EmployeeList.Rows)
           {
               if ((int)row["employeeid"] == selectedemployeeid)
               {
                   row["selected"] = true;
                   m_employeeid = selectedemployeeid;
                   break;
               }
           }
           
       }


       public DataTable EmployeeList
       {
           get { return m_employeelist; }
           set
           {
               m_employeelist = value;
               NotifyPropertyChanged("EmployeeList");
           }
       }

       public DataTable AvailableList
       {
           get { return m_availablelist; }
           set
           {
               m_availablelist = value;
               NotifyPropertyChanged("AvailableList");
           }
       }
       //--------------------------------------------------------------  Button Enable ----------------------------------------------

       public bool IsButtonSelected
       {

           get
           {
               if (m_employeeid == 0) return false;
               else return true;
           }
       }

        //---------------------------------------------------------------------- Button Commands ------------------------------------------------
       public void ExecuteAdminCheckInClicked(object empid)
       {
           try
           {
               //EmployeeList empl = new EmployeeList(m_parent,m_security);
               //empl.ShowDialog();
               int employeeid = (int)empid;

               DataTable dt = m_dbemployee.GetCheckIn(employeeid, DateTime.Now);
               if (dt.Rows.Count > 0)
               {
                   TouchMessageBox.Show("Already check in!!");
               }
               else
               {
                   m_dbemployee.InsertCheckIn(employeeid, DateTime.Now);
                   LoadList(employeeid);
               }
           }
           catch (Exception e)
           {
               TouchMessageBox.Show("Check in tech:" + e.Message);

           }

       }
       public void ExecuteMoveTopClicked(object button)
       {
           try
           {

               m_dbemployee.MoveCheckInToTop(m_employeeid, DateTime.Now);
               LoadList(m_employeeid);

           }
           catch (Exception e)
           {
               TouchMessageBox.Show("Turns Move Top:" + e.Message);

           }

       }
       public void ExecuteMoveUpClicked(object button)
       {
           try
           {
               int newspotid = -9999;
               int newturn = 0;
               int employeeturn = 0;
               foreach(DataRow row in EmployeeList.Rows)
               {

                   if ((int)row["employeeid"] == m_employeeid)
                   {
                       employeeturn = (int)row["turn"];
                       break;
                   }
                   newspotid = (int)row["employeeid"];
                   newturn = (int)row["turn"];
               }

               //person is already on top if newspot is not found
               if (newspotid == -9999) return;



               //switch turn spot with the person above
               m_dbemployee.UpdateCheckIn(m_employeeid, DateTime.Now, newturn);
               m_dbemployee.UpdateCheckIn(newspotid, DateTime.Now, employeeturn);
               LoadList(m_employeeid);

           }
           catch (Exception e)
           {
               TouchMessageBox.Show("Turns Move Up:" + e.Message);

           }

       }

       public void ExecuteMoveDownClicked(object button)
       {
           try
           {

               int newspotid = -9999;
               int newturn = 0;
               int employeeturn = 0;
               DataTable ReversedList = EmployeeList.AsEnumerable().Reverse().CopyToDataTable();  //reverse data table so now we are parsing from bottom up
               foreach (DataRow row in ReversedList.Rows)
               {

                   if ((int)row["employeeid"] == m_employeeid)
                   {
                       employeeturn = (int)row["turn"];
                       break;
                   }
                   newspotid = (int)row["employeeid"];
                   newturn = (int)row["turn"];
               }

               //person is already on top if newspot is not found
               if (newspotid == -9999) return;



               //switch turn spot with the person above
               m_dbemployee.UpdateCheckIn(m_employeeid, DateTime.Now, newturn);
               m_dbemployee.UpdateCheckIn(newspotid, DateTime.Now, employeeturn);
               LoadList(m_employeeid);

           }
           catch (Exception e)
           {
               TouchMessageBox.Show("Turns Move Down:" + e.Message);

           }

       }
       public void ExecuteMoveBottomClicked(object button)
       {
           try
           {
               m_dbemployee.MoveCheckInToBottom(m_employeeid, DateTime.Now);
               LoadList(m_employeeid);

           }
           catch (Exception e)
           {
               TouchMessageBox.Show("Turns Move Bottom:" + e.Message);

           }

       }
       public void ExecuteDeleteClicked(object button)
       {
           try
           {

               m_dbemployee.DeleteCheckIn(m_employeeid, DateTime.Now);
               LoadList(0);

           }
           catch (Exception e)
           {
               TouchMessageBox.Show("Delete Turns:" + e.Message);

           }

       }


       public void ExecuteTurnClicked(object objturnid)
       {
           try
           {
               
               if (objturnid != null) m_employeeid = (int)objturnid;


           }
           catch (Exception e)
           {
               TouchMessageBox.Show("Turns Selected:" + e.Message);

           }

       }
    }
}
