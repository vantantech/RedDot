using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Windows;
using RedDot.DataManager;
using com.clover.remotepay.sdk;
using Clover;
using System.Data;


namespace RedDot
{
    public class GlobalSettings
    {
        private  static GlobalSettings _Instance;
     
   
        private Object _remotescreen;
        private Object _notificationscreen;

        public static GlobalSettings Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new GlobalSettings();
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





        public string[] GetAllUserNames { get; private set; }

        public int[] GetallfingerIDs { get; private set; }

        public string[] GetAllUserPins { get; private set; }
        public int[] GetallPinIDs { get; private set; }

        public IDataInterface RedDotData
        {
            get { return _dbsettings; }
        }

        private IDataInterface _dbsettings;

        int selection = 0;


      

        public void Init()
        {
  
           Shop = new Location();
        

        
         
                switch (selection)
                {
                    case 0:
                    _dbsettings = (IDataInterface)new MySQLData();
                        break;
                    case 1:
                      //  m_selectedData = (IDataInterface)new SQLiteData();
                        break;
                    default:
                    _dbsettings = (IDataInterface)new MySQLData();
                        break;
                }

          

        }

        public Location Shop { get; set; }

        public string MachineID { get; set; }
        public System.Drawing.Rectangle r0 { get; set; }
        public System.Drawing.Rectangle r1 { get; set; }


        public DateTime LastLogon
        {
            get { return _dbsettings.GetDateSetting("System", "LastLogOn", "Last Log On", DateTime.Now.ToShortDateString()); }
            set { _dbsettings.SaveSetting("LastLogOn", value.ToString()); }
        }


        public Object RemoteScreen {
            get { return _remotescreen; }
            set { _remotescreen = value; }
        }

        public Object NotificationScreen
        {
            get { return _notificationscreen; }
            set { _notificationscreen = value; }
        }


        public ICloverConnector cloverConnector { get; set; }
        public CloverListener ccl { get; set; }

        public Visibility ProVersion { get; set; }

        public Visibility BaseVersion { get; set; }


        public bool Demo { get; set; }
        public int DemoLeft { get; set; }



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
            set { _dbsettings.SaveSetting("PayPeriodStartDate", value.ToString()); }
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


        int _portno = 0;
        public int PortNo
        {
            get { if(_portno==0) _portno =int.Parse(Utility.GetINIString("Port", "DataBase","3306"));
            return _portno;
            }
            set {
                _portno = value;
                Utility.PutINIString("Port", "DataBase", value.ToString());
            }
        }



        public int SMTPPort
        {
            get { return _dbsettings.IntGetSetting("Email", "SMTPPort", "SMTP Port", "25"); }
            set { _dbsettings.IntSaveSetting("SMTPPort", value); }
        }

   

        public int Tier1
        {
            get { return _dbsettings.IntGetSetting("Reward","Tier1","Tier 1 Min Referral","10"); }
            set { _dbsettings.IntSaveSetting("Tier1", value); }
        }
        public int Tier2
        {
            get { return _dbsettings.IntGetSetting("Reward", "Tier2", "Tier 2 Min Referral", "20"); }
            set { _dbsettings.IntSaveSetting("Tier2", value); }
        }

        public int Tier3
        {
            get { return _dbsettings.IntGetSetting("Reward", "Tier3", "Tier 3 Min Referral", "30"); }
            set { _dbsettings.IntSaveSetting("Tier3", value); }
        }

 

        public int ReceiptWidth
        {
            get { return _dbsettings.IntGetSetting("Ticket","ReceiptWidth","Receipt Paper Width","80"); }
            set { _dbsettings.IntSaveSetting("ReceiptWidth", value); }
        }
   

        public int PinLength
        {
            get { return _dbsettings.IntGetSetting("Application","PinLength","PIN Length","4"); }
            set { _dbsettings.IntSaveSetting("PinLength", value); }
        }



        public int DayStartHour
        {
            get { return _dbsettings.IntGetSetting("Application","DayStartHour","Appointment Start Hour","9"); }
            set { _dbsettings.IntSaveSetting("DayStartHour", value); }
        }

        public int DayLength
        {
            get { return _dbsettings.IntGetSetting("Application", "DayLength","Appointment Day Length","10"); }
            set { _dbsettings.IntSaveSetting("DayLength", value); }
        }

        public int AppointmentInterval
        {
            get { return _dbsettings.IntGetSetting("Application", "AppointmentInterval", "Appointment Interval","15"); }
            set { _dbsettings.IntSaveSetting("AppointmentInterval", value); }
        }


        public int CategoryWidth
        {
            get { return _dbsettings.IntGetSetting("Application", "CategoryWidth","Category Box Width","160"); }
            set { _dbsettings.IntSaveSetting("CategoryWidth", value); }
        }

        public int CategoryHeight
        {
            get { return _dbsettings.IntGetSetting("Application", "CategoryHeight","Category Box Height","70"); }
            set { _dbsettings.IntSaveSetting("CategoryHeight", value); }
        }


        public int ProductWidth
        {
            get { return _dbsettings.IntGetSetting("Application", "ProductWidth","Product Box Width","160"); }
            set { _dbsettings.IntSaveSetting("ProductWidth", value); }
        }

        public int ProductHeight
        {
            get { return _dbsettings.IntGetSetting("Application", "ProductHeight", "Product Box Height","60"); }
            set { _dbsettings.IntSaveSetting("ProductHeight", value); }
        }

        public int ProductFontSize
        {
            get { return _dbsettings.IntGetSetting("Application", "ProductFontSize","Product Box Font Size","14"); }
            set { _dbsettings.IntSaveSetting("ProductFontSize", value); }
        }

        public int CategoryFontSize
        {
            get { return _dbsettings.IntGetSetting("Application", "CategoryFontSize","Category Box Font Size","18"); }
            set { _dbsettings.IntSaveSetting("CategoryFontSize", value); }
        }

        public int WebUserID
        {
            get { return _dbsettings.IntGetSetting("Webservice", "WebUserID", "Web User ID", "0"); }
            set { _dbsettings.IntSaveSetting("WebUserID", value); }
        }

        public int WebSyncCheckInterval
        {
            get { return _dbsettings.IntGetSetting("Webservice", "WebSyncCheckInterval", "Web Sync Check Interval", "5"); }
            set { _dbsettings.IntSaveSetting("WebSyncCheckInterval", value); }
        }




        /***
        *    ██████╗ ███████╗ ██████╗██╗███╗   ███╗ █████╗ ██╗     
        *    ██╔══██╗██╔════╝██╔════╝██║████╗ ████║██╔══██╗██║     
        *    ██║  ██║█████╗  ██║     ██║██╔████╔██║███████║██║     
        *    ██║  ██║██╔══╝  ██║     ██║██║╚██╔╝██║██╔══██║██║     
        *    ██████╔╝███████╗╚██████╗██║██║ ╚═╝ ██║██║  ██║███████╗
        *    ╚═════╝ ╚══════╝ ╚═════╝╚═╝╚═╝     ╚═╝╚═╝  ╚═╝╚══════╝
        *                                                          
        */


        /// <summary>
        /// Decimal Type settings
        /// </summary>


        public decimal TipAlertAmount
        {
            get { return _dbsettings.DecimalGetSetting("CreditCard", "TipAlertAmount", "Tip Alert Amount", "30"); }
            set { _dbsettings.DecimalSaveSetting("TipAlertAmount", value); }
        }


        public decimal Tier0Reward
        {
            get { return _dbsettings.DecimalGetSetting("Reward","Tier0Reward","Tier 0 Reward Amount","1"); }
            set { _dbsettings.DecimalSaveSetting("Tier0Reward", value); }
        }

        public decimal Tier1Reward
        {
            get { return _dbsettings.DecimalGetSetting("Reward", "Tier1Reward", "Tier 1 Reward Amount", "1.5"); }
            set { _dbsettings.DecimalSaveSetting("Tier1Reward", value); }
        }

        public decimal Tier2Reward
        {
            get { return _dbsettings.DecimalGetSetting("Reward", "Tier2Reward", "Tier 2 Reward Amount", "2"); }
            set { _dbsettings.DecimalSaveSetting("Tier2Reward", value); }
        }
        public decimal Tier3Reward
        {
            get { return _dbsettings.DecimalGetSetting("Reward", "Tier3Reward", "Tier 3 Reward Amount", "2.5"); }
            set { _dbsettings.DecimalSaveSetting("Tier3Reward", value); }
        }

        public decimal RewardPercent
        {
            get { return _dbsettings.DecimalGetSetting("Reward","RewardPercent","Reward Percent","5"); }
            set { _dbsettings.DecimalSaveSetting("RewardPercent", value); }
        }

        public decimal CashRewardPercent
        {
            get { return _dbsettings.DecimalGetSetting("Reward", "CashRewardPercent", "Cash Reward Percent", "5"); }
            set { _dbsettings.DecimalSaveSetting("CashRewardPercent", value); }
        }

        public decimal MinReward
        {
            get { return _dbsettings.DecimalGetSetting("Reward","MinReward","Minimum Reward Usage Amount","15"); }
            set { _dbsettings.DecimalSaveSetting("MinReward", value); }
        }
        public decimal MaxReward
        {
            get { return _dbsettings.DecimalGetSetting("Reward","MaxReward","Maximum Reward Usage Amount","20"); }
            set { _dbsettings.DecimalSaveSetting("MaxReward", value); }
        }
        public decimal SalesTaxPercent
        {
            get { return _dbsettings.DecimalGetSetting("Application","SalesTaxPercent","Sales Tax Percent","8.25"); }
            set { _dbsettings.DecimalSaveSetting("SalesTaxPercent", value); }
        }

        public decimal MinimumTurnAmount
        {
            get { return _dbsettings.DecimalGetSetting("Application","MinimumTurnAmount","Minimum Turn Amount","20"); }
            set { _dbsettings.DecimalSaveSetting("MinimumTurnAmount", value); }
        }

        public decimal CreditCardFeePercent
        {
            get { return _dbsettings.DecimalGetSetting("Application","CreditCardFeePercent","% Deduct from Credit Card Tips","3"); }
            set { _dbsettings.DecimalSaveSetting("CreditCardFeePercent", value); }
        }
   





        /*

                ██████╗  ██████╗  ██████╗ ██╗     ███████╗ █████╗ ███╗   ██╗
                ██╔══██╗██╔═══██╗██╔═══██╗██║     ██╔════╝██╔══██╗████╗  ██║
                ██████╔╝██║   ██║██║   ██║██║     █████╗  ███████║██╔██╗ ██║
                ██╔══██╗██║   ██║██║   ██║██║     ██╔══╝  ██╔══██║██║╚██╗██║
                ██████╔╝╚██████╔╝╚██████╔╝███████╗███████╗██║  ██║██║ ╚████║
                ╚═════╝  ╚═════╝  ╚═════╝ ╚══════╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝


                */


        #region Boolean


        private bool _homeclicked;
        public bool HomeClicked
        {
            get { return _homeclicked; }
            set { _homeclicked = value; }
        }


        public bool EditCustomerProfileOnCreate
        {
            get { return _dbsettings.BoolGetSetting("Application", "EditCustomerProfileOnCreate", "Edit Customer Profile On Create", "false"); }
            set { _dbsettings.BoolSaveSetting( "EditCustomerProfileOnCreate", value); }
        }

        public bool MustEnterAllTipBeforeSettlement
        {
            get { return _dbsettings.BoolGetSetting("CreditCard", "MustEnterAllTipBeforeSettlement", "Must Enter All Tip Before Settlement", "true"); }
            set { _dbsettings.BoolSaveSetting("MustEnterAllTipBeforeSettlement", value); }
        }

        public bool AllowDuplicates
        {
            get { return _dbsettings.BoolGetSetting("CreditCard", "AllowDuplicates", "Allow Duplicate Swipe", "true"); }
            set { _dbsettings.BoolSaveSetting("AllowDuplicates", value); }
        }

        public bool AllowCashBack
        {
            get { return _dbsettings.BoolGetSetting("CreditCard", "AllowCashBack", "Allow Cash Back", "false"); }
            set { _dbsettings.BoolSaveSetting("AllowCashBack", value); }
        }

        public bool AllowTipOnCashSales
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "AllowTipOnCashSales", "Allow Tip On Cash Sales", "false"); }
            set { _dbsettings.SaveSetting("AllowTipOnCashSales", value.ToString()); }
        }


        public bool PrintCreditSlipOnClose
        {
            get { return _dbsettings.BoolGetSetting("CreditCard", "PrintCreditSlipOnClose", "Print Credit Slip On Close", "true"); }
            set { _dbsettings.BoolSaveSetting("PrintCreditSlipOnClose", value); }
        }



        public bool PrintCustomerCopy
        {
            get { return _dbsettings.BoolGetSetting("CreditCard", "PrintCustomerCopy", "Print Customer Copy", "true"); }
            set { _dbsettings.BoolSaveSetting("PrintCustomerCopy", value); }
        }

        public bool PrintTipOnReceipt
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "PrintTipOnReceipt", "Print Tip On Receipt", "false"); }
            set { _dbsettings.BoolSaveSetting("PrintTipOnReceipt", value); }
        }

        public bool PrintTipGuide
        {
            get { return _dbsettings.BoolGetSetting("CreditCard", "PrintTipGuide", "Print Tip Guide", "false"); }
            set { _dbsettings.BoolSaveSetting("PrintTipGuide", value); }
        }

        public bool RequestTip
        {
            get { return _dbsettings.BoolGetSetting("CreditCard", "RequestTip", "Request Tip from Customer", "true"); }
            set { _dbsettings.BoolSaveSetting("RequestTip", value); }
        }

        public bool EnableHoldButton
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "EnableHoldButton", "Enable Hold Button", "true"); }
            set { _dbsettings.BoolSaveSetting("EnableHoldButton", value); }
        }


        public bool EnableCreditSale
        {
            get { return _dbsettings.BoolGetSetting("CreditCard", "EnableCreditSale", "Enable Credit Sale", "true"); }
            set { _dbsettings.BoolSaveSetting("EnableCreditSale", value); }
        }


        public bool EnableDebitSale
        {
            get { return _dbsettings.BoolGetSetting("CreditCard", "EnableDebitSale", "Enable Debit Sale", "true"); }
            set { _dbsettings.BoolSaveSetting("EnableDebitSale", value); }
        }

        public bool EnableAuthCapture
        {
            get { return _dbsettings.BoolGetSetting("CreditCard", "EnableAuthCapture", "Enable Authorize/Capture", "true"); }
            set { _dbsettings.BoolSaveSetting("EnableAuthCapture", value); }
        }

        public bool EnableManualSale
        {
            get { return _dbsettings.BoolGetSetting("CreditCard", "EnableManualSale", "Enable Manual Sale", "false"); }
            set { _dbsettings.BoolSaveSetting("EnableManualSale", value); }
        }

        public bool RequireSignatureOnAllCreditSale
        {
            get { return _dbsettings.BoolGetSetting("CreditCard", "RequireSignatureOnAllCreditSale", "Require Signature On All Credit Sale", "false"); }
            set { _dbsettings.BoolSaveSetting("RequireSignatureOnAllCreditSale", value); }
        }

        public bool SignatureOnScreen
        {
            get { return _dbsettings.BoolGetSetting("CreditCard", "SignatureOnScreen", "Signature On Screen", "false"); }
            set { _dbsettings.BoolSaveSetting("SignatureOnScreen", value); }
        }

        public bool AuthSigOnPaper
        {
            get { return _dbsettings.BoolGetSetting("CreditCard", "AuthSigOnPaper", "Signature on Paper for Credit Authorize", "true"); }
            set { _dbsettings.BoolSaveSetting("AuthSigOnPaper", value); }
        }

        public bool DelayTicketCreation
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "DelayTicketCreation", "Delay Ticket Creation", "false"); }
            set { _dbsettings.BoolSaveSetting("DelayTicketCreation", value); }
        }

        public bool AllowLegacyGiftCertificate
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "AllowLegacyGiftCertificate", "Allow Legacy Gift Certificate", "false"); }
            set { _dbsettings.BoolSaveSetting("AllowLegacyGiftCertificate", value); }
        }


        public bool EnableVirtualKeyboard
        {
            get { return _dbsettings.BoolGetSetting("Application", "EnableVirtualKeyboard", "Enable Virtual Keyboard", "true"); }
            set { _dbsettings.BoolSaveSetting("EnableVirtualKeyboard", value); }
        }

        public bool ShowRewardButton
        {
            get { return _dbsettings.BoolGetSetting("Application", "ShowRewardButton", "Show Reward Button", "true"); }
            set { _dbsettings.BoolSaveSetting("ShowRewardButton", value); }
        }

        public bool ShowStampCardButton
        {
            get { return _dbsettings.BoolGetSetting("Application", "ShowStampCardButton", "Show Stamp Card Button", "true"); }
            set { _dbsettings.BoolSaveSetting("ShowStampCardButton", value); }
        }

        public bool ShowCheckButton
        {
            get { return _dbsettings.BoolGetSetting("Application", "ShowCheckButton", "Show Check Button", "true"); }
            set { _dbsettings.BoolSaveSetting("ShowCheckButton", value); }
        }
        public bool ShowCommissionOnReport
        {
            get { return _dbsettings.BoolGetSetting("Application", "ShowCommissionOnReport", "ShowCommissionOnReport", "true"); }
            set { _dbsettings.BoolSaveSetting("ShowCommissionOnReport", value); }
        }



        public bool AutoHideClosedTicket
        {
            get {
                if (_dbsettings == null) TouchMessageBox.Show("NULLL");


                return _dbsettings.BoolGetSetting("Ticket", "AutoHideClosedTicket", "Auto Hide Closed Ticket", "false");


            }
            set { _dbsettings.BoolSaveSetting("AutoHideClosedTicket", value); }
        }


        public bool PrintBarCode
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "PrintBarCode", "Print BarCode on Receipt", "false"); }
            set { _dbsettings.BoolSaveSetting("PrintBarCode", value); }
        }



        public bool DeductDiscountFromCommission
        {
            get { return _dbsettings.BoolGetSetting("Application", "DeductDiscountFromCommission", "Deduct Discount From Commission", "false"); }
            set { _dbsettings.BoolSaveSetting("DeductDiscountFromCommission", value); }
        }


        public bool TurnUseSubTotal
        {
            get { return _dbsettings.BoolGetSetting("Application", "TurnUseSubTotal", "Turn Use SubTotal", "false"); }
            set { _dbsettings.BoolSaveSetting("TurnUseSubTotal", value); }
        }

        public bool TurnUsePoints
        {
            get { return _dbsettings.BoolGetSetting("Application", "TurnUsePoints", "Turn Use Points", "false"); }
            set { _dbsettings.BoolSaveSetting("TurnUsePoints", value); }
        }




     

     
        public bool OpenDrawOnClose
        {
            get { return _dbsettings.BoolGetSetting("Application","OpenDrawOnClose","Open Cash Drawer Automatically","true"); }
            set { _dbsettings.BoolSaveSetting("OpenDrawOnClose", value); }
        }

    

        public bool ReceiptPrintReward
        {
            get { return _dbsettings.BoolGetSetting("Reward","ReceiptPrintReward","Print Reward on Receipt","true"); }
            set { _dbsettings.BoolSaveSetting("ReceiptPrintReward", value); }
        }
        public bool AutoReceiptPrint
        {
            get { return _dbsettings.BoolGetSetting("Ticket","AutoReceiptPrint","Print Receipt Automatically","true"); }
            set { _dbsettings.BoolSaveSetting("AutoReceiptPrint", value); }
        }


        public bool AskCustomerPhone
        {
            get { return _dbsettings.BoolGetSetting("Ticket","AskCustomerPhone","Ask For Customer Phone","true"); }
            set { _dbsettings.BoolSaveSetting("AskCustomerPhone", value); }
        }

 

   
        public bool DisplayRewardAlert
        {
            get { return _dbsettings.BoolGetSetting("Reward","DisplayRewardAlert","Display Customer Reward Alert","true"); }
            set { _dbsettings.BoolSaveSetting("DisplayRewardAlert", value); }
        }



        public bool AllowCashBackGiftCertificate
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "AllowCashBackGiftCertificate", "Allow CashBack Gift Certificate", "false"); }
            set { _dbsettings.BoolSaveSetting("AllowCashBackGiftCertificate", value); }
        }

        public bool AllowCashBackGiftCard
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "AllowCashBackGiftCard", "Allow CashBack Gift Card", "false"); }
            set { _dbsettings.BoolSaveSetting("AllowCashBackGiftCard", value); }
        }


        /*
        public bool AutoCloseProduct
        {
            get { return _dbsettings.BoolGetSetting("Ticket","AutoCloseProduct","Close Item Page Automatically","true"); }
            set { _dbsettings.BoolSaveSetting("AutoCloseProduct", value); }
        }
        */
  

        public bool ReceiptPrintEmployee
        {
            get { return _dbsettings.BoolGetSetting("Ticket","ReceiptPrintEmployee","Print Employee on Receipt","true"); }
            set { _dbsettings.BoolSaveSetting("ReceiptPrintEmployee", value); }
        }


        public bool ProductShowDiscount
        {
            get { return _dbsettings.BoolGetSetting("Application","ProductShowDiscount","Show Discount on Items","false"); }
            set { _dbsettings.BoolSaveSetting("ProductShowDiscount", value); }
        }


        public bool UseCheckInList
        {
            get { return _dbsettings.BoolGetSetting("Application","UseCheckInList","Use Check in List for Employees","false"); }
            set { _dbsettings.BoolSaveSetting("UseCheckInList", value); }
        }


        //due to slow load time of history data , this will only pull main data fields
        public bool HistorySimpleView
        {
            get { return _dbsettings.BoolGetSetting("Application", "HistorySimpleView", "History Simple View", "false"); }
            set { _dbsettings.BoolSaveSetting("HistorySimpleView", value); }
        }

        public bool AutoSendMessageToCustomer
        {
            get { return _dbsettings.BoolGetSetting("SMS", "AutoSendMessageToCustomer", "Auto Send Message To Customer", "false"); }
            set { _dbsettings.BoolSaveSetting("AutoSendMessageToCustomer", value); }
        }

        public bool AutoSendReceiptToCustomer
        {
            get { return _dbsettings.BoolGetSetting("SMS", "AutoSendReceiptToCustomer", "Auto Send Receipt To Customer", "false"); }
            set { _dbsettings.BoolSaveSetting("AutoSendReceiptToCustomer", value); }
        }


        public bool DisplayCustomerPhone
        {
            get { return _dbsettings.BoolGetSetting("Application", "DisplayCustomerPhone", "Display Customer Phone", "true"); }
            set { _dbsettings.BoolSaveSetting("DisplayCustomerPhone", value); }
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









        private string m_versionnumber;
        public string VersionNumber
        {
            get { return m_versionnumber; }
            set {m_versionnumber = value; }
        }
        public string DotNetVersion { get; set; }
      
       
     

        public string LanguageCode
        {
            get { return _dbsettings.StringGetSetting("System","LanguageCode","Language Code","en-US"); }
            set { _dbsettings.StringSaveSetting("LanguageCode", value.ToString()); }
        }

        public string BackupDirectory
        {
            get { return _dbsettings.StringGetSetting("System","BackupDirectory","Backup Directory","c:\\reddot\\backup"); }
            set { _dbsettings.StringSaveSetting("BackupDirectory", value.ToString()); }
        }

        string _receiptprinter = "";
        public string ReceiptPrinter
        {
            get { 
                   if(_receiptprinter =="") _receiptprinter =    Utility.GetINIString( "ReceiptPrinter","Printer","none");
                   return _receiptprinter;
            }
            set {
                _receiptprinter = value.ToString();
                Utility.PutINIString("ReceiptPrinter", "Printer", value.ToString()); }
        }

        public string LargeFormatPrinter
        {
            get { return Utility.GetINIString( "LargeFormatPrinter","Printer","none"); }
            set { Utility.PutINIString("LargeFormatPrinter", "Printer", value.ToString()); }
        }

        string _displaycomport = "";
        public string DisplayComPort
        {
            get { 
                if(_displaycomport == "") _displaycomport = Utility.GetINIString("DisplayComPort", "System", "none");
                return _displaycomport;
            }
            set {
                _displaycomport = value.ToString();
                Utility.PutINIString("DisplayComPort", "System", value.ToString()); }
        }

    

        public string MainBackgroundImage
        {
            get { return _dbsettings.StringGetSetting("Application","MainBackgroundImage","Main Background",""); }
            set { _dbsettings.StringSaveSetting("MainBackgroundImage", value.ToString()); }
        }

        public string WindowBackgroundColor
        {
            get { return _dbsettings.StringGetSetting("Application", "WindowBackgroundColor", "Window Background Color", "Black","color"); }
            set { _dbsettings.StringSaveSetting("WindowBackgroundColor", value.ToString()); }
        }



    
        public string StoreLogo
        {
            get { 
                return _dbsettings.StringGetSetting("Application","StoreLogo","Store Logo File","");
            }
            set {
                _dbsettings.StringSaveSetting("StoreLogo", value.ToString());
            }
        }


        public string PayPeriodType
        {
            get { return _dbsettings.StringGetSetting("Application","PayPeriodType","Pay Period Type","BiWeekly"); }
            set { _dbsettings.StringSaveSetting("PayPeriodType", value.ToString()); }
        }


        public string ReceiptNotice
        {
            get { return _dbsettings.StringGetSetting("Ticket","ReceiptNotice","Receipt Notice at Bottom",""); }
            set { _dbsettings.StringSaveSetting("ReceiptNotice", value.ToString()); }
        }

        public string PaymentNotice
        {
            get { return _dbsettings.StringGetSetting("Ticket","PaymentNotice","Payment Notice at Bottom",""); }
            set { _dbsettings.StringSaveSetting("PaymentNotice", value.ToString()); }
        }

   

        public string SIPDefaultIPAddress
        {
            get { return Utility.GetINIString( "SIPDefaultIPAddress", "CreditCard", "S300-xxxxxx"); }
            set { Utility.PutINIString("SIPDefaultIPAddress","CreditCard", value.ToString()); }
        }

        public string SIPPort
        {
            get { return Utility.GetINIString("SIPPort", "CreditCard", "8080"); } //10009 for PAXS300
            set { Utility.PutINIString("SIPPort", "CreditCard", value.ToString()); }
        }

        public string SMSUserName
        {
            get { return _dbsettings.StringGetSetting("SMS","SMSUserName","SMS UserName",""); }
            set { _dbsettings.StringSaveSetting("SMSUserName", value.ToString()); }
        }
        public string SMSAccountID
        {
            get { return _dbsettings.StringGetSetting("SMS","SMSAccountID","SMS Account ID",""); }
            set { _dbsettings.StringSaveSetting("SMSAccountID", value.ToString()); }
        }
        public string SMSEmail
        {
            get { return _dbsettings.StringGetSetting("SMS","SMSEmail","SMS Email",""); }
            set { _dbsettings.StringSaveSetting("SMSEmail", value.ToString()); }
        }

        private string m_SMSPassword="";
        public string SMSPassword
        {
            get {

                if(m_SMSPassword == "") m_SMSPassword =  _dbsettings.StringGetSetting("SMS","SMSPassword","SMS Password","");
                return m_SMSPassword;
            }
            set { _dbsettings.StringSaveSetting("SMSPassword", value.ToString()); }
        }

        private string m_APIKey="";
        public string APIKey
        {
            get {

                if(m_APIKey == "") m_APIKey = _dbsettings.StringGetSetting("SMS", "APIKey", "API Key", "");
                return m_APIKey;
            }
            set { _dbsettings.StringSaveSetting("APIKey", value.ToString()); }
        }


        public string SMSMessage
        {
            get { return _dbsettings.StringGetSetting("SMS","SMSMessage","SMS Message",""); }
            set { _dbsettings.StringSaveSetting("SMSMessage", value.ToString()); }
        }

        public string SMSCustomerMessage
        {
            get { return _dbsettings.StringGetSetting("SMS", "SMSCustomerMessage", "SMS Customer Message", ""); }
            set { _dbsettings.StringSaveSetting("SMSCustomerMessage", value.ToString()); }
        }

        private string m_SMSFromPhone="";
        public string SMSFromPhone
        {
            get {
                if(m_SMSFromPhone =="") m_SMSFromPhone =  _dbsettings.StringGetSetting("SMS","SMSFromPhone", "SMS From Phone","");
                return m_SMSFromPhone;

            }
            set { _dbsettings.StringSaveSetting("SMSFromPhone", value.ToString()); }
        }

        public string CreditCardChoices
        {
            get { return _dbsettings.StringGetSetting("CreditCard","CreditCardChoices","Payment Card List","Credit"); }
            set { _dbsettings.StringSaveSetting("CreditCardChoices", value.ToString()); }
        }


        public string RefundChoices
        {
            get { return _dbsettings.StringGetSetting("CreditCard", "RefundChoices", "Refund Card List", "Credit"); }
            set { _dbsettings.StringSaveSetting("RefundChoices", value.ToString()); }
        }


        public string StoreEmail
        {
            get { return _dbsettings.StringGetSetting("Email","StoreEmail","Store Email",""); }
            set { _dbsettings.StringSaveSetting("StoreEmail", value.ToString()); }
        }

        public string SMTPServer
        {
            get { return _dbsettings.StringGetSetting("Email","SMTPServer","SMTP Server",""); }
            set { _dbsettings.StringSaveSetting("SMTPServer", value.ToString()); }
        }

        public string SMTPPassword
        {
            get { return _dbsettings.StringGetSetting("Email","SMTPPassword","SMTP Password",""); }
            set { _dbsettings.StringSaveSetting("SMTPPassword", value.ToString()); }
        }

        private string m_SMTPUserName="";
        public string SMTPUserName
        {
            get {
                if(m_SMTPUserName == "") m_SMTPUserName = _dbsettings.StringGetSetting("Email","SMTPUserName","SMTP UserName","");
                return m_SMTPUserName;
            }
            set { _dbsettings.StringSaveSetting("SMTPUserName", value.ToString()); }
        }

        public string SMTPCopyTo
        {
            get { return _dbsettings.StringGetSetting("Email","SMTPCopyTo","SMTP Copy To:",""); }
            set { _dbsettings.StringSaveSetting("SMTPCopyTo", value.ToString()); }
        }

        public string AutoTip
        {
            get { return _dbsettings.StringGetSetting("Ticket", "AutoTip", "Automatic Tip Splitting", "Equal"); }
            set { _dbsettings.StringSaveSetting("AutoTip", value.ToString()); }
        }


        public string DatabaseVersion
        {
            get { return _dbsettings.StringGetSetting("System","DatabaseVersion","Database Version",""); }
            set { _dbsettings.StringSaveSetting("DatabaseVersion", value.ToString()); }
        }


  

        public string RewardUsageRestriction
        {
            get { return _dbsettings.StringGetSetting("Reward","RewardUsageRestriction","Reward Usage Restriction","none"); }
            set { _dbsettings.StringSaveSetting("RewardUsageRestriction", value.ToString()); }
        }

        public string RewardException
        {
            get { return _dbsettings.StringGetSetting("Reward", "RewardException", "Reward Exception", "none"); }
            set { _dbsettings.SaveSetting("RewardException", value.ToString()); }
        }

        string _databaseserver = "";
        public string DatabaseServer
        {
            get {
                if (_databaseserver == "") _databaseserver = Utility.GetINIString("Server", "DataBase", "localhost");
                
                return _databaseserver;
            }
            set {
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

        public string WebSyncUpdateTime
        {
            get { return _dbsettings.StringGetSetting("Webservice", "WebSyncUpdateTime", "WebSync Update Time", "9:30 pm"); }
            set { _dbsettings.StringSaveSetting("WebSyncUpdateTime", value.ToString()); }
        }

        public string MessageColor
        {
            get { return _dbsettings.StringGetSetting("Application", "MessageColor", "Message ForeColor", "Green"); }
            set { _dbsettings.StringSaveSetting("MessageColor", value.ToString()); }
        }


        public string CreditCardProcessor
        {
            get { return _dbsettings.StringGetSetting("CreditCard", "CreditCardProcessor", "Credit Card Processor", "External","list", "Clover,External,VANTIV,HeartSIP,PAX_S300,HSIP_ISC250"); }
            set { _dbsettings.StringSaveSetting("CreditCardProcessor", value.ToString()); }
        }


        public string SalesViewMode
        {
            get { return _dbsettings.StringGetSetting("Application", "SalesViewMode", "Main Screen Mode", "Normal","list","Large,Normal,Compact"); }
            set { _dbsettings.StringSaveSetting("SalesViewMode", value.ToString()); }
        }
        public string SalesViewScreen
        {
            get { return _dbsettings.StringGetSetting("Application", "SalesViewScreen", "Sales View Screen", "SalonSales","list","SalonSales,Custom1"); }
            set { _dbsettings.StringSaveSetting("SalesViewScreen", value.ToString()); }
        }


        public string SpecialStatusChoices
        {
            get { return _dbsettings.StringGetSetting("Application", "SpecialStatusChoices", "Special Status Choices", "Military,Medical,Student,None"); }
            set { _dbsettings.StringSaveSetting("SpecialStatusChoices", value.ToString()); }
        }

        public string SMSTextPlatform
        {
            get {
               return _dbsettings.StringGetSetting("SMS", "SMSTextPlatform", "SMS Texting Platform", "Clickatell REST", "list", "Clickatell REST, Clickatell API, Clickatell HTTP,Nexmo API");
       
            }
            set { _dbsettings.StringSaveSetting("SMSTextPlatform", value.ToString()); }
        }


        public string ElementExpressURL
        {
            get { return _dbsettings.StringGetSetting("Credit Card", "ElementExpressURL", "Element Express URL", "https://transaction.elementexpress.com/"); }
            set { _dbsettings.StringSaveSetting( "ElementExpressURL", value.ToString()); }
        }
    }
}
