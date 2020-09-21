using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManager;
using System.Windows;

namespace RedDot
{
    public class GlobalSettings
    {
        private  static GlobalSettings _Instance;
        private DBSettings _dbsettings;
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


       public void Init()
        {
  
           Shop = new Location();
            _dbsettings = new DBSettings();

        }

        public Location Shop { get; set; }


        public Object RemoteScreen {
            get { return _remotescreen; }
            set { _remotescreen = value; }
        }

        public Object NotificationScreen
        {
            get { return _notificationscreen; }
            set { _notificationscreen = value; }
        }


        public DateTime PayPeriodStartDate
        {
            get { return _dbsettings.GetDateSetting("PayPeriodStartDate"); }
            set { _dbsettings.SaveSetting("PayPeriodStartDate", value.ToString()); }
        }


        public Visibility ProVersion { get; set; }

        public Visibility BaseVersion { get; set; }

        /// <summary>
        /// Integer Type Settings
        /// </summary>

 

        public int LocationId
        {
            get { return int.Parse(Utility.GetINIString("LocationId","System")); }
            set { Utility.PutINIString("LocationId", value.ToString(),"System"); }
        }

        public int Tier1
        {
            get { return _dbsettings.IntGetSetting("Tier1"); }
            set { _dbsettings.SaveSetting("Tier1", value.ToString()); }
        }
        public int Tier2
        {
            get { return _dbsettings.IntGetSetting("Tier2"); }
            set { _dbsettings.SaveSetting("Tier2", value.ToString()); }
        }

        public int Tier3
        {
            get { return _dbsettings.IntGetSetting("Tier3"); }
            set { _dbsettings.SaveSetting("Tier3", value.ToString()); }
        }

 

        public int ReceiptWidth
        {
            get { return _dbsettings.IntGetSetting("ReceiptWidth"); }
            set { _dbsettings.SaveSetting("ReceiptWidth", value.ToString()); }
        }
        public int PayPeriodStartDay
        {
            get { return _dbsettings.IntGetSetting("PayPeriodStartDay"); }
            set { _dbsettings.SaveSetting("PayPeriodStartDay", value.ToString()); }
        }

        public int PinLength
        {
            get { return _dbsettings.IntGetSetting("PinLength"); }
            set { _dbsettings.SaveSetting("PinLength", value.ToString()); }
        }

        public int DayStartHour
        {
            get { return _dbsettings.IntGetSetting("DayStartHour"); }
            set { _dbsettings.SaveSetting("DayStartHour", value.ToString()); }
        }

        public int DayLength
        {
            get { return _dbsettings.IntGetSetting("DayLength"); }
            set { _dbsettings.SaveSetting("DayLength", value.ToString()); }
        }

        public int AppointmentInterval
        {
            get { return _dbsettings.IntGetSetting("AppointmentInterval"); }
            set { _dbsettings.SaveSetting("AppointmentInterval", value.ToString()); }
        }


        public int CategoryWidth
        {
            get { return _dbsettings.IntGetSetting("CategoryWidth"); }
            set { _dbsettings.SaveSetting("CategoryWidth", value.ToString()); }
        }

        public int CategoryHeight
        {
            get { return _dbsettings.IntGetSetting("CategoryHeight"); }
            set { _dbsettings.SaveSetting("CategoryHeight", value.ToString()); }
        }


        public int ProductWidth
        {
            get { return _dbsettings.IntGetSetting("ProductWidth"); }
            set { _dbsettings.SaveSetting("ProductWidth", value.ToString()); }
        }

        public int ProductHeight
        {
            get { return _dbsettings.IntGetSetting("ProductHeight"); }
            set { _dbsettings.SaveSetting("ProductHeight", value.ToString()); }
        }

        public int ProductFontSize
        {
            get { return _dbsettings.IntGetSetting("ProductFontSize"); }
            set { _dbsettings.SaveSetting("ProductFontSize", value.ToString()); }
        }

        public int CategoryFontSize
        {
            get { return _dbsettings.IntGetSetting("CategoryFontSize"); }
            set { _dbsettings.SaveSetting("CategoryFontSize", value.ToString()); }
        }






















        /// <summary>
        /// Decimal Type settings
        /// </summary>
        public decimal ShopFeePercent
        {
            get { return _dbsettings.DecimalGetSetting("ShopFeePercent"); }
            set { _dbsettings.SaveSetting("ShopFeePercent", value.ToString()); }
        }
        public decimal Tier0Reward
        {
            get { return _dbsettings.DecimalGetSetting("Tier0Reward"); }
            set { _dbsettings.SaveSetting("Tier0Reward", value.ToString()); }
        }

        public decimal Tier1Reward
        {
            get { return _dbsettings.DecimalGetSetting("Tier1Reward"); }
            set { _dbsettings.SaveSetting("Tier1Reward", value.ToString()); }
        }

        public decimal Tier2Reward
        {
            get { return _dbsettings.DecimalGetSetting("Tier2Reward"); }
            set { _dbsettings.SaveSetting("Tier2Reward", value.ToString()); }
        }
        public decimal Tier3Reward
        {
            get { return _dbsettings.DecimalGetSetting("Tier3Reward"); }
            set { _dbsettings.SaveSetting("Tier3Reward", value.ToString()); }
        }
        public decimal CommissionPercent
        {
            get { return _dbsettings.DecimalGetSetting("CommissionPercent"); }
            set { _dbsettings.SaveSetting("CommissionPercent", value.ToString()); }
        }
        public decimal RewardPercent
        {
            get { return _dbsettings.DecimalGetSetting("RewardPercent"); }
            set { _dbsettings.SaveSetting("RewardFeePercent", value.ToString()); }
        }
        public decimal MinReward
        {
            get { return _dbsettings.DecimalGetSetting("MinReward"); }
            set { _dbsettings.SaveSetting("MinReward", value.ToString()); }
        }
        public decimal MaxReward
        {
            get { return _dbsettings.DecimalGetSetting("MaxReward"); }
            set { _dbsettings.SaveSetting("MaxReward", value.ToString()); }
        }
        public decimal SalesTaxPercent
        {
            get { return _dbsettings.DecimalGetSetting("SalesTaxPercent"); }
            set { _dbsettings.SaveSetting("SalesTaxPercent", value.ToString()); }
        }

        public decimal MinimumTurnAmount
        {
            get { return _dbsettings.DecimalGetSetting("MinimumTurnAmount"); }
            set { _dbsettings.SaveSetting("MinimumTurnAmount", value.ToString()); }
        }

        public decimal CreditCardFeePercent
        {
            get { return _dbsettings.DecimalGetSetting("CreditCardFeePercent"); }
            set { _dbsettings.SaveSetting("CreditCardFeePercent", value.ToString()); }
        }
        public decimal CommissionAmount
        {
            get { return _dbsettings.DecimalGetSetting("CommissionAmount"); }
            set { _dbsettings.SaveSetting("CommissionAmount", value.ToString()); }
        }

        public decimal PaySplit
        {
            get { return _dbsettings.DecimalGetSetting("PaySplit"); }
            set { _dbsettings.SaveSetting("PaySplit", value.ToString()); }
        }

        public bool GetBoolSetting(string item)
        {
            return _dbsettings.BoolGetSetting(item);
        }

        /// <summary>
        /// Bool Type Settings
        /// </summary>
        /// 

        private bool _homeclicked;
        public bool HomeClicked
        {
            get { return _homeclicked; }
            set { _homeclicked = value; }
        }
        public bool PaymentCreditUsePOS
        {
            get { return _dbsettings.BoolGetSetting("PaymentCreditUsePOS"); }
            set { _dbsettings.SaveSetting("PaymentCreditUsePOS", value.ToString()); }
        }

        public bool GlobalCommission
        {
            get { return _dbsettings.BoolGetSetting("GlobalCommission"); }
            set { _dbsettings.SaveSetting("GlobalCommission", value.ToString()); }
        }

        public bool RunPrinterTest
        {
            get { return _dbsettings.BoolGetSetting("RunPrinterTest"); }
            set { _dbsettings.SaveSetting("RunPrinterTest", value.ToString()); }
        }
        public bool OpenDrawOnClose
        {
            get { return _dbsettings.BoolGetSetting("OpenDrawOnClose"); }
            set { _dbsettings.SaveSetting("OpenDrawOnClose", value.ToString()); }
        }

        public bool DisplayCustomerReward
        {
            get { return _dbsettings.BoolGetSetting("DisplayCustomerReward"); }
            set { _dbsettings.SaveSetting("DisplayCustomerReward", value.ToString()); }
        }

        public bool ReceiptPrintReward
        {
            get { return _dbsettings.BoolGetSetting("ReceiptPrintReward"); }
            set { _dbsettings.SaveSetting("ReceiptPrintReward", value.ToString()); }
        }
        public bool AutoReceiptPrint
        {
            get { return _dbsettings.BoolGetSetting("AutoReceiptPrint"); }
            set { _dbsettings.SaveSetting("AutoReceiptPrint", value.ToString()); }
        }


        public bool ForceEmployeeAssign
        {
            get { return _dbsettings.BoolGetSetting("ForceEmployeeAssign"); }
            set { _dbsettings.SaveSetting("ForceEmployeeAssign", value.ToString()); }
        }
        public bool AskEmployeeAssign
        {
            get { return _dbsettings.BoolGetSetting("AskEmployeeAssign"); }
            set { _dbsettings.SaveSetting("AskEmployeeAssign", value.ToString()); }
        }

        public bool AskCustomerPhone
        {
            get { return _dbsettings.BoolGetSetting("AskCustomerPhone"); }
            set { _dbsettings.SaveSetting("AskCustomerPhone", value.ToString()); }
        }

        public bool AskTaxExempt
        {
            get { return _dbsettings.BoolGetSetting("AskTaxExempt"); }
            set { _dbsettings.SaveSetting("AskTaxExempt", value.ToString()); }
        }

        public bool StayInHistory
        {
            get { return _dbsettings.BoolGetSetting("StayInHistory"); }
            set { _dbsettings.SaveSetting("StayInHistory", value.ToString()); }
        }

        public bool StayInCustomer
        {
            get { return _dbsettings.BoolGetSetting("StayInCustomer"); }
            set { _dbsettings.SaveSetting("StayInCustomer", value.ToString()); }
        }
        public bool DisplayRewardAlert
        {
            get { return _dbsettings.BoolGetSetting("DisplayRewardAlert"); }
            set { _dbsettings.SaveSetting("DisplayRewardAlert", value.ToString()); }
        }

        public bool AskSurcharge
        {
            get { return _dbsettings.BoolGetSetting("AskSurcharge"); }
            set { _dbsettings.SaveSetting("AskSurcharge", value.ToString()); }
        }


        public bool SalesTabEnable1
        {
            get { return _dbsettings.BoolGetSetting("SalesTabEnable1"); }
            set { _dbsettings.SaveSetting("SalesTabEnable1", value.ToString()); }
        }

        public bool SalesTabEnable2
        {
            get { return _dbsettings.BoolGetSetting("SalesTabEnable2"); }
            set { _dbsettings.SaveSetting("SalesTabEnable2", value.ToString()); }
        }
        public bool SalesTabEnable3
        {
            get { return _dbsettings.BoolGetSetting("SalesTabEnable3"); }
            set { _dbsettings.SaveSetting("SalesTabEnable3", value.ToString()); }
        }

        public bool ReceiptShowModifierPrice
        {
            get { return _dbsettings.BoolGetSetting("ReceiptShowModifierPrice"); }
            set { _dbsettings.SaveSetting("ReceiptShowModifierPrice", value.ToString()); }
        }
        public bool DisplayShowModifierPrice
        {
            get { return _dbsettings.BoolGetSetting("DisplayShowModifierPrice"); }
            set { _dbsettings.SaveSetting("DisplayShowModifierPrice", value.ToString()); }
        }


        public bool AutoCloseProduct
        {
            get { return _dbsettings.BoolGetSetting("AutoCloseProduct"); }
            set { _dbsettings.SaveSetting("AutoCloseProduct", value.ToString()); }
        }

        public bool HideQuantity
        {
            get { return _dbsettings.BoolGetSetting("HideQuantity"); }
            set { _dbsettings.SaveSetting("HideQuantity", value.ToString()); }
        }

        public bool ReceiptPrintEmployee
        {
            get { return _dbsettings.BoolGetSetting("ReceiptPrintEmployee"); }
            set { _dbsettings.SaveSetting("ReceiptPrintEmployee", value.ToString()); }
        }


        public bool ProductShowDiscount
        {
            get { return _dbsettings.BoolGetSetting("ProductShowDiscount"); }
            set { _dbsettings.SaveSetting("ProductShowDiscount", value.ToString()); }
        }






















        /// <summary>
        /// String settings --------------------------------------------------------- STRINGS -----------------------------------------------------------------
        /// </summary>
        /// 
   
        public string DisplayComPort
        {
            get { return _dbsettings.GetStringSetting("DisplayComPort"); }
            set { _dbsettings.SaveSetting("DisplayComPort", value.ToString()); }
        }

        public string MSRCardPrefix
        {
            get { return _dbsettings.GetStringSetting("MSRCardPrefix"); }
            set { _dbsettings.SaveSetting("MSRCardPrefix", value.ToString()); }
        }

        private string m_versionnumber;
        public string VersionNumber
        {
            get { return m_versionnumber; }
            set {m_versionnumber = value; }
        }

 
        public string Language
        {
            get { return _dbsettings.GetStringSetting("Language"); }
            set { _dbsettings.SaveSetting("Language", value.ToString()); }
        }

        public string LanguageCode
        {
            get { return _dbsettings.GetStringSetting("LanguageCode"); }
            set { _dbsettings.SaveSetting("LanguageCode", value.ToString()); }
        }

        public string BackupDirectory
        {
            get { return _dbsettings.GetStringSetting("BackupDirectory"); }
            set { _dbsettings.SaveSetting("BackupDirectory", value.ToString()); }
        }

        public string ReceiptPrinter
        {
            get { return Utility.GetINIString( "ReceiptPrinter","Printer"); }
            set { Utility.PutINIString( "ReceiptPrinter", value.ToString(),"Printer");}
        }

        public string LargeFormatPrinter
        {
            get { return Utility.GetINIString( "LargeFormatPrinter","Printer"); }
            set { Utility.PutINIString( "LargeFormatPrinter", value.ToString(),"Printer"); }
        }

  

        public string ReceiptPrinterType
        {
            get { return _dbsettings.GetStringSetting("ReceiptPrinterType"); }
            set { _dbsettings.SaveSetting("ReceiptPrinterType", value.ToString()); }
        }

        public string MainBackgroundImage
        {
            get { return _dbsettings.GetStringSetting("MainBackgroundImage"); }
            set { _dbsettings.SaveSetting("MainBackgroundImage", value.ToString()); }
        }


        public string StoreLogo
        {
            get { return _dbsettings.GetStringSetting("StoreLogo"); }
            set { _dbsettings.SaveSetting("StoreLogo", value.ToString()); }
        }

        public string SurchargeQuestion
        {
            get { return _dbsettings.GetStringSetting("SurchargeQuestion"); }
            set { _dbsettings.SaveSetting("SurchargeQuestion", value.ToString()); }
        }



        public string PayPeriodType
        {
            get { return _dbsettings.GetStringSetting("PayPeriodType"); }
            set { _dbsettings.SaveSetting("PayPeriodType", value.ToString()); }
        }


        public string ReceiptNotice
        {
            get { return _dbsettings.GetStringSetting("ReceiptNotice"); }
            set { _dbsettings.SaveSetting("ReceiptNotice", value.ToString()); }
        }

        public string PaymentNotice
        {
            get { return _dbsettings.GetStringSetting("PaymentNotice"); }
            set { _dbsettings.SaveSetting("PaymentNotice", value.ToString()); }
        }

        public string MySQLDumpPath
        {
            get { return _dbsettings.GetStringSetting("MySQLDumpPath"); }
            set { _dbsettings.SaveSetting("MySQLDumpPath", value.ToString()); }
        }



        public string SalesTabName1
        {
            get { return _dbsettings.GetStringSetting("SalesTabName1"); }
            set { _dbsettings.SaveSetting("SalesTabName1", value.ToString()); }
        }

        public string SalesTabName2
        {
            get { return _dbsettings.GetStringSetting("SalesTabName2"); }
            set { _dbsettings.SaveSetting("SalesTabName2", value.ToString()); }
        }

        public string SalesTabName3
        {
            get { return _dbsettings.GetStringSetting("SalesTabName3"); }
            set { _dbsettings.SaveSetting("SalesTabName3", value.ToString()); }
        }

        public string SalesCustomName1
        {
            get { return _dbsettings.GetStringSetting("SalesCustomName1"); }
            set { _dbsettings.SaveSetting("SalesCustomName1", value.ToString()); }
        }
        public string SalesCustomName2
        {
            get { return _dbsettings.GetStringSetting("SalesCustomName2"); }
            set { _dbsettings.SaveSetting("SalesCustomName2", value.ToString()); }
        }
        public string SalesCustomName3
        {
            get { return _dbsettings.GetStringSetting("SalesCustomName3"); }
            set { _dbsettings.SaveSetting("SalesCustomName3", value.ToString()); }
        }
        public string SalesCustomName4
        {
            get { return _dbsettings.GetStringSetting("SalesCustomName4"); }
            set { _dbsettings.SaveSetting("SalesCustomName4", value.ToString()); }
        }
        public string SMSUserName
        {
            get { return _dbsettings.GetStringSetting("SMSUserName"); }
            set { _dbsettings.SaveSetting("SMSUserName", value.ToString()); }
        }
        public string SMSAccountID
        {
            get { return _dbsettings.GetStringSetting("SMSAccountID"); }
            set { _dbsettings.SaveSetting("SMSAccountID", value.ToString()); }
        }
        public string SMSEmail
        {
            get { return _dbsettings.GetStringSetting("SMSEmail"); }
            set { _dbsettings.SaveSetting("SMSEmail", value.ToString()); }
        }
        public string SMSPassword
        {
            get { return _dbsettings.GetStringSetting("SMSPassword"); }
            set { _dbsettings.SaveSetting("SMSPassword", value.ToString()); }
        }

        public string SMSMessage
        {
            get { return _dbsettings.GetStringSetting("SMSMessage"); }
            set { _dbsettings.SaveSetting("SMSMessage", value.ToString()); }
        }

        public string SMSFromPhone
        {
            get { return _dbsettings.GetStringSetting("SMSFromPhone"); }
            set { _dbsettings.SaveSetting("SMSFromPhone", value.ToString()); }
        }

        public string CreditCardChoices
        {
            get { return _dbsettings.GetStringSetting("CreditCardChoices"); }
            set { _dbsettings.SaveSetting("CreditCardChoices", value.ToString()); }
        }

        public string StoreEmail
        {
            get { return _dbsettings.GetStringSetting("StoreEmail"); }
            set { _dbsettings.SaveSetting("StoreEmail", value.ToString()); }
        }

        public string SMTPServer
        {
            get { return _dbsettings.GetStringSetting("SMTPServer"); }
            set { _dbsettings.SaveSetting("SMTPServer", value.ToString()); }
        }

        public string SMTPPassword
        {
            get { return _dbsettings.GetStringSetting("SMTPPassword"); }
            set { _dbsettings.SaveSetting("SMTPPassword", value.ToString()); }
        }
        public string SMTPUserName
        {
            get { return _dbsettings.GetStringSetting("SMTPUserName"); }
            set { _dbsettings.SaveSetting("SMTPUserName", value.ToString()); }
        }
        public string SMTPCopyTo
        {
            get { return _dbsettings.GetStringSetting("SMTPCopyTo"); }
            set { _dbsettings.SaveSetting("SMTPCopyTo", value.ToString()); }
        }


        public string DatabaseVersion
        {
            get { return _dbsettings.GetStringSetting("DatabaseVersion"); }
            set { _dbsettings.SaveSetting("DatabaseVersion", value.ToString()); }
        }



        public string CommissionType
        {
            get { return _dbsettings.GetStringSetting("CommissionType"); }
            set { _dbsettings.SaveSetting("CommissionType", value.ToString()); }
        }

        public string DeductSupplyFee
        {
            get { return _dbsettings.GetStringSetting("DeductSupplyFee"); }
            set { _dbsettings.SaveSetting("DeductSupplyFee", value.ToString()); }
        }

        public string RewardUsageRestriction
        {
            get { return _dbsettings.GetStringSetting("RewardUsageRestriction"); }
            set { _dbsettings.SaveSetting("RewardUsageRestriction", value.ToString()); }
        }


        public string RewardException
        {
            get { return _dbsettings.GetStringSetting("RewardException"); }
            set { _dbsettings.SaveSetting("RewardException", value.ToString()); }
        }








    }
}
