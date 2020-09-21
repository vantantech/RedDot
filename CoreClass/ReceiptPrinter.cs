using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows;

namespace RedDot
{
    public class ReceiptPrinter
    {
        private string PrinterName;
        private string PrintString;
        //private decimal _printerwidth;

        //58mm printer = 32 chars  , 80mm printer = 48 chars

            //initialize
            public ReceiptPrinter(string prnName) 
            {
                PrinterName = prnName;
               // _printerwidth = printerwidth;
                RawPrinterHelper.SendStringToPrinter(PrinterName, (char)27 + "@");
                PrintString = "";
                

            }
            public void PrintImmediate(string prnString)
            {
                RawPrinterHelper.SendStringToPrinter(PrinterName, prnString);
            }

            public void PrintLF(string prnString)
            {
                PrintString = PrintString + prnString + "\n";
            }
        public void LineFeed()
            { PrintString = PrintString + "\n"; }


        public void Left()
        { PrintString = PrintString + (char)27 + "a0"; }

 
        public void Center()
        { PrintString = PrintString + (char)27 + "a1"; }

        public void Right()
        { PrintString = PrintString + (char)27 + "a2"; }

        public void DoubleHeight()
        {
            PrintString = PrintString + (char)27 + "!1"; 
        }

        public void DoubleHeightOFF()
        {
            PrintString = PrintString + (char)27 +  "@";
        }

        public void CashDrawer()
        { 
           
            RawPrinterHelper.SendStringToPrinter(PrinterName, (char)27 + "p050");
        
        }

        public static void CashDrawer(string printername)
        {
            RawPrinterHelper.SendStringToPrinter(printername, (char)27 + "@");
            RawPrinterHelper.SendStringToPrinter(printername, (char)27 + "p050");
        }



      //  public void PrintStoredLogo()
       // {
           // RawPrinterHelper.SendStringToPrinter(PrinterName,"" +  (char)29 + (char)76 + (char)40 + (char)6 + (char)0 + (char)48 + (char)69 + (char)32 + (char)32 + +(char)1 + (char)1 );


       // }

  

        public void PrintBarCode(string str)
        {
          


           RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "h" + (char)80); //height : n=height in dots , 80 dots
            RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "w" + (char)5);  //width :  GS w n   n= 1 to 6 ( 1= 0.141mm , 6 = 0.847 mm)
         // RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "f" + (char)0); // font: 0=Font A , 1=font B for HRI
            RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "H" + (char)2);  //print HRI ( Human Readable Interface)  2=below
            RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "k" + (char)69 + (char)str.Length + str + (char)0 + "\n");

          //  RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "d" + (char)3 + "\n");
          //  RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "@");







        }
        public void Cut()
        {
            RawPrinterHelper.SendStringToPrinter(PrinterName, "\n\n\n");
           RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "V1" );
        }

        public void Send()
        {
            RawPrinterHelper.SendStringToPrinter(PrinterName,PrintString);
            PrintString = "";
        }

        public void Clear()
        {
            PrintString = "";
        }



       
        
    }


}
