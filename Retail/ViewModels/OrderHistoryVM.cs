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
    public class OrderHistoryVM:INPCBase
    {

        private History _history;

        private bool CanExecute = true;

        Window _parent;
        Security _security;
        Employee _currentemployee;

        DataTable _historydata;
        DataTable _reversedtickets;
        DataTable _opentickets;

        DataTable _quotes;

        private DateTime _startdate;
        private DateTime _enddate;
        private int _subid;

        public string StorePrefix
        {
            get { return GlobalSettings.Instance.Shop.StorePrefix; }
        }

        public ICommand Refresh { get; set; }
  
        public ICommand OpenOrderClicked { get; set; }
 
   

        public ICommand ViewClicked { get; set; }
        public ICommand PrintOrderClicked { get; set; }

        public ICommand TodayClicked { get; set; }

        public ICommand PreviousClicked { get; set; }

        public ICommand NextClicked { get; set; }


      
        public ICommand Past7DaysClicked { get; set; }
        public ICommand CustomClicked { get; set; }

        public ICommand ByTicketIDClicked { get; set; }

        public ICommand ShipOrderClicked { get; set; }

        public OrderHistoryVM(Window parent, Security security)
        {

            _history         = new History();
            _security        = security;
            _parent          = parent;



            Refresh = new RelayCommand(ExecuteRefresh, param => this.CanExecute);
            OpenOrderClicked = new RelayCommand(ExecuteOpenOrderClicked, param => this.CanExecute);
            ViewClicked         = new RelayCommand(ExecuteViewClicked, param => this.CanExecute);
            TodayClicked        = new RelayCommand(ExecuteTodayClicked, param => this.CanExecute);

            PreviousClicked = new RelayCommand(ExecutePreviousClicked, param => this.CanExecute);
            NextClicked = new RelayCommand(ExecuteNextClicked, param => this.CanExecute);

            ShipOrderClicked = new RelayCommand(ExecuteShipOrderClicked, param => this.CanExecute);


            Past7DaysClicked    = new RelayCommand(ExecutePast7DaysClicked, param => this.CanExecute);
            CustomClicked       = new RelayCommand(ExecuteCustomClicked, param => this.CanExecute);

            ByTicketIDClicked = new RelayCommand(ExecuteByTicketIDClicked, param => this.CanExecute);

            _currentemployee = new Employee(0);
            StartDate        = DateTime.Today;
            EndDate          = DateTime.Today;

            HistoryData      = _history.GetOrders(StartDate, EndDate);
            ReversedTickets = _history.GetReversedTickets();
          
        

        }



        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        ____                            _         
        // |  _ \ _   _| |__ | (_) ___  |  _ \ _ __ ___  _ __   ___ _ __| |_ _   _ 
        // | |_) | | | | '_ \| | |/ __| | |_) | '__/ _ \| '_ \ / _ \ '__| __| | | |
        // |  __/| |_| | |_) | | | (__  |  __/| | | (_) | |_) |  __/ |  | |_| |_| |
        // |_|    \__,_|_.__/|_|_|\___| |_|   |_|  \___/| .__/ \___|_|   \__|\__, |
        //                                              |_|                  |___/ 
        //------------------------------------------------------------------------------------------
        public int SubID
        { get { return _subid; }
            set
            {
                _subid = value;
                NotifyPropertyChanged("SubID");
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
            get{  return _historydata;  }
            set
            {
                _historydata = value;
                NotifyPropertyChanged("HistoryData");
            }
        }

        public DataTable ReversedTickets
        {
            get { return _reversedtickets; }
            set
            {
                _reversedtickets = value;
                NotifyPropertyChanged("ReversedTickets");
            }
        }


        public DataTable OpenTickets
        {
            get { return _opentickets; }
            set
            {
                _opentickets = value;
                NotifyPropertyChanged("OpenTickets");
            }
        }
        public DataTable Quotes
        {
            get { return _quotes; }
            set
            {
                _quotes = value;
                NotifyPropertyChanged("Quotes");
            }
        }




        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        __  __      _   _               _     
        // |  _ \ _   _| |__ | (_) ___  |  \/  | ___| |_| |__   ___   __| |___ 
        // | |_) | | | | '_ \| | |/ __| | |\/| |/ _ \ __| '_ \ / _ \ / _` / __|
        // |  __/| |_| | |_) | | | (__  | |  | |  __/ |_| | | | (_) | (_| \__ \
        // |_|    \__,_|_.__/|_|_|\___| |_|  |_|\___|\__|_| |_|\___/ \__,_|___/
        //    
        //------------------------------------------------------------------------------------------  


        public void ExecuteShipOrderClicked(object salesid)
        {

            int id;

            if (salesid == null) return;

            if (salesid.ToString() != "") id = int.Parse(salesid.ToString());
            else id = 0;


            //check security for editing also

            if (_security.WindowAccess("ShippingOrder"))
            {


                AuditModel.WriteLog(_security.CurrentEmployee.FullName, "Ship Order", "Open/Edit", "Ship Order", id);
                Ticket CurrentTicket = new Ticket(id);

                ShipOrderEdit wk = new ShipOrderEdit(CurrentTicket);
                Utility.OpenModal(_parent, wk);


            }






        }
        public void ExecuteRefresh(object tickettype)
        {
            string _tickettype = (string) tickettype;

            if(_tickettype == "OpenTickets") OpenTickets = _history.GetOpenTickets();

            if(_tickettype == "Quotes") Quotes = _history.GetQuotes();

        }

        public void ExecuteOpenOrderClicked(object salesid)
        {
            int id;

            if (salesid == null) return;

            if (salesid.ToString() != "") id = int.Parse(salesid.ToString());
            else id = 0;


                    RetailSales qs = new RetailSales(_security, id);



                    if (!GlobalSettings.Instance.StayInHistory) _parent.Close();
                    Utility.OpenModal(_parent, qs);



            //Refresh history list
                    HistoryData = _history.GetOrders(StartDate, EndDate);
                    ReversedTickets = _history.GetReversedTickets();

        }


    

        public void ExecuteViewClicked(object salesid)
        {

            try
            {
                int id;

                if (salesid == null) return;

                if (salesid.ToString() != "") id = int.Parse(salesid.ToString());
                else id = 0;

                    RetailSalesView ord = new RetailSalesView(_security, id);
                    Utility.OpenModal(_parent, ord);


                    HistoryData = _history.GetOrders(StartDate, EndDate);
                    ReversedTickets = _history.GetReversedTickets();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteEditClicked: " + e.Message);
            }
        }
        public void ExecuteTodayClicked(object obj)
        {
          
            try
            {
                StartDate = DateTime.Today;
                EndDate = StartDate;

                HistoryData = _history.GetOrders(StartDate, EndDate);
                ReversedTickets = _history.GetReversedTickets();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }
        }


        public void ExecutePreviousClicked(object obj)
        {

            try
            {
                StartDate = StartDate.AddDays(-1);
                EndDate = StartDate;

                HistoryData = _history.GetOrders(StartDate, EndDate);
                ReversedTickets = _history.GetReversedTickets();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }
        }


        public void ExecuteNextClicked(object obj)
        {

            try
            {
                StartDate = StartDate.AddDays(1);
                EndDate = StartDate;

                HistoryData = _history.GetOrders(StartDate, EndDate);
                ReversedTickets = _history.GetReversedTickets();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }
        }

   

        public void ExecutePast7DaysClicked(object obj)
        {

            try
            {

                StartDate = DateTime.Today.AddDays(-7);
                EndDate = DateTime.Today;

                HistoryData = _history.GetOrders(StartDate, EndDate);
                ReversedTickets = _history.GetReversedTickets();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }
        }


        public void ExecuteCustomClicked(object obj)
        {

            try
            {
                CustomDate cd = new CustomDate(Visibility.Visible, DateTime.Now);
                Utility.OpenModal(_parent, cd);
                //Utility.OpenModal(this, cd);

                StartDate = cd.StartDate;
                EndDate = cd.EndDate;


                HistoryData = _history.GetOrders(StartDate, EndDate);
                ReversedTickets = _history.GetReversedTickets();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }
        }


        public void ExecuteByTicketIDClicked(object obj)
        {
            try
            {

                NumPad np = new NumPad("Enter Ticket #",true);
                Utility.OpenModal(_parent, np);
                int id = 0;
                if (np.Amount != "") id = int.Parse(np.Amount);

                HistoryData = _history.GetOrdersByID(id);
               

            }
            catch (Exception e)
            {
                MessageBox.Show("Execute By Ticket ID: " + e.Message);
            }


        }

    }
}
