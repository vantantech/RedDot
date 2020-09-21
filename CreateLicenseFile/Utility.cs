using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace CreateLicenseFile
{
    public class Utility
    {

        public static void Alert(string message)
        {
            AlertView dlg = new AlertView(message);
            dlg.ShowDialog();

        }

        public static void WriteStringToFile(string filename, string text)
        {

            System.IO.File.WriteAllText(filename, text);
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
                numspace = finalwidth - leftstr.Trim().Length - rightstr.Trim().Length;
                if (numspace > 0) //needs to fill space
                {
                    formattedstr = leftstr.Trim() + new String(' ', numspace) + rightstr.Trim();
                }
                else //combined string is longer than numchars
                {
                    formattedstr = leftstr.Substring(0, finalwidth - rightstr.Trim().Length) + rightstr.Trim();
                }
                return formattedstr;
            }
            catch (Exception e)
            {
                MessageBox.Show("FormatPrintRow:" + e.Message);
                return "";
            }
        }

        public static string GetConnectionString()
        {

            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software");
                key = key.OpenSubKey("RedDot");
                if (key != null)
                {
                    key = key.OpenSubKey("DataBase");

                    return key.GetValue("ConnectionString").ToString();

                }
                else return CreateRegistry();

            }
            catch (Exception e)
            {

                MessageBox.Show("GetConnectionString:" + e.Message);
                return "";
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


        public static bool SaveConnectionString(string connectionstr)
        {
            try
            {

                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);

                key = key.OpenSubKey("RedDot");
                key = key.OpenSubKey("DataBase", true);

                key.SetValue("ConnectionString", connectionstr);
                return true;

            }
            catch (Exception e)
            {
                MessageBox.Show("SaveConnectionstring:" + e.Message);
                return false;
            }


        }


        public static void PlaySound()
        {

            string path;

            try
            {

                path = Path.GetFullPath("media/out.wav");
                //path = Path.GetFullPath("media/sound6.mp3");
                Uri uri = new Uri(path);
                var player = new MediaPlayer();

                player.Open(uri);
                player.Play();
            }
            catch (Exception e)
            {
                string st = Properties.Resources.ResourceManager.GetString("PlaySoundError");
                MessageBox.Show(st + e.Message);
            }
        }

        public static void OpenModal(Window parent, Window child)
        {
            parent.Effect = new BlurEffect();
            parent.IsEnabled = false;
            child.ShowDialog();
            parent.Effect = null;
            parent.IsEnabled = true;
            parent.Focus();

        }


  


        public static bool IsLicenseExpired()
        {
            DateTime expireddate = new DateTime(2016, 12, 30);

            TimeSpan ts = expireddate - DateTime.Now;


            if (ts.Days < 30)
            {
                Alert("Your license will expire in " + ts.Days + " Days");

            }

            if (DateTime.Now > expireddate)
            {
                Alert("Your license has expired.");
                return true;

            }
            else return false;

        }

    }
}
