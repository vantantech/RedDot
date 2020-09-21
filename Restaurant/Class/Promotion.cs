using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace RedDot
{
    public class Promotion : INPCBase
    {
        private bool m_savetodb = false;
        private DBPromotions m_dbpromotions;
        private DataRow m_promotiondata;

        public Promotion(DataRow row, bool savetodb)
        {
            m_savetodb = savetodb;
            m_dbpromotions = new DBPromotions();
            m_promotiondata = row;
            InitData(row);
        }

        public Promotion()
        {
            m_savetodb = false;

        }
        public void InitData(DataRow row)
        {
            m_description = m_promotiondata["description"].ToString();
            m_discounttype = m_promotiondata["discounttype"].ToString();
            m_discountmethod = m_promotiondata["discountmethod"].ToString();
     
            m_promocode = m_promotiondata["promocode"].ToString();
             

            m_startdate = (DateTime)m_promotiondata["startdate"];
            m_enddate = (DateTime)m_promotiondata["enddate"];
            m_starttime = (DateTime)m_promotiondata["starttime"];
            m_endtime = (DateTime)m_promotiondata["endtime"];


            if (m_promotiondata["minimumamount"].ToString() != "") m_minimumamount = (decimal)m_promotiondata["minimumamount"]; else m_minimumamount = 0;
            if (m_promotiondata["maxdiscount"].ToString() != "") m_maxdiscount = (decimal)m_promotiondata["maxdiscount"]; else m_maxdiscount = 0;
            if (m_promotiondata["minimumquantity"].ToString() != "") m_minimumquantity = (decimal)m_promotiondata["minimumquantity"]; else m_minimumquantity = 0;
            if (m_promotiondata["minimumweight"].ToString() != "") m_minimumweight = (decimal)m_promotiondata["minimumweight"]; else m_minimumweight = 0;
            if (m_promotiondata["discountamount"].ToString() != "") m_discountamount = (decimal)m_promotiondata["discountamount"]; else m_discountamount = 0;
            if (m_promotiondata["usagenumber"].ToString() != "") m_usagenumber = (int)m_promotiondata["usagenumber"]; else m_usagenumber = 0;
            if (m_promotiondata["securitylevel"].ToString() != "") m_securitylevel = (int)m_promotiondata["securitylevel"]; else m_securitylevel = 50;


            m_sun = m_promotiondata["sun"].ToString() == "1" ? true : false;
            m_mon = m_promotiondata["mon"].ToString() == "1" ? true : false;
            m_tue = m_promotiondata["tue"].ToString() == "1" ? true : false;
            m_wed = m_promotiondata["wed"].ToString() == "1" ? true : false;
            m_thu = m_promotiondata["thu"].ToString() == "1" ? true : false;
            m_fri = m_promotiondata["fri"].ToString() == "1" ? true : false;
            m_sat = m_promotiondata["sat"].ToString() == "1" ? true : false;

            m_active = m_promotiondata["active"].ToString() == "1" ? true : false;

            m_employeeonly = m_promotiondata["employeeonly"].ToString() == "1" ? true : false;
            m_dailyuseonly = m_promotiondata["dailyuseonly"].ToString() == "1" ? true : false;
            m_fullpriceonly = m_promotiondata["fullpriceonly"].ToString() == "1" ? true : false;
            m_limiteduseonly = m_promotiondata["limiteduseonly"].ToString() == "1" ? true : false;
        }



        private string m_description;
        public string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionString(ID, "description", m_description);

                NotifyPropertyChanged("Description");
            }
        }

        public int ID
        {
            get
            {
                if (m_promotiondata["id"].ToString() != "") return (int)m_promotiondata["id"];
                else return 0;
            }

        }

        private DateTime m_startdate;
        public DateTime StartDate
        {
            get
            {
                return m_startdate;
            }

            set
            {
                m_startdate = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionString(ID, "startdate", m_startdate.ToString("yyyy-MM-dd"));
                NotifyPropertyChanged("StartDate");
            }
        }

        private DateTime m_starttime;
        public DateTime StartTime
        {
            get
            {
                return m_starttime;
            }

            set
            {
                m_starttime = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionString(ID, "starttime", m_starttime.ToString("yyyy-MM-dd HH:mm:ss"));
                NotifyPropertyChanged("StartTime");
            }
        }

        public string StartTimeStr
        {
            get
            {
                return StartTime.ToShortTimeString();
            }
            set
            {
                string temp = value;
                try
                {
                    StartTime = DateTime.Parse(temp);
                }
                catch
                {


                }

            }
        }

        private DateTime m_enddate;
        public DateTime EndDate
        {
            get
            {
                return m_enddate;
            }

            set
            {
                m_enddate = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionString(ID, "EndDate", m_enddate.ToString("yyyy-MM-dd"));
                NotifyPropertyChanged("EndDate");
            }
        }


        private DateTime m_endtime;
        public DateTime EndTime
        {
            get
            {
                return m_endtime;
            }

            set
            {
                m_endtime = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionString(ID, "endtime", m_endtime.ToString("yyyy-MM-dd HH:mm:ss"));
                NotifyPropertyChanged("EndTime");
            }
        }

        public string EndTimeStr
        {
            get
            {
                return EndTime.ToShortTimeString();
            }
            set
            {
                string temp = value;
                try
                {
                    EndTime = DateTime.Parse(temp);
                }
                catch
                {
                }
            }
        }


        private string m_discounttype;
        public string DiscountType
        {
            get { return m_discounttype; }

            set
            {
                m_discounttype = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionString(ID, "discounttype", m_discounttype.ToString());
                NotifyPropertyChanged("DiscountType");
            }
        }

        private string m_discountmethod;
        public string DiscountMethod
        {
            get { return m_discountmethod; }

            set
            {
                m_discountmethod = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionString(ID, "discountmethod", m_discountmethod.ToString());
                NotifyPropertyChanged("DiscountMethod");
            }
        }


        private int m_usagenumber;
        public int UsageNumber
        {
            get { return m_usagenumber; }
            set
            {
                m_usagenumber = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "usagenumber", m_usagenumber.ToString());
                NotifyPropertyChanged("UsageNumber");
            }
        }

        public void DeductUsage()
        {
            m_dbpromotions.DeductUsage(ID);
        }

        private int m_securitylevel;
        public int SecurityLevel
        {
            get { return m_securitylevel; }
            set
            {
                m_securitylevel = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "securitylevel", m_securitylevel.ToString());
                NotifyPropertyChanged("SecurityLevel");
            }
        }


        private string m_promocode;
        public string PromoCode
        {
            get { return m_promocode; }

            set
            {
                m_promocode = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionString(ID, "promocode", m_promocode.ToString());
                NotifyPropertyChanged("PromoCode");
            }
        }

        private decimal m_discountamount;
        public decimal DiscountAmount
        {
            get { return m_discountamount; }

            set
            {
                m_discountamount = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "discountamount", m_discountamount.ToString());
                NotifyPropertyChanged("DiscountAmount");
            }

        }

        private decimal m_maxdiscount;
        public decimal MaxDiscount
        {
            get { return m_maxdiscount; }

            set
            {
                m_maxdiscount = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "maxdiscount", m_maxdiscount.ToString());
                NotifyPropertyChanged("MaxDiscount");
            }

        }

        private decimal m_minimumamount;
        public decimal MinimumAmount
        {
            get { return m_minimumamount; }

            set
            {
                m_minimumamount = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "minimumamount", m_minimumamount.ToString());
                NotifyPropertyChanged("MinimumAmount");
            }

        }


        private decimal m_minimumquantity;
        public decimal MinimumQuantity
        {
            get { return m_minimumquantity; }

            set
            {
                m_minimumquantity = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "minimumquantity", m_minimumquantity.ToString());
                NotifyPropertyChanged("MinimumQuantity");
            }

        }

        private decimal m_minimumweight;
        public decimal MinimumWeight
        {
            get { return m_minimumweight; }

            set
            {
                m_minimumweight = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "minimumweight", m_minimumweight.ToString());
                NotifyPropertyChanged("MinimumWeight");
            }

        }


        private bool m_active;
        public bool Active
        {
            get { return m_active; }

            set
            {
                m_active = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "active", m_active ? "1" : "0");
                NotifyPropertyChanged("Active");
            }

        }





        private bool m_sun;
        public bool SUN
        {
            get { return m_sun; }
            set
            {
                m_sun = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "sun", m_sun ? "1" : "0");
                NotifyPropertyChanged("SUN");
            }
        }
        private bool m_mon;
        public bool MON
        {
            get { return m_mon; }
            set
            {
                m_mon = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "mon", m_mon ? "1" : "0");
                NotifyPropertyChanged("MON");
            }
        }

        private bool m_tue;
        public bool TUE
        {
            get { return m_tue; }
            set
            {
                m_tue = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "tue", m_tue ? "1" : "0");
                NotifyPropertyChanged("TUE");
            }
        }

        private bool m_wed;
        public bool WED
        {
            get { return m_wed; }
            set
            {
                m_wed = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "wed", m_wed ? "1" : "0");
                NotifyPropertyChanged("WED");
            }
        }

        private bool m_thu;
        public bool THU
        {
            get { return m_thu; }
            set
            {
                m_thu = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "thu", m_thu ? "1" : "0");
                NotifyPropertyChanged("THU");
            }
        }

        private bool m_fri;
        public bool FRI
        {
            get { return m_fri; }
            set
            {
                m_fri = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "fri", m_fri ? "1" : "0");
                NotifyPropertyChanged("FRI");
            }
        }

        private bool m_sat;
        public bool SAT
        {
            get { return m_sat; }
            set
            {
                m_sat = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "sat", m_sat ? "1" : "0");
                NotifyPropertyChanged("SAT");
            }
        }

        private bool m_employeeonly;
        public bool EmployeeOnly
        {
            get { return m_employeeonly; }
            set
            {
                m_employeeonly = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "employeeonly", m_employeeonly ? "1" : "0");
                NotifyPropertyChanged("EmployeeOnly");
            }
        }

  

        private bool m_dailyuseonly;
        public bool DailyUseOnly
        {
            get { return m_dailyuseonly; }
            set
            {
                m_dailyuseonly = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "dailyuseonly", m_dailyuseonly ? "1" : "0");
                NotifyPropertyChanged("DailyUseOnly");
            }
        }


        private bool m_fullpriceonly;
        public bool FullPriceOnly
        {
            get { return m_fullpriceonly; }
            set
            {
                m_fullpriceonly = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "fullpriceonly", m_fullpriceonly ? "1" : "0");
                NotifyPropertyChanged("FullPriceOnly");
            }
        }

        private bool m_limiteduseonly;
        public bool LimitedUseOnly
        {
            get { return m_limiteduseonly; }
            set
            {
                m_limiteduseonly = value;
                if (m_savetodb) m_dbpromotions.UpdatePromotionNumeric(ID, "limiteduseonly", m_limiteduseonly ? "1" : "0");
                NotifyPropertyChanged("LimitedUseOnly");
            }
        }

        public DataTable GetProductIDs()
        {
           return m_dbpromotions.GetProductIDs(ID);
        }


    }


}
