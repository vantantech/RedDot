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
using RedDot.DataManager;
using GlobalPayments.Api.Terminals;
using RedDot.Models;
using System.ComponentModel;
using RedDot.Models.CardConnect;
using RedDotBase;

namespace RedDot
{
    public class Ticket : INPCBase
    {
       
        private ObservableCollection<Payment>       m_payments;
        private ObservableCollection<Gratuity>      m_gratuities;
        private ObservableCollection<TicketEmployee> m_ticketemployees;
        private int                                 m_salesid;

        private string                              m_note;
        private string                              m_status;
        private DBTicket                            m_dbTicket;
        private DBProducts                          m_dbproducts;
        private DBEmployee                          m_dbemployee;
        private string                              m_custom1;
        private string                              m_custom2;
        private string                              m_custom3;
        private string                              m_custom4;
        private decimal                             m_producttotal;
        private decimal                             m_taxabletotal;
        //   private decimal                             m_subtotal_commission;
        private decimal                             m_labortotal;
        private decimal                             m_subtotal;
        private decimal                             m_total;
        private decimal                             m_totalpayment;
        private decimal                             m_balance;
        private decimal                             m_adjustment;
        private string m_adjustmenttype;
        private string m_adjustmentname;
        private decimal                             m_gratuitytotal;
        private decimal                             m_creditcardsurcharge;
        private decimal                             m_adjustedpayment;
        private int                                 m_itemcount;
        private decimal                             m_taxtotal;

        private AuditModel                          m_audit;
        private DateTime                            m_saledate;
    
    
        private Employee                            m_employee;
        private Customer                            m_customer;

        private bool                                m_Global_TurnUseSubTotal;
        private bool                                m_Global_TurnUsePoints;
        private bool                                m_hasdiscount;
     

        private VFD                                 m_vfd;




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

            m_dbTicket               = new DBTicket();
            m_dbproducts             = new DBProducts();
            m_dbemployee             = new DBEmployee();
            m_ticketemployees           = new ObservableCollection<TicketEmployee>();
            m_payments               = new ObservableCollection<Payment>();
            m_gratuities             = new ObservableCollection<Gratuity>();
            m_audit                  = new AuditModel();
            m_vfd                         = new VFD(GlobalSettings.Instance.DisplayComPort);
            m_status                 = null;
            m_salesid                = 0;
     
            m_labortotal             = 0;
            m_taxtotal               = 0;

            HasDebitPayment = false;
            HasCreditPayment = false;
            HasAuthorCode = false;
          

            //load all global settings
            m_Global_TurnUseSubTotal = GlobalSettings.Instance.TurnUseSubTotal;
            m_Global_TurnUsePoints = GlobalSettings.Instance.TurnUsePoints;
        

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

       


        public ObservableCollection<TicketEmployee> TicketEmployees
        {
            get { return m_ticketemployees; }
            set { m_ticketemployees = value; NotifyPropertyChanged("TicketEmployees"); }
        }

        public ObservableCollection<Payment> Payments
        {
            get { return m_payments; }
            set { m_payments = value; NotifyPropertyChanged("Payments"); }
        }


        public ObservableCollection<Gratuity> Gratuities
        {
            get { return m_gratuities; }
            set { m_gratuities = value; NotifyPropertyChanged("Gratuities"); }
        }

        public int SalesID
        {
            get { return m_salesid; }
            set { m_salesid = value; NotifyPropertyChanged("SalesID"); }
        }

        public string Note
        {
            get { return m_note; }
            set { m_note = value; NotifyPropertyChanged("Note"); }
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
            set
            {
                m_customer = value;

                if (value == null) m_dbTicket.DBUpdateCustomerID(m_salesid, 0);
                else m_dbTicket.DBUpdateCustomerID(m_salesid, value.ID);

                NotifyPropertyChanged("CurrentCustomer");
            }
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

        public bool HasVoidedDebitPayment { get; set; }

        public bool HasVoidedCreditPayment { get; set; }


        public bool HasDebitPayment { get; set; }

        public bool HasCreditPayment { get; set; }
        public bool HasCashPayment { get; set; }

        public bool HasAuthorCode { get; set; }

        public bool HasExternalGiftPayment { get; set; }
        public bool HasAUTH { get; set; }

        public bool HasTip { get; set; }

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
                if (Payments.Count > 0) return true; else return false;
            }
        }

        public bool HasDiscount
        {
            get
            {
                return m_hasdiscount;
            }
        }

        private bool m_hascomps;
        public bool HasComps
        {
            get
            {
                return m_hascomps;
            }
        }

        public Visibility m_ischangedue;

        public Visibility IsChangeDue
        {
            get { return m_ischangedue; }
            set { m_ischangedue = value; NotifyPropertyChanged("IsChangeDue"); }
        }


        public Visibility m_isbalance;

        public Visibility IsBalance
        {
            get { return m_isbalance; }
            set { m_isbalance = value; NotifyPropertyChanged("IsBalance"); }
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


        public decimal ProductTotal
        {
            get { return m_producttotal; }
            set { m_producttotal = value; NotifyPropertyChanged("ProductTotal"); }
        }


        public decimal TaxableTotal
        {
            get { return m_taxabletotal; }
            set { m_taxabletotal = value; NotifyPropertyChanged("TaxableTotal"); }
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


        public decimal TaxTotal
        {
            get { return m_taxtotal; }
            set { m_taxtotal = value; NotifyPropertyChanged("TaxTotal"); }
        }

        public decimal Total
        {
            get { return m_total; }
            set { m_total = value; NotifyPropertyChanged("Total"); }
        }



        public decimal GratuityTotal
        {
            get { return m_gratuitytotal; }
            set { m_gratuitytotal = value; NotifyPropertyChanged("GratuityTotal"); }
        }

        public decimal Adjustment
        {
            get { return m_adjustment; }
            set
            {
                m_adjustment = value;
                NotifyPropertyChanged("Adjustment");
                RecalculateTotal();
            }
        }


        public string AdjustmentType
        {
            get { return m_adjustmenttype; }
            set
            {
                m_adjustmenttype = value;
                NotifyPropertyChanged("AdjustmentType");
            }
        }

        public string AdjustmentName
        {
            get { return m_adjustmentname; }
            set
            {
                m_adjustmentname = value;
                NotifyPropertyChanged("AdjustmentName");
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



        private decimal m_cashpayment;
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

        private decimal m_totalplustip;
        public decimal TotalPlusTip
        {
            get { return m_totalplustip; }
            set
            {
                m_totalplustip = value;
                NotifyPropertyChanged("TotalPlusTip");

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

        decimal m_tipamount;
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


        public bool HasPaymentType(string paymenttype)
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



        public static int GetCustomerID(int salesid)
        {

            DBTicket dbticket = new DBTicket();
            return dbticket.GetCustomerID(salesid);
        }
  

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
        public bool CloseTicket(bool force = false)
        {
            bool rewardexceptionflag = false;
            decimal rewardpercent = GlobalSettings.Instance.RewardPercent;
            decimal cashrewardpercent = GlobalSettings.Instance.CashRewardPercent;

            string rewardexceptions = GlobalSettings.Instance.RewardException.ToUpper();

         
            bool AutoReceiptPrint = GlobalSettings.Instance.AutoReceiptPrint;
            bool OpenDrawOnClose = GlobalSettings.Instance.OpenDrawOnClose;

            try
            {
                if (Status == null) return false;
                if (Status == "Reversed" && force == false) return false;


                //test to see if total payments is equal or greater than subtotal or doing a force close
                if (m_totalpayment >= m_total || force)
                {
                    if(Status == "Reversed")
                    {
                        m_dbTicket.DBUpdateGiftCards(m_salesid);  //only update if it's a reversed ticket
                        m_dbTicket.DBUpdateGiftCertificates(m_salesid); 
                    }
                    else
                    {
                        m_dbTicket.DBActivateGiftCards(m_salesid);
                        m_dbTicket.DBActivateGiftCertificates(m_salesid);

             
                    }
                    if (rewardexceptions.Contains("DISCOUNT"))
                    {
                        if (HasDiscount) rewardexceptionflag = true;
                    }

                    if (rewardexceptions.Contains("COMPS"))
                    {
                        if (HasComps) rewardexceptionflag = true;
                    }

                    //Reward is calculated based on Service ( labor) Total * the reward percent
                    //Even if this is reversed , it's OK .. the insert code below will delete and duplicates
                    if (CurrentCustomer != null)
                    {
                        if (!rewardexceptionflag)
                        {

                            //first need to calculate the rewrad based on payment types, incentive to use cash to get more rewards

                            decimal rewardtotal = LaborTotal * (1 - CashPayment / Total) * rewardpercent / 100;
                            decimal cashrewardtotal = LaborTotal *(CashPayment /Total) * cashrewardpercent / 100;



                            m_dbTicket.DBInsertCustomerReward(CurrentCustomer.ID, SalesID, SaleDate, Total,cashrewardtotal + rewardtotal , "ADD", "");
                        }
                    }



                    Status = "Closed";

                 

                  

                    m_dbTicket.DBCloseTicket(m_salesid,m_subtotal, m_total,m_taxtotal);

           
               
           
                      


                 



                    DataTable turnlist;

                    //Turn method using points and partial points
                    if(m_Global_TurnUsePoints)
                    {
                        turnlist = m_dbTicket.GetTicketTurnListUsingTurnvalue(m_salesid,DateTime.Now);

                        foreach(DataRow row in turnlist.Rows)
                        {

                            if ((decimal)row["total"]  >= 1)
                                {

                                      m_dbemployee.MoveCheckInToBottom((int)row["employeeid"], DateTime.Now);

                                    //counts each turn if greater than minimum
                                    m_dbemployee.IncrementTurnCount((int)row["employeeid"], DateTime.Now);
                                    m_dbemployee.UpdatePartialTurn((int)row["employeeid"], DateTime.Now, (decimal)row["total"] + (decimal)row["partialturn"] - 1);
                                   
                                }else
                                {
                                    if ((decimal)row["total"] + (decimal)row["partialturn"] >= 1)
                                    {
                                        m_dbemployee.MoveCheckInToBottom((int)row["employeeid"], DateTime.Now);

                                        //counts each turn if greater than minimum
                                        m_dbemployee.IncrementTurnCount((int)row["employeeid"], DateTime.Now);
                                        m_dbemployee.UpdatePartialTurn((int)row["employeeid"], DateTime.Now, (decimal)row["total"] + (decimal)row["partialturn"] - 1);
                                    }
                                    else
                                    {
                                        m_dbemployee.UpdatePartialTurn((int)row["employeeid"], DateTime.Now, (decimal)row["total"] + (decimal)row["partialturn"]);

                                    }

                                    

                                }

                        }


                        //Check to see if top person has partial turn >= 1 , then move them down
                        turnlist = m_dbemployee.GetCheckInList(DateTime.Now);
                        if(turnlist.Rows.Count > 0)
                        {

                            if ((decimal)turnlist.Rows[0]["partialturn"]  >= 1)
                            {

                                m_dbemployee.MoveCheckInToBottom((int)turnlist.Rows[0]["employeeid"], DateTime.Now);

                                //move value from partial to turncount
                                m_dbemployee.IncrementTurnCount((int)turnlist.Rows[0]["employeeid"], DateTime.Now);
                                m_dbemployee.UpdatePartialTurn((int)turnlist.Rows[0]["employeeid"], DateTime.Now,  (decimal)turnlist.Rows[0]["partialturn"] - 1);

                            }


                        }

                    }else
                    {
                        //Turn method using price of items
                        if(m_Global_TurnUseSubTotal)
                        {
                        
                             turnlist = m_dbTicket.GetTicketTurnListUsingTotal(m_salesid, DateTime.Now);  //turn is based on total of multiple items added together
                        }else
                        {
                            turnlist = m_dbTicket.GetTicketTurnListUsingMax(m_salesid, DateTime.Now); //turns is based on a single item, must be greater than minimum
                        }
                        

                        foreach(DataRow row in turnlist.Rows)
                        {
  
                                if((decimal)row["total"] >= GlobalSettings.Instance.MinimumTurnAmount)
                                {

                                      m_dbemployee.MoveCheckInToBottom((int)row["employeeid"], DateTime.Now);

                                    //counts each turn if greater than minimum
                                    m_dbemployee.IncrementTurnCount((int)row["employeeid"], DateTime.Now);
                                   
                                }

                        }




                    }




                    foreach(TicketEmployee emp in TicketEmployees)
                        foreach (LineItem line in emp.LineItems)
                        {


                            if (line.ItemType == "product")
                            {
                                m_dbproducts.DBDeductInventory(line.ProductID);

                            }

                        }
                  
             


                    LoadGratuity();

                    if (CurrentCustomer != null) CurrentCustomer.ReloadHistory();
              

                    if (AutoReceiptPrint) ReceiptPrinterModel.PrintReceipt(this);

                    if (OpenDrawOnClose && HasCashPayment) OpenDrawer();


                    if (Balance < 0)
                        TouchMessageBox.Show(String.Format("Change Due: {0:c}", Balance * (-1)));

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


        public bool CreateTicket()
        {
            try
            {
                if (m_employee == null) return false;

                SalesID = m_dbTicket.DBCreateTicket(m_employee.ID);
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

        public void SplitTips()
        {

            //refresh data to make sure any recent payments and tips are loaded.
            LoadPayment();
            LoadGratuity();


            decimal tip = Payments.Sum(x => x.TipAmount);

            if (tip > 0)
            {
                if (Gratuities.Count == 1)
                {
                    //only one so assigned automatically
                    Gratuity grat = Gratuities.First();
                    if (grat.ID == 0)
                    {
                        //no gratuity assigned yet
                        AddGratuity(grat.CurrentEmployee.ID, tip);
                    }
                    else
                    {
                        UpdateGratuity(grat.ID, tip);
                    }

                }
                else
                {
                    //if automatic tip , then system takes care off 
                    switch (GlobalSettings.Instance.AutoTip.ToUpper())
                    {
                        case "EQUAL":
                            int count = 0;
                            decimal adjamt = 0;

                            decimal equalamt = 0;


                            equalamt = Math.Round(tip / Gratuities.Count, 2);

                            foreach (Gratuity grat in Gratuities)
                            {
                                if (count == 0)
                                {
                                    adjamt = tip - equalamt * (Gratuities.Count - 1);
                                    count++;
                                }
                                else adjamt = equalamt;

                                if (grat.ID == 0)
                                {
                                    //no gratuity assigned yet
                                    AddGratuity(grat.CurrentEmployee.ID, adjamt);
                                }
                                else
                                {
                                    UpdateGratuity(grat.ID, adjamt);
                                }

                            }

                            break;

                        case "PERCENT":


                            decimal percentamt = 0;
                            decimal percent = 0;

                            decimal sharetotal = 0;


                            foreach (Gratuity grat in Gratuities)
                            {
                                sharetotal = sharetotal + grat.TicketShareAmount;

                            }

                            int totalemployee = Gratuities.Count();
                            decimal totalused = 0;
                             count = 0;


                            foreach (Gratuity grat in Gratuities)
                            {
                                count++;
                                // cannot use ticket total because it is discounted.
                                percent = grat.TicketShareAmount / sharetotal;
                                percentamt = tip * percent;

                                //if last person then they get remaining to remove rounding error.
                                if (count == totalemployee)
                                    percentamt = tip - totalused;
                                else
                                    percentamt =Math.Round( tip * percent,2);

                                totalused = totalused + percentamt;



                                if (grat.ID == 0)
                                {
                                    //no gratuity assigned yet
                                   AddGratuity(grat.CurrentEmployee.ID, percentamt);

                                }
                                else
                                {
                                    UpdateGratuity(grat.ID, percentamt);
                                }

                            }

                            break;
                        case "MANUAL":

                            GratuityView gv = new GratuityView(this,null);
                            gv.Topmost = true;
                            gv.ShowDialog();

                            break;
                    }



                }



            }
            else
            {
                //set all techs to zero tip
                foreach (Gratuity grat in Gratuities)
                {

                    if (grat.ID == 0)
                    {
                        //no gratuity assigned yet
                        AddGratuity(grat.CurrentEmployee.ID, 0);
                    }
                    else
                    {
                        UpdateGratuity(grat.ID, 0);
                    }


                }
            }

        }


















        public bool IsGiftCardOnPayment(string giftcardnumber)
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
            foreach (TicketEmployee emp in TicketEmployees)
                foreach (LineItem line in emp.LineItems)
                {
                    if (line.Custom1 == giftcardnumber)
                    {
                        return true;
                    }
                }
            return false;
        }


        public bool AddProductLineItem(MenuItem prod,int quantity, int employeeid, string custom1,string custom2,string custom3,string custom4 )
        {
            bool result;
            

            try
            {
                if (m_status == "Closed") return false;




                result = m_dbTicket.DBAddProductLineItem(m_salesid, employeeid, prod.ID, prod.Description, prod.Price, prod.Discount, prod.Type, prod.CommissionType, prod.ReportCategory, prod.CommissionAmt, prod.TurnValue, prod.SupplyFee, quantity, custom1, custom2, custom3, custom4, prod.Taxable);


                LoadTicketItems();
          
                return result;

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Ticket - AddProductLineItem:" + e.Message);
                return false;
            }

        }

  

   


  

        public bool DeleteLineItem(int id)
        {
            bool result;


            try
            {
                if (m_status == "Closed") return false;

                result = m_dbTicket.DBDeleteLineItem(id);
                LoadTicketItems();

                return result;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("DeleteLineItem:" + e.Message);
                return false;
            }

        }



   
        public bool VoidTicket(string reason)
        {
            bool result;


            try
            {
                //only reversed or open ticket can be voided ..
                if (m_status == "Closed" ) return false;

                //need to check to see if  there was a gift card on the ticket that is going to be voided
                //1) choice one is to leave giftcard alone 
                //2) void gift card if not used yet
                //3) remove remaining balance


                //remove all item from ticket
                result = m_dbTicket.DBVoidTicket(m_salesid,reason);
  

                return result;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("VoidTicket:" + e.Message);
                return false;
            }

        }

   

        public bool VoidPayment(int id)
        {
            bool result;

            try
            {
                if (Status == "Closed") return false;


                //mark payment as void
                result = m_dbTicket.DBVoidPayment(id);

                LoadPayment();

                return result;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("DeletePayment:" + e.Message);
                return false;
            }

        }

        public bool VoidGiftCard(string accountnumber)
        {
            bool result;

            try
            {
                if (Status == "Closed") return false;


                //mark payment as void
                result = m_dbTicket.DBVoidGiftCard(SalesID,accountnumber);

                LoadPayment();

                return result;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("DeletePayment:" + e.Message);
                return false;
            }

        }

        public bool VoidGiftCardPayment(string accountnumber)
        {
            bool result;

            try
            {
                if (Status == "Closed") return false;


                //mark payment as void
                result = m_dbTicket.DBVoidGiftCardPayment(SalesID, accountnumber);

                LoadPayment();

                return result;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("DeletePayment:" + e.Message);
                return false;
            }

        }

        public bool VoidRewardADD()
        {
            bool result;

            try
            {
                if (Status == "Closed") return false;


                result = m_dbTicket.DBVoidRewardADD(SalesID);


                return result;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Void Reward:" + e.Message);
                return false;
            }

        }

        public bool VoidRewardREDEEM()
        {
            bool result;

            try
            {
                if (Status == "Closed") return false;


                result = m_dbTicket.DBVoidRewardREDEEM(SalesID);


                return result;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Void Reward:" + e.Message);
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
                LoadTicketItems();

                return result;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Override Line Item Price:" + e.Message);
                return false;
            }

        }


        public bool DiscountLineItem(int id, decimal amount, string discounttype, string reason)
        {
            bool result;


            try
            {
                if (Status == "Closed") return false;
                m_hasdiscount = false;

                result = m_dbTicket.DBUpdateSalesItemDiscount(id, amount, discounttype, reason);
                LoadTicketItems();

                return result;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Discount Line Item:" + e.Message);
                return false;
            }

        }

        public bool UpgradeLineItem(int id, decimal amount)
        {
            bool result;


            try
            {
                if (Status == "Closed") return false;
                m_hasdiscount = false;

                result = m_dbTicket.DBUpdateSalesItemUpgrade(id, amount);
                LoadTicketItems();

                return result;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Upgrade Line Item:" + e.Message);
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
                m_dbTicket.DBInsertPayment(m_salesid, paytype, amount, netamount, authorizeCode,"","",0,DateTime.Now,transtype);
                SalesModel.RedeemGiftCard(m_salesid, "Gift Certificate", amount, authorizeCode, DateTime.Now);

                m_vfd.WriteDisplay(paytype + ":", amount, "Balance:", m_balance);

              
                return true;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("AddPayment:" + e.Message);
                return false;
            }
        }


        public bool AddGratuity(int employeeid, decimal amount)
        {

            try
            {
                //removed because client wants to add tips on a cash sale ( use credit card just for tips)
                //if (CreditPaymentID == 0)
               // {
                 //   TouchMessageBox.Show("Credit Card payment not found!");
                 //   return false;

               // }

              
                   // m_dbTicket.DBDeleteGratuity(m_salesid, employeeid);
              

                m_dbTicket.DBInsertGratuity(m_salesid, employeeid, amount);
              
                

                LoadGratuity();

                return true;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("AddGratuity:" + e.Message);
                return false;
            }
        }



        public bool UpdateGratuity(int gratuityid, decimal amount)
        {

            try
            {
                if (gratuityid == 0)
                {
                    TouchMessageBox.Show("Gratuity ID not found!");
                    return false;

                }

                m_dbTicket.DBUpdateGratuity(gratuityid, amount);

                LoadGratuity();

                return true;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Ticket:UpdateGratuity:" + e.Message);
                return false;
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
                TouchMessageBox.Show("UpdateNote:" + e.Message);
                return false;
            }


        }
        public bool AdjustTicket(decimal amount, string adjustmenttype, string adjustmentreason)
        {

            try
            {

                m_dbTicket.DBUpdateAdjustment(m_salesid, amount, adjustmenttype, adjustmentreason);
                //reload ticket
                Reload();

                return true;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Ticket:Adjustment:" + e.Message);
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

      
                    if (row["adjustment"].ToString() != "") Adjustment= decimal.Parse(row["adjustment"].ToString());
                    AdjustmentType = row["adjustmenttype"].ToString();
                    AdjustmentName = row["adjustmentname"].ToString();

                    if (AdjustmentName == "") AdjustmentName = "DISCOUNT";


                    if (Adjustment < 0) m_hasdiscount = true;

                    Note = row["Note"].ToString();

                    Status = row["Status"].ToString();
                    Custom1 = row["custom1"].ToString();
                    Custom2 = row["custom2"].ToString();
                    Custom3 = row["custom3"].ToString();
                    Custom4 = row["custom4"].ToString();

                    SaleDate = (DateTime)row["saledate"];

              
                 

                    if (row["EmployeeID"].ToString() != "") CurrentEmployee = new Employee(int.Parse(row["EmployeeID"].ToString()));

                    if (row["customerid"].ToString() != "")
                    {
                        customerid = int.Parse(row["customerid"].ToString());
                        if (customerid > 0)
                        {
                            CurrentCustomer = new Customer(customerid,true);
                        }
                    }


                    LoadTicketItems(); //will also test for existence of discounts here
                    LoadPayment();
                    LoadGratuity();
                }
                else
                { //ticket not found
                    SalesID = 0;
                    Status = null;

                }


            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Ticket:LoadTicket:" + e.Message);
            }

        }


        public  bool InsertPayment( string paytype, decimal amount, decimal netamount, string authorizecode, string cardtype, string maskedpan, decimal tip, DateTime paymentdate, string transtype)
        {
            var result =  SalesModel.InsertPayment(m_salesid, paytype, amount, netamount, authorizecode, cardtype, maskedpan, tip, paymentdate, transtype);
            LoadPayment();
            return result;
        }

        public  bool InsertCreditPayment( decimal requested_amount, TerminalResponse resp, DateTime paymentdate, string cardgroup)
        {
            return SalesModel.InsertCreditPayment(SalesID, requested_amount, resp, paymentdate, cardgroup);
        }

        public bool InsertCreditPayment(string transtype,decimal requested_amount,decimal tip, CCSaleResponse resp, DateTime paymentdate)
        {
            return SalesModel.InsertCreditPayment(transtype, SalesID, requested_amount,tip, resp, paymentdate);
        }

        public bool InsertCreditPayment(string transtype, decimal requested_amount, TriPOS.ResponseModels.SaleResponse resp, DateTime paymentdate)
        {
            return SalesModel.InsertCreditPayment(transtype, SalesID, requested_amount, resp,paymentdate);
        }

        //Updates the Ticket Items with the selected server name , used in Nail Salon to give each service a tech name
        public void UpdateSalesItemEmployeeID(int salesitemid, int employeeid)
        {

            try
            {

                m_dbTicket.DBUpdateSalesItemEmployeeID(salesitemid, employeeid);

                LoadTicketItems();
                LoadGratuity();

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Ticket: Update SalesItem Employee ID:" + e.Message);
            }

        }

        public void DeleteReward()
        {
            m_dbTicket.DBVoidCustomerReward(SalesID);
        }
 
        public void DeleteGratuity( int employeeid=0)
        {
            //0 = all employee
            try
            {

                m_dbTicket.DBDeleteGratuity(SalesID, employeeid);
                Reload();

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Ticket: Remove Gratuity:" + e.Message);
            }

        }

        public void UpdateSalesDate(DateTime salesdate)
        {
            m_dbTicket.DBUpdateSalesDate(m_salesid, salesdate);
            SaleDate = salesdate;
        }

        public void UpdateCustomerID(int customerid)
        {


            try
            {

                //saves to ticket record on database
                m_dbTicket.DBUpdateCustomerID(m_salesid, customerid);

                //Assigns customer object to Ticket object
                CurrentCustomer = new Customer(customerid,true);

           
                if (CurrentCustomer.RewardBalance > 0)
                {
                    string line1 = "Customer:" + Utility.FormatRight(CurrentCustomer.Phone1, 11);
                    string line2 = "Reward Bal:" + Utility.FormatRight(CurrentCustomer.RewardBalance.ToString(), 9);

                    m_vfd.WriteRaw(line1, line2);

                    // vfd.WriteDisplay("Customer:", CurrentCustomer.Phone1,"Reward Balance:", CurrentCustomer.RewardBalance);
                }

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Ticket: Update Customer (telephone):" + e.Message);
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
                TouchMessageBox.Show("Ticket: Update Customer (telephone):" + e.Message);
            }

        }


        public void LoadTicketItems()
        {
            ObservableCollection<TicketEmployee> ticketemployees;

            ticketemployees = new ObservableCollection<TicketEmployee>();

            ProductTotal = 0;
            LaborTotal = 0;
            TaxableTotal = 0;
            ItemCount = 0;

           // DateTime start = DateTime.Now;
          

            DataTable dt = m_dbTicket.GetLineItemsEmployees(m_salesid); // get list of employeess on ticket
         
   

            foreach(DataRow row in dt.Rows)
            {
                TicketEmployee newitem = new TicketEmployee();
                int empid = 0;
                if(row["employeeid"].ToString() != "") empid = int.Parse(row["employeeid"].ToString());
                newitem.EmployeeId = empid;
                newitem.EmployeeName = row["displayname"].ToString();
               
                newitem.LineItems = LoadLineItems(empid); //Load Line Item in function below
       
                ticketemployees.Add(newitem);
            }
         
            //also need to load items that have no employees assigned
            TicketEmployee newitem1 = new TicketEmployee();
               
            newitem1.EmployeeId = 0;
            newitem1.EmployeeName = "--No Employee--";
        
            newitem1.LineItems = LoadLineItems(0); //Load Line Item in function below
            if(newitem1.LineItems.Count > 0)  ticketemployees.Add(newitem1);  //only add to list if count is greater than 0

          

            TicketEmployees = ticketemployees;

           // TimeSpan span = DateTime.Now - start;
          //  TouchMessageBox.Show("total: " + span.TotalMilliseconds.ToString());

            RecalculateTotal();
        }
        public ObservableCollection<LineItem> LoadLineItems(int techid)
        {
            ObservableCollection<LineItem> lineitems;
          
            DataTable data_receipt;
            LineItem line;
         
            decimal producttotal = 0;

            decimal taxabletotal = 0;
     
            decimal labortotal = 0;
 
            int itemcount = 0;
          
          


            try
            {
               


                lineitems = new ObservableCollection<LineItem>();

                if (SalesID == 0)
                {
                    TouchMessageBox.Show("No SalesID ");
                    return null;
                }
         


              

                //load purchased item that are services
                data_receipt = m_dbTicket.GetLineItems(m_salesid, techid);
              

               
                //load ticket item from product table
                foreach (DataRow row in data_receipt.Rows)
                {
                    itemcount++;
                    line = new LineItem(row);

                    if (line.Discount > 0) m_hasdiscount = true;

                    lineitems.Add(line);
                    if(line.ItemEnumType == ProductType.Service)
                    {
                        labortotal = labortotal + line.AdjustedPrice * line.Quantity;
                    }else
                    {
                        producttotal = producttotal + line.AdjustedPrice * line.Quantity;

                       
                    }


                    //doesn't matter if its a service or product or giftcard , only cares if it's marked as taxable
                    if (line.Taxable)
                        taxabletotal = taxabletotal + line.AdjustedPrice * line.Quantity;

                }

              
               
            

                //Assign new value  to ticket object
                ProductTotal = ProductTotal +  producttotal;
                TaxableTotal = TaxableTotal + taxabletotal;
                LaborTotal = LaborTotal +  labortotal;
             
                ItemCount = ItemCount +  itemcount;

               

                return lineitems;
            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("Load LineItem:" + ex.Message);
                return null;
            }


        }

        private void RecalculateTotal()   //--------------------------------------------------------------RECALCULATE TOTAL ------------------------------------------------------------------------------------
        {
      


            SubTotal = ProductTotal + LaborTotal ;
            if ((TaxableTotal + Adjustment) > 0)
            {
                Decimal taxtotal_nonRounded = (TaxableTotal + Adjustment) * GlobalSettings.Instance.SalesTaxPercent / 100;
                TaxTotal = Math.Round(taxtotal_nonRounded, 2, MidpointRounding.AwayFromZero);
            }
            else
                TaxTotal = 0;
         

         

            Total = SubTotal + Adjustment + TaxTotal; //Adjustment is discount so always negative
            Balance = Total - TotalPayment;
           AdjustedPayment = TotalPayment + CreditCardSurcharge;

            TotalPlusTip = TotalPayment + TipAmount;

            if (Balance < 0)
            {
                IsChangeDue = Visibility.Visible;
                IsBalance = Visibility.Collapsed;
            }
            else
            {
                IsChangeDue = Visibility.Collapsed;
                IsBalance = Visibility.Visible;
            }



            if (Status == "Voided")
            {
             
                Total = 0;
                Balance = 0;
                IsChangeDue = Visibility.Collapsed;
                IsBalance = Visibility.Collapsed;
                return;
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


    


        public LineItem GetLineItemLine(int id)
        {

            foreach (TicketEmployee emp in TicketEmployees)
                foreach (LineItem line in emp.LineItems)
                {
                    if (line.ID == id) return line;
                }
            return null;
        }

     

        public void LoadPayment()
        {
            DataTable data_payments;
            decimal totalpayment;
            decimal totaltip;
            decimal totalcreditcardsurcharge = 0;
            HasDebitPayment = false;
            HasCreditPayment = false;
            HasCashPayment = false;
            HasAuthorCode = false;
            HasTip = false;
            HasAUTH = false;
            HasExternalGiftPayment = false;
            HasVoidedDebitPayment = false;
            HasVoidedCreditPayment = false;


            try
            {

                totalpayment = 0;
                totaltip = 0;

                Payments.Clear();
                CashPayment = 0;
             

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
                        if (pay.CardGroup.ToUpper() == "CREDIT")
                            totalpayment = totalpayment + pay.NetAmount;
                        else
                            totalpayment = totalpayment + pay.Amount;  


                        if (pay.CardGroup.ToUpper() == "GIFT")
                        {
                            if (pay.CardType == "GIFT/REWARDS")
                                HasExternalGiftPayment = true;

                            HasTip = true;

                        }


                        if (pay.CardGroup.ToUpper() == "DEBIT")
                        {
                            HasTip = true;
                            HasDebitPayment = true;

                        }

                        if (pay.TransType == "AUTH") HasAUTH = true;

                        if (pay.CardGroup.ToUpper() == "CREDIT")
                        {
                            if (pay.AuthorCode != "" && pay.AuthorCode != "standalone" && pay.AuthorCode != "external") HasAuthorCode = true;
                            HasTip = true;

                            HasCreditPayment = true;
                            totalcreditcardsurcharge = totalcreditcardsurcharge + m_dbTicket.GetCreditCardSurcharge(pay.CardGroup, pay.NetAmount);

                        }

                        if (pay.CardGroup.ToUpper() == "CASH")
                        {
                            HasCashPayment = true;
                            CashPayment += pay.NetAmount;
                        }


                        if (pay.TipAmount > 0)
                        {
                            HasTip = true;
                            totaltip = totaltip + pay.TipAmount;  //add tip to total tip amount
                        }



                    }
                    else
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
                TipAmount = totaltip;  //total tip from all credit card on ticket
                CreditCardSurcharge = Math.Round(totalcreditcardsurcharge,2);

                RecalculateTotal();


                NotifyPropertyChanged("BalanceVisibility");
                NotifyPropertyChanged("ChangeDueVisibility");
                NotifyPropertyChanged("Payments");

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("LoadPayment:" + ex.Message);
            }


        }

 

        public void LoadGratuity()
        {
            ObservableCollection<Gratuity> gratuities;
            DataTable data_receipt;
            Gratuity grat; 
            decimal gratuitytotal = 0;
         

            try
            {

                gratuitytotal = 0;
         

                gratuities = new ObservableCollection<Gratuity>();

                data_receipt = m_dbTicket.GetGratuityString(m_salesid);

                foreach (DataRow row in data_receipt.Rows)
                {

                    grat = new Gratuity(row);
                    gratuities.Add(grat);
                    gratuitytotal = gratuitytotal + grat.Amount;


                }

                //assign new loaded objects to ticket
                GratuityTotal = gratuitytotal;
                Gratuities = gratuities;


            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("Ticket:Load Gratuity:" + ex.Message);
            }


        }


          
   

        public void OpenDrawer()
        {
            string printername = GlobalSettings.Instance.ReceiptPrinter;

            if (printername == "none") return;
            ReceiptPrinter printer = new ReceiptPrinter(printername);
            printer.CashDrawer();
            printer = null;
        }

 

        #endregion public methods
    }
}
