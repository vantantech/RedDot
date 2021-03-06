using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace RedDot
{
    public class VFD
    {


        public static void WriteDisplay(string description, string valuestring, string description2, decimal price2)
        {
            string printline;
            string printline2;

            try
            {
                if (GlobalSettings.Instance.DisplayComPort == "none") return;

                LD220 ld = new LD220();
                ld.Open();
                ld.Clear();
                ld.Write((char)31 + "");
              //  ld.Write((char)10 + "");


                printline = Utility.FormatLeft(description, 12) + Utility.FormatRight(valuestring, 8) + (char)13 + (char)10;
                printline2 = Utility.FormatLeft(description2, 12) + Utility.FormatRight( "$" + String.Format("{0:0.00}", price2),8) ;

                ld.Write(printline);
                ld.Write(printline2);

                ld.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("VFD:Write Display:" + ex.Message);
                return;
            }
        }

        public static void WriteDisplay(string description, decimal price, string description2, decimal price2)
        {
            string printline;
            string printline2;

            try
            {
                if (GlobalSettings.Instance.DisplayComPort == "none") return;

                LD220 ld = new LD220();
                ld.Open();
                ld.Clear();
                ld.Write((char)31 + "");
              //  ld.Write((char)10 + "");



                printline = Utility.FormatLeft(description, 12) + Utility.FormatRight( "$" + String.Format("{0:0.00}", price),8) + (char)13 + (char)10;
                printline2 = Utility.FormatLeft(description2, 12) + Utility.FormatRight( "$" + String.Format("{0:0.00}", price2),8) ;

                ld.Write(printline);
                ld.Write(printline2);

                ld.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("VFD:Write Display:" + ex.Message);
                return;
            }
        }

        public static void WriteRaw(string description, string description2)
        {
            string printline;
            string printline2;

            try
            {
                if (GlobalSettings.Instance.DisplayComPort == "none") return;

                LD220 ld = new LD220();
                ld.Open();
                ld.Clear(); //(char)12

                
               ld.Write((char)31 + "");
            //  ld.Write((char)10 + "");
                printline = Utility.FormatLeft( description,20) + (char)13 + (char)10;
                printline2 = Utility.FormatLeft(description2, 20) ;
                ld.Write(printline);
                ld.Write(printline2);
                ld.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("VFD:WriteRaw Display:" + ex.Message);
                return;
            }
        }

        public static void Clear()
        {

            try
            {
                if (GlobalSettings.Instance.DisplayComPort == "none") return;

                LD220 ld = new LD220();
                ld.Open();
                ld.Clear(); //(char)12
                ld.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("VFD:Write Display:" + ex.Message);
                return;
            }
        }



    }
}
