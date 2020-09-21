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
        public int CustomerID=0;
        public CustomerSearch()
        {
            InitializeComponent();
        }

        private void Validate(string action)
        {
            CustomerModelCore cust = new CustomerModelCore();


            if (action == "ID")
            {

                NumPad pad = new NumPad("Enter Customer ID:",true);
                pad.Amount = "";
                Utility.OpenModal(this, pad);
                //if user cancel , returns ""
                if (pad.Amount != "")
                {
                    if (CustomerModelCore.CheckID(int.Parse(pad.Amount)))
                    {
                        CustomerID = int.Parse(pad.Amount);
                    }
                    else CustomerID = 0;

                }
            }

            if (action == "Phone")
            {

                CustomerPhone pad = new CustomerPhone();
                pad.Amount = "";
                Utility.OpenModal(this, pad);
                //if user cancel , returns ""
                if (pad.Amount != "")
                {
                    DataTable dt = cust.LookupByPhone(pad.Amount);
                    if (dt.Rows.Count == 1)
                    {
                        CustomerID = int.Parse(dt.Rows[0]["id"].ToString());

                    }
                    else
                    {
                        //Display list of names to pick from
                        CustomerFoundList cfl = new CustomerFoundList(dt);
                        Utility.OpenModal(this, cfl);
                        CustomerID = cfl.CustomerID;
                    }

                }
            }

            if (action == "Name")
            {

                SearchbyName dlg;
                bool stillsearching=true;
                do
                {
                    dlg = new SearchbyName();
                    Utility.OpenModal(this, dlg);

                    //if user cancel , returns ""
                    if (dlg.FirstName != "" || dlg.LastName != "")
                    {

                        DataTable dt;

                        if (dlg.FirstName == "") dt = cust.GetCustomerbyLastName(dlg.LastName);
                        else if (dlg.LastName == "") dt = cust.GetCustomerbyFirstName(dlg.FirstName);
                        else dt = cust.GetCustomerbyNames(dlg.FirstName, dlg.LastName);

                        if(dt.Rows.Count == 0)
                        {
                            MessageBox.Show("None Found...");

                        }else
                        {
                            if (dt.Rows.Count == 1)
                            {
                                CustomerID = int.Parse(dt.Rows[0]["id"].ToString());
                                stillsearching = false;
                            }
                            else
                            {
                                //Display list of names to pick from
                                CustomerFoundList cfl = new CustomerFoundList(dt);
                                Utility.OpenModal(this, cfl);
                                CustomerID = cfl.CustomerID;
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
                CustomerFoundList cfl = new CustomerFoundList(dt);
                Utility.OpenModal(this, cfl);
                CustomerID = cfl.CustomerID;


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
