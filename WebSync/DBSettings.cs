
using System;
using System.Data;


namespace WebSync
{
   public  class DBSettings
    {
       private DBConnect _dbconnect;

       public DBSettings()
       {

           _dbconnect = new DBConnect();
       }

       public bool BoolGetSetting(string category,string item,string description, string defaultvalue)
       {
           string returnstring;
           try
           {
               returnstring = GetSettingFromDB(item);
               if (returnstring != "notfound") return bool.Parse(returnstring);
               else 
               {
                   //not found so need to create
                   CreateStringSetting(item, defaultvalue, category, description, "bool");

                   return bool.Parse(defaultvalue);
               }
           }catch(Exception e)
           {
              // MessageBox.Show("BoolGetSettings:" + item + ":" +  e.Message);
               return bool.Parse(defaultvalue);
           }
       }

       public int IntGetSetting(string category, string item, string description, string defaultvalue)
       {
           string returnstring;
           try
           {
               returnstring = GetSettingFromDB( item);
               if (returnstring != "notfound") return int.Parse(returnstring);
               else
               {
                   //not found so need to create
                   CreateStringSetting(item, defaultvalue, category, description, "numeric");
                   return int.Parse(defaultvalue);
               }
           }
           catch (Exception e)
           {
              // MessageBox.Show("IntGetSettings:" + item + ":" +  e.Message);
               return int.Parse(defaultvalue);
           }
       }

       public decimal DecimalGetSetting(string category, string item, string description, string defaultvalue)
       {
           string returnstring;
           try
           {
               returnstring = GetSettingFromDB( item);
               if (returnstring != "notfound") return decimal.Parse(returnstring);
               else
               {
                   CreateStringSetting(item, defaultvalue, category, description, "numeric");
                   return decimal.Parse(defaultvalue);
               }
           }
           catch (Exception e)
           {
             //  MessageBox.Show("DecimalGetSetting:" + e.Message);
               return decimal.Parse(defaultvalue);
           }
       }


       public string StringGetSetting(string category, string item, string description, string defaultvalue)
       {
           string returnstring;
           try
           {
               returnstring = GetSettingFromDB(item);
               if (returnstring != "notfound") return returnstring;
               else
               {
                   CreateStringSetting(item, defaultvalue, category, description, "string");
                   return defaultvalue;
               }
           }
           catch (Exception e)
           {
             //  MessageBox.Show("StringGetSettings:" + e.Message);
               return defaultvalue;
           }
       }
       public string GetSettingFromDB( string item)
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
              
               return "notfound";
           }

       }

       public bool CreateStringSetting(string item, string value, string category, string description, string type)
       {

           string sql;
      
           sql = "insert into settings (item,value,category,description,type) values ('" + item + "','" + value + "','" + category + "','" + description + "','" + type + "')";
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
                   CreateStringSetting(item, defaultvalue, category, description, "string");
                   return DateTime.Parse(defaultvalue);
               }
           }catch(Exception ex)
           {
             //  MessageBox.Show("Get Date Settings:" + ex.Message);
               return DateTime.Parse(defaultvalue);
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
    }
}
