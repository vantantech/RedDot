using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RedDot;


namespace RedDot.SalonBase
{
    public class Customer:CustomerData
    {



   
        private DBCustomer m_dbcustomer;
        private DataTable  m_purchasehistory;
        private DataTable  m_credithistory;




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

        public void Save()
        {
            m_dbcustomer.UpdateAllowSMS(ID, m_allowsms);
            m_dbcustomer.UpdateString(ID, "Company", m_company);
            m_dbcustomer.UpdateString(ID, "MiddleName", m_middlename);
            m_dbcustomer.UpdateString(ID, "Lastname", m_lastname);
            m_dbcustomer.UpdateString(ID, "firstname", m_firstname);
            m_dbcustomer.UpdateString(ID, "Phone1", m_phone1);
            m_dbcustomer.UpdateString(ID, "Address1", m_address1);
            m_dbcustomer.UpdateString(ID, "Address2", m_address2);
            m_dbcustomer.UpdateString(ID, "City", m_city);
            m_dbcustomer.UpdateString(ID, "State", m_state);
            m_dbcustomer.UpdateString(ID, "ZipCode", m_zipcode);
            m_dbcustomer.UpdateString(ID, "Email", m_email);
            m_dbcustomer.UpdateInt(ID, "Rating", m_rating);



        }



        public void loadcustomer(int id)
        {
            try
            {
                if (id == 0) return;

                if (m_dbcustomer == null) m_dbcustomer = new DBCustomer();

                DataTable table;
             
                table = m_dbcustomer.GetCustomerByID(id);

                if (table.Rows.Count >= 1)
                {
                    this.Init(table.Rows[0]); 

                }


            }
            catch (Exception e)
            {

               // TouchMessageBox.Show("Load Customer:" + e.Message);

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

            decimal totalearned = 0;


            if (m_dbcustomer == null) m_dbcustomer = new DBCustomer();
            DataTable table  = m_dbcustomer.GetPurchaseHistory(id);

            NumberofVisit    = table.Rows.Count;

            foreach (DataRow row in table.Rows)
            {
               if(row["Total"].ToString() !="") total = total + (decimal)row["Total"];
               if (row["rewardearned"].ToString() != "") totalearned = totalearned + (decimal)row["rewardearned"];
   
                if (row["RewardUsed"].ToString() != "") rewardused = rewardused + (decimal)row["RewardUsed"];
            }


            TotalPurchased = total;
            RewardEarned   = totalearned;
            RewardEarned   = Math.Round(RewardEarned, 2); //round to 2 decimal places
            RewardUsed     = rewardused;
            RewardTotal    = RewardCredit + RewardEarned;
            RewardBalance  = RewardTotal  - RewardUsed;
            

     



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

            

         //   TierReward = GlobalSettings.Instance.Tier0Reward;  //default reward level

            //test each level to see if qualifies
          //  if (numberreferral >= GlobalSettings.Instance.Tier1) TierReward = GlobalSettings.Instance.Tier1Reward;
          //  if (numberreferral >= GlobalSettings.Instance.Tier2) TierReward = GlobalSettings.Instance.Tier2Reward;
          //  if (numberreferral >= GlobalSettings.Instance.Tier3) TierReward = GlobalSettings.Instance.Tier3Reward;

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



    }
}
