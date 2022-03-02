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
using GlobalPayments.Api.Terminals;
using System.Runtime.Serialization;

namespace RedDot
{
    [CollectionDataContract]
    public class Ticket : INPCBase
    {
      

 
        private int m_salesid;
        private int m_creditpaymentid;
 
        private string m_note;
        private string m_status;
        private DBTicket m_dbTicket;
        private DBProducts m_dbproducts;
        private DBEmployee m_dbemployee;

        

        private string m_custom1;
        private string m_custom2;
        private string m_custom3;
        private string m_custom4;
        private string m_discounttype;
        private string m_discountname;
        private decimal m_producttotal;
         private decimal m_subtotal;
        private decimal m_salestax;
        private decimal m_discountedsalestax;
        private decimal m_discountedtotal;

        private decimal m_total;
        private decimal m_taxabletotal;
        private decimal m_totalpayment;
        private decimal m_balance;
   
        private decimal m_creditcardsurcharge;
        private decimal m_adjustedpayment;
        private int m_itemcount;

        private int m_tablenumber;
        private int m_customercount;

        private DateTime m_saledate;
        private DateTime m_closedate;
        private bool m_taxexempt = false;

        private Employee m_employee;
        private Customer m_customer;
     
        private DateTime m_ordertime;


        private ObservableCollection<Seat> m_seats;
        private ObservableCollection<Payment> m_payments;


        //----------------------------------------Constructor --------------------------
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
         
            m_payments = new ObservableCollection<Payment>();
            m_licenses = new ObservableCollection<DriverLicense>();

            m_status = null;
            m_salesid = 0;
            m_creditpaymentid = 0;
           
 
      
            m_ordertime = DateTime.Now;

       

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

        [DataMember]
        public bool AllFired { get; set; }


        private OrderType m_ordertype;
        [DataMember]
        public OrderType TicketOrderType
        {
            get { return m_ordertype; }
            set
            {
                m_ordertype = value;
                NotifyPropertyChanged("TicketOrderType");
                NotifyPropertyChanged("OrderTypeStr");
            }
        }

        private SubOrderType m_subordertype;
        [DataMember]
        public SubOrderType TicketSubOrderType
        {
            get { return m_subordertype; }
            set
            {
                m_subordertype = value;
                NotifyPropertyChanged("TicketSubOrderType");
                NotifyPropertyChanged("OrderTypeStr");
            }
        }

        [DataMember]
        public string OrderTypeStr
        {
            get
            {
                

                switch(TicketOrderType)
                {
       
                    case OrderType.ToGo: return "ToGo Order-" + ( TicketSubOrderType == SubOrderType.CallIn ? "Call-In" :(TicketSubOrderType == SubOrderType.WalkIn ? "Walk-In":"Drive-Thru"));
                    case OrderType.Bar: return "Bar Service";
                    case OrderType.DineIn: return "Dine In " + (TicketSubOrderType == SubOrderType.CounterService ? "-Counter" : "");
                    case OrderType.Delivery: return "Delivery";

                }
                return "Dine In"; 
            }
        }
        public Visibility m_ischangedue;

        public Visibility IsChangeDue
        {
            get { return m_ischangedue; }
            set { m_ischangedue = value;
                NotifyPropertyChanged("IsChangeDue"); }
        }


        [DataMember]
        public ObservableCollection<Seat> Seats
        {
            get { return m_seats; }
            set { m_seats = value;
                NotifyPropertyChanged("Seats"); }
        }


        [DataMember]
        public ObservableCollection<Payment> Payments
        {
            get { return m_payments; }
            set { m_payments = value;
                NotifyPropertyChanged("Payments"); }
        }



        private ObservableCollection<DriverLicense> m_licenses;
        [DataMember]
        public ObservableCollection<DriverLicense> Licenses
        {
            get { return m_licenses; }
            set
            {
                m_licenses = value;
                NotifyPropertyChanged("Licenses");
            }

        }
        private Payment m_preauth;
        [DataMember]
        public Payment PreAuth
        {
            get { return m_preauth; }
            set { m_preauth = value;
                NotifyPropertyChanged("PreAuth"); }
        }

        private string m_customername;
        [DataMember]
        public string CustomerName
        {
            get { return m_customername; }
            set { m_customername = value;
                NotifyPropertyChanged("CustomerName"); }
        }

        [DataMember]
        public int CustomerCount
        {
            get { return m_customercount; }
            set { m_customercount = value;
                NotifyPropertyChanged("CustomerCount"); }
        }

        [DataMember]
        public int TableNumber
        {
            get { return m_tablenumber; }
            set { m_tablenumber = value;
                NotifyPropertyChanged("TableNumber");
                NotifyPropertyChanged("OrderType");
                NotifyPropertyChanged("TableNumberStr"); }
        }




        [DataMember]
        public string TableNumberStr
        {
            get {
                if (m_tablenumber > 0) return m_tablenumber.ToString(); else return "";

            }
          
        }




        [DataMember]
        public int SalesID
        {
            get { return m_salesid; }
            set { m_salesid = value;
                NotifyPropertyChanged("SalesID"); }
        }

        private int m_ordernumber;
        [DataMember]
        public int OrderNumber
        {
            get { return m_ordernumber; }
            set { m_ordernumber = value;
                NotifyPropertyChanged("OrderNumber"); }
        }

        private int m_stationno;
        [DataMember]
        public int StationNo
        {
            get { return m_stationno; }
            set { m_stationno = value;
                NotifyPropertyChanged("StationNo"); }
        }
        [DataMember]
        public string Note
        {
            get { return m_note; }
            set { m_note = value;
                NotifyPropertyChanged("Note"); }
        }
        [DataMember]
        public string Custom1
        {
            get { return m_custom1; }
            set {
                m_custom1 = value;
                m_dbTicket.DBUpdateSalesString(m_salesid, "custom1", value);
                NotifyPropertyChanged("Custom1"); }
        }
        [DataMember]
        public string Custom2
        {
            get { return m_custom2; }
            set { m_custom2 = value;
            m_dbTicket.DBUpdateSalesString(m_salesid, "custom2", value); 
                NotifyPropertyChanged("Custom2");
            }
        }
        [DataMember]
        public string Custom3
        {
            get { return m_custom3; }
            set { m_custom3 = value;
            m_dbTicket.DBUpdateSalesString(m_salesid, "custom3", value); 
                NotifyPropertyChanged("Custom3");
            }
        }
        [DataMember]
        public string Custom4
        {
            get { return m_custom4; }
            set { m_custom4 = value;
            m_dbTicket.DBUpdateSalesString(m_salesid, "custom4", value); 
                NotifyPropertyChanged("Custom4");
            }
        }
        [DataMember]
        public string DiscountName
        {
            get { return m_discountname; }
            set
            {
                m_discountname = value;
                NotifyPropertyChanged("DiscountName");
                NotifyPropertyChanged("DiscountStr");
            }
        }
        [DataMember]
        public string DiscountStr
        {
            get
            {
                return DiscountName !="" ?  "(" +  DiscountName + ")" : "Discount";
            }
        }
        [DataMember]
        public string DiscountType
        {
            get { return m_discounttype; }
            set
            {
                m_discounttype = value;
                NotifyPropertyChanged("DiscountType");
            }
        }

        public Customer CurrentCustomer
        {
            get { return m_customer; }
            set
            {
                m_customer = value;

                if (value == null) m_dbTicket.DBUpdateCustomerID(m_salesid, 0);
                else m_dbTicket.DBUpdateCustomerID(m_salesid, value.ID);

                NotifyPropertyChanged("CurrentCustomer");
            }
        }

        /*
        [DataMember]
        public string MaxRewardStr
        {
            get
            {
                if (CurrentCustomer.UsableBalance > Balance) return BalanceStr;
                else
                    return CurrentCustomer.UsableBalance.ToString();

            }
        }
        */



        [DataMember]
        public bool HasDebitPayment { get; set; }
        [DataMember]
        public bool HasCreditPayment { get; set; }
        [DataMember]
        public bool HasCashPayment { get; set; }
        [DataMember]
        public bool HasVoidedDebitPayment { get; set; }
        [DataMember]
        public bool HasVoidedCreditPayment { get; set; }

        [DataMember]
        public bool HasAuthorCode { get; set; }
        [DataMember]
        public bool HasExternalGiftPayment { get; set; }



        [DataMember]
        public bool SentToKitchen { get; set; }
        [DataMember]
        public bool HasAUTH { get; set; }

        [DataMember]
        public string Server
        {
            get
            {
                if (CurrentEmployee != null) return CurrentEmployee.DisplayName; else return "";

            }
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
        [DataMember]
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
        [DataMember]
        public string FireStatus
        {
            get
            {
                if (AllFired) return "AllFired";
            
                if(HasHoldDate)
                {
                    //check to see it it's past date
                    if ((DateTime.Now - HoldDate).Minutes > 2) return "Alert";
                }


               return "NeedToSend";
            }
         
        }
        [DataMember]
        public bool HasPayment
        {
            get
            {
                if (Payments.Count > 0) return true; else return false;
            }
        }

        private bool m_hasdiscount;
        [DataMember]
        public bool HasDiscount
        {
            get
            {
                return m_hasdiscount;
            }
        }
        [DataMember]
        public bool TaxExempt
        {
            get { return m_taxexempt; }
            set
            {
                m_taxexempt = value;
                NotifyPropertyChanged("TaxExempt");
            }
        }

        [DataMember]
        public DateTime CloseDate
        {
            get { return m_closedate; }
            set
            {
                m_closedate = value;
                NotifyPropertyChanged("CloseDate");
            }
        }
        [DataMember]
        public DateTime SaleDate
        {
            get { return m_saledate; }
            set
            {
                m_saledate = value;
                NotifyPropertyChanged("SaleDate");
            }
        }
        [DataMember]
        public bool HasHoldDate
        {
            get
            {
                if (HoldDate != DateTime.MinValue && AllFired == false) return true; else return false;
            }
        }

        private DateTime m_holddate;
        [DataMember]
        public DateTime HoldDate
        {
            get { return m_holddate; }
            set
            {
                m_holddate = value;
                NotifyPropertyChanged("HoldDate");
            }
        }

        [DataMember]
        public string HoldDateStr
        {
            get
            {
                TimeSpan difference = HoldDate -  DateTime.Now ;
                if (difference.TotalDays > 100) return "Indefinite";
                else  return HoldDate.ToShortTimeString();
            }
        }
        [DataMember]
        public string ElapsedTime
        {
            get
            {
                TimeSpan difference = DateTime.Now - SaleDate;
                string rtn = " [ " +  Math.Round(difference.TotalMinutes).ToString() + " Mins ]";
                return rtn;
              
            }
        }


        [DataMember]
        public decimal SubTotal
        {
            get { return m_subtotal; }
            set { m_subtotal = value; NotifyPropertyChanged("SubTotal"); }
        }
        [DataMember]
        public decimal TaxableTotal
        {
            get { return m_taxabletotal; }
            set { m_taxabletotal = value; NotifyPropertyChanged("TaxableTotal"); }
        }
        [DataMember]
        public decimal Total
        {
            get { return m_total; }
            set { m_total = value; NotifyPropertyChanged("Total"); }
        }

  

        private decimal m_discount;
        [DataMember]
        public decimal Discount
        {
            get { return m_discount; }
            set
            {
                m_discount = value;
                NotifyPropertyChanged("Discount");
               // RecalculateTotal();
            }
        }






        [DataMember]
        public int ActiveItemCount
        {
            get { return m_itemcount; }
            set
            {
                m_itemcount = value;
                NotifyPropertyChanged("ActiveItemCount");
            }
        }

        private int m_allitemcount;
        [DataMember]
        public int AllItemCount
        {
            get { return m_allitemcount; }
            set
            {
                m_allitemcount = value;
                NotifyPropertyChanged("AllItemCount");
            }
        }

        private int m_voideditemcount;
        [DataMember]
        public int VoidedItemCount
        {
            get { return m_voideditemcount; }
            set
            {
                m_voideditemcount = value;
                NotifyPropertyChanged("VoidedItemCount");
            }
        }

        [DataMember]
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

        [DataMember]
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

        private decimal m_cashpayment;
        [DataMember]
        public decimal CashPayment
        {
            get
            {
                return m_cashpayment;
            }

            set
            {
                m_cashpayment = value;
                NotifyPropertyChanged("CashPayment");
            }
        }

        private decimal m_cashnetpayment;
        [DataMember]
        public decimal CashNetPayment
        {
            get
            {
                return m_cashnetpayment;
            }

            set
            {
                m_cashnetpayment = value;
                NotifyPropertyChanged("CashNetPayment");
            }
        }
        [DataMember]
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
        [DataMember]
        public decimal AdjustedPayment
        {
            get { return m_adjustedpayment; }
            set
            {
                m_adjustedpayment = value;
                NotifyPropertyChanged("AdjustedPayment");
              
            }
        }

        [DataMember]
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

        private decimal m_changedue;

        [DataMember]
        public decimal ChangeDue
        {
            get { return m_changedue; }
            set
            {
                m_changedue = value;
                NotifyPropertyChanged("ChangeDue");
            }
        }

        decimal m_tipamount;
        [DataMember]
        public decimal TipAmount
        {
            get
            {
                return m_tipamount;
            }

            set
            {
                m_tipamount = value;
                NotifyPropertyChanged("TipAmount");
            }
        }


        decimal m_autotipamount;
        [DataMember]
        public decimal AutoTipAmount
        {
            get
            {
                return m_autotipamount;
            }

            set
            {
                m_autotipamount = value;
                NotifyPropertyChanged("AutoTipAmount");
            }
        }


        [DataMember]
        public string BalanceStr
        {
            get
            {
                return String.Format("{0:0.00}", m_balance);
            }
        }
        [DataMember]
        public int CustomerID
        {
            get
            {
                if (CurrentCustomer != null) return CurrentCustomer.ID;
                else return 0;
            }
        }

        [DataMember]
        public bool IsClosed
        {
            get
            {
                if (Status == "Closed")
                {
                    return true;
                }
                else return false;
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
                if (pt.CardGroup == paymenttype && pt.Voided == false) paid = true;
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


        public void ForceClose()
        {
            if (Status == null) return;

            Status = "Closed";
            m_dbTicket.DBCloseTicket(m_salesid, m_subtotal, m_total, m_salestax,m_autotipamount);
            Reload(); //reloades ticket

        }


        public bool TryCloseTicket()
        {
     


            try
            {
                if (Status == null) return false;

               // bool WasReversed = (Status == "Reversed");

                //test to see if total payments is equal or greater than subtotal or doing a force close
                if (m_totalpayment >= m_total )
                {
                    m_dbTicket.DBActivateGiftCards(m_salesid); //runs everytime ticket close , will not double insert
                    m_dbTicket.DBActivateGiftCertificates(m_salesid); //runs everytime ticket close , will not double insert


                  

                    Status = "Closed";

              
                   m_dbTicket.DBCloseTicket(m_salesid, m_subtotal, m_total, m_salestax, m_autotipamount);

 


                    Reload(); //reloades ticket
                    AssignOrderNumber();

                    if (TableNumber >= 0) //togo or dine in .. not for ECR sales
                    {
                        ReceiptPrinterModel.SendKitchen(this);
                    }
                    else
                    {
                        //ECR so no need to send to kitchen but still need to mark as sent
                        MarkAsSent();
                    }


                    if (CurrentCustomer !=null) CurrentCustomer.ReloadHistory();

                    //open drawer,then tell user change due amount
                    if (GlobalSettings.Instance.OpenDrawOnClose && HasCashPayment) OpenDrawer();


                    if (ChangeDue > 0)
                        TouchMessageBox.ShowSmall(String.Format("Change Due: {0:c}", ChangeDue), GlobalSettings.Instance.AutoAskTimeout);


                

          


                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Close Ticket:" + e.Message);
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
                TouchMessageBox.Show("Reverse Ticket:" + e.Message);
                return false;
            }

        }


        public bool CreateTicket(OrderType ordertype,SubOrderType subordertype, int tablenumber)
        {
            try
            {
                if (m_employee == null) return false;

                SalesID = m_dbTicket.DBCreateTicketQS(m_employee.ID,tablenumber,ordertype,subordertype,GlobalSettings.Instance.StationNo);
                Status = "Open";
                LoadTicket(m_salesid); //we call load ticket just so we can refresh the screen
                return true;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show(e.Message);
                SalesID = 0;
                Status = null;
                return false;
            }

        }

        public void AssignOrderNumber()
        {
            if(OrderNumber ==0)
            {
                m_dbTicket.AssignOrderNumber(SalesID);
                LoadTicket(m_salesid);
            }
          
        }

        public void UpdateStatus(TicketStatus status)
        {
            m_dbTicket.UpdateStatus(SalesID, status);
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
            foreach(Seat seat in Seats)
            foreach (LineItem line in seat.LineItems)
            {
                if (line.Custom1 == giftcardnumber)
                {
                    return true;
                }
            }
            return false;
        }

        public TicketTotals GetSelectedItemTotal(bool tipselected)
        {
            TicketTotals total = new TicketTotals();
            decimal producttotal = 0;


            decimal taxtotal = 0;

            decimal taxabletotal = 0;


            foreach (Seat emp in Seats)
                foreach (LineItem line in emp.LineItems)
                {
                    //selected so see as a whole .. ignore modifiers
                    if (line.Selected)
                    {
                        producttotal = producttotal + line.TotalAdjustedPriceWithModifier;  //includes quantity

                        //doesn't matter if its a product or giftcard , only cares if it's marked as taxable
                        if (line.Taxable)
                            taxabletotal = taxabletotal + line.TotalAdjustedPriceWithModifier;
                    }else
                    {
                        //look at modifiers if not selected
                        foreach(SalesModifier mod in line.Modifiers)
                        {
                            if(mod.Selected)
                            {
                                producttotal = producttotal + mod.TotalPrice;
                                if (line.Taxable)
                                    taxabletotal = taxabletotal + mod.TotalPrice;
                            }
                          
                        }


                    }

                }


            decimal tip = 0;
            if (tipselected)
                foreach (Payment pay in Payments)
                    tip += pay.TipAmount;

            taxtotal = Math.Round(taxabletotal * GlobalSettings.Instance.SalesTaxPercent / 100, 2);

            total.SelectedTotal = Math.Round(producttotal,2) ;
            total.TaxTotal = Math.Round(taxtotal, 2);
            total.TipTotal = Math.Round(tip,2);

            return total;
        }



        public void VoidSelectedItem(string reason)
        {


            foreach (Seat emp in Seats)
                foreach (LineItem line in emp.LineItems)
                {
                    if (line.Selected)
                    {

                        VoidLineItem(line.ID,reason, false);
                    }else
                    {
                        //void modifiers


                    }

                }

      


            Reload();

        }

        public void InsertIDCheck(DriverLicense dl)
        {
            if(Licenses.Where(x=>x.LicenseNo == dl.LicenseNo).Count() == 0)
            {
                m_dbTicket.InsertIDCheck(SalesID, dl);
                LoadLicenses();
            }
         
        }

        public LineItem AddProductLineItem(Product prod,int seatnumber,decimal price, decimal quantity, decimal weight, string note, string custom1,string custom2,string custom3,string custom4)
        {
            int result;
            string description = prod.Description;


            try
            {
                if (m_status == "Closed") return null;
                //if gift card, then check to see if it is already in ticket
                if (prod.Type == "giftcard")
                {
                    if (GiftCardOnLineItem(custom1))
                    {
                        TouchMessageBox.Show("Gift Card already on ticket..");
                        return null;
                    }
                }

                if (prod.Description == "-1")
                {
                    TextPad np = new TextPad("Please Enter Custom Description:","");
                    np.Topmost = true;
                    np.ShowDialog();
                    description = np.ReturnText;
                  
                }

                result = m_dbTicket.DBQSAddProductLineItem(prod.ID, 0,0,m_salesid,seatnumber, description,prod.Description2, prod.Description3,prod.Unit, quantity,weight, price,  prod.Type, note,   prod.ReportCategory, prod.Taxable, prod.MenuPrefix, custom1, custom2, custom3, custom4,prod.SpecialPrice,prod.AllowPartial,prod.Weighted, 0);

                LoadSeats();

                return GetLineItemLine(result);

            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket - AddProductLineItem:" + e.Message);
                return null;
            }

        }

        public bool AddModifier(int salesitemid,int modid, int sortorder)
        {
            bool result;

            try
            {
                    result = m_dbTicket.DBAddModifier( salesitemid,modid, sortorder);

                    LoadSeats();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket - Add Modifier:" + e.Message);
                return false;
            }

        }


        public bool AddManualModifier(int salesitemid, string description, int quantity, decimal price)
        {
            bool result;

            try
            {
                result = m_dbTicket.DBAddManualModifier(salesitemid,description,quantity,price);

                LoadSeats();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket - Add Modifier:" + e.Message);
                return false;
            }

        }

        public bool AddModifierQuantity(int salesmodifierid)
        {
            bool result;

            try
            {
                result = m_dbTicket.DBAddModifierQuantity(salesmodifierid);

                LoadSeats();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket - Add Modifier Quantity:" + e.Message);
                return false;
            }

        }

        public bool DeleteModifierQuantity(int salesmodifierid)
        {
            bool result;

            try
            {
                result = m_dbTicket.DBDeleteModifierQuantity(salesmodifierid);

                LoadSeats();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket - Delete Modifier Quantity:" + e.Message);
                return false;
            }

        }

        public bool DeleteModifier(int salesmodifierid)
        {
            bool result;


            try
            {

                result = m_dbTicket.DBDeleteModifier(salesmodifierid);


                LoadSeats();



                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket - Add Modifier:" + e.Message);
                return false;
            }

        }

  

        public int AddComboLineItem(Product prod, int comboid,decimal combomaxprice, decimal price, decimal quantity, decimal weight, string note, int sortorder)
        {
            int result;



            try
            {
                if (m_status == "Closed") return 0;
            
                //do not put Sales ID into line item!!!   it is a subitem so should not be counted.
                result = m_dbTicket.DBQSAddProductLineItem(prod.ID, comboid,combomaxprice, 0, 0, prod.Description, prod.Description2, prod.Description3,prod.Unit, quantity,weight, price, prod.Type, note, prod.ReportCategory, prod.Taxable, prod.MenuPrefix, "", "", "", "",prod.SpecialPrice,prod.AllowPartial,prod.Weighted,sortorder);

                LoadSeats();

                return result;

            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket - Add Combo LineItem:" + e.Message);
                return 0;
            }

        }

 

        public bool DeleteAllLineItem()
        {
            bool result = false;


            try
            {
                if (m_status == "Closed") return false;

                foreach (Seat emp in Seats)
                    foreach (LineItem line in emp.LineItems)
                        result = m_dbTicket.DBQSDeleteLineItem(line.ID);


                LoadSeats();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Clear All LineItem:" + e.Message);
                return false;
            }

        }
        public bool DeleteLineItem(int id)
        {
            bool result;


            try
            {
                if (m_status == "Closed") return false;

                result = m_dbTicket.DBQSDeleteLineItem(id);

                LoadSeats();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Delete LineItem:" + e.Message);
                return false;
            }

        }

        public bool VoidLineItem(int id,string reason, bool refreshseat)
        {
            bool result;


            try
            {
                if (m_status == "Closed") return false;

                result = m_dbTicket.DBVoidLineItem(id, reason);

                if(refreshseat)
                    LoadSeats();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Void LineItem:" + e.Message);
                return false;
            }

        }

        public bool VoidAllLineItem(string reason )
        {
            bool result=false;


            try
            {
                if (m_status == "Closed") return false;

                foreach (Seat emp in Seats)
                    foreach (LineItem line in emp.LineItems)
                        result = m_dbTicket.DBVoidLineItem(line.ID,reason);


                LoadSeats();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Void All LineItem:" + e.Message);
                return false;
            }

        }

        public bool VoidTicket(string reason)
        {
            bool result;


            try
            {
                //only  open ticket can be voided ..
                if (m_status == "Closed") return false;

                //remove all item from ticket
                result = m_dbTicket.DBVoidTicket(m_salesid, reason);


                return result;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("VoidTicket:" + e.Message);
                return false;
            }

        }

        public bool DeleteTicket()
        {
            bool result;


            try
            {
                //only  open ticket can be deleted ..
                if (m_status == "Closed") return false;

                //remove all item from ticket
                result = m_dbTicket.DBDeleteTicket(m_salesid);


                return result;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Delete Ticket:" + e.Message);
                return false;
            }

        }


        public bool VoidAllCashPayment(string reason)
        {
            bool result=false;

            try
            {
                if (Status == "Closed") return false;

                foreach(Payment pay in Payments)
                    if(pay.CardGroup == "CASH")
                        result = m_dbTicket.DBVoidPayment(pay.ID, reason);

                LoadPayment();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("DeletePayment:" + e.Message);
                return false;
            }

        }

        public bool VoidPayment(int id, string reason)
        {
            bool result;

            try
            {
                if (Status == "Closed") return false;


                //remove payment from Payment table
                result = m_dbTicket.DBVoidPayment(id, reason);

                LoadPayment();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Void Payment:" + e.Message);
                return false;
            }

        }


        public bool VoidPayment(string transactionid)
        {
            bool result;

            try
            {
                if (Status == "Closed") return false;


                //remove payment from Payment table
                result = m_dbTicket.DBVoidCreditPayment(transactionid);

                LoadPayment();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Void Payment:" + e.Message);
                return false;
            }

        }


        public bool UpdateSalesItemQuantity(int salesitemid, decimal quantity)
        {
            bool result;
            try
            {
                if (Status == "Closed") return false;

                result = m_dbTicket.DBUpdateSalesItemQuantity(salesitemid, quantity);

                LoadSeats();
                return result;

            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket: Update SalesItem Quantity" + e.Message);
                return false;
            }

        }

        public bool UpdateSalesItemWeight(int salesitemid, decimal weight)
        {
            bool result;
            try
            {
                if (Status == "Closed") return false;

                result = m_dbTicket.DBUpdateSalesItemWeight(salesitemid,weight);

                LoadSeats();
                return result;

            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket: Update SalesItem Quantity" + e.Message);
                return false;
            }

        }

        public bool UpdateSalesItemSeat(int salesitemid, decimal seat)
        {
            bool result;
            try
            {
                if (Status == "Closed") return false;

                result = m_dbTicket.DBUpdateSalesItemSeat(salesitemid, seat);

                LoadSeats();
                return result;

            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket: Update SalesItem Seat" + e.Message);
                return false;
            }

        }

        /*
        public bool OverrideLineItemPrice(int id, decimal amount)
        {
            bool result;


            try
            {
                if (Status == "Closed") return false;


                result = m_dbTicket.DBUpdateSalesItemPrice(id, amount);
                LoadSeats();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Override Line Item Price:" + e.Message);
                return false;
            }

        }
        */

        public bool DiscountLineItem(int id, decimal amount,string discounttype, string reason)
        {
            bool result;


            try
            {
                if (Status == "Closed") return false;
                m_hasdiscount = false;

                result = m_dbTicket.DBUpdateSalesItemDiscount(id, amount,discounttype,reason);
                LoadSeats();

                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("Discount Line Item:" + e.Message);
                return false;
            }

        }



        public bool AddPayment(string paytype, decimal amount, string authorizeCode, string transtype)
        {

            try
            {
                decimal netamount;



                //if change is given to customers, then record the balance, not the tender 
                if (amount > Balance) netamount = Balance;
                else netamount = amount;

                //not credit card so don't need cardtype and masked pan parameter, 0 tip
                PaymentModel.InsertPayment(m_salesid, paytype, amount, netamount, authorizeCode, "", "", 0, DateTime.Now, transtype);
                GlobalSettings.CustomerDisplay.WriteDisplay(paytype + ":", amount, "Balance:", m_balance);

                NotifyPropertyChanged("Payments");
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("AddPayment:" + e.Message);
                return false;
            }
        }


        public bool UpdateGratuity(int paymentid, decimal amount)
        {

            try
            {
                if (!HasCreditPayment)
                {
                    MessageBox.Show("Credit Card payment not found!");
                    return false;

                }



                m_dbTicket.DBUpdateGratuity(paymentid, amount);   // gratuity removed from sales , need to be on payment

                Reload(); //reloades ticket

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("AddGratuity:" + e.Message);
                return false;
            }
        }

        public void UpdateSubOrderType(SubOrderType subtype)
        {
            TicketSubOrderType = subtype;
            m_dbTicket.DBUpdateSalesString(SalesID, "subordertype", subtype.ToString());
        }

        public void UpdateOrderType(OrderType ot)
        {
            TicketOrderType = ot;
            m_dbTicket.DBUpdateSalesString(SalesID, "ordertype", ot.ToString());
            //will also need to update price on the order if there is different price
            foreach(Seat seat in Seats)
                foreach(LineItem line in seat.LineItems)
                {
                    var prod = new Product(line.ProductID, ot);
                    line.UpdatePrice( prod.AdjustedPrice, prod.SpecialPrice);
                }


        }


        public bool UpdateNote(int salesitemid, string note)
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

        public bool SetHoldDate(double minutes)
        {
            if(HasHoldDate)
            {
                DateTime newdate = HoldDate.AddMinutes(minutes);
                HoldDate = newdate;
             
            }else
            {
                DateTime newdate = DateTime.Now.AddMinutes(minutes);
                HoldDate = newdate;
            }

            NotifyPropertyChanged("HasHoldDate");
            return m_dbTicket.DBUpdateHoldDate(SalesID, HoldDate);
           
        }

        


        public bool ClearHoldDate()
        {

            HoldDate = DateTime.MinValue;
            NotifyPropertyChanged("HasHoldDate");
            return m_dbTicket.DBClearHoldDate(SalesID);

        }

        public bool UpdateBarTabCustomer(string name)
        {
            CustomerName = name;
            return  m_dbTicket.DBUpdateSalesString(SalesID, "bartabcustomer", name);

        }


        public bool UpdateCustomerCount(int count)
        {
            CustomerCount = count;
            return m_dbTicket.DBUpdateSalesValue(SalesID, "customercount", count);
        }


        public bool AdjustTicketAmount(decimal amount)
        {

            try
            {

                m_dbTicket.DBUpdateAdjustmentAmount(m_salesid, amount);

                Discount = amount;

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket: Adjustment Amount:" + e.Message);
                return false;
            }
        }
        public bool AdjustTicket(decimal amount, string adjustmenttype, string adjustmentreason,int employeemealid=0)
        {

            try
            {

                m_dbTicket.DBUpdateAdjustment(m_salesid, amount, adjustmenttype, adjustmentreason,employeemealid);

                Discount = amount;

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket: Adjustment:" + e.Message);
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
                m_hasdiscount = false;

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

        
                    if (row["adjustment"].ToString() != "") Discount = decimal.Parse(row["adjustment"].ToString());

                    if (Discount < 0) m_hasdiscount = true;

   
                    if (row["salestax"].ToString() != "") SalesTax = decimal.Parse(row["salestax"].ToString());
                    if (row["autotip"].ToString() != "") AutoTipAmount = decimal.Parse(row["autotip"].ToString());

                    Note = row["Note"].ToString();

                    if (row["ordernumber"].ToString() != "")
                        OrderNumber = int.Parse(row["ordernumber"].ToString());
                    else OrderNumber = 0;

                    if (row["stationno"].ToString() != "")
                        StationNo = int.Parse(row["stationno"].ToString());
                    else StationNo = 0;

                    Status = row["Status"].ToString();
                    Custom1 = row["custom1"].ToString();
                    Custom2 = row["custom2"].ToString();
                    Custom3 = row["custom3"].ToString();
                    Custom4 = row["custom4"].ToString();
                    DiscountType = row["adjustmenttype"].ToString();
                    DiscountName = row["adjustmentname"].ToString();

                   // TicketOrderType = OrderType.ToGo;
                   TicketOrderType =(OrderType)Enum.Parse(typeof(OrderType),row["ordertype"].ToString());

                    if(row["subordertype"].ToString() != "") TicketSubOrderType = (SubOrderType)Enum.Parse(typeof(SubOrderType), row["subordertype"].ToString());


                    CustomerName = row["bartabcustomer"].ToString();

                    if(row["saledate"].ToString() !="") SaleDate = (DateTime)row["saledate"];

                   if (row["closedate"].ToString() != "") CloseDate = (DateTime)row["closedate"];
                    if (row["holddate"].ToString() != "") HoldDate = (DateTime)row["holddate"];


                    if (row["EmployeeID"].ToString() != "") CurrentEmployee = new Employee(int.Parse(row["EmployeeID"].ToString()));

                    if (row["customerid"].ToString() != "")
                    {
                        customerid = int.Parse(row["customerid"].ToString());
                        if (customerid > 0)
                        {
                            CurrentCustomer = new Customer(customerid,false,false);
                        }
                    }

                    if (row["tablenumber"].ToString() != "") TableNumber = int.Parse(row["tablenumber"].ToString()); else TableNumber = 0;
                    if (row["customercount"].ToString() != "") CustomerCount = int.Parse(row["customercount"].ToString()); else CustomerCount = 0;


                    LoadSeats();
              
                    LoadPayment();
                    LoadLicenses();

                }
                else
                { //ticket not found
                    SalesID = 0;
                    Status = null;

                }


            }
            catch (Exception e)
            {
                MessageBox.Show("LoadTicket:" + e.Message);
            }

        }






        public bool InsertPayment(string paytype, decimal amount, decimal netamount, string authorizecode, string cardtype, string maskedpan, decimal tip, DateTime paymentdate, string transtype)
        {
            var result = PaymentModel.InsertPayment(m_salesid, paytype, amount, netamount, authorizecode, cardtype, maskedpan, tip, paymentdate, transtype);
            LoadPayment();
            return result;
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

        public void UpdateTable(int tablenumber)
        {
            try
            {
                m_dbTicket.DBUpdateTable(m_salesid, tablenumber);
                

            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket:Update Table:" + e.Message);
            }

        }

 
        public void MarkAsSent()
        {
            if (Seats == null) return;


            foreach (Seat st in Seats)
                foreach (LineItem lineitem in st.LineItems)
                {
                    UpdateSalesItemValue(lineitem.ID, "sent", 1);
                    foreach(LineItem comboline in lineitem.LineItems)
                        UpdateSalesItemValue(comboline.ID, "sent", 1);
                }
                  
        }
        public void UpdateSalesItemValue(int id, string fieldstr, int value )
        {
            m_dbTicket.DBUpdateSalesItemValue(id,fieldstr,value);
          
        }

        public void UpdateCustomerID(int customerid)
        {


            try
            {
                if (customerid == 0) return;

                //saves to ticket record on database
                m_dbTicket.DBUpdateCustomerID(m_salesid, customerid);

                //Assigns customer object to Ticket object
                CurrentCustomer = new Customer(customerid,false,true);
                if (CurrentCustomer.RewardBalance > 0)
                {
                    string line1 = "Customer:" + Utility.FormatRight(CurrentCustomer.Phone1, 11);
                    string line2 = "Reward Bal:" + Utility.FormatRight(CurrentCustomer.RewardBalance.ToString(), 9);

                    GlobalSettings.CustomerDisplay.WriteRaw(line1, line2);

                    // GlobalSettings.CustomerDisplay.WriteDisplay("Customer:", CurrentCustomer.Phone1,"Reward Balance:", CurrentCustomer.RewardBalance);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Ticket: Update Customer (telephone):" + e.Message);
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


  

        public void LoadSeats()
        {
            ObservableCollection<Seat> seats;
            seats = new ObservableCollection<Seat>();

            //must initialize to 0 before totalizaing line items
            SubTotal = 0;
            TaxableTotal = 0;
            ActiveItemCount = 0;
            AllItemCount = 0;
            VoidedItemCount = 0;

            try
            {
                if (m_salesid == 0)
                {
                    MessageBox.Show("No SalesID ");
                    return;
                }

                //clear SentToKitchen flag until line items are loaded
                SentToKitchen = false;
              

                DataTable dt = m_dbTicket.GetSeats(m_salesid);
                AllFired = true;

                foreach (DataRow row in dt.Rows)
                {
                    Seat seat = new Seat();
                    int seatnumber = int.Parse(row["seatnumber"].ToString());
                    seat.SeatNumber = seatnumber;
                    seat.Ticket_Seat_ID = m_salesid.ToString() + "-" + seatnumber.ToString();
                    seat.LineItems = LoadLineItem(seatnumber);
                    seats.Add(seat);
                }

                Seats = seats;
                RecalculateTotal();

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load Seats:" + ex.Message);
            }
      


        }

        public ObservableCollection<LineItem> LoadLineItem(int seatnumber)
        {
            ObservableCollection<LineItem> lineitems;
            DataTable data_receipt;
            LineItem line;
            decimal producttotal = 0;

            decimal taxabletotal = 0;
            int itemcount = 0;
            int allitemcount = 0;
            int voideditemcount = 0;

           
          

            try
            {

                CurrentLine = null;
                
                lineitems = new ObservableCollection<LineItem>();

   

                //load purchased item that are products
                data_receipt = m_dbTicket.GetLineItemPerSeat(m_salesid, seatnumber);

           

                //load ticket item from sales item table
                foreach (DataRow row in data_receipt.Rows)
                {
                    
                    line = new LineItem(row);
                    if (line.NeedToSend) AllFired = false;
                    if (line.Discount > 0) m_hasdiscount = true;



                    lineitems.Add(line);
                    allitemcount++;

                    if(!line.Voided)
                    {
                        itemcount++;
                        if (line.ItemType.ToUpper() == "PRODUCT" || line.ItemType.ToUpper() == "COMBO")
                        {

                            //if combo, need to check if combo has any additional charges
                         
                            producttotal += line.TotalAdjustedPriceWithModifier ; //contains quantity multiplier already

                            if (line.Taxable) taxabletotal += line.TotalAdjustedPriceWithModifier ;

                        }
                        else
                        {
                            producttotal = producttotal + line.AdjustedPrice * line.Quantity; //giftcards
                        }

                       
                    }else
                    {
                        voideditemcount++;
                    }

                    //does not matter if item is voided or not .. if that line is sent , then ticket is considered sent
                    if (line.Sent) SentToKitchen = true;  //sets to true if atleast one item has been sent 

                }

          

                //Assign new value  to ticket object
                SubTotal += producttotal;
                TaxableTotal += taxabletotal;
                ActiveItemCount += itemcount;
                AllItemCount += allitemcount;
                VoidedItemCount += voideditemcount;

                return lineitems;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load LineItem:" + ex.Message);
                return null;
            }


        }



        public void SplitQuantities()
        {

            foreach(Seat seat in Seats)
                foreach(LineItem line in seat.LineItems)
                {
                    if(line.Quantity > 1 && !line.AllowPartial)
                    {
                        int count = (int)line.Quantity;
                        UpdateSalesItemQuantity(line.ID, 1);  //updates the database record
                        for(int i=1; i < count;i++)
                        {
                            //just need to duplicate the current line
                            DuplicateLine(line);
                        }
                    }
                }


            LoadSeats();
        }


        public void DuplicateLine(LineItem line)
        {
            int salesitem_id = m_dbTicket.DBQSAddProductLineItem(line.ProductID, 0, 0, m_salesid, line.SeatNumber, line.Description, line.Description2, line.Description3,line.Unit,1, line.Weight, line.Price,line.ItemType, line.Note, line.ReportCategory, line.Taxable, line.MenuPrefix, line.Custom1, line.Custom2, line.Custom3, line.Custom4, line.SpecialPricing, line.AllowPartial, line.Weighted, 0,line.Sent);
            foreach(SalesModifier mod in line.Modifiers)
            {
               bool result = m_dbTicket.DBAddModifier(salesitem_id,mod.ModifierID, mod.SortOrder,mod.Quantity);
            }
        }

        private void RecalculateTotal()   //--------------------------------------------------------------RECALCULATE TOTAL ------------------------------------------------------------------------------------
        {

      
            SubTotal = Math.Round(SubTotal, 2);

            if(SubTotal < Math.Abs(Discount) && SubTotal > 0)
            {
                AdjustTicketAmount((-1) * SubTotal);
           
            }


            TaxableTotal = Math.Round(TaxableTotal, 2);


            //open ticket ..  so override certain values , otherwise keep number from database since it's closed ticket
            if (Status != null)
                if (Status.ToUpper() != "CLOSED")
                {

                  
                    //assume close
                    if (TaxExempt) SalesTax = 0;
                    else SalesTax = Math.Round((TaxableTotal + Discount) * GlobalSettings.Instance.SalesTaxPercent * 0.01m, 2); 

                    if (CustomerCount >= GlobalSettings.Instance.AutoTipCustomerCount)
                        AutoTipAmount = Math.Round((SubTotal + Discount + SalesTax) * GlobalSettings.Instance.AutoTipPercent * 0.01m, 2);
                    else AutoTipAmount = 0;
                }



  

            Total = SubTotal + Discount + AutoTipAmount + SalesTax;//discount is neg
 



            Balance = Total  - TotalPayment;
            ChangeDue = CashPayment -CashNetPayment  ;
      

            AdjustedPayment = TotalPayment + CreditCardSurcharge;

            if (ChangeDue > 0)
            {
                IsChangeDue = Visibility.Visible;
            }
            else
            {
                IsChangeDue = Visibility.Collapsed;
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

        private LineItem m_currentline;
        public LineItem CurrentLine
        {
            get { return m_currentline; }
            set
            {
                m_currentline = value;
                NotifyPropertyChanged("CurrentLine");
            }
        }

        public LineItem GetLineItemLine(int id)
        {
            CurrentLine = null;

            foreach (Seat seat in Seats)
            {
                foreach (LineItem line in seat.LineItems)
                {
                    line.Selected = false;

                    if (line.ID == id)
                    {
                        CurrentLine = line;
                        CurrentLine.Selected = true;
                        return line;
                    }

                    //if not , then try to see if it's a combo and search it's sibling
                    foreach (LineItem comboline in line.LineItems)
                        if (comboline.ID == id)
                        {
                            CurrentLine = comboline;
                            CurrentLine.Selected = true;
                            return comboline;
                        }
                }
            }
        



            return null;
        }

   
    


        public void PayWithAuth()
        {
            m_dbTicket.DBPayWithAuth(PreAuth.ID, Balance);
        }
    


        public void LoadLicenses()
        {
            DataTable licenses = m_dbTicket.GetIDChecks(m_salesid);
            Licenses.Clear();



            foreach(DataRow row in licenses.Rows)
            {
                DriverLicense lic = new DriverLicense(row);
                Licenses.Add(lic);
            }

        }


        public void LoadPayment()
        {
            DataTable data_payments;
            decimal totalpayment;
            decimal totaltip;
            decimal totalcreditcardsurcharge = 0;
            decimal cashpayment = 0;
            decimal cashnetpayment = 0;
            HasDebitPayment = false;
            HasCreditPayment = false;
            HasCashPayment = false;
            HasVoidedDebitPayment = false;
            HasVoidedCreditPayment = false;
            HasAuthorCode = false;
      
            HasExternalGiftPayment = false;
            HasAUTH = false;

            
            try
            {

                totalpayment = 0;
                totaltip = 0;

                Payments.Clear();
                PreAuth = null;


                data_payments = m_dbTicket.GetPayments(m_salesid);


                foreach (DataRow row in data_payments.Rows)
                {
                    //added to collection
                    Payment pay = new Payment(row);
                    Payments.Add(pay);


                    //only count towards certain things if it's not VOIDED
                    if (!pay.Voided)
                    {


                        //need to load Amount , not NetAmount if you want to see actual amount given before change is given for other .. not Credit
                      //  if (pay.CardGroup.ToUpper() == "CREDIT" || pay.CardGroup.ToUpper() == "DEBIT")
                            totalpayment = totalpayment + pay.NetAmount;
                        // else
                        // totalpayment = totalpayment + pay.Amount;




                        if (pay.CardGroup == "CASH")
                        {
                            cashpayment += pay.Amount;
                            cashnetpayment += pay.NetAmount;
                            HasCashPayment = true;
                        }

                        if (pay.CardGroup.ToUpper() == "GIFT")
                        {
                            if (pay.CardType == "GIFT/REWARDS")
                                HasExternalGiftPayment = true;

                            if (pay.TipAmount > 0)
                            {
                              
                                totaltip = totaltip + pay.TipAmount;  //add tip to total tip amount
                            }
                        }


                        if (pay.CardGroup.ToUpper() == "DEBIT")
                        {
                          
                            HasDebitPayment = true;
                            totaltip = totaltip + pay.TipAmount;
                        }

                        if (pay.TransType == "AUTH") HasAUTH = true;

                        if (pay.CardGroup.ToUpper() == "CREDIT" )
                        {
                            if (pay.AuthorCode != "" && pay.AuthorCode != "standalone" && pay.AuthorCode != "external") HasAuthorCode = true;

                       
                            HasCreditPayment = true;

                            totaltip = totaltip + pay.TipAmount;
                            totalcreditcardsurcharge = totalcreditcardsurcharge + m_dbTicket.GetCreditCardSurcharge(pay.CardGroup, pay.NetAmount);

                        }



                    }else
                    {

                        if (pay.CardGroup.ToUpper() == "DEBIT")
                        {
                            HasVoidedDebitPayment = true;
                        }

                        if (pay.CardGroup.ToUpper() == "CREDIT")
                        {
                            HasVoidedCreditPayment = true;
                        }

                    }


                }




                TotalPayment = totalpayment;
                CashPayment = cashpayment;
                CashNetPayment = cashnetpayment;
         
                TipAmount = totaltip;

                CreditCardSurcharge = Math.Round(totalcreditcardsurcharge, 2);

                RecalculateTotal();

                var preauth = m_dbTicket.GetPreAuth(m_salesid);
                if(preauth != null)
                    PreAuth = new Payment(preauth);


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

 


       
      



        public void OpenDrawer()
        {
            string printername = GlobalSettings.Instance.ReceiptPrinter;
            string m_mode = GlobalSettings.Instance.ReceiptPrinterMode;

            if (printername == "none") return;
            ReceiptPrinter printer = new ReceiptPrinter(printername,m_mode);
            printer.CashDrawer(GlobalSettings.Instance.StationNo);
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

        public void SetTracker(string tracker)
        {
           
            
                m_dbTicket.DBUpdateSalesString(SalesID, "tracker", tracker);
                Reload();
            

        }

        #endregion public methods
    }
}
