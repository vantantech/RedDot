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

        // DATA INSERTS

        public int DBCreateTicket(int employeeid)
        {

            _dbconnect.Command("Insert into sales(employeeid,status) values (" + employeeid + ",'Open')");

            DataTable maxtable = _dbconnect.GetData("Select max(id) as maxid from sales where employeeid =" + employeeid, "max");
            if (maxtable.Rows.Count > 0)
            {
                return int.Parse(maxtable.Rows[0]["maxid"].ToString());
            }
            else return 0;
        }





        public int DBAddProductLineItem(int salesid, int employeeid, MenuItem prod,int quantity, string custom1, string custom2, string custom3, string custom4)
        {
            int result;
            string querystring;

            querystring = "Insert into salesitem (productid,salesid,description, quantity, price, discount,type, employeeid,commissiontype,custom1,custom2,custom3,custom4,reportcategory,commissionamt,turnvalue,supplyfee) " +
                " values (" + prod.ID + "," + salesid + ",'" + prod.MenuPrefix + " " +  prod.Description + "'," + quantity + "," + prod.Price + "," + prod.Discount +
                ",'" + prod.Type + "'," + employeeid + ",'" + prod.CommissionType + "','" +
                custom1 + "','" + custom2 + "','" + custom3 + "','" + custom4 + "','" + prod.ReportCategory + 
                "'," + prod.CommissionAmt +  "," + prod.TurnValue + "," + prod.SupplyFee +  ")";




            _dbconnect.Command(querystring);

            querystring = "select max(id) as maxid from salesitem where salesid =" + salesid;
            DataTable dt = _dbconnect.GetData(querystring);
            if (dt.Rows.Count > 0)
            {
                result = (int)dt.Rows[0]["maxid"];
            }
            else result = 0;

            return result;
        }



        public bool DBInsertPayment(int salesid, string paytype, decimal amount, decimal netamount, string authorizationCode)
        {
            string querystring = "";
            querystring = "Insert into payment (salesid,description,  amount,netamount, authorcode) values (" + salesid + ",'" + paytype + "'," + amount +  "," + netamount + ",'" + authorizationCode + "')";
            return _dbconnect.Command(querystring);
        }

        public bool DBInsertGratuity(int salesid, int employeeid, decimal amount)
        {
            string querystring = "";
            querystring = "Insert into gratuity (salesid,employeeid,  amount) values (" + salesid + "," + employeeid + "," + amount + ")";
            return _dbconnect.Command(querystring);
        }

        // DATA UPDATE


 
        public bool DBCloseTicket(int salesid, decimal subtotal, decimal total,  bool RewardException, decimal rewardpercent)
        {
            string querystring = "";

            if (salesid == 0) return false;

            //set ticket status to closed 
            querystring = "Update sales set status = 'Closed', subtotal = " + subtotal + ", total = " + total +  "  where id =" + salesid;
            _dbconnect.Command(querystring);


   


            //update all items in ticket to rewardexception = true, if flag is true.  Flag is set to true in the ticket if any single item in ticket has discounts .. that is current logic...
            if(RewardException)
            {
                querystring = "Update salesitem set rewardexception = 1, rewardamount=0  where   salesid=" + salesid;
            }else
            {
                querystring = "Update salesitem set rewardexception =0, rewardamount=0  where   salesid=" + salesid;
            }
            _dbconnect.Command(querystring);


            //by default gift certificate can not earn rewards since it's not a purchase
            querystring = "Update salesitem set rewardexception = 1, rewardamount=0  where (type='Gift Card' or type='Gift Certificate') and   salesid=" + salesid;

            _dbconnect.Command(querystring);




            querystring = "Update salesitem set rewardamount = ((price * quantity) - discount) * " + rewardpercent + "/100  where  rewardexception=0 and  salesid=" + salesid;

            return _dbconnect.Command(querystring);
        }


        public bool DBReverseTicket(int salesid)
        {
            if (salesid == 0) return false;

            string querystring = "";
            //set ticket status to closed
            querystring = "Update sales set status = 'Reversed'   where id =" + salesid;
            return _dbconnect.Command(querystring);

        }

        public bool DBActivateGiftCards(int salesid)
        {
            if (salesid == 0) return false;

            string querystring;
            //set ticket status to closed
            querystring = "insert into giftcard (salesid,accountnumber,amount,cardtype)  select salesid,custom1,price,type from salesitem where salesid= " + salesid + " and (type='Gift Card' or type='Gift Certificate')";
            return _dbconnect.Command(querystring);


        }

       

        public bool DBUpdateAdjustment(int salesid, decimal amount)
        {
            if (salesid == 0) return false;

            string querystring = "";
            querystring = "Update sales set adjustment = " + amount + "  where id =" + salesid;
            return _dbconnect.Command(querystring);
        }

        public bool DBUpdateGratuity(int gratuityid, decimal amount)
        {
            string querystring = "";
            querystring = "Update gratuity set amount = " + amount + " where id = " + gratuityid;
            return _dbconnect.Command(querystring);
        }


        public bool DBUpdateSalesItemString(int lineitemid, string fieldstr , string fieldval)
        {
            string querystring = "";
            querystring = "Update salesitem set " + fieldstr + " = '" + fieldval + "' where id = " + lineitemid;
            return _dbconnect.Command(querystring);

        }

        public bool DBUpdateSalesItemValue(int lineitemid, string fieldstr, decimal fieldval)
        {
            string querystring = "";
            querystring = "Update salesitem set " + fieldstr + " = " + fieldval + " where id = " + lineitemid;
            return _dbconnect.Command(querystring);

        }

        public bool DBUpdateSalesValue(int salesid, string fieldstr, int fieldval)
        {
            string querystring = "";
            querystring = "Update sales set " + fieldstr + " = " + fieldval + " where id = " + salesid;
            return _dbconnect.Command(querystring);

        }
        public bool DBUpdateSalesString(int salesid, string fieldstr, string fieldval)
        {
            string querystring = "";
            querystring = "Update sales set " + fieldstr + " = '" + fieldval + "' where id = " + salesid;
            return _dbconnect.Command(querystring);

        }
        // DATA DELETE



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

        public bool DBDeleteGratuity(int salesid, int employeeid)
        {
            string querystring;
            if(employeeid == 0)  querystring = "Delete from  gratuity where salesid=" + salesid;
            else querystring = "Delete from  gratuity where salesid=" + salesid + " and employeeid =" + employeeid;

            return _dbconnect.Command(querystring);

        }

        public bool DBDeleteSales(int salesid)
        {
            string querystring;
            querystring = "Delete from  sales where id=" + salesid;
            return _dbconnect.Command(querystring);

        }





        public bool DBVoidTicket(int salesid, string reason)
        {
            if (salesid == 0) return false;

            if (reason.Length > 50) reason = reason.Substring(0, 50);

            string querystring = "";
            //set ticket status to closed
            querystring = "Update sales set status = 'Voided', custom4='" + reason + "'   where id =" + salesid;
            return _dbconnect.Command(querystring);
        }


        //can only delete if ticket is Open status
        public bool DBDeleteTicket(int salesid)
        {
            bool result;
            result = DBDeleteLineItems(salesid);

            if (result)
            {

                result = DBDeletePayments(salesid);
                if (result)
                {
                    result = DBDeleteSales(salesid);
                }

            }

            return result;
        }







        //Select

        public DataTable GetGiftCardList()
        {

            string query = "select *,(amount- coalesce(spent,0)) as balance from (select sum(amount) as amount,accountnumber, salesid, datesold,cardtype from giftcard group by accountnumber) as giftcardpurchase " +
                            " left outer join (select sum(amount) as spent, authorcode from payment where (description='Gift Card' or description='Gift Certificate') group by authorcode ) as giftcardspent " +
                            " on giftcardpurchase.accountnumber = giftcardspent.authorcode";

            DataTable table = _dbconnect.GetData(query);
            return table;
        }
        public decimal GetGiftCardBalance(string acctno)
        {

            string query = "select *,amount-coalesce( spent,0) as balance from (select sum(amount) as amount, accountnumber from giftcard where accountnumber='" + acctno + "') as giftcardpurchase " +
                            " left outer join (select sum(amount) as spent, authorcode from payment where authorcode = '" + acctno + "') as giftcardspent  " +
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
            DataTable table = _dbconnect.GetData("select * from payment where salesid =" + salesid, "table");
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
