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
   
    public class EmployeeSalesData : INPCBase
    {

        public Employee CurrentEmployee { get; set; }
        public Visibility SubTotalVisiblity { get; set; }


        public EmployeeSalesData(Employee employee)
        {

            CurrentEmployee = employee;
            SubTotalVisiblity = Visibility.Collapsed;
            
        }

        public EmployeeSalesData(int id)
        {

            CurrentEmployee =new Employee(id);
             SubTotalVisiblity = Visibility.Collapsed;

        }


        //----------------------------------Properties ----------------------------------------------------------------------------


        private ObservableCollection<SalesData> _employeesales;
        public ObservableCollection<SalesData> EmployeeSales { 
            get { return _employeesales; } 
            set { 
                _employeesales = value;
                CalculateTotals();
                NotifyPropertyChanged("EmployeeSales");
            } 
        }


        public decimal GrandTotalCommission { get; set; }

        public decimal GrandTotalMargin { get { return (GrandTotalSales - GrandTotalCost) ; } }

        private decimal _grandtotalsales = 0;
        public decimal GrandTotalSales
        {

            get { return _grandtotalsales; }

            set { _grandtotalsales = value; NotifyPropertyChanged("GrandTotalSales"); NotifyPropertyChanged("GrandTotalCommission"); NotifyPropertyChanged("GrandTotalMargin"); }

        }

        private decimal _grandtotaladjustments = 0;
        public decimal GrandTotalAdjustments
        {

            get { return _grandtotaladjustments; }

            set { _grandtotaladjustments = value; NotifyPropertyChanged("GrandTotalAdjustments"); NotifyPropertyChanged("GrandTotalCommission"); NotifyPropertyChanged("GrandTotalMargin"); }

        }

        private decimal _grandtotalcommissionadjustments = 0;
        public decimal GrandTotalCommissionAdjustments
        {

            get { return _grandtotalcommissionadjustments; }

            set { _grandtotalcommissionadjustments = value; NotifyPropertyChanged("GrandTotalCommissionAdjustments"); NotifyPropertyChanged("GrandTotalCommission"); NotifyPropertyChanged("GrandTotalMargin"); }

        }

        private decimal _grandtotalcost = 0;
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
