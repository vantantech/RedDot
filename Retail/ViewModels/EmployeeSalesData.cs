using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    //employee's daily ticket totals
    public class EmployeeSalesData : INPCBase
    {

        public Employee CurrentEmployee { get; set; }
        public Visibility SubTotalVisiblity { get; set; }




        

        private ObservableCollection<SalesData> _employeesales;



        //private decimal _commissionpercent;
        private decimal _grandtotalsales = 0;
        private decimal _grandtotalcost = 0;
        private decimal _grandtotaladjustments = 0;
        private decimal _grandtotalcommissionadjustments = 0;

        Reports _reports;
        public EmployeeSalesData(int employeeid, DateTime startdate, DateTime enddate)
        {


            SubTotalVisiblity = Visibility.Collapsed;

            CurrentEmployee = new Employee(employeeid);


        

            _reports = new Reports();

          

            EmployeeSales = _reports.GetEmployeeCommission(CurrentEmployee.ID, startdate, enddate);

            CalculateTotals();
            
        }




        //----------------------------------Public Properties ----------------------------------------------------------------------------
        public ObservableCollection<SalesData> EmployeeSales { get { return _employeesales; } set { _employeesales = value; NotifyPropertyChanged("EmployeeSales"); } }


        public decimal GrandTotalCommission { get; set; }

        public decimal GrandTotalMargin { get { return (GrandTotalSales - GrandTotalCost) ; } }


        public decimal GrandTotalSales
        {

            get { return _grandtotalsales; }

            set { _grandtotalsales = value; NotifyPropertyChanged("GrandTotalSales"); NotifyPropertyChanged("GrandTotalCommission"); NotifyPropertyChanged("GrandTotalMargin"); }

        }

        public decimal GrandTotalAdjustments
        {

            get { return _grandtotaladjustments; }

            set { _grandtotaladjustments = value; NotifyPropertyChanged("GrandTotalAdjustments"); NotifyPropertyChanged("GrandTotalCommission"); NotifyPropertyChanged("GrandTotalMargin"); }

        }


        public decimal GrandTotalCommissionAdjustments
        {

            get { return _grandtotalcommissionadjustments; }

            set { _grandtotalcommissionadjustments = value; NotifyPropertyChanged("GrandTotalCommissionAdjustments"); NotifyPropertyChanged("GrandTotalCommission"); NotifyPropertyChanged("GrandTotalMargin"); }

        }

        public decimal GrandTotalCost
        {
            get { return _grandtotalcost; }
            set { _grandtotalcost = value; NotifyPropertyChanged("GrandTotalCost"); NotifyPropertyChanged("GrandTotalCommission"); NotifyPropertyChanged("GrandTotalMargin"); }
        }

        public void CalculateTotals()
        {

            decimal totalsales = 0;
            decimal totalcost = 0;
            decimal totaladjust = 0;
            decimal totalcommission = 0;
            decimal totalcommissionadjust = 0;

                foreach (SalesData salesdata in EmployeeSales)
                {
                    totalsales = totalsales + salesdata.TotalSales;
                    totalcost = totalcost + salesdata.TotalCost;
                totalcommission += salesdata.TotalCommission;
                    totaladjust += salesdata.TotalAdjustments;
                    totalcommissionadjust += salesdata.TotalCommissionAdjustment;
            }






            GrandTotalSales = totalsales;
            GrandTotalCost = totalcost;
            GrandTotalCommission = totalcommission;
            GrandTotalAdjustments = totaladjust;
            GrandTotalCommissionAdjustments = totalcommissionadjust;

        }


    }


}
