using Microsoft.Win32;
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
    public class CashierInOutVM : INPCBase
    {

        private Employee _employee;

        private DBSales _dbsales;
      
        public ICommand BackClicked { get; set; }

        public ICommand CashierInClicked { get; set; }
        public ICommand CashierOutClicked { get; set; }

        private Window _parent;
        decimal difference = 0;
        private bool _editmode = false;
        public int StationNo { get { return GlobalSettings.Instance.StationNo; } }

        private CashDrawer m_currentdrawer;
        public CashDrawer CurrentDrawer
        {
            get { return m_currentdrawer; }
            set
            {
                m_currentdrawer = value;
                NotifyPropertyChanged("CurrentDrawer");
            }
        }

        public CashierInOutVM(Window parent, SecurityModel security, Employee employee, bool EditMode=false) 
        {
            _employee = employee;
            _parent = parent;
            _dbsales = new DBSales();
            _editmode = EditMode;

            int stationno = GlobalSettings.Instance.StationNo;

             GetDrawer(EditMode);



            BackClicked = new RelayCommand(ExecuteBackClicked, param => true);

            CashierInClicked = new RelayCommand(ExecuteCashierInClicked, param => this.CanCashierIn);
            CashierOutClicked = new RelayCommand(ExecuteCashierOutClicked, param => this.CanCashierOut);

            DateTime startdate = DateTime.Today;

            DBReports db = new DBReports();
          

           if(CurrentDrawer  != null)
            {
                cashsales = db.GetStationPaymentTotal(stationno, "CASH", CashIn.CashDate, DateTime.Now);
                creditsales = db.GetStationPaymentTotal(stationno, "CREDIT", CashIn.CashDate, DateTime.Now);
                checksales = db.GetStationPaymentTotal(stationno, "CHECK", CashIn.CashDate, DateTime.Now);
                giftcardsales = db.GetStationPaymentTotal(stationno, "GIFTCARD", CashIn.CashDate, DateTime.Now);
            }
              


            totalsales = cashsales + creditsales + checksales + giftcardsales;
        }

  
        public bool CanCashierIn
        {
            get
            {
                if (CurrentDrawer != null) return false; else return true;
            }
        }

        public bool CanCashierOut
        {
            get
            {
                return !CanCashierIn;
            }
        }

        decimal cashsales = 0;
        public decimal CashSales
        {
            get{ return cashsales; }
        }

        decimal creditsales = 0;
        public decimal CreditSales
        {
            get { return creditsales; }
        }

        decimal checksales = 0;
        public decimal CheckSales
        {
            get { return checksales; }
        }


        decimal giftcardsales = 0;
        public decimal GiftCardSales
        {
            get { return giftcardsales; }
        }

        decimal totalsales = 0;
        public decimal TotalSales
        {
            get { return totalsales; }
        }


        protected CashCount m_cashin;

        public CashCount CashIn
        {
            get { return m_cashin; }
            set
            {
                m_cashin = value;
                NotifyPropertyChanged("CashIn");
            }
        }

        protected CashCount m_cashout;
        public CashCount CashOut
        {
            get { return m_cashout; }
            set
            {
                m_cashout = value;
                NotifyPropertyChanged("CashOut");
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
       

        public void ExecuteBackClicked(object butt)
        {
            _employee.SaveSchedule();
            _parent.Close();
        }


        public void ExecuteCashierInClicked(object obj)
        {
            if(CashIn.CashTotal > 0)
                if(Confirm.Ask("Cashier-In with " + CashIn.CashTotal))
                {
                    CashierIn();
                    _parent.Close();
                }
           
 
        }

        public void ExecuteCashierOutClicked(object obj)
        {
          
            string note = "";

            if (Confirm.Ask("Cashier-Out with " + CashOut.CashTotal))
            {
                difference = CashOut.CashTotal - CashIn.CashTotal - cashsales;

                if (difference != 0)
                {
                    TextPad tp = new TextPad("Please enter reason for discrepancies (" + (difference > 0 ? "OVER" : "SHORT") + "): " + difference, "");
                    Utility.OpenModal(_parent, tp);

                    if (tp.ReturnText == "") return;
                    else note = "Difference:" + difference + ":" +  tp.ReturnText;
                }



                CashierOut(note);
                _parent.Close();
            }
        }


        public void GetDrawer(bool editmode)
        {
            CashIn = new CashCount();
            CashOut = new CashCount();

            DataTable dt = _dbsales.GetCashDrawer(GlobalSettings.Instance.StationNo);

            //get closed drawer
            if (editmode)  dt = _dbsales.GetLastCashDrawer(GlobalSettings.Instance.StationNo);


            if (dt.Rows.Count > 0)
            {
                 CurrentDrawer = new CashDrawer();

                if (dt.Rows[0]["cashindate"].ToString() != "") CashIn.CashDate = (DateTime)dt.Rows[0]["cashindate"];

                CashIn.Cash100 = int.Parse(dt.Rows[0]["cashin100"].ToString());
                CashIn.Cash50 = int.Parse(dt.Rows[0]["cashin50"].ToString());
                CashIn.Cash20 = int.Parse(dt.Rows[0]["cashin20"].ToString());
                CashIn.Cash10 = int.Parse(dt.Rows[0]["cashin10"].ToString());
                CashIn.Cash5 = int.Parse(dt.Rows[0]["cashin5"].ToString());
                CashIn.Cash2 = int.Parse(dt.Rows[0]["cashin2"].ToString());
                CashIn.Cash1 = int.Parse(dt.Rows[0]["cashin1"].ToString());
                CashIn.Cash50cent = int.Parse(dt.Rows[0]["cashin50cent"].ToString());
                CashIn.Cash25cent = int.Parse(dt.Rows[0]["cashin25cent"].ToString());
                CashIn.Cash10cent = int.Parse(dt.Rows[0]["cashin10cent"].ToString());
                CashIn.Cash5cent = int.Parse(dt.Rows[0]["cashin5cent"].ToString());
                CashIn.Cash1cent = int.Parse(dt.Rows[0]["cashin1cent"].ToString());

                if (dt.Rows[0]["cashoutdate"].ToString() != "") CashOut.CashDate = (DateTime)dt.Rows[0]["cashoutdate"];
                CashOut.Cash100 = int.Parse(dt.Rows[0]["cashout100"].ToString());
                CashOut.Cash50 = int.Parse(dt.Rows[0]["cashout50"].ToString());
                CashOut.Cash20 = int.Parse(dt.Rows[0]["cashout20"].ToString());
                CashOut.Cash10 = int.Parse(dt.Rows[0]["cashout10"].ToString());
                CashOut.Cash5 = int.Parse(dt.Rows[0]["cashout5"].ToString());
                CashOut.Cash2 = int.Parse(dt.Rows[0]["cashout2"].ToString());
                CashOut.Cash1 = int.Parse(dt.Rows[0]["cashout1"].ToString());
                CashOut.Cash50cent = int.Parse(dt.Rows[0]["cashout50cent"].ToString());
                CashOut.Cash25cent = int.Parse(dt.Rows[0]["cashout25cent"].ToString());
                CashOut.Cash10cent = int.Parse(dt.Rows[0]["cashout10cent"].ToString());
                CashOut.Cash5cent = int.Parse(dt.Rows[0]["cashout5cent"].ToString());
                CashOut.Cash1cent = int.Parse(dt.Rows[0]["cashout1cent"].ToString());

                CurrentDrawer.CashIn = CashIn;
                CurrentDrawer.CashOut = CashOut;
                CurrentDrawer.Note = dt.Rows[0]["note"].ToString();
                CurrentDrawer.ID = int.Parse(dt.Rows[0]["id"].ToString());

            }
            else CurrentDrawer = null;

           
        }

        public void CashierIn()
        {
            if (CashIn.CashTotal == 0) return;
            _dbsales.CashierIn(GlobalSettings.Instance.StationNo, CurrentEmployee.ID, CashIn.Cash100, CashIn.Cash50, CashIn.Cash20, CashIn.Cash10, CashIn.Cash5, CashIn.Cash2, CashIn.Cash1, CashIn.Cash50cent, CashIn.Cash25cent, CashIn.Cash10cent, CashIn.Cash5cent, CashIn.Cash1cent, CashIn.CashTotal);
            GetDrawer(_editmode);
        }

        public void CashierOut(string note)
        {
            if (CurrentDrawer == null) return;


            _dbsales.CashierOut(CurrentDrawer.ID, CashOut.Cash100, CashOut.Cash50, CashOut.Cash20, CashOut.Cash10, CashOut.Cash5, CashOut.Cash2, CashOut.Cash1, CashOut.Cash50cent, CashOut.Cash25cent, CashOut.Cash10cent, CashOut.Cash5cent, CashOut.Cash1cent, CashOut.CashTotal, note);
             GetDrawer(true);

            QSReports reports = new QSReports();
            reports.PrintCashierOut(CurrentDrawer,difference);
        }

    }






}
