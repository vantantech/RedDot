using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedDot.OrderService.Class
{
    public class Utility
    {
        public static string FormatPrintRow(string leftstr, string rightstr, int finalwidth)
        {
            string formattedstr;
            int numspace;

            try
            {
                numspace = finalwidth - leftstr.Length - rightstr.Length;
                if (numspace > 0) //needs to fill space
                {
                    formattedstr = leftstr + new String(' ', numspace) + rightstr;
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
            return MyIni.Read(key, defaultvalue, section);


        }

        public static void PutINIString(string key, string section, string val)
        {
            var MyIni = new IniFile("RedDot.ini");
            MyIni.Write(key, val, section);
        }


    }
}