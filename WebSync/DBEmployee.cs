using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSync
{
    public class DBEmployee
    {
        private DBConnect dbConnect;

        public DBEmployee()
        {
            dbConnect = new DBConnect();
        }

        public DataTable GetEmployeeList()
        {
            string query = "Select * from employee ";

            return dbConnect.GetData(query, "Table");
        }
    }
}
