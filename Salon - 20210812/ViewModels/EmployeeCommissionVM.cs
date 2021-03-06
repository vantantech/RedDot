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
    public class EmployeeCommissionVM : INPCBase
    {

        public Employee CurrentEmployee { get; set; }
        public Visibility SubTotalVisiblity { get; set; }




        

        private ObservableCollection<SalesData> _employeesales;



  
        private decimal _grandtotalsales;
        private decimal _grandtotalcommission;
        private decimal _grandtotalgratuities;
        private decimal _grandtotalnetgratuities;



        private decimal _custom1 = 0;
        private decimal _custom2 = 0;
        private decimal _custom3 = 0;

   

        Reports _reports;
        public EmployeeCommissionVM(int employeeid, DateTime startdate, DateTime enddate)
        {


            SubTotalVisiblity = Visibility.Collapsed;
            SubTotalVisiblity = Visibility.Visible;

            CurrentEmployee = new Employee(employeeid);


            _reports = new Reports();

          

            EmployeeSales = _reports.GetEmployeeCommission(CurrentEmployee, startdate, enddate);

            CalculateGrandTotals();
            
        }




        //----------------------------------Public Properties ----------------------------------------------------------------------------
        public ObservableCollection<SalesData> EmployeeSales
        {
            get { return _employeesales; }
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


        //calculates the grand total that goes on bottom of report
        public void CalculateGrandTotals()
        {

            decimal grandtotalsales = 0;
            decimal grandtotalcommission = 0;
            decimal grandtotalgratuities = 0;
            decimal grandtotalnetgratuities = 0;
            decimal servicecount = 0;
            decimal ticketcount = 0;

      
      

                foreach (SalesData salesdata in EmployeeSales)
                {
                    grandtotalsales += salesdata.TotalSales;
                    grandtotalcommission += salesdata.TotalCommission;
                    grandtotalgratuities += salesdata.Gratuity;
                    grandtotalnetgratuities += salesdata.NetGratuity;
                    servicecount += salesdata.ServiceCount;
                    if(salesdata.ServiceCount >0) ticketcount++;
                }



            GrandServiceCount = servicecount;
            GrandTicketCount = ticketcount;

            GrandTotalSales = grandtotalsales;
            GrandTotalCommission = grandtotalcommission;


            GrandTotalGratuity = grandtotalgratuities;
            GrandTotalNetGratuity = grandtotalnetgratuities;



            Custom1 =Math.Round( GrandTotalCommission * (CurrentEmployee.PaySplit/100m),2);
            Custom2 =Math.Round( GrandTotalCommission * (1-(CurrentEmployee.PaySplit/100m)) + GrandTotalNetGratuity,2);

        }


    }


}
