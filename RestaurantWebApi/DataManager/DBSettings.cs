using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RedDot
{
   public  class DBSettings
    {
       private DBConnect _dbconnect;
        Dictionary<string, bool> _boolsettings;
        Dictionary<string, string> _stringsettings;
        Dictionary<string, int> _intsettings;
        Dictionary<string, decimal> _decimalsettings;


        public DBSettings()
       {
            _boolsettings = new Dictionary<string, bool>();
            _stringsettings = new Dictionary<string, string>();
            _intsettings = new Dictionary<string, int>();
            _decimalsettings = new Dictionary<string, decimal>();
            _dbconnect = new DBConnect();
       }


        public bool BoolGetSetting(string category, string item, string description, string defaultvalue)
        {
            string returnstring;
            try
            {
                bool test;
                if (_boolsettings.TryGetValue(category+item, out test))
                {
                    return test;
                }
                returnstring = GetSettingFromDB(category,item);
                if (returnstring != "notfound")
                {
                    bool result = bool.Parse(returnstring);
                    _boolsettings.Add(category+item, result);
                    return result;
                }
                else
                {
                    //not found so need to create
                    CreateStringSetting(item, defaultvalue, category, description, "bool", "");

                    return bool.Parse(defaultvalue);
                }
            }
            catch (Exception e)
            {
         
                return bool.Parse(defaultvalue);
            }
        }

        public int IntGetSetting(string category, string item, string description, string defaultvalue)
        {
            string returnstring;
            try
            {
                int test;
                if (_intsettings.TryGetValue(category + item, out test))
                {
                    return test;
                }
                returnstring = GetSettingFromDB(category, item);
                if (returnstring != "notfound")
                {
                    int result = int.Parse(returnstring);
                    _intsettings.Add(category + item, result);
                    return result;
                }

                else
                {
                    //not found so need to create
                    CreateStringSetting(item, defaultvalue, category, description, "integer", "");
                    return int.Parse(defaultvalue);
                }
            }
            catch (Exception e)
            {
             
                return int.Parse(defaultvalue);
            }
        }

        public decimal DecimalGetSetting(string category, string item, string description, string defaultvalue)
        {
            string returnstring;
            try
            {
                decimal test;
                if (_decimalsettings.TryGetValue(category + item, out test))
                {
                    return test;
                }
                returnstring = GetSettingFromDB(category, item);

                if (returnstring != "notfound")
                {
                    decimal result = decimal.Parse(returnstring);
                    _decimalsettings.Add(category + item, result);
                    return result;
                }
                else
                {
                    CreateStringSetting(item, defaultvalue, category, description, "decimal", "");
                    return decimal.Parse(defaultvalue);
                }
            }
            catch (Exception e)
            {
            
                return decimal.Parse(defaultvalue);
            }
        }


        public string StringGetSetting(string category, string item, string description, string defaultvalue, string datatype = "string", string data = "")
        {
            string returnstring;
            try
            {

                string test;
                if (_stringsettings.TryGetValue(category + item, out test))
                {
                    return test;
                }
                returnstring = GetSettingFromDB(category,item);
                if (returnstring != "notfound")
                {
                    _stringsettings.Add(category + item, returnstring);
                    return returnstring;
                }
                else
                {
                    CreateStringSetting(item, defaultvalue, category, description, datatype, data);
                    return defaultvalue;
                }
            }
            catch (Exception e)
            {
             
                return defaultvalue;
            }
        }
        public string GetSettingFromDB(string category,string item)
        {

            string sql;
            DataTable table;
            sql = "Select value from settings where category ='" + category + "' and  item = '" + item.Trim() + "'";
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

            sql = "insert into settings (item,value,category,description,type,typedata) values ('" + item + "','" + value + "','" + category + "','" + description + "','" + type + "','" + data + "')";
            return _dbconnect.Command(sql);


        }

        public DateTime GetDateSetting(string category, string item, string description, string defaultvalue)
        {
            string returnstring;
            try
            {
                returnstring = GetSettingFromDB(category, item);
                if (returnstring != "notfound")
                {
                    return DateTime.Parse(returnstring);
                }
                else
                {
                    CreateStringSetting(item, defaultvalue, category, description, "string", "");
                    return DateTime.Parse(defaultvalue);
                }
            }
            catch (Exception ex)
            {
            
                return DateTime.Parse(defaultvalue);
            }


        }

        public TimeSpan GetTimeSetting(string category, string item, string description, string defaultvalue)
        {
            string returnstring;
            try
            {
                returnstring = GetSettingFromDB(category, item);
                if (returnstring != "notfound")
                {
                    return StringToTimeSpan(returnstring);
                }
                else
                {
                    CreateStringSetting(item, defaultvalue, category, description, "string", "");
                    return StringToTimeSpan(defaultvalue);
                }
            }
            catch (Exception ex)
            {
              
                return StringToTimeSpan("23:59");
            }


        }

        public TimeSpan StringToTimeSpan(string timein)
        {
            DateTime dt;
            if (!DateTime.TryParseExact(timein, "h:mm tt", CultureInfo.InvariantCulture,DateTimeStyles.None, out dt))
            {
                return TimeSpan.Parse("23:59");
            }
            TimeSpan time = dt.TimeOfDay;
            return time;
        }

        public void BoolSaveSetting(string category,string item, bool value)
        {
            _boolsettings[category + item] = value;
            SaveSetting(category,item, value.ToString());
        }

        public void IntSaveSetting(string category, string item, int value)
        {
            _intsettings[category + item] = value;
            SaveSetting(category, item, value.ToString());
        }

        public void DecimalSaveSetting(string category, string item, decimal value)
        {
            _decimalsettings[category + item] = value;
            SaveSetting(category, item, value.ToString());
        }

        public void NumericSaveSetting(string category, string item, decimal value)
        {
            _intsettings[category + item] = (int)value;
            _decimalsettings[item] = value;
            SaveSetting(category, item, value.ToString());
        }

        public void StringSaveSetting(string category,string item, string value)
        {
            _stringsettings[category + item] = value;
            SaveSetting(category,item, value);
        }




        public void SaveSetting(string category,string item, string value)
       {
           string sql;
           sql = "Update settings set value = '" + value.Replace("'", "''").Trim() + "' where category='" + category + "' and  item ='" + item.Trim() + "'";
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


        public DataTable GetPrinterLocations()
        {
            string query;

            query = "Select *, 0 as selected from printers";
            return _dbconnect.GetData(query);
        }

        public DataRow GetPrinter(int id)
        {
            string query;
            query = "Select * from printers where id =" + id;
            DataTable dt = _dbconnect.GetData(query);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }
            else return null;

           
          
        }

        public bool AddPrinterLocation(string description, string assignedprinter)
        {
            string sql;
            sql = "Insert into printers ( description , assignedprinter, printermode) values ('" + description + "','" + assignedprinter + "','Epson')";
            return _dbconnect.Command(sql);

        }

    


        public bool UpdatePrinterLocation_AssignedPrinter(int id, string assignedprinter, string description, string printermode, int receiptwidth, bool islabel,  bool landscape)
        {
            string sql;
            sql = "Update printers set assignedprinter = '" + assignedprinter.Trim() + "', description='" + description + "', printermode='" + printermode + "',receiptwidth=" + receiptwidth + ",islabel =" + (islabel?"1":"0") +  ", landscape=" + (landscape?"1":"0") +  "  where id =" + id;
            return _dbconnect.Command(sql);
        }

        public bool DeletePrinterLocation(int id)
        {
            string sql;
            sql = "Delete from printers where id =" + id;
            return _dbconnect.Command(sql);
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

            sql = "truncate salesmodifiers";
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
