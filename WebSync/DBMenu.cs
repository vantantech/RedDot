using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSync
{
    public class DBMenu
    {
        private DBConnect dbConnect;

        public DBMenu()
        {
            dbConnect = new DBConnect();
        }

        public DataTable GetCategoryList()
        {
            string query = "Select * from category ";

            return dbConnect.GetData(query, "Table");
        }
    }
}
