using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RedDot
{
    /// <summary>
    /// Interaction logic for CustomerSearch.xaml
    /// </summary>
    public partial class CustomerSearch : Window
    {
        public int customerid=0;
        SecurityModel m_security;
        public CustomerSearch(SecurityModel security)
        {
            m_security = security;
            InitializeComponent();
        }

        private void Validate(string action)
        {
            CustomerModel cust = new CustomerModel();


            if (action == "ID")
            {

                NumPad pad = new NumPad("Enter Customer ID:", true)
                {
                    Topmost = true,
                    Amount = ""
                };
                pad.ShowDialog();

                //if user cancel , returns ""
                if (pad.Amount != "")
                {
                    if (CustomerModel.CheckID(int.Parse(pad.Amount)))
                    {
                        customerid = int.Parse(pad.Amount);
                    }
                    else customerid = 0;

                }
            }

            if (action == "Phone" || action=="Create")
            {

                CustomerPhone pad = new CustomerPhone
                {
                    Topmost = true,
                    Amount = ""
                };
                pad.ShowDialog();

                //if user cancel , returns ""
                if (pad.Amount != "")
                {
                    DataTable dt = cust.LookupByPhone(pad.Amount);
                    if(dt.Rows.Count > 0)
                    {
                        if (dt.Rows.Count == 1)
                        {
                            customerid = int.Parse(dt.Rows[0]["id"].ToString());

                        }
                        else
                        {
                            //Display list of names to pick from
                            CustomerFoundList cfl = new CustomerFoundList(dt);
                            cfl.Topmost = true;
                            cfl.ShowDialog();
                            customerid = cfl.CustomerID;
                        }
                    }
                    else
                    {
                        //customer phone number was not found

                        if (pad.Amount.Length == 10)
                        {
                            //Create new customer
                            customerid = cust.CreateNew(pad.Amount);
                            TouchMessageBox.Show("New Customer Created");

                            if (GlobalSettings.Instance.EditCustomerProfileOnCreate)
                            {
                                CustomerView custvw = new CustomerView(m_security, customerid);
                                custvw.Topmost = true;
                                custvw.ShowDialog();
                            }


                        }
                        else
                        {

                            TouchMessageBox.Show("Please Enter 10 digit number to create customer account");

                            pad = new CustomerPhone
                            {
                                Topmost = true,
                                Amount = "",
                                FullNumberRequired = true
                            };
                
                            pad.ShowDialog();


                            if (pad.Amount != "")
                            {

                                DataTable dt2 = cust.LookupByPhone(pad.Amount);
                                if (dt2.Rows.Count == 0)
                                {
                                    //Create new customer

                                    customerid = cust.CreateNew(pad.Amount);
                                    TouchMessageBox.Show("New Customer Created");

                                    if (GlobalSettings.Instance.EditCustomerProfileOnCreate)
                                    {
                                        CustomerView custvw = new CustomerView(m_security, customerid);
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


                    }





                }
                else return;



            }

            if (action == "Name")
            {

                SearchbyName dlg;
                bool stillsearching=true;
                do
                {
                    dlg = new SearchbyName() { Topmost = true };
                    dlg.ShowDialog();

                    //if user cancel , returns ""
                    if (dlg.FirstName != "" || dlg.LastName != "")
                    {

                        DataTable dt;

                        if (dlg.FirstName == "") dt = cust.GetCustomerbyLastName(dlg.LastName);
                        else if (dlg.LastName == "") dt = cust.GetCustomerbyFirstName(dlg.FirstName);
                        else dt = cust.GetCustomerbyNames(dlg.FirstName, dlg.LastName);

                        if(dt.Rows.Count == 0)
                        {
                            TouchMessageBox.Show("None Found...");

                        }else
                        {
                            if (dt.Rows.Count == 1)
                            {
                                customerid = int.Parse(dt.Rows[0]["id"].ToString());
                                stillsearching = false;
                            }
                            else
                            {
                                //Display list of names to pick from
                                CustomerFoundList cfl = new CustomerFoundList(dt) { Topmost = true };
                                cfl.ShowDialog();
                                customerid = cfl.CustomerID;
                                stillsearching = false;
                            }
                        }


                    }
                    else
                    {
                        stillsearching = false;
                    }
                } while (stillsearching);







            }

            if (action == "All")
            {

                DataTable dt = cust.GetAllCustomer();
              
 
                //Display list of names to pick from
                CustomerFoundList cfl = new CustomerFoundList(dt) { Topmost = true };
                cfl.ShowDialog();
                customerid = cfl.CustomerID;


            }
        }
        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            string action = "";
            Button chosen = sender as Button;
            action = chosen.Tag.ToString().Trim();
            Validate(action);
            this.Close();
        }

     
    }
}
