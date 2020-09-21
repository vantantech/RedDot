using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DBTicket
    {

        DBConnect _dbconnect;

        public DBTicket()
        {
            _dbconnect = new DBConnect();
        }

        public int DBCreateTicket(Ticket ticket)
        {

            _dbconnect.Command("Insert into sales(salesperson,tablenumber) values (" + ticket.CurrentEmployee.ID + "," + ticket.TableNumber + ")");
            DataTable maxtable = _dbconnect.GetData("Select max(id) as maxid from sales where salesperson =" + ticket.CurrentEmployee.ID, "max");
            if (maxtable.Rows.Count > 0)
            {
                return int.Parse(maxtable.Rows[0]["maxid"].ToString());
            }
            else return 0;
        }

        public bool DBCloseTicket(int salesid)
        {
            if (salesid == 0) return false;

            _dbconnect.Command("Update sales set status = 'Closed'  where id =" + salesid);
            return true;
        }

        public bool DBAddLineItem(int salesid, string itemstring)
        {
            bool result;
            string querystring;
            querystring = "Insert into salesitem (salesid,description, quantity, price) values (" + salesid + "," + itemstring + ")";
            result = _dbconnect.Command(querystring);
            return result;
        }


        public bool DBDeleteLineItem(int id)
        {
            string querystring;
            querystring = "Delete from  salesitem where id=" + id;
            return _dbconnect.Command(querystring);

        }

        public bool DBDeleteLineItems(int salesid)
        {
            string querystring;
            querystring = "Delete from  salesitem where salesid=" + salesid;
            return _dbconnect.Command(querystring);

        }


        public bool DBDeletePayment(int id)
        {
            string querystring;
            querystring = "Delete from  payment where id=" + id;
            return _dbconnect.Command(querystring);

        }
        public bool DBDeletePayments(int salesid)
        {
            string querystring;
            querystring = "Delete from  payment where salesid=" + salesid;
            return _dbconnect.Command(querystring);

        }

        public bool DBUpdatePayment(decimal amount,int id)
        {
            string querystring = "";
            querystring = "update payment set amount = " + amount + "  where id=" + id;
           return _dbconnect.Command(querystring);

        }
        public bool DBInsertPayment(int salesid, string paytype, decimal amount)
        {
            string querystring = "";
            querystring = "Insert into payment (salesid,description,  amount) values (" + salesid + ",'" + paytype + "'," + amount + ")";
            return _dbconnect.Command(querystring);

        }

        public DataTable GetSalesTicket(int salesid)
        {

            DataTable table = _dbconnect.GetData("Select * from sales where id=" + salesid, "table");
            return table;

        }

        public DataTable GetLineItems(int salesid)
        {
            DataTable table = _dbconnect.GetData("select * from salesitem where salesid =" + salesid, "table");
            return table;

        }


        public DataTable GetPayments(int salesid)
        {
            DataTable table = _dbconnect.GetData("select * from payment where salesid =" + salesid, "table");
            return table;

        }


        public bool DBUpdateEmployeeID(int salesid, int employeeid)
        {
            string querystring = "";
            querystring = "update sales set salesperson =" + employeeid + " where id=" + salesid;
            return _dbconnect.Command(querystring);

        }


        public bool DBUpdateCustomerID(int salesid, int customerid)
        {
            string querystring = "";
            querystring = "update sales set customerid =" + customerid + " where id=" + salesid;
            return _dbconnect.Command(querystring);

        }



    }
}
