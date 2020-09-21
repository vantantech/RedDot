using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RedDot;

namespace RedDot
{
    public class CustomerModel : CustomerModelCore
    {

        private Window m_parent;
        DBCustomer m_dbcustomer;
        public CustomerModel(Window parent)
        {
            m_parent = parent;
            m_dbcustomer = new DBCustomer();

        }

        public int EditViewCustomer(int customerid, Security security)
        {
    

            if (customerid > 0)
            {
                //Ask if view or delete
                CustomerActionMenu cm = new CustomerActionMenu();
                Utility.OpenModal(m_parent, cm);

                if (cm.Action == "View")
                {
                    CustomerView custvw = new CustomerView(security, customerid);
                    Utility.OpenModal(m_parent, custvw);
                    return customerid;  
                }

                if (cm.Action == "Delete")
                {

                    return 0;

                }

                return customerid;

            }
            else return customerid;
        }

        public int GetCreateCustomer()
        {

            int customerid = 0;

            //Ask cashier to lookup or create new
            CustomerLookupCreate lookup = new CustomerLookupCreate();
            Utility.OpenModal(m_parent, lookup);
            if (lookup.Action == "LookUp")
            {

                CustomerSearch cs = new CustomerSearch();
                Utility.OpenModal(m_parent, cs);


                if (cs.CustomerID > 0)
                {
                    customerid = cs.CustomerID;
                }
                
            }
            else if (lookup.Action == "Create")
            {
                CustomerPhone pad = new CustomerPhone();
                pad.Amount = "";
                pad.FullNumberRequired = true;
                VFD.WriteRaw("Please Enter Phone", "");
                Utility.OpenModal(m_parent, pad);
                if (pad.Amount != "")
                {
                    CustomerModel cust = new CustomerModel(m_parent);
                    DataTable dt = cust.LookupByPhone(pad.Amount);
                    if (dt.Rows.Count == 0)
                    {
                        //Create new customer

                        customerid = cust.CreateNew(pad.Amount);
                        MessageBox.Show("New Customer Created");


                    }
                    else
                    {
                        MessageBox.Show("Can not create customer, phone number already exist.");
                    }
                }
            }


            return customerid;
        }


        public bool DeleteCustomer(int id)
        {
            return m_dbcustomer.DeleteCustomer(id);

        }


        public DataTable GetSMSList()
        {

            DataTable table = m_dbcustomer.GetSMSList();
            return table;
        }














    }
}
