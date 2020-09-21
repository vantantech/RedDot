using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedDot.Models
{
    public class PrintModel
    {


        public static void PrintEmployeeHours(Employee employee)
        {

            string printername = GlobalSettings.Instance.ReceiptPrinter;
            int receiptwidth = GlobalSettings.Instance.ReceiptWidth;
            Location store = GlobalSettings.Instance.Shop;

 

            if (printername == "none") return;

            //defaults to receipt width since some customer had the character width setup.
            int receiptchars = receiptwidth;
            //translate to chars if value is in millimeters
            //58mm printer = 32 chars  , 80mm printer = 48 chars
            if (receiptwidth == 58) receiptchars = 32;
            if (receiptwidth == 80) receiptchars = 48;


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

                printer.DoubleHeight();
                printer.PrintLF("Employee Time");

                printer.DoubleHeightOFF();
                printer.LineFeed();
                printer.Left();

                printer.PrintLF(new String('=', receiptchars));
                // printer.PrintLF(Utility.FormatPrintRow(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), receiptchars));
                printer.PrintLF("Time In           TimeOut          Hours");
                decimal totalhours = 0;
                decimal time = 0;
                foreach (DataRow row in employee.Hours.Rows)
                {
                    //initialize with empty data
                    DateTime timein = DateTime.MinValue;
                    DateTime timeout = DateTime.MinValue;
                    time = 0;


                    //just incase there is a blank record .. we don't want it to error out
                    if (row["hours"].ToString() != "") time = decimal.Parse(row["hours"].ToString());

                    if (row["timein"].ToString() != "") timein = DateTime.Parse(row["timein"].ToString());

                    if (row["timeout"].ToString() != "") timeout = DateTime.Parse(row["timeout"].ToString());

                    printer.PrintLF(timein.ToString("MM/dd hh:mm tt") + " => " + (timeout == DateTime.MinValue ? "              " : timeout.ToString("MM/dd hh:mm tt")) + " : " + String.Format("{0:0.00}", time).PadLeft(5));


                    if (row["note"].ToString() != "") printer.PrintLF(row["note"].ToString());

                    totalhours += time;

                }


                printer.LineFeed();
                printer.PrintLF(new String('=', receiptchars));

                printer.PrintLF("Total Hours: " + String.Format("{0:0.00}", totalhours).PadLeft(23));



                printer.LineFeed();

                printer.Send();
                printer.Cut();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Print Employee Hours:" + ex.Message);
            }
        }

    }
}
