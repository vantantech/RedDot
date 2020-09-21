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
    public class CustomerReportVM : INPCBase
    {

        public ICommand PrintClicked { get; set; }

        private DataTable m_customers;
        private CustomerModel m_customermodel;

        public CustomerReportVM(Window parent, SecurityModel security)
        {
            PrintClicked = new RelayCommand(PrintCustomerReport, param => true);

            m_customermodel = new CustomerModel();

            Customers = m_customermodel.GetCustomerReport();
        }

        public DataTable Customers
        {
            get { return m_customers; }

            set
            {
                m_customers = value;
                NotifyPropertyChanged("Customers");
            }
        }

        public void PrintCustomerReport(object obj)
        {

        }

    }
}
