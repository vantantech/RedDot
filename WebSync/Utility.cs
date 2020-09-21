using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Effects;

//using License;


namespace WebSync
{
    public class Utility
    {

        public static void OpenModal(Window parent, Window child)
        {
           // parent.Effect = new BlurEffect();
            // parent.IsEnabled = false;
            child.ShowInTaskbar = false;
            child.Owner = parent;
            child.ShowDialog();
           //  parent.Effect = null;
           //  parent.IsEnabled = true;
            parent.Focus();

        }

        public static void WriteStringToFile(string filename, string text)
        {

            System.IO.File.WriteAllText(filename,text);
        }

        public static string ReadStringFromFile(string filename)
        {
            return System.IO.File.ReadAllText(filename);
        }


        public static string FormatLeft(string target, int limit)
        {
            if (target == null) return "";
            if (target.Trim().Length >= limit)
            {
                return target.Substring(0, limit);
            }
            else
            {
                return target.PadRight(limit);
            }

        }

        public static string FormatRight(string target, int limit)
        {
            if (target == null) return "";
            if (target.Trim().Length >= limit)
            {
                return target.Substring(0, limit);
            }
            else
            {
                return target.PadLeft(limit);
            }

        }

        public static string FormatPrintRow(string leftstr, string rightstr, int finalwidth)
        {
            string formattedstr;
            int numspace;

            try
            {
                numspace = finalwidth - leftstr.Length - rightstr.Length;
                if (numspace > 0) //needs to fill space
                {
                    formattedstr = leftstr+ new String(' ', numspace) + rightstr;
                }
                else //combined string is longer than numchars
                {
                    formattedstr = leftstr.Substring(0, finalwidth - rightstr.Length) + rightstr;
                }
                return formattedstr;
            }
            catch
            {
                //MessageBox.Show("FormatPrintRow:" + e.Message);
                return "";
            }
        }

   

  

        public static string GetINIString(string key, string section, string defaultvalue)
        {
            var MyIni = new IniFile("RedDot.ini");
            return  MyIni.Read(key,defaultvalue, section);
          

        }

        public static void PutINIString(string key, string section, string val)
        {
            var MyIni = new IniFile("RedDot.ini");
            MyIni.Write(key, val, section);
        }
        public static string GetRegString(string main, string sub)
        {

            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software");
                key = key.OpenSubKey("RedDot");
                if (key != null)
                {
                    key = key.OpenSubKey(main);

                    return key.GetValue(sub).ToString();

                }
                else return CreateRegistry();

            }
            catch
            {

               // MessageBox.Show("Get Reg String" + e.Message);
                return "";
            }
        }

        public static bool SaveRegString(string main, string sub,string valuestr)
        {
            try
            {

                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);

                key = key.OpenSubKey("RedDot");
                key = key.OpenSubKey(main, true);

                key.SetValue(sub, valuestr);
                return true;

            }
            catch 
            {
               // MessageBox.Show("Save Reg String:" + e.Message);
                return false;
            }


        }

        private static string CreateRegistry()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);

            key.CreateSubKey("RedDot");
            key = key.OpenSubKey("RedDot", true);


            key.CreateSubKey("DataBase");
            key = key.OpenSubKey("DataBase", true);

            key.SetValue("ConnectionString", "SERVER=localhost;Port=3306;DATABASE=reddot;UID=root;PASSWORD=sparcman;");
            return "SERVER=localhost;Port=3306;DATABASE=reddot;UID=root;PASSWORD=sparcman;";

        }



        



        public static int RandomPin(int digits)
        {
            if (digits < 4) digits = 4;
            if (digits > 8) digits = 8;
            Random random = new Random();
            int start = (int)Math.Pow(10, digits-1);
            int stop = (int)Math.Pow(10, digits)-1;
            int rand = random.Next(start, stop);
          


            return rand;
        }

        //extension method so that all DAtatable has this method attached to it


    }



    public static class DataUtility
    {

        public static void ToCSV(this DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers  
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
    }
}
