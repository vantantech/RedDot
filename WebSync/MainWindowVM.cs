using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NLog;

namespace WebSync
{
    public class MainWindowVM:INPCBase
    {

        private History _history;
  

        private bool CanExecute = true;

        Window m_parent;
    

        DataTable _historydata;
    

       

        private DateTime _startdate;
        private DateTime _enddate;



        private static Logger logger = LogManager.GetCurrentClassLogger();
   
  
       

        public ICommand TodayClicked { get; set; }
        public ICommand PreviousClicked { get; set; }

        public ICommand NextClicked { get; set; }

        public ICommand Past7DaysClicked { get; set; }
        public ICommand CustomClicked { get; set; }

        public ICommand ByTicketIDClicked { get; set; }

        public ICommand SyncListClicked { get; set; }


        public ICommand SyncEmployeeClicked { get; set; }





        public MainWindowVM(Window parent)
        {

            _history            = new History();
        
            m_parent            = parent;

       

           
            
            TodayClicked        = new RelayCommand(ExecuteTodayClicked, param => this.CanExecute);
            PreviousClicked     = new RelayCommand(ExecutePreviousClicked, param => this.CanExecute);
            NextClicked         = new RelayCommand(ExecuteNextClicked, param => this.CanExecute);
            Past7DaysClicked    = new RelayCommand(ExecutePast7DaysClicked, param => this.CanExecute);
            CustomClicked       = new RelayCommand(ExecuteCustomClicked, param => this.CanExecute);

        

            SyncListClicked     = new RelayCommand(ExecuteSyncListClicked, param => this.CanExecute);
            SyncEmployeeClicked = new RelayCommand(ExecuteSyncEmployeeClicked, param => this.CanExecute);
          
         
          


        
            StartDate           = DateTime.Today;
            EndDate             = DateTime.Today;

            HistoryData         = _history.GetOrders(StartDate, EndDate);



            WebUserId = GlobalSettings.Instance.WebUserID;


        
        }



        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        ____                            _         
        // |  _ \ _   _| |__ | (_) ___  |  _ \ _ __ ___  _ __   ___ _ __| |_ _   _ 
        // | |_) | | | | '_ \| | |/ __| | |_) | '__/ _ \| '_ \ / _ \ '__| __| | | |
        // |  __/| |_| | |_) | | | (__  |  __/| | | (_) | |_) |  __/ |  | |_| |_| |
        // |_|    \__,_|_.__/|_|_|\___| |_|   |_|  \___/| .__/ \___|_|   \__|\__, |
        //                                              |_|                  |___/ 
        //------------------------------------------------------------------------------------------



        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                NotifyPropertyChanged("Message");
            }
        }

        public DateTime StartDate
        {
            get { return _startdate; }
            set { _startdate = value; NotifyPropertyChanged("StartDate"); }
        }

        public DateTime EndDate
        {
            get { return _enddate; }
            set { _enddate = value; NotifyPropertyChanged("EndDate"); }
        }

        public DataTable HistoryData
        {

            get
            {

                return _historydata;

            }

            set
            {
                _historydata = value;
                NotifyPropertyChanged("HistoryData");
            }

        }






        public int WebUserId { get; set; }



    



        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        __  __      _   _               _     
        // |  _ \ _   _| |__ | (_) ___  |  \/  | ___| |_| |__   ___   __| |___ 
        // | |_) | | | | '_ \| | |/ __| | |\/| |/ _ \ __| '_ \ / _ \ / _` / __|
        // |  __/| |_| | |_) | | | (__  | |  | |  __/ |_| | | | (_) | (_| \__ \
        // |_|    \__,_|_.__/|_|_|\___| |_|  |_|\___|\__|_| |_|\___/ \__,_|___/
        //    
        //------------------------------------------------------------------------------------------  

     
        public void LoadHistory()
        {
          
            HistoryData = _history.GetOrders(StartDate, EndDate);
            logger.Info("History Data Loaded.");
 
        }




 
        public void ExecuteTodayClicked(object salesid)
        {
          
            try
            {
                logger.Info("Today Clicked");
                StartDate = DateTime.Today;
                EndDate = DateTime.Today;
            
                LoadHistory();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteTodayClicked: " + e.Message);
                logger.Error("ExecuteTodayClicked: " + e.Message);
            }
        }


        public void ExecutePreviousClicked(object obj)
        {

            try
            {
                StartDate = StartDate.AddDays(-1);
                EndDate = StartDate;
           
                LoadHistory();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecutePreviousClicked: " + e.Message);
                logger.Error("ExecutePreviousClicked: " + e.Message);
            }
        }


        public void ExecuteNextClicked(object obj)
        {

            try
            {
                StartDate = StartDate.AddDays(1);
                EndDate = StartDate;
          
                LoadHistory();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteNextClicked: " + e.Message);
                logger.Error("ExecuteNextClicked: " + e.Message);
            }
        }

        public void ExecutePast7DaysClicked(object salesid)
        {

            try
            {

                StartDate = DateTime.Today.AddDays(-7);
                EndDate = DateTime.Today;
         
                LoadHistory();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecutePast7DaysClicked: " + e.Message);
                logger.Error("ExecutePast7DaysClicked: " + e.Message);
            }
        }


        public void ExecuteCustomClicked(object salesid)
        {

            try
            {
                logger.Info("Custom Clicked");
                CustomDate cd = new CustomDate(Visibility.Visible);
                cd.ShowDialog();
                //Utility.OpenModal(this, cd);

                StartDate = cd.StartDate;
                EndDate = cd.EndDate;
              

                LoadHistory();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteCustomClicked: " + e.Message);
                logger.Error("Custom Clicked: " + e.Message);
            }
        }


    

        public void ExecuteSyncListClicked(object salesid)
        {
            try
            {
           
                SalonWebClient m_webclient = new SalonWebClient();

                foreach(DataRow item in HistoryData.Rows)
                {
                    m_webclient.SyncTicket(WebUserId, int.Parse(item["id"].ToString()));
                }

                logger.Info("Sync List Completed Successfully.");
                
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteSyncListClicked: " + e.Message);
                logger.Error("Sync List Clicked: " + e.Message);
            }


        }

        public void ExecuteSyncEmployeeClicked(object obj)
        {
            try
            {
              
                SalonWebClient m_webclient = new SalonWebClient();
               
                    m_webclient.SyncEmployees(WebUserId);

                    logger.Info("Employee Sync Successfully.");
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteSyncEmployeeClicked: " + e.Message);
                logger.Error("Sync Employee Clicked: " + e.Message);
            }
        }


    }
}
