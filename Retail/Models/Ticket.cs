using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Timers;
using RedDot;
using System.Drawing.Printing;
using System.Drawing;
using System.Net.Mail;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;

namespace RedDot
{
    public class Ticket : INPCBase
    {
        private ObservableCollection<LineItem> m_lineitems;
        private ObservableCollection<Payment>  m_payments;
        private int                            m_stationno;
        private int                            m_salesid;
        private int                            m_creditpaymentid;
        private int                            m_numofcustomers;
        private string                         m_note;
        private string                         m_status;
        private DBTicket                       m_dbTicket;
        private DBProducts                     m_dbproducts;
        private DBEmployee                     m_dbemployee;
        private DateTime                       m_ordertime;
        private Timer                          m_liveUpdateTimer = new Timer();
        private string                         m_orderspan;
        private string                         m_custom1;
        private string                         m_custom2;
        private string                         m_custom3;
        private string                         m_custom4;
        private decimal                        m_producttotal;
        private decimal                        m_subtotal_commission;
        private decimal                        m_labortotal;
        private decimal                      m_shippingtotal;
        private decimal                        m_subtotal;
        private decimal                        m_salestax;
        private decimal                        m_shopfee;
        private decimal                        m_total;
        private decimal                        m_taxabletotal;
        private decimal                        m_totalpayment;
        private decimal                        m_balance;
        private decimal                        m_discount;
        private decimal                        m_creditcardsurcharge;
        private decimal                        m_adjustedpayment;
        private int                            m_itemcount;
        private int                            m_parentticket;

        private AuditModel                          m_audit;
        private DateTime                       m_saledate;
        private DateTime m_lastupdated;
        private DateTime m_reversedate;
        private DateTime                       m_closedate;
        private bool                           m_taxexempt = false;

        private Employee                       m_employee;
        private Customer                       m_customer;
        private string                         m_shoptype;


        public Ticket()
        {
            Initialize();
        }

        public Ticket(int salesid)
        {

            Initialize();
            LoadTicket(salesid);
        }

        public Ticket(Employee employee)
        {

            m_employee = employee;

            Initialize();

        }



        private void Initialize()
        {

            m_dbTicket = new DBTicket();
            m_dbproducts = new DBProducts();
            m_dbemployee = new DBEmployee();
            m_lineitems = new ObservableCollection<LineItem>();
            m_payments = new ObservableCollection<Payment>();
               m_audit = new AuditModel();
            m_status = null;
            m_salesid = 0;
            m_creditpaymentid = 0;
            m_labortotal = 0;
            m_shoptype = GlobalSettings.Instance.Shop.Type;

            m_ordertime = DateTime.Now;

            m_liveUpdateTimer.Interval = 1000;
            m_liveUpdateTimer.Elapsed += new ElapsedEventHandler(LiveUpdateTimer_Elapsed);
            m_liveUpdateTimer.Start();
        }

        void LiveUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimeSpan diff = DateTime.Now - m_ordertime;
            m_orderspan = diff.ToString(@"hh\:mm\:ss");
            NotifyPropertyChanged("OrderSpan");
        }


        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        ____                            _         
        // |  _ \ _   _| |__ | (_) ___  |  _ \ _ __ ___  _ __   ___ _ __| |_ _   _ 
        // | |_) | | | | '_ \| | |/ __| | |_) | '__/ _ \| '_ \ / _ \ '__| __| | | |
        // |  __/| |_| | |_) | | | (__  |  __/| | | (_) | |_) |  __/ |  | |_| |_| |
        // |_|    \__,_|_.__/|_|_|\___| |_|   |_|  \___/| .__/ \___|_|   \__|\__, |
        //                                              |_|                  |___/ 
        //------------------------------------------------------------------------------------------
        #region public property

        private WorkOrder m_workorder;
        public WorkOrder WorkOrder
        {
            get
            {
                return m_workorder;
            }
            set
            {
                m_workorder = value;
                NotifyPropertyChanged("WorkOrder");
            }

        }


        private List<ShipOrder> m_shiporders;
        public List<ShipOrder> ShipOrders
        {
            get
            {
                return m_shiporders;
            }
            set
            {
                m_shiporders = value;
                NotifyPropertyChanged("ShipOrders");
            }

        }

        public ObservableCollection<LineItem> LineItems
        {
            get { return m_lineitems; }
            set { m_lineitems = value; NotifyPropertyChanged("LineItems"); }
        }


        public ObservableCollection<Payment> Payments
        {
            get { return m_payments; }
            set { m_payments = value; NotifyPropertyChanged("Payments"); }
        }

        public int ParentTicketID
        {
            get { return m_parentticket; }
            set { m_parentticket = value; NotifyPropertyChanged("ParentTicketID"); }
        }

        public int StationNo
        {
            get { return m_stationno; }
            set { m_stationno = value; NotifyPropertyChanged("StationNo"); }
        }
        public int SalesID
        {
            get { return m_salesid; }
            set { m_salesid = value; NotifyPropertyChanged("SalesID"); }
        }


        public string TicketNo
        {
            get { return GlobalSettings.Instance.Shop.StorePrefix + m_salesid; }
        }
        public int NumOfCustumers
        {
            get { return m_numofcustomers; }
            set { m_numofcustomers = value; NotifyPropertyChanged("NumOfCustomers"); }
        }
        public string Note
        {
            get { return m_note; }
            set { m_note = value; NotifyPropertyChanged("Note"); }
        }

        private string m_internalnote;
        public string InternalNote
        {
            get { return m_internalnote; }
            set { m_internalnote = value; NotifyPropertyChanged("InternalNote"); }
        }


        public string Custom1
        {
            get { return m_custom1; }
            set {
                m_custom1 = value;
                m_dbTicket.DBUpdateSalesString(m_salesid, "custom1", value);
                NotifyPropertyChanged("Custom1"); }
        }

        public string Custom2
        {
            get { return m_custom2; }
            set { m_custom2 = value;
            m_dbTicket.DBUpdateSalesString(m_salesid, "custom2", value); 
                NotifyPropertyChanged("Custom2");
            }
        }

        public string Custom3
        {
            get { return m_custom3; }
            set { m_custom3 = value;
            m_dbTicket.DBUpdateSalesString(m_salesid, "custom3", value); 
                NotifyPropertyChanged("Custom3");
            }
        }

        public string Custom4
        {
            get { return m_custom4; }
            set { m_custom4 = value;
            m_dbTicket.DBUpdateSalesString(m_salesid, "custom4", value); 
                NotifyPropertyChanged("Custom4");
            }
        }
        public Customer CurrentCustomer
        {
            get { return m_customer; }
            set { m_customer = value;

            if (value == null) m_dbTicket.DBUpdateCustomerID(m_salesid, 0);
            else m_dbTicket.DBUpdateCustomerID(m_salesid, value.ID);

                NotifyPropertyChanged("CurrentCustomer"); }
        }

        public string MaxRewardStr
        {
            get
            {
                if (CurrentCustomer.UsableBalance > Balance) return BalanceStr;
                else
                    return CurrentCustomer.UsableBalance.ToString();

            }
        }
        public int CreditPaymentID
        {
            get { return m_creditpaymentid; }
            set { m_creditpaymentid = value; NotifyPropertyChanged("CredidPaymentID"); }
        }

        public Employee CurrentEmployee
        {
            get { return m_employee; }
            set
            {
                m_employee = value;
                NotifyPropertyChanged("CurrentEmployee");
            }
        }

        public string Status
        {
            get
            {
                 return m_status;
            }
            set
            {
                m_status = value;
                NotifyPropertyChanged("Status");
            }
        }

        public bool HasPayment
        {
            get
            {
               
                foreach (Payment pay in Payments)
                    if (!pay.Voided) return true;

                return false;
            }
        }


        public bool HasShipping
        {
            get
            {
                foreach (LineItem line in LineItems)
                    if (line.ItemType == "shipping") return true;

                return false;
            }
        }

        public bool IsRefund
        {
            get
            {
                if (ParentTicketID > 0) return true; else return false;
            }
        }

        public bool TaxExempt
        {
            get { return m_taxexempt; }
            set
            {
                m_taxexempt = value;
                NotifyPropertyChanged("TaxExempt");
            }
        }

        public DateTime OrderTime
        {
            get
            {
                return m_ordertime;
            }
            set
            {
                m_ordertime = value;
                NotifyPropertyChanged("OrderTime");
            }
        }
        public DateTime CloseDate
        {
            get { return m_closedate; }
            set
            {
                m_closedate = value;
                NotifyPropertyChanged("CloseDate");
            }
        }
        public DateTime SaleDate
        {
            get { return m_saledate; }
            set
            {
                m_saledate = value;
                NotifyPropertyChanged("SaleDate");
            }
        }

        public DateTime ReverseDate
        {
            get { return m_reversedate; }
            set
            {
                m_reversedate = value;
                NotifyPropertyChanged("ReverseDate");
            }
        }

        public DateTime LastUpdated
        {
            get { return m_lastupdated; }
            set
            {
                m_lastupdated = value;
                NotifyPropertyChanged("LastUpdated");
            }
        }

        public string OrderSpan
        {
            get
            {
                return m_orderspan;
            }
        }

        public decimal ProductTotal
        {
            get { return m_producttotal; }
            set { m_producttotal = value; NotifyPropertyChanged("ProductTotal"); }
        }

        public decimal SubTotalCommission
        {
            get { return m_subtotal_commission; }
            set { m_subtotal_commission = value; NotifyPropertyChanged("SubTotalCommission"); }
        }

        public decimal ShippingTotal
        {
            get { return m_shippingtotal; }
            set { m_shippingtotal = value; NotifyPropertyChanged("ShippingTotal"); }
        }

        public decimal LaborTotal
        {
            get { return m_labortotal; }
            set { m_labortotal = value; NotifyPropertyChanged("LaborTotal"); }
        }

        public decimal SubTotal
        {
            get { return m_subtotal; }
            set { m_subtotal = value; NotifyPropertyChanged("SubTotal"); }
        }

        public decimal TaxableTotal
        {
            get { return m_taxabletotal; }
            set { m_taxabletotal = value; NotifyPropertyChanged("TaxableTotal"); }
        }

        public decimal Total
        {
            get { return m_total; }
            set { m_total = value; NotifyPropertyChanged("Total"); }
        }

        public decimal ShopFee
        {
            get { return m_shopfee; }
            set { m_shopfee = value; NotifyPropertyChanged("ShopFee"); }
        }

  

        public decimal Discount
        {
            get { return m_discount; }
            set
            {
                m_discount = value;
                NotifyPropertyChanged("Discount");
                RecalculateTotal();
            }
        }

        public int ItemCount
        {
            get { return m_itemcount; }
            set
            {
                m_itemcount = value;
                NotifyPropertyChanged("ItemCount");
            }
        }



        public decimal SalesTax
        {
            get
            {
                return m_salestax;
            }

            set
            {
                m_salestax = value;
                NotifyPropertyChanged("SalesTax");
            }
        }

        public decimal TotalPayment
        {
            get
            {
                return m_totalpayment;
            }

            set
            {
                m_totalpayment = value;
                NotifyPropertyChanged("TotalPayment");
            }
        }

        public decimal CreditCardSurcharge
        {
            get { return m_creditcardsurcharge; }
            set
            {
                m_creditcardsurcharge = value;
                NotifyPropertyChanged("CreditCardSurcharge");
                RecalculateTotal();
            }
        }

        public decimal AdjustedPayment
        {
            get { return m_adjustedpayment; }
            set
            {
                m_adjustedpayment = value;
                NotifyPropertyChanged("AdjustedPayment");
              
            }
        }
        public decimal Balance
        {
            get
            {
                return m_balance;
            }

            set
            {
                m_balance = value;
                NotifyPropertyChanged("Balance");

            }
        }

        public string BalanceStr
        {
            get
            {
                return String.Format("{0:0.00}", m_balance);
            }
        }

        public int CustomerID
        {
            get
            {
                if (CurrentCustomer != null) return CurrentCustomer.ID;
                else return 0;
            }
        }

        public int EmployeeID
        {
            get
            {
                if (CurrentEmployee != null) return CurrentEmployee.ID;
                else return 0;
            }
        }



        #endregion public property

       

        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        __  __      _   _               _     
        // |  _ \ _   _| |__ | (_) ___  |  \/  | ___| |_| |__   ___   __| |___ 
        // | |_) | | | | '_ \| | |/ __| | |\/| |/ _ \ __| '_ \ / _ \ / _` / __|
        // |  __/| |_| | |_) | | | (__  | |  | |  __/ |_| | | | (_) | (_| \__ \
        // |_|    \__,_|_.__/|_|_|\___| |_|  |_|\___|\__|_| |_|\___/ \__,_|___/
        //    
        //------------------------------------------------------------------------------------------  
        #region public methods


        public bool HasBeenPaid(string paymenttype)
        {

            bool paid = false;
            foreach (Payment pt in m_payments)
            {
                if (pt.Description == paymenttype) paid = true;
            }
            return paid;

        }

        public static string GetTicketStatus(int salesid)
        {
            DBTicket dbticket = new DBTicket();
            return dbticket.GetTicketStatus(salesid);

        }

        public static decimal GetGiftCardBalance(string accountno)
        {
            DBTicket dbticket = new DBTicket();
            return dbticket.GetGiftCardBalance(accountno);

        }

        public static int GetCustomerID(int salesid)
        {

            DBTicket dbticket = new DBTicket();
            return dbticket.GetCustomerID(salesid);
        }


        public bool CheckforBalance()
        {
         
            if (Status == "Closed")
            {

                Decimal dec = 0m;

        
                dec = Balance * (-1m);
                if (dec > 0)
                    MessageBox.Show(String.Format("Change Due: {0:c}", dec));
                return true;
            }
            else return false;

        }


        public bool PendingTicket()
        {


            try
            {
                if (Status == null) return false;

                 

                    Status = "Pending";
                    m_dbTicket.DBPendingTicket(m_salesid);


                    foreach (LineItem line in LineItems)
                    {


                        if (line.ItemType == "product")
                        {
                            m_dbproducts.DBDeductInventory(line.ProductID, line.Quantity);

                        }

                    }



                    if (CurrentCustomer != null) CurrentCustomer.ReloadHistory();
      

                    return true;
             
            }
            catch (Exception e)
            {
                MessageBox.Show("Pending Ticket:" + e.Message);
                return false;
            }
        }

        public bool ReOpenTicket()
        {


            try
            {
                if (Status == null) return false;

             


                    Status = "Open";
                    m_dbTicket.DBReOpenTicket(m_salesid);


                    foreach (LineItem line in LineItems)
                    {


                        if (line.ItemType == "product")
                        {
                            m_dbproducts.DBAddToInventory(line.ProductID, line.Quantity);

                        }

                    }



                    if (CurrentCustomer != null) CurrentCustomer.ReloadHistory();

                    return true;


            }
            catch (Exception e)
            {
                MessageBox.Show("ReOpen Ticket:" + e.Message);
                return false;
            }
        }
        public bool CloseTicket(bool force = false)
        {
      

            try
            {
                if (Status == null) return false;

                //test to see if total payments is equal or greater than subtotal or doing a force close
                if (m_totalpayment >= m_total || force)
                {
                    m_dbTicket.DBActivateGiftCards(m_salesid); //runs everytime ticket close 

                    Status = "Closed";
                    m_dbTicket.DBCloseTicket(m_salesid,m_subtotal, m_total, m_salestax, m_shopfee, m_producttotal, m_labortotal);
                    

                    foreach(LineItem line in LineItems)
                    {


                        if(line.ItemType=="product")
                        {
                            m_dbproducts.DBDeductInventory(line.ProductID, line.Quantity);

                        }

                   

                    }


             
                    if(CurrentCustomer !=null) CurrentCustomer.ReloadHistory();
                    if (GlobalSettings.Instance.AutoReceiptPrint) PrintReceipt();
                    if (GlobalSettings.Instance.OpenDrawOnClose) OpenDrawer();
               
                    

                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                MessageBox.Show("Close Ticket:" + e.Message);
                return false;
            }
        }

        public bool ReverseTicket()
        {
            try
            {
                m_dbTicket.DBReverseTicket(SalesID);
                Reload();
                return true;

            }
            catch (Exception e)
            {
                MessageBox.Show("Reverse Ticket:" + e.Message);
                return false;
            }

        }


        public bool CreateTicket(int parentid)
        {
            try
            {
                if (m_employee == null) return false;

                SalesID = m_dbTicket.DBCreateTicket(parentid,m_employee.ID,GlobalSettings.Instance.StationNo);
                Status = "Open";
                LoadTicket(m_salesid); //we call load ticket just so we can refresh the screen
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                SalesID = 0;
                Status = null;
                return false;
            }

        }

 

        public bool GiftCardOnPayment(string giftcardnumber)
        {
            foreach (Payment line in Payments)
            {
                if (line.AuthorCode == giftcardnumber)
                {
                    return true;
                }
            }
            return false;
        }

        public bool GiftCardOnLineItem(string giftcardnumber)
        {
            foreach (LineItem line in LineItems)
            {
                if (line.Custom1 == giftcardnumber)
                {
                    return true;
                }
            }
            return false;
        }


        public LineItem AddProductLineItem(bool AddSurcharge, Product prod,decimal price, int quantity, string note, string custom1,string custom2,string custom3,string custom4 )
        {
            int result;


            try
            {
                if (m_status == "Closed") return null;
                //if gift card, then check to see if it is already in ticket
                if (prod.Type == "giftcard")
                {
                    if (GiftCardOnLineItem(custom1))
                    {
                        MessageBox.Show("Gift Card already on ticket..");
                        return null;
                    }
                }



                //insert line item into database
                if (AddSurcharge)
                    result = m_dbTicket.DBAddProductLineItem(prod.ID, m_salesid, prod.Description, quantity, price, prod.Surcharge, prod.Discount, prod.Type,0, note, prod.CommissionType, custom1, custom2, custom3, custom4, prod.ReportCategory, prod.PartNumber, prod.ModelNumber, prod.Cost,prod.MSRP, prod.CommissionAmt,prod.TaxExempt,prod.BarCode);
                else
                    result = m_dbTicket.DBAddProductLineItem(prod.ID, m_salesid, prod.Description, quantity, price, 0, prod.Discount, prod.Type, 0, note, prod.CommissionType, custom1, custom2, custom3, custom4, prod.ReportCategory, prod.PartNumber, prod.ModelNumber, prod.Cost, prod.MSRP, prod.CommissionAmt, prod.TaxExempt, prod.BarCode);


                LoadLineItem();

                return GetLineItemLine(result);

            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket - AddProductLineItem:" + e.Message);
                return null;
            }

        }

      



        /*
                public bool AddServiceLineItem(Service service, int quantity, int employeeid, string note)
                {
                    bool result;


                    try
                    {
                        if (_status == "Closed") return false;

                        //insert line item into database
                        result = _dbTicket.DBAddServiceLineItem(_salesid, service.Description, quantity, service.Price, service.DiscountAmount,service.Type, employeeid, note);


                        LoadLineItem();



                        return result;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Ticket - AddServiceLineItem:" + e.Message);
                        return false;
                    }

                }
                */

        public bool DeleteLineItem(int id)
        {
            bool result;


            try
            {
                if (m_status == "Closed") return false;

                result = m_dbTicket.DBDeleteLineItem(id);
                LoadLineItem();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("DeleteLineItem:" + e.Message);
                return false;
            }

        }



        public bool VoidTicket(string reason)
        {
            bool result;


            try
            {
                if (m_status == "Closed") return false;

                //remove all item from ticket
                result = m_dbTicket.DBVoidTicket(m_salesid);
  

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("VoidTicket:" + e.Message);
                return false;
            }

        }

        public bool VoidPayment(int id)
        {
            bool result;

            try
            {
                if (Status == "Closed") return false;


                //voids payment .. not delete
                result = m_dbTicket.DBDeletePayment(id);

                LoadPayment();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Void Payment:" + e.Message);
                return false;
            }

        }
        public bool UpdateSalesItemQuantity(int salesitemid, int quantity)
        {
            bool result;
            try
            {
                if (Status == "Closed") return false;

                result = m_dbTicket.DBUpdateSalesItemQuantity(salesitemid, quantity);

                LoadLineItem();
                return result;

            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket: Update SalesItem Quantity" + e.Message);
                return false;
            }

        }
        public bool OverrideLineItemPrice(int id, decimal amount)
        {
            bool result;


            try
            {
                if (Status == "Closed") return false;


                result = m_dbTicket.DBUpdateSalesItemPrice(id, amount);
                LoadLineItem();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Override Line Item Price:" + e.Message);
                return false;
            }

        }

        public bool UpdateCost(int id, decimal amount)
        {
            bool result;


            try
            {
                //if (Status == "Closed") return false;


                result = m_dbTicket.DBUpdateSalesItemValue(id, "cost", amount);
                LoadLineItem();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Update Cost:" + e.Message);
                return false;
            }

        }
        public bool DiscountLineItem(int id, decimal amount)
        {
            bool result;


            try
            {
                if (Status == "Closed") return false;


                result = m_dbTicket.DBUpdateSalesItemDiscount(id, amount);
                LoadLineItem();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Discount Line Item:" + e.Message);
                return false;
            }

        }

        public bool LineItemSurcharge(int id, decimal amount)
        {
            bool result;


            try
            {
                if (Status == "Closed") return false;


                result = m_dbTicket.DBUpdateSalesItemValue(id, "surcharge", amount);
                LoadLineItem();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Surcharge Line Item:" + e.Message);
                return false;
            }

        }





        public bool UpdateInternalNote( string note)
        {
            try
            {
                m_dbTicket.DBUpdateSalesString(SalesID, "internalnote", note);
                Reload();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Update Internal Note:" + e.Message);
                return false;
            }


        }

        public bool UpdateNote(string note)
        {
            try
            {
                m_dbTicket.DBUpdateSalesString(SalesID, "note", note);
                Reload();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Update Note:" + e.Message);
                return false;
            }


        }




        public bool UpdateLineItemNote(int salesitemid, string note)
        {
            try
            {
                m_dbTicket.DBUpdateSalesItemString(salesitemid, "note", note);
                Reload();
                return true;
            }catch(Exception e)
            {
                MessageBox.Show("UpdateNote:" + e.Message);
                return false;
            }


        }


        public bool UpdateLineItemInternalNote(int salesitemid, string note)
        {
            try
            {
                m_dbTicket.DBUpdateSalesItemString(salesitemid, "internalnote", note);
                Reload();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Update Intenral Note:" + e.Message);
                return false;
            }


        }


        public bool AddDiscount(decimal amount)
        {

            try
            {

                m_dbTicket.DBUpdateDiscount(m_salesid, amount);

                Discount = amount;

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket:AddDiscount:" + e.Message);
                return false;
            }
        }

        public void Reload() //same as Refresh below
        {

            LoadTicket(m_salesid);
        }


        public void LoadTicket(int salesid)
        {
            int customerid;

            try
            {
                if (salesid == 0) return; //ticket id not passed


                SalesID = salesid;
                DataTable dt = m_dbTicket.GetSalesTicket(salesid);


                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    if (row["parentid"].ToString() != "") ParentTicketID = int.Parse(row["parentid"].ToString());
                    if (row["StationNo"].ToString() != "") StationNo = int.Parse(row["StationNo"].ToString());
                    if (row["discount"].ToString() != "") Discount = decimal.Parse(row["discount"].ToString());
                    if (row["taxexempt"].ToString() != "") TaxExempt = row["taxexempt"].ToString().Equals("1");

                    Note = row["Note"].ToString();
                    InternalNote = row["internalnote"].ToString();

                    Status = row["Status"].ToString();
                    Custom1 = row["custom1"].ToString();
                    Custom2 = row["custom2"].ToString();
                    Custom3 = row["custom3"].ToString();
                    Custom4 = row["custom4"].ToString();

                    SaleDate = (DateTime)row["saledate"];

                    if (row["closedate"].ToString() != "") CloseDate = (DateTime)row["closedate"];
                    if (row["reversedate"].ToString() != "") ReverseDate = (DateTime)row["reversedate"];




                    if (row["EmployeeID"].ToString() != "") CurrentEmployee = new Employee(int.Parse(row["EmployeeID"].ToString()));

                    if (row["customerid"].ToString() != "")
                    {
                        customerid = int.Parse(row["customerid"].ToString());
                        if (customerid > 0)
                        {
                            CurrentCustomer = new Customer(customerid,true);
                        }
                    }


                    LoadLineItem();
                    LoadPayment();
                    LoadWorkOrder();
                   
                }
                else
                { //ticket not found
                    SalesID = 0;
                    Status = null;

                }


            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket:LoadTicket:" + e.Message);
            }

        }



        //Updates the Ticket with the current Server name
        public void ChangeServer(int employeeid)
        {
            try
            {
                m_dbTicket.DBUpdateEmployeeID(m_salesid, employeeid);
                CurrentEmployee = new Employee(employeeid);

            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket:Change Server:" + e.Message);
            }

        }

        //Updates the Ticket Items with the selected server name , used in Nail Salon to give each service a tech name
        public void UpdateSalesItemEmployeeID(int salesitemid, int employeeid)
        {

            try
            {

                m_dbTicket.DBUpdateSalesItemEmployeeID(salesitemid, employeeid);

                LoadLineItem();
               

            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket: Update SalesItem Employee ID:" + e.Message);
            }

        }

 




   
        public static void UpdateCustomerID(int salesid, int customerid)
        {

            try
            {

                //saves to ticket record on database
                DBTicket dbticket = new DBTicket();
                dbticket.DBUpdateCustomerID(salesid, customerid);

            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket: Update Customer (telephone):" + e.Message);
            }

        }

        public void LoadWorkOrder()
        {
            var wo = m_dbTicket.GetWorkOrder(SalesID);
            if(wo != null)
                if(wo.Rows.Count > 0)
                     WorkOrder = new WorkOrder(wo.Rows[0]);
        }

        public void CreateWorkOrder(Security sec)
        {
           
             if(sec.HasAccess("WorkOrder"))
            {
                m_dbTicket.CreateWorkOrder(SalesID, "enclosure", sec.CurrentEmployee.FullName);
                LoadWorkOrder();
            }
        

        }
        public void LoadLineItem()
        {
            ObservableCollection<LineItem> lineitems;
            DataTable data_receipt;
            LineItem line;

            decimal shopfeepercent;

            decimal producttotal = 0;
            decimal subtotal_commission = 0;
            decimal labortotal = 0;
            decimal shippingtotal = 0;
            decimal taxabletotal = 0;
            int itemcount = 0;
 

            try
            {

                shopfeepercent = GlobalSettings.Instance.ShopFeePercent;
                
                lineitems = new ObservableCollection<LineItem>();

                if (SalesID == 0)
                {
                    MessageBox.Show("No SalesID ");
                    return;
                }

                //load purchased item that are products
                data_receipt = m_dbTicket.GetLineItems(m_salesid, "product");

                //load ticket item from sales item table
                foreach (DataRow row in data_receipt.Rows)
                {
                    itemcount++;
                    line = new LineItem(row);
                    lineitems.Add(line);
                    producttotal = producttotal + line.AdjustedPrice * line.Quantity;
                    if (!line.TaxExempt) taxabletotal = taxabletotal + line.AdjustedPrice * line.Quantity;
                    if(line.CommissionType.ToUpper() == "PERCENT" ) subtotal_commission = subtotal_commission + line.Price * line.Quantity;
                }

                //load purchased item that are giftcard
                data_receipt = m_dbTicket.GetLineItems(m_salesid, "giftcard");
                //load ticket item from product table
                foreach (DataRow row in data_receipt.Rows)
                {
                    itemcount++;
                    line = new LineItem(row);
                    lineitems.Add(line);
                    //adjustedprice = gross price + surcharge - discounts

                    producttotal = producttotal + line.AdjustedPrice * line.Quantity;

                    if (line.CommissionType.ToUpper() == "PERCENT") subtotal_commission = subtotal_commission + line.Price * line.Quantity;
                }

                //---------------------------------------------------------------SERVICE / LABOR ------------------------------
      
                    line = new LineItem("======Labor======");
                    lineitems.Add(line);
            


                //load purchased item that are services
                data_receipt = m_dbTicket.GetLineItems(m_salesid, "service");

                //load ticket item from product table
                foreach (DataRow row in data_receipt.Rows)
                {
                    itemcount++;
                    // line = new LineItem(int.Parse(row["id"].ToString()), int.Parse(row["quantity"].ToString()), row["Description"].ToString().Trim(), (decimal)row["price"], (decimal)row["discount"]);
                    line = new LineItem(row);
                    lineitems.Add(line);
                    labortotal = labortotal + line.AdjustedPrice * line.Quantity;
                    if (!line.TaxExempt) taxabletotal = taxabletotal + line.AdjustedPrice * line.Quantity;
                    subtotal_commission = subtotal_commission + line.Price * line.Quantity;
                }


                //---------------------------------------------------------------SHIPPING ------------------------------

                line = new LineItem("=====Shipping=====");
                lineitems.Add(line);



                //load purchased item that are services
                data_receipt = m_dbTicket.GetLineItems(m_salesid, "shipping");

                //load ticket item from product table
                foreach (DataRow row in data_receipt.Rows)
                {
                    itemcount++;
                    // line = new LineItem(int.Parse(row["id"].ToString()), int.Parse(row["quantity"].ToString()), row["Description"].ToString().Trim(), (decimal)row["price"], (decimal)row["discount"]);
                    line = new LineItem(row);
                    lineitems.Add(line);
                    shippingtotal = shippingtotal + line.AdjustedPrice * line.Quantity;
                    if (!line.TaxExempt) taxabletotal = taxabletotal + line.AdjustedPrice * line.Quantity;
                    subtotal_commission = subtotal_commission + line.Price * line.Quantity;
                }











                //Assign new value  to ticket object
                ProductTotal = producttotal;

                LaborTotal = labortotal;
                ShippingTotal = shippingtotal;
                SubTotalCommission = subtotal_commission;
                ShopFee = Math.Round(labortotal * shopfeepercent * 0.01m, 2);
                TaxableTotal = taxabletotal;


  

                RecalculateTotal();

                LineItems = lineitems;
                ItemCount = itemcount;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Load LineItem:" + ex.Message);
            }


        }

        private void RecalculateTotal()   //--------------------------------------------------------------RECALCULATE TOTAL ------------------------------------------------------------------------------------
        {
            if(Status != "Voided")
            {
                if (TaxExempt) SalesTax = 0;
                else SalesTax = Math.Round((TaxableTotal + ShopFee - Discount) * GlobalSettings.Instance.SalesTaxPercent * 0.01m, 2);  //shop fee are also charged a sales tax

                SubTotal = ProductTotal + LaborTotal + ShippingTotal +  ShopFee - Discount;
                Total = SubTotal + SalesTax;
                Balance = Total - TotalPayment;
                AdjustedPayment = TotalPayment + CreditCardSurcharge;
            }else
            {
                SalesTax = 0;
              
                SubTotal = 0;
                Total = 0;
                Balance = 0;
                AdjustedPayment = 0;
            }
         
        }


        public Payment GetPaymentLine(int paymentid)
        {

            foreach (Payment pay in Payments)
            {
                if (pay.ID == paymentid) return pay;
            }
            return null;
        }

        public void UpdatePayment(int paymentid, DateTime date, string paytype , decimal amount)
        {
            m_dbTicket.DBUpdatePayment(paymentid, date, paytype, amount);
        }

        public LineItem GetLineItemLine(int id)
        {

            foreach (LineItem line in LineItems)
            {
                if (line.ID == id) return line;
            }
            return null;
        }





        public bool AddPayment(string paytype, decimal amount, string authorizeCode)
        {

            try
            {
                decimal netamount;



                //if change is given to customers, then record the balance, not the tender 
                if (amount > Balance) netamount = Balance;
                else netamount = amount;



                m_dbTicket.DBInsertPayment(m_salesid, paytype, amount, netamount, authorizeCode);


                LoadPayment();

                VFD.WriteDisplay(paytype + ":", amount, "Balance:", m_balance);

                CloseTicket(); //need to load payment  to refresh object first before trying to close ticket
                NotifyPropertyChanged("Payments");
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("AddPayment:" + e.Message);
                return false;
            }
        }



        public void LoadPayment()
        {
            DataTable data_payments;
            decimal totalpayment;
            decimal totalcreditcardsurcharge = 0;


            try
            {

                totalpayment = 0;

                Payments.Clear();


                data_payments = m_dbTicket.GetPayments(m_salesid);


                foreach (DataRow row in data_payments.Rows)
                {
                    totalcreditcardsurcharge = totalcreditcardsurcharge + m_dbTicket.GetCreditCardSurcharge(row["description"].ToString(), (decimal)row["netamount"]);

                    if (IsCreditPayment(row["description"].ToString().Trim()) )
                    {
                        CreditPaymentID = int.Parse(row["id"].ToString());
                     

                    }

                    Payment pay = new Payment(int.Parse(row["id"].ToString()), row["description"].ToString(), (decimal)row["amount"], (decimal)row["netamount"], row["authorcode"].ToString(), Convert.ToBoolean(row["void"]));
                    pay.PaymentDate = (DateTime)row["paymentdate"];

                    if (row["voiddate"].ToString() != "") pay.VoidDate = (DateTime)row["voiddate"];

                    Payments.Add(pay);

                    if(!pay.Voided)
                    {
                        totalpayment = totalpayment + (decimal)row["amount"];
                    }
                   
                }

                TotalPayment = totalpayment;
                CreditCardSurcharge = Math.Round(totalcreditcardsurcharge,2);

                RecalculateTotal();


                NotifyPropertyChanged("BalanceVisibility");
                NotifyPropertyChanged("ChangeDueVisibility");

            }
            catch (Exception ex)
            {
                MessageBox.Show("LoadPayment:" + ex.Message);
            }


        }

        private bool IsCreditPayment(string paymenttype)
        {
            if (paymenttype == "Credit") return true;
            if (paymenttype == "Visa") return true;
            if (paymenttype == "American Express") return true;
            if (paymenttype == "Mastercard") return true;
            if (paymenttype == "Discover") return true;
            return false;
        }

   
          

        public void PrintReceipt()
        {

            if (GlobalSettings.Instance.ReceiptPrinterType.ToUpper() == "LARGEFORMAT")
            {
                PrintLargeReceipt();
                return;
            }


            string receiptline;
            string printername = GlobalSettings.Instance.ReceiptPrinter;
            Location store = GlobalSettings.Instance.Shop;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
            if (printername == "none") return;


            //58mm printer = 32 chars  , 80mm printer = 48 chars
            ReceiptPrinter printer = new ReceiptPrinter(printername);

            try
            {

                if (store == null)
                {

                    MessageBox.Show("Shop/store info missing");
                    return;
                }

                

                printer.Center();
               // printer.PrintStoredLogo();   image printing not workign so gonna set auto logo on printer
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();




                //customer info
                if (CurrentCustomer != null )
                {
                    printer.Left();
                    printer.PrintLF(CurrentCustomer.FirstName + " " + CurrentCustomer.LastName);
                   // printer.PrintLF(CurrentCustomer.Address1);
                   // printer.PrintLF(CurrentCustomer.City + "," + CurrentCustomer.State + " " + CurrentCustomer.ZipCode);
                    printer.PrintLF(CurrentCustomer.Phone1);
                    

                }

                printer.PrintLF(GlobalSettings.Instance.SalesCustomName1 + Custom1);
                printer.PrintLF(GlobalSettings.Instance.SalesCustomName2 + Custom2);
                printer.PrintLF(GlobalSettings.Instance.SalesCustomName3 + Custom3);

                printer.PrintLF(new String('=', receiptwidth));
                printer.DoubleHeight();
                if(IsRefund)
                {
                   
                    printer.PrintLF("****---REFUND---****");
                }else
                {
                    printer.PrintLF("****Sales Ticket****");
                }

                printer.DoubleHeightOFF();
                printer.PrintLF(new String('=', receiptwidth));

                printer.Left();

                if (CurrentEmployee.DisplayName == "[None]") printer.PrintLF(Utility.FormatPrintRow(" ", "Ticket #:" + store.StorePrefix + SalesID, receiptwidth));
                else printer.PrintLF(Utility.FormatPrintRow(CurrentEmployee.Role + ":" + CurrentEmployee.DisplayName, "Ticket #:" + store.StorePrefix + SalesID, receiptwidth));

     

                printer.Left();

                printer.PrintLF(Utility.FormatPrintRow(SaleDate.ToShortDateString(), SaleDate.ToShortTimeString(), receiptwidth));
                printer.PrintLF(new String('-', receiptwidth));




                //starts to print each item on ticket

                decimal itemtotal;
                string receiptstr = "";

                foreach (LineItem line in LineItems)
                {
                    //wether or not to include the modifier prices in the lineitem pricing
                     itemtotal = line.PriceSurcharge * line.Quantity;
                 

                    receiptstr = line.ModelNumber + "--" + line.Description;
                    if (receiptstr.Length > 35) receiptstr = receiptstr.Substring(0, 35) + "...";

                    if (line.Quantity != 0) receiptline = Utility.FormatPrintRow(line.Quantity + " " + receiptstr, String.Format("{0:0.00}", itemtotal), receiptwidth);
                    else receiptline = Utility.FormatPrintRow(line.Description, "", receiptwidth);

                    printer.PrintLF(receiptline);
                    if (line.Discount > 0)
                    {

                        receiptline = Utility.FormatPrintRow("     **Promo Discount**", String.Format("-{0:0.00}", line.Discount), receiptwidth);
                        printer.PrintLF(receiptline);
                    }
 
                    if (line.Note != "")
                    {
                        printer.PrintLF(line.Note);
                    }



                }

                printer.PrintLF(new String('-', receiptwidth));
                printer.LineFeed();

                    printer.PrintLF(Utility.FormatPrintRow("PARTS:", String.Format("{0:0.00}", ProductTotal), receiptwidth));
                    printer.PrintLF(Utility.FormatPrintRow("LABOR:", String.Format("{0:0.00}", LaborTotal), receiptwidth));
                    printer.PrintLF(Utility.FormatPrintRow("SHOP FEE:", String.Format("{0:0.00}",ShopFee), receiptwidth));
                printer.PrintLF(Utility.FormatPrintRow("SHIPPING:", String.Format("{0:0.00}", ShippingTotal), receiptwidth));
                printer.PrintLF(Utility.FormatPrintRow("", "------------", receiptwidth));



                

                  printer.PrintLF(Utility.FormatPrintRow("SUBTOTAL:", String.Format("{0:0.00}", SubTotal), receiptwidth));


                 printer.PrintLF(Utility.FormatPrintRow("ADJUST:", String.Format("{0:0.00}", Discount), receiptwidth));

                 printer.PrintLF(Utility.FormatPrintRow("SALES TAX:", String.Format("{0:0.00}", SalesTax), receiptwidth));

               // printer.PrintLF(Utility.FormatPrintRow("SERVICE:", String.Format("{0:0.00}", LaborTotal), receiptwidth));


       
                printer.PrintLF(Utility.FormatPrintRow("", "============", receiptwidth));
                printer.PrintLF(Utility.FormatPrintRow("TOTAL:", String.Format("{0:0.00}", Total), receiptwidth));

                printer.LineFeed();



                foreach (Payment line in Payments)
                {
                    receiptline = Utility.FormatPrintRow(line.PaymentDate.ToShortDateString() + " " +  line.Description + ":", line.AmountStr, receiptwidth);

                    printer.PrintLF(receiptline);

                }



                printer.PrintLF(Utility.FormatPrintRow("", "============", receiptwidth));

                if (Balance >= 0)
                {
                    printer.PrintLF(Utility.FormatPrintRow("BALANCE:", String.Format("{0:0.00}", Math.Round(Balance, 2)), receiptwidth));
                }
                else
                {
                    printer.PrintLF(Utility.FormatPrintRow("CHANGE:", String.Format("{0:0.00}", Math.Round(Balance * (decimal)(-1.0), 2)), receiptwidth));
                }


                if (CreditCardSurcharge > 0)
                {

                    printer.PrintLF(Utility.FormatPrintRow("CC Surcharge:", String.Format("{0:0.00}", CreditCardSurcharge), receiptwidth));
                    printer.PrintLF(Utility.FormatPrintRow("Adj. Payment:", String.Format("{0:0.00}", AdjustedPayment), receiptwidth));

                 }

                //Payment notice

                printer.PrintLF(GlobalSettings.Instance.PaymentNotice);
 
                //Receipt notice
                printer.PrintLF(GlobalSettings.Instance.ReceiptNotice);


                if (GlobalSettings.Instance.ReceiptPrintReward)
                {
                    if (CurrentCustomer != null)
                    {
                        if (CurrentCustomer.ID > 0)
                        {
                            printer.LineFeed();
                            if(CurrentCustomer.Phone1.Length > 4) printer.PrintLF(Utility.FormatPrintRow("Customer:",CurrentCustomer.ID.ToString() + ":" + CurrentCustomer.Phone1.Substring(CurrentCustomer.Phone1.Length - 4 , 4), receiptwidth));
                            printer.PrintLF(Utility.FormatPrintRow("REWARD BALANCE:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.RewardBalance, 2)), receiptwidth));
                        }


                    }


                }

                printer.PrintLF(Note);

                printer.Center();

                if (Status == "Closed")
                {
                    printer.LineFeed();
                    printer.PrintLF("PAID IN FULL");
                }
                printer.LineFeed();
                printer.PrintLF("     THANK YOU!");
                printer.LineFeed();
                printer.LineFeed();
                printer.Send(); //sends buffer to printer


                printer.Cut();

            }
            catch (Exception ex)
            {
                MessageBox.Show("PrintReceipt:" + ex.Message);
            }

        }

     

      


        public void EmailPDF()
        {

            try
            {

                string attachfile;
                string workorderfile="";
                string AppPath;
                string attachments;


                AppPath = System.AppDomain.CurrentDomain.BaseDirectory;

                attachfile = AppPath + "pdf\\Ticket" + TicketNo + ".pdf";

                attachments = "Ticket" + TicketNo + ".pdf";

                if (WorkOrder != null)
                {
                    workorderfile = AppPath + "pdf\\WorkOrder" + WorkOrder.WorkOrderNo + ".pdf";
                    attachments += " + WorkOrder" + WorkOrder.WorkOrderNo + ".pdf";
                }
                 

                if (!File.Exists(attachfile))
                {
                    MessageBox.Show("PDf file not found:  Please Print PDF first");
                    return;
                }

                EmailCustomer eml = new EmailCustomer(CurrentCustomer.Email, "Ticket:" + TicketNo, "Your Ticket:" + TicketNo + " has been attached", attachments);
                eml.ShowDialog();

                if (eml.Action == "cancel") return;


                if (eml.To.Length < 1)
                {
                    MessageBox.Show("Address is invalid");
                    return;
                }

                MailMessage mail = new MailMessage(GlobalSettings.Instance.StoreEmail, eml.To);
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = GlobalSettings.Instance.SMTPServer;
                client.Credentials = new System.Net.NetworkCredential(GlobalSettings.Instance.SMTPUserName, GlobalSettings.Instance.SMTPPassword);

                MailAddress copy = new MailAddress(GlobalSettings.Instance.SMTPCopyTo);
                mail.CC.Add(copy);

                mail.Subject = eml.Subject;
                mail.Body = eml.Message;
                mail.Attachments.Add(new Attachment(attachfile));
                if (WorkOrder != null)
                    if (File.Exists(workorderfile))
                        mail.Attachments.Add(new Attachment(workorderfile));


                client.Send(mail);
                MessageBox.Show("Email Sent!");
            }catch(Exception e)
            {
                MessageBox.Show("Email: " + e.Message);
            }

            
        }
        public void PrintLargeReceipt()
        {
            try
            {
                PrintDocument pdoc = null;
                System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();
                pdoc = new PrintDocument();
                // PrinterSettings ps = new PrinterSettings();
                // Font font = new Font("Courier New", 15);
                pd.UseEXDialog = true;

                //PaperSize psize = new PaperSize("Custom", 600, 800);
                //ps.DefaultPageSettings.PaperSize = psize;

                pd.Document = pdoc;
                // pd.Document.DefaultPageSettings.PaperSize = psize;
                //pdoc.DefaultPageSettings.PaperSize.Height =320;
                //pdoc.DefaultPageSettings.PaperSize.Height = 820;

                // pdoc.DefaultPageSettings.PaperSize.Width = 520;

                pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintLargeReceipt);
                pdoc.PrinterSettings.PrinterName = GlobalSettings.Instance.LargeFormatPrinter;

                //pdoc.Print();


                //pdoc.PrintPage += (sender, e) => PrintReport(e, id, lcDate, lcEndDate);  //this method allows you to pass parameters


                /*
                            System.Windows.Forms.DialogResult result;
                            System.Windows.Forms.PrintPreviewDialog pp = new System.Windows.Forms.PrintPreviewDialog();
                            pp.PrintPreviewControl.Zoom = 1;
                            pp.Document = pdoc;
                            pp.WindowState = System.Windows.Forms.FormWindowState.Maximized;

                            result = pp.ShowDialog();
                            if (result == System.Windows.Forms.DialogResult.OK)
                            {
                                pdoc.Print();
                            }

                */

                pdoc.Print(); 
            }catch(Exception ex)
            {
                MessageBox.Show("Printing Error:" + ex.Message);
            }
        


        }

        

        void pdoc_PrintLargeReceipt(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Font font = new Font("Courier New", 7, System.Drawing.FontStyle.Bold);
            Font fontitalic = new Font("Courier New", 7, System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold);
            Font fontbold = new Font("Courier New", 8, System.Drawing.FontStyle.Bold);
            // Create pen.
            Pen blackPen = new Pen(Color.Black, 2);
            Brush blackBrush = new SolidBrush(Color.Black);
            Location store = GlobalSettings.Instance.Shop;

            int fontHeight = (int)font.GetHeight();
            int fontBoldHeight = (int)fontbold.GetHeight();

            int startX = 50;
            int startY = 20;
            int XOffset = 0;
            int YOffset = 0;
            int rightLimit = 750;
            string receiptstr;

            int savedY = 0;


            try
            {
                if (store == null)
                {

                    MessageBox.Show("Shop/store info missing");
                    return;
                }


                string bitmapfile = GlobalSettings.Instance.StoreLogo;
                if(File.Exists(bitmapfile))
                    if (GlobalSettings.Instance.StoreLogo !="")   graphics.DrawImage(new Bitmap(bitmapfile), startX, startY, 300, 50);




                XOffset = 500;
                YOffset = 20;
                //customer invoide #
                PrintRightAlign(graphics, "Invoice #" + TicketNo, fontbold, rightLimit, startY + YOffset);

                YOffset = YOffset + fontBoldHeight;
                //Ticket Date and Time
                PrintRightAlign(graphics, SaleDate.ToString(), font, rightLimit, startY + YOffset);

                YOffset = YOffset + fontBoldHeight;
                graphics.DrawLine(blackPen, startX, startY + YOffset, startX + 700, startY + YOffset);

                YOffset = YOffset + fontHeight;
                savedY = YOffset;
                PrintRightAlign(graphics, store.Address1, font, rightLimit, startY + YOffset);

                YOffset = YOffset + fontHeight;
                PrintRightAlign(graphics, store.City + "," + store.State + " " + store.Zip, font, rightLimit, startY + YOffset);

                YOffset = YOffset + fontHeight;
                PrintRightAlign(graphics, store.Phone, font, rightLimit, startY + YOffset);

                //customer info
                if (CurrentCustomer != null)
                {
                    //YOffset = YOffset + fontHeight;
                    YOffset = savedY;
                    graphics.DrawString(CurrentCustomer.FirstName + " " + CurrentCustomer.LastName, font, new SolidBrush(Color.Black), startX, startY + YOffset);
                    YOffset = YOffset + fontHeight;
                    graphics.DrawString(CurrentCustomer.Address1, font, new SolidBrush(Color.Black), startX, startY + YOffset);
                    YOffset = YOffset + fontHeight;

                    if (CurrentCustomer.Address2.Trim() != "")
                    {
                        graphics.DrawString(CurrentCustomer.Address2, font, new SolidBrush(Color.Black), startX, startY + YOffset);
                        YOffset = YOffset + fontHeight;
                    }
                    graphics.DrawString(CurrentCustomer.City + "," + CurrentCustomer.State + " " + CurrentCustomer.ZipCode, font, new SolidBrush(Color.Black), startX, startY + YOffset);
                    YOffset = YOffset + fontHeight;
                    graphics.DrawString(CurrentCustomer.Phone1, font, new SolidBrush(Color.Black), startX, startY + YOffset);

                }
                else
                {
                    //YOffset = YOffset + fontHeight * 3;
                    YOffset = savedY;
                    graphics.DrawString("[Customer info ...]", fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);


                }

                //custom info
                YOffset = YOffset + fontHeight;
                graphics.DrawString(GlobalSettings.Instance.SalesCustomName1  + Custom1, font, new SolidBrush(Color.Black), startX , startY + YOffset);

               // YOffset = YOffset + fontHeight;
                graphics.DrawString(GlobalSettings.Instance.SalesCustomName2 + Custom2, font, new SolidBrush(Color.Black), startX + 200, startY + YOffset);

               // YOffset = YOffset + fontHeight;
                graphics.DrawString(GlobalSettings.Instance.SalesCustomName3  + Custom3, font, new SolidBrush(Color.Black), startX + 400, startY + YOffset);
               
                YOffset = YOffset + fontHeight;
       
                if(IsRefund)
                {
                    YOffset = YOffset + fontHeight;
                    graphics.DrawString("****************************************************************************", fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);
                    YOffset = YOffset + fontHeight;
                    graphics.DrawString("***************************      REFUND       ******************************", fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);
                    YOffset = YOffset + fontHeight;
                    graphics.DrawString("****************************************************************************", fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);
                }

                //Title for ticket items
                YOffset = YOffset + fontHeight;
                graphics.DrawString("Description", fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);
                PrintRightAlign(graphics, "Qty", fontbold, rightLimit - 160, startY + YOffset);
                PrintRightAlign(graphics, "Price", fontbold, rightLimit - 80, startY + YOffset);
                PrintRightAlign(graphics, "Total", fontbold, rightLimit, startY + YOffset);
                // line beneath title line
                YOffset = YOffset + fontBoldHeight;
                graphics.DrawLine(blackPen, startX, startY + YOffset, startX + 700, startY + YOffset);

                YOffset = YOffset + fontHeight/2;
                //loop through items
                foreach (LineItem line in LineItems)
                {
                    

                    if (line.Quantity != 0)
                    {
                        //reduce description if it's too long so doesn't overwrite the quantity and price
                        receiptstr = line.ModelNumber ;
                        if (receiptstr.Length > 70) receiptstr = receiptstr.Substring(0, 70) + "...";
                            graphics.DrawString(receiptstr, font, new SolidBrush(Color.Black), startX, startY + YOffset);
                         




                           PrintRightAlign(graphics, line.Quantity.ToString(), font, rightLimit - 160, startY + YOffset);
                            PrintRightAlign(graphics, line.PriceSurcharge.ToString(), font, rightLimit - 80, startY + YOffset);
                            PrintRightAlign(graphics, (line.PriceSurcharge * line.Quantity).ToString(), font, rightLimit, startY + YOffset);

                        YOffset = YOffset + fontHeight;
                        receiptstr = line.Description;
                        if (receiptstr.Length > 140) receiptstr = receiptstr.Substring(0, 140) + "...";
                        graphics.DrawString(receiptstr, fontitalic, new SolidBrush(Color.Gray), startX + 5, startY + YOffset);

                        //prints discount
                        if (line.Discount > 0)
                            {
                                YOffset = YOffset + fontHeight;
                                graphics.DrawString("     **Promo Discount**", fontitalic, new SolidBrush(Color.Gray), startX + 5, startY + YOffset);
                                PrintRightAlign(graphics,  String.Format("-{0:0.00}", line.Discount * line.Quantity), font, rightLimit, startY + YOffset);
                            }
 
                       if (line.Note.Length > 0)
                       {
                           YOffset = YOffset + fontHeight;
                            PrintLeftAlign(graphics, line.Note, fontitalic, new SolidBrush(Color.Gray), startX + 5, startY + YOffset, fontHeight,345);

                            int rtncount = line.Note.Count(x => x == (char)13);
                           YOffset = YOffset + fontHeight*rtncount;
                       }


                    }
                    else
                    {
                        // =====Service===== line
                       
                        graphics.DrawString(line.Description, fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);
                       
                    }

                    YOffset = YOffset + fontHeight;
                }

                XOffset = 400;

                YOffset = YOffset + fontHeight ;
                graphics.DrawLine(blackPen, startX + XOffset, startY + YOffset, startX + 700, startY + YOffset); //line below items

                YOffset = YOffset + fontHeight;

             
            
                PrintLeftAlign(graphics, Note, fontitalic, new SolidBrush(Color.Black),startX, startY + YOffset,fontHeight, 150);

                graphics.DrawString("Parts:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                PrintRightAlign(graphics, ProductTotal.ToString(), font, rightLimit, startY + YOffset);

                YOffset = YOffset + fontHeight;
                graphics.DrawString("Labor:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                PrintRightAlign(graphics, LaborTotal.ToString(), font, rightLimit, startY + YOffset);

                YOffset = YOffset + fontHeight;
                graphics.DrawString("Shop Fee:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                PrintRightAlign(graphics, ShopFee.ToString(), font, rightLimit, startY + YOffset);

                YOffset = YOffset + fontHeight;
                graphics.DrawString("Shipping:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                PrintRightAlign(graphics, ShippingTotal.ToString(), font, rightLimit, startY + YOffset);



                YOffset = YOffset + fontHeight;
                graphics.DrawString("Adjustments:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                PrintRightAlign(graphics,"-" +  Discount.ToString(), font, rightLimit, startY + YOffset);

                YOffset = YOffset + fontHeight * 2;
                graphics.DrawLine(blackPen, startX + XOffset, startY + YOffset, startX + 700, startY + YOffset); //line below Discount


                YOffset = YOffset + fontHeight;
                graphics.DrawString("Sub-Total:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                PrintRightAlign(graphics, SubTotal.ToString(), font, rightLimit, startY + YOffset);

  

                YOffset = YOffset + fontHeight;
                graphics.DrawString("Sales Tax:" + (TaxExempt?"  (Tax Exempt)":""), fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
               PrintRightAlign(graphics, SalesTax.ToString(), font, rightLimit, startY + YOffset);

                YOffset = YOffset + fontHeight;
                graphics.DrawString("Total:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                PrintRightAlign(graphics, Total.ToString(), font, rightLimit, startY + YOffset);

               

                //Payments
                YOffset = YOffset + fontHeight * 2;
                graphics.DrawString("Payments:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                YOffset = YOffset + fontHeight;
                //loop through payments

                foreach (Payment pay in Payments)
                {
                    YOffset = YOffset + fontHeight;

                    graphics.DrawString(pay.PaymentDate.ToShortDateString() + " " +  pay.Description, font, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                    PrintRightAlign(graphics, pay.AmountStr, font, rightLimit, startY + YOffset);

                }

 

                YOffset = YOffset + fontBoldHeight ;
                graphics.DrawLine(blackPen, startX + XOffset, startY + YOffset, startX + 700, startY + YOffset); //line below payments


                YOffset = YOffset + fontHeight;
                graphics.DrawString("Total Payments:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                PrintRightAlign(graphics, TotalPayment.ToString(), font, rightLimit, startY + YOffset);

                YOffset = YOffset + fontBoldHeight ;
                graphics.DrawLine(blackPen, startX + XOffset, startY + YOffset, startX + 700, startY + YOffset);
                graphics.DrawLine(blackPen, startX + XOffset, startY + YOffset + 5, startX + 700, startY + YOffset + 5); //double lines

                YOffset = YOffset + fontHeight;
                graphics.DrawString("Balance:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                PrintRightAlign(graphics, BalanceStr, font, rightLimit, startY + YOffset);

                if(CreditCardSurcharge > 0)
                {
                    YOffset = YOffset + fontHeight * 3;
                    graphics.DrawString("CC Surcharge:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                    PrintRightAlign(graphics, CreditCardSurcharge.ToString(), font, rightLimit, startY + YOffset);

                    YOffset = YOffset + fontHeight;
                    graphics.DrawString("Adj. Payment:", fontbold, new SolidBrush(Color.Black), startX + XOffset, startY + YOffset);
                    PrintRightAlign(graphics, AdjustedPayment.ToString(), font, rightLimit, startY + YOffset);

                }

                //Payment notice
                YOffset = YOffset + fontHeight * 4;
                graphics.DrawString(GlobalSettings.Instance.PaymentNotice, fontitalic, new SolidBrush(Color.Gray), startX, startY + YOffset);


                //Receipt notice
                YOffset = YOffset + fontHeight * 4;
                graphics.DrawString(GlobalSettings.Instance.ReceiptNotice, fontitalic, new SolidBrush(Color.Gray), startX, startY + YOffset);
               // PrintLeftAlign(graphics, GlobalSettings.Instance.ReceiptNotice, font, startX, startY + YOffset, fontHeight, 350);
            

            }
            catch (Exception ex)
            {
                MessageBox.Show("pdoc_PrintPage:" + ex.Message);
            }


        }

        private void PrintRightAlign(Graphics graphics, string receiptline, Font font, int rightlimit, int Y)
        {
            graphics.DrawString(receiptline, font, new SolidBrush(Color.Black), rightlimit - graphics.MeasureString(receiptline, font).Width, Y);
        }


        private void PrintRightAlign(XGraphics graphics, string receiptline, XFont font, int rightlimit, int Y)
        {
            graphics.DrawString(receiptline, font, new SolidBrush(Color.Black), rightlimit - graphics.MeasureString(receiptline, font).Width, Y);
        }

        private int PrintLeftAlign(XGraphics graphics, string receiptline, XFont font,Brush brush, int X, int Y, int height,int width)
        {

            List<string> lines = WrapText(receiptline.Trim(), width, font.Name, (float)font.Size);
            int YOffset = Y;


            foreach (var item in lines)
            {
                graphics.DrawString(item, font, brush, X, YOffset);
                YOffset = YOffset + height;
            }

            return YOffset;
        }

        private int PrintLeftAlign(Graphics graphics, string receiptline, Font font,Brush brush, int X, int Y, int height,int width)
        {

            List<string> lines = WrapText(receiptline, width, font.Name, font.Size);
            int YOffset = Y;



            foreach (var item in lines)
            {
                graphics.DrawString(item, font,brush, X, YOffset);
                YOffset = YOffset + height;
            }

            return YOffset;
        }

        static List<string> WrapText(string text, double pixels, string fontFamily, float emSize)
        {
            string[] originalLines = text.Split(new string[] { " " }, StringSplitOptions.None);

            List<string> wrappedLines = new List<string>();

            StringBuilder actualLine = new StringBuilder();
            double actualWidth = 0;

            foreach (var item in originalLines)
            {
                System.Windows.Media.FormattedText formatted = new System.Windows.Media.FormattedText(item,
                    CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight,
                    new System.Windows.Media.Typeface(fontFamily), emSize, System.Windows.Media.Brushes.Black);

                actualLine.Append(item + " ");
                actualWidth += formatted.Width;

                if (actualWidth > pixels)
                {
                    wrappedLines.Add(actualLine.ToString());
                    actualLine.Clear();
                    actualWidth = 0;
                }
            }

            if (actualLine.Length > 0)
                wrappedLines.Add(actualLine.ToString());

            return wrappedLines;
        }

        public void OpenDrawer()
        {
            string printername = GlobalSettings.Instance.ReceiptPrinter;

            if (printername == "none") return;
            ReceiptPrinter printer = new ReceiptPrinter(printername);
            printer.CashDrawer();
            printer = null;
        }

        public void SetTaxExempt(bool taxexempt)
        {
            if(TaxExempt != taxexempt)
            {
                TaxExempt = taxexempt;
                m_dbTicket.DBUpdateSalesValue(SalesID, "taxexempt", taxexempt ? 1 : 0);
                Reload();
            }

        }

        #endregion public methods
    }
}
