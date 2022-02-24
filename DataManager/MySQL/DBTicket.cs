using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RedDot.DataManager
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

            DataTable maxtable = _dbconnect.GetData("Select max(id) as maxid from sales where employeeid =" + employeeid);
            if (maxtable.Rows.Count > 0)
            {
                return int.Parse(maxtable.Rows[0]["maxid"].ToString());
            }
            else return 0;
        }





        public bool DBAddProductLineItem(int salesid, int employeeid, int prodid, string description, decimal price, decimal discount, string prodtype, string commissiontype, string reportcategory, decimal commissionamt, decimal turnvalue, decimal suppplyfee, int quantity, string custom1, string custom2, string custom3, string custom4, bool taxable)
        {
            // int result;
            string querystring;

            querystring = "Insert into salesitem (productid,salesid,description, quantity, price, discount,type, employeeid,commissiontype,custom1,custom2,custom3,custom4,reportcategory,commissionamt,turnvalue,supplyfee,taxable) " +
                " values (" + prodid + "," + salesid + ",'" + description + "'," + quantity + "," + price + "," + discount +
                ",'" + prodtype + "'," + employeeid + ",'" + commissiontype + "','" +
                custom1 + "','" + custom2 + "','" + custom3 + "','" + custom4 + "','" + reportcategory +
                "'," + commissionamt + "," + turnvalue + "," + suppplyfee + "," + (taxable ? "1" : "0") + ")";




            return _dbconnect.Command(querystring);

            //returns the new item that was added
            /*
            querystring = "select max(id) as maxid from salesitem where salesid =" + salesid;
            DataTable dt = _dbconnect.GetData(querystring);
            if (dt.Rows.Count > 0)
            {
                result = (int)dt.Rows[0]["maxid"];
            }
            else result = 0;

            return result;
             * */
        }

        public bool DBRedeemGiftCard(int salesid, string paytype, decimal amount, string giftcardnumber, DateTime paymentdate)
        {
            string querystring;

            querystring = "Insert into giftcard (salesid,cardtype,amount,accountnumber,datesold,transtype) values (" + salesid + ",'" + paytype + "'," + amount + ",'" + giftcardnumber + "','" + paymentdate.ToString("yyyy-MM-dd HH:mm:ss") + "','REDEEM')";
            return _dbconnect.Command(querystring);
        }

        public bool DBInsertPayment(int salesid,
            string paytype,
            decimal amount,
            decimal netamount,
            string authorizationCode,
            string cardtype,
            string maskedpan,
            decimal tip,
            DateTime paymentdate,
            string transtype)
        {
            string querystring = "";
            string m_transtype = "";

            switch(transtype.ToUpper())
            {
                case "POSTAUTH":
                case "SALE":
                    m_transtype = "SALE";
                    break;
                case "REFUND":
                case "RETURN":
                    m_transtype = "REFUND";
                    break;

            }

            if (transtype.ToUpper() == "REFUND" || transtype.ToUpper() == "RETURN")
            {
                amount = (-1) * amount;
                netamount = (-1) * netamount;
            }


            querystring = "Insert into payment (salesid,cardgroup,  amount,netamount, authorcode,cardtype,maskedpan,tipamount,paymentdate,transtype) values (" + salesid + ",'" + paytype + "'," + amount +  "," + netamount + ",'" + authorizationCode + "','" + cardtype + "','" + maskedpan +  "'," + tip + ",'" + paymentdate.ToString("yyyy-MM-dd HH:mm:ss")  + "','" + m_transtype +   "')";
            return _dbconnect.Command(querystring);
        }

        public bool DBInsertCreditPayment(
            int salesid,
            decimal requested_amount,
            string CardGroup,
            string ApprovalCode,
            string CardType,
            string MaskedPAN,
            string CardAcquisition,
            string ResponseId,
            decimal authamt,
            decimal cashbackamount,
            decimal tip,
            string TransType,
            int PinVerified,
            string SignatureLine,
            string TipAdjustAllowed,
            string EMV_ApplicationName,
            string EMV_Cryptogram,
            string EMV_CryptogramType,
            string EMV_AID,
            string EMV_CardholderName,
            DateTime paymentdate,
            string CloverPaymentId,
            string CloverOrderId)
        {
            string querystring = "";
          
          



            //first check to make sure same payment doesn't exist.
            querystring = "select * from payment where salesid = " + salesid.ToString() + " and responseid='" + ResponseId + "'";
            var table = _dbconnect.GetData(querystring);

            if(table.Rows.Count == 0)
            {


                if (TransType == "01") TransType = "SALE";  //debit sale on PAX returns 01 instead of Sale
                if (TransType == "02") TransType = "RETURN";  //debit sale on PAX returns 01 instead of Sale


                if (TransType.ToUpper() == "REFUND" || TransType.ToUpper() == "RETURN")
                {
                    authamt = (-1) * authamt;
                    requested_amount = (-1) * requested_amount;
                    tip = authamt - requested_amount;
                }

                //partial authorizations
                if (authamt < requested_amount)
                {
                    requested_amount = authamt;
                 
                }

                querystring = "Insert into payment (salesid,cardgroup,  amount,netamount, authorcode,cardtype,maskedpan," + 
                    " tipamount,paymentdate, cashbackamount,cardacquisition,responseid,transtype,pinverified,signatureline," + 
                    " tipadjustallowed,emv_applicationname,emv_cryptogram,emv_cryptogramtype,emv_aid,cardholdername,custom1, custom2) values (" + 
                    salesid + ",'" + CardGroup + "'," + authamt + "," + requested_amount + ",'" + ApprovalCode + "','" +
                    CardType + "','" + MaskedPAN + "'," + tip + ",'" + paymentdate.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                    cashbackamount + ",'" + CardAcquisition + "','" + ResponseId + "','" + TransType + "'," + PinVerified + "," +
                    SignatureLine + "," + TipAdjustAllowed + ",'" + (EMV_ApplicationName==null?"":EMV_ApplicationName) + "','" + (EMV_Cryptogram==null?"":EMV_Cryptogram) + "','" +
                    (EMV_CryptogramType==null?"":EMV_CryptogramType) + "','" + (EMV_AID==null?"":EMV_AID) + "','" + (EMV_CardholderName==null?"":EMV_CardholderName.Replace("'","''")) + "','" + CloverPaymentId + "','" + CloverOrderId +  "')";

              var result =  _dbconnect.Command(querystring);

                return result;


            } return false;

         
        }

        //for PAX credit card pin pad
        public bool DBVoidCreditPayment(string responseid)
        {
            string querystring = "";
            querystring = "update payment set void = 1, voiddate ='" +  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where responseid ='" + responseid + "'";
            return _dbconnect.Command(querystring);
        }

        //for clover pin pad
        public bool DBVoidCreditPayment(string cloverpaymentid, string cloverorderid)
        {
            string querystring = "";
            querystring = "update payment set void = 1, voiddate ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where custom1 ='" + cloverpaymentid + "' and custom2 ='" + cloverorderid + "'";
            return _dbconnect.Command(querystring);
        }

        //for external pin pad and other payment types
        public bool DBVoidPayment(int id)
        {
            string querystring = "";
            querystring = "update payment set void = 1, transtype='VOIDED', voiddate ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where id =" + id;
            return _dbconnect.Command(querystring);
        }

        public bool DBVoidGiftCard(int salesid, string accountnumber)
        {
            string querystring = "";
            querystring = "update giftcard set void = 1, voiddate ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where salesid =" + salesid + " and accountnumber='" + accountnumber + "' and transtype='ADD'";
            return _dbconnect.Command(querystring);
        }

        public bool DBVoidGiftCardPayment(int salesid, string accountnumber)
        {
            string querystring = "";
            querystring = "update giftcard set void = 1, voiddate ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where salesid =" + salesid + " and accountnumber='" + accountnumber + "' and transtype='REDEEM'";
            return _dbconnect.Command(querystring);
        }

        public bool DBVoidRewardADD(int salesid)
        {
            string querystring = "";
            querystring = "update customerrewards set void = 1, voiddate ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where salesid =" + salesid + " and transtype='ADD'";
            return _dbconnect.Command(querystring);
        }



        public bool DBInsertGratuity(int salesid, int employeeid, decimal amount)
        {
            string querystring = "";
            querystring = "Insert into gratuity (salesid,employeeid,  amount) values (" + salesid + "," + employeeid + "," + amount + ")";
            return _dbconnect.Command(querystring);
        }

        // DATA UPDATE
        public bool DBVoidRewardREDEEM(int salesid)
        {
            string querystring = "";
            querystring = "update customerrewards set void = 1,Note='Voided', voiddate ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where salesid =" + salesid + " and transtype='REDEEM' and void=0";
            return _dbconnect.Command(querystring);
        }
        public bool DBVoidCustomerReward(int salesid)
        {
            string querystring = "";
            querystring = "update customerrewards set void = 1,Note='Voided', voiddate ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where salesid =" + salesid + " and transtype='ADD' and void=0";
            return _dbconnect.Command(querystring);
        }

        public bool DBInsertCustomerReward(int customerid,int salesid, DateTime saledate, decimal tickettotal, decimal rewardamount, string transtype,string note)
        {
            DBVoidCustomerReward(salesid);

            string querystring = "insert into customerrewards (customerid,salesid,saledate,tickettotal,amount,transtype,note) values (" + customerid + "," + salesid + ",'" + saledate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + tickettotal + "," + rewardamount + ",'" + transtype + "','" + note + "')";

          return   _dbconnect.Command(querystring);
        }
 
        public bool DBCloseTicket(int salesid, decimal subtotal, decimal total, decimal salestax)
        {
            string querystring = "";

            if (salesid == 0) return false;

            //set ticket status to closed 
            querystring = "Update sales set status = 'Closed', websyncdate = null,  subtotal = " + subtotal + ", total = " + total +  ", salestax = " + salestax + "  where id =" + salesid;
           return  _dbconnect.Command(querystring);


        }


        public bool DBReverseTicket(int salesid)
        {
            if (salesid == 0) return false;

            string querystring = "";
            //set ticket status to closed
            querystring = "Update sales set status = 'Reversed'   where id =" + salesid;
            return _dbconnect.Command(querystring);

        }

        public bool DBActivateGiftCertificates(int salesid)
        {
            if (salesid == 0) return false;



            string querystring;


            //set ticket status to closed
            querystring = "insert into giftcard (salesid,accountnumber,amount,cardtype,transtype)  select salesid,custom1,price,type,'ADD' as transtype from salesitem where salesid= " + salesid + " and type='Gift Certificate' ";
            return _dbconnect.Command(querystring);
        }

        public bool DBActivateGiftCards(int salesid)
        {
            if (salesid == 0) return false;



            string querystring;


            //set ticket status to closed
            querystring = "insert into giftcard (salesid,accountnumber,amount,cardtype,transtype)  select salesid,custom1,price,type,'ADD' as transtype from salesitem where salesid= " + salesid + " and type='Gift Card' ";
            return _dbconnect.Command(querystring);


        }

        public bool DBUpdateGiftCards(int salesid)
        {
            if (salesid == 0) return false;



            string querystring;

            querystring = " select id,salesid,custom1,price,type from salesitem where salesid= " + salesid + " and  type='Gift Card'";
            DataTable giftcards = _dbconnect.GetData(querystring);
            if (giftcards.Rows.Count > 0)
            {
                foreach (DataRow row in giftcards.Rows)
                {
                    //search to see if exist
                    DataTable exist = _dbconnect.GetData("Select * from giftcard where salesid = " + salesid + " and accountnumber =" + row["custom1"].ToString());
                    if (exist.Rows.Count > 0)
                    {
                        _dbconnect.Command("Update giftcard set amount = " + row["price"].ToString() + " where salesid = " + salesid + " and accountnumber = " + row["custom1"].ToString());
                    }
                 
                }
            }

            return true;

        }


        public bool DBUpdateGiftCertificates(int salesid)
        {
            if (salesid == 0) return false;



            string querystring;


            //set ticket status to closed
            querystring = " select id,salesid,custom1,price,type from salesitem where salesid= " + salesid + " and  type='Gift Certificate'";
            DataTable giftscert = _dbconnect.GetData(querystring);
            if (giftscert.Rows.Count > 0)
            {
                foreach (DataRow row in giftscert.Rows)
                {
                    //search to see if exist
                    DataTable exist = _dbconnect.GetData("Select * from giftcard where salesid = " + salesid + " and accountnumber =" + row["custom1"].ToString());
                    if (exist.Rows.Count > 0)
                    {
                        _dbconnect.Command("Update giftcard set amount = " + row["price"].ToString() + " where salesid = " + salesid + " and accountnumber = " + row["custom1"].ToString());
                    }
                
                }
            }

            return true;
        }

        public bool DBUpdateAdjustment(int salesid, decimal amount, string adjustmenttype, string adjustmentreason)
        {
            if (salesid == 0) return false;

            string querystring = "";
            querystring = "Update sales set adjustment = " + amount + ",adjustmenttype='" + adjustmenttype + "', adjustmentname='" + adjustmentreason + "'  where id =" + salesid;
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
            querystring = "Update salesitem set " + fieldstr + " = '" + fieldval.Replace("'", "''") + "' where id = " + lineitemid;
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

    





        public bool DBVoidTicket(int salesid, string reason)
        {
            if (salesid == 0) return false;

            if (reason.Length > 50) reason = reason.Substring(0, 50);

            string querystring = "";
            //set ticket status to closed
            querystring = "Update sales set status = 'Voided', websyncdate=null, custom4='" + reason + "', total=0, subtotal=0, adjustment=0, salestax=0   where id =" + salesid;
            bool result;
            result =  _dbconnect.Command(querystring);


            return result;
        }



        //no longer use ...  just for reference
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
        //no longer use ...  just for reference
        public bool DBDeleteSales(int salesid)
        {
            string querystring;
            querystring = "Delete from  sales where id=" + salesid;
            return _dbconnect.Command(querystring);

        }




        //Select

        public DataTable GetGiftCardList(bool showall)
        {

            string query = "select cardtype,giftcardpurchase.accountnumber,amount, coalesce(spent,0) as spent,(coalesce(amount,0)- coalesce(spent,0)) as balance from (select sum(coalesce(amount,0)) as amount,accountnumber, salesid, datesold,cardtype from giftcard  where transtype = 'ADD' group by accountnumber) as giftcardpurchase " +
                            " left outer join (select sum(coalesce(amount,0)) as spent, accountnumber from giftcard where  void=0 and transtype='REDEEM' group by accountnumber ) as giftcardspent " +
                            " on giftcardpurchase.accountnumber = giftcardspent.accountnumber order by giftcardpurchase.accountnumber * 1 ";

            if(showall == false)
            {
                query =  "select * from (" + query + ") as giftlist where balance > 0";
            }

            DataTable table = _dbconnect.GetData(query);
            return table;
        }

        public DataTable GetGiftCardDetails(string accountno)
        {

            string query = "select salesid, if(void=1,'add (voided)','add') as type, amount, void from giftcard where accountnumber=" + accountno + " and transtype = 'ADD' " + 
                            " union " +
                            "select salesid,if(void=1,'redeem (voided)', 'redeem') as type,  amount, void from giftcard where accountnumber= " + accountno + " and transtype = 'REDEEM'";

           

            DataTable table = _dbconnect.GetData(query);
            return table;
        }

        public decimal GetGiftCardBalance(string acctno)
        {

            string query = "select *,amount-coalesce( spent,0) as balance " +
                        " from (select sum(coalesce(amount,0)) as amount, accountnumber from giftcard where void=0 and transtype='ADD' and  accountnumber='" + acctno + "') as giftcardpurchase " +
                " left outer join (select sum(coalesce(amount,0)) as spent, accountnumber from giftcard where void=0 and transtype='REDEEM' and accountnumber = '" + acctno + "') as giftcardspent  " +
                            " on giftcardpurchase.accountnumber = giftcardspent.accountnumber";
            DataTable table = _dbconnect.GetData(query);

            if (table.Rows.Count > 0)
            {
                if (table.Rows[0]["balance"].ToString() != "") return decimal.Parse(table.Rows[0]["balance"].ToString());
                else return -99;

            } return -99;

        }

        public bool GiftCertificateExist(string acctno)
        {
            string query = "Select * from giftcard where accountnumber ='" + acctno + "' and cardtype ='Gift Certificate' ";
            DataTable table = _dbconnect.GetData(query);
            if (table.Rows.Count > 0) return true; else return false;
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
            DataTable table = _dbconnect.GetData("Select status from sales where id=" + salesid);

            if (table.Rows.Count > 0)
            {
                if (table.Rows[0]["status"].ToString() == "Close") return "Closed";
                else return table.Rows[0]["status"].ToString();

            } return null;
        }


        public int GetCustomerID(int salesid)
        {
            DataTable table = _dbconnect.GetData("Select customerid from sales where id=" + salesid);

            if (table.Rows.Count > 0)
            {
                if (table.Rows[0]["customerid"].ToString() == "") return 0;
                else return int.Parse(table.Rows[0]["customerid"].ToString());

            } return 0;
        }


  

        public DataTable GetSalesTicket(int salesid)
        {

            DataTable table = _dbconnect.GetData("Select * from sales where id=" + salesid);
            return table;

        }

        public bool HasBeenPaid(int salesid, string paymenttype)
        {
            DataTable table = _dbconnect.GetData("Select * from payment  where salesid=" + salesid + " and cardgroup ='" + paymenttype + "'");
            if (table.Rows.Count > 1) return true;
            else return false;

        }


        public DataTable GetLineItemsEmployees(int salesid)
        {
            DataTable table = _dbconnect.GetData("select distinct employeeid, displayname, imagesrc from salesitem inner join employee on salesitem.employeeid = employee.id where salesid =" + salesid + "  order by employeeid");
            return table;

        }


        public DataTable GetLineItems(int salesid,int employeeid)
        {
            DataTable table = _dbconnect.GetData("select * from salesitem where employeeid=" + employeeid + " and  salesid =" + salesid + " order by employeeid");
            return table;

        }

        public DataTable GetLineItems(int salesid)
        {
            DataTable table = _dbconnect.GetData("select * from salesitem where  salesid =" + salesid + "  order by employeeid");
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
            DataTable table = _dbconnect.GetData("select * from payment where salesid =" + salesid);
            return table;

        }

  

        //this gets a list of sold item with tips or without tips .. so that's why we cannot build this list from the gratuity table
        public DataTable GetGratuityString(int salesid)
        {
            DataTable table = _dbconnect.GetData("select distinct grat.id, displayname,salesitem.employeeid, grat.amount, coalesce(amount,'[empty]') as amountstr, sum( (salesitem.price + salesitem.surcharge) * salesitem.quantity) as techamount  from salesitem left outer join employee on salesitem.employeeid = employee.id left outer join (select *  from  gratuity where gratuity.salesid = " + salesid + ") as grat on employee.id = grat.EmployeeID where salesitem.employeeid > 0 and  salesitem.salesid = " + salesid + " group by employeeid");
            return table;

        }

        public DataTable GetGratuities(int salesid)
        {
            DataTable table = _dbconnect.GetData("select  *  from  gratuity where salesid = " + salesid );
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
        public bool DBUpdateSalesItemDiscount(int salesitemid, decimal amount, string discounttype, string reason)
        {
            string querystring = "";
            querystring = "update salesitem set discount =" + amount + ", discounttype='" + discounttype + "', discountname='" + reason + "' where id=" + salesitemid;
            return _dbconnect.Command(querystring);

        }

        //-------------Upgrade for a ticket item
        public bool DBUpdateSalesItemUpgrade(int salesitemid, decimal amount)
        {
            string querystring = "";
            querystring = "update salesitem set surcharge =" + amount + " where id=" + salesitemid;
            return _dbconnect.Command(querystring);

        }




        //------------------Assign a customer to the ticket / order 
        public bool DBUpdateCustomerID(int salesid, int customerid)
        {
            string querystring = "";
            querystring = "update sales set customerid =" + customerid + " where id=" + salesid;
            return _dbconnect.Command(querystring);

        }

        public bool ResetWebSyncDate(int paymentid)
        {
            string querystring = "update sales inner join payment on sales.id = payment.salesid set websyncdate = null where payment.id = " + paymentid;
            return _dbconnect.Command(querystring);
        }

    }
}
