using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RedDotBase;

namespace RedDot
{
    //employee's daily ticket totals
    public class EmployeeCommissionVM : INPCBase
    {

        public Employee CurrentEmployee { get; set; }
        public Visibility SubTotalVisiblity { get; set; }




        

        private ObservableCollection<SalesData> _employeesales;



  
        private decimal _grandtotalsales;
        private decimal _grandtotalsupplyfees;
        private decimal _grandtotaldailyfees;
        private decimal _grandtotalcommission;

        private decimal _grandtotalgratuities;
        private decimal _grandtotalnetgratuities;


        private int _daycount = 0;


        private decimal _custom1 = 0;
        private decimal _custom2 = 0;
        private decimal _custom3 = 0;

        private bool _summary = false;

        Reports _reports;
        public EmployeeCommissionVM(int employeeid, DateTime startdate, DateTime enddate, bool summary)
        {


            SubTotalVisiblity = Visibility.Collapsed;
            SubTotalVisiblity = Visibility.Visible;

            CurrentEmployee = new Employee(employeeid);


            _reports = new Reports();
            _summary = summary;
          

            EmployeeSales = _reports.GetEmployeeCommission(CurrentEmployee, startdate, enddate);

            CalculateGrandTotals();
            
        }


        public bool IsSummary
        {
            get { return _summary; }
            set
            {
                _summary = value;
                NotifyPropertyChanged("IsSummary");
            }
        }



        //----------------------------------Public Properties ----------------------------------------------------------------------------
        public ObservableCollection<SalesData> EmployeeSales
        {
            get {
                if (_summary) return new ObservableCollection<SalesData>();
                else
                    return _employeesales;
            
            
            }
            set {
                _employeesales = value;
                NotifyPropertyChanged("EmployeeSales");
            }
        }


   




        public decimal Custom1
        {
            get { return _custom1; }
            set
            {
                _custom1 = value;
                NotifyPropertyChanged("Custom1");
            }
        }

        public decimal Custom2
        {
            get { return _custom2; }
            set
            {
                _custom2 = value;
                NotifyPropertyChanged("Custom2");
            }
        }

        public decimal Custom3
        {
            get { return _custom3; }
            set
            {
                _custom3 = value;
                NotifyPropertyChanged("Custom3");
            }
        }

        public decimal GrandTotalSales
        {

            get { return _grandtotalsales; }

            set { _grandtotalsales = value; NotifyPropertyChanged("GrandTotalSales"); }

        }

        public decimal GrandTotalDailyFees
        {

            get { return _grandtotaldailyfees; }

            set { _grandtotaldailyfees = value; NotifyPropertyChanged("GrandTotalDailyFees"); }

        }

        public decimal TotalPay
        {
            get { return GrandTotalCommission + GrandTotalNetGratuity; }
        }

        public decimal TotalNetPay
        {
            get { return TotalPay - GrandTotalDailyFees; }
        }

        public decimal GrandTotalSupplyFees
        {

            get { return _grandtotalsupplyfees; }

            set { _grandtotalsupplyfees = value; NotifyPropertyChanged("GrandTotalSupplyFees"); }

        }

        private decimal _grandservicecount;
        public decimal GrandServiceCount
        {

            get { return _grandservicecount; }

            set { _grandservicecount = value; NotifyPropertyChanged("GrandServiceCount"); }

        }

        private decimal _grandticketcount;
        public decimal GrandTicketCount
        {

            get { return _grandticketcount; }

            set { _grandticketcount = value; NotifyPropertyChanged("GrandTicketCount"); }

        }



        public decimal GrandTotalCommission
        {

            get { return _grandtotalcommission; }

            set { _grandtotalcommission = value;  NotifyPropertyChanged("GrandTotalCommission"); }

        }


        public decimal GrandTotalGratuity
        {
            get { return _grandtotalgratuities; }
            set { _grandtotalgratuities = value; NotifyPropertyChanged("GrandTotalGratuity"); }
        }


        //Net Gratuity = Gratuity - Credit Card Fees
        public decimal GrandTotalNetGratuity
        {
            get { return _grandtotalnetgratuities; }
            set { _grandtotalnetgratuities = value; NotifyPropertyChanged("GrandTotalNetGratuity"); }
        }

        public int GrandTotalDayCount
        {
            get { return _daycount; }
            set { _daycount = value; NotifyPropertyChanged("GrandTotalDayCount"); }
        }

        //calculates the grand total that goes on bottom of report
        public void CalculateGrandTotals()
        {

            decimal grandtotalsales = 0;
            decimal grandtotalcommission = 0;
            decimal grandtotalgratuities = 0;
            decimal grandtotalnetgratuities = 0;
            decimal servicecount = 0;
            decimal ticketcount = 0;
            decimal grandtotalsupplyfees = 0;




            if(_employeesales != null)
            foreach (SalesData salesdata in _employeesales)
            {
                grandtotalsales += salesdata.TotalSales;
                grandtotalcommission += salesdata.TotalCommission;
                grandtotalgratuities += salesdata.Gratuity;
                grandtotalnetgratuities += salesdata.NetGratuity;
                grandtotalsupplyfees += salesdata.TotalSupplyFee;
                servicecount += salesdata.ServiceCount;
                if (salesdata.ServiceCount > 0) ticketcount++;
            }

            var daycount = _employeesales.Select(x => x.SaleDate.ToString("M/d/yy")).Distinct().Count();

            GrandTotalDayCount = daycount;

            GrandTotalDailyFees =  daycount * CurrentEmployee.DailyFee;

            GrandTotalSupplyFees = grandtotalsupplyfees;

            GrandServiceCount = servicecount;
            GrandTicketCount = ticketcount;

            GrandTotalSales = grandtotalsales;
            GrandTotalCommission = grandtotalcommission;


            GrandTotalGratuity = grandtotalgratuities;
            GrandTotalNetGratuity = grandtotalnetgratuities;

            Custom1 =Math.Round( (GrandTotalCommission * (CurrentEmployee.PaySplit/100m) - GrandTotalDailyFees),2);
            Custom2 =Math.Round( GrandTotalCommission * (1-(CurrentEmployee.PaySplit/100m)) + GrandTotalNetGratuity,2);

        }


    }


}
