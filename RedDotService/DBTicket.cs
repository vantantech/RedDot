using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RedDotService
{
    public class DBTicket
    {

        DBConnect _dbconnect;


   

        public DBTicket(string connectionstring)
        {
          
            _dbconnect = new DBConnect(connectionstring);
        }

        public void Log(string message)
        {
            _dbconnect.Command("insert into Logs (message) values ('" + message + "')");
        }

        public void SMSReply(string api_id, string fromphone, string tophone, string replytimestamp, string replytext)
        {
            _dbconnect.Command("insert into smsreply (api_id,fromphone,tophone,replytimestamp,replytext) values ('" + api_id + "','" + fromphone + "','" + tophone + "','" + replytimestamp + "','" + replytext + "')");
        }

        public string GetReply(string api_id,string fromphone)
        {
            string sql = "select * from smsreply where api_id='" + api_id + "' and fromphone='" + fromphone + "'";
            DataTable dt= _dbconnect.GetData(sql);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["replytext"].ToString();
            }
            else return "none";
        }
        public bool CloseConnection()
        {
            return _dbconnect.CloseConnection();
        }

        // DATA INSERTS

        public bool DBRemoveTicket(int salesid)
        {
            //delete old record if exist

            try
            {

                DataTable dt = _dbconnect.GetData("select * from sales where id=" + salesid);

                foreach(DataRow item in dt.Rows)
                {
                    _dbconnect.Command("delete from salesitem where  salesid=" + item["id"].ToString());
                    _dbconnect.Command("delete from payment where  salesid=" + item["id"].ToString());
                    _dbconnect.Command("delete from gratuity where  salesid=" + item["id"].ToString());

                    _dbconnect.Command("delete from sales where  id=" + item["id"].ToString());

                }
              
               
                return true;
            }catch
            {
                return false;
            }
          
        }

        private int GetSalesId(int userid, int ticketno)
        {
            DataTable maxtable = _dbconnect.GetData("Select max(id) as maxid from sales where userid =" + userid + " and ticketno=" + ticketno, "max");
            if (maxtable.Rows.Count > 0)
            {

                return int.Parse(maxtable.Rows[0]["maxid"].ToString());

            }
            else return 0;
        }

        public string DBCreateTicket(int userid, SalesRecord sr)
        {

            try
            {

                
                
                if (DBRemoveTicket(sr.TicketNo) == false) return "failed delete";
               

              



                //insert ticket record
                string command = "Insert into sales(userid,ticketno,customerid,saledate,lastupdated,adjustment,status,total,subtotal,note,employeeid,custom1,custom2,custom3,custom4,rewardexception,stationno) values" +
              " (" + userid + "," + sr.TicketNo + "," + sr.CustomerId + ",'" + sr.SalesDate.ToString("yyyyMMdd HH:mm:ss") + "','" + sr.LastUpdated.ToString("yyyyMMdd HH:mm:ss") + "'," + sr.Adjustment + ",'" + sr.Status +
              "'," + sr.Total + "," + sr.SubTotal + ",'" + sr.Note + "'," + sr.EmployeeId + ",'" + sr.Custom1 + "','" + sr.Custom2 + "','" + sr.Custom3 + "','" + sr.Custom4 + "'," +
              sr.RewardException + "," + sr.StationNo + ")";


                string result= _dbconnect.Command(command);

                _dbconnect.CloseConnection();

                return "Insert Ticket:" + result;
              
            }catch(Exception ex)
            {
                return "WriteTicket:" + ex.Message;
            }
     
        }



        public string DBCreateSalesItem(int userid, SalesItemRecord item)
        {
            try
            {
               

                //insert the sales item

                string query = "Insert into salesitem(salesid,description,discount,price,quantity,employeeid,note,commissiontype,type,custom1,custom2,custom3,custom4,reportcategory,commissionamt,supplyfee,turnvalue,rewardamount,rewardexception) values" +
                    " (" + GetSalesId(userid,item.TicketNo) + ",'" + item.Description + "'," + item.Discount + "," + item.Price + "," + item.Quantity + "," + item.EmployeeId +
                    ",'" + item.Note + "','" + item.CommissionType + "','" + item.Type + "','" + item.Custom1 + "','" + item.Custom2 + "','" + item.Custom3 + "','" + item.Custom4 +
                    "','" + item.ReportCategory + "'," + item.CommissionAmt + "," + item.SupplyFee + "," + item.TurnValue + "," + item.RewardAmount + "," + item.RewardException + ")";
                string result = _dbconnect.Command(query);

                _dbconnect.CloseConnection();
                return "Salesitem:" + result;
            }catch(Exception ex)
            {
                return "Salesitem:" + ex.Message;
            }
      
        }


        public string DBCreateSalesPayment(int userid, PaymentRecord item)
        {
            try
            {
             




                //insert payment

                string query = "Insert into payment(salesid,description,authorcode,amount,netamount) values" +
                     " (" +  GetSalesId(userid, item.TicketNo) + ",'" + item.Description + "','" + item.AuthorCode + "'," + item.Amount + "," + item.NetAmount + ")";
                string result = _dbconnect.Command(query);


                _dbconnect.CloseConnection();

                return "Payment:" + result;
            }catch(Exception ex)
            {
                return "Payment:" + ex.Message;
            }

   
        }



        public string DBCreateSalesGratuity(int userid, GratuityRecord item)
        {
            try
            {
               


                //insert gratuity

                string query = "Insert into gratuity(salesid,employeeid,amount) values" +
                      " (" +  GetSalesId(userid, item.TicketNo) + "," + item.EmployeeId + "," + item.Amount + ")";
                string result = _dbconnect.Command(query);


                _dbconnect.CloseConnection();

                return "Gratuiity:" + result;

            }catch(Exception ex)
            {
                return "Gratuity:" + ex.Message;

            }
    
        }




        public string DBCreateEmployee(int userid, EmployeeRecord item)
        {
            try
            {
                string query;
                DataTable dt = _dbconnect.GetData("select * from employee where userid=" + userid + " and employeeid=" + item.EmployeeId);

                if(dt.Rows.Count > 0)
                {
                    query = "Update employee set firstname='" + item.FirstName + "'," +
                        " middlename='" + item.MiddleName + "'," +
                        " lastname ='" + item.LastName + "'," +
                        " active =" + item.Active +
                        " where userid=" + userid + " and employeeid=" + item.EmployeeId;

                }else
                {
                     query = "Insert into employee(userid,employeeid,firstname,middlename, lastname,active) values" +
                 " (" + userid + "," + item.EmployeeId + ",'" + item.FirstName + "','" + item.MiddleName + "','" + item.LastName + "'," + item.Active + ")";
                }

             

                string result = _dbconnect.Command(query);

                _dbconnect.CloseConnection();

                return "Create Employee:" + result;


            }catch(Exception ex)
            {
                return "Create Employee:" + ex.Message;
            }
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
        public DataTable GetGratuities(int salesid)
        {
            DataTable table = _dbconnect.GetData("select distinct grat.id, displayname,salesitem.employeeid, grat.amount, coalesce(amount,'[empty]') as amountstr, sum( salesitem.price * salesitem.quantity) as techamount  from salesitem left outer join employee on salesitem.employeeid = employee.id left outer join (select *  from  gratuity where gratuity.salesid = " + salesid + ") as grat on employee.id = grat.EmployeeID where salesitem.employeeid > 0 and  salesitem.salesid = " + salesid + " group by employeeid", "table");
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





    }
}
