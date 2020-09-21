using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantService
{
    public class DBCallerID
    {

        DBConnect _dbconnect;
        public DBCallerID()
        {

            _dbconnect = new DBConnect();

        }

        public void SaveCaller(string phonenumber, string callername)
        {
            string sql;

            sql = "Insert into callerid (phonenumber, callername) values ('" + phonenumber +"','" + callername + "')";

            _dbconnect.Command(sql);

        }
    }
}

