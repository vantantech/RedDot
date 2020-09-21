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
    public class Customer : INPCBase
    {

        public int ID { get; set; }



        private bool m_allowsms;
        private string m_company;
        private string m_driverslicense;
        private string m_creditcardnumber;
        private string m_firstname;
        private string m_middlename;
        private string m_lastname;
        private string m_phone1;
        private string m_address1;
        private string m_address2;
        private string m_city;
        private string m_state;
        private string m_zipcode;
        private string m_email;
        private string m_alertmessage;

        private string m_imgurl;
        private DBCustomer m_dbcustomer;
        private DataTable m_purchasehistory;
        private DataTable m_credithistory;
        private decimal m_rewardcredit;
        private decimal m_rewardearned;
        private decimal m_rewardtotal;
        private decimal m_rewardused;
        private decimal m_rewardbalance;
        private decimal m_usablebalance;
        private decimal m_totalpurchased;
        private int m_numberofvisit;
        private decimal m_tierreward;
        private int m_numberofreferral;
        private bool m_includeopen;
        private bool m_enablealert;
 
        private DateTime m_dob;



        public Customer(int id, bool includeopentickets, bool loadhistory)
        {

            m_includeopen = includeopentickets;

            if (id != 0)
            {
                loadcustomer(id);

                if (loadhistory)
                {
                    CreditHistory = GetCreditHistory(id);
                    PurchaseHistory = GetPurchaseHistory(id);
                }


            }
        }


        public void ReloadHistory()
        {
            CreditHistory = GetCreditHistory(ID);
            PurchaseHistory = GetPurchaseHistory(ID);

        }

        public bool EnableAlert
        {
            get { return m_enablealert; }
            set
            {
                m_enablealert = value;
                m_dbcustomer.UpdateInt(ID, "enablealert", m_enablealert ? 1 : 0);
            }

        }

        public bool AllowSMS
        {
            get { return m_allowsms; }
            set
            {

                m_allowsms = value;
                m_dbcustomer.UpdateInt(ID, "allowsms", m_allowsms ? 1 : 0);
                NotifyPropertyChanged("AllowSMS");

            }

        }

        public string AlertMessage
        {
            get { return m_alertmessage; }
            set
            {
                m_alertmessage = value;
                m_dbcustomer.UpdateString(ID, "AlertMessage", m_alertmessage);
                NotifyPropertyChanged("AlertMessage");
            }
        }

        public string Company
        {
            get { return m_company; }
            set
            {
                m_company = value;
                m_dbcustomer.UpdateString(ID, "Company", m_company);
                NotifyPropertyChanged("Company");
            }
        }

        public string DriversLicense
        {
            get { return m_driverslicense; }
            set
            {
                m_driverslicense = value;
                m_dbcustomer.UpdateString(ID, "driverslicense", m_driverslicense);
                NotifyPropertyChanged("DriversLicense");
            }
        }

        public string CreditCardNumber
        {
            get { return m_creditcardnumber; }
            set
            {
                m_creditcardnumber = value;
                m_dbcustomer.UpdateString(ID, "creditcardnumber", m_creditcardnumber);
                NotifyPropertyChanged("CreditCardNumber");
            }
        }

        public DateTime DOB
        {
            get { return m_dob; }
            set
            {
                m_dob = value;
                m_dbcustomer.UpdateString(ID, "dob", m_dob.ToString("yyyy-MM-dd"));
                NotifyPropertyChanged("DOB");
            }
        }
       
        public int YearsOld
        {
            get
            {
                if (DOB == DateTime.MinValue) return 0;


                TimeSpan tp = (DateTime.Now - DOB);
                return (int)Math.Round(tp.TotalDays / 365.25);
            }
        }



        public string  MiddleName { get { return m_middlename; }
            set
            {
                m_middlename = value;
                m_dbcustomer.UpdateString(ID, "MiddleName", m_middlename);
                NotifyPropertyChanged("MiddleName");
            }
        }
        public string LastName { get { return m_lastname; }
            set {
                m_lastname = value;
                m_dbcustomer.UpdateString(ID, "Lastname", m_lastname);
                NotifyPropertyChanged("LastName");
            }
        }

        public string FirstName { get { return m_firstname; }

            set
            {
                m_firstname = value;
                m_dbcustomer.UpdateString(ID, "firstname", m_firstname);
                NotifyPropertyChanged("FirstName");
            }

        }

       
        public string FullName
        {
            get { return m_firstname + " " + m_lastname; }

 
        }

        public string Phone1 { get { return m_phone1; }
            set
            {
                m_phone1= value;
                m_dbcustomer.UpdateString(ID, "Phone1", m_phone1);
                NotifyPropertyChanged("Phone1");
            }
        }



        public string Address1 { get { return m_address1; }
            set
            {
                m_address1 = value;
                m_dbcustomer.UpdateString(ID, "Address1", m_address1);
                NotifyPropertyChanged("Address1");
            }
        }

        public string Address2 { get { return m_address2; }
            set
            {
                m_address2 = value;
                m_dbcustomer.UpdateString(ID, "Address2", m_address2);
                NotifyPropertyChanged("Address2");
            }
        }

        public string City { get { return m_city; }
            set
            {
                m_city = value;
                m_dbcustomer.UpdateString(ID, "City", m_city);
                NotifyPropertyChanged("City");
            }
        }


        public string State { get { return m_state; }
            set
            {
                m_state = value;
                m_dbcustomer.UpdateString(ID, "State", m_state);
                NotifyPropertyChanged("State");
            }
        }

        public string ZipCode { get { return m_zipcode; }
            set
            {
                m_zipcode = value;
                m_dbcustomer.UpdateString(ID, "ZipCode", m_zipcode);
                NotifyPropertyChanged("ZipCode");
            }
        }

        public string Email { get { return m_email; }
            set
            {
                m_email = value;
                m_dbcustomer.UpdateString(ID, "Email", m_email);
                NotifyPropertyChanged("Email");
            }
        }







        private int PIN { get; set; }




        public int NumberofVisit
        {
            get { return m_numberofvisit; }
            set
            {
                m_numberofvisit = value;
                NotifyPropertyChanged("NumberofVisit");

            }
        }
        public int NumberofReferral
        {
            get { return m_numberofreferral; }
            set
            {
                m_numberofreferral = value;
                NotifyPropertyChanged("NumberofReferral");

            }
        }
        public decimal TierReward
        {
            get { return m_tierreward; }
            set
            {
                m_tierreward = value;
                NotifyPropertyChanged("TierReward");

            }
        }

        public decimal TotalPurchased
        {
            get { return m_totalpurchased; }
            set
            {
                m_totalpurchased = value;
                NotifyPropertyChanged("TotalPurchased");
            }
        }


        public decimal RewardEarned
        {
            get { return m_rewardearned; }
            set
            {
                m_rewardearned = value;
                NotifyPropertyChanged("RewardEarned");
            }
        }

        public decimal RewardTotal
        {
            get { return m_rewardtotal; }
            set
            {
                m_rewardtotal = value;
                NotifyPropertyChanged("RewardTotal");
            }
        }

        public decimal RewardUsed
        {
            get { return m_rewardused; }
            set
            {
                m_rewardused = value;
                NotifyPropertyChanged("RewardUsed");
            }
        }

        public decimal RewardBalance
        {
            get { return m_rewardbalance; }
            set
            {
                m_rewardbalance = value;
                NotifyPropertyChanged("RewardBalance");
            }
        }

        public decimal UsableBalance
        {
            get { return m_usablebalance; }
            set
            {
                m_usablebalance = value;
                NotifyPropertyChanged("UsableBalance");
            }
        }
        public decimal RewardCredit
        {
            get { return m_rewardcredit; }
            set
            {
                m_rewardcredit = value;
                NotifyPropertyChanged("RewardCredit");

            }
        }

        public void loadcustomer(int id)
        {
            try
            {
                if (m_dbcustomer == null) m_dbcustomer = new DBCustomer();

                ID = id;
                DataTable table;
                if (id != 0)
                {
                    table = m_dbcustomer.GetCustomerByID(id);
                }
                else
                {

                        MessageBox.Show("Error loading Customer info");
                        return;
                   
                }
               

                if (table.Rows.Count >= 1)
                {
                    ID         = int.Parse(table.Rows[0]["id"].ToString());
                    m_company    = table.Rows[0]["company"].ToString();
                    m_lastname  = table.Rows[0]["lastname"].ToString();
                    m_middlename = table.Rows[0]["middlename"].ToString();
                    m_firstname = table.Rows[0]["firstname"].ToString();
                    m_address1   = table.Rows[0]["address1"].ToString();
                    m_address2   = table.Rows[0]["address2"].ToString();
                    m_city     = table.Rows[0]["city"].ToString();
                    m_state      = table.Rows[0]["state"].ToString();
                    m_zipcode    = table.Rows[0]["zipcode"].ToString();
                    m_phone1    = table.Rows[0]["phone1"].ToString();
                    m_email     = table.Rows[0]["email"].ToString();
                    m_alertmessage = table.Rows[0]["alertmessage"].ToString();
                    m_driverslicense = table.Rows[0]["driverslicense"].ToString();
                    m_creditcardnumber = table.Rows[0]["creditcardnumber"].ToString();

                    if (table.Rows[0]["dob"].ToString() != "") DOB = (DateTime)table.Rows[0]["dob"];


                    if (table.Rows[0]["AllowSMS"].ToString() != "")
                        m_allowsms  = int.Parse(table.Rows[0]["AllowSMS"].ToString()) == 1 ? true : false;
                    else m_allowsms = true;

             


                    m_imgurl = table.Rows[0]["ImageSrc"].ToString();
                    m_enablealert = int.Parse(table.Rows[0]["EnableAlert"].ToString()) == 1 ? true : false;



                }


            }
            catch (Exception e)
            {

                MessageBox.Show("Load Customer:" + e.Message);

            }

        }

        public DataTable PurchaseHistory
        {
            get { return m_purchasehistory; }
            set
            {
                m_purchasehistory = value;
                NotifyPropertyChanged("PurchaseHistory");

            }
        }

        public DataTable CreditHistory
        {
            get { return m_credithistory; }
            set
            {
                m_credithistory = value;
                NotifyPropertyChanged("CreditHistory");

            }
        }





        private DataTable GetPurchaseHistory(int id)
        {

            decimal total            = 0;
            decimal rewardused       = 0;
        
            decimal minreward        = 0;
            decimal maxreward        = 0;
            decimal totalearned = 0;

         
            minreward        = GlobalSettings.Instance.MinReward;
            maxreward        = GlobalSettings.Instance.MaxReward;

            if (m_dbcustomer == null) m_dbcustomer = new DBCustomer();
            DataTable table  = m_dbcustomer.GetPurchaseHistory(id);

            int visit = 0;
            foreach (DataRow row in table.Rows)
            {


                if (row["transtype"].ToString() == "ADD")
                {
                    if (row["tickettotal"].ToString() != "") total = total + (decimal)row["tickettotal"];
                    if (row["amount"].ToString() != "") totalearned = totalearned + (decimal)row["amount"];
                    visit++;
                }
                else
                     if (row["amount"].ToString() != "") rewardused = rewardused + (decimal)row["amount"];
            }

            NumberofVisit = visit;
            TotalPurchased = total;
            RewardEarned = totalearned;
            RewardEarned = Math.Round(RewardEarned, 2); //round to 2 decimal places
            RewardUsed = rewardused;
            RewardTotal = RewardCredit + RewardEarned;
            RewardBalance = RewardTotal - RewardUsed;


            if (RewardBalance >= minreward)
            {
                if (RewardBalance <= maxreward) UsableBalance = RewardBalance;
                else UsableBalance = maxreward;
            }
            else UsableBalance = 0m;



            return table;
            
        }

        private DataTable GetCreditHistory(int id)
        {

            decimal total      = 0;
            int numberreferral = 0;

            if (m_dbcustomer == null) m_dbcustomer = new DBCustomer();
            DataTable table    = m_dbcustomer.GetCreditHistory(id);

                 foreach (DataRow row in table.Rows)
            {
                total = total + (decimal)row["cash"];
                if (row["note"].ToString() == "Referral") numberreferral++;
            }

            RewardCredit     = total;
            RewardTotal      = RewardCredit + RewardEarned;
            RewardBalance    = RewardTotal - RewardUsed;
            NumberofReferral = numberreferral;

            

            TierReward = GlobalSettings.Instance.Tier0Reward;  //default reward level

            //test each level to see if qualifies
            if (numberreferral >= GlobalSettings.Instance.Tier1) TierReward = GlobalSettings.Instance.Tier1Reward;
            if (numberreferral >= GlobalSettings.Instance.Tier2) TierReward = GlobalSettings.Instance.Tier2Reward;
            if (numberreferral >= GlobalSettings.Instance.Tier3) TierReward = GlobalSettings.Instance.Tier3Reward;

            return table;

        }

        public void AddCredit(  decimal cash, string note)
        {
            if (m_dbcustomer == null) m_dbcustomer = new DBCustomer();
            m_dbcustomer.AddCredit(ID, cash, note);
            CreditHistory = GetCreditHistory(ID);
        }


        public void DeleteCredit(int creditid)
        {
            if (m_dbcustomer == null) m_dbcustomer = new DBCustomer();
            m_dbcustomer.DeleteCredit(creditid);
            CreditHistory = GetCreditHistory(ID);
        }

        public string ImageSrc
        {
            get
            {
                return "pack://siteoforigin:,,,/" + m_imgurl;
            }

            set
            {
                if (value != null) m_imgurl = value;
            }
        }



    }
}
