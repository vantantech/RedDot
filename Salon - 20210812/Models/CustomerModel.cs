using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;
using System.Windows;
using RedDot.DataManager;

namespace RedDot
{
    public class CustomerModel
    {
        private DBCustomer m_dbcustomer;
        private VFD vfd;
        public CustomerModel()
        {

            m_dbcustomer = new DBCustomer();

            vfd = new VFD(GlobalSettings.Instance.DisplayComPort);
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

        public static int LookupCustomer(SecurityModel security)
        {
            CustomerSearch cs = new CustomerSearch(security);
            cs.Topmost = true;
            cs.ShowDialog();
            return (int)cs.customerid;

        }

        public  static decimal GetUsableReward(int customerid)
        {

            Customer cust = new Customer(customerid,true);
            return cust.UsableBalance;
        }


        public Customer EditViewCustomer(Customer CurrentCustomer, SecurityModel security)
        {
            if (CurrentCustomer.ID > 0)
            {
                //Ask if view or delete
                CustomerActionMenu cm = new CustomerActionMenu(security, CurrentCustomer.ID);
                cm.Topmost = true;
                cm.ShowDialog();

                if (cm.Action == "View")
                {
                    CustomerView custvw = new CustomerView(security, CurrentCustomer.ID);
                    custvw.Topmost = true;
                    custvw.ShowDialog();
                    return new Customer(CurrentCustomer.ID, false);  //loads new info that was changed in teh customerview above
                }

                if (cm.Action == "Delete")
                {

                    // CurrentTicket.UpdateCustomerID(0); //vfd display is shown here
                    return null;

                }

                return CurrentCustomer;

            }
            else return CurrentCustomer;
        }

        public int GetCreateCustomer(SecurityModel security)
        {

            int customerid = 0;

            //Ask cashier to lookup or create new
            CustomerLookupCreate lookup = new CustomerLookupCreate();
            lookup.Topmost = true;
            lookup.ShowDialog();
            if (lookup.Action == "LookUp")
            {

                CustomerSearch cs = new CustomerSearch(security);
                cs.Topmost = true;
                cs.ShowDialog();


                if (cs.customerid > 0)
                {
                    customerid = cs.customerid;
                }

            }
            else if (lookup.Action == "Create")
            {
                CustomerPhone pad = new CustomerPhone();
                pad.Amount = "";
                pad.FullNumberRequired = true;
                vfd.WriteRaw("Please Enter Phone", "");
                pad.Topmost = true;
                pad.ShowDialog();
                if (pad.Amount != "")
                {

                    DataTable dt = LookupByPhone(pad.Amount);
                    if (dt.Rows.Count == 0)
                    {
                        //Create new customer

                        customerid = CreateNew(pad.Amount);
                        TouchMessageBox.Show("New Customer Created");
                        if (GlobalSettings.Instance.EditCustomerProfileOnCreate)
                        {
                            CustomerView custvw = new CustomerView(security, customerid);
                            custvw.Topmost = true;
                            custvw.ShowDialog();
                        }

                    }
                    else
                    {
                        TouchMessageBox.Show("Can not create customer, phone number already exist.");
                    }
                }
            }


            return customerid;
        }

    }
}
