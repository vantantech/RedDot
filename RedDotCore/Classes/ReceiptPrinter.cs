
namespace RedDot
{
    public class ReceiptPrinter
    {
        private string PrinterName;
        private string PrintString;
        private string m_mode;
        //private decimal _printerwidth;

        //58mm printer = 32 chars  , 80mm printer = 48 chars

        //initialize
        public ReceiptPrinter(string prnName, string mode)
        {
            PrinterName = prnName;
            // _printerwidth = printerwidth;
            RawPrinterHelper.SendStringToPrinter(PrinterName, (char)27 + "@");
            PrintString = "";
            m_mode = mode;


        }
        public void PrintImmediate(string prnString)
        {
            RawPrinterHelper.SendStringToPrinter(PrinterName, prnString);
        }

    

        public void ColorBlack()
        {
            if(m_mode == "STAR")
                PrintString = PrintString + (char)27 + "5";
           // else
               // PrintString = PrintString + (char)27 + "r0";//epson
        }

        public void ColorRed()
        {

            if (m_mode == "STAR")
                PrintString = PrintString + (char)27 + "4";
          //  else
              //  PrintString = PrintString + (char)27 + "r1"; //epson
        }


        public void PrintLF(string prnString)
            {
                PrintString = PrintString + prnString + "\n";
            }

        public void LineFeed()
            { PrintString = PrintString + "\n"; }


        public void Left()
        {
            if(m_mode == "STAR")
                PrintString = PrintString + (char)27 + (char)29 + "a0";
            else
                PrintString = PrintString + (char)27 + "a0";
        }

 
        public void Center()
        {
            if (m_mode == "STAR")
                PrintString = PrintString + (char)27 + (char)29 + "a1";
            else
                PrintString = PrintString + (char)27 + "a1";
        }

        public void Right()
        {
            if (m_mode == "STAR")
                PrintString = PrintString + (char)27 + (char)29 + "a2";
            else
                PrintString = PrintString + (char)27 + "a2";
        }
        public void Emphasize()
        {
            if (m_mode == "STAR")
                PrintString = PrintString + (char)27 + "E";
            else
                PrintString = PrintString + (char)27 + "E";
        }

        public void EmphasizeOFF()
        {
            if (m_mode == "STAR")
                PrintString = PrintString + (char)27 + "F";
            else
                PrintString = PrintString + (char)27 + "F";
        }



        public void DoubleHeight()
        {
            if(m_mode == "STAR")
                PrintString = PrintString + (char)27 + "h1";
            else
                PrintString = PrintString + (char)27 + "!1"; 
        }

        public void DoubleHeightOFF()
        {
            if(m_mode == "STAR")
                PrintString = PrintString + (char)27 + "h0";
            else
                PrintString = PrintString + (char)27 +  "@";
        }


        public void DoubleWidth()
        {
            if (m_mode == "STAR")
                PrintString = PrintString + (char)27 + "W1";
            else
                PrintString = PrintString + (char)27 + "!1";
        }

        public void DoubleWidthOFF()
        {
            if (m_mode == "STAR")
                PrintString = PrintString + (char)27 + "W0";
            else
                PrintString = PrintString + (char)27 + "@";
        }


        public void Viet()
        {
            PrintString += (char)27 + "t94";
        }
        public void CashDrawer(int drawer)
        { 
           
           if(m_mode == "STAR")
            {
                if (drawer == 1)
                    RawPrinterHelper.SendStringToPrinter(PrinterName, (char)27 + "p050");
                else
                    RawPrinterHelper.SendStringToPrinter(PrinterName, (char)27 + "p050");
            }
            else
            {
                if (drawer == 1)
                    RawPrinterHelper.SendStringToPrinter(PrinterName, (char)27 + "p050");
                else
                    RawPrinterHelper.SendStringToPrinter(PrinterName, (char)27 + "p051");
            }
           
        
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
          //RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "w" + (char)2);  //width :  GS w n   n= 1 to 6 ( 1= 0.141mm , 6 = 0.847 mm)
         // RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "f" + (char)0); // font: 0=Font A , 1=font B for HRI
            RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "H" + (char)2);  //print HRI ( Human Readable Interface)  2=below


            RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "k" + (char)72 + (char)str.Length + str ); //69=code39   72=code93 65=UPC-A




          // RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "k" + (char)4  + str + (char)0 + "\n"); //69=code39   72=code93 65=UPC-A



          //  RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "d" + (char)3 + "\n");
          //  RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "@");


        }


        public void Cut()
        {
            RawPrinterHelper.SendStringToPrinter(PrinterName, "\n\n\n");

            if (m_mode == "STAR")
                RawPrinterHelper.SendStringToPrinter(PrinterName, (char)27 + "d3");
            else
                RawPrinterHelper.SendStringToPrinter(PrinterName, (char)29 + "V1" );//epson
        }

        public void Send()
        {
          

          //  if (GlobalSettings.Instance.Demo) PrintString = (char)27 + "a1" +  "*** DEMO ONLY***" + "\n" + "*** Not Licensed ***" + "\n" + "*** DEMO ONLY***" + "\n" + PrintString;


            RawPrinterHelper.SendStringToPrinter(PrinterName,PrintString);
            PrintString = "";
        }

        public void Clear()
        {
            PrintString = "";
        }

        public void PrinterTest(string locationname="")
        {

            Center();
            LineFeed();
            PrintLF("Printer Test ");
            PrintLF("Red Dot POS ");
            PrintLF(locationname);
            PrintLF("==========================");
            PrintLF("Printer Test Complete");
            Send();
            Cut();
        }





    }


}
