using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedDot.OrderService.Class
{
    public class GlobalSettings
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



   

        private DBSettings _dbsettings;
        public DBSettings DBSettings { get { return _dbsettings; } }

        public string ApplicationPath { get { return System.AppDomain.CurrentDomain.BaseDirectory; } }
        public void Init()
        {

           
            _dbsettings = new DBSettings();


        }



        public System.Drawing.Rectangle r0 { get; set; }
        public System.Drawing.Rectangle r1 { get; set; }



        public int LastAreaId { get; set; }

        public bool Demo { get; set; }

        public int DemoLeft { get; set; }



        public string[] GetAllUserNames { get; private set; }

        public int[] GetallfingerIDs { get; private set; }

        public string[] GetAllUserPins { get; private set; }
        public int[] GetallPinIDs { get; private set; }


  

        public Object NotificationScreen
        {
            get { return _notificationscreen; }
            set { _notificationscreen = value; }
        }




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
            get { return int.Parse(Utility.GetINIString("LocationId", "System", "1")); }
            set { Utility.PutINIString("LocationId", "System", value.ToString()); }
        }

        public int StationNo
        {
            get { return int.Parse(Utility.GetINIString("StationNo", "System", "1")); }
            set { Utility.PutINIString("StationNo", "System", value.ToString()); }
        }

        public int LaneId
        {
            get { return int.Parse(Utility.GetINIString("LaneId", "CreditCard", "1")); }
            set { Utility.PutINIString("LaneId", "CreditCard", value.ToString()); }

        }

        public int PortNo
        {
            get { return int.Parse(Utility.GetINIString("Port", "DataBase", "3306")); }
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

             ██████╗  ██████╗  ██████╗ ██╗     ███████╗ █████╗ ███╗   ██╗
             ██╔══██╗██╔═══██╗██╔═══██╗██║     ██╔════╝██╔══██╗████╗  ██║
             ██████╔╝██║   ██║██║   ██║██║     █████╗  ███████║██╔██╗ ██║
             ██╔══██╗██║   ██║██║   ██║██║     ██╔══╝  ██╔══██║██║╚██╗██║
             ██████╔╝╚██████╔╝╚██████╔╝███████╗███████╗██║  ██║██║ ╚████║
             ╚═════╝  ╚═════╝  ╚═════╝ ╚══════╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝


             */

        #region Boolean

        public bool AllowServerChangeOpenTabAmount
        {
            get { return _dbsettings.BoolGetSetting("Credit Card", "AllowServerChangeOpenTabAmount", "Allow Server Change Open Tab Amount", "false"); }
            set { _dbsettings.SaveSetting("Credit Card", "AllowServerChangeOpenTabAmount", value.ToString()); }
        }

        public bool MustEnterAllTipBeforeSettlement
        {
            get { return _dbsettings.BoolGetSetting("Credit Card", "MustEnterAllTipBeforeSettlement", "Must Enter All Tip Before Settlement", "true"); }
            set { _dbsettings.SaveSetting("Credit Card", "MustEnterAllTipBeforeSettlement", value.ToString()); }
        }

        public bool AllowDuplicates
        {
            get { return _dbsettings.BoolGetSetting("Credit Card", "AllowDuplicates", "Allow Duplicate Swipe", "false"); }
            set { _dbsettings.SaveSetting("Credit Card", "AllowDuplicates", value.ToString()); }
        }

        public bool AllowCashBack
        {
            get { return _dbsettings.BoolGetSetting("Credit Card", "AllowCashBack", "Allow Cash Back", "false"); }
            set { _dbsettings.SaveSetting("Credit Card", "AllowCashBack", value.ToString()); }
        }


        public bool PrintCustomerCopy
        {
            get { return _dbsettings.BoolGetSetting("Credit Card", "PrintCustomerCopy", "Print Customer Copy", "true"); }
            set { _dbsettings.SaveSetting("Credit Card", "PrintCustomerCopy", value.ToString()); }
        }

        public bool RequestTip
        {
            get { return _dbsettings.BoolGetSetting("Credit Card", "RequestTip", "Request Tip from Customer", "true"); }
            set { _dbsettings.SaveSetting("Credit Card", "RequestTip", value.ToString()); }
        }
        public bool SignatureOnScreen
        {
            get { return _dbsettings.BoolGetSetting("Credit Card", "SignatureOnScreen", "Signature On Screen", "false"); }
            set { _dbsettings.SaveSetting("Credit Card", "SignatureOnScreen", value.ToString()); }
        }
        public bool AuthSigOnPaper
        {
            get { return _dbsettings.BoolGetSetting("Credit Card", "AuthSigOnPaper", "Signature on Paper for Credit Authorize", "true"); }
            set { _dbsettings.SaveSetting("Credit Card", "AuthSigOnPaper", value.ToString()); }
        }
        public bool PrintTipOnReceipt
        {
            get { return _dbsettings.BoolGetSetting("Receipt", "PrintTipOnReceipt", "Print Tip On Receipt", "true"); }
            set { _dbsettings.SaveSetting("Receipt", "PrintTipOnReceipt", value.ToString()); }
        }

        public bool PrintTipGuide
        {
            get { return _dbsettings.BoolGetSetting("Credit Card", "PrintTipGuide", "Print Tip Guide", "false"); }
            set { _dbsettings.SaveSetting("Credit Card", "PrintTipGuide", value.ToString()); }
        }

        public bool EnableCreditSale
        {
            get { return _dbsettings.BoolGetSetting("Credit Card", "EnableCreditSale", "Enable Credit Sale", "true"); }
            set { _dbsettings.SaveSetting("Credit Card", "EnableCreditSale", value.ToString()); }
        }


        public bool EnableDebitSale
        {
            get { return _dbsettings.BoolGetSetting("Credit Card", "EnableDebitSale", "Enable Debit Sale", "true"); }
            set { _dbsettings.SaveSetting("Credit Card", "EnableDebitSale", value.ToString()); }
        }

        public bool EnableAuthCapture
        {
            get { return _dbsettings.BoolGetSetting("Credit Card", "EnableAuthCapture", "Enable Authorize/Capture", "true"); }
            set { _dbsettings.SaveSetting("Credit Card", "EnableAuthCapture", value.ToString()); }
        }

        public bool RequireSignatureOnAllCreditSale
        {
            get { return _dbsettings.BoolGetSetting("Credit Card", "RequireSignatureOnAllCreditSale", "Require Signature On All Credit Sale", "false"); }
            set { _dbsettings.SaveSetting("Credit Card", "RequireSignatureOnAllCreditSale", value.ToString()); }
        }




        public bool AllowLegacyGiftCertificate
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "AllowLegacyGiftCertificate", "Allow Legacy Gift Certificate", "false"); }
            set { _dbsettings.SaveSetting("Ticket", "AllowLegacyGiftCertificate", value.ToString()); }
        }

        public bool ShowCustomerScreen
        {
            get { return _dbsettings.BoolGetSetting("Application", "ShowCustomerScreen", "Show Customer Screen", "true"); }
            set { _dbsettings.BoolSaveSetting("Application", "ShowCustomerScreen", value); }
        }



        public bool ShowCancelledTickets
        {
            get { return _dbsettings.BoolGetSetting("Application", "ShowCancelledTickets", "Show Cancelled Tickets", "true"); }
            set { _dbsettings.BoolSaveSetting("Application", "ShowCancelledTickets", value); }
        }

        public bool EnableVirtualKeyboard
        {
            get { return _dbsettings.BoolGetSetting("Application", "EnableVirtualKeyboard", "Enable Virtual Keyboard", "true"); }
            set { _dbsettings.BoolSaveSetting("Application", "EnableVirtualKeyboard", value); }
        }


        public bool ShowRewardButton
        {
            get { return _dbsettings.BoolGetSetting("Application", "ShowRewardButton", "Show Reward Button", "false"); }
            set { _dbsettings.SaveSetting("Application", "ShowRewardButton", value.ToString()); }
        }

        public bool ShowStampCardButton
        {
            get { return _dbsettings.BoolGetSetting("Application", "ShowStampCardButton", "Show Stamp Card Button", "false"); }
            set { _dbsettings.SaveSetting("Application", "ShowStampCardButton", value.ToString()); }
        }

        public bool ShowCheckButton
        {
            get { return _dbsettings.BoolGetSetting("Application", "ShowCheckButton", "Show Check Button", "false"); }
            set { _dbsettings.SaveSetting("Application", "ShowCheckButton", value.ToString()); }
        }

        public bool EditCustomerProfileOnCreate
        {
            get { return _dbsettings.BoolGetSetting("Application", "EditCustomerProfileOnCreate", "Edit Customer Profile On Create", "false"); }
            set { _dbsettings.SaveSetting("Application", "EditCustomerProfileOnCreate", value.ToString()); }
        }


        public bool AutoHideClosedTicket
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "AutoHideClosedTicket", "Auto Hide Closed Ticket", "false"); }
            set { _dbsettings.SaveSetting("Ticket", "AutoHideClosedTicket", value.ToString()); }
        }

        public bool PrintBarCode
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "PrintBarCode", "Print BarCode on Receipt", "false"); }
            set { _dbsettings.SaveSetting("Ticket", "PrintBarCode", value.ToString()); }
        }



        private bool _homeclicked;
        public bool HomeClicked
        {
            get { return _homeclicked; }
            set { _homeclicked = value; }
        }


        public bool FingerPrintRequiredForTimeCard
        {
            get { return _dbsettings.BoolGetSetting("Application", "FingerPrintRequiredForTimeCard", "FingerPrint Required For TimeCard", "false"); }
            set { _dbsettings.BoolSaveSetting("Application", "FingerPrintRequiredForTimeCard", value); }
        }


        public bool OpenDrawOnClose
        {
            get { return _dbsettings.BoolGetSetting("Application", "OpenDrawOnClose", "Open Cash Drawer Automatically", "true"); }
            set { _dbsettings.BoolSaveSetting("Application", "OpenDrawOnClose", value); }
        }

        public bool ReceiptPrintReward
        {
            get { return _dbsettings.BoolGetSetting("Reward", "ReceiptPrintReward", "Print Reward on Receipt", "true"); }
            set { _dbsettings.BoolSaveSetting("Reward", "ReceiptPrintReward", value); }
        }

        public bool DisplayRewardAlert
        {
            get { return _dbsettings.BoolGetSetting("Reward", "DisplayRewardAlert", "Display Customer Reward Alert", "true"); }
            set { _dbsettings.BoolSaveSetting("Reward", "DisplayRewardAlert", value); }
        }






        /***
         *    ██████╗  ██████╗  ██████╗ ██╗     ███████╗ █████╗ ███╗   ██╗       ██████╗ ██████╗ ██████╗ ███████╗██████╗     ███████╗███╗   ██╗████████╗██████╗ ██╗   ██╗
         *    ██╔══██╗██╔═══██╗██╔═══██╗██║     ██╔════╝██╔══██╗████╗  ██║      ██╔═══██╗██╔══██╗██╔══██╗██╔════╝██╔══██╗    ██╔════╝████╗  ██║╚══██╔══╝██╔══██╗╚██╗ ██╔╝
         *    ██████╔╝██║   ██║██║   ██║██║     █████╗  ███████║██╔██╗ ██║█████╗██║   ██║██████╔╝██║  ██║█████╗  ██████╔╝    █████╗  ██╔██╗ ██║   ██║   ██████╔╝ ╚████╔╝ 
         *    ██╔══██╗██║   ██║██║   ██║██║     ██╔══╝  ██╔══██║██║╚██╗██║╚════╝██║   ██║██╔══██╗██║  ██║██╔══╝  ██╔══██╗    ██╔══╝  ██║╚██╗██║   ██║   ██╔══██╗  ╚██╔╝  
         *    ██████╔╝╚██████╔╝╚██████╔╝███████╗███████╗██║  ██║██║ ╚████║      ╚██████╔╝██║  ██║██████╔╝███████╗██║  ██║    ███████╗██║ ╚████║   ██║   ██║  ██║   ██║   
         *    ╚═════╝  ╚═════╝  ╚═════╝ ╚══════╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝       ╚═════╝ ╚═╝  ╚═╝╚═════╝ ╚══════╝╚═╝  ╚═╝    ╚══════╝╚═╝  ╚═══╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   
         *                                                                                                                                                               
         */


        public bool ClockInRequired
        {
            get { return _dbsettings.BoolGetSetting("Order Entry", "ClockInRequired", "ClockIn Required", "True"); }
            set { _dbsettings.SaveSetting("Order Entry", "ClockInRequired", value.ToString()); }
        }


        public bool DisplayPriceOnButton
        {
            get { return _dbsettings.BoolGetSetting("Order Entry", "DisplayPriceOnButton", "Display Price On Button", "False"); }
            set { _dbsettings.SaveSetting("Order Entry", "DisplayPriceOnButton", value.ToString()); }
        }



        public bool EnableToGoOrders
        {
            get { return _dbsettings.BoolGetSetting("To Go", "EnableToGoOrders", "Enable To Go Orders", "true"); }
            set { _dbsettings.SaveSetting("To Go", "EnableToGoOrders", value.ToString()); }
        }

        public bool EnableBar
        {
            get { return _dbsettings.BoolGetSetting("Bar", "EnableBar", "Enable Bar Orders", "true"); }
            set { _dbsettings.SaveSetting("Bar", "EnableBar", value.ToString()); }
        }

        public bool EnableDineIn
        {
            get { return _dbsettings.BoolGetSetting("Dine In", "EnableDineIn", "Enable Dine In", "true"); }
            set { _dbsettings.SaveSetting("Dine In", "EnableDineIn", value.ToString()); }
        }

        public bool EnableDelivery
        {
            get { return _dbsettings.BoolGetSetting("Delivery", "EnableDelivery", "Enable Delivery", "true"); }
            set { _dbsettings.SaveSetting("Delivery", "EnableDelivery", value.ToString()); }
        }

        public bool EnableECR
        {
            get { return _dbsettings.BoolGetSetting("Order Entry", "EnableECR", "Enable Electronic Cash Register", "true"); }
            set { _dbsettings.SaveSetting("Order Entry", "EnableECR", value.ToString()); }
        }

        public bool EnableCounterService
        {
            get { return _dbsettings.BoolGetSetting("Counter Service", "EnableCounterService", "Enable Counter Service", "true"); }
            set { _dbsettings.SaveSetting("Counter Service", "EnableCounterService", value.ToString()); }
        }

        public bool AskCustomerCount
        {
            get { return _dbsettings.BoolGetSetting("Order Entry", "AskCustomerCount", "Ask Customer Count", "false"); }
            set { _dbsettings.SaveSetting("Order Entry", "AskCustomerCount", value.ToString()); }
        }





        /***
         *    ██████╗  ██████╗  ██████╗ ██╗     ███████╗ █████╗ ███╗   ██╗   ████████╗██╗ ██████╗██╗  ██╗███████╗████████╗
         *    ██╔══██╗██╔═══██╗██╔═══██╗██║     ██╔════╝██╔══██╗████╗  ██║   ╚══██╔══╝██║██╔════╝██║ ██╔╝██╔════╝╚══██╔══╝
         *    ██████╔╝██║   ██║██║   ██║██║     █████╗  ███████║██╔██╗ ██║█████╗██║   ██║██║     █████╔╝ █████╗     ██║   
         *    ██╔══██╗██║   ██║██║   ██║██║     ██╔══╝  ██╔══██║██║╚██╗██║╚════╝██║   ██║██║     ██╔═██╗ ██╔══╝     ██║   
         *    ██████╔╝╚██████╔╝╚██████╔╝███████╗███████╗██║  ██║██║ ╚████║      ██║   ██║╚██████╗██║  ██╗███████╗   ██║   
         *    ╚═════╝  ╚═════╝  ╚═════╝ ╚══════╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝      ╚═╝   ╚═╝ ╚═════╝╚═╝  ╚═╝╚══════╝   ╚═╝   
         *                                                                                                                
         */

        public bool AutoReceiptPrint
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "AutoReceiptPrint", "Print Receipt On Ticket Close", "true"); }
            set { _dbsettings.BoolSaveSetting("Ticket", "AutoReceiptPrint", value); }
        }

        public bool AutoPackagingReceiptPrint
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "AutoPackagingReceiptPrint", "Print Packaging Receipt", "true"); }
            set { _dbsettings.BoolSaveSetting("Ticket", "AutoPackagingReceiptPrint", value); }
        }

        public bool PrintStoreOnPackagingReceipt
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "PrintStoreOnPackagingReceipt", "Print Store On Packaging Receipt", "true"); }
            set { _dbsettings.BoolSaveSetting("Ticket", "PrintStoreOnPackagingReceipt", value); }
        }


        //ask customer
        public bool DineInAskCustomerPhone
        {
            get { return _dbsettings.BoolGetSetting("Dine In", "AskCustomerPhone", "Ask For Customer Info", "false"); }
            set { _dbsettings.BoolSaveSetting("Dine In", "AskCustomerPhone", value); }
        }

        public bool DineInAskForCustomerName
        {
            get { return _dbsettings.BoolGetSetting("Dine In", "AskForCustomerName", "Ask For Name", "False"); }
            set { _dbsettings.SaveSetting("Dine In", "AskForCustomerName", value.ToString()); }
        }


        public bool CallInAskCustomerPhone
        {
            get { return _dbsettings.BoolGetSetting("Call In", "AskCustomerPhone", "Ask For Customer Info", "false"); }
            set { _dbsettings.BoolSaveSetting("Call In", "AskCustomerPhone", value); }
        }
        public bool CallInCustomerInfoRequired
        {
            get { return _dbsettings.BoolGetSetting("Call In", "CustomerInfoRequired", "Customer Info Required", "false"); }
            set { _dbsettings.BoolSaveSetting("Call In", "CustomerInfoRequired", value); }
        }



        public bool CallInAskForCustomerName
        {
            get { return _dbsettings.BoolGetSetting("Call In", "AskForCustomerName", "Ask For Name", "False"); }
            set { _dbsettings.SaveSetting("Call In", "AskForCustomerName", value.ToString()); }
        }



        public bool WalkInAskCustomerPhone
        {
            get { return _dbsettings.BoolGetSetting("Walk In", "AskCustomerPhone", "Ask For Customer Info", "false"); }
            set { _dbsettings.BoolSaveSetting("Walk In", "AskCustomerPhone", value); }
        }

        public bool WalkInAskForCustomerName
        {
            get { return _dbsettings.BoolGetSetting("Walk In", "AskForCustomerName", "Ask For Name", "False"); }
            set { _dbsettings.SaveSetting("Walk In", "AskForCustomerName", value.ToString()); }
        }

        public bool BarAskCustomerPhone
        {
            get { return _dbsettings.BoolGetSetting("Bar", "AskCustomerPhone", "Ask For Customer Info", "false"); }
            set { _dbsettings.BoolSaveSetting("Bar", "AskCustomerPhone", value); }
        }

        public bool BarAskForCustomerName
        {
            get { return _dbsettings.BoolGetSetting("Bar", "AskForCustomerName", "Ask For Name", "False"); }
            set { _dbsettings.SaveSetting("Bar", "AskForCustomerName", value.ToString()); }
        }

        public bool DeliveryAskCustomerPhone
        {
            get { return _dbsettings.BoolGetSetting("Delivery", "AskCustomerPhone", "Ask For Customer Info", "false"); }
            set { _dbsettings.BoolSaveSetting("Delivery", "AskCustomerPhone", value); }
        }

        public bool DeliveryAskForCustomerName
        {
            get { return _dbsettings.BoolGetSetting("Delivery", "AskForCustomerName", "Ask For Name", "False"); }
            set { _dbsettings.SaveSetting("Delivery", "AskForCustomerName", value.ToString()); }
        }
        //----------------------------------------------------


        public bool AllowCashBackGiftCertificate
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "AllowCashBackGiftCertificate", "Allow CashBack Gift Certificate", "false"); }
            set { _dbsettings.BoolSaveSetting("Ticket", "AllowCashBackGiftCertificate", value); }
        }

        public bool AllowCashBackGiftCard
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "AllowCashBackGiftCard", "Allow CashBack Gift Card", "false"); }
            set { _dbsettings.BoolSaveSetting("Ticket", "AllowCashBackGiftCard", value); }
        }



        public bool ReceiptShowModifierPrice
        {
            get { return _dbsettings.BoolGetSetting("Receipt", "ReceiptShowModifierPrice", "Show Modifier Price on Receipt", "false"); }
            set { _dbsettings.SaveSetting("Receipt", "ReceiptShowModifierPrice", value.ToString()); }
        }
        public bool DisplayShowModifierPrice
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "DisplayShowModifierPrice", "Show Modifier Price on Screen", "False"); }
            set { _dbsettings.SaveSetting("Ticket", "DisplayShowModifierPrice", value.ToString()); }
        }


        public bool AutoCloseModifierWhenFull
        {
            get { return _dbsettings.BoolGetSetting("Order Entry", "AutoCloseModifierWhenFull", "Auto Close Modifier When Full", "true"); }
            set { _dbsettings.SaveSetting("Order Entry", "AutoCloseModifierWhenFull", value.ToString()); }
        }

        public bool AutoCloseComboWhenFull
        {
            get { return _dbsettings.BoolGetSetting("Order Entry", "AutoCloseComboWhenFull", "Auto Close Combo When Full", "true"); }
            set { _dbsettings.SaveSetting("Order Entry", "AutoCloseComboWhenFull", value.ToString()); }
        }


        public bool ReceiptPrintEmployee
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "ReceiptPrintEmployee", "Print Server Name On Receipt", "true"); }
            set { _dbsettings.SaveSetting("Ticket", "ReceiptPrintEmployee", value.ToString()); }
        }

        public bool EnableTicketCollaberation
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "EnableTicketCollaberation", "Enable Ticket Collaberation", "false"); }
            set { _dbsettings.SaveSetting("Ticket", "EnableTicketCollaberation", value.ToString()); }
        }



        public bool ProductShowDiscount
        {
            get { return _dbsettings.BoolGetSetting("Application", "ProductShowDiscount", "Show Discount on Item Button", "false"); }
            set { _dbsettings.BoolSaveSetting("Application", "ProductShowDiscount", value); }
        }


        public bool KitchenPrintDoubleHeight
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "KitchenPrintDoubleHeight", "Kitchen Print Double Height", "true"); }
            set { _dbsettings.SaveSetting("Ticket", "KitchenPrintDoubleHeight", value.ToString()); }
        }

        public bool KitchenPrintDoubleWidth
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "KitchenPrintDoubleWidth", "Kitchen Print Double Height", "false"); }
            set { _dbsettings.SaveSetting("Ticket", "KitchenPrintDoubleWidth", value.ToString()); }
        }

        public bool KitchenPrintDoubleImpact
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "KitchenPrintDoubleImpact", "Kitchen Print Double Impact", "false"); }
            set { _dbsettings.SaveSetting("Ticket", "KitchenPrintDoubleImpact", value.ToString()); }
        }


        public bool AutoSendMessageToCustomer
        {
            get { return _dbsettings.BoolGetSetting("SMS", "AutoSendMessageToCustomer", "Auto Send Message To Customer", "false"); }
            set { _dbsettings.BoolSaveSetting("SMS", "AutoSendMessageToCustomer", value); }
        }

        public bool AutoSendReceiptToCustomer
        {
            get { return _dbsettings.BoolGetSetting("SMS", "AutoSendReceiptToCustomer", "Auto Send Receipt To Customer", "false"); }
            set { _dbsettings.BoolSaveSetting("SMS", "AutoSendReceiptToCustomer", value); }
        }



        public bool CallInMustPayFirst
        {
            get { return _dbsettings.BoolGetSetting("Call In", "MustPayFirst", "Must Pay First Before Firing", "false"); }
            set { _dbsettings.SaveSetting("Call In", "MustPayFirst", value.ToString()); }
        }

        public bool WalkInMustPayFirst
        {
            get { return _dbsettings.BoolGetSetting("Walk In", "MustPayFirst", "Must Pay First Before Firing", "false"); }
            set { _dbsettings.SaveSetting("Walk In", "MustPayFirst", value.ToString()); }
        }

        public bool CounterServiceMustPayFirst
        {
            get { return _dbsettings.BoolGetSetting("Counter Service", "MustPayFirst", "Must Pay First Before Firing", "true"); }
            set { _dbsettings.SaveSetting("Counter Service", "MustPayFirst", value.ToString()); }
        }

        public bool DeliveryMustPayFirst
        {
            get { return _dbsettings.BoolGetSetting("Delivery", "MustPayFirst", "Must Pay First Before Firing", "false"); }
            set { _dbsettings.SaveSetting("Delivery", "MustPayFirst", value.ToString()); }
        }

        public bool BarMustPayFirst
        {
            get { return _dbsettings.BoolGetSetting("Bar", "MustPayFirst", "Must Pay First Before Firing", "false"); }
            set { _dbsettings.SaveSetting("Bar", "MustPayFirst", value.ToString()); }
        }


        public bool UseTipPooling
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "UseTipPooling", "Use Tip Pooling", "false"); }
            set { _dbsettings.SaveSetting("Ticket", "UseTipPooling", value.ToString()); }
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

        public string DisplayComPort
        {
            get { return Utility.GetINIString("DisplayComPort", "System", "COM2"); }
            set { Utility.PutINIString("DisplayComPort", "System", value.ToString()); }
        }



        private string m_versionnumber;
        public string VersionNumber
        {
            get { return m_versionnumber; }
            set { m_versionnumber = value; }
        }

        public string SIPDefaultIPAddress
        {
            get { return Utility.GetINIString("SIPDefaultIPAddress", "CreditCard", "localhost"); }
            set { Utility.PutINIString("SIPDefaultIPAddress", "CreditCard", value.ToString()); }
        }

        public string SIPPort
        {
            get { return Utility.GetINIString("SIPPort", "CreditCard", "8080"); } //10009 for PAXS300
            set { Utility.PutINIString("SIPPort", "CreditCard", value.ToString()); }
        }



        public string LanguageCode
        {
            get { return _dbsettings.StringGetSetting("System", "LanguageCode", "Language Code", "en-US"); }
            set { _dbsettings.StringSaveSetting("System", "LanguageCode", value.ToString()); }
        }

        public string BackupDirectory
        {
            get { return _dbsettings.StringGetSetting("Application", "BackupDirectory", "Backup Directory", ""); }
            set { _dbsettings.StringSaveSetting("Application", "BackupDirectory", value.ToString()); }
        }

        public string PackagingPrinter
        {
            get { return _dbsettings.StringGetSetting("Application", "PackagingPrinter", "Packaging Printer", "", "printer"); }
            set { _dbsettings.StringSaveSetting("Application", "PackagingPrinter", value.ToString()); }
        }

        public string ReceiptPrinter
        {
            get { return Utility.GetINIString("ReceiptPrinter", "Printer", ""); }
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
            get { return Utility.GetINIString("ReportPrinter", "Printer", ""); }
            set { Utility.PutINIString("ReportPrinter", "Printer", value.ToString()); }
        }





        public string MainBackgroundImage
        {
            get { return _dbsettings.StringGetSetting("Application", "MainBackgroundImage", "Main Background Image", "", "image"); }
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
            { _dbsettings.StringSaveSetting("Application", "StoreLogo", value.ToString()); }
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
            get { return _dbsettings.StringGetSetting("SMS", "SMSPassword", "SMS Password", ""); }
            set { _dbsettings.StringSaveSetting("SMS", "SMSPassword", value.ToString()); }
        }


        public string APIKey
        {
            get { return _dbsettings.StringGetSetting("SMS", "APIKey", "API Key", ""); }
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
            get { return Utility.GetINIString("ActiveMenu", "Order Entry", "menu1"); }
            set { Utility.PutINIString("ActiveMenu", "Order Entry", value.ToString()); }
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


        public string ElementExpressURL
        {
            get { return _dbsettings.StringGetSetting("Credit Card", "ElementExpressURL", "Element Express URL", "https://transaction.elementexpress.com/"); }
            set { _dbsettings.StringSaveSetting("Credit Card", "ElementExpressURL", value.ToString()); }
        }







        #endregion
    }
}