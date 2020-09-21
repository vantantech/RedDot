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
        public DataTable CallerList { get; set; }
        private DBConnect dbConnect;
        private SecurityModel m_security;

        public CustomerSearch(SecurityModel security)
        {
            InitializeComponent();
            this.DataContext = this;

            m_security = security;


            dbConnect = new DBConnect();
            if (dbConnect == null) dbConnect = new DBConnect();
            string query = "Select * from callerid order by calltime desc";
            CallerList = dbConnect.GetData(query, "Table");
        }

        private void Validate(string action)
        {
            CustomerModelCore cust = new CustomerModelCore();

            switch(action.ToUpper())
            {
                

               

                case "ID":
                    NumPad pad = new NumPad("Enter Customer ID:", true, false);
                    pad.Amount = "";
                    Utility.OpenModal(this, pad);
                    //if user cancel , returns ""
                    if (pad.Amount != "")
                    {
                        if (CustomerModelCore.FindCustomerByID(int.Parse(pad.Amount)))
                        {
                            CustomerID = int.Parse(pad.Amount);
                        }
                        else CustomerID = 0;

                    }
                    break;

                case "CREATE":
                case "PHONE":
                    CustomerPhone ph = new CustomerPhone();
                    ph.Amount = "";
                    Utility.OpenModal(this, ph);
                    //if user cancel , returns ""
                    if (ph.Amount != "")
                    {
                        DataTable dt = cust.LookupByPhone(ph.Amount);
                        if (dt.Rows.Count == 1)
                        {
                            CustomerID = int.Parse(dt.Rows[0]["id"].ToString());

                        }
                        else
                        {
                            if (dt.Rows.Count == 0)
                            {
                                //none was found so create new
                                CustomerID = cust.CreateNewCustomer(m_security, ph.Amount);

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


                    break;
                case "DL#":

                    GetDriverLicense dl = new GetDriverLicense(null);
                    Utility.OpenModal(this, dl);
                    CustomerID = cust.LookupCustomerbyDL(this, m_security, dl.CustomerLicense);

                    break;

                case "NAME":
                    SearchbyName dlg;
                    bool stillsearching = true;
                    do
                    {
                        dlg = new SearchbyName();
                        Utility.OpenModal(this, dlg);

                        //if user cancel , returns ""
                        if (dlg.FirstName != "" || dlg.LastName != "")
                        {

                            DataTable dt2;

                            if (dlg.FirstName == "") dt2 = cust.GetCustomerbyLastName(dlg.LastName);
                            else if (dlg.LastName == "") dt2 = cust.GetCustomerbyFirstName(dlg.FirstName);
                            else dt2 = cust.GetCustomerbyNames(dlg.FirstName, dlg.LastName);

                            if (dt2.Rows.Count == 0)
                            {
                                MessageBox.Show("None Found...");

                            }
                            else
                            {
                                if (dt2.Rows.Count == 1)
                                {
                                    CustomerID = int.Parse(dt2.Rows[0]["id"].ToString());
                                    stillsearching = false;
                                }
                                else
                                {
                                    //Display list of names to pick from
                                    CustomerFoundList cfl2 = new CustomerFoundList(dt2);
                                    Utility.OpenModal(this, cfl2);
                                    CustomerID = cfl2.CustomerID;
                                    stillsearching = false;
                                }
                            }


                        }
                        else
                        {
                            stillsearching = false;
                        }
                    } while (stillsearching);

                    break;

                case "ALL":

                    DataTable dt3 = cust.GetAllCustomer();

                    //Display list of names to pick from
                    CustomerFoundList cfl3 = new CustomerFoundList(dt3);
                    Utility.OpenModal(this, cfl3);
                    CustomerID = cfl3.CustomerID;
                    break;
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

        private void CallerClicked(object sender, RoutedEventArgs e)
        {
            Button picked = sender as Button;
            string customernumber = picked.Tag.ToString().Replace("-","");
            long phonenumber;
            long.TryParse(customernumber, out phonenumber);

            CustomerModelCore cust = new CustomerModelCore();
            DataTable dt = cust.LookupByPhone(phonenumber.ToString());
            if (dt.Rows.Count == 1)
            {
                CustomerID = int.Parse(dt.Rows[0]["id"].ToString());

            }
            else
            {
                if (dt.Rows.Count == 0)
                {
                    //none was found so create new
                    CustomerID = cust.CreateNewCustomer(m_security,phonenumber.ToString());

                }
                else
                {
                    TouchMessageBox.Show("Customer Record Creation failed.");
                    CustomerID = 0;
                }
            

            }

            this.Close();
        }
    }
}
