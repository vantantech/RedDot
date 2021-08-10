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

        public int DBCreateTicket(int parentid,int employeeid,int stationno)
        {

            _dbconnect.Command("Insert into sales(parentid,employeeid,status,stationno) values (" + parentid + "," + employeeid + ",'Open'," + stationno + ")");
            DataTable maxtable = _dbconnect.GetData("Select max(id) as maxid from sales where employeeid =" + employeeid, "max");
            if (maxtable.Rows.Count > 0)
            {
                return int.Parse(maxtable.Rows[0]["maxid"].ToString());
            }
            else return 0;
        }



        public int DBAddProductLineItem(int productid,int salesid, string Description, int quantity, decimal price, decimal surcharge, decimal discount,
            string type, int employeeid, string note, string commissiontype, string custom1, string custom2, string custom3, string custom4, string reportcategory, string partnumber, string modelnumber, decimal cost, decimal msrp, decimal commissionamt,
            bool taxexempt, string barcode)
        {
            int result;
            string querystring;

            querystring = "Insert into salesitem (productid,salesid,description, quantity, price,surcharge, discount,type, employeeid,note,commissiontype,custom1,custom2,custom3,custom4,reportcategory,partnumber,modelnumber,cost,msrp,commissionamt, taxexempt, barcode) " +
                " values ("+ productid + "," + salesid + ",'" + Description + "'," + quantity + "," + price + "," + surcharge + "," + discount +
                ",'" + type + "'," + employeeid + ",'" + note + "','" + commissiontype + "','" + custom1 + "','" + custom2 + "','" + custom3 + "','" + custom4 + "','" + reportcategory + "','" + partnumber + "','" + modelnumber + "'," + cost + "," + msrp + "," + commissionamt + 
                "," + (taxexempt?1:0).ToString() + ",'" + barcode  +   "')";




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

  

        // DATA UPDATE


        public bool DBCloseTicket(int salesid, decimal subtotal, decimal total, decimal salestax, decimal shopfee, decimal productsubtotal, decimal laborsubtotal)
        {
            if (salesid == 0) return false;

            string querystring = "";
            //set ticket status to closed
            querystring = "Update sales set status = 'Closed',closedate= NOW(), subtotal = " + subtotal + ", total = " + total + ", salestax =" + salestax + ",shopfee=" + shopfee + ",productsubtotal=" + productsubtotal + ",laborsubtotal=" + laborsubtotal + "  where id =" + salesid;
            return _dbconnect.Command(querystring);

        }

        public bool DBPendingTicket(int salesid)
        {
            if (salesid == 0) return false;

            string querystring = "";
            //set ticket status to closed
            querystring = "Update sales set status = 'Pending'  where id =" + salesid;
            return _dbconnect.Command(querystring);

        }

        public bool DBReOpenTicket(int salesid)
        {
            if (salesid == 0) return false;

            string querystring = "";
            //set ticket status to open
            querystring = "Update sales set status = 'Open'  where id =" + salesid;
            return _dbconnect.Command(querystring);

        }

        public bool DBReverseTicket(int salesid)
        {
            if (salesid == 0) return false;

            string querystring = "";
            //set ticket status to reverse
            querystring = "Update sales set status = 'Reversed', reversedate = NOW()  where id =" + salesid;
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

       

        public bool DBUpdateDiscount(int salesid, decimal amount)
        {
            if (salesid == 0) return false;

            string querystring = "";
            querystring = "Update sales set discount = " + amount + "  where id =" + salesid;
            return _dbconnect.Command(querystring);
        }

        public bool DBUpdatePaymentDate(int paymentid, DateTime date)
        {
            if (paymentid == 0) return false;

            string querystring = "";
            querystring = "Update payment set paymentdate = '" + date.ToString("yyyy-MM-dd") + "'  where id =" + paymentid;
            return _dbconnect.Command(querystring);
        }


        public bool DBUpdateSalesItemString(int lineitemid, string fieldstr , string fieldval)
        {
            string querystring = "";
            querystring = "Update salesitem set " + fieldstr + " = '" + fieldval.Replace("'","''") + "' where id = " + lineitemid;
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
            querystring = "Update sales set " + fieldstr + " = '" + fieldval.Replace("'", "''") + "' where id = " + salesid;
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
            querystring = "Update payment set void = 1, voiddate= CURDATE() where id=" + id;
            return _dbconnect.Command(querystring);

        }


  

        public bool DBDeleteSales(int salesid)
        {
            string querystring;
            querystring = "Delete from  sales where id=" + salesid;
            return _dbconnect.Command(querystring);

        }



        public bool DBVoidTicket(int salesid)
        {
            string querystring;
            querystring = "update  sales set status = 'Voided', total =0, subtotal=0, discount=0, shopfeepercent=0, salestax=0, productsubtotal=0, shopfee=0, laborsubtotal=0 where id=" + salesid;
            return _dbconnect.Command(querystring);
        }

        public bool DBDeleteTicket(int salesid)
        {
            bool result;
            result = DBDeleteLineItems(salesid);

            if (result)
            {
                result = DBDeleteSales(salesid);
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

        public DataTable GetLineItems(int salesid, string type)
        {
            DataTable table = _dbconnect.GetData("select * from salesitem where salesid =" + salesid + " and type='" + type + "' order by id", "table");
            return table;

        }

        public DataTable GetLineItems(int salesid)
        {
            DataTable table = _dbconnect.GetData("select * from salesitem where salesid =" + salesid + "  order by id", "table");
            return table;

        }






  

        public DataTable GetPayments(int salesid)
        {
            DataTable table = _dbconnect.GetData("select * from payment where salesid =" + salesid, "table");
            return table;

        }

        public DataTable GetWorkOrder(int salesid)
        {
            DataTable table = _dbconnect.GetData("select * from workorder where salesid =" + salesid, "table");
            return table;

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


        //-----------Assigns a server/salesperson to the current sales ticket
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



        //-------------Override a price for a sold item
        public bool DBUpdateSalesItemPrice(int salesitemid, decimal amount)
        {
            string querystring = "";
            querystring = "update salesitem set price =" + amount + " where id=" + salesitemid;
            return _dbconnect.Command(querystring);

        }
        //-------------update quantity for a sold item
        public bool DBUpdateSalesItemQuantity(int salesitemid, int quantity)
        {
            string querystring = "";
            querystring = "update salesitem set quantity =" + quantity + " where id=" + salesitemid;
            return _dbconnect.Command(querystring);

        }


        //-------------Discount for a sold item
        public bool DBUpdateSalesItemDiscount(int salesitemid, decimal amount)
        {
            string querystring = "";
            querystring = "update salesitem set discount =" + amount + " where id=" + salesitemid;
            return _dbconnect.Command(querystring);

        }

        public bool UpdateWorkOrder(int id,
            string custom1,
            string custom2,
            string custom3,
            string custom4,
            string custom5,
            string custom6,
            string custom7,
            string custom8,
            string custom9,
            string custom10,
            string custom11,
            string custom12,
            string custom13,
            string custom14,
            string custom15,
            string custom16,
            string custom17,
            string custom18,
            string custom19,
            string custom20,
            string custom21
           )
        {
            string querystring = "Update workorder set custom1='" 
                + custom1.Replace("'", "''") 
                + "', custom2='" + custom2.Replace("'", "''") 
                + "', custom3='" + custom3.Replace("'", "''") 
                + "',custom4='" + custom4.Replace("'", "''") 
                + "',custom5='" + custom5.Replace("'", "''") 
                + "',custom6='" + custom6.Replace("'", "''") 
                + "',custom7='" + custom7.Replace("'", "''") 
                + "',custom8='" + custom8.Replace("'", "''") 
                + "',custom9='" + custom9.Replace("'", "''") 
                + "',custom10='" + custom10.Replace("'", "''") 
                + "', custom11='" + custom11.Replace("'", "''") 
                + "', custom12='" + custom12.Replace("'", "''") 
                + "', custom13='" + custom13.Replace("'", "''") 
                + "', custom14='" + custom14.Replace("'", "''") 
                + "', custom15='" + custom15.Replace("'", "''") 
                + "', custom16='" + custom16.Replace("'", "''")
                + "', custom17='" + custom17.Replace("'", "''")
                + "', custom18='" + custom18.Replace("'", "''")
                + "', custom19='" + custom19.Replace("'", "''")
                + "', custom20='" + custom20.Replace("'", "''")
                + "', custom21='" + custom21.Replace("'", "''")
                + "' where id=" + id;
            return _dbconnect.Command(querystring);
        }

        public bool CreateWorkOrder(int salesid, string workordertype, string submittedby)
        {
            string querystring = "insert into workorder (salesid,workordertype,custom17) values (" + salesid + ",'" + workordertype + "','" + submittedby  + "')";
            return _dbconnect.Command(querystring);
        }

        public bool UpdateShipOrderString(int id,string field, string strval)
        {
            string querystring = "update shiporder set " + field + "='" + strval + "' where id=" + id;
            return _dbconnect.Command(querystring);
        }


        public bool CreateShipOrder(int salesid)
        {


            string querystring = "insert into shiporder (salesid, custname, address, city , state, zipcode,phonenumber) select  " + salesid + ", concat(firstname , lastname), address1, city, state, zipcode, phone1 from sales left outer join customer on sales.customerid = customer.id where sales.id =" + salesid;
            return _dbconnect.Command(querystring);
        }

        public bool DeleteShipment(int shiporderid)
        {

            string query = "delete from shipment2item where shiporderid =" + shiporderid;
            _dbconnect.Command(query);

            query = "delete from shiporder where id=" + shiporderid;
            return _dbconnect.Command(query);
        }

        public bool AddItemToShipment(int shiporderid,int salesitemid)
        {
            string querystring = "insert into shipment2item (shiporderid,salesitemid) values (" + shiporderid + "," + salesitemid + ")";
            return _dbconnect.Command(querystring);
        }

        public bool RemoveItemFromShipment(int shipitemid)
        {
            string querystring = "delete from shipment2item where id =" + shipitemid;
            return _dbconnect.Command(querystring);
        }
        public DataTable GetShipOrders(int salesid)
        {
            DataTable table = _dbconnect.GetData("select * from shiporder where salesid =" + salesid, "table");
            return table;

        }


        public DataTable GetShipAvailableItems(int salesid)
        {
            string query = "select salesitem.* from salesitem " +
                         " left outer join shipment2item on salesitem.id = shipment2item.salesitemid " +
                          " where shipment2item.id is null  and salesitem.type = 'product' and salesitem.salesid = " + salesid;

            DataTable table = _dbconnect.GetData(query);
            return table;

        }


        public DataTable GetShipSelectedItems(int shipid)
        {
            string query = "select shipment2item.id as shipitemid, salesitem.* from shiporder inner join shipment2item on shiporder.id = shipment2item.shiporderid inner join salesitem on shipment2item.salesitemid = salesitem.id where shiporder.id =" + shipid;

            DataTable table = _dbconnect.GetData(query);
            return table;

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
