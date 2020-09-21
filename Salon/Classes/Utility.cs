using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Effects;

//using License;


namespace RedDot
{
    public class Utility
    {

        public static void OpenModal(Window parent, Window child)
        {
            try
            {
                if(parent != null)
                {
                    parent.Effect = new BlurEffect();
                    parent.Opacity = 0.50;
                    child.ShowInTaskbar = false;
                    child.Topmost = true;
                    child.Owner = parent;
                    child.ShowDialog();
                    parent.Effect = null;
                    parent.Opacity = 1;
                    parent.Focus();
                }else
                {

                    child.ShowInTaskbar = false;
                    child.Topmost = true;
                    child.Owner = parent;
                    child.ShowDialog();
              
                }
  
          
            }catch(Exception ex)
            {
                if (parent != null)
                {
                    parent.Effect = null;
                    parent.Opacity = 1;
                    parent.Focus();
                }
                  
            }
     

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

               // TouchMessageBox.Show("Get Reg String" + e.Message);
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
               // TouchMessageBox.Show("Save Reg String:" + e.Message);
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

        public static string GenerateReferenceNumber()
        {
            try
            {

                return RandomPin(8).ToString();
            }
            catch
            {
                return "0";
            }

        }


        public static string Get45or451FromRegistry()
        {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
             
                    return CheckFor45DotVersion(releaseKey);
               
            }
        }





        // Checking the version using >= will enable forward compatibility,  
        // however you should always compile your code on newer versions of 
        // the framework to ensure your app works the same. 
        private static string CheckFor45DotVersion(int releaseKey)
                {
                    if (releaseKey >= 461808)
                    {
                        return "4.7.2 or later";
                    }
                    if (releaseKey >= 461308)
                    {
                        return "4.7.1 or later";
                    }
                    if (releaseKey >= 460798)
                    {
                        return "4.7 or later";
                    }
                    if (releaseKey >= 394802)
                    {
                        return "4.6.2 or later";
                    }
                    if (releaseKey >= 394254)
                    {
                        return "4.6.1 or later";
                    }
                    if (releaseKey >= 393295)
                    {
                        return "4.6 or later";
                    }
                    if (releaseKey >= 393273)
                    {
                        return "4.6 RC or later";
                    }
                    if ((releaseKey >= 379893))
                    {
                        return "4.5.2 or later";
                    }
                    if ((releaseKey >= 378675))
                    {
                        return "4.5.1 or later";
                    }
                    if ((releaseKey >= 378389))
                    {
                        return "4.5 or later";
                    }
                    // This line should never execute. A non-null release key should mean 
                    // that 4.5 or later is installed. 
                    return "No 4.5 or later version detected";
                }

        //doesn't work
        public static Bitmap ConvertToBitmap(Byte[] bytes, int width, int height)
        {
            Bitmap bmp;
            using (var ms = new MemoryStream(bytes))
            {
                bmp = new Bitmap(ms);
            }
            return bmp;

        }

        public static Bitmap CopyDataToBitmap(byte[] data, int width, int height)
        {
            //Here create the Bitmap to the know height, width and format
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            //Create a BitmapData and Lock all pixels to be written 
            BitmapData bmpData = bmp.LockBits(
                                 new Rectangle(0, 0, bmp.Width, bmp.Height),
                                 ImageLockMode.WriteOnly, bmp.PixelFormat);

            //Copy the data from the byte array into BitmapData.Scan0
            Marshal.Copy(data, 0, bmpData.Scan0, data.Length);


            //Unlock the pixels
            bmp.UnlockBits(bmpData);


            //Return the bitmap 
            return bmp;
        }

        public static Bitmap CreateBitmap(Byte[] bytes, int width, int height)
        {
            try
            {
                byte[] rgbBytes = new byte[bytes.Length * 3];
                for (int i = 0; i <= bytes.Length - 1; i++)
                {
                    rgbBytes[(i * 3)] = bytes[i];
                    rgbBytes[(i * 3) + 1] = bytes[i];
                    rgbBytes[(i * 3) + 2] = bytes[i];
                }
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                for (int i = 0; i <= bmp.Height - 1; i++)
                {
                    IntPtr p = new IntPtr(data.Scan0.ToInt32() + data.Stride * i);
                    System.Runtime.InteropServices.Marshal.Copy(rgbBytes, i * bmp.Width * 3, p, bmp.Width * 3);
                }

                bmp.UnlockBits(data);

                return bmp;
            }
            catch
            {
                return null;
            }

        }


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



    public class CustomList
    {
        public int id { get; set; }
        public string description { get; set; }
        public string returnstring { get; set; }

    }

}
