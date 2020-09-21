using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedDot.DataManager
{
   public  class MySQLDBSettings
    {
       private DBConnect _dbconnect;
        Dictionary<string, bool> _boolsettings;
        Dictionary<string, string> _stringsettings;
        Dictionary<string, int> _intsettings;
        Dictionary<string, decimal> _decimalsettings;

        public MySQLDBSettings()
       {

            _boolsettings = new Dictionary<string, bool>();
            _stringsettings = new Dictionary<string, string>();
            _intsettings = new Dictionary<string, int>();
            _decimalsettings = new Dictionary<string, decimal>();


           _dbconnect = new DBConnect();
       }

       public bool BoolGetSetting(string category,string item,string description, string defaultvalue)
       {
           string returnstring;
           try
           {
                bool test;
                if(_boolsettings.TryGetValue(item, out test))
                {
                    return test;
                }
               returnstring = GetSettingFromDB(item);
                if (returnstring != "notfound")
                {
                    bool result = bool.Parse(returnstring);
                    _boolsettings.Add(item, result);
                    return result;
                }
                else
                {
                    //not found so need to create
                    CreateStringSetting(item, defaultvalue, category, description, "bool", "");

                    return bool.Parse(defaultvalue);
                }
           }catch(Exception e)
           {
               MessageBox.Show("BoolGetSettings:" + item + ":" +  e.Message);
               return bool.Parse(defaultvalue);
           }
       }

       public int IntGetSetting(string category, string item, string description, string defaultvalue)
       {
           string returnstring;
           try
           {
                int test;
                if (_intsettings.TryGetValue(item, out test))
                {
                    return test;
                }
                returnstring = GetSettingFromDB( item);
                if (returnstring != "notfound")
                {
                    int result = int.Parse(returnstring);
                    _intsettings.Add(item, result);
                    return result;
                }
              
               else
               {
                   //not found so need to create
                   CreateStringSetting(item, defaultvalue, category, description, "integer","");
                   return int.Parse(defaultvalue);
               }
           }
           catch (Exception e)
           {
               MessageBox.Show("IntGetSettings:" + item + ":" +  e.Message);
               return int.Parse(defaultvalue);
           }
       }

       public decimal DecimalGetSetting(string category, string item, string description, string defaultvalue)
       {
           string returnstring;
           try
           {
                decimal test;
                if (_decimalsettings.TryGetValue(item, out test))
                {
                    return test;
                }
                returnstring = GetSettingFromDB( item);

                if (returnstring != "notfound")
                {
                    decimal result = decimal.Parse(returnstring);
                    _decimalsettings.Add(item, result);
                    return result;
                }
               else
               {
                   CreateStringSetting(item, defaultvalue, category, description, "decimal","");
                   return decimal.Parse(defaultvalue);
               }
           }
           catch (Exception e)
           {
               MessageBox.Show("Decimal Get Setting:" + category + ":" + item + ":" + e.Message);
               return decimal.Parse(defaultvalue);
           }
       }


       public string StringGetSetting(string category, string item, string description, string defaultvalue, string datatype="string", string data="")
       {
           string returnstring;
           try
           {

                string test;
                if (_stringsettings.TryGetValue(item, out test))
                {
                    return test;
                }
                returnstring = GetSettingFromDB(item);
               if (returnstring != "notfound")
                {
                    _stringsettings.Add(item, returnstring);
                    return returnstring;
                }
                else
               {
                   CreateStringSetting(item, defaultvalue, category, description, datatype,data);
                   return defaultvalue;
               }
           }
           catch (Exception e)
           {
               MessageBox.Show("StringGetSettings:" + e.Message);
               return defaultvalue;
           }
       }
       public string GetSettingFromDB( string item)
       {

           string sql;
           DataTable table;
           sql = "Select value from settings where item = '" + item.Trim() + "'";
           table = _dbconnect.GetData(sql);
        
                if (table.Rows.Count > 0)
                {
                    return table.Rows[0]["value"].ToString();
                }
                else
                {
                    return "notfound";
                }
     
       }

       public bool CreateStringSetting(string item, string value, string category, string description, string type, string data)
       {

           string sql;
      
           sql = "insert into settings (item,value,category,description,type,typedata) values ('" + item + "','" + value + "','" + category + "','" + description + "','" + type + "','" + data +  "')";
           return  _dbconnect.Command(sql);
  

       }

       public DateTime GetDateSetting(string category, string item, string description, string defaultvalue)
       {
           string returnstring;
           try
           {
               returnstring = GetSettingFromDB(item);
               if (returnstring != "notfound")
               {
                   return DateTime.Parse(returnstring);
               }
               else
               {
                   CreateStringSetting(item, defaultvalue, category, description, "string","");
                   return DateTime.Parse(defaultvalue);
               }
           }catch(Exception ex)
           {
               MessageBox.Show("Get Date Settings:" + ex.Message);
               return DateTime.Parse(defaultvalue);
           }
     

       }

        public void BoolSaveSetting(string item, bool value)
        {
            _boolsettings[item] = value;
            SaveSetting(item, value.ToString());
        }

        public void IntSaveSetting(string item, int value)
        {
            _intsettings[item] = value;
            SaveSetting(item, value.ToString());
        }

        public void DecimalSaveSetting(string item, decimal value)
        {
            _decimalsettings[item] = value;
            SaveSetting(item, value.ToString());
        }

        public void StringSaveSetting(string item, string value)
        {
            _stringsettings[item] = value;
            SaveSetting(item, value);
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
           query = "Select * from settings where category='" + cat + "' order by description";
           return _dbconnect.GetData(query);
       }


       public DataTable GetSettingCategories()
       {
           string query;

          
             query = "Select distinct category from settings order by category";
           return _dbconnect.GetData(query);
       }

       public DataTable GetSettingbyID(int id)
       {
           string query;

               query = "Select * from settings where id='" + id + "'";
           return _dbconnect.GetData(query);

       }


        public bool WipeSalesData()
        {
            string sql;
            bool success = true;


            sql = "truncate audit";
            if (!_dbconnect.Command(sql)) success = false;

            sql = "truncate sales";
            if (!_dbconnect.Command(sql)) success = false;

            sql = "truncate salesitem";
            if (!_dbconnect.Command(sql)) success = false;

            sql = "truncate gratuity";
            if (!_dbconnect.Command(sql)) success = false;

            sql = "truncate payment";
            if (!_dbconnect.Command(sql)) success = false;

            return success;
        }

        public bool WipeCustomerData()
        {
            string sql;
            bool success = true;


            sql = "truncate customer";
            if (!_dbconnect.Command(sql)) success = false;

            return success;
        }


        public bool WipeGiftCardData()
        {
            string sql;
            bool success = true;


            sql = "truncate giftcard";
            if (!_dbconnect.Command(sql)) success = false;

            return success;
        }
    }
}
