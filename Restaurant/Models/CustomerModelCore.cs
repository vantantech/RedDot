using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;
using System.Windows;

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

        public int CreateNew(string dl,string firstname, string lastname, DateTime DOB)
        {
            return m_dbcustomer.CreateCustomer(dl,firstname,lastname,DOB);
        }
        public DataTable LookupByPhone(string phone1)
        {
            return m_dbcustomer.LookupByPhone(phone1);
        }
        public DataTable GetAllCustomer()
        {

            return m_dbcustomer.GetAllCustomer();
        }

        public DataTable GetCustomerbyDriversLicense( string DL)
        {

            return m_dbcustomer.GetCustomerByDriversLicense(DL);
        }

        public DataTable GetCustomerbyCreditCard(string cardno)
        {

            return m_dbcustomer.GetCustomerByCreditCard(cardno);
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
        public static bool FindCustomerByID(int customerid)
        {
            DataTable dt;
            DBCustomer dbcustomer = new DBCustomer();

            dt = dbcustomer.FindCustomerByID(customerid);
            if (dt.Rows.Count > 0) return true; else return false;

        }

        public static int LookupCustomer(SecurityModel security)
        {
            CustomerSearch cs = new CustomerSearch(security)
            {
                Topmost = true,
                ShowInTaskbar = false
            };
            cs.ShowDialog();
            return (int)cs.CustomerID;

        }


        public int LookupCustomerbyDL(Window parent, SecurityModel security, DriverLicense CustomerLicense)
        {


            if (CustomerLicense.LicenseNo.Length < 4) return 0;

            string last4 = CustomerLicense.LicenseNo.Substring(CustomerLicense.LicenseNo.Length - 4);
            DataTable dt = GetCustomerbyDriversLicense(last4);

            if (dt.Rows.Count == 0)
            {
                Confirm conf = new Confirm("Customer not found .. Create New Record?");
                Utility.OpenModal(parent, conf);

                if (conf.Response == Confirm.OK)
                {
                    //none was found so create new
                    return  CreateNewCustomer(security, last4, CustomerLicense.FirstName, CustomerLicense.LastName, CustomerLicense.DOB);
                }

            }
            else
            {
                if (dt.Rows.Count == 1)
                {
                    return int.Parse(dt.Rows[0]["id"].ToString());

                }
                else
                {

                    //Display list of names to pick from
                    CustomerFoundList cfl = new CustomerFoundList(dt);
                    Utility.OpenModal(parent, cfl);
                    return cfl.CustomerID;


                }
            }

            return 0;
        }

        public  static decimal GetUsableReward(int customerid)
        {

            Customer cust = new Customer(customerid,false,true);
            return cust.UsableBalance;
        }

        public DataTable GetCustomerReport()
        {
            return m_dbcustomer.GetCustomerReport();
        }

        public int CreateNewCustomer(SecurityModel security, string DL, string firstname, string lastname, DateTime DOB)
        {
            return CreateNew(DL,firstname,lastname,DOB);
        }
        public  int CreateNewCustomer(SecurityModel security, string customerphonenumber)
        {
            if (customerphonenumber == "") return 0;

            int customerid;
         


            //customer not found ... so try to create new customer record
            if (customerphonenumber.Length == 10)
            {
                DataTable dt = LookupByPhone(customerphonenumber);
                if (dt.Rows.Count == 0)
                {
                    //Create new customer

                    customerid = CreateNew(customerphonenumber);
                    TouchMessageBox.Show("New Customer Created");

                    if(GlobalSettings.Instance.EditCustomerProfileOnCreate)
                    {
                        CustomerView custvw = new CustomerView(security, customerid);
                        custvw.Topmost = true;
                        custvw.ShowDialog();
                    }

                    return customerid;
                }
                else
                {
                    TouchMessageBox.Show("Can not create customer, phone number already exist.");
                    return 0;
                }


            }
            else
            {
                //try again .. ask for 10 digit number
                TouchMessageBox.Show("Please Enter 10 digit number to create customer account");

                CustomerPhone pad = new CustomerPhone();
                pad.Topmost = true;
                pad.Amount = "";
                pad.FullNumberRequired = true;
                GlobalSettings.CustomerDisplay.WriteRaw("Please Enter Phone", "");
                pad.ShowDialog();


                if (pad.Amount != "")
                {

                    DataTable dt = LookupByPhone(pad.Amount);
                    if (dt.Rows.Count == 0)
                    {
                        //Create new customer

                        customerid = CreateNew(pad.Amount);
                        TouchMessageBox.Show("New Customer Created");
                     
                        return customerid;
                    }
                    else
                    {
                        TouchMessageBox.Show("Can not create customer, phone number already exist.");
                        return 0;
                    }
                }

                return 0;
            }


        }


    
    }
}
