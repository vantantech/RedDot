using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;
using System.Windows.Input;
using System.Windows;




namespace RedDot
{
    public class CustomerVM:INPCBase
    {

        private Customer _customer;
        private Window _parent;
        Security _security;
        CustomerModel m_customermodel;
        private bool CanExecute = true;
        public ICommand PrintRewardClicked { get; set; }
        public ICommand CreditDeleteClicked { get; set; }
        public ICommand AddCreditClicked { get; set; }
        public ICommand PrintRecordClicked { get; set; }
        public ICommand OpenOrderClicked { get; set; }
        public ICommand DeleteClicked { get; set; }

        public CustomerVM(Window parent, Security security, int customerid)
        {
            _customer = new Customer(customerid,true);
            _parent = parent;
             _security = security;
            m_customermodel = new CustomerModel(parent);

            CreditDeleteClicked = new RelayCommand(ExecuteCreditDeleteClicked, param => this.CanExecute);
            AddCreditClicked = new RelayCommand(ExecuteAddCreditClicked, param => this.CanExecute);
            PrintRewardClicked = new RelayCommand(ExecutePrintRewardClicked, param => this.CanExecute);
            PrintRecordClicked = new RelayCommand(ExecutePrintRecordClicked, param => this.CanExecute);
            OpenOrderClicked = new RelayCommand(ExecuteOpenOrderClicked, param => this.CanExecute);
            DeleteClicked = new RelayCommand(ExecuteDeleteClicked, param => this.CanDelete);



            string line1="Customer:" + Utility.FormatRight(CurrentCustomer.Phone1,11);
            string line2="Reward Bal:" + Utility.FormatRight( CurrentCustomer.RewardBalance.ToString(),9);

            VFD.WriteRaw(line1, line2);
        }


        public bool CanDelete
        {
            get
            {
                if (CurrentCustomer.NumberofVisit == 0) return true;
                else return false;

            }

    }

        public Customer CurrentCustomer
        {
            get { return _customer; }

            set { _customer = value; NotifyPropertyChanged("CurrentCustomer"); }

        }
        public void ExecuteAddCreditClicked(object creditid)
        {
            NumPad dlg;

            if(_security.WindowAccess("CustomerCredit"))
            {
                CreditSelection crd = new CreditSelection();
                Utility.OpenModal(_parent, crd);


                switch(crd.Action)
                {
                    case "Referral":
                        _customer.AddCredit(0, CurrentCustomer.TierReward, crd.Action); //automatically gives ONE referral credit equal to some preassigned dollar amount
                        break;
                    case "Reward Credit":
                        dlg = new NumPad("Enter Reward Credit Amount:",false);
                        Utility.OpenModal(_parent, dlg);

                        if (dlg.Amount != "")
                        {
                            _customer.AddCredit(0, decimal.Parse(dlg.Amount), crd.Action);
                        }
                        break;
                    case "Reward Points":
                         dlg = new NumPad("Enter Reward Points Amount:",true);
                        Utility.OpenModal(_parent, dlg);

                        if (dlg.Amount != "")
                        {
                            _customer.AddCredit(int.Parse(dlg.Amount),0 , crd.Action);
                        }
                        break;

                }


            }

 
 

        }

        public void ExecuteCreditDeleteClicked(object creditid)
        {

            Confirm dlg = new Confirm("Delete Stamp/Reward?");
            Utility.OpenModal(_parent, dlg);
            if(dlg.Action == "Yes")
            {
                if (_security.WindowAccess("CustomerCredit"))
                {

                     _customer.DeleteCredit((int)creditid);

                }
            }


        }
        public void ExecuteOpenOrderClicked(object salesid)
        {

            int id;

            if (salesid == null) return;

            if (salesid.ToString() != "") id = int.Parse(salesid.ToString());
            else id = 0;
                    RetailSalesView ord = new RetailSalesView(_security, id);
                    Utility.OpenModal(_parent, ord);

        }


        public void ExecuteDeleteClicked(object customerid)
        {

            int id;

            if (customerid == null) return;
            if (customerid.ToString() != "") id = int.Parse(customerid.ToString());
            else id = 0;

            m_customermodel.DeleteCustomer(id);

        }

        public void ExecutePrintRewardClicked(object sender)
        {


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
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.PrintLF("========================================");

                printer.Left();

                printer.PrintLF(Utility.FormatPrintRow(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), receiptwidth));
                printer.PrintLF("-----------------------------------------");


                printer.PrintLF(Utility.FormatPrintRow("Customer ID#: ", CurrentCustomer.ID.ToString(), receiptwidth));
                printer.PrintLF(Utility.FormatPrintRow("Customer Phone #: ", CurrentCustomer.Phone1, receiptwidth));

                printer.LineFeed();

                printer.LineFeed();
                printer.PrintLF(Utility.FormatPrintRow("REWARD TOTAL:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.RewardTotal, 2)), receiptwidth));
                printer.PrintLF(Utility.FormatPrintRow("REWARD USED:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.RewardUsed, 2)), receiptwidth));
                printer.PrintLF(Utility.FormatPrintRow("REWARD BALANCE:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.RewardBalance, 2)), receiptwidth));
                printer.PrintLF(Utility.FormatPrintRow("USABLE BALANCE:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.UsableBalance, 2)), receiptwidth));


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

        public void ExecutePrintRecordClicked(object sender)
        {


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
                printer.LineFeed();
                printer.PrintLF(store.Name);

                printer.PrintLF(store.Address1);
                if (store.Address2.Trim() != "") printer.PrintLF(store.Address2);
                printer.PrintLF(store.City + ", " + store.State + " " + store.Zip);
                printer.PrintLF(store.Phone);
                printer.LineFeed();

                printer.PrintLF("===================================================");

                printer.Left();

                printer.PrintLF(Utility.FormatPrintRow(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), receiptwidth));
                printer.PrintLF("---------------------------------------------------");


                printer.PrintLF(Utility.FormatPrintRow("Customer ID#: ", CurrentCustomer.ID.ToString(), receiptwidth));
                printer.PrintLF(Utility.FormatPrintRow("Customer Phone #: ", CurrentCustomer.Phone1, receiptwidth));
                printer.PrintLF("---------------------------------------------------");

                printer.LineFeed();

                foreach(DataRow row in CurrentCustomer.PurchaseHistory.Rows)
                {
   
                   printer.PrintLF(Utility.FormatPrintRow(row["id"].ToString() + " " + row["saledate"].ToString(), row["total"].ToString(), receiptwidth));

                }
                printer.PrintLF("---------------------------------------------------");

                printer.LineFeed();
                printer.PrintLF(Utility.FormatPrintRow("TOTAL SPENT:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.TotalPurchased, 2)), receiptwidth));               


                printer.LineFeed();
                printer.PrintLF(Utility.FormatPrintRow("REWARD TOTAL:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.RewardTotal, 2)), receiptwidth));
                printer.PrintLF(Utility.FormatPrintRow("REWARD USED:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.RewardUsed, 2)), receiptwidth)); 
                printer.PrintLF(Utility.FormatPrintRow("REWARD BALANCE:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.RewardBalance, 2)), receiptwidth));
                printer.PrintLF(Utility.FormatPrintRow("USABLE BALANCE:", String.Format("{0:0.00}", Math.Round(CurrentCustomer.UsableBalance, 2)), receiptwidth));


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

    }
}
