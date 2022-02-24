using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using RedDot.Base;
using RedDot.DataManager;

namespace RedDot
{
    public class Customer:INPCBase
    {

        public int ID { get; set; }

        public int PreferPhone { get { return m_preferphone; }
            set {
                m_preferphone = value;
                NotifyPropertyChanged("PreferPhone");
            } }


        private bool       m_allowsms;
        private string     m_company;
        private string     m_firstname;
        private string     m_middlename;
        private string     m_lastname;
        private string     m_phone1;

        private string     m_address1;
        private string     m_address2;
        private string     m_city;
        private string     m_state;
        private string     m_zipcode;
        private string     m_email;

        private string     m_imgurl;
        private DBCustomer m_dbcustomer;
        private DataTable  m_purchasehistory;
        private DataTable  m_credithistory;
        private decimal    m_rewardcredit;
        private decimal    m_rewardearned;
        private decimal    m_rewardtotal;
        private decimal    m_rewardused;
        private decimal    m_rewardbalance;
        private decimal    m_usablebalance;
        private decimal    m_totalpurchased;
        private int        m_numberofvisit;
        private decimal    m_tierreward;
        private int        m_numberofreferral;
       private int        m_preferphone;
        private int         m_rating;
        private string m_custom1;   /// use for customer colors and note
 
        private string m_custom2; // use for special status
        private string m_custom3;
        private string m_custom4;


        public Customer(int id,bool loadhistory)
        {
           
          

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



        public bool AllowSMS
        {
            get { return m_allowsms; }
            set
            {

                m_allowsms = value;
                m_dbcustomer.UpdateAllowSMS(ID, m_allowsms);
                NotifyPropertyChanged("AllowSMS");

            }

        }

        private List<ColorList> m_customercolors;

        public List<ColorList> CustomerColors
        {
            get
            {
                
                return m_customercolors;
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
        public string DisplayName
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

        public string PhoneStr
        {
            get
            {
                if (GlobalSettings.Instance.DisplayCustomerPhone)
                {
                    return Phone1;
                }
                else return "";
            }
        }

        public string Custom2
        {
            get { return m_custom2; }
            set
            {
                m_custom2 = value;
                m_dbcustomer.UpdateString(ID, "Custom2", m_custom2);
                NotifyPropertyChanged("Custom2");
            }
        }


        public string Custom3
        {
            get { return m_custom3; }
            set
            {
                m_custom3 = value;
                m_dbcustomer.UpdateString(ID, "Custom3", m_custom3);
                NotifyPropertyChanged("Custom3");
            }
        }

        public string Custom4
        {
            get { return m_custom4; }
            set
            {
                m_custom4 = value;
                m_dbcustomer.UpdateString(ID, "Custom4", m_custom4);
                NotifyPropertyChanged("Custom4");
            }
        }
        public string PhoneFormatted
        {
            get {
                if (Phone1.Length == 10) return "(" + Phone1.Substring(0, 3) + ") " + Phone1.Substring(3, 3) + "-" + Phone1.Substring(6, 4);
                else return Phone1;
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


        public int Rating
        {
            get { return m_rating; }
            set
            {
                m_rating = value;
                m_dbcustomer.UpdateInt(ID, "Rating", m_rating);
                NotifyPropertyChanged("Rating");

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

                        TouchMessageBox.Show("Error loading Customer info");
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
                    m_custom1 = table.Rows[0]["custom1"].ToString();
                    m_custom2 = table.Rows[0]["custom2"].ToString();
                    m_custom3 = table.Rows[0]["custom3"].ToString();
                    m_custom4 = table.Rows[0]["custom4"].ToString();

                    m_email     = table.Rows[0]["email"].ToString();

                    if (table.Rows[0]["AllowSMS"].ToString() != "")
                        m_allowsms  = int.Parse(table.Rows[0]["AllowSMS"].ToString()) == 1 ? true : false;
                    else m_allowsms = true;

                    if (table.Rows[0]["preferphone"].ToString() != "")
                        m_preferphone  = int.Parse(table.Rows[0]["preferphone"].ToString());
                    else m_preferphone = 1;

                    if (table.Rows[0]["rating"].ToString() != "")
                        m_rating = int.Parse(table.Rows[0]["rating"].ToString());
                    else m_rating = 0;

                    m_imgurl = table.Rows[0]["ImageSrc"].ToString();

                    if (m_custom1 == "") m_custom1 = "[]";
                

                    m_customercolors = JsonConvert.DeserializeObject<List<ColorList>>(m_custom1);

                    m_customercolors.Add(new ColorList { color = "Add Note / Color" });

                }


            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Load Customer:" + e.Message);

            }

        }


        public void DeleteColor(string color)
        {
            ConfirmDelete del = new ConfirmDelete("Delete " + color + "?");
          
            del.ShowDialog();
            if (!del.Confirmed) return;


            //reset customercolors to remvoe the "add color" item
            m_customercolors = JsonConvert.DeserializeObject<List<ColorList>>(m_custom1);

            foreach (var col in m_customercolors)
            {
                if (col.color == color)
                {
                    m_customercolors.Remove(col);
                    break;
                }
            }
            var json = JsonConvert.SerializeObject(m_customercolors);
            m_custom1 = json.ToString();


            m_dbcustomer.UpdateString(ID, "custom1", m_custom1);

            

            //reload array
            m_customercolors = JsonConvert.DeserializeObject<List<ColorList>>(m_custom1);

            m_customercolors.Add(new ColorList { color = "Add Note / Color" });
            NotifyPropertyChanged("CustomerColors");

        }

        public void AddColor(string color)
        {
            //reset customercolors to remvoe the "add color" item
            m_customercolors = JsonConvert.DeserializeObject<List<ColorList>>(m_custom1);
            m_customercolors.Add(new ColorList { color = color });
            var json = JsonConvert.SerializeObject(m_customercolors);
            m_custom1 = json.ToString();
            //save to database
            m_dbcustomer.UpdateString(ID, "custom1", m_custom1);

            //reload array
            m_customercolors = JsonConvert.DeserializeObject<List<ColorList>>(m_custom1);

            m_customercolors.Add(new ColorList { color = "Add Note / Color" });
            NotifyPropertyChanged("CustomerColors");

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
               

               if(row["transtype"].ToString() == "ADD" && row["void"].ToString() != "1")
                {
                    if (row["tickettotal"].ToString() != "") total = total + (decimal)row["tickettotal"];
                    if (row["amount"].ToString() != "") totalearned = totalearned + (decimal)row["amount"];
                    visit++;
                }
               else
                if (row["transtype"].ToString() == "REDEEM" && row["void"].ToString() != "1")
                    if (row["amount"].ToString() != "") rewardused = rewardused + (decimal)row["amount"];
            }

            NumberofVisit = visit;
            TotalPurchased = total;
            RewardEarned   = totalearned;
            RewardEarned   = Math.Round(RewardEarned, 2); //round to 2 decimal places
            RewardUsed     = rewardused;
            RewardTotal    = RewardCredit + RewardEarned;
            RewardBalance  = RewardTotal  - RewardUsed;
            

            if(RewardBalance >= minreward)
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

        public void AddCredit( int points, decimal cash, string note)
        {
            if (m_dbcustomer == null) m_dbcustomer = new DBCustomer();
            m_dbcustomer.AddCredit(ID, points, cash, note);
            CreditHistory = GetCreditHistory(ID);
        }


        public void DeleteCredit(int creditid)
        {
            if (m_dbcustomer == null) m_dbcustomer = new DBCustomer();
            m_dbcustomer.DeleteCredit(creditid);
            CreditHistory = GetCreditHistory(ID);
        }


        public void MigrateTo(int customerid)
        {
            m_dbcustomer.MigrateTo(ID,customerid);

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
