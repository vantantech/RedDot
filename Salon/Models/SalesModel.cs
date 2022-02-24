
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Windows;
using RedDot.DataManager;
using GlobalPayments.Api.Terminals;
using TriPOS.SignatureFiles;
using TriPOS.ResponseModels;
using POSLink;
using System.Xml;
using RedDot.Models;
using RedDot.Models.CardConnect;

namespace RedDot
{
    public class SalesModel
    {
        private DBProducts   m_dbproducts;
    
        private DBPromotions m_dbpromotions;
        private IDataInterface m_dataconnection;
     
       // private Window m_parent;
        private VFD vfd;

        bool UseMySQL = true;

        private HeartPOS m_ccp;
    
        public SalesModel(HeartPOS ccp)
        {
            m_dbproducts   = new DBProducts();

            this.m_ccp = ccp;

            m_dataconnection = GlobalSettings.Instance.RedDotData;
         

            m_dbpromotions = new DBPromotions();
          //  m_parent = parent;
            vfd = new VFD(GlobalSettings.Instance.DisplayComPort);
        
            
        }






        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        __  __      _   _               _     
        // |  _ \ _   _| |__ | (_) ___  |  \/  | ___| |_| |__   ___   __| |___ 
        // | |_) | | | | '_ \| | |/ __| | |\/| |/ _ \ __| '_ \ / _ \ / _` / __|
        // |  __/| |_| | |_) | | | (__  | |  | |  __/ |_| | | | (_) | (_| \__ \
        // |_|    \__,_|_.__/|_|_|\___| |_|  |_|\___|\__|_| |_|\___/ \__,_|___/
        //    
        //------------------------------------------------------------------------------------------  

        public int GetSalonSalesCount(int employeeid)
        {
            DataTable dt;
            dt = m_dataconnection.GetSalonSalesCount(employeeid);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["count"].ToString() != "")
                    return int.Parse(dt.Rows[0]["count"].ToString());
                else return 0;

            }
            else return 0;

        }
     
        public Ticket LoadTicket( int salesid)
        {

             Ticket newticket;
             newticket = new Ticket(salesid);  //constructor also takes salesid , will load ticket using id
             return newticket;   
   
        }

        public int AskEmployeeSelect(Window parent, SecurityModel security)
        {
            PickEmployee empl = new PickEmployee(parent,security);
            Utility.OpenModal(parent, empl);
      

            return empl.EmployeeID;
        }


        public DataTable LoadOpenTicketsByEmployee(int employeeid)
        {
            return m_dataconnection.GetOpenSalesbyEmployee(employeeid);
        }
        public DataTable LoadAllOpenTickets()
        {
            return m_dataconnection.GetAllOpenSales();
            
        }

        public DataTable LoadOpenTicketsByCustomer(int customerid)
        {
            return m_dataconnection.GetOpenSalesbyCustomer(customerid);
        }

        public DataTable LoadOpenTicketsByTicket(int id)
        {
            return m_dataconnection.GetOpenSalesbyTicket(id);
        }
        public DataTable LoadOpenTicketsByDate(DateTime startdate, DateTime enddate)
        {
            return m_dataconnection.GetOpenSalesbyDates(startdate, enddate);
        }

    

        public bool UpdateTipAmount(Payment payment , decimal tipamount)
        {
            decimal totalamount = payment.Amount;

            if (payment.CardGroup.ToUpper() == "CREDIT" || payment.CardGroup.ToUpper() == "DEBIT") totalamount += tipamount;

            return m_dataconnection.DBUpdateTipAmount(payment.ID, tipamount, totalamount);
        }


        public bool UpdateTipAmount(int paymentid, decimal tipamount, decimal totalamount)
        {

            return m_dataconnection.DBUpdateTipAmount(paymentid, tipamount, totalamount);
        }

        public bool UpdatePaymentCapture(int paymentid, decimal tipamount,decimal amount, decimal netamount, string transactionid)
        {
            return m_dataconnection.UpdatePaymentCapture(paymentid, tipamount, amount, netamount, transactionid);
        }


        public bool UpdatePaymentCapture(int paymentid, decimal tipamount, decimal amount)
        {
            return m_dataconnection.UpdatePaymentCapture(paymentid, tipamount, amount);
        }
        /*
        public ObservableCollection<Ticket> LoadTickets(Employee currentemployee)
        {


            DataTable dt;
            Ticket newticket;
            ObservableCollection<Ticket> ticketcollecttion = new ObservableCollection<Ticket>();


                dt =_dbsales.GetSalesbyEmployee( currentemployee.ID );



            foreach(DataRow row in dt.Rows)
            {
                newticket = new Ticket(int.Parse(row["ID"].ToString()));
                ticketcollecttion.Add(newticket);

            }

            return ticketcollecttion;
           
        }


        public ObservableCollection<Ticket> LoadAllOpenTickets()
        {


            DataTable dt;
            Ticket newticket;
            ObservableCollection<Ticket> ticketcollecttion = new ObservableCollection<Ticket>();


            dt = _dbsales.GetAllOpenSales();

            foreach (DataRow row in dt.Rows)
            {
                newticket = new Ticket(int.Parse(row["ID"].ToString()));
                ticketcollecttion.Add(newticket);

            }

            return ticketcollecttion;

        }

        */
        public bool AddItemToTicket(int techid,int productid, Ticket currentticket)
        {
           
          //  MenuItemSimple product;
            MenuItem product;  //testing to see if we can remove menuitemsimple



            try
            {
               // product = new MenuItemSimple(productid);
                product = new MenuItem(productid,false);   //set savetodb to false since we are not editing .. just using as readonly


                if (product.Price < 0)
                {
                    NumPad np = new NumPad("Please Enter Custom Amount:",false);
                    np.Topmost = true;
                    np.ShowDialog();
                    
                    if (np.Amount != "") product.Price = decimal.Parse(np.Amount);
                    else return false;
                }


                switch (product.Type.Replace(" ","").ToUpper())
                {
                    case "GIFTCARD":

                        //gift card requires special handling
                        CardScanner gcard = new CardScanner();
                        gcard.Topmost = true;
                        gcard.ShowDialog();


                        if (gcard.CardNumber != "")
                        {
                            if (currentticket.GiftCardOnLineItem(gcard.CardNumber))
                            {
                                TouchMessageBox.Show("Gift Card already on ticket...");

                            }
                            else
                            {
                                decimal balance = GiftCardModel.GetGiftCardBalance(gcard.CardNumber);
                                if (balance != -99)
                                {
                                    TouchMessageBox.Show("Balance: " + balance + " \n Amount will be added to Card Balance...");
                                }

                                AddItemToTicketDB(techid,currentticket, product,1, gcard.CardNumber, "", "", "");
                            }

                        }
                        break;

                    case "GIFTCERTIFICATE":

                        //gift certificate requires saving number for future lookup
                        GiftCertificate num = new GiftCertificate();
                        num.Topmost = true;
                        num.ShowDialog();


                        if (num.CertificateNumber != "")
                        {
                            if (currentticket.GiftCardOnLineItem(num.CertificateNumber))
                            {
                                TouchMessageBox.Show("Gift Certificate already on ticket...");

                            }
                            else
                            {
                                
                                if (GiftCardModel.GiftCertificateExist(num.CertificateNumber))
                                {
                                    TouchMessageBox.Show("Gift Certificate NUMBER is already used!!!");
                                    
                                }else
                                {
                                    AddItemToTicketDB(techid, currentticket, product, 1, num.CertificateNumber, "", "", "");
                                }

                               
                            }

                        }
                        break;

                    case "PRODUCT":

                        AddItemToTicketDB(techid, currentticket, product, 1, "", "", "", "");
                      
                        break;
                   case "SERVICE":


                        AddItemToTicketDB(techid, currentticket, product, 1, "", "", "", "");
                      
                        break;

                    default:
                        TouchMessageBox.Show("Product/Service type not recognized!!");
                        break;

                }

                return true;


            }catch(Exception e)
            {
                TouchMessageBox.Show("Add Item to Ticket:" + e.Message);
                return false;
            }
        }


        public void AddItemToTicketDB(int techid,Ticket m_currentticket, MenuItem prod,int quantity, string custom1, string custom2, string custom3, string custom4)
        {
               
           

            try
            {

                bool result = m_currentticket.AddProductLineItem( prod,quantity, techid, custom1, custom2, custom3, custom4);

                if (result )
                {
                    vfd.WriteDisplay(prod.Description, prod.AdjustedPrice, "Total:", m_currentticket.Total);

                }


            }
            catch (Exception e)
            {
                TouchMessageBox.Show("AddItemToTicket:" + e.Message);
            }
        }


        public bool ProcessCreditCard(Window parent, Ticket currentticket)
        {
            try
            {
                CreditCardView ccv = new CreditCardView(parent, currentticket, "SALE");
                Utility.OpenModal(parent, ccv);

                currentticket.LoadPayment();
                currentticket.CloseTicket(); //need to load payment  to refresh object first before trying to close ticket
                currentticket.Reload();
                return true;

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Process Credit Card:" + e.Message);
                return false;
            }

        }

        public bool ProcessCash(Window parent, Ticket currentticket)
        {
            try
            {
                CashTenderedView ctv = new CashTenderedView(parent, currentticket, 674);
                Utility.OpenModal(parent, ctv);

                currentticket.LoadPayment();
                currentticket.CloseTicket(); //need to load payment  to refresh object first before trying to close ticket
                currentticket.Reload();
                return true;

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Process Credit Card:" + e.Message);
                return false;
            }

        }

        public bool ProcessCheck(Window parent,Ticket currentticket)
        {

            try
            {
               decimal amt;
                NumPad ccv = new NumPad("Enter Check Amount",false,currentticket.Balance.ToString());
                Utility.OpenModal(parent, ccv);

                if (ccv.Amount == "") return false;

                amt = decimal.Parse(ccv.Amount);
                currentticket.AddPayment("Check", amt, "","Sale");

                currentticket.LoadPayment();
                currentticket.CloseTicket();

                return true;

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ProcessCheck:" + e.Message);
                return false;
            }
        }

        public bool ProcessStampCard(Window parent, Ticket currentticket)
        {

            try
            {
                decimal amt;
                NumPad ccv = new NumPad("Enter Stamp Card Amount", false);
                Utility.OpenModal(parent, ccv);

                if (ccv.Amount == "") return false;
             

                amt = decimal.Parse(ccv.Amount);
                currentticket.AddPayment("Stamp Card", amt, "", "Sale");

                currentticket.LoadPayment();
                currentticket.CloseTicket();

                return true;

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ProcessCheck:" + e.Message);
                return false;
            }
        }

        public bool ProcessGiftCertificate(Window parent, Ticket currentticket)
        {

            try
            {

                GiftCertificate num = new GiftCertificate();
                Utility.OpenModal(parent, num);
                
                if (num.CertificateNumber == "") return false;

                //check to see if in system .. if found display Amount
                DBTicket dbticket = new DBTicket();

               decimal bal =  dbticket.GetGiftCardBalance(num.CertificateNumber);
                if(bal > 0 )
                {
                    TouchMessageBox.Show("Certificate Found. BALANCE LEFT: " + bal.ToString());
                }else
                {
                    if(bal == 0)
                    {
                        TouchMessageBox.Show("Certificate Found.  BALANCE LEFT: 0 ");
                        return false;
                        
                    }else
                    {
                        TouchMessageBox.Show("Certificate NOT FOUND!!!");
                        if (GlobalSettings.Instance.AllowLegacyGiftCertificate)
                            bal = 0;
                        else return false;
                    }
                 
                }

                decimal amt;
                decimal defaultamt = 0;

                if (bal >= currentticket.Balance)
                    defaultamt = currentticket.Balance;
                else
                    defaultamt = bal;



                NumPad ccv = new NumPad("Enter Redeem Amount", false, defaultamt.ToString());
                Utility.OpenModal(parent, ccv);
                if (ccv.Amount == "") return false;

                amt = decimal.Parse(ccv.Amount);

               if(!GlobalSettings.Instance.AllowCashBackGiftCertificate)
                {
                    //check to see if amount is greater than balance
                    if(amt > currentticket.Balance)
                    {
                        TouchMessageBox.Show("Amount greater than balance due is not allowed!!");
                        return false;
                    }
                }


                currentticket.AddPayment("Gift Certificate", amt,num.CertificateNumber,"Sale");

                currentticket.LoadPayment();
                currentticket.CloseTicket();

                return true;

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ProcessGiftCertificate:" + e.Message);
                return false;
            }
        }



        public ObservableCollection<MenuItem> FillProductbyCatID(int catid)
        {
            ObservableCollection<MenuItem> menuitems = new ObservableCollection<MenuItem>();

            DataTable data_category;
            MenuItem newitem;

            m_dbproducts = new DBProducts();
            if (catid == 1000)
            {
                data_category = m_dbproducts.GetAllProducts(" order by SUBSTRING(product.menuprefix,1,1), cast(SUBSTRING(product.menuprefix,2) as unsigned), product.id");
            }
            else if (catid == 999)
            {
                data_category = m_dbproducts.GetRecentProducts(" order by SUBSTRING(product.menuprefix,1,1), cast(SUBSTRING(product.menuprefix,2) as unsigned), product.id");
            }
            else
                data_category = m_dbproducts.GetProductsByCat(catid, " order by SUBSTRING(product.menuprefix,1,1), cast(SUBSTRING(product.menuprefix,2) as unsigned), product.id");

            foreach (DataRow row in data_category.Rows)
            {

                newitem = new MenuItem(row, false);


                menuitems.Add(newitem);

            }
            return menuitems;

        }



        public DataTable GetCategoryList()
        {
            // ObservableCollection<Category> data = new ObservableCollection<Category>();

            // Category category;

            DataTable data_category = m_dbproducts.GetCategories(0);
            DataRow newrow = data_category.NewRow();
            newrow["id"] = 999;
            newrow["sortorder"] = 99;
            newrow["colorcode"] = "RoyalBlue";
            newrow["description"] = "Recent 20 Items";
           data_category.Rows.InsertAt(newrow, data_category.Rows.Count);

            newrow = data_category.NewRow();
            newrow["id"] = 1000;
            newrow["sortorder"] = 100;
            newrow["colorcode"] = "Black";
            newrow["description"] = "All Inventory Items";
           data_category.Rows.InsertAt(newrow, data_category.Rows.Count);


            return data_category;


        }

        public DataTable GetMenuPrefix()
        {
            return m_dbproducts.GetMenuPrefix();
        }


        public int GetProductID(string menuprefix)
        {
            return m_dbproducts.GetProductIDByMenuPrefix(menuprefix);
        }

        public DataTable GetMenuPrefix2(string menuprefix)
        {
            return m_dbproducts.GetMenuPrefix2(menuprefix);
        }


        public int FindProductByBarCode(string searchstr)
        {
           return m_dbproducts.GetProductByBarCode(searchstr);
        }


        public Payment GetPayment(string transactionNo)
        {
            DataTable rtn = m_dataconnection.GetPayment(transactionNo);
            if (rtn.Rows.Count > 0)
            {
                return new Payment(rtn.Rows[0]);
            }
            else return null;
        }


        public  Payment GetPayment(int id)
        {
            DataTable rtn = m_dataconnection.GetPayment(id);
            if (rtn.Rows.Count > 0)
            {
                return new Payment(rtn.Rows[0]);
            }
            else return null;
        }

        public static bool IsGiftCardOnPayment(int salesid, string giftcardnumber)
        {
            DBTicket dbticket = new DBTicket();
            return dbticket.GiftCardOnPayment(salesid, giftcardnumber);


        }

        public static bool InsertPayment(int salesid, string paytype, decimal amount, decimal netamount, string authorizecode, string cardtype, string maskedpan, decimal tip, DateTime paymentdate, string transtype)
        {
            DBTicket dbticket = new DBTicket();

            return dbticket.DBInsertPayment(salesid, paytype, amount, netamount, authorizecode, cardtype, maskedpan, tip, paymentdate, transtype);
        }


        public static bool RedeemReward(int customerid, int salesid, DateTime saledate, decimal tickettotal, decimal rewardamount, string note)
        {
            DBTicket dbticket = new DBTicket();
            return dbticket.DBInsertCustomerReward(customerid, salesid, saledate, tickettotal, rewardamount, "REDEEM", note);
        }
        public static bool RedeemGiftCard(int salesid, string paytype, decimal amount, string giftcardnumber, DateTime paymentdate)
        {
            DBTicket dbticket = new DBTicket();

            return dbticket.DBRedeemGiftCard(salesid, paytype, amount, giftcardnumber, paymentdate);
        }


        //for HeartSIP SDK
        public static bool InsertCreditPayment(int salesid, decimal requested_amount, ResponseEventArgs e, DateTime paymentdate)
        {
            DBTicket dbticket = new DBTicket();

            decimal authamt = 0;
            decimal cashback = 0;
            decimal tip = 0;
            int pinverified = (e.PinVerified == "1" ? 1 : 0);


            if (e.AuthorizedAmount != null && e.AuthorizedAmount != "") authamt = int.Parse(e.AuthorizedAmount) / 100m;
            if (e.CashbackAmount != null && e.CashbackAmount != "") cashback = int.Parse(e.CashbackAmount) / 100m;
            if (e.TipAmount != null && e.TipAmount != "") tip = int.Parse(e.TipAmount) / 100m;

            return dbticket.DBInsertCreditPayment(salesid, requested_amount, e.CardGroup, e.ApprovalCode, e.CardType, e.MaskedPAN, e.CardAcquisition, e.ResponseId, authamt, cashback, tip, e.TransType,pinverified, e.SignatureLine, e.TipAdjustAllowed, e.EMV_ApplicationName, e.EMV_Cryptogram, e.EMV_CryptogramType, e.EMV_AID, e.CardholderName, paymentdate, "", "");
        }

        //for Global Payment SDK
        public static bool InsertCreditPayment(int salesid, decimal requested_amount, TerminalResponse resp, DateTime paymentdate, string cardgroup)
        {
            DBTicket dbticket = new DBTicket();
            string signature = "1";  //defaults to required
            string tipadjustallowed = "0";


            var signatureRequiredCvms = new List<string> { "3", "5", "6" }; //chip card pin not verified
            var signatureRequiredStatus = new List<string> { "2", "3", "6" }; //these signature status requires a signature  ( 1 and 5 = signature capture)
            if (resp.EntryMethod == "Chip")
            {
                var cvm = resp.CardHolderVerificationMethod;
                if (signatureRequiredCvms.Contains(cvm))
                {
                    var sign_status = resp.SignatureStatus ?? "0";
                    if (sign_status == "1" || sign_status == "5")
                        //signature captured electronically .. so not required
                        signature = "2";
                    else if (signatureRequiredStatus.Contains(sign_status))
                        signature = "1"; //required signature

                }
                else
                    signature = "0"; //pin verified online so no need for signature
            }
            else signature = "1"; //required signature

            //CardHolderVerificationMethod == "0" , no pin provided
            //CardHolderVerificationMethod == "1" , pin verified online
            //CardHolderVerificationMethod == "2" , off line pin

            int pinverified = int.Parse(resp.CardHolderVerificationMethod);

            if (resp.TransactionType == "AUTH") tipadjustallowed = "1"; else tipadjustallowed = "0";


            return dbticket.DBInsertCreditPayment(salesid, requested_amount, cardgroup, resp.AuthorizationCode, resp.PaymentType, resp.MaskedCardNumber, resp.EntryMethod, resp.TransactionId, (decimal)resp.TransactionAmount, (decimal)resp.CashBackAmount, (decimal)resp.TipAmount, resp.TransactionType, pinverified, signature, tipadjustallowed, resp.ApplicationPreferredName, resp.ApplicationCryptogram, resp.ApplicationCryptogramType.ToString(), resp.ApplicationId, resp.CardHolderName, paymentdate, "", "");
        }


        //payment for Clover SDK
        public static bool InsertCreditPayment(int salesid, decimal requested_amount, com.clover.sdk.v3.payments.Payment resp, DateTime paymentdate)
        {
            DBTicket dbticket = new DBTicket();
            string transtype = resp.cardTransaction.type.ToString();
            if (transtype == "AUTH") transtype = "SALE";
            else if (transtype == "PREAUTH") transtype = "AUTH";

            string cardgroup = "";
            if (resp.tender.label == "Debit Card") cardgroup = "DEBIT";

            if (resp.tender.label == "Credit Card") cardgroup = "CREDIT";

            string signature = "0";

            if(resp.cardTransaction.extra.Count > 0)
            {
                var extra = resp.cardTransaction.extra;
                var val = extra.Values.ToArray()[0].ToString();
                if (val.ToString().ToUpper() == "SIGNATURE") signature = "2";
            }
            return dbticket.DBInsertCreditPayment(salesid, requested_amount, cardgroup, resp.cardTransaction.authCode, resp.cardTransaction.cardType.ToString(), resp.cardTransaction.last4, resp.cardTransaction.entryType.ToString(), resp.cardTransaction.referenceId, (decimal)resp.amount / 100, (decimal)resp.cashbackAmount / 100, (decimal)resp.tipAmount / 100, transtype,0, signature, "0", "", "", "", "", resp.cardTransaction.cardholderName, paymentdate, resp.id, resp.order.id);
        }

        //payment for Vantiv TriPOS
        public static bool InsertCreditPayment(string transtype, int salesid, decimal requested_amount, TriPOS.ResponseModels.SaleResponse resp, DateTime paymentdate)
        {
            DBTicket dbticket = new DBTicket();

            string signature = "0";
            //depending on the signature object returned
            if (!Enum.TryParse(resp.Signature.SignatureStatusCode, true, out SignatureStatusCode signatureStatusCode))
            {
                signature = "1";
            }


            switch (signatureStatusCode)
            {
                // Signature required, check for signature data
                case SignatureStatusCode.SignatureRequired:
                    if (resp.Signature.SignatureData != null)
                    {
                        signature = "2";
                    }
                    else
                    {
                        // Signature is required but the response contains no signature data, include blank signature line on receipt 
                        signature = "1";
                    }

                    break;

                // Signature present, display it
                case SignatureStatusCode.SignaturePresent:
                    signature = "2";
                    break;

                // Signature required, cancelled by cardholder, display signature line
                case SignatureStatusCode.SignatureRequiredCancelledByCardholder:

                // Signature required, not supported by PIN pad, display signature line
                case SignatureStatusCode.SignatureRequiredNotSupportedByPinPad:

                // Signature required, PIN pad error, display signature line
                case SignatureStatusCode.SignatureRequiredPinPadError:
                    signature = "1";
                    break;

                // Unknown/error, do not display signature or signature line
                case SignatureStatusCode.Unknown:

                // Signature not required by threshold amount, do not display signature or signature line
                case SignatureStatusCode.SignatureNotRequiredByThresholdAmount:

                // Signature not required by payment type, do not display signature or signature line
                case SignatureStatusCode.SignatureNotRequiredByPaymentType:

                // Signature not required by transaction type, do not display signature or signature line
                case SignatureStatusCode.SignatureNotRequiredByTransactionType:
                    signature = "0";
                    break;
                default:
                    signature = "1";
                    break;
            }


            string cryptogram = "";
            string cryptogramtype = "";
            string ApplicationLabel = "";
            string ApplicationIdentifier = "";


            if (resp.Emv != null)
            {
                ApplicationLabel = resp.Emv.ApplicationLabel;
                ApplicationIdentifier = resp.Emv.ApplicationIdentifier;

                if (resp.Emv.Cryptogram != null)
                {
                    if (resp.Emv.Cryptogram != null && resp.Emv.Cryptogram != "")
                    {
                        var parts = resp.Emv.Cryptogram.Split(' ');
                        cryptogramtype = parts[0].ToString();
                        cryptogram = parts[1].ToString();
                    }

                }
            }
           


            return dbticket.DBInsertCreditPayment(salesid, resp.SubTotalAmount, resp.PaymentType.ToUpper(), resp.ApprovalNumber, resp.CardLogo.ToUpper(), MaskCardNumber(resp), resp.EntryMode.ToUpper(), resp.TransactionId, (decimal)resp.ApprovedAmount, (decimal)resp.CashbackAmount, (decimal)resp.TipAmount,transtype.ToUpper(),resp.PinVerified?1:0, signature, "0", ApplicationLabel, cryptogram, cryptogramtype, ApplicationIdentifier, resp.CardHolderName, paymentdate, "", "");


        }

        //payment for Card Connect
        public static bool InsertCreditPayment(string transtype,int salesid, decimal requested_amount,decimal tip,CCSaleResponse resp, DateTime paymentdate)
        {
            string signatureline = "0";
            string tipadjustallowed = "0";
            decimal netamount = resp.amount - tip;
            DBTicket dbticket = new DBTicket();
            int pinverified = resp.EMV_Data.PIN == "Verified by PIN" ? 1 : 0;
            return dbticket.DBInsertCreditPayment(salesid,  netamount, "CREDIT", resp.authcode, resp.EMV_Data.Network_Label, "****", resp.EMV_Data.Entry_method, resp.retref,resp.amount, 0,tip, transtype.ToUpper(), pinverified, signatureline, tipadjustallowed, resp.EMV_Data.Application_Label,resp.EMV_Data.AID,resp.EMV_Data.IAD, resp.EMV_Data.AID, resp.receiptData.nameOnCard, paymentdate, "", "");

        }

        //payment for PAX
        public static bool InsertCreditPayment(string transtype, int salesid, decimal requested_amount, PaymentResponse resp, DateTime paymentdate)
        {
            DBTicket dbticket = new DBTicket();

            string signature = "0";






            /*
            if (resp.Emv != null)
            {
                ApplicationLabel = resp.Emv.ApplicationLabel;
                ApplicationIdentifier = resp.Emv.ApplicationIdentifier;

                if (resp.Emv.Cryptogram != null)
                {
                    if (resp.Emv.Cryptogram != null && resp.Emv.Cryptogram != "")
                    {
                        var parts = resp.Emv.Cryptogram.Split(' ');
                        cryptogramtype = parts[0].ToString();
                        cryptogram = parts[1].ToString();
                    }

                }
            }
            */
            string extdata = resp.ExtData.ToUpper();

            string entrymode = "";
            string temp = findXMl(resp.ExtData, "PLEntryMode");
            if (temp == "0")
            {
                string cardPresent = findXMl(resp.ExtData,"PLCardPresent");
                if (cardPresent == "1")
                {
                    entrymode = "Keyed CNP";

                }
                else
                {
                    entrymode = "Keyed CP";
                }
            }
            else if (temp == "1")
            {
                entrymode = "Swiped MSD";
            }
            else if (temp == "2")
            {
                if (findXMl(resp.ExtData,"TC") != "")
                {
                    entrymode = "Contactless Chip";
                }
                else
                {
                    entrymode = "Contactless MSD";
                }
            }
            else if (temp == "3")
            {
                entrymode = "Scanner";
            }
            else if (temp == "4")
            {
                entrymode = "Contact Chip";
            }
            else if (temp == "5")
            {
                //right = "Chip Fall Back Swipe";
                entrymode = "FALLBACK-Swiped";
            }

            decimal cashbackamount = 0;
            temp = findXMl(resp.ExtData,"CashBackAmout");
            if (temp != "" && temp != "0")
            {
                cashbackamount = Convert.ToInt32(temp);
            }


            decimal tip = 0;
            temp = findXMl(resp.ExtData,"TipAmount");
            if (temp != "" && temp != "0")
            {
                tip = Convert.ToInt32(temp);
            }

            string transactionid = findXMl(resp.ExtData,"HRef");

            string cryptogram = "";
            string cryptogramtype = "";
            string ApplicationLabel =  findXMl(resp.ExtData,"APPPN");
            string ApplicationIdentifier = findXMl(resp.ExtData,"AID");

            return dbticket.DBInsertCreditPayment(salesid, int.Parse(resp.RequestedAmount)/100.00m, resp.CardType.ToUpper(), resp.AuthCode, resp.CardType.ToUpper(), resp.BogusAccountNum,entrymode, resp.RefNum, int.Parse(resp.ApprovedAmount)/100.00m,cashbackamount, tip, transtype.ToUpper(), 0, signature, "0", ApplicationLabel, cryptogram, cryptogramtype, ApplicationIdentifier,"customername", paymentdate, "", "");


        }

        private static string findXMl(string content,string node)
        {
            XmlDocument xml = new XmlDocument();
    
            int index;

           ;

            index = content.IndexOf("<ADDLRSPDATA>");
            if (index > 0)
            {
                content = content.Substring(0, index);
            }

            string extData = "<root>";

            extData += content;

            extData += "</root>";

            string value;

            xml.LoadXml(extData);


            foreach (XmlElement element in xml.DocumentElement.ChildNodes)
            {
                if (element.Name == node)
                {
                    value = element.InnerText;
                    return value;
                }
            }

            return "";
        }


        private static string MaskCardNumber(TriPOS.ResponseModels.SaleResponse response)
        {
            const int NumberOfAllowedUnmaskedDigits = 4;

            if (string.IsNullOrWhiteSpace(response.AccountNumber))
            {
                return null;
            }

            int numberOfDigitsInCardNumber = response.AccountNumber.Length;
            int numberOfDigitsToMask = numberOfDigitsInCardNumber - NumberOfAllowedUnmaskedDigits;

            var maskedDigitsBuilder = new StringBuilder();

            for (int i = 0; i < numberOfDigitsToMask; i++)
            {
                maskedDigitsBuilder.Append('*');
            }

            string maskedDigits = maskedDigitsBuilder.ToString();

            string lastFourDigitsOfCardNumber = response.AccountNumber.Substring(numberOfDigitsToMask);
            string maskedCardNumber = string.Format("{0}{1}", maskedDigits, lastFourDigitsOfCardNumber);
            return maskedCardNumber;
        }
        public static bool InsertCreditRefund(int salesid, decimal requested_amount, com.clover.sdk.v3.payments.Credit resp, DateTime paymentdate, string cardgroup, string paymentid, string orderid)
        {
            DBTicket dbticket = new DBTicket();
            string transtype = "REFUND";


            return dbticket.DBInsertCreditPayment(salesid, requested_amount, cardgroup, resp.cardTransaction.authCode, resp.cardTransaction.cardType.ToString(), resp.cardTransaction.last4, resp.cardTransaction.entryType.ToString(), resp.cardTransaction.referenceId, (decimal)resp.amount / 100, 0, 0, transtype, 0, "0", "1", "", "", "", "", resp.cardTransaction.cardholderName, paymentdate, resp.id, resp.orderRef.id);
        }


        public static bool VoidCreditPayment(string responseid)
        {
            DBTicket dbticket = new DBTicket();

            return dbticket.DBVoidCreditPayment(responseid);
        }

        public static bool VoidCreditPayment(string cloverpaymentid, string cloverorderid)
        {
            DBTicket dbticket = new DBTicket();

            return dbticket.DBVoidCreditPayment(cloverpaymentid, cloverorderid);
        }

        public static bool HasBeenPaid(int salesid, string paymenttype)
        {
            DBTicket dbticket = new DBTicket();

            return dbticket.HasBeenPaid(salesid, paymenttype);

        }
    }
}
