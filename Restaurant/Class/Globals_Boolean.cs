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

      ██████╗  ██████╗  ██████╗ ██╗     ███████╗ █████╗ ███╗   ██╗
      ██╔══██╗██╔═══██╗██╔═══██╗██║     ██╔════╝██╔══██╗████╗  ██║
      ██████╔╝██║   ██║██║   ██║██║     █████╗  ███████║██╔██╗ ██║
      ██╔══██╗██║   ██║██║   ██║██║     ██╔══╝  ██╔══██║██║╚██╗██║
      ██████╔╝╚██████╔╝╚██████╔╝███████╗███████╗██║  ██║██║ ╚████║
      ╚═════╝  ╚═════╝  ╚═════╝ ╚══════╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝


      */

        #region Boolean

        public bool DebugMode
        {
            get { return Utility.GetINIString("DebugMode", "System", "false").ToUpper() == "TRUE"; }
            set { Utility.PutINIString("DebugMode", "System", value ? "true" : "false"); }
        }

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

        public bool AutoCapture
        {
            get { return _dbsettings.BoolGetSetting("CreditCard", "AutoCapture", "Credit Auto Capture", "true"); }
            set { _dbsettings.SaveSetting("Credit Card","AutoCapture", value.ToString()); }
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


        public bool SeparateStaffOrders
        {
            get { return _dbsettings.BoolGetSetting("Ticket", "SeparateStaffOrders", "Separate Staff Orders", "true"); }
            set { _dbsettings.SaveSetting("Ticket", "SeparateStaffOrders", value.ToString()); }
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
            get { return _dbsettings.BoolGetSetting("Application", "EnableVirtualKeyboard", "Enable Virtual Keyboard", "false"); }
            set { _dbsettings.BoolSaveSetting("Application", "EnableVirtualKeyboard", value); }
        }
        public bool EnableWebService
        {
            get { return _dbsettings.BoolGetSetting("Application", "EnableWebService", "Enable Web Service", "false"); }
            set { _dbsettings.BoolSaveSetting("Application", "EnableWebService", value); }
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

        public bool EnableWalkInOrders
        {
            get { return _dbsettings.BoolGetSetting("Walk In", "EnableWalkInOrders", "Enable Walk In Orders", "false"); }
            set { _dbsettings.SaveSetting("Walk In", "EnableWalkInOrders", value.ToString()); }
        }

        public bool EnableDriveThruOrders
        {
            get { return _dbsettings.BoolGetSetting("To Go", "EnableDriveThruOrders", "Enable Drive Thru Orders", "false"); }
            set { _dbsettings.SaveSetting("To Go", "EnableDriveThruOrders", value.ToString()); }
        }

        public bool EnableCallInOrders
        {
            get { return _dbsettings.BoolGetSetting("Call In", "EnableCallInOrders", "Enable Call In Orders", "false"); }
            set { _dbsettings.SaveSetting("Call In", "EnableCallInOrders", value.ToString()); }
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
            get { return _dbsettings.BoolGetSetting("Ticket", "KitchenPrintDoubleWidth", "Kitchen Print Double Width", "false"); }
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

    }
}
