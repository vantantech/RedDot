
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using GlobalPayments.Api.Terminals;
using TriPOS.ResponseModels;
using TriPOS.SignatureFiles;

namespace RedDot
{
    public class SalesModel
    {
        private DBProducts m_dbproducts;
        private DBSales m_dbsales;
        private DBPromotions m_dbpromotions;
        private DBTicket m_dbticket;
        private SecurityModel m_security;
    
        public SalesModel(SecurityModel security)
        {
            m_dbproducts = new DBProducts();
            m_dbsales = new DBSales();
            m_dbpromotions = new DBPromotions();
            m_dbticket = new DBTicket();
            m_security = security;
            
        }






        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        __  __      _   _               _     
        // |  _ \ _   _| |__ | (_) ___  |  \/  | ___| |_| |__   ___   __| |___ 
        // | |_) | | | | '_ \| | |/ __| | |\/| |/ _ \ __| '_ \ / _ \ / _` / __|
        // |  __/| |_| | |_) | | | (__  | |  | |  __/ |_| | | | (_) | (_| \__ \
        // |_|    \__,_|_.__/|_|_|\___| |_|  |_|\___|\__|_| |_|\___/ \__,_|___/
        //    
        //------------------------------------------------------------------------------------------  

        public static int GetSalesCount(int employeeid)
        {
            DBSales m_dbsales = new DBSales();
            DataTable dt;

            dt = m_dbsales.GetSalesCount(employeeid);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["count"].ToString() != "")
                    return int.Parse(dt.Rows[0]["count"].ToString());
                else return 0;

            }
            else return 0;

        }


        public static Ticket GetTicket(int salesid)
        {
            Ticket newticket;
            newticket = new Ticket(salesid);  //constructor also takes salesid , will load ticket using id
            return newticket;
        }


        public static DataTable GetHoldTickets(int stationno)
        {
            try
            {
                DBSales m_dbsales = new DBSales();
                DataTable dt = m_dbsales.GetHoldOrders(stationno);

                return dt;
            }
            catch {
                return null;
            }
        

        }

        public Ticket LoadTicket( int salesid)
        {

                    Ticket newticket;
                    newticket = new Ticket(salesid);  //constructor also takes salesid , will load ticket using id
                     return newticket;   
   
        }

        public int AskEmployeeSelect(Window parent)
        {
            PickEmployee empl = new PickEmployee(parent,null);

            Utility.OpenModal(parent, empl);
            return empl.EmployeeID;
        }


        public DataTable LoadOpenTicketsByEmployee(int employeeid)
        {
            return m_dbsales.GetOpenSalesbyEmployee(employeeid);
        }

        public ObservableCollection<Ticket> LoadOpenTicketsByTable(int tablenumber, int employeeid)
        {

            ObservableCollection<Ticket> rtn = new ObservableCollection<Ticket>();

            DataTable dat = m_dbsales.GetOpenSalesbyTable(tablenumber, employeeid);

            foreach (DataRow row in dat.Rows)
            {
                Ticket newrec = new Ticket(int.Parse(row["salesid"].ToString()));
                rtn.Add(newrec);
            }

            return rtn;


        }

        public ObservableCollection<Ticket> LoadOpenTicketsByOrderNumber(int ordernumber, int employeeid)
        {

            ObservableCollection<Ticket> rtn = new ObservableCollection<Ticket>();

            DataTable dat = m_dbsales.GetOpenSalesbyOrderNumber(ordernumber, employeeid);

            foreach (DataRow row in dat.Rows)
            {
                Ticket newrec = new Ticket(int.Parse(row["salesid"].ToString()));
                rtn.Add(newrec);
            }

            return rtn;


        }

        public ObservableCollection<Ticket> LoadSplitTickets(string tracker)
        {

            ObservableCollection<Ticket> rtn = new ObservableCollection<Ticket>();

            DataTable dat = m_dbsales.GetSplitTickets(tracker);

            foreach (DataRow row in dat.Rows)
            {
                Ticket newrec = new Ticket(int.Parse(row["salesid"].ToString()));
                rtn.Add(newrec);
            }

            return rtn;


        }

        //load other tickets by same employee so can be combined with current
        public ObservableCollection<Ticket> LoadCombineTickets(int employeeid, int currentsalesid)
        {
            ObservableCollection<Ticket> rtn = new ObservableCollection<Ticket>();

            DataTable dat = m_dbsales.GetCombineTickets(employeeid, currentsalesid);

            foreach (DataRow row in dat.Rows)
            {
                Ticket newrec = new Ticket(int.Parse(row["salesid"].ToString()));
                rtn.Add(newrec);
            }

            return rtn;
        }


        public ObservableCollection<Ticket> LoadOpenTickets(int employeeid, string ordertype)
        {
            ObservableCollection<Ticket> rtn = new ObservableCollection<Ticket>();

            DataTable dat = GetOpenOrders(employeeid, ordertype);

            foreach (DataRow row in dat.Rows)
            {
                Ticket newrec = new Ticket(int.Parse(row["salesid"].ToString()));
                rtn.Add(newrec);
            }

            return rtn;
        }

        public DataTable GetOpenOrders(int employeeid, string ordertype)
        {
            return m_dbsales.GetOpenOrders(employeeid, ordertype);
        }

        public ObservableCollection<Ticket> LoadOpenTicketsByOthers(int employeeid, string ordertype)
        {
            ObservableCollection<Ticket> rtn = new ObservableCollection<Ticket>();

            DataTable dat = m_dbsales.GetOpenOrdersByOthers(employeeid, ordertype);

            foreach (DataRow row in dat.Rows)
            {
                Ticket newrec = new Ticket(int.Parse(row["salesid"].ToString()));
                rtn.Add(newrec);
            }

            return rtn;
        }

        public DataTable LoadAllOpenTickets()
        {
            return m_dbsales.GetAllOpenSalesQS();
        }

        public DataTable LoadOpenTicketsByCustomer(int customerid)
        {
            return m_dbsales.GetOpenSalesbyCustomer(customerid);
        }

        public DataTable LoadOpenTicketsByTicket(int id)
        {
            return m_dbsales.GetOpenSalesbyTicket(id);
        }
        public DataTable LoadOpenTicketsByDate(DateTime startdate, DateTime enddate)
        {
            return m_dbsales.GetOpenSalesbyDates(startdate, enddate);
        }




        public DataTable GetProductsByCat(int catid)
        {

            return m_dbproducts.GetProductsByCat(catid);
        }


        public Payment GetPayment(string transactionNo)
        {
            DataTable rtn = m_dbsales.GetPayment(transactionNo);
            if (rtn.Rows.Count > 0)
            {
                return new Payment(rtn.Rows[0]);
            }
            else return null;
        }


        public Payment GetPayment(int id)
        {
            DataTable rtn = m_dbsales.GetPayment(id);
            if (rtn.Rows.Count > 0)
            {
                return new Payment(rtn.Rows[0]);
            }
            else return null;
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



        public void AddItemToTicket(Ticket m_currentticket, Window m_parent, int m_currentseat, decimal m_quantity, int prodid, OrderType m_ordertype)
        {
            decimal price;
            Product product;
            decimal weight = 1;

            product = new Product(prodid,m_ordertype);
            price = product.AdjustedPrice;


            if (price < 0)
            {
                NumPad np = new NumPad("Please Enter Custom Amount:",false,false);
                Utility.OpenModal(m_parent, np);

                if (np.Amount != "") price = decimal.Parse(np.Amount);
                else return;
            }



            if (product.Weighted)
            {
                NumPad np = new NumPad("Please Enter Weight:", !product.AllowPartial, false);
                Utility.OpenModal(m_parent, np);

                if (np.Amount != "") weight = decimal.Parse(np.Amount);
                else return;
            }

            switch (product.Type.Replace(" ", "").ToUpper())
            {
                case "GIFTCARD":

                    //gift card requires special handling
                    CardScanner gcard = new CardScanner();
                    Utility.OpenModal(m_parent, gcard);


                    if (gcard.CardNumber != "")
                    {
                        if (m_currentticket.GiftCardOnLineItem(gcard.CardNumber))
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

                        
                            AddItemToTicketDB(m_currentticket, m_parent, m_currentseat, m_quantity,weight, product, price, "", gcard.CardNumber, "", "", "");
                        }

                    }
                  
                       
                   
                    break;


                case "COMBO":

                case "PRODUCT":

                    //other items
                    AddItemToTicketDB(m_currentticket, m_parent, m_currentseat, m_quantity,weight, product, price, "", "", "", "", "");
                    break;

             

                case "GIFTCERTIFICATE":

                    //gift certificate requires saving number for future lookup
                    GiftCertificate num = new GiftCertificate();
                    Utility.OpenModal(m_parent, num);



                    if (num.CertificateNumber != "")
                    {
                        if (m_currentticket.GiftCardOnLineItem(num.CertificateNumber))
                        {
                            MessageBox.Show("Gift Certificate already on ticket...");

                        }
                        else
                        {

                            if (GiftCardModel.GiftCertificateExist(num.CertificateNumber))
                            {
                                MessageBox.Show("Gift Certificate NUMBER is already used!!!");

                            }
                            else
                            {
                               
                                AddItemToTicketDB(m_currentticket, m_parent, m_currentseat, m_quantity,weight, product, price, "", num.CertificateNumber, "", "", "");
                            }


                        }

                    }
                    break;

                default:
                    TouchMessageBox.Show("Menu Item Type is not definded!!!");
                    break;
            }

        }



        public LineItem AddItemToTicketDB(Ticket m_currentticket, Window m_parent, int m_currentseat, decimal m_quantity, decimal weight,Product prod, decimal price, string note, string custom1, string custom2, string custom3, string custom4)
        {

            LineItem newsalesitem;

            try
            {
                newsalesitem = m_currentticket.AddProductLineItem(prod, m_currentseat, price, m_quantity,weight, note, custom1, custom2, custom3, custom4);

                if (newsalesitem != null)
                {
                    GlobalSettings.CustomerDisplay.WriteDisplay(prod.Description, prod.AdjustedPrice, "Total:", m_currentticket.Total);


                    //Goes thru modifiers for this item
                    if (prod.HasModifiers)
                    {
                        //now check to see if it has FORCED modifers

                        //AddComboView dlg;
                        SalesModifierView dlg;

                        dlg = new SalesModifierView(m_parent, m_currentticket, newsalesitem.ID, prod,true);
                        Utility.OpenModal(m_parent, dlg);

                    }

                   //check to make sure items wasn't cancelled in previous step ( modifier step)
                    if (prod != null)
                    {
                        //check to see if it's a combo .. which would then have it's own product line item in it
                        if (prod.Type.ToUpper() == "COMBO")
                            if (prod.ComboGroupId > 0)
                            {
                                SalesComboItemView dlg;
                                dlg = new SalesComboItemView(m_parent, m_currentticket, newsalesitem.ID, prod,true);
                                Utility.OpenModal(m_parent, dlg);

                            }

                        GlobalSettings.CustomerDisplay.WriteDisplay("", "", "Total:", m_currentticket.Total);
                    }
                }

                return newsalesitem;
            }
            catch (Exception e)
            {
                MessageBox.Show("AddItemToTicket:" + e.Message);
                return null;
            }
        }


        public void AdjustTicket(Ticket currentticket)
        {

            DiscountPad np = new DiscountPad(0, currentticket.SubTotal);
            Utility.OpenModal(null, np);
            int employeemealid = 0;
     

                if (np.ReturnPromotion == null) return;  /// user cancelled

                switch (np.ReturnPromotion.Description)
                {

                    case "Manual":
                        //need manual discount access
                        if (m_security.WindowAccess("Discount") == false) { return; }

                        if (np.ReturnPromotion.DiscountAmount >= 0)
                        {
                            string reason = "";
                            TextPad tp1 = new TextPad("Enter Discount Description", "");
                            Utility.OpenModal(null, tp1);
                            reason = tp1.ReturnText;

                           // currentticket.DiscountLineItem(line.ID, np.ReturnPromotion.DiscountAmount, "Manual Discount", reason);

                            currentticket.AdjustTicket((-1) * np.ReturnPromotion.DiscountAmount,"Manual Discount", reason);
                            AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, np.ReturnPromotion.Description + "=" + np.ReturnPromotion.DiscountAmount.ToString(),reason, "ticket Adjustment", currentticket.SalesID);

                        }
                    break;

                    case "Remove":

                       // CurrentTicket.DiscountLineItem(line.ID, 0, "", "");
                        currentticket.AdjustTicket(0, "", "");
        

                            break;

                    default:

                        //normal predefined discounts
                        //check for security override requirements
                        if (currentticket.CurrentEmployee.SecurityLevel < np.ReturnPromotion.SecurityLevel)
                        {
                            var manager = m_security.GetEmployee(false,"Manager Override Needed");
                            if (manager.SecurityLevel < np.ReturnPromotion.SecurityLevel)
                            {
                                TouchMessageBox.Show("Need Approval Override!!");
                                return;
                            }
                        }

       

                        //limited usage
                        if (np.ReturnPromotion.LimitedUseOnly && np.ReturnPromotion.UsageNumber == 0)
                        {
                            TouchMessageBox.Show("Limted Number of Usage Reached!!");
                            return;
                        }


              

                        //Employee Meals
                        if (np.ReturnPromotion.DiscountType == "EMPLOYEE MEAL")
                        {
                            DateTime time = DateTime.Now;

                            DateTime shift1start = new DateTime(time.Year, time.Month, time.Day, GlobalSettings.Instance.Shift1Start.Hours, GlobalSettings.Instance.Shift1Start.Minutes, 0);
                            DateTime shift1end = new DateTime(time.Year, time.Month, time.Day, GlobalSettings.Instance.Shift1End.Hours, GlobalSettings.Instance.Shift1End.Minutes, 0);

                            DateTime shift2start = new DateTime(time.Year, time.Month, time.Day, GlobalSettings.Instance.Shift2Start.Hours, GlobalSettings.Instance.Shift2Start.Minutes, 0);
                            DateTime shift2end = new DateTime(time.Year, time.Month, time.Day, GlobalSettings.Instance.Shift2End.Hours, GlobalSettings.Instance.Shift2End.Minutes, 0);

                            DateTime shift3start = new DateTime(time.Year, time.Month, time.Day, GlobalSettings.Instance.Shift3Start.Hours, GlobalSettings.Instance.Shift3Start.Minutes, 0);
                            DateTime shift3end = new DateTime(time.Year, time.Month, time.Day, GlobalSettings.Instance.Shift3End.Hours, GlobalSettings.Instance.Shift3End.Minutes, 0);

                        SecurityModel employeemeal = new SecurityModel();
                        employeemeal.WindowNewAccess("EmployeeMeals","Who is Employee?");

                        if (employeemeal.CurrentEmployee == null) return;



                        employeemealid = employeemeal.CurrentEmployee.ID;
                        np.ReturnPromotion.Description = "EM-" + employeemeal.CurrentEmployee.FullName;


                            if (shift1start <= DateTime.Now && DateTime.Now <= shift1end)
                            {
                                var Meals = employeemeal.CurrentEmployee.GetEmployeeMeals(shift1start, shift1end);
                                if (Meals.Rows.Count >= 1)
                                {
                                    TouchMessageBox.Show("You've already used your discount for this shift.");
                                    return;
                                }
                            }
                            if (shift2start <= DateTime.Now && DateTime.Now <= shift2end)
                            {
                                var Meals = employeemeal.CurrentEmployee.GetEmployeeMeals(shift2start, shift2end);
                                if (Meals.Rows.Count > 1)
                                {
                                    TouchMessageBox.Show("You've already used your discount for this shift.");
                                    return;
                                }
                            }
                            if (shift3start <= DateTime.Now && DateTime.Now <= shift3end)
                            {
                                var Meals = employeemeal.CurrentEmployee.GetEmployeeMeals(shift3start, shift3end);
                                if (Meals.Rows.Count > 1)
                                {
                                    TouchMessageBox.Show("You've already used your discount for this shift.");
                                    return;
                                }
                            }



                        }

                    //IS THIS DISCOUNT for Ticket??  Or individual Items , check for restrictions
                    DataTable productlist = np.ReturnPromotion.GetProductIDs();
                    string[] array = productlist.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();
                    if (array.Count() > 0)
                    {
                        foreach(Seat seat in currentticket.Seats)
                            foreach(LineItem line in seat.LineItems)
                            {
                                //check to see if item is even on list of qualified items
                               if(!array.Contains(line.ProductID.ToString()))
                                {
                                    continue;
                                }
                                //check for restrictions
                                if (np.ReturnPromotion.FullPriceOnly)
                                {
                                    if (line.SpecialPricing)
                                    {
                                        //marke line as bad
                                        // currentticket.UpdateNote(line.ID, "**Not Discountable**");
                                        currentticket.DiscountLineItem(line.ID, 0, "", "Not Discountable");
                                        continue;  // go to next line
                                    }
                                   
                                }
                                if (line.PriceWithModifiers < np.ReturnPromotion.MinimumAmount) 
                                {
                                    //marke line as bad
                                    currentticket.DiscountLineItem(line.ID, 0, "", "Not Discountable");
                                    continue;  // go to next line
                                }

                                if (line.Weight < np.ReturnPromotion.MinimumWeight)
                                {
                 
                                    currentticket.DiscountLineItem(line.ID, 0, "", "Not Discountable");
                                    return;
                                }

                                if (line.Quantity < np.ReturnPromotion.MinimumQuantity)
                                {
           
                                    currentticket.DiscountLineItem(line.ID, 0, "", "Not Discountable");
                                    return;
                                }


                                decimal discount = 0;
                                switch (np.ReturnPromotion.DiscountMethod)
                                {
                                    case "PERCENT":
                                        discount = line.Price *  line.Weight * np.ReturnPromotion.DiscountAmount / 100;
                                        break;

                                    case "AMOUNT":
                                        discount = np.ReturnPromotion.DiscountAmount;
                                        break;

                                }


                                currentticket.DiscountLineItem(line.ID, discount, np.ReturnPromotion.DiscountType, np.ReturnPromotion.Description);

                                if (np.ReturnPromotion.LimitedUseOnly)
                                {
                                    //reduce usage by one
                                    np.ReturnPromotion.DeductUsage();
                                }
                               


                            }

                     
                    }else
                    {

                        decimal discount = 0;
                        switch (np.ReturnPromotion.DiscountMethod)
                        {
                            case "PERCENT":
                                discount = currentticket.SubTotal * np.ReturnPromotion.DiscountAmount / 100;

                                if(np.ReturnPromotion.MaxDiscount > 0)
                                {
                                    if (discount > np.ReturnPromotion.MaxDiscount) discount = np.ReturnPromotion.MaxDiscount;
                                }
                                break;

                            case "AMOUNT":
                                discount = np.ReturnPromotion.DiscountAmount;
                                if (np.ReturnPromotion.DiscountType == "EMPLOYEE MEAL")
                                {
                                    if (currentticket.SubTotal < discount) discount = currentticket.SubTotal;
                                }
                            
                                    
                                break;

                        }
                        //apply to whole ticket instead
                        currentticket.AdjustTicket((-1) * discount,np.ReturnPromotion.DiscountType, np.ReturnPromotion.Description,employeemealid);
                        AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, np.ReturnPromotion.DiscountType + "=" + np.ReturnPromotion.DiscountAmount.ToString(), np.ReturnPromotion.Description, "ticket Adjustment", currentticket.SalesID);


                    }


                    break;

            }
                np = null;

            currentticket.Reload();
            
        }

 

        public bool ProcessCashTender(Window parent, Ticket currentticket)
        {
            try
            {

             


                CashTenderedView ctv = new CashTenderedView(parent, currentticket);
                Utility.OpenModal(parent, ctv);
        

                return currentticket.IsClosed; 

            }
            catch (Exception e)
            {
                MessageBox.Show("Process Cash:" + e.Message);
                return false;
            }

        }

        public bool ProcessQuickCash(Window parent, Ticket currentticket, decimal amt)
        {
            try
            {
                if(currentticket.Balance == 0 && amt == 0)
                {
                   return currentticket.TryCloseTicket(); //need to load payment  to refresh object first before trying to close ticket
                   
                }

                decimal netamount;

                //if change is given to customers, then record the balance, not the tender 
                if (amt > currentticket.Balance)
                {
                        netamount = currentticket.Balance;
                }
                else netamount = amt;


                bool succeed = PaymentModel.InsertPayment(currentticket.SalesID, "CASH", amt, netamount, "", "", "", 0, DateTime.Now, "SALE");
                if (succeed)
                {
                    currentticket.Reload();
                    GlobalSettings.CustomerDisplay.WriteDisplay("Cash:", amt, "Balance:", currentticket.Balance);
                }
                else
                    TouchMessageBox.Show("Error Inserting payment record.");


                //need to load payment  to refresh object first before trying to close ticket
                currentticket.LoadPayment();
                return currentticket.TryCloseTicket(); 
               

            }
            catch (Exception e)
            {
                MessageBox.Show("Process Quick Cash:" + e.Message);
                return false;
            }

        }

        public bool ProcessCCP(Window parent, Ticket currentticket)
        {
            switch (GlobalSettings.Instance.CreditCardProcessor.ToUpper())
            {

                case "PAX_S300":
                case "WORLDPAY":
                case "VANTIV":
                case "VIRTUAL":

                    string transtype = "SALE";

                    if (currentticket.Balance < 0) transtype = "REFUND";

                    CCPPayment ccp2 = new CCPPayment(currentticket, m_security, transtype, null,""); //pax300 and isc250 on global payments
                    Utility.OpenModal(parent, ccp2);
                    break;

   


                case "CLOVER":
                    CloverPayment pay = new CloverPayment(currentticket, m_security, "SALE", null,"");
                    Utility.OpenModal(parent, pay);
                    break;

                case "EXTERNAL":
                    CreditCardView ccv = new CreditCardView(parent, currentticket, m_security, 674);
                    Utility.OpenModal(parent, ccv);

                    break;


            }

            //need to load payment  to refresh object first before trying to close ticket
            currentticket.LoadPayment();
            return currentticket.TryCloseTicket();

        }


        public bool ProcessExternalPay(Window parent, Ticket currentticket)
        {
            try
            {
                CreditCardView ccv = new CreditCardView(parent, currentticket, m_security, 674);
                Utility.OpenModal(parent, ccv);
                //need to load payment  to refresh object first before trying to close ticket
                currentticket.LoadPayment();
                return currentticket.TryCloseTicket();

            }
            catch (Exception e)
            {
                MessageBox.Show("Process External Pay:" + e.Message);
                return false;
            }

        }

        public bool ProcessPreAuth(Window parent, Ticket currentticket)
        {

            //check to see if it already has preauth

            if(currentticket.PreAuth == null)
            {
                switch (GlobalSettings.Instance.CreditCardProcessor)
                {

                    case "PAX_S300":
                    case "WORLDPAY":
                    case "VANTIV":
   
                            CCPPayment ccp2 = new CCPPayment(currentticket, m_security, "PREAUTH", null,""); //pax300 and isc250 on global payments
                            Utility.OpenModal(parent, ccp2);
                

                        break;


                    case "Clover":
                        CloverPayment pay = new CloverPayment(currentticket, m_security, "PREAUTH", null,"");
                        Utility.OpenModal(parent, pay);
                        break;

                }

                currentticket.LoadPayment();
                return false;
            }
            else
            {
                //just update the current pre auth to AUTH
                int preauthid = currentticket.PreAuth.ID;
                currentticket.PayWithAuth();
                currentticket.LoadPayment();

                //need to load payment above first ...

                Payment payment = currentticket.GetPaymentLine(preauthid);
                //print credit slip
           
                ReceiptPrinterModel.AutoPrintCreditSlip(payment);

                return currentticket.TryCloseTicket();

            }
   

          
       
        }


  

        public bool ProcessCheck(Window parent, Ticket currentticket)
        {

            try
            {
                decimal amt;
                NumPad ccv = new NumPad("Enter Check Amount", false, false, currentticket.Balance.ToString());
                Utility.OpenModal(parent, ccv);
                if (ccv.Amount == "") return false;

                amt = decimal.Parse(ccv.Amount);
                currentticket.AddPayment("Check", amt, "", "Sale");


                //need to load payment  to refresh object first before trying to close ticket
                currentticket.LoadPayment();
                return currentticket.TryCloseTicket();

            }
            catch (Exception e)
            {
                MessageBox.Show("ProcessCheck:" + e.Message);
                return false;
            }
        }

   

        public bool ProcessHouseCredit(Window parent, Ticket currentticket)
        {

            try
            {
                decimal amt;
                NumPad ccv = new NumPad("Enter Credit Amount", false, false, currentticket.Balance.ToString());
                Utility.OpenModal(parent, ccv);
                if (ccv.Amount == "") return false;

                amt = decimal.Parse(ccv.Amount);
                currentticket.AddPayment("House Credit", amt, "", "HouseCredit");

                //need to load payment  to refresh object first before trying to close ticket
                currentticket.LoadPayment();
                return currentticket.TryCloseTicket();

            }
            catch (Exception e)
            {
                MessageBox.Show("ProcessHouseCredit:" + e.Message);
                return false;
            }
        }

        public bool ProcessRewardCredit(Window parent, Ticket currentticket)
        {
            try
            {
                RewardView ccv = new RewardView(currentticket);
                Utility.OpenModal(parent, ccv);
                //need to load payment  to refresh object first before trying to close ticket
                currentticket.LoadPayment();
                return currentticket.TryCloseTicket();
            }
            catch (Exception e)
            {
                MessageBox.Show("Process REward Credit:" + e.Message);
                return false;
            }
        }

        public bool ProcessStampCard(Window parent, Ticket currentticket)
        {
            try
            {
                decimal amt;
                NumPad ccv = new NumPad("Enter Stamp Card Amount", false, false);
                Utility.OpenModal(parent, ccv);

                if (ccv.Amount != "")
                {
                    amt = decimal.Parse(ccv.Amount);

                    currentticket.AddPayment("Stamp Card", amt, "", "Sale");

                    //need to load payment  to refresh object first before trying to close ticket
                    currentticket.LoadPayment();
                    return currentticket.TryCloseTicket();


                }
                else return false;
            }
            catch (Exception e)
            {
                MessageBox.Show("Process StampCard Credit:" + e.Message);
                return false;
            }
          
        }

        public bool ProcessGiftCard(Window parent, Ticket currentticket)
        {
            try
            {
                GiftCardView ccv = new GiftCardView(currentticket, m_security);
                Utility.OpenModal(parent, ccv);

                //need to load payment  to refresh object first before trying to close ticket
                currentticket.LoadPayment();
                return currentticket.TryCloseTicket();


            }
            catch (Exception e)
            {
               
                TouchMessageBox.Show("Process gift card:" + e.Message);
                return false;
            }
        }

        public bool ProcessGiftCertificate(Window parent, Ticket currentticket)
        {

            try
            {

                GiftCertificate num = new GiftCertificate();
                Utility.OpenModal(parent, num);

                if (num.CertificateNumber == "cancel") return false;

                //check to see if in system .. if found display Amount
                DBTicket dbticket = new DBTicket();

                decimal bal = dbticket.GetGiftCardBalance(num.CertificateNumber);
                if (bal > 0)
                {
                    TouchMessageBox.Show("Certificate Found. Amount: " + bal.ToString());
                }
                else
                {
                    if (bal == 0)
                    {
                        TouchMessageBox.Show("Certificate Found.  It has been USED. No Balance Left. ");

                    }
                    else
                    {
                        TouchMessageBox.Show("Certificate NOT FOUND!!!");
                        return false;
                    }

                }

                decimal amt;
                decimal defaultamt = 0;

                if (bal >= currentticket.Balance)
                    defaultamt = currentticket.Balance;
                else
                    defaultamt = bal;

                NumPad ccv = new NumPad("Enter Redeem Amount", false, false, defaultamt.ToString());
                Utility.OpenModal(parent, ccv);
                if (ccv.Amount == "") return false;

                amt = decimal.Parse(ccv.Amount);

                if (!GlobalSettings.Instance.AllowCashBackGiftCertificate)
                {
                    //check to see if amount is greater than balance
                    if (amt > currentticket.Balance)
                    {
                        TouchMessageBox.Show("Amount greater than balance due is not allowed!!");
                        return false;
                    }
                }

                currentticket.AddPayment("Gift Certificate", amt, num.CertificateNumber, "Sale");
                SalesModel.RedeemGiftCard(currentticket.SalesID, "Gift Certificate", amt, num.CertificateNumber, DateTime.Now);

                //need to load payment  to refresh object first before trying to close ticket
                currentticket.LoadPayment();
                return currentticket.TryCloseTicket();

            }
            catch (Exception e)
            {
                MessageBox.Show("ProcessGiftCertificate:" + e.Message);
                return false;
            }
        }

        public void ProcessLineItemAction(Window parent,SecurityModel security, string action, Ticket currentticket)
        {
            if (currentticket.CurrentLine == null) return;

            switch (action)
            {
                
                 
                //if item is already cooked ..then need override
                case "Quantity":
                    if (currentticket.CurrentLine.Sent)
                      if (security.ManagerOverrideAccess("EditSentTicket","Manager Override Needed") == false) return;
                      

                  
                    NumPad np1 = new NumPad("Enter Quantity", !currentticket.CurrentLine.AllowPartial,true);
                    Utility.OpenModal(parent, np1);


                    if (np1.Amount != "")
                    {
                        currentticket.UpdateSalesItemQuantity(currentticket.CurrentLine.ID, decimal.Parse(np1.Amount));

                    }
                    break;


                case "Weight":
                    if (currentticket.CurrentLine.Sent)
                        if (security.ManagerOverrideAccess("EditSentTicket", "Manager Override Needed") == false) return;


                    if (currentticket.CurrentLine.Weighted)
                    {
                        decimal weight = 0m;

                        NumPad npw = new NumPad("Please Enter Weight:", !currentticket.CurrentLine.AllowPartial, false);
                        Utility.OpenModal(parent, npw);

                        if (npw.Amount != "") weight = decimal.Parse(npw.Amount);
                        else return;

                        //update line weight
                        currentticket.UpdateSalesItemWeight(currentticket.CurrentLine.ID, weight);
                    }

                    break;
                case "Seat":
                    NumPad seat = new NumPad("Enter Seat Number", false, false);
                    Utility.OpenModal(parent, seat);



                    if (seat.Amount != "")
                    {
                        currentticket.UpdateSalesItemSeat(currentticket.CurrentLine.ID, decimal.Parse(seat.Amount));

                    }
                    break;

                case "Delete":

                    if (currentticket.Status == "Closed")
                    {
                        MessageBox.Show("Ticket is Closed!");
                        return;
                    }

                    if (currentticket.CurrentLine.Sent)
                    {
                        //test to see if user has security access
                        if (m_security.ManagerOverrideAccess("VoidClosedTicket", "Manager Override Needed") == false) return;

                        ConfirmAudit win = new ConfirmAudit();
                        Utility.OpenModal(parent, win);
                        if (win.Reason != "")
                        {
                            Audit.WriteLog(currentticket.CurrentEmployee.DisplayName, "Void Closed Ticket", win.Reason, "sales", currentticket.SalesID);
                        }
                        else return;

                        //Item has been sent so cannot be deleted .. has to be void
                        currentticket.VoidLineItem(currentticket.CurrentLine.ID,win.Reason, true);
                    }
                    else
                    {

                        //if it makes it to here .. then you can delete since items is not sent to kitchen
                        currentticket.DeleteLineItem(currentticket.CurrentLine.ID);
                        currentticket.CurrentLine = null;
                    }





                    break;



                case "PriceOverride":

                    if (currentticket.Status == "Closed") MessageBox.Show("Ticket is Closed!");
                    else
                    {
                        if (m_security.ManagerOverrideAccess("PriceOverride", "Manager Override Needed") == false) return;


                        NumPad np2 = new NumPad("Enter NEW Price:", false, false);
                        Utility.OpenModal(parent, np2);
                        if (np2.Amount != "")
                        {
                            if (decimal.Parse(np2.Amount) >= 0)
                            {
                                currentticket.CurrentLine.UpdatePrice(decimal.Parse(np2.Amount));
                                currentticket.LoadSeats();
                            }
                        }


                    }
                    break;






                case "Discount":
                    //NO Security on teh Discount button itself since each discount item has separate security level

                    if (currentticket.Status == "Closed")
                    {
                        MessageBox.Show("Ticket is Closed!");
                        return;
                    }


                    DiscountPad np = new DiscountPad(currentticket.CurrentLine.ProductID, currentticket.CurrentLine.Price);
                    Utility.OpenModal(parent, np);
                    if (np.ReturnPromotion == null) return;  /// user cancelled

                    switch (np.ReturnPromotion.Description)
                    {

                        case "Manual":
                            //need manual discount access
                            if (m_security.WindowAccess("Discount") == false) { return; }

                            if (np.ReturnPromotion.DiscountAmount >= 0)
                            {
                                string reason = "";
                                TextPad tp1 = new TextPad("Enter Discount Description", "");
                                Utility.OpenModal(parent, tp1);
                                reason = tp1.ReturnText;

                                currentticket.DiscountLineItem(currentticket.CurrentLine.ID, np.ReturnPromotion.DiscountAmount, "Manual Discount", reason);

                            }
                            break;

                        case "Remove":

                            currentticket.DiscountLineItem(currentticket.CurrentLine.ID, 0, "", "");
                            break;

                        default:

                            //normal predefined discounts
                            //check for security override requirements
                            if (currentticket.CurrentEmployee.SecurityLevel < np.ReturnPromotion.SecurityLevel)
                            {
                                var manager = m_security.GetEmployee(false,"Manager Override Needed");
                                if (manager.SecurityLevel < np.ReturnPromotion.SecurityLevel)
                                {
                                    TouchMessageBox.Show("Need Approval Override!!");
                                    return;
                                }
                            }

                            //check for restrictions
                            if (np.ReturnPromotion.FullPriceOnly)
                            {
                                if (currentticket.CurrentLine.SpecialPricing)
                                {
                                    TouchMessageBox.Show("This Discount can not be used on Pre-Discounted Items.");
                                    return;
                                }
                            }

                            //limited usage
                            if (np.ReturnPromotion.LimitedUseOnly && np.ReturnPromotion.UsageNumber == 0)
                            {
                                TouchMessageBox.Show("Limted Number of Usage Reached!!");
                                return;
                            }

                            if (currentticket.CurrentLine.PriceWithModifiers < np.ReturnPromotion.MinimumAmount)
                            {
                                TouchMessageBox.Show("Amount must be atleast:" + np.ReturnPromotion.MinimumAmount);
                                return;
                            }

                            if(currentticket.CurrentLine.Weight < np.ReturnPromotion.MinimumWeight)
                            {
                                TouchMessageBox.Show("Weight must be atleast:" + np.ReturnPromotion.MinimumWeight);
                                return;
                            }

                            if (currentticket.CurrentLine.Quantity < np.ReturnPromotion.MinimumQuantity)
                            {
                                TouchMessageBox.Show("Quantity must be atleast:" + np.ReturnPromotion.MinimumQuantity);
                                return;
                            }

                            //Employee Meals
                            if (np.ReturnPromotion.DiscountType == "EMPLOYEE MEAL")
                            {

                                TouchMessageBox.Show("Employee Meals must be Discounted at the Ticket level.  Click on Ticket Discount instead.");
                                return;


                            }


                            decimal discount = 0;
                            switch (np.ReturnPromotion.DiscountMethod)
                            {
                                case "PERCENT":
                                    discount = currentticket.CurrentLine.Price * currentticket.CurrentLine.Weight  * np.ReturnPromotion.DiscountAmount / 100;
                                    if (np.ReturnPromotion.MaxDiscount > 0)
                                    {
                                        if (discount > np.ReturnPromotion.MaxDiscount) discount = np.ReturnPromotion.MaxDiscount;
                                    }
                                    break;
                              

                                case "AMOUNT":
                                    discount = np.ReturnPromotion.DiscountAmount;
                                    break;

                            }


                            currentticket.DiscountLineItem(currentticket.CurrentLine.ID, discount, np.ReturnPromotion.DiscountType, np.ReturnPromotion.Description);
                            if (np.ReturnPromotion.LimitedUseOnly)
                            {
                                //reduce usage by one
                                np.ReturnPromotion.DeductUsage();
                            }
                            break;

                    }
                    np = null;


                    break;


                case "Notes":


                    TextPad tp = new TextPad("Item Notes:", currentticket.CurrentLine.Note);
                    Utility.OpenModal(parent, tp);
                    currentticket.UpdateNote(currentticket.CurrentLine.ID, tp.ReturnText);
                 

                    break;

                case "Modifiers":

                    if (currentticket.CurrentLine == null) return;


                    if (currentticket.CurrentLine.Sent)
                    {
                        if (m_security.WindowAccess("EditSentTicket") == false)
                        {

                            return;
                        }
                    }


                    Product prod = new Product(currentticket.CurrentLine.ProductID,currentticket.TicketOrderType);
                 //   if (prod.ModProfileID > 0)
                  //  {
                        SalesModifierView dlg;


                        dlg = new SalesModifierView(parent, currentticket, currentticket.CurrentLine.ID, prod,false);
                        Utility.OpenModal(parent, dlg);

                   // }


                    break;


                case "ManualModifiers":
                    if (currentticket.CurrentLine == null) return;

                    if (currentticket.CurrentLine.Sent)
                    {
                        if (m_security.WindowAccess("EditSentTicket") == false)
                        {

                            return;
                        }
                    }

                    TextPad tpmod = new TextPad("Enter Modifier Name","");
                    Utility.OpenModal(parent, tpmod);

           

                    currentticket.AddManualModifier(currentticket.CurrentLine.ID, tpmod.ReturnText,1, 10.0m);




                    break;
                case "Combo":
                    if (currentticket.CurrentLine == null) return;

                    if (currentticket.CurrentLine.Sent)
                    {
                        if (m_security.WindowAccess("EditSentTicket") == false)
                        {

                            return;
                        }
                    }


                    Product prodcombo = new Product(currentticket.CurrentLine.ProductID, currentticket.TicketOrderType);
                    if (prodcombo.ComboGroupId > 0)
                    {
                        SalesComboItemView combo;


                        combo = new SalesComboItemView(parent, currentticket, currentticket.CurrentLine.ID, prodcombo, false);
                        Utility.OpenModal(parent,combo);

                    }


                    break;
            }
        }














        public  bool DBUpdateTicketID(int salesitemid, int newsalesid)
        {

            try
            {

                //saves to ticket record on database
               
                return m_dbticket.DBUpdateTicketID(salesitemid, newsalesid);

            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket ID update:" + e.Message);
                return false;
            }

        }


        public bool UpdateTipAmount(int paymentid, decimal tipamount, decimal totalamount)
        {
            return m_dbsales.DBUpdateTipAmount(paymentid, tipamount, totalamount);
        }


        public bool UpdatePaymentCapture(int paymentid, decimal tipamount, decimal amount, decimal netamount, string transactionid)
        {
            return m_dbsales.UpdatePaymentCapture(paymentid, tipamount, amount, netamount, transactionid);
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




        //------------------------------------------------------------------  SALE / AUTH / REFUND -----------------------------------------------


        //for HeartSIP SDK -- Heartland
        public static bool InsertCreditPayment(int salesid, decimal requested_amount, ResponseEventArgs e, DateTime paymentdate,string reason)
        {
            DBTicket dbticket = new DBTicket();

            decimal authamt = 0;
            decimal cashback = 0;
            decimal tip = 0;


            if (e.AuthorizedAmount != null && e.AuthorizedAmount != "") authamt = int.Parse(e.AuthorizedAmount) / 100m;
            if (e.CashbackAmount != null && e.CashbackAmount != "") cashback = int.Parse(e.CashbackAmount) / 100m;
            if (e.TipAmount != null && e.TipAmount != "") tip = int.Parse(e.TipAmount) / 100m;

            return dbticket.DBInsertCreditPayment(salesid, requested_amount, e.CardGroup, e.ApprovalCode, e.CardType, e.MaskedPAN, e.CardAcquisition, e.ResponseId, authamt, cashback, tip, e.TransType, e.PinVerified, e.SignatureLine, e.TipAdjustAllowed, e.EMV_ApplicationName, e.EMV_Cryptogram, e.EMV_CryptogramType, e.EMV_AID, e.CardholderName, paymentdate, "", "",reason);
        }

        //for Global Payment SDK -- Heartland
        public static bool InsertCreditPayment(int salesid, decimal requested_amount, TerminalResponse resp, DateTime paymentdate, string cardgroup,string reason, string transtype)
        {
            DBTicket dbticket = new DBTicket();
            string signature = "1";  //defaults to required
            string tipadjustallowed = "0";


            var signatureRequiredCvms = new List<string> { "3", "5", "6" }; //chip card pin not verified
            var signatureRequiredStatus = new List<string> { "2", "3", "6" }; //these signature status requires a signature  ( 1 and 5 = signature capture)
            if (resp.EntryMethod.ToUpper() == "CHIP")
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


            // if (resp.TransactionType == "AUTH") tipadjustallowed = "1"; else tipadjustallowed = "0";
            if (transtype == "AUTH" || transtype == "PREAUTH") tipadjustallowed = "1"; else tipadjustallowed = "0";


            return dbticket.DBInsertCreditPayment(salesid, requested_amount, cardgroup, resp.AuthorizationCode, resp.PaymentType, resp.MaskedCardNumber, resp.EntryMethod.ToUpper(), resp.TransactionId, (decimal)resp.TransactionAmount, (decimal)resp.CashBackAmount, (decimal)resp.TipAmount, transtype, resp.CardHolderVerificationMethod, signature, tipadjustallowed, resp.ApplicationPreferredName, resp.ApplicationCryptogram, resp.ApplicationCryptogramType.ToString(), resp.ApplicationId, resp.CardHolderName, paymentdate, "", "",reason);
        }


        //payment for Clover SDK
        public static bool InsertCreditPayment(int salesid, decimal requested_amount, com.clover.sdk.v3.payments.Payment resp, DateTime paymentdate,string reason)
        {
            DBTicket dbticket = new DBTicket();
            string transtype = resp.cardTransaction.type.ToString();
            if (transtype == "AUTH") transtype = "SALE";
            else if (transtype == "PREAUTH") transtype = "AUTH";

            string cardgroup = "";
            if (resp.tender.label.ToUpper() == "DEBIT CARD") cardgroup = "DEBIT";

            if (resp.tender.label.ToUpper() == "CREDIT CARD") cardgroup = "CREDIT";

            string signature = "0";

            if (resp.cardTransaction.extra.Count > 0)
            {
                var extra = resp.cardTransaction.extra;
                var val = extra.Values.ToArray()[0].ToString();
                if (val.ToString().ToUpper() == "SIGNATURE") signature = "2";
            }
            return dbticket.DBInsertCreditPayment(salesid, requested_amount, cardgroup, resp.cardTransaction.authCode, resp.cardTransaction.cardType.ToString(), resp.cardTransaction.last4, resp.cardTransaction.entryType.ToString(), resp.cardTransaction.referenceId, (decimal)resp.amount / 100, (decimal)resp.cashbackAmount / 100, (decimal)resp.tipAmount / 100, transtype, "0", signature, "0", "", "", "", "", resp.cardTransaction.cardholderName, paymentdate, resp.id, resp.order.id,reason);
        }
        //clover doesn't mark their transaction for refund so we need to call different function instead of InsertCreditPayment
        public static bool InsertCreditRefund(int salesid, decimal requested_amount, com.clover.sdk.v3.payments.Credit resp, DateTime paymentdate, string cardgroup, string paymentid, string orderid, string reason)
        {
            DBTicket dbticket = new DBTicket();
            string transtype = "REFUND";


            return dbticket.DBInsertCreditPayment(salesid, requested_amount, cardgroup, resp.cardTransaction.authCode, resp.cardTransaction.cardType.ToString(), resp.cardTransaction.last4, resp.cardTransaction.entryType.ToString(), resp.cardTransaction.referenceId, (decimal)resp.amount / 100, 0, 0, transtype, "0", "0", "1", "", "", "", "", resp.cardTransaction.cardholderName, paymentdate, resp.id, resp.orderRef.id,reason);
        }


        //payment for Vantiv TriPOS
        public static bool InsertCreditPayment(string transtype, int salesid, decimal requested_amount,SaleResponse2 resp, DateTime paymentdate, string reason)
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
               

            //CardHolderVerificationMethod == "0" , no pin provided
            //CardHolderVerificationMethod == "1" , pin verified online
            //CardHolderVerificationMethod == "2" , off line pin
            return dbticket.DBInsertCreditPayment(salesid, resp.SubTotalAmount, resp.PaymentType.ToUpper(), resp.ApprovalNumber, resp.CardLogo.ToUpper(), MaskCardNumber(resp), resp.EntryMode.ToUpper(), resp.TransactionId, (decimal)resp.ApprovedAmount, (decimal)resp.CashbackAmount, (decimal)resp.TipAmount, transtype.ToUpper(), resp.PinVerified ? "1" : "0", signature, "0", ApplicationLabel, cryptogram, cryptogramtype, ApplicationIdentifier, resp.CardHolderName, paymentdate, "", "",reason);


        }



        //------------------------------------------------------------------- VOID -----------------------------------------------------------------

       

        //clover credit void
        public static bool VoidCreditPayment(string cloverpaymentid, string cloverorderid)
        {
            DBTicket dbticket = new DBTicket();

            return dbticket.DBVoidCreditPayment(cloverpaymentid, cloverorderid);
        }


        //others .,.. based on reference id  .. heartland PAXS300
        public static bool VoidCreditPayment(string responseid)
        {
            DBTicket dbticket = new DBTicket();

            return dbticket.DBVoidCreditPayment(responseid);
        }

        //----------------------------------------------------------------------------------------------- utilities -----------------------------------
        private static string MaskCardNumber(SaleResponse2 response)
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


        public static bool HasBeenPaid(int salesid, string paymenttype)
        {
            DBTicket dbticket = new DBTicket();
            return dbticket.HasBeenPaid(salesid, paymenttype);

        }

        public static bool GiftCardOnPayment(int salesid, string giftcardnumber)
        {
            DBTicket dbticket = new DBTicket();
            return dbticket.GiftCardOnPayment(salesid, giftcardnumber);

        }
    }
}
