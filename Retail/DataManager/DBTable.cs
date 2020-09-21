using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DBTable
    {
        DBConnect _dbconnect;

        public DBTable()
        {

            _dbconnect = new DBConnect();
        }

       public  DataTable GetTables(int areaid)
        {
            string query;

            query = "select tablelayout.*, sales.saledate, employee.lastname, employee.id as employeeid from tablelayout  left outer join ( select * from sales where status='Open' )  as sales on tablelayout.number = sales.tablenumber  left outer join employee on sales.employeeid = employee.id where areaid =" + areaid;
            return _dbconnect.GetData(query, "TableLayout");
        }

    }
}
