using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DBEmployee
    {

        DBConnect dbconnect;

        public DBEmployee()
        {
            dbconnect = new DBConnect();


        }

        public DataTable GetEmployeeAll()
        {
            DataTable tbl = new DataTable();
            tbl = dbconnect.GetData("Select * from employee", "employee");
            return tbl;

        }

        public DataTable GetEmployeeActive()
        {
            DataTable tbl = new DataTable();
            tbl = dbconnect.GetData("Select * from employee where active=1", "employee");
            return tbl;

        }

        public DataTable GetEmployeeInactive()
        {
            DataTable tbl = new DataTable();
            tbl = dbconnect.GetData("Select * from employee where active=0", "employee");
            return tbl;

        }

        public DataTable GetEmployee(int id)
        {

            return dbconnect.GetData("select * from employee where id=" + id.ToString(), "Employee");
        }

    }
}
