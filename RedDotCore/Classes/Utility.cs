using Microsoft.Win32;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;


//using License;


namespace RedDot
{
    public class Utility
    {
        public static string GetPictureFile()
        {
            OpenFileDialog picfile = new OpenFileDialog();
            picfile.DefaultExt = "*.*";
            picfile.Filter = "All files (*.*)|*.*|PNG Files (*.png)|*.png|JPG files (*.jpg)|*.jpg|BMP files (*.bmp)|*.bmp";
            string AppPath;
            string selectedPath;

            AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            AppPath = AppPath.ToUpper();

            if (picfile.ShowDialog() == true)
            {
                selectedPath = picfile.FileName;
                selectedPath = selectedPath.ToUpper();
                selectedPath = selectedPath.Replace(AppPath, "");

                return selectedPath.Replace("\\", "\\\\");
            }
            else return null;
        }

        public static void OpenModal(Window parent, Window child)
        {
            if(parent != null)
            {
                // parent.Effect = new BlurEffect();
                parent.Opacity = 0.60;
                // parent.IsEnabled = false;
            }


            child.ShowInTaskbar = false;
            child.Topmost = true;
            child.Owner = parent;
            child.ShowDialog();

            if(parent != null)
            {
                //  parent.Effect = null;
                parent.Opacity = 1;
                //  parent.IsEnabled = true;
                parent.Focus();
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
            }catch
            {
                return "0";
            }
         
        }
        //extension method so that all DAtatable has this method attached to it



        public static string GetImageFilePath(string oldvalue)
        {

            Selection sel = new Selection("Choose Image", "Clear Image") { Topmost = true };
            sel.ShowDialog();

            if (sel.Action == "Clear Image") return ""; //clear
            else if (sel.Action == "") return oldvalue.Replace("\\", "\\\\"); //cancel



            OpenFileDialog picfile = new OpenFileDialog();
            picfile.DefaultExt = "png";
            picfile.Filter = "All files (*.*)|*.*|PNG Files (*.png)|*.png|JPG files (*.jpg)|*.jpg|BMP files (*.bmp)|*.bmp|GIF Files(*.gif)|*.gif";
            string AppPath;
            string selectedPath;

            AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            AppPath = AppPath.ToUpper();

            if (picfile.ShowDialog() == true)
            {
                selectedPath = picfile.FileName;
                selectedPath = selectedPath.ToUpper();
                selectedPath = selectedPath.Replace(AppPath, "");

                return selectedPath.Replace("\\", "\\\\");
            }
            else return oldvalue.Replace("\\", "\\\\");

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
