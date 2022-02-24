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
            get { return int.Parse(Utility.GetINIString("LaneId", "CreditCard", "1")); }
            set { Utility.PutINIString("LaneId", "CreditCard", value.ToString()); }

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



        /*
                ███████╗████████╗██████╗ ██╗███╗   ██╗ ██████╗ ███████╗
                ██╔════╝╚══██╔══╝██╔══██╗██║████╗  ██║██╔════╝ ██╔════╝
                ███████╗   ██║   ██████╔╝██║██╔██╗ ██║██║  ███╗███████╗
                ╚════██║   ██║   ██╔══██╗██║██║╚██╗██║██║   ██║╚════██║
                ███████║   ██║   ██║  ██║██║██║ ╚████║╚██████╔╝███████║
                ╚══════╝   ╚═╝   ╚═╝  ╚═╝╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚══════╝

        */

        #region Strings

        public string BoltBaseURL
        {
            get { return _dbsettings.StringGetSetting("CreditCard", "BoltBaseURL", "Bolt Base URL", "https://bolt.cardpointe.com/api"); }
            set { _dbsettings.StringSaveSetting("CreditCard","BoltBaseURL", value.ToString()); }
        }


        public string CardConnectURL
        {
            get { return _dbsettings.StringGetSetting("CreditCard", "CardConnectURL", "Card Connect URL", "https://boltgw.cardconnect.com/cardconnect/rest"); }
            set { _dbsettings.StringSaveSetting("CreditCard", "CardConnectURL", value.ToString()); }
        }

        public string CardConnectUsernamePassword
        {
            get { return _dbsettings.StringGetSetting("CreditCard", "CardConnectUsernamePassword", "CardConnect Username Password", "vantanprod:2#jfFVnn&9K4g=sMJ3xu"); }
            set { _dbsettings.StringSaveSetting("CreditCard", "CardConnectUsernamePassword", value.ToString()); }
        }

        public string CardConnectAuthorization
        {
            get { return _dbsettings.StringGetSetting("CreditCard", "CardConnectAuthorization", "CardConnect Authorization", "FJjybuRK6EDYAVAWtP+0ufZ6Vnru3QjbHc3pJiyKW4I="); }
            set { _dbsettings.StringSaveSetting("CreditCard", "CardConnectAuthorization", value.ToString()); }
        }

        public string MerchantID
        {
            get { return _dbsettings.StringGetSetting("CreditCard", "MerchantID", "Merchant ID", "496440222883"); }
            set { _dbsettings.StringSaveSetting("CreditCard", "MerchantID", value.ToString()); }
        }


        string _hsn = "";
        public string HardwareSerialNumber
        {
            get
            {
                if (_hsn == "")
                {
                    _hsn = Utility.GetINIString("HardwareSerialNumber", "CreditCard", "C032UQ03960675");
                }
                return _hsn;
            }
            set
            {
                _hsn = value.ToString();
                Utility.PutINIString("HardwareSerialNumber", "CreditCard", value.ToString());
            }
        }


        string _model = "";
        public string PinPadModel
        {
            get
            {
                if (_model == "")
                {
                    _model = Utility.GetINIString("PinPadModel", "CreditCard", "Mini2");
                }
                return _model;
            }
            set
            {
                _model = value.ToString();
                Utility.PutINIString("PinPadModel", "CreditCard", value.ToString());
            }
        }


        public string DisplayComPort
        {
            get { return Utility.GetINIString("DisplayComPort", "System", "COM2"); }
            set { Utility.PutINIString("DisplayComPort", "System", value.ToString()); }
        }

    

        private string m_versionnumber;
        public string VersionNumber
        {
            get { return m_versionnumber; }
            set {m_versionnumber = value; }
        }

        public string SIPDefaultIPAddress
        {
            get { return Utility.GetINIString( "SIPDefaultIPAddress", "CreditCard", "localhost"); }
            set { Utility.PutINIString("SIPDefaultIPAddress","CreditCard", value.ToString()); }
        }

        public string SIPPort
        {
            get { return Utility.GetINIString("SIPPort", "CreditCard", "8080"); } //10009 for PAXS300
            set { Utility.PutINIString("SIPPort","CreditCard", value.ToString()); }
        }
        public string ElementExpressURL
        {
            get { return Utility.GetINIString("ElementExpressURL","CreditCard", "https://transaction.elementexpress.com/"); }
            set { Utility.PutINIString("ElementExpressURL", "CreditCard", value.ToString()); }
        }

        public string ElementExpressReportingURL
        {
            get { return Utility.GetINIString("ElementExpressReportingURL", "CreditCard", "https://reporting.elementexpress.com/"); }
            set { Utility.PutINIString("ElementExpressReportingURL", "CreditCard", value.ToString()); }
        }

        public string AcceptorID
        {
            get { return Utility.GetINIString("AcceptorID", "CreditCard", ""); }
            set { Utility.PutINIString("AcceptorID", "CreditCard", value.ToString()); }
        }

        public string AccountID
        {
            get { return Utility.GetINIString("AccountID", "CreditCard", ""); }
            set { Utility.PutINIString("AccountID", "CreditCard", value.ToString()); }
        }

        public string AccountToken
        {
            get { return Utility.GetINIString("AccountToken", "CreditCard", ""); }
            set { Utility.PutINIString("AccountToken", "CreditCard", value.ToString()); }
        }

        public string TerminalID
        {
            get { return Utility.GetINIString("TerminalID", "CreditCard", ""); }
            set { Utility.PutINIString("TerminalID", "CreditCard", value.ToString()); }
        }


        public string WebApiLocalPort
        {
            get { return Utility.GetINIString("LocalPort", "WebApi", "4040"); } //4040 for local web service - mobile app
            set { Utility.PutINIString("LocalPort", "WebApi", value.ToString()); }
        }

        public string WebApiLocalAddress
        {
            get { return Utility.GetINIString("LocalAddress", "WebApi", "http://192.168.1.30"); } //localhost
            set { Utility.PutINIString("LocalAddress", "WebApi", value.ToString()); }
        }

        public string LanguageCode
        {
            get { return _dbsettings.StringGetSetting("System", "LanguageCode", "Language Code", "en-US"); }
            set { _dbsettings.StringSaveSetting("System", "LanguageCode", value.ToString()); }
        }

        public string BatchTime
        {
            get { return _dbsettings.StringGetSetting("Application", "BatchTime", "Batch Time", "11:30 PM"); }
            set { _dbsettings.StringSaveSetting("Application", "BatchTime", value.ToString()); }
        }

        public string BackupDirectory
        {
            get { return _dbsettings.StringGetSetting("Application", "BackupDirectory", "Backup Directory","c:\reddot\backup"); }
            set { _dbsettings.StringSaveSetting("Application", "BackupDirectory", value.ToString()); }
        }

        public string PackagingPrinter
        {
            get { return Utility.GetINIString("PackagingPrinter", "Printer",  "Packaging"); }
            set { Utility.PutINIString("PackagingPrinter", "Printer", value.ToString()); }
        }

        public string ReceiptPrinter
        {
            get { return Utility.GetINIString("ReceiptPrinter","Printer",""); }
            set { Utility.PutINIString("ReceiptPrinter", "Printer", value.ToString()); }
        }

        public string ReceiptPrinterMode
        {
            get { return Utility.GetINIString("ReceiptPrinterMode", "Printer", "EPSON"); }
            set { Utility.PutINIString("ReceiptPrinterMode", "Printer", value.ToString()); }
        }

        public string ReportPrinterMode
        {
            get { return Utility.GetINIString("ReportPrinterMode", "Printer", "EPSON"); }
            set { Utility.PutINIString("ReportPrinterMode", "Printer", value.ToString()); }
        }

        public string ReportPrinter
        {
            get { return Utility.GetINIString("ReportPrinter", "Printer",""); }
            set { Utility.PutINIString("ReportPrinter", "Printer", value.ToString()); }
        }


    


        public string MainBackgroundImage
        {
            get { return _dbsettings.StringGetSetting("Application", "MainBackgroundImage", "Main Background Image", "","image"); }
            set { _dbsettings.StringSaveSetting("Application", "MainBackgroundImage", value.ToString()); }
        }

        public string WindowBackgroundColor
        {
            get { return _dbsettings.StringGetSetting("Application", "WindowBackgroundColor", "Window Background Color", "Black", "color"); }
            set { _dbsettings.StringSaveSetting("Application", "WindowBackgroundColor", value.ToString()); }
        }



        public string StoreLogo
        {
            get
            { return _dbsettings.StringGetSetting("Application", "StoreLogo", "Store Logo File", ""); }
            set
            {  _dbsettings.StringSaveSetting("Application", "StoreLogo", value.ToString());   }
        }




        public string PayPeriodType
        {
            get { return _dbsettings.StringGetSetting("Application", "PayPeriodType", "Pay Period Type", "BiWeekly"); }
            set { _dbsettings.StringSaveSetting("Application", "PayPeriodType", value.ToString()); }
        }

        public string ReceiptNotice
        {
            get { return _dbsettings.StringGetSetting("Ticket", "ReceiptNotice", "Receipt Notice at Bottom", ""); }
            set { _dbsettings.StringSaveSetting("Ticket", "ReceiptNotice", value.ToString()); }
        }

        public string PaymentNotice
        {
            get { return _dbsettings.StringGetSetting("Ticket", "PaymentNotice", "Payment Notice at Bottom", ""); }
            set { _dbsettings.StringSaveSetting("Ticket", "PaymentNotice", value.ToString()); }
        }


        public string SMSUserName
        {
            get { return _dbsettings.StringGetSetting("SMS", "SMSUserName", "SMS UserName", ""); }
            set { _dbsettings.StringSaveSetting("SMS", "SMSUserName", value.ToString()); }
        }
        public string SMSAccountID
        {
            get { return _dbsettings.StringGetSetting("SMS", "SMSAccountID", "SMS Account ID", ""); }
            set { _dbsettings.StringSaveSetting("SMS", "SMSAccountID", value.ToString()); }
        }
        public string SMSEmail
        {
            get { return _dbsettings.StringGetSetting("SMS", "SMSEmail", "SMS Email", ""); }
            set { _dbsettings.StringSaveSetting("SMS", "SMSEmail", value.ToString()); }
        }

   
        public string SMSPassword
        {
            get{ return  _dbsettings.StringGetSetting("SMS", "SMSPassword", "SMS Password", "");}
            set { _dbsettings.StringSaveSetting("SMS", "SMSPassword", value.ToString()); }
        }

   
        public string APIKey
        {
            get{ return _dbsettings.StringGetSetting("SMS", "APIKey", "API Key", "");  }
            set { _dbsettings.StringSaveSetting("SMS", "APIKey", value.ToString()); }
        }


        public string SMSMessage
        {
            get { return _dbsettings.StringGetSetting("SMS", "SMSMessage", "SMS Message", ""); }
            set { _dbsettings.StringSaveSetting("SMS", "SMSMessage", value.ToString()); }
        }

        public string SMSCustomerMessage
        {
            get { return _dbsettings.StringGetSetting("SMS", "SMSCustomerMessage", "SMS Customer Message", ""); }
            set { _dbsettings.StringSaveSetting("SMS", "SMSCustomerMessage", value.ToString()); }
        }


        public string SMSFromPhone
        {
            get
            {
                return _dbsettings.StringGetSetting("SMS", "SMSFromPhone", "SMS From Phone", "");


            }
            set { _dbsettings.StringSaveSetting("SMS", "SMSFromPhone", value.ToString()); }
        }


        public string ExternalPayChoices
        {
            get { return _dbsettings.StringGetSetting("Payment", "ExternalPayChoices", "ExternalPayChoices", "Payment1,Payment2"); }
            set { _dbsettings.StringSaveSetting("Payment", "ExternalPayChoices", value.ToString()); }
        }


        public string RefundChoices
        {
            get { return _dbsettings.StringGetSetting("Credit Card", "RefundChoices", "Refund Card List", "Credit"); }
            set { _dbsettings.StringSaveSetting("Credit Card", "RefundChoices", value.ToString()); }
        }

        public string StoreEmail
        {
            get { return _dbsettings.StringGetSetting("Email", "StoreEmail", "Store Email", ""); }
            set { _dbsettings.StringSaveSetting("Email", "StoreEmail", value.ToString()); }
        }

        public string SMTPServer
        {
            get { return _dbsettings.StringGetSetting("Email", "SMTPServer", "SMTP Server", ""); }
            set { _dbsettings.StringSaveSetting("Email", "SMTPServer", value.ToString()); }
        }

        public string SMTPPassword
        {
            get { return _dbsettings.StringGetSetting("Email", "SMTPPassword", "SMTP Password", ""); }
            set { _dbsettings.StringSaveSetting("Email", "SMTPPassword", value.ToString()); }
        }


        public string SMTPUserName
        {
            get
            {
               return _dbsettings.StringGetSetting("Email", "SMTPUserName", "SMTP UserName", "");
               
            }
            set { _dbsettings.StringSaveSetting("Email", "SMTPUserName", value.ToString()); }
        }

        public string SMTPCopyTo
        {
            get { return _dbsettings.StringGetSetting("Email", "SMTPCopyTo", "SMTP Copy To:", ""); }
            set { _dbsettings.StringSaveSetting("Email", "SMTPCopyTo", value.ToString()); }
        }


        public string DatabaseVersion
        {
            get { return _dbsettings.StringGetSetting("System", "DatabaseVersion", "Database Version", ""); }
            set { _dbsettings.StringSaveSetting("System", "DatabaseVersion", value.ToString()); }
        }



        public string RewardUsageRestriction
        {
            get { return _dbsettings.StringGetSetting("Reward", "RewardUsageRestriction", "Reward Usage Restriction", "none"); }
            set { _dbsettings.StringSaveSetting("Reward", "RewardUsageRestriction", value.ToString()); }
        }


        public string RewardException
        {
            get { return _dbsettings.StringGetSetting("Reward", "RewardException", "Reward Exception", "none"); }
            set { _dbsettings.StringSaveSetting("Reward", "RewardException", value.ToString()); }
        }


        string _databaseserver = "";
        public string DatabaseServer
        {
            get
            {
                if (_databaseserver == "") _databaseserver = Utility.GetINIString("Server", "DataBase", "localhost");
                return _databaseserver;
            }
            set
            {
                _databaseserver = value.ToString();
                Utility.PutINIString("Server", "DataBase", value.ToString());
            }
        }


        string _databasename = "";
        public string DatabaseName
        {
            get
            {
                if (_databasename == "")
                {
                    _databasename = Utility.GetINIString("Database", "DataBase", "diner");
                }
                return _databasename;
            }
            set
            {
                _databasename = value.ToString();
                Utility.PutINIString("Database", "DataBase", value.ToString());
            }
        }

   

 

        public string KitchenPrintDescription
        {
            get { return _dbsettings.StringGetSetting("Ticket", "KitchenPrintDescription", "Kitchen Print Description", "description 1"); }
            set { _dbsettings.StringSaveSetting("Ticket", "KitchenPrintDescription", value.ToString()); }
        }

 

        public string SMSTextPlatform
        {
            get { return _dbsettings.StringGetSetting("SMS", "SMSTextPlatform", "SMS Texting Platform", "Clickatell REST", "list", "Clickatell REST, Clickatell API, Clickatell HTTP,Nexmo API"); }
            set { _dbsettings.StringSaveSetting("SMS", "SMSTextPlatform", value.ToString()); }
        }

        public string CreditCardProcessor
        {
            get { return _dbsettings.StringGetSetting("Credit Card", "CreditCardProcessor", "Credit Card Processor", "External"); }
            set { _dbsettings.StringSaveSetting("Credit Card", "CreditCardProcessor", value.ToString()); }
        }

        public string ActiveMenu
        {
            get { return Utility.GetINIString( "ActiveMenu", "Order Entry", "menu1"); }
            set { Utility.PutINIString("ActiveMenu","Order Entry", value.ToString()); }
        }

        public string ButtonBorderColor
        {
            get { return _dbsettings.StringGetSetting("Screens", "ButtonBorderColor", "Button Border Color", "DarkRed", "color"); }
            set { _dbsettings.StringSaveSetting("Screens", "ButtonBorderColor", value.ToString()); }
        }

        public string OrderScreenName
        {
            get { return _dbsettings.StringGetSetting("Order Entry", "OrderScreenName", "Order Screen Name", "Classic", "list", "Classic, Deluxe, Custom"); }
            set { _dbsettings.StringSaveSetting("Order Entry", "OrderScreenName", value.ToString()); }
        }


    







        #endregion
    }
}
