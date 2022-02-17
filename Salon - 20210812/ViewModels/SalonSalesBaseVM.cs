using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Data;
using com.clover.remotepay.sdk;
using GlobalPayments.Api.Terminals;
using Clover;
using RedDot.Models;

namespace RedDot
{
    public class SalonSalesBase : INPCBase
    {
        protected Ticket m_currentticket;
      
        public  VFD vfd{get;set;}

        public SecurityModel m_security { get; set; }

        public ICommand PrintCreditSlipClicked { get; set; }

        private bool integrated = (GlobalSettings.Instance.CreditCardProcessor != "External");
        //  private bool tipadjustallowed = (GlobalSettings.Instance.CreditCardProcessor == "External");
        private bool AllowTipOnCashSales = GlobalSettings.Instance.AllowTipOnCashSales;


        IDeviceInterface _device;  //PAX 300
        ICloverConnector cloverConnector;
        CloverListener ccl;

        public Visibility Clover { get; set; }
        public Visibility Other { get; set; }

        private Window m_parent;

        public SalonSalesBase(Window parent)
        {
            m_parent = parent;

            if(GlobalSettings.Instance.CreditCardProcessor.ToUpper() == "CLOVER")
            {
                Clover = Visibility.Visible;
                Other = Visibility.Collapsed;
            }else
            {
                Clover = Visibility.Collapsed;
                Other = Visibility.Visible;
            }

            cloverConnector = GlobalSettings.Instance.cloverConnector;





            vfd                 = new VFD(GlobalSettings.Instance.DisplayComPort);
            m_security            = new SecurityModel();

            PrintCreditSlipClicked = new RelayCommand(ExecutePrintCreditSlipClicked, param => HasCreditDebitPayment);
        }



        public bool CanExecuteNoPayment
        {
            get
            {
                if (CurrentTicket == null) return false;
                return !CurrentTicket.HasPayment;
            }
        }
        public bool CanExecuteReversedTicket
        {
            get
            {
                if (CurrentTicket == null) return false;
                if (CurrentTicket.Status == "Reversed")
                {
                    return true;
                }
                else return false;
            }
        }



        public bool CanExecuteOpenTicket
        {
            get
            {
                if (CurrentTicket == null) return false;
                if (CurrentTicket.Status == "Open")
                {
                    return true;
                }
                else return false;
            }
        }
        public bool CanExecutePayment
        {
            get
            {

                if (CurrentTicket == null) return false;

                if (CurrentTicket.Status != "Voided" && CurrentTicket.Status != "Closed")
                {
                    return true;
                }
                else return false;
            }

        }

        public bool CanExecutePaymentClicked
        {
            get
            {

                if (CurrentTicket == null) return false;
                if (CurrentTicket.Status != "Voided" && CurrentTicket.Status != "Closed" && CurrentTicket.ItemCount >= 1)
                {
                    return true;
                }
                else return false;
            }

        }
        public bool IsOpenOrReversed
        {
            get
            {
                if (CanExecuteOpenTicket || CanExecuteReversedTicket) return true; else return false;
            }
        }
        public bool CanExecuteNotClosed
        {
            get
            {
                if (CurrentTicket == null) return false;

                if (CurrentTicket.Status != "Closed")
                {
                    return true;
                }
                else return false;
            }

        }

   

        public bool CanExecuteNotOpen
        {
            get
            {

                if (CurrentTicket == null) return true;

                if (CurrentTicket.Status != "Open")
                {
                    return true;
                }
                else return false;
            }

        }
        public bool CanExecuteClosedTicket
        {
            get
            {
                if (CurrentTicket == null) return false;
                if (CurrentTicket.Status == "Closed")
                {
                    return true;
                }
                else return false;
            }
        }

        public bool CanExecuteNotNullTicket
        {
            get
            {
                if (CurrentTicket != null) return true;
                else return false;
            }
        }

        public bool CanExecuteGratuityClicked
        {
            get
            {
                    if (CanExecuteNotNullTicket && (CurrentTicket.Status == "Closed" || CurrentTicket.Status == "Reversed") && (CurrentTicket.HasTip || AllowTipOnCashSales) && CurrentTicket.Gratuities.Count > 0) return true;
                    else return false;
            }

        }

        public bool CanExecuteNotVoided
        {
            get
            {
                if (CurrentTicket == null) return false;

                if (CurrentTicket.Status != "Voided")
                {
                    return true;
                }
                else return false;
            }

        }

        public bool CanExecuteCustomer
        {
            get
            {
                if (CurrentTicket != null && CanExecuteNotVoided) return true;
                else return false;
            }
        }
        public bool CanExecutePrintReceipt
        {
            get
            {
                if (CurrentTicket == null)
                {
                    return false;
                }
                else
                {
                    if (CurrentTicket.ItemCount > 0) return true;
                    else return false;

                }

            }
        }

        public bool HasCreditDebitPayment
        {
            get
            {
                if (!integrated) return false;  //no credit slips if not integrated


                if (CanExecutePrintReceipt)
                {
                    return (CurrentTicket.HasVoidedDebitPayment || CurrentTicket.HasDebitPayment || (CurrentTicket.HasVoidedCreditPayment) || (CurrentTicket.HasCreditPayment && CurrentTicket.HasAuthorCode) || CurrentTicket.HasExternalGiftPayment);
                }
                else return false;
                    

           

            }
        }

        public Ticket CurrentTicket
        {
            get { return m_currentticket; }

            set
            {
                m_currentticket = value;
                NotifyPropertyChanged("CurrentTicket");
            }
        }


        public int WebUserId { get; set; }












        //-------------------------------------------------------------------- Button Commands ------------------------------------------------



        public void ExecutePrintReceiptClicked(object button)
        {
            ReceiptPrinterModel.PrintReceipt(CurrentTicket);
        }


        public void ExecutePrintCreditSlipClicked(object param)
        {

            string location="";
            
            if(param != null) location= param.ToString();

            var result = CurrentTicket.Payments.Where(x => x.CardGroup == "DEBIT" || x.CardGroup == "CREDIT" || (x.CardGroup == "GIFT" && x.CardType == "GIFT/REWARDS"));
            if (result != null)
            {
                if (result.Count() == 0) return;

                if (result.Count() > 1) 
                {
                    List<CustomList> list = new List<CustomList>();



                    foreach (var item in result)
                    {
                        list.Add(new CustomList { returnstring = item.ID.ToString(), description = item.CardType + " " + item.AmountStr });
                    }
                    ListPad lp = new ListPad("Pick slip to print:", list) { Topmost = true };
                    lp.ShowDialog();
                    if (lp.ReturnString != "")
                    {
                        result = CurrentTicket.Payments.Where(x => x.ID.ToString() == lp.ReturnString);
  
                    }
                }

                DisplayPaymentReceiptOptionsRequest request = new DisplayPaymentReceiptOptionsRequest();
                request.OrderID = result.First().CloverOrderId;
                request.PaymentID = result.First().CloverPaymentId;


                switch (GlobalSettings.Instance.CreditCardProcessor)
                {

                    case "CardConnect":

                        CardConnectModel.PrintReceipt(CurrentTicket.SalesID);

                        break;

                    case "Clover":

                        if (location == "Clover") cloverConnector.DisplayPaymentReceiptOptions(request);
                        else
                            ReceiptPrinterModel.PrintCreditSlip(CurrentTicket, result.First(), "**---COPY---**");
                        break;

                    default:

                        ReceiptPrinterModel.PrintCreditSlip(CurrentTicket, result.First(), "**---COPY---**");
                        break;

                }



              

              


            }

  


     
        }




        public void CustomerViewEditDelete()
        {
            if (m_security.WindowNewAccess("CustomerView") == false)
            {

                return; //jump out of routine
            }

            if (CurrentTicket.CurrentCustomer.ID > 0)
            {
                //Ask if view or delete
                CustomerActionMenu cm = new CustomerActionMenu(m_security, CurrentTicket.CurrentCustomer.ID) { Topmost = true };
                cm.ShowDialog();

                if (cm.Action == "View")
                {
                    CustomerView custvw = new CustomerView(m_security, CurrentTicket.CurrentCustomer.ID) { Topmost = true };
                    custvw.ShowDialog();
                    CurrentTicket.CurrentCustomer = new Customer(CurrentTicket.CurrentCustomer.ID, true);  //loades new
                }

                if (cm.Action == "Delete")
                {
                    if (CurrentTicket.Status.ToUpper() == "CLOSED")
                    {
                        TouchMessageBox.Show("Ticket is Closed, Need to reverse first");
                    }
                    else
                    {
                        if (CurrentTicket.Status.ToUpper() == "REVERSED") //also need to remove rewards
                        {
                            CurrentTicket.DeleteReward();
                        }

                        CurrentTicket.UpdateCustomerID(0);
                        CurrentTicket.CurrentCustomer = null;



                    }


                }

                return; //exits code here

            }
        }


        public void ExecuteCustomerClicked(object button)
        {

            //if ticket already has customer number, then bring up edit screen
            if (CurrentTicket.CurrentCustomer != null)
            {
                CustomerViewEditDelete();
                return;

            }


            //if ticket is Closed , you can not add customer
            if (CurrentTicket.Status.ToUpper() == "CLOSED")
            {
                TouchMessageBox.Show("Ticket is Closed, Need to reverse first");
                return;
            }


            //if no customers then code continues below

            CustomerModel cust = new CustomerModel();
            int customerid = 0;
     



            //ask for customer phone number
            CustomerPhone pad = new CustomerPhone
            {
                Topmost = true,
                Amount = ""
            };
            pad.ShowDialog();

            //if user cancel , returns ""
            if (pad.Amount != "")
            {
          
                DataTable dt = cust.LookupByPhone(pad.Amount);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows.Count == 1)
                    {
                        customerid = int.Parse(dt.Rows[0]["id"].ToString());

                    }
                    else
                    {
                        //Display list of names to pick from
                        CustomerFoundList cfl = new CustomerFoundList(dt) { Topmost = true };
                        cfl.ShowDialog();
                        customerid = cfl.CustomerID;
                    }
                }
                else
                {
                    //customer phone number was not found

                    if (pad.Amount.Length == 10)
                    {
                        //Create new customer
                        customerid = cust.CreateNew(pad.Amount);
                        TouchMessageBox.Show("New Customer Created");

                        if (GlobalSettings.Instance.EditCustomerProfileOnCreate)
                        {
                            CustomerView custvw = new CustomerView(m_security, customerid);
                            custvw.Topmost = true;
                            custvw.ShowDialog();
                        }



                        //add customer id to ticket .. it will update database
                        CurrentTicket.UpdateCustomerID(customerid);
                    }
                    else
                    {

                        TouchMessageBox.Show("Please Enter 10 digit number to create customer account");

                        pad = new CustomerPhone
                        {
                            Topmost = true,
                            Amount = "",
                            FullNumberRequired = true
                        };
                        vfd.WriteRaw("Please Enter Phone", "");
                        pad.ShowDialog();


                        if (pad.Amount != "")
                        {

                            DataTable dt2 = cust.LookupByPhone(pad.Amount);
                            if (dt2.Rows.Count == 0)
                            {
                                //Create new customer

                                customerid = cust.CreateNew(pad.Amount);
                                TouchMessageBox.Show("New Customer Created");

                                if (GlobalSettings.Instance.EditCustomerProfileOnCreate)
                                {
                                    CustomerView custvw = new CustomerView(m_security, customerid);
                                    custvw.Topmost = true;
                                    custvw.ShowDialog();
                                }


                                //add customer id to ticket .. it will update database
                                CurrentTicket.UpdateCustomerID(customerid);

                            }
                            else
                            {
                                TouchMessageBox.Show("Can not create customer, phone number already exist.");
                            }
                        }


                    }


                }


            }
            else return;




            if (customerid > 0)
            {
                //check for customer reward
                CurrentTicket.UpdateCustomerID(customerid);


                if (CurrentTicket.CurrentCustomer.Custom2 != "" && CurrentTicket.CurrentCustomer.Custom2.ToUpper() != "NONE")
                {
                    TouchMessageBox.Show("Special Status:  ***   " + CurrentTicket.CurrentCustomer.Custom2 + "   ***");

                }


                if (GlobalSettings.Instance.DisplayRewardAlert)
                {
                    //check to see if customer has usable rewards
                    if (CurrentTicket.CurrentCustomer.UsableBalance > 0)
                    {
                        string message;
                        message = "Customer has Reward: " + CurrentTicket.CurrentCustomer.UsableBalance.ToString("c");
                        TouchMessageBox.Show(message);
                    }

                }
            }
         




            


        }


     
    }
}
