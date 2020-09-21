using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TriPOS.ResponseModels;

namespace RedDot
{
    /// <summary>
    /// Interaction logic for backofficemenu.xaml
    /// </summary>
    public partial class BackOfficeMenu : Window
    {
        private SecurityModel _security;
        private Window _parent;
        public BackOfficeMenu(Window parent,SecurityModel security)
        {
            InitializeComponent();
            this.Left = parent.Left;
            this.Top = parent.Top;
            _security = security;
            _parent = parent;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            if(_security.WindowAccess("Settings"))
            {
                Settings set = new Settings(_parent);
                Utility.OpenModal(this, set);
                GlobalSettings.CustomerDisplay.WriteRaw("Settings", "Saved");
                this.Close();
            }
           
        }

        private void btnReportSetup_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("Settings"))
            {
                ReportFields set = new ReportFields(_parent);
                Utility.OpenModal(this, set);
     
                this.Close();
            }

        }


  
        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("MenuSetup"))
            {

             
                    MenuSetup rpt = new MenuSetup(_security);
                    Utility.OpenModal(this, rpt);
          

                this.Close();


            }
        }

        private void Security_Clicked(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("Security"))
            {
                SecurityView dlg = new SecurityView();
                Utility.OpenModal(this, dlg);
                this.Close();

            }
        }
        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Database_Click(object sender, RoutedEventArgs e)
        {
            if (!_security.WindowAccess("DatabaseView"))
            {
                return;
            }
            
                Admin_Database adm = new Admin_Database();
            Utility.OpenModal(this,adm);
            this.Close();
        }


        private void Send_Clicked(object sender, RoutedEventArgs e)
        {
            if (!_security.WindowAccess("SMS"))
            {
                return;
            }

            SendSMS sms = new SendSMS();
            Utility.OpenModal(this, sms);
        }
        private void Employees_Clicked(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("EmployeeList"))
            {
                EmployeeList emp = new EmployeeList(this,_security);
                emp._employeelistviewmodel.CanAddEmployeeClicked = true;
                emp.AutoClose = false;
                Utility.OpenModal(this, emp);
                this.Close();
            }

        }

        private void TimeCard_Clicked(object sender, RoutedEventArgs e)
        {


            EmployeeTimeSheets rpt = new EmployeeTimeSheets(_security);
            Utility.OpenModal(this, rpt);
        }

        private void CreditCard_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_security.WindowNewAccess("CreditCardManager"))
                {
                    string ipaddress = GlobalSettings.Instance.SIPDefaultIPAddress;
                    string displaycomport = GlobalSettings.Instance.DisplayComPort;

                    PAXManager dlg2 = new PAXManager(ipaddress, displaycomport, ReceiptPrinterModel.PrintResponse)
                    {
                        Topmost = true,
                        ShowInTaskbar = false
                    };
                    dlg2.ShowDialog();

                }

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show(ex.Message);
            }
        }

        private void btnTable_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("TableLayout"))
            {
                TableLayout vw = new TableLayout(_security);
                Utility.OpenModal(this, vw);
                this.Close();
            }
        }


        private void btnPrinters_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("PrinterLocations"))
            {
                PrinterLocations vw = new PrinterLocations();
                Utility.OpenModal(this, vw);
                this.Close();
            }
        }

        private void Promotions_Click(object sender, RoutedEventArgs e)
        {
            if (!_security.WindowAccess("Promotions"))
            {
                //   Message("Access Denied.");
                return;
            }
            PromotionList adm = new PromotionList();

            Utility.OpenModal(this, adm);

        }

        private void DBBackup(object sender, RoutedEventArgs e)
        {

            try
            {
                string backupdirectory = GlobalSettings.Instance.BackupDirectory;

                BackupSelect bk = new BackupSelect(backupdirectory);

                bk.ChosenDrive = "";

                Utility.OpenModal(this, bk);

                if (bk.ChosenDrive != "")
                {
                    DBConnect db = new DBConnect();
                    if (bk.ChosenDrive != backupdirectory) backupdirectory = bk.ChosenDrive + "backup";
                    //"C:\\Program Files\\MySQL\\MySQL Server 5.6\\bin\\mysqldump"
                    db.Backup(backupdirectory);
                    TouchMessageBox.Show("Backup successful to  " + backupdirectory);

                }
            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("Error:" + ex.Message);
            }


        }


        //----------------------------------------------------------------   REPORTS -------------------------------------------------------------





        private void SalesReport_Click(object sender, RoutedEventArgs e)
        {

            if (_security.WindowAccess("SalesReport"))
            {

                SalesReportView rpt = new SalesReportView() { Topmost = true };
                rpt.ShowDialog();
                this.Close();
            }
        }

        private void EmployeesReport_Click(object sender, RoutedEventArgs e)
        {

            if (_security.WindowAccess("EmployeesReport"))
            {

                EmployeeReports rpt = new EmployeeReports(_security) { Topmost = true };
                rpt.ShowDialog();
                this.Close();
            }
        }


        private void CustomerReport_Click(object sender, RoutedEventArgs e)
        {

            if (_security.WindowAccess("CustomerReport"))
            {

                CustomerReport rpt = new CustomerReport(_security) { Topmost = true };
                rpt.ShowDialog();
                this.Close();
            }
        }


        private void CallerIDReport_Click(object sender, RoutedEventArgs e)
        {

            if (_security.WindowAccess("CallerIDReport"))
            {

                CallerID rpt = new CallerID() { Topmost = true };
                rpt.ShowDialog();
                this.Close();
            }
        }





        private void Certify(object sender, RoutedEventArgs e)
        {
           
         

            TextPad text = new TextPad("reference", "");
            Utility.OpenModal(this, text);

            TriPOSModel triposmodel = new TriPOSModel(1);
            TransactionQueryResponse rtn = triposmodel.ExecuteTransactionQuery(text.ReturnText);
            
            if(rtn.ExpResponse.ExpReportingData != null)
            {
                string str = "Amount:" +  rtn.ExpResponse.ExpReportingData.Items[0].ApprovedAmount.ToString() + (char)13 + (char)10;
                str = str + "Approval:" + rtn.ExpResponse.ExpReportingData.Items[0].ApprovalNumber + (char)13 + (char)10;
                str = str + "transactionid:" + rtn.ExpResponse.ExpReportingData.Items[0].TransactionId + (char)13 + (char)10;

                TouchMessageBox.Show(str);
            }
            TouchMessageBox.Show("Retrieve failed");

            
            /*

            NumPad num = new NumPad("", false, "");
            Utility.OpenModal(this, num);

           string amtstr = num.Amount;
           decimal amt = decimal.Parse(amtstr);

            SaleResponse resp = triposmodel.ExecuteCreditSale(0,amt);

            string message = "Response Code:" + (int)resp.ProcessorResponse.ProcessorResponseCode + "\r\n" +
               "Response Message:" + resp.ProcessorResponse.ProcessorResponseMessage + "\r\n" +
               "TransactionID:" + resp.TransactionId + "\r\n" +
               "Transaction DAte Time:" + resp.TransactionDateTime + "\r\n" +
               "Approved Amount:" + resp.TotalAmount + "\r\n";

            TouchMessageBox.Show(message);

            */

            // AuthorizationResponse resp = triposmodel.ExecuteCreditAuthorize(amt);
            //AuthorizationCompletionResponse resp = triposmodel.ExecuteCreditAuthorizeCompletion(text.ReturnText, amt);

            //IncrementalAuthorizationResponse resp = triposmodel.ExecuteCreditAuthorizeIncremental(text.ReturnText, amt);

            //RefundResponse resp = triposmodel.ExecuteRefund(amt);

            //ReturnResponse resp = triposmodel.ExecuteReturn(text.ReturnText, "CREDIT", amt);
            //ReversalResponse resp = triposmodel.ExecuteReversal(text.ReturnText, "CREDIT", amt);

            // VoidResponse resp2 = triposmodel.ExecuteVoid(resp.TransactionId);

        
      


           /* BatchCloseResponse resp3 = triposmodel.ExecuteBatch();


            message = "Response Code:" + (int)resp3.ExpResponse.ExpressResponseCode + "\r\n" +
                "Response Message:" + resp3.ExpResponse.ExpressResponseMessage + "\r\n" +
                "DAte:" + resp3.ExpResponse.ExpressTransactionDate + "\r\n" +
                 "Time:" + resp3.ExpResponse.ExpressTransactionTime + "\r\n";



            TouchMessageBox.Show(message);




            ReturnResponse resp4 = triposmodel.ExecuteReturn(resp.TransactionId, "CREDIT", 1m);

             message = "Response Code:" + (int)resp4.ProcessorResponse.ProcessorResponseCode + "\r\n" +
            "Response Message:" + resp4.ProcessorResponse.ProcessorResponseMessage + "\r\n" +
            "TransactionID:" + resp4.TransactionId + "\r\n" +
            "Transaction DAte Time:" + resp4.TransactionDateTime + "\r\n" +
            "Approved Amount:" + resp4.TotalAmount + "\r\n";

            TouchMessageBox.Show(message);




            BatchCloseResponse resp5 = triposmodel.ExecuteBatch();


            message = "Response Code:" + (int)resp5.ExpResponse.ExpressResponseCode + "\r\n" +
                "Response Message:" + resp5.ExpResponse.ExpressResponseMessage + "\r\n" +
                "DAte:" + resp5.ExpResponse.ExpressTransactionDate + "\r\n" +
                 "Time:" + resp5.ExpResponse.ExpressTransactionTime + "\r\n";



            TouchMessageBox.Show(message);
            */

        }
    }
}
