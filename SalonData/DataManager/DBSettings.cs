using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedDot
{
   public  class DBSettings
    {
       private DBConnect _dbconnect;

       public DBSettings()
       {

           _dbconnect = new DBConnect();
       }

       public bool BoolGetSetting(string item)
       {
           string returnstring;
           try
           {
               returnstring = GetStringSetting(item);
               if (returnstring != "") return bool.Parse(returnstring);
               else 
               {
                   MessageBox.Show("Setting:[" + item + "] Not found in Settings Table");
                   return false;
               }
           }catch(Exception e)
           {
               MessageBox.Show("BoolGetSettings:" + item + ":" +  e.Message);
               return false;
           }
       }

       public int IntGetSetting(string item)
       {
           string returnstring;
           try
           {
               returnstring = GetStringSetting(item);
               if (returnstring != "") return int.Parse(returnstring);
               else
               {
                   MessageBox.Show("Setting:[" + item + "] Not found in Settings Table");
                   return 0;
               }
           }
           catch (Exception e)
           {
               MessageBox.Show("IntGetSettings:" + item + ":" +  e.Message);
               return 0;
           }
       }

       public decimal DecimalGetSetting(string item)
       {
           string returnstring;
           try
           {
               returnstring = GetStringSetting(item);
               if (returnstring != "") return decimal.Parse(returnstring);
               else
               {
                   MessageBox.Show("Setting:[" + item + "] Not found in Settings Table");
                   return 0;
               }
           }
           catch (Exception e)
           {
               MessageBox.Show("BoolGetSettings:" + e.Message);
               return 0;
           }
       }
       public string GetStringSetting(string item)
       {

           string sql;
           DataTable table;
           sql = "Select value from settings where item = '" + item.Trim() + "'";
           table = _dbconnect.GetData(sql, "settings");
           if (table.Rows.Count > 0)
           {

               return table.Rows[0]["value"].ToString();

           }
           else
           {
              
               return "";
           }

       }

       public DateTime GetDateSetting(string item)
       {

           string sql;
           DataTable table;
           sql = "Select value from settings where item = '" + item.Trim() + "'";
           table = _dbconnect.GetData(sql, "settings");
           if (table.Rows.Count > 0)
           {
               if (table.Rows[0]["value"].ToString() != "") return DateTime.Parse(table.Rows[0]["value"].ToString());
               else return DateTime.Today;

           }
           else
           {

               return DateTime.Today;
           }

       }
       public void SaveSetting(string item, string value)
       {
           string sql;
           sql = "Update settings set value = '" + value.Trim() + "' where item ='" + item.Trim() + "'";
           _dbconnect.Command(sql);

       }

       public DataTable GetSettingsbyCategory(string cat)
       {
           string query;

           if (cat == "")
               query = "select * from settings";
           else
           query = "Select * from settings where category='" + cat + "' order by item";
           return _dbconnect.GetData(query);
       }


       public DataTable GetSettingbyID(int id)
       {
           string query;

               query = "Select * from settings where id='" + id + "'";
           return _dbconnect.GetData(query);

       }
    }
}
