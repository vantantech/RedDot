using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WebSync
{
    public class DBTicket
    {

        DBConnect _dbconnect;
        int m_locationid = 1;

        public DBTicket()
        {
          
            _dbconnect = new DBConnect();
        }

        public DBTicket(int locationid)
        {
            m_locationid = locationid;
            _dbconnect = new DBConnect();
        }

       


        

        //Select

        public DataTable GetGiftCardList(bool showall)
        {

            string query = "select cardtype,accountnumber,amount, coalesce(spent,0) as spent,(coalesce(amount,0)- coalesce(spent,0)) as balance from (select sum(coalesce(amount,0)) as amount,accountnumber, salesid, datesold,cardtype from giftcard group by accountnumber) as giftcardpurchase " +
                            " left outer join (select sum(coalesce(amount,0)) as spent, authorcode from payment where (description='Gift Card' or description='Gift Certificate') group by authorcode ) as giftcardspent " +
                            " on giftcardpurchase.accountnumber = giftcardspent.authorcode";

            if(showall == false)
            {
                query =  "select * from (" + query + ") as giftlist where balance > 0";
            }

            DataTable table = _dbconnect.GetData(query);
            return table;
        }

        public DataTable GetGiftCardDetails(string accountno)
        {

            string query = "select salesid, 'purchase' as type, amount from giftcard where accountnumber=" + accountno +
                            " union " +
                            "select salesid, 'redeem',  amount from payment where authorcode= " + accountno;

           

            DataTable table = _dbconnect.GetData(query);
            return table;
        }

        public decimal GetGiftCardBalance(string acctno)
        {

            string query = "select *,amount-coalesce( spent,0) as balance from (select sum(coalesce(amount,0)) as amount, accountnumber from giftcard where accountnumber='" + acctno + "') as giftcardpurchase " +
                            " left outer join (select sum(coalesce(amount,0)) as spent, authorcode from payment where authorcode = '" + acctno + "') as giftcardspent  " +
                            " on giftcardpurchase.accountnumber = giftcardspent.authorcode";
            DataTable table = _dbconnect.GetData(query, "table");

            if (table.Rows.Count > 0)
            {
                if (table.Rows[0]["balance"].ToString() != "") return decimal.Parse(table.Rows[0]["balance"].ToString());
                else return -99;

            } return -99;

        }

        public bool GiftCardOnPayment(int salesid, string giftcardnumber)
        {
            string query = "select * from payment where salesid=" + salesid + " and authorcode='" + giftcardnumber + "'";
            DataTable table = _dbconnect.GetData(query);
            if (table.Rows.Count > 0) return true;
            else return false;
        }

        public string GetTicketStatus(int salesid)
        {
            DataTable table = _dbconnect.GetData("Select status from sales where id=" + salesid, "table");

            if (table.Rows.Count > 0)
            {
                if (table.Rows[0]["status"].ToString() == "Close") return "Closed";
                else return table.Rows[0]["status"].ToString();

            } return null;
        }


        public int GetCustomerID(int salesid)
        {
            DataTable table = _dbconnect.GetData("Select customerid from sales where id=" + salesid, "table");

            if (table.Rows.Count > 0)
            {
                if (table.Rows[0]["customerid"].ToString() == "") return 0;
                else return int.Parse(table.Rows[0]["customerid"].ToString());

            } return 0;
        }


        public bool TicketHasCredit(int salesid)
        {
            DataTable table = _dbconnect.GetData("Select count(id) as total from payment where description in ('Credit','Visa','Mastercard','Discover','American Express') and  salesid=" + salesid, "table");
            if (int.Parse(table.Rows[0]["total"].ToString()) > 0) return true; else return false;
        }

        public DataTable GetSalesTicket(int salesid)
        {

            DataTable table = _dbconnect.GetData("Select * from sales where id=" + salesid, "table");
            return table;

        }

        public bool HasBeenPaid(int salesid, string paymenttype)
        {
            DataTable table = _dbconnect.GetData("Select * from payment  where salesid=" + salesid + " and description ='" + paymenttype + "'", "table");
            if (table.Rows.Count > 1) return true;
            else return false;

        }


        public DataTable GetLineItemsEmployees(int salesid)
        {
            DataTable table = _dbconnect.GetData("select distinct employeeid, displayname, imagesrc from salesitem inner join employee on salesitem.employeeid = employee.id where salesid =" + salesid + "  order by employeeid", "table");
            return table;

        }


        public DataTable GetLineItems(int salesid,int employeeid, string type)
        {
            DataTable table = _dbconnect.GetData("select * from salesitem where employeeid=" + employeeid + " and  salesid =" + salesid + " and type='" + type + "' order by employeeid", "table");
            return table;

        }

        public DataTable GetLineItems(int salesid)
        {
            DataTable table = _dbconnect.GetData("select * from salesitem where  salesid =" + salesid + "  order by employeeid", "table");
            return table;

        }

        public DataTable GetTicketTurnListUsingMax(int salesid , DateTime time)
        {

            string query = "select  coalesce(checkin.turn,0) as turn, displayname,salesitem.employeeid, salesitem.price as total from salesitem " +
                               " left outer join employee on salesitem.employeeid = employee.id " +
                               " left outer join ( select * from  checkin where DATE(checkindate) = '" + time.ToString("yyyy-MM-dd") + "') as checkin " +
                               " on employee.id = checkin.EmployeeID  where salesitem.employeeid > 0 and  salesitem.salesid = " + salesid +
                                "  order by turn"; 

            return _dbconnect.GetData(query);
        }


        public DataTable GetTicketTurnListUsingTotal(int salesid, DateTime time)
        {

            string query = "select  coalesce(checkin.turn,0) as turn, displayname,salesitem.employeeid, sum(salesitem.price) as total from salesitem " +
                          " left outer join employee on salesitem.employeeid = employee.id " +
                          " left outer join ( select * from  checkin where DATE(checkindate) = '" + time.ToString("yyyy-MM-dd") + "') as checkin " +
                          " on employee.id = checkin.EmployeeID  where salesitem.employeeid > 0 and  salesitem.salesid = " + salesid +
                           " group by employeeid order by turn";

            return _dbconnect.GetData(query);
        }


        public DataTable GetTicketTurnListUsingTurnvalue(int salesid, DateTime time)
        {

            string query = "select  coalesce(checkin.partialturn,0) as partialturn, coalesce(checkin.turn,0) as turn, displayname,salesitem.employeeid, sum(salesitem.turnvalue) as total from salesitem " +
                          " left outer join employee on salesitem.employeeid = employee.id " +
                          " left outer join ( select * from  checkin where DATE(checkindate) = '" + time.ToString("yyyy-MM-dd") + "') as checkin " +
                          " on employee.id = checkin.EmployeeID  where salesitem.employeeid > 0 and  salesitem.salesid = " + salesid +
                           " group by employeeid order by turn";

            return _dbconnect.GetData(query);
        }

        public DataTable GetPayments(int salesid)
        {
            DataTable table = _dbconnect.GetData("select * from payment where  salesid =" + salesid, "table");
            return table;

        }

  

        //this gets a list of sold item with tips or without tips .. so that's why we cannot build this list from the gratuity table
        public DataTable GetGratuityString(int salesid)
        {
            DataTable table = _dbconnect.GetData("select distinct grat.id, displayname,salesitem.employeeid, grat.amount, coalesce(amount,'[empty]') as amountstr, sum( salesitem.price * salesitem.quantity) as techamount  from salesitem left outer join employee on salesitem.employeeid = employee.id left outer join (select *  from  gratuity where gratuity.salesid = " + salesid + ") as grat on employee.id = grat.EmployeeID where salesitem.employeeid > 0 and  salesitem.salesid = " + salesid + " group by employeeid", "table");
            return table;

        }

        public DataTable GetGratuities(int salesid)
        {
            DataTable table = _dbconnect.GetData("select  *  from  gratuity where salesid = " + salesid , "table");
            return table;

        }

        public decimal GetGratuityTotal(int salesid)
        {
            DataTable table = _dbconnect.GetData("select *  from  gratuity where gratuity.salesid = " + salesid);
            if(table.Rows.Count > 0)
            {
                return (decimal)table.Rows[0]["amount"];
            } else return 0;

        }
        public decimal GetCreditCardSurcharge(string cardname, decimal amount)
        {
            decimal over=0;
            decimal under=0;
            decimal fixamount=0;
            decimal percentage=0;

            string query = "Select * from surcharge where type='payment' and description = '" + cardname + "'";
            DataTable dt = _dbconnect.GetData(query);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["under"].ToString() != "") under = decimal.Parse(dt.Rows[0]["under"].ToString());
                if (dt.Rows[0]["over"].ToString() != "") over = decimal.Parse(dt.Rows[0]["over"].ToString());
                if (dt.Rows[0]["fixamount"].ToString() != "") fixamount = decimal.Parse(dt.Rows[0]["fixamount"].ToString());
                if (dt.Rows[0]["percentage"].ToString() != "") percentage = decimal.Parse(dt.Rows[0]["percentage"].ToString());

                if(under > 0)
                {
                    if (amount < under)
                    {
                        if (fixamount > 0) return fixamount;
                        if (percentage > 0) return amount * percentage * 0.01m;
                        return 0;
                    }
                    else return 0;
                }

                if(over > 0)
                {
                    if (amount > over)
                    {
                        if (fixamount > 0) return fixamount;
                        if (percentage > 0) return amount * percentage * 0.01m;
                        return 0;
                    }
                    else return 0;
                }

                return 0;
            }
            else return 0;

        }





        //-----------Change table
        public bool DBUpdateTable(int salesid, int tablenumber)
        {
            string querystring = "";

            querystring = "update sales set tablenumber =" + tablenumber + " where id=" + salesid;
            return _dbconnect.Command(querystring);

        }



        //-----------Assigns a server/salesperson to the current sales ticket
        public bool DBUpdateEmployeeID(int salesid, int employeeid)
        {
            string querystring = "";

            querystring = "update sales set employeeid =" + employeeid + " where id=" + salesid;
            return _dbconnect.Command(querystring);

        }


        //----------update sales date to the current sales ticket
        public bool DBUpdateSalesDate(int salesid, DateTime salesdate)
        {
            string querystring = "";

            querystring = "update sales set saledate ='" + salesdate.ToString("yyyy-MM-dd") + "' where id=" + salesid;
            return _dbconnect.Command(querystring);

        }


        //------------Assigns a salesperson/tech to a particular item/service sold
        public bool DBUpdateSalesItemEmployeeID(int salesitemid, int employeeid)
        {
            string querystring = "";
            querystring = "update salesitem set employeeid =" + employeeid + " where id=" + salesitemid;
            return _dbconnect.Command(querystring);

        }



        //-------------Override a price for a ticket item
        public bool DBUpdateSalesItemPrice(int salesitemid, decimal amount)
        {
            string querystring = "";
            querystring = "update salesitem set price =" + amount + " where id=" + salesitemid;
            return _dbconnect.Command(querystring);

        }
        //-------------update quantity for a ticket item
        public bool DBUpdateSalesItemQuantity(int salesitemid, int quantity)
        {
            string querystring = "";
            querystring = "update salesitem set quantity =" + quantity + " where id=" + salesitemid;
            return _dbconnect.Command(querystring);

        }


        //-------------Discount for a ticket item
        public bool DBUpdateSalesItemDiscount(int salesitemid, decimal amount)
        {
            string querystring = "";
            querystring = "update salesitem set discount =" + amount + " where id=" + salesitemid;
            return _dbconnect.Command(querystring);

        }






        //------------------Assign a customer to the ticket / order 
        public bool DBUpdateCustomerID(int salesid, int customerid)
        {
            string querystring = "";
            querystring = "update sales set customerid =" + customerid + " where id=" + salesid;
            return _dbconnect.Command(querystring);

        }



    }
}
