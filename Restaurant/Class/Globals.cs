using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Windows;
using com.clover.remotepay.sdk;
using DPUruNet;
using System.Data;
using RedDot.Models;

namespace RedDot
{
    public partial class GlobalSettings
    {
        private static GlobalSettings _Instance;
     
     
        private Object _notificationscreen;

        public static GlobalSettings Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new GlobalSettings();
                }
                  
                return _Instance;
            }
        }

        private static VFD _vfd;
        public static VFD CustomerDisplay
        {
            get
            {
                if (_vfd == null)
                    _vfd = new VFD(GlobalSettings.Instance.DisplayComPort);
                return _vfd;
            }
        }

        public ICloverConnector cloverConnector { get; set; }
        public CloverListener ccl { get; set; }

        private DBSettings _dbsettings;
        public DBSettings DBSettings { get { return _dbsettings; } }

        public string ApplicationPath { get { return System.AppDomain.CurrentDomain.BaseDirectory; } }
        public void Init()
        {

            Shop = new Location();
            _dbsettings = new DBSettings();


        }


        public RemoteScreen RemoteScreen { get; set; }
     
        public Reader CurrentReader { get; set; }
        public string SessionKey { get; set; }
        public DateTime SessionExpire { get; set; }
        public Ticket CurrentTicket { get; set; }
   
        public System.Drawing.Rectangle r0 { get; set; }
        public System.Drawing.Rectangle r1 { get; set; }


        public Location Shop { get; set; }

        public int LastAreaId { get; set; }

        public bool Demo { get; set; }

        public int DemoLeft { get; set; }

   

        public  Fmd[] GetAllFmd1s { get; private set; }

        public Fmd[] GetAllFmd2s { get; private set; }
        public  string[] GetAllUserNames { get; private set; }
        
        public  int[] GetallfingerIDs { get; private set; }

        public string[] GetAllUserPins { get; private set; }
        public int[] GetallPinIDs { get; private set; }


        public  void LoadAllFmdsUserIDs()
        {
            DBEmployee dbemployee = new DBEmployee();

            int i = 0;
            int j = 0;

            // populate all fmds and usernames
            DataTable dt = dbemployee.GetEmployeeActive();

            GetAllFmd1s = new Fmd[dt.Rows.Count];
            GetAllFmd2s = new Fmd[dt.Rows.Count];
            GetAllUserNames = new string[dt.Rows.Count];
            GetAllUserPins = new string[dt.Rows.Count];
       

            GetallfingerIDs = new int[dt.Rows.Count];
            GetallPinIDs = new int[dt.Rows.Count];

            bool hasprints = false;

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if ((dr["fmd1"].ToString().Length != 0) && (dr["fmd2"].ToString().Length != 0))
                    {
                        hasprints = true;
                        GetAllFmd1s[i] = Fmd.DeserializeXml(dr["fmd1"].ToString());
                        GetAllFmd2s[i] = Fmd.DeserializeXml(dr["fmd2"].ToString());
                        GetAllUserNames[i] = dr["firstname"].ToString().TrimEnd() + " " + dr["lastname"].ToString().TrimEnd();
                 
                        GetallfingerIDs[i] = int.Parse(dr["id"].ToString());
                        i++;
                    }

                    //need to load pins and separate set of IDs since not all employee might have fingerprints
                    GetAllUserPins[j] = dr["pin"].ToString().TrimEnd();
                    GetallPinIDs[j] = int.Parse(dr["id"].ToString());
                    j++;
                }
            }

           if(!hasprints)
            {
                GetAllFmd1s = null;
                GetAllFmd2s = null;
            }
           
        }

        public Object NotificationScreen
        {
            get { return _notificationscreen; }
            set { _notificationscreen = value; }
        }

        public Visibility ProVersion { get; set; }

        public Visibility BaseVersion { get; set; }


        /*


      ██████╗  █████╗ ████████╗███████╗███████╗
      ██╔══██╗██╔══██╗╚══██╔══╝██╔════╝██╔════╝
      ██║  ██║███████║   ██║   █████╗  ███████╗
      ██║  ██║██╔══██║   ██║   ██╔══╝  ╚════██║
      ██████╔╝██║  ██║   ██║   ███████╗███████║
      ╚═════╝ ╚═╝  ╚═╝   ╚═╝   ╚══════╝╚══════╝


      */
    

        public DateTime PayPeriodStartDate
        {
            get { return _dbsettings.GetDateSetting("Application", "PayPeriodStartDate", "First Day of Pay Period", "01/01/2001"); }
            set { _dbsettings.SaveSetting("Application", "PayPeriodStartDate", value.ToString()); }
        }

        public TimeSpan Shift1Start
        {
            get { return _dbsettings.GetTimeSetting("Application", "Shift1Start", "Shift 1 Start", "9:00"); }
            set { _dbsettings.SaveSetting("Application", "Shift1Start", value.ToString()); }
        }

        public TimeSpan Shift1End
        {
            get { return _dbsettings.GetTimeSetting("Application", "Shift1End", "Shift 1 End", "4:00"); }
            set { _dbsettings.SaveSetting("Application", "Shift1End", value.ToString()); }
        }

        public TimeSpan Shift2Start
        {
            get { return _dbsettings.GetTimeSetting("Application", "Shift2Start", "Shift 2 Start", "9:00"); }
            set { _dbsettings.SaveSetting("Application", "Shift2Start", value.ToString()); }
        }

        public TimeSpan Shift2End
        {
            get { return _dbsettings.GetTimeSetting("Application", "Shift2End", "Shift 2 End", "4:00"); }
            set { _dbsettings.SaveSetting("Application", "Shift2End", value.ToString()); }
        }


        public TimeSpan Shift3Start
        {
            get { return _dbsettings.GetTimeSetting("Application", "Shift3Start", "Shift 3 Start", "12:00"); }
            set { _dbsettings.SaveSetting("Application", "Shift3Start", value.ToString()); }
        }

        public TimeSpan Shift3End
        {
            get { return _dbsettings.GetTimeSetting("Application", "Shift3End", "Shift 3 End", "12:00"); }
            set { _dbsettings.SaveSetting("Application", "Shift3End", value.ToString()); }
        }



        /***
        *    ██╗███╗   ██╗████████╗███████╗ ██████╗ ███████╗██████╗ 
        *    ██║████╗  ██║╚══██╔══╝██╔════╝██╔════╝ ██╔════╝██╔══██╗
        *    ██║██╔██╗ ██║   ██║   █████╗  ██║  ███╗█████╗  ██████╔╝
        *    ██║██║╚██╗██║   ██║   ██╔══╝  ██║   ██║██╔══╝  ██╔══██╗
        *    ██║██║ ╚████║   ██║   ███████╗╚██████╔╝███████╗██║  ██║
        *    ╚═╝╚═╝  ╚═══╝   ╚═╝   ╚══════╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝
        *                                                           
        */
        #region Integer


        public int LocationId
        {
            get { return int.Parse(Utility.GetINIString("LocationId","System","1")); }
            set { Utility.PutINIString("LocationId", "System", value.ToString()); }
        }

        public int StationNo
        {
            get { return int.Parse(Utility.GetINIString("StationNo", "System","1")); }
            set { Utility.PutINIString("StationNo", "System", value.ToString()); }
        }

        public int LaneId
        {
            get { return int.Parse(Utility.GetINIString("LaneId", "Credit Card", "1")); }
            set { Utility.PutINIString("LaneId", "Credit Card", value.ToString()); }

        }

        public int PortNo
        {
            get { return int.Parse(Utility.GetINIString("Port", "DataBase","3306")); }
            set { Utility.PutINIString("Port", "DataBase", value.ToString()); }
        }

        public int SMTPPort
        {
            get { return _dbsettings.IntGetSetting("Email", "SMTPPort", "SMTP Port", "25"); }
            set { _dbsettings.IntSaveSetting("Email", "SMTPPort", value); }
        }

    

        public int Tier1
        {
            get { return _dbsettings.IntGetSetting("Reward", "Tier1", "Tier 1 Min Referral", "10"); }
            set { _dbsettings.IntSaveSetting("Reward", "Tier1", value); }
        }
        public int Tier2
        {
            get { return _dbsettings.IntGetSetting("Reward", "Tier2", "Tier 2 Min Referral", "20"); }
            set { _dbsettings.IntSaveSetting("Reward", "Tier2", value); }
        }

        public int Tier3
        {
            get { return _dbsettings.IntGetSetting("Reward", "Tier3", "Tier 3 Min Referral", "30"); }
            set { _dbsettings.IntSaveSetting("Reward", "Tier3", value); }
        }



        public int ReceiptWidth
        {
            get { return _dbsettings.IntGetSetting("Receipt", "ReceiptWidth", "Receipt Paper Width", "80"); }
            set { _dbsettings.IntSaveSetting("Receipt", "ReceiptWidth", value); }
        }

        public int OpenOrdersAutoCloseSeconds
        {
            get { return _dbsettings.IntGetSetting("Application", "OpenOrdersAutoCloseSeconds", "Open Orders Auto Close Seconds", "60"); }
            set { _dbsettings.IntSaveSetting("Application", "OpenOrdersAutoCloseSeconds", value); }
        }


        public int PinLength
        {
            get { return _dbsettings.IntGetSetting("Application", "PinLength", "PIN Length", "4"); }
            set { _dbsettings.IntSaveSetting("Application", "PinLength", value); }
        }


        public int DayStartHour
        {
            get { return _dbsettings.IntGetSetting("Application", "DayStartHour", "Appointment Start Hour", "9"); }
            set { _dbsettings.IntSaveSetting("Application", "DayStartHour", value); }
        }

        public int DayLength
        {
            get { return _dbsettings.IntGetSetting("Application", "DayLength", "Appointment Day Length", "10"); }
            set { _dbsettings.IntSaveSetting("Application", "DayLength", value); }
        }




        public int CategoryWidth
        {
            get { return _dbsettings.IntGetSetting("Screens", "CategoryWidth", "Category Box Width", "160"); }
            set { _dbsettings.IntSaveSetting("Application", "CategoryWidth", value); }
        }

        public int CategoryHeight
        {
            get { return _dbsettings.IntGetSetting("Screens", "CategoryHeight", "Category Box Height", "70"); }
            set { _dbsettings.IntSaveSetting("Application", "CategoryHeight", value); }
        }


        public int ProductWidth
        {
            get { return _dbsettings.IntGetSetting("Screens", "ProductWidth", "Product Box Width", "160"); }
            set { _dbsettings.IntSaveSetting("Application", "ProductWidth", value); }
        }

        public int ProductHeight
        {
            get { return _dbsettings.IntGetSetting("Screens", "ProductHeight", "Product Box Height", "60"); }
            set { _dbsettings.IntSaveSetting("Application", "ProductHeight", value); }
        }

        public int ProductFontSize
        {
            get { return _dbsettings.IntGetSetting("Screens", "ProductFontSize", "Product Box Font Size", "14"); }
            set { _dbsettings.IntSaveSetting("Application", "ProductFontSize", value); }
        }

        public int CategoryFontSize
        {
            get { return _dbsettings.IntGetSetting("Screens", "CategoryFontSize", "Category Box Font Size", "18"); }
            set { _dbsettings.IntSaveSetting("Application", "CategoryFontSize", value); }
        }

        public int WebUserID
        {
            get { return _dbsettings.IntGetSetting("Webservice", "WebUserID", "Web User ID", "0"); }
            set { _dbsettings.SaveSetting("Webservice", "WebUserID", value.ToString()); }
        }

        public int WebSyncCheckInterval
        {
            get { return _dbsettings.IntGetSetting("Webservice", "WebSyncCheckInterval", "Web Sync Check Interval", "5"); }
            set { _dbsettings.IntSaveSetting("Webservice", "WebSyncCheckInterval", value); }
        }


        public int ButtonBorderThickness
        {
            get { return _dbsettings.IntGetSetting("Screens", "ButtonBorderThickness", "Main Button Border Thickness", "5"); }
            set { _dbsettings.SaveSetting("Screens", "ButtonBorderThickness", value.ToString()); }
        }

        public int AutoTipCustomerCount
        {
            get { return _dbsettings.IntGetSetting("Ticket", "AutoTipCustomerCount", "Auto Tip Customer Count", "100"); }
            set { _dbsettings.SaveSetting("Ticket", "AutoTipCustomerCount", value.ToString()); }
        }


        public int AutoTipPercent
        {
            get { return _dbsettings.IntGetSetting("Ticket", "AutoTipPercent", "Auto Tip Percent", "18"); }
            set { _dbsettings.SaveSetting("Ticket", "AutoTipPercent", value.ToString()); }
        }




        public int AutoAskTimeout
        {
            get { return _dbsettings.IntGetSetting("Application", "AutoAskTimeout", "Auto Ask Timeout (seconds)", "5"); }
            set { _dbsettings.SaveSetting("Application", "AutoAskTimeout", value.ToString()); }
        }

        public int EarlyClockInAllowance
        {
            get { return _dbsettings.IntGetSetting("Application", "EarlyClockInAllowance", "Early Clock-In Allowance (minutes)", "10"); }
            set { _dbsettings.SaveSetting("Application", "EarlyClockInAllowance", value.ToString()); }
        }


        public int MinimumAgeRestriction
        {
            get { return _dbsettings.IntGetSetting("Order Entry", "MinimumAgeRestriction", "Minimum Age Restriction in Years", "21"); }
            set { _dbsettings.IntSaveSetting("Order Entry", "MinimumAgeRestriction", value); }
        }


        #endregion

        /***
        *    ██████╗ ███████╗ ██████╗██╗███╗   ███╗ █████╗ ██╗     
        *    ██╔══██╗██╔════╝██╔════╝██║████╗ ████║██╔══██╗██║     
        *    ██║  ██║█████╗  ██║     ██║██╔████╔██║███████║██║     
        *    ██║  ██║██╔══╝  ██║     ██║██║╚██╔╝██║██╔══██║██║     
        *    ██████╔╝███████╗╚██████╗██║██║ ╚═╝ ██║██║  ██║███████╗
        *    ╚═════╝ ╚══════╝ ╚═════╝╚═╝╚═╝     ╚═╝╚═╝  ╚═╝╚══════╝
        *                                                          
        */

        #region Decimal

        public decimal TipAlertAmount
        {
            get { return _dbsettings.DecimalGetSetting("Credit Card", "TipAlertAmount", "Tip Alert Amount", "30"); }
            set { _dbsettings.SaveSetting("Credit Card", "TipAlertAmount", value.ToString()); }
        }

        public decimal OpenTabPreAuthAmount
        {
            get { return _dbsettings.DecimalGetSetting("Credit Card", "OpenTabPreAuthAmount", "Open Tab PreAuth Amount", "25.00"); }
            set { _dbsettings.SaveSetting("Credit Card", "OpenTabPreAuthAmount", value.ToString()); }
        }

        public decimal Tier0Reward
        {
            get { return _dbsettings.DecimalGetSetting("Reward", "Tier0Reward", "Tier 0 Reward Amount", "1"); }
            set { _dbsettings.DecimalSaveSetting("Reward", "Tier0Reward", value); }
        }

        public decimal Tier1Reward
        {
            get { return _dbsettings.DecimalGetSetting("Reward", "Tier1Reward", "Tier 1 Reward Amount", "1.5"); }
            set { _dbsettings.DecimalSaveSetting("Reward", "Tier1Reward", value); }
        }

        public decimal Tier2Reward
        {
            get { return _dbsettings.DecimalGetSetting("Reward", "Tier2Reward", "Tier 2 Reward Amount", "2"); }
            set { _dbsettings.DecimalSaveSetting("Reward", "Tier2Reward", value); }
        }
        public decimal Tier3Reward
        {
            get { return _dbsettings.DecimalGetSetting("Reward", "Tier3Reward", "Tier 3 Reward Amount", "2.5"); }
            set { _dbsettings.DecimalSaveSetting("Reward", "Tier3Reward", value); }
        }


        public decimal RewardPercent
        {
            get { return _dbsettings.DecimalGetSetting("Reward", "RewardPercent", "Reward Percent", "5"); }
            set { _dbsettings.DecimalSaveSetting("Reward", "RewardFeePercent", value); }
        }
        public decimal MinReward
        {
            get { return _dbsettings.DecimalGetSetting("Reward", "MinReward", "Minimum Reward Usage Amount", "15"); }
            set { _dbsettings.DecimalSaveSetting("Reward", "MinReward", value); }
        }
        public decimal MaxReward
        {
            get { return _dbsettings.DecimalGetSetting("Reward", "MaxReward", "Maximum Reward Usage Amount", "20"); }
            set { _dbsettings.DecimalSaveSetting("Reward", "MaxReward", value); }
        }
        public decimal SalesTaxPercent
        {
            get { return _dbsettings.DecimalGetSetting("Application", "SalesTaxPercent", "Sales Tax Percent", "8.25"); }
            set { _dbsettings.DecimalSaveSetting("Application", "SalesTaxPercent", value); }
        }



        public decimal CreditCardFeePercent
        {
            get { return _dbsettings.DecimalGetSetting("Application", "CreditCardFeePercent", "% Deduct from Credit Card Tips", "3"); }
            set { _dbsettings.DecimalSaveSetting("Application", "CreditCardFeePercent", value); }
        }




        #endregion



    }
}
