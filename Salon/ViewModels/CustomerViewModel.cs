using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;
using System.Windows.Input;
using System.Windows;
using RedDot.DataManager;
using RedDot.Models;
using RedDotBase;

namespace RedDot
{
    public class CustomerViewModel:INPCBase
    {

        private Customer m_customer;
        private Window m_parent;
        Employee m_currentemployee;
        SecurityModel m_security;
        private bool CanExecute = true;
        public ICommand PrintRewardClicked { get; set; }
        public ICommand CreditDeleteClicked { get; set; }
        public ICommand AddCreditClicked { get; set; }
        public ICommand PrintRecordClicked { get; set; }
        public ICommand OpenOrderClicked { get; set; }

        public ICommand DeleteColorClicked { get; set; }

        public ICommand SendSMSRewardClicked { get; set; }

        public ICommand Star1Clicked { get; set; }
        public ICommand Star2Clicked { get; set; }
        public ICommand Star3Clicked { get; set; }
        public ICommand Star4Clicked { get; set; }

        public ICommand SpecialStatusClicked { get; set; }


        public ICommand MigrateClicked { get; set; }

        public CustomerViewModel(Window parent, SecurityModel security, int customerid)
        {
            m_customer = new Customer(customerid,true);

            ProcessStars();
          


            m_parent = parent;
            m_currentemployee = new Employee(0);
            m_security = security;
     
            CreditDeleteClicked = new RelayCommand(ExecuteCreditDeleteClicked, param => this.CanExecute);
            AddCreditClicked = new RelayCommand(ExecuteAddCreditClicked, param => this.CanExecute);
            PrintRewardClicked = new RelayCommand(ExecutePrintRewardClicked, param => this.CanExecute);
            PrintRecordClicked = new RelayCommand(ExecutePrintRecordClicked, param => this.CanExecute);
            OpenOrderClicked = new RelayCommand(ExecuteOpenOrderClicked, param => this.CanExecute);
            SendSMSRewardClicked = new RelayCommand(ExecuteSendSMSRewardClicked, param => this.CanExecuteSMS);

            Star1Clicked = new RelayCommand(ExecuteStar1Clicked, param => this.CanExecute);
            Star2Clicked = new RelayCommand(ExecuteStar2Clicked, param => this.CanExecute);
            Star3Clicked = new RelayCommand(ExecuteStar3Clicked, param => this.CanExecute);
            Star4Clicked = new RelayCommand(ExecuteStar4Clicked, param => this.CanExecute);
            MigrateClicked = new RelayCommand(ExecuteMigrateClicked, param => this.CanExecute);
            DeleteColorClicked = new RelayCommand(ExecuteDeleteColorClicked, param => this.CanExecute);

            SpecialStatusClicked = new RelayCommand(ExecuteSpecialStatusClicked, param => true);
        }

        private void ProcessStars()
        {

            if (m_customer.Rating >= 1) Star1 = "\\media\\star.png"; else Star1 = "\\media\\starblank.png";
            if (m_customer.Rating >= 2) Star2 = "\\media\\star.png"; else Star2 = "\\media\\starblank.png";
            if (m_customer.Rating >= 3) Star3 = "\\media\\star.png"; else Star3 = "\\media\\starblank.png";
            if (m_customer.Rating >= 4) Star4 = "\\media\\star.png"; else Star4 = "\\media\\starblank.png";
        }


        public bool CanExecuteSMS
        {
            get { return CurrentCustomer.AllowSMS; }
        }

        public Customer CurrentCustomer
        {
            get { return m_customer; }

            set { m_customer = value; NotifyPropertyChanged("CurrentCustomer"); }

        }


        private string _star1;
        public string Star1
        {
            get { return _star1; }
            set { _star1 = value; NotifyPropertyChanged("Star1"); }
        }

        private string _star2;
        public string Star2
        {
            get { return _star2; }
            set { _star2 = value; NotifyPropertyChanged("Star2"); }
        }

        private string _star3;
        public string Star3
        {
            get { return _star3; }
            set { _star3 = value; NotifyPropertyChanged("Star3"); }
        }

        private string _star4;
        public string Star4
        {
            get { return _star4; }
            set { _star4 = value; NotifyPropertyChanged("Star4"); }
        }


        public void ExecuteStar1Clicked(object obj)
        {

            CurrentCustomer.Rating = 1;
            ProcessStars();

        }

        public void ExecuteStar2Clicked(object obj)
        {

            CurrentCustomer.Rating = 2;
            ProcessStars();

        }

        public void ExecuteStar3Clicked(object obj)
        {

            CurrentCustomer.Rating = 3;
            ProcessStars();

        }

        public void ExecuteStar4Clicked(object obj)
        {

            CurrentCustomer.Rating = 4;
            ProcessStars();

        }



        public void ExecuteDeleteColorClicked(object obj)
        {
            string color = obj.ToString();
            if (color == "Add Note / Color")
            {
                TextPad text = new TextPad("Enter Note / Color Code:","");
                text.ShowDialog();
                if(text.ReturnText != "")
                    CurrentCustomer.AddColor(text.ReturnText);
            }
            else CurrentCustomer.DeleteColor(color);
         
        }
        public void ExecuteMigrateClicked(object obj)
        {

            CustomerModel cust = new CustomerModel();

            CustomerPhone pad = new CustomerPhone
            {
                Topmost = true,
                Amount = ""
            };
            pad.ShowDialog();

            if (pad.Amount != "")
            {
                DataTable dt = cust.LookupByPhone(pad.Amount);
                if (dt.Rows.Count == 1)
                {

                    int CustomerID = int.Parse(dt.Rows[0]["id"].ToString());
                    m_customer.MigrateTo(CustomerID);
                    m_customer.ReloadHistory();
                    NotifyPropertyChanged("CurrentCustomer");
                }else
                {
                    TouchMessageBox.Show("New Customer Not Found!! Enter all 10 digits");
                }
               

            }


           
        }

        public void ExecuteAddCreditClicked(object creditid)
        {
            NumPad dlg;

            try
            {
                if (m_security.WindowNewAccess("CustomerCredit"))
                {
                    CreditSelection crd = new CreditSelection();
                    Utility.OpenModal(m_parent, crd);


                    switch (crd.Action)
                    {
                        case "Referral":
                            m_customer.AddCredit(0, CurrentCustomer.TierReward, crd.Action); //automatically gives ONE referral credit equal to some preassigned dollar amount
                            break;
                        case "Reward Credit":
                            dlg = new NumPad("Enter Reward Credit Amount:",false);
                            Utility.OpenModal(m_parent, dlg);

                            if (dlg.Amount != "")
                            {
                                m_customer.AddCredit(0, decimal.Parse(dlg.Amount), crd.Action);
                            }
                            break;
                        case "Reward Points":
                            dlg = new NumPad("Enter Reward Points Amount:",true);
                            Utility.OpenModal(m_parent, dlg);

                            if (dlg.Amount != "")
                            {
                                m_customer.AddCredit(int.Parse(dlg.Amount), 0, crd.Action);
                            }
                            break;

                    }

                }

            }catch(Exception e)
            {
                TouchMessageBox.Show("Add credit:" + e.Message);
            }
        }


        public void ExecuteCreditDeleteClicked(object creditid)
        {

            Confirm dlg = new Confirm("Delete Stamp/Reward?");
            Utility.OpenModal(m_parent, dlg);
            if(dlg.Action == "Yes")
            {
                if (m_security.WindowNewAccess("CustomerCredit"))
                {
                     m_customer.DeleteCredit((int)creditid);
                }
            }
        }

        public void ExecuteOpenOrderClicked(object salesid)
        {

            int id;

            if (salesid == null) return;

            if (salesid.ToString() != "") id = int.Parse(salesid.ToString());
            else id = 0;

            SalesView ord = new SalesView( id);
            Utility.OpenModal(m_parent, ord);

        }

        public void ExecutePrintRewardClicked(object sender)
        {


                 string printername = GlobalSettings.Instance.ReceiptPrinter;
            Location store = GlobalSettings.Instance.Shop;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;

            int receiptchars = receiptwidth;
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;


            if (printername == "none") return;


            //58mm printer = 32 chars  , 80mm printer = 48 chars
            ReceiptPrinter printer = new ReceiptPrinter(printername);

            try
            {

                if (store == null)
                {

                    TouchMessageBox.Show("Shop/store info missing");
                    return;
                }



                printer.Center();
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.PrintLF("========================================");

                printer.Left();

                printer.PrintLF(Utility.FormatPrintRow(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), receiptchars));
                printer.PrintLF("-----------------------------------------");


                printer.PrintLF(Utility.FormatPrintRow("Customer ID#: ", CurrentCustomer.ID.ToString(), receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("Customer Phone #: ", CurrentCustomer.Phone1, receiptchars));

                printer.LineFeed();

                printer.LineFeed();
                printer.PrintLF(Utility.FormatPrintRow("REWARD TOTAL:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.RewardTotal, 2)), receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("REWARD USED:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.RewardUsed, 2)), receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("REWARD BALANCE:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.RewardBalance, 2)), receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("USABLE BALANCE:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.UsableBalance, 2)), receiptchars));


                printer.LineFeed();
                printer.PrintLF("     THANK YOU!");
                printer.LineFeed();
                printer.LineFeed();
                printer.Send(); //sends buffer to printer


                printer.Cut();

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("PrintReceipt:" + ex.Message);
            }

        }

        public void ExecuteSendSMSRewardClicked(object sender)
        {
        
            string message;


            message ="Hello from " +  GlobalSettings.Instance.Shop.Name + ". Your current Reward Balance is: " + CurrentCustomer.RewardBalance ;


            Confirm cnf = new Confirm("Send SMS to " + CurrentCustomer.Phone1 + " ?")
            {
                Topmost = true
            };

            cnf.ShowDialog();

            if(cnf.Action.ToUpper() == "YES")
            {

                string phone = CurrentCustomer.Phone1;


                if (phone.Length == 10) phone = "1" + phone;
                SMSModel.SendSMS(message,phone);
                TouchMessageBox.Show("Message Sent");
            }
           



        }
        public void ExecutePrintRecordClicked(object sender)
        {


            string printername = GlobalSettings.Instance.ReceiptPrinter;
            Location store = GlobalSettings.Instance.Shop;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
            int receiptchars = receiptwidth;
            //translate to chars if value is in millimeters
            //58mm printer = 32 chars  , 80mm printer = 48 chars
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;

            if (printername == "none") return;


            //58mm printer = 32 chars  , 80mm printer = 48 chars
            ReceiptPrinter printer = new ReceiptPrinter(printername);

            try
            {

                if (store == null)
                {

                    TouchMessageBox.Show("Shop/store info missing");
                    return;
                }



                printer.Center();
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.PrintLF("===================================================");

                printer.Left();

                printer.PrintLF(Utility.FormatPrintRow(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), receiptchars));
                printer.PrintLF("---------------------------------------------------");


                printer.PrintLF(Utility.FormatPrintRow("Customer ID#: ", CurrentCustomer.ID.ToString(), receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("Customer Phone #: ", CurrentCustomer.Phone1, receiptchars));
                printer.PrintLF("---------------------------------------------------");

                printer.LineFeed();

                foreach(DataRow row in CurrentCustomer.PurchaseHistory.Rows)
                {
   
                   printer.PrintLF(Utility.FormatPrintRow(row["id"].ToString() + " " + row["saledate"].ToString(), row["total"].ToString(), receiptchars));

                }
                printer.PrintLF("---------------------------------------------------");

                printer.LineFeed();
                printer.PrintLF(Utility.FormatPrintRow("TOTAL SPENT:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.TotalPurchased, 2)), receiptchars));               


                printer.LineFeed();
                printer.PrintLF(Utility.FormatPrintRow("REWARD TOTAL:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.RewardTotal, 2)), receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("REWARD USED:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.RewardUsed, 2)), receiptchars)); 
                printer.PrintLF(Utility.FormatPrintRow("REWARD BALANCE:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.RewardBalance, 2)), receiptchars));
                printer.PrintLF(Utility.FormatPrintRow("USABLE BALANCE:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.UsableBalance, 2)), receiptchars));


                printer.LineFeed();
                printer.PrintLF("     THANK YOU!");
                printer.LineFeed();
                printer.LineFeed();
                printer.Send(); //sends buffer to printer


                printer.Cut();

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("PrintReceipt:" + ex.Message);
            }

        }



        public void ExecuteSpecialStatusClicked(object obj)
        {
            string choices = GlobalSettings.Instance.SpecialStatusChoices;

            List<CustomList> list = new List<CustomList>();

            foreach(string item in choices.Split(','))
            {
                CustomList newitem = new CustomList { description = item, returnstring = item };
                list.Add(newitem);
            }

            ListPad lp = new ListPad("Select from List:", list);
            Utility.OpenModal(m_parent, lp);

            if (lp.ReturnString != "")
                CurrentCustomer.Custom2 = lp.ReturnString;

        }
    }
}
