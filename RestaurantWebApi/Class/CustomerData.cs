
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.OrderService.Class
{
    public class CustomerData
    {

  


        protected bool m_allowsms;
        protected string m_company;
        protected string m_firstname;
        protected string m_middlename;
        protected string m_lastname;
        protected string m_phone1;

        protected string m_address1;
        protected string m_address2;
        protected string m_city;
        protected string m_state;
        protected string m_zipcode;
        protected string m_email;

        protected string m_imgurl;
        protected decimal m_rewardcredit;
        protected decimal m_rewardearned;
        protected decimal m_rewardtotal;
        protected decimal m_rewardused;
        protected decimal m_rewardbalance;
        protected decimal m_usablebalance;
        protected decimal m_totalpurchased;
        protected int m_numberofvisit;
        protected decimal m_tierreward;
        protected int m_numberofreferral;
        protected int m_preferphone;
        protected int m_rating;


        public CustomerData(DataRow row)
        {
            Init(row);
        }

        public CustomerData()
        {

        }


        protected void Init(DataRow row)
        {
            ID = int.Parse(row["id"].ToString());
            m_company = row["company"].ToString();
            m_lastname = row["lastname"].ToString();
            m_middlename = row["middlename"].ToString();
            m_firstname = row["firstname"].ToString();
            m_address1 = row["address1"].ToString();
            m_address2 = row["address2"].ToString();
            m_city = row["city"].ToString();
            m_state = row["state"].ToString();
            m_zipcode = row["zipcode"].ToString();
            m_phone1 = row["phone1"].ToString();

            m_email = row["email"].ToString();

        }

        public int ID { get; set; }

        public int PreferPhone
        {
            get { return m_preferphone; }
            set
            {
                m_preferphone = value;
           
            }
        }


        public bool AllowSMS
        {
            get { return m_allowsms; }
            set
            {

                m_allowsms = value;
          

            }

        }



        public string Company
        {
            get { return m_company; }
            set
            {
                m_company = value;
          
            }
        }
        public string MiddleName
        {
            get { return m_middlename; }
            set
            {
                m_middlename = value;
            
            }
        }
        public string LastName
        {
            get { return m_lastname; }
            set
            {
                m_lastname = value;
            
            }
        }

        public string FirstName
        {
            get { return m_firstname; }

            set
            {
                m_firstname = value;
        
            }

        }
        public string DisplayName
        {
            get { return m_firstname + " " + m_lastname; }


        }

        public string Phone1
        {
            get { return m_phone1; }
            set
            {
                m_phone1 = value;
             
            }
        }

        public string PhoneFormatted
        {
            get
            {
                if (Phone1.Length == 10) return "(" + Phone1.Substring(0, 3) + ") " + Phone1.Substring(3, 3) + "-" + Phone1.Substring(6, 4);
                else return Phone1;
            }
        }

        public string Address1
        {
            get { return m_address1; }
            set
            {
                m_address1 = value;
             
            }
        }

        public string Address2
        {
            get { return m_address2; }
            set
            {
                m_address2 = value;
            
            }
        }

        public string City
        {
            get { return m_city; }
            set
            {
                m_city = value;
             
            }
        }


        public string State
        {
            get { return m_state; }
            set
            {
                m_state = value;
          
            }
        }

        public string ZipCode
        {
            get { return m_zipcode; }
            set
            {
                m_zipcode = value;
        
            }
        }

        public string Email
        {
            get { return m_email; }
            set
            {
                m_email = value;
     
            }
        }


        public int Rating
        {
            get { return m_rating; }
            set
            {
                m_rating = value;


            }
        }




        protected int PIN { get; set; }




        public int NumberofVisit
        {
            get { return m_numberofvisit; }
            set
            {
                m_numberofvisit = value;


            }
        }
        public int NumberofReferral
        {
            get { return m_numberofreferral; }
            set
            {
                m_numberofreferral = value;


            }
        }
        public decimal TierReward
        {
            get { return m_tierreward; }
            set
            {
                m_tierreward = value;
   

            }
        }

        public decimal TotalPurchased
        {
            get { return m_totalpurchased; }
            set
            {
                m_totalpurchased = value;
 
            }
        }


        public decimal RewardEarned
        {
            get { return m_rewardearned; }
            set
            {
                m_rewardearned = value;

            }
        }

        public decimal RewardTotal
        {
            get { return m_rewardtotal; }
            set
            {
                m_rewardtotal = value;

            }
        }

        public decimal RewardUsed
        {
            get { return m_rewardused; }
            set
            {
                m_rewardused = value;

            }
        }

        public decimal RewardBalance
        {
            get { return m_rewardbalance; }
            set
            {
                m_rewardbalance = value;

            }
        }

        public decimal UsableBalance
        {
            get { return m_usablebalance; }
            set
            {
                m_usablebalance = value;

            }
        }
        public decimal RewardCredit
        {
            get { return m_rewardcredit; }
            set
            {
                m_rewardcredit = value;
      

            }
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
