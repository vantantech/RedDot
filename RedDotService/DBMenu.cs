using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedDotService
{
   
    public class DBMenu
    {
        DBConnect _dbconnect;

        public DBMenu(string connectionstring)
        {
            _dbconnect = new DBConnect(connectionstring);
        }

        public string GetStatus()
        {
            if (_dbconnect.TestConnection()) return "Connected"; else return "Connection Error";
        }
     
        public string DBInsertCategory(Category cat)
        {
            try
            {
             
                string sqlstr = "insert into Category(id,colorcode,lettercode,description,imagesrc,cattype,sortorder) values " +
        " (" + cat.id + ",'" + cat.colorcode + "','" + cat.lettercode + "','" + cat.description + "','" + cat.imagesrc + "','" + cat.cattype + "'," + cat.sortorder + ")";
                string result = _dbconnect.Command(sqlstr);
                return "Insert Category:" + result;
            }catch(Exception ex)
            {
                return ex.Message;
            }
    
        }
    }
}