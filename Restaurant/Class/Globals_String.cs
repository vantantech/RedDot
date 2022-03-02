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

        /*
                ███████╗████████╗██████╗ ██╗███╗   ██╗ ██████╗ ███████╗
                ██╔════╝╚══██╔══╝██╔══██╗██║████╗  ██║██╔════╝ ██╔════╝
                ███████╗   ██║   ██████╔╝██║██╔██╗ ██║██║  ███╗███████╗
                ╚════██║   ██║   ██╔══██╗██║██║╚██╗██║██║   ██║╚════██║
                ███████║   ██║   ██║  ██║██║██║ ╚████║╚██████╔╝███████║
                ╚══════╝   ╚═╝   ╚═╝  ╚═╝╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚══════╝

        */

        #region Strings


        public string CardConnectAuthorization
        {
            get { 
                if(System.Environment.MachineName.ToUpper().Contains("DEVELOPER"))
                {
                    return "ZCb8pPkXcZDVO0CIngLSFrBJgA/BYyUZIHT8zaj3MPg=";
                }
                else
                return "FJjybuRK6EDYAVAWtP+0ufZ6Vnru3QjbHc3pJiyKW4I=" ;
            
            }
          
        }


        public string BoltBaseURL
        {
            get { return _dbsettings.StringGetSetting("Credit Card", "BoltBaseURL", "Bolt Base URL", "https://bolt.cardpointe.com/api"); }
            set { _dbsettings.StringSaveSetting("Credit Card", "BoltBaseURL", value.ToString()); }
        }


        public string CardConnectURL
        {
            get { return _dbsettings.StringGetSetting("Credit Card", "CardConnectURL", "Card Connect URL", "https://boltgw.cardconnect.com/cardconnect/rest"); }
            set { _dbsettings.StringSaveSetting("Credit Card", "CardConnectURL", value.ToString()); }
        }

        public string CardConnectUsername
        {
            get { return _dbsettings.StringGetSetting("Credit Card", "CardConnectUsername", "CardConnect Username", "testing"); }
            set { _dbsettings.StringSaveSetting("Credit Card", "CardConnectUsername", value.ToString()); }
        }

        public string CardConnectPassword
        {
            get { return _dbsettings.StringGetSetting("Credit Card", "CardConnectPassword", "CardConnect Password", "testing123"); }
            set { _dbsettings.StringSaveSetting("Credit Card", "CardConnectPassword", value.ToString()); }
        }

        public string MerchantID
        {
            get { return _dbsettings.StringGetSetting("Credit Card", "MerchantID", "Merchant ID", "123456789"); }
            set { _dbsettings.StringSaveSetting("Credit Card", "MerchantID", value.ToString()); }
        }

        public string TipSuggestion
        {
            get { return _dbsettings.StringGetSetting("Credit Card", "TipSuggestion", "Tip Suggestion", "18,20,25"); }
            set { _dbsettings.StringSaveSetting("Credit Card", "TipSuggestion", value.ToString()); }
        }

        public string TipPrompt
        {
            get { return _dbsettings.StringGetSetting("Credit Card", "TipPrompt", "Tip Prompt", "Would you like to leave a tip?"); }
            set { _dbsettings.StringSaveSetting("Credit Card", "TipPrompt", value.ToString()); }
        }

        string _hsn = "";
        public string HardwareSerialNumber
        {
            get
            {
                if (_hsn == "")
                {
                    _hsn = Utility.GetINIString("HardwareSerialNumber", "Credit Card", "C032UQ03960675");
                }
                return _hsn;
            }
            set
            {
                _hsn = value.ToString();
                Utility.PutINIString("HardwareSerialNumber", "Credit Card", value.ToString());
            }
        }


        string _model = "";
        public string PinPadModel
        {
            get
            {
                if (_model == "")
                {
                    _model = Utility.GetINIString("PinPadModel", "Credit Card", "Mini2");
                }
                return _model;
            }
            set
            {
                _model = value.ToString();
                Utility.PutINIString("PinPadModel", "Credit Card", value.ToString());
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
            set { m_versionnumber = value; }
        }


        public string Port
        {
            get { return Utility.GetINIString("Port", "Credit Card", "10009"); } //10009 for PAXS300
            set { Utility.PutINIString("Port", "Credit Card", value.ToString()); }
        }

        public string IPAddress
        {
            get { return Utility.GetINIString("IPAddress", "Credit Card", "S300-xxxxxx"); }
            set { Utility.PutINIString("IPAddress", "Credit Card", value.ToString()); }
        }

        public string SIPDefaultIPAddress
        {
            get { return Utility.GetINIString("SIPDefaultIPAddress", "Credit Card", "localhost"); }
            set { Utility.PutINIString("SIPDefaultIPAddress", "Credit Card", value.ToString()); }
        }

        public string SIPPort
        {
            get { return Utility.GetINIString("SIPPort", "Credit Card", "8080"); } //10009 for PAXS300
            set { Utility.PutINIString("SIPPort", "Credit Card", value.ToString()); }
        }
        public string ElementExpressURL
        {
            get { return Utility.GetINIString("ElementExpressURL", "Credit Card", "https://transaction.elementexpress.com/"); }
            set { Utility.PutINIString("ElementExpressURL", "Credit Card", value.ToString()); }
        }

        public string ElementExpressReportingURL
        {
            get { return Utility.GetINIString("ElementExpressReportingURL", "Credit Card", "https://reporting.elementexpress.com/"); }
            set { Utility.PutINIString("ElementExpressReportingURL", "Credit Card", value.ToString()); }
        }

        public string AcceptorID
        {
            get { return Utility.GetINIString("AcceptorID", "Credit Card", ""); }
            set { Utility.PutINIString("AcceptorID", "Credit Card", value.ToString()); }
        }

        public string AccountID
        {
            get { return Utility.GetINIString("AccountID", "Credit Card", ""); }
            set { Utility.PutINIString("AccountID", "Credit Card", value.ToString()); }
        }

        public string AccountToken
        {
            get { return Utility.GetINIString("AccountToken", "Credit Card", ""); }
            set { Utility.PutINIString("AccountToken", "Credit Card", value.ToString()); }
        }

        public string TerminalID
        {
            get { return Utility.GetINIString("TerminalID", "Credit Card", ""); }
            set { Utility.PutINIString("TerminalID", "Credit Card", value.ToString()); }
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
            get { return _dbsettings.StringGetSetting("Application", "BackupDirectory", "Backup Directory", "c:\reddot\backup"); }
            set { _dbsettings.StringSaveSetting("Application", "BackupDirectory", value.ToString()); }
        }

        public string PackagingPrinter
        {
            get { return Utility.GetINIString("PackagingPrinter", "Printer", "Packaging"); }
            set { Utility.PutINIString("PackagingPrinter", "Printer", value.ToString()); }
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










        #endregion
    }
}
