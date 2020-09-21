using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RedDot
{
   public  class CustomerModel:CustomerModelCore
   {

        public static void AddEditCustomer(Ticket m_currentTicket, SecurityModel m_security, Window m_parent)
        {



            //if ticket already has customer linked, then bring up edit screen
            if (m_currentTicket.CurrentCustomer != null)
            {
                EditCustomer(m_currentTicket, m_security, m_parent);
                return;
            }

            CustomerModel cust = new CustomerModel();
            int customerid = 0;
      
            //Lookup customer , if not found, then create new
            customerid = LookupCustomer(m_security);


            //add customer id to ticket .. it will update database
            m_currentTicket.UpdateCustomerID(customerid);




            //Now perform other function like Alerts and Rewards
            if (customerid > 0)
            {

                    //link customer to ticket
                    m_currentTicket.UpdateCustomerID(customerid);
                    if (GlobalSettings.Instance.DisplayRewardAlert)
                    {
                        //check to see if customer has usable rewards
                        if (m_currentTicket.CurrentCustomer.UsableBalance > 0)
                        {
                            string message;
                            message = "Customer has Reward: " + m_currentTicket.CurrentCustomer.UsableBalance.ToString("c");
                        TouchMessageBox.Show(message);
                        }

                    }


                    //also checks if customer has Alert Set
                    if (m_currentTicket.CurrentCustomer.EnableAlert)
                        TouchMessageBox.Show(m_currentTicket.CurrentCustomer.AlertMessage);

            }
   


        }

        public static void EditCustomer(Ticket m_currentTicket, SecurityModel m_security, Window m_parent)
        {
            if (m_security.WindowAccess("CustomerView") == false)
            {

                return; //jump out of routine
            }

            if (m_currentTicket.CurrentCustomer.ID > 0)
            {
                //Ask if view or delete
                CustomerActionMenu cm = new CustomerActionMenu();
                Utility.OpenModal(m_parent, cm);

                if (cm.Action == "View")
                {
                    CustomerView custvw = new CustomerView(m_security, m_currentTicket.CurrentCustomer.ID);
                    Utility.OpenModal(m_parent, custvw);
                    m_currentTicket.CurrentCustomer = new Customer(m_currentTicket.CurrentCustomer.ID, false, false);  //loades new
                }

                if (cm.Action == "Delete")
                {

                    m_currentTicket.UpdateCustomerID(0); //VFD display is shown here
                    m_currentTicket.CurrentCustomer = null;

                }

                return; //exits code here

            }
        }

     

   

    }
}
