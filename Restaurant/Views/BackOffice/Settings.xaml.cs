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
using System.Printing;
using System.IO.Ports;
using Microsoft.Win32;
using System.Globalization;
using WpfLocalization;





namespace RedDot
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings :Window
    {
       // private bool _isloaded = false;
        SettingsVM settingsvm;
        bool enablekeyboard = GlobalSettings.Instance.EnableVirtualKeyboard;

        public Settings(Window parent)
        {
            InitializeComponent();
            settingsvm = new SettingsVM(parent);
            this.DataContext = settingsvm;
            Init();


        }

        private void Init()
        {



            FillPrinterList(comboReceiptPrinter, GlobalSettings.Instance.ReceiptPrinter);
        

            FillPrinterList(comboReportPrinter, GlobalSettings.Instance.ReportPrinter);
            FillPrinterList(comboPackagingPrinter, GlobalSettings.Instance.PackagingPrinter);

            FillCOMPortList(comboVFD);
  

     
        }



    

        private void FillCOMPortList(ComboBox combo)
        {
             
            int newitem;

            string vfd = GlobalSettings.Instance.DisplayComPort;
            newitem = combo.Items.Add("none");
            foreach (string port in SerialPort.GetPortNames())
            {

                newitem = combo.Items.Add(port.Trim());
                if (port == vfd) combo.SelectedIndex = newitem;

            }
            if (vfd == "none") combo.SelectedIndex = 0;
            //_isloaded = true;

        }
        private void FillPrinterList(ComboBox combo, string printername)
        {
            LocalPrintServer printServer = new LocalPrintServer();

            PrintQueueCollection printQueuesOnLocalServer = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
            int newitem;
            

            newitem = combo.Items.Add("none");
            foreach (PrintQueue printer in printQueuesOnLocalServer)
            {

               newitem =  combo.Items.Add(printer.Name.Trim());
               if (printer.Name == printername) combo.SelectedIndex = newitem;

            }
            if (printername == "none") combo.SelectedIndex = 0;
            //_isloaded = true;
            
        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            try
            {
          
     

                GlobalSettings.Instance.DisplayComPort = comboVFD.SelectedValue.ToString();
       
   

                if (comboReceiptPrinter.SelectedValue == null) GlobalSettings.Instance.ReceiptPrinter = "none";
                else  GlobalSettings.Instance.ReceiptPrinter = comboReceiptPrinter.SelectedValue.ToString();


                if (comboReportPrinter.SelectedValue == null) GlobalSettings.Instance.ReportPrinter = "none";
                else GlobalSettings.Instance.ReportPrinter = comboReportPrinter.SelectedValue.ToString();


                if (comboPackagingPrinter.SelectedValue == null) GlobalSettings.Instance.PackagingPrinter = "none";
                else GlobalSettings.Instance.PackagingPrinter = comboPackagingPrinter.SelectedValue.ToString();


                TapTipKeyboard.HideKeyboard();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Settings Save:" + ex.Message);
            }


         
        }


   

        private void test(object sender, RoutedEventArgs e)
        {
            ExpressionParser test = new ExpressionParser();
            decimal answer =test.Evaluate("x+y", 5, 4);
            MessageBox.Show(answer.ToString());

        }




        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (enablekeyboard)
                TapTipKeyboard.ShowKeyboard();
        }





    }
}
