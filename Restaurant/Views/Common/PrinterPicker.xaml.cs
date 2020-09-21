using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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

namespace RedDot
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class PrinterPicker : Window
    {
        public string ReturnText { get; set; }
        public string Action { get; set; }

        public static string OK = "Ok";
        public static string Cancel = "Cancel";

     
        public List<ListPair> PrinterList { get; set; }
        public string AssignedPrinter { get; set; }

        private string m_oldvalue;


        public PrinterPicker(string oldvalue)
        {
            m_oldvalue = oldvalue;

            InitializeComponent();
            this.DataContext = this;
            FillPrinterList();
        }

        private void FillPrinterList()
        {
            LocalPrintServer printServer = new LocalPrintServer();

            PrintQueueCollection printQueuesOnLocalServer = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });


            PrinterList = new List<ListPair>();


            foreach (PrintQueue printer in printQueuesOnLocalServer)
            {
                PrinterList.Add(new ListPair() { Description = printer.Name, StrValue = printer.Name });
            }


        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            Action = Cancel;
            ReturnText = m_oldvalue;
            this.Close();
        }

        private void Button_OK(object sender, RoutedEventArgs e)
        {
            Action = OK;
            ReturnText = AssignedPrinter;
            this.Close();
        }
    }
}
