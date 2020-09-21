using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;

namespace RedDot
{
    public class CustomerModelCore
    {
        private DBCustomer m_dbcustomer;

        public CustomerModelCore()
        {

            m_dbcustomer = new DBCustomer();
        }


        public int CreateNew(string phonenumber)
        {
            return m_dbcustomer.CreateCustomer(phonenumber);
        }
        public DataTable LookupByPhone(string phone1)
        {
            return m_dbcustomer.LookupByPhone(phone1);
        }
        public DataTable GetAllCustomer()
        {

            return m_dbcustomer.GetAllCustomer();
        }
        public DataTable GetCustomerbyLastName(string lastname)
        {

            return m_dbcustomer.GetCustomerByLastName(lastname);
        }
        public DataTable GetCustomerbyFirstName(string firstname)
        {

            return m_dbcustomer.GetCustomerByFirstName(firstname);
        }
        public DataTable GetCustomerbyNames(string firstname, string lastname)
        {

            return m_dbcustomer.GetCustomerByNames(firstname, lastname);
        }
        public DataTable GetSMSList()
        {

            DataTable table = m_dbcustomer.GetSMSList();
            return table;
        }
        public static bool CheckID(int customerid)
        {
            DataTable dt;
            DBCustomer dbcustomer = new DBCustomer();

            dt = dbcustomer.CheckID(customerid);
            if (dt.Rows.Count > 0) return true; else return false;

        }

        public static int LookupCustomer()
        {
            CustomerSearch cs = new CustomerSearch();
            cs.ShowDialog();
            return (int)cs.CustomerID;

        }

        public  static decimal GetUsableReward(int customerid)
        {

            Customer cust = new Customer(customerid,true);
            return cust.UsableBalance;
        }


    }
}
