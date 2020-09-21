
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.SalonBase
{
    public class CustomerData : INPCBase
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

            if (row["AllowSMS"].ToString() != "")
                m_allowsms = int.Parse(row["AllowSMS"].ToString()) == 1 ? true : false;
            else m_allowsms = true;

            if (row["preferphone"].ToString() != "")
                m_preferphone = int.Parse(row["preferphone"].ToString());
            else m_preferphone = 1;

            if (row["rating"].ToString() != "")
                m_rating = int.Parse(row["rating"].ToString());
            else m_rating = 0;

            m_imgurl = row["ImageSrc"].ToString();
        }

        public int ID { get; set; }

        public int PreferPhone
        {
            get { return m_preferphone; }
            set
            {
                m_preferphone = value;
                NotifyPropertyChanged("PreferPhone");
            }
        }


        public bool AllowSMS
        {
            get { return m_allowsms; }
            set
            {

                m_allowsms = value;
                NotifyPropertyChanged("AllowSMS");

            }

        }



        public string Company
        {
            get { return m_company; }
            set
            {
                m_company = value;
                NotifyPropertyChanged("Company");
            }
        }
        public string MiddleName
        {
            get { return m_middlename; }
            set
            {
                m_middlename = value;
                NotifyPropertyChanged("MiddleName");
            }
        }
        public string LastName
        {
            get { return m_lastname; }
            set
            {
                m_lastname = value;
                NotifyPropertyChanged("LastName");
            }
        }

        public string FirstName
        {
            get { return m_firstname; }

            set
            {
                m_firstname = value;
                NotifyPropertyChanged("FirstName");
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
                NotifyPropertyChanged("Phone1");
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
                NotifyPropertyChanged("Address1");
            }
        }

        public string Address2
        {
            get { return m_address2; }
            set
            {
                m_address2 = value;
                NotifyPropertyChanged("Address2");
            }
        }

        public string City
        {
            get { return m_city; }
            set
            {
                m_city = value;
                NotifyPropertyChanged("City");
            }
        }


        public string State
        {
            get { return m_state; }
            set
            {
                m_state = value;
                NotifyPropertyChanged("State");
            }
        }

        public string ZipCode
        {
            get { return m_zipcode; }
            set
            {
                m_zipcode = value;
                NotifyPropertyChanged("ZipCode");
            }
        }

        public string Email
        {
            get { return m_email; }
            set
            {
                m_email = value;
                NotifyPropertyChanged("Email");
            }
        }


        public int Rating
        {
            get { return m_rating; }
            set
            {
                m_rating = value;
                NotifyPropertyChanged("Rating");

            }
        }




        protected int PIN { get; set; }




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
