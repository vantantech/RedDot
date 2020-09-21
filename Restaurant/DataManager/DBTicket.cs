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

        public int GetMaxOrder()
        {
          
            DataTable maxtable = _dbconnect.GetData("Select max(ordernumber) as maxid from sales where DATE(saledate) = CURDATE()", "max");
            if (maxtable.Rows.Count > 0)
            {
                string max = maxtable.Rows[0]["maxid"].ToString();
                if (max != "") return int.Parse(max);
                else return 0;

            }
            else return 0;
        }

        public int DBCreateTicketQS(int employeeid,int tablenumber,  OrderType ordertype, SubOrderType subordertype, int stationno)
        {
           
           


            _dbconnect.Command("Insert into sales(employeeid,customercount,status,tablenumber,ordertype,subordertype,stationno) values (" + employeeid + ",1,'OpenTemp'," + tablenumber + ",'" + ordertype.ToString() + "','" + subordertype.ToString() +  "'," + stationno +   ")");
            DataTable maxtable = _dbconnect.GetData("Select max(id) as maxid from sales where status='OpenTemp' and  employeeid =" + employeeid, "max");
            if (maxtable.Rows.Count > 0)
            {
                string max = maxtable.Rows[0]["maxid"].ToString();
                if (max != "") return int.Parse(max);
                else return 0;
            }
            else return 0;
        }

        public int DBCreateTabletTicket(int employeeid, int tablenumber, OrderType ordertype, SubOrderType subordertype, int stationno)
        {




            _dbconnect.Command("Insert into sales(employeeid,customercount,status,tablenumber,ordertype,subordertype,stationno) values (" + employeeid + ",1,'OpenTablet'," + tablenumber + ",'" + ordertype.ToString() + "','" + subordertype.ToString() + "'," + stationno + ")");
            DataTable maxtable = _dbconnect.GetData("Select max(id) as maxid from sales where status='OpenTablet' and employeeid =" + employeeid, "max");
            if (maxtable.Rows.Count > 0)
            {
                string max = maxtable.Rows[0]["maxid"].ToString();
                if (max != "") return int.Parse(max);
                else return 0;
            }
            else return 0;
        }

        public bool AssignOrderNumber(int salesid)
        {
            int maxorder = GetMaxOrder() + 1;  //get next number in line
            string querystring = "update sales set ordernumber =" + maxorder + " where id =" + salesid;
            _dbconnect.Command(querystring);

            querystring = "update sales set status = 'Open' where (status = 'OpenTemp' or status = 'OpenTablet') and  id =" + salesid;


            return _dbconnect.Command(querystring);
        }

        public bool UpdateStatus(int salesid, TicketStatus status)
        {
            string querystring = "update sales set status='" + status.ToString() + "' where id =" + salesid;
            return _dbconnect.Command(querystring);
        }

        public bool InsertIDCheck(int salesid, DriverLicense dl)
        {
            string querystring;
            querystring = "insert into idcheck (salesid,firstname,lastname,driverslicense,dob, age) values (" + salesid + ",'"  + dl.FirstName + "','" + dl.LastName + "','" + dl.LicenseNo + "','" + dl.DOB.ToString("yyyy-MM-dd HH:mm:ss") + "'," + dl.Age + ")";
            return _dbconnect.Command(querystring);
        }



        public int DBQSAddProductLineItem(int productid,int comboid,decimal combomaxprice, int salesid, int seatnumber, string Description, string Description2, string Description3,string Unit, decimal quantity, decimal weight, decimal price, string type,  string note,  string reportcategory,  bool taxable,  string menuprefix, string custom1, string custom2, string custom3, string custom4, bool priceoverride,bool allowpartial,bool weighted, int sortorder,bool sent=false)
        {
            int result;
            string querystring;

            querystring = "Insert into salesitem (productid,comboid,combomaxprice,salesid,seatnumber,description, description2,description3,unit,quantity,weight, price, type, note,reportcategory, taxable, menuprefix, custom1, custom2, custom3, custom4, specialpricing,allowpartial,weighted,sortorder,sent) " +
                " values (" + productid + "," + comboid + "," + combomaxprice + "," + salesid + "," + seatnumber +  ",'" + Description + "','"  + Description2 + "','" + Description3 + "','" + Unit + "'," + quantity + "," + weight + "," + price +  ",'" + type + "','" + note + "','"  + reportcategory  + "'," +  (taxable ? 1 : 0).ToString() + ",'" + menuprefix + "','" + custom1 + "','" + custom2 + "','" + custom3 + "','" + custom4 + "'," +  (priceoverride?"1":"0") + "," + (allowpartial ? "1" : "0") + "," +  (weighted ? "1":"0") + "," + sortorder + "," + (sent ? "1" : "0") + ")";




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
        public bool DBAddModifier( int salesitemid, int modid,int sortorder, decimal quantity=1)
        {
            bool result;
            string querystring;

            querystring = "Insert into salesmodifiers (salesitemid,modifierid,description,colorcode,quantity,  price, sortorder,quantifiable)  select " + salesitemid + " as salesitemid,modifiers.id as modifierid, description,colorcode," + quantity + ", price," + sortorder + ",quantifiable from modifiers " +
                         " where modifiers.id =" + modid;

            result = _dbconnect.Command(querystring);
            return result;
        }

        public bool DBAddManualModifier(int salesitemid, string description, int quantity, decimal price)
        {
            bool result;
            string querystring;

            querystring = "Insert into salesmodifiers (salesitemid,description,quantity, price, sortorder)  values ( " + salesitemid + ",'" + description + "'," + quantity + "," + price + ", (select coalesce(max(sortorder),0) from salesmodifiers as max where salesitemid=" + salesitemid + ") + 1 )";
                       

            result = _dbconnect.Command(querystring);
            return result;
        }

        public bool DBAddModifierQuantity(int id)
        {
            bool result;
            string querystring;

            querystring = "update salesmodifiers set quantity = quantity + 1 where id = " + id;

            result = _dbconnect.Command(querystring);
            return result;
        }


        public bool DBDeleteModifierQuantity(int id)
        {
            bool result;
            string querystring;

            querystring = "update salesmodifiers set quantity = quantity - 1 where id = " + id;

            result = _dbconnect.Command(querystring);
            return result;
        }

        public bool DBDeleteModifier(int salesmodifierid)
        {
            bool result;
            string querystring;

            querystring = "Delete from  salesmodifiers where id=" + salesmodifierid;

            result = _dbconnect.Command(querystring);
            return result;
        }


  













        public bool DBInsertPayment(int stationno,int salesid, string paytype, decimal amount, decimal netamount, string authorizationCode, string cardtype, string maskedpan, decimal tip, DateTime paymentdate, string transtype)
        {
            string querystring = "";

            if(transtype == "REFUND")
            {
                amount = (-1) * amount;
                netamount = (-1) * netamount;
            }
            querystring = "Insert into payment (stationno,salesid,cardgroup,  amount,netamount, authorcode,cardtype,maskedpan,tipamount,paymentdate,transtype) values (" + stationno + ","  + salesid + ",'" + paytype + "'," + amount + "," + netamount + ",'" + authorizationCode + "','" + cardtype + "','" + maskedpan + "'," + tip + ",'" + paymentdate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + transtype + "')";
            return _dbconnect.Command(querystring);
        }


        public bool DBRedeemGiftCard(int salesid, string paytype, decimal amount, string giftcardnumber, DateTime paymentdate)
        {
            string querystring;

            querystring = "Insert into giftcard (salesid,cardtype,amount,accountnumber,datesold,transtype) values (" + salesid + ",'" + paytype + "'," + amount + ",'" + giftcardnumber + "','" + paymentdate.ToString("yyyy-MM-dd HH:mm:ss") + "','REDEEM')";
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
           string PinVerified,
           string SignatureLine,
           string TipAdjustAllowed,
           string EMV_ApplicationName,
           string EMV_Cryptogram,
           string EMV_CryptogramType,
           string EMV_AID,
           string EMV_CardholderName,
           DateTime paymentdate,
           string CloverPaymentId,
           string CloverOrderId,
           string reason)
        {
            string querystring = "";


            int stationno = GlobalSettings.Instance.StationNo;


            //first check to make sure same payment doesn't exist.
            querystring = "select * from payment where salesid = " + salesid.ToString() + " and responseid='" + ResponseId + "'";
            var table = _dbconnect.GetData(querystring);

            if (table.Rows.Count == 0)
            {


                if (TransType == "01") TransType = "SALE";  //debit sale on PAX returns 01 instead of Sale
                if (TransType == "02") TransType = "RETURN";  


                if (TransType.ToUpper() == "REFUND" || TransType.ToUpper() == "RETURN")
                {
                    authamt = (-1) * authamt;  //authorized amount has tip included
                    requested_amount = (-1) * requested_amount;  //net amount
                    tip = authamt - requested_amount;
                }


                //partial authorizations
                if(authamt < requested_amount)
                {
                    requested_amount = authamt;
                    TouchMessageBox.Show("Partial Amount was Authorized!!!");
                }

                querystring = "Insert into payment (stationno,salesid,cardgroup,  amount,netamount, authorcode,cardtype,maskedpan," +
                    " tipamount,paymentdate, cashbackamount,cardacquisition,responseid,transtype,pinverified,signatureline," +
                    " tipadjustallowed,emv_applicationname,emv_cryptogram,emv_cryptogramtype,emv_aid,cardholdername,custom1, custom2,custom4) values (" +
                    stationno + "," + salesid + ",'" + CardGroup + "'," + authamt + "," + requested_amount + ",'" + ApprovalCode + "','" +
                    CardType + "','" + MaskedPAN + "'," + tip + ",'" + paymentdate.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                    cashbackamount + ",'" + CardAcquisition + "','" + ResponseId + "','" + TransType + "'," + (PinVerified == null ? "0" : PinVerified) + "," +
                    SignatureLine + "," + TipAdjustAllowed + ",'" + (EMV_ApplicationName == null ? "" : EMV_ApplicationName) + "','" + (EMV_Cryptogram == null ? "" : EMV_Cryptogram) + "','" +
                    (EMV_CryptogramType == null ? "" : EMV_CryptogramType) + "','" + (EMV_AID == null ? "" : EMV_AID) + "','" + (EMV_CardholderName == null ? "" : EMV_CardholderName.Replace("'", "''")) + "','" + CloverPaymentId + "','" + CloverOrderId + "','" + reason +  "')";

                var result = _dbconnect.Command(querystring);

                return result;


            }
            return false;


        }

        //for PAX credit card pin pad
        public bool DBVoidCreditPayment(string responseid)
        {
            string querystring = "";
            querystring = "update payment set tipamount =0, void = 1, voiddate ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where responseid =" + responseid;
            return _dbconnect.Command(querystring);
        }

        //for clover pin pad
        public bool DBVoidCreditPayment(string cloverpaymentid, string cloverorderid)
        {
            string querystring = "";
            querystring = "update payment set tipamount =0, void = 1, voiddate ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where custom1 ='" + cloverpaymentid + "' and custom2 ='" + cloverorderid + "'";
            return _dbconnect.Command(querystring);
        }

        //for external pin pad and other payment types
        public bool DBVoidPayment(int id, string reason)
        {
            string querystring = "";
            querystring = "update payment set voiddate = NOW(), custom4='" + reason + "', void = 1 where id =" + id;
            return _dbconnect.Command(querystring);
        }


        public bool DBPayWithAuth(int id, decimal amt)
        {
            string querystring = "";
            querystring = "update payment set transtype='AUTH', amount=" + amt + ", netamount=" + amt + "  where id =" + id;
            return _dbconnect.Command(querystring);
        }


        // DATA UPDATE

        public bool DBDeleteCustomerReward(int salesid)
        {
            string querystring = "";

            //need to delete a previous reward record incase this is a reversed ticket the user closed it again
            querystring = "delete from customerrewards where transtype='ADD' and salesid=" + salesid;
            return _dbconnect.Command(querystring);
        }

        public bool DBInsertCustomerReward(int customerid, int salesid, DateTime saledate, decimal tickettotal, decimal rewardamount, string transtype, string note)
        {
            DBDeleteCustomerReward(salesid);

            string querystring = "insert into customerrewards (customerid,salesid,saledate,tickettotal,amount,transtype,note) values (" + customerid + "," + salesid + ",'" + saledate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + tickettotal + "," + rewardamount + ",'" + transtype + "','" + note + "')";

            return _dbconnect.Command(querystring);
        }



        public bool DBQSCloseTicket(int salesid, decimal subtotal, decimal total, decimal salestax, decimal autotip)
        {
            if (salesid == 0) return false;

            string querystring = "";
            //set ticket status to closed
            querystring = "Update sales set status = 'Closed',closedate= NOW(), subtotal = " + subtotal + ", total = " + total + ", salestax =" + salestax +  ",autotip=" + autotip + "  where id =" + salesid;
            return _dbconnect.Command(querystring);

        }



        public bool DBReverseTicket(int salesid)
        {
            if (salesid == 0) return false;

            string querystring = "";
            //set ticket status to Open 
            querystring = "Update sales set status = 'Open'   where id =" + salesid;
            return _dbconnect.Command(querystring);

        }

        
   

        
    

    public bool DBActivateGiftCards(int salesid)
    {
            if (salesid == 0) return false;



            string querystring;

            querystring = " select id,salesid,custom1,price,type from salesitem where salesid= " + salesid + " and ( type='Gift Card' or type='giftcard')";
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
                    }else
                    {
                        querystring = "insert into giftcard (salesid,accountnumber,amount,cardtype,transtype)  select salesid,custom1,price,type,'ADD' as transtype from salesitem where salesid= " + salesid + " and ( type='Gift Card' or type='giftcard') ";
                        return _dbconnect.Command(querystring);

                    }

                }
            }

            return true;

        }


        public bool DBActivateGiftCertificates(int salesid)
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
                    }else
                    {
                        querystring = "insert into giftcard (salesid,accountnumber,amount,cardtype,transtype)  select salesid,custom1,price,type,'ADD' as transtype from salesitem where salesid= " + salesid + " and type='Gift Certificate' ";
                        return _dbconnect.Command(querystring);
                    }

                }
            }

            return true;
        }

        public bool DBUpdateAdjustment(int salesid, decimal amount, string adjustmenttype, string adjustmentreason, int employeemealid)
        {
            if (salesid == 0) return false;

            string querystring = "";
            querystring = "Update sales set adjustment = " + amount + ",adjustmenttype='" + adjustmenttype + "', adjustmentname='" + adjustmentreason + "', employeemealid=" + employeemealid + "   where id =" + salesid;
            return _dbconnect.Command(querystring);
        }

        public bool DBUpdateAdjustmentAmount(int salesid, decimal amount)
        {
            if (salesid == 0) return false;

            string querystring = "";
            querystring = "Update sales set adjustment = " + amount + "  where id =" + salesid;
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

        public bool DBVoidLineItem(int id, string reason)
        {
            string querystring;



            querystring = "update salesitem set voiddate=NOW() ,  void=1 , custom4='" + reason + "'  where id=" + id + " or comboid=" + id;
            _dbconnect.Command(querystring);

            querystring = "update  salesmodifiers set void=1 where salesitemid=" + id;
            return _dbconnect.Command(querystring);
        }

        //delete single line item
        public bool DBQSDeleteLineItem(int salesitemid)
        {
            string querystring;
      


            querystring = "Delete from  salesitem where id=" + salesitemid + " or comboid=" + salesitemid;
             _dbconnect.Command(querystring);

            querystring = "Delete from  salesmodifiers where salesitemid=" + salesitemid;
            return _dbconnect.Command(querystring);
        }


  

        //delete entire ticket
        public bool DBQSDeleteLineItems(int salesid)
        {
            string querystring;

            querystring = "Delete from  salesmodifiers where salesitemid in ( select id from salesitem where salesid=" + salesid + ")";
             _dbconnect.Command(querystring);


            querystring = "Delete from  salesitem where salesid=" + salesid;
            return _dbconnect.Command(querystring);

        }

 
 

  

        public bool DBDeleteTicket(int salesid)
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
            querystring = "Update sales set status = 'Voided', voiddate = NOW() ,  websyncdate=null, custom4='" + reason + "'   where id =" + salesid;
            bool result;
            result = _dbconnect.Command(querystring);


            return result;
        }




        //Select

        public DataTable GetGiftCardList(bool showall)
        {

            string query = "select cardtype,giftcardpurchase.accountnumber,amount, coalesce(spent,0) as spent,(coalesce(amount,0)- coalesce(spent,0)) as balance from (select sum(coalesce(amount,0)) as amount,accountnumber, salesid, datesold,cardtype from giftcard  where transtype = 'ADD' group by accountnumber) as giftcardpurchase " +
                            " left outer join (select sum(coalesce(amount,0)) as spent, accountnumber from giftcard where  void=0 and transtype='REDEEM' group by accountnumber ) as giftcardspent " +
                            " on giftcardpurchase.accountnumber = giftcardspent.accountnumber order by giftcardpurchase.accountnumber * 1 ";

            if (showall == false)
            {
                query = "select * from (" + query + ") as giftlist where balance > 0";
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

            }
            return -99;

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


  

        public DataTable GetSalesTicket(int salesid)
        {

            DataTable table = _dbconnect.GetData("Select * from sales where id=" + salesid, "table");
            return table;

        }

        public bool HasBeenPaid(int salesid, string paymenttype)
        {
            DataTable table = _dbconnect.GetData("Select * from payment  where salesid=" + salesid + " and cardgroup ='" + paymenttype + "'", "table");
            if (table.Rows.Count > 1) return true;
            else return false;

        }

        public DataTable GetLineItems(int salesid, string type)
        {
            DataTable table = _dbconnect.GetData("select * from salesitem where salesid =" + salesid + " and type='" + type + "'", "table");
            return table;

        }

        public DataTable GetLineItems(int salesid)
        {
            DataTable table = _dbconnect.GetData("select * from salesitem where salesid =" + salesid , "table");
            return table;

        }

        public DataTable GetPrinters()
        {
            DataTable table = _dbconnect.GetData("select * from printers", "table");
            return table;

        }

        public DataTable GetUnsentKitchenItems(int salesid, int printerid)
        {
          
            //gets all the items and combo's items
            string queryold = "select combined.*, product.kitchenprinter from (select salesitem.* from salesitem " +
                            " where salesid = " + salesid + " and type = 'product'   union  " +
                            " select salesitem.* from salesitem as combo inner join salesitem on combo.id = salesitem.comboid " +
                            " where combo.salesid = " + salesid + " and combo.type = 'combo')  as combined inner join product on combined.productid = product.id  where sent=0 and kitchenprinter = " + printerid;

            //only gets items and combo .. not the combo's items
            string query = "select salesitem.*, product.kitchenprinter from salesitem inner join product on salesitem.productid = product.id  where sent=0 and kitchenprinter = " + printerid + " and salesid=" + salesid;

            DataTable table = _dbconnect.GetData(query, "table");
            return table;

        }




        public DataTable GetCollaborationKitchenItems(int salesid, int printerid)
        {

            string query = "select combined.*, product.kitchenprinter from (select salesitem.* from salesitem " +
                            " where salesid = " + salesid + " and type = 'product'   union  " +
                            " select salesitem.* from salesitem as combo inner join salesitem on combo.id = salesitem.comboid " +
                            " where combo.salesid = " + salesid + " and combo.type = 'combo')  as combined inner join product on combined.productid = product.id  where sent=0 and  kitchenprinter != " + printerid;



            DataTable table = _dbconnect.GetData(query, "table");
            return table;

        }

        public DataTable GetLineItemPerSeat(int salesid, int seatnumber)
        {
            string query = "select salesitem.*, product.kitchenprinter from salesitem left outer join product on salesitem.productid = product.id where salesid =" + salesid + " and seatnumber=" + seatnumber;


            DataTable table = _dbconnect.GetData(query , "table");
            return table;

        }

        public DataTable GetSeats(int salesid)
        {
            DataTable table = _dbconnect.GetData("select distinct seatnumber from salesitem where salesid =" + salesid + "   order by seatnumber", "table");
            return table;

        }

        public DataTable GetIDChecks(int salesid)
        {
            DataTable table = _dbconnect.GetData("select * from idcheck where  salesid =" + salesid, "table");
            return table;

        }


        public DataTable GetPayments(int salesid)
        {
            DataTable table = _dbconnect.GetData("select * from payment where transtype !='PreAuth' and   salesid =" + salesid, "table");
            return table;

        }

        public DataRow GetPreAuth(int salesid)
        {
            DataTable table = _dbconnect.GetData("select * from payment where transtype='PreAuth' and  salesid =" + salesid, "table");
            if (table.Rows.Count > 0)
                return table.Rows[0];
            else return null;

        }

        public DataTable GetSalesItemModifiers(int salesitemid)
        {

            string query;
            query = "select * from salesmodifiers where salesitemid = " + salesitemid + " order by sortorder asc";
            return _dbconnect.GetData(query);

        }
        public DataTable GetComboSalesItem(int comboid)
        {

            string query;
             query = "select salesitem.*, product.kitchenprinter from salesitem left outer join product on salesitem.productid = product.id where comboid =" + comboid + " order by salesitem.sortorder asc";

            return _dbconnect.GetData(query);

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

        //-----------Change ticket for an item
        public bool DBUpdateTicketID(int salesitemid, int newsalesid)
        {
            string querystring = "";

            querystring = "update salesitem set salesid =" + newsalesid + " where id=" + salesitemid;
            return _dbconnect.Command(querystring);

        }


        //-----------Assigns a server/salesperson to the current sales ticket
        public bool DBUpdateEmployeeID(int salesid, int employeeid)
        {
            string querystring = "";

            querystring = "update sales set employeeid =" + employeeid + " where id=" + salesid;
            return _dbconnect.Command(querystring);

        }


 
  


        public bool DBUpdateHoldDate(int salesid, DateTime holddate)
        {
            string querystring = "";

            querystring = "update sales set holddate ='" + holddate.ToString("yyyy-MM-dd HH:mm:ss") + "' where id=" + salesid;
            return _dbconnect.Command(querystring);

        }

        public bool DBClearHoldDate(int salesid)
        {
            string querystring = "";

            querystring = "update sales set holddate = null where id=" + salesid;
            return _dbconnect.Command(querystring);

        }


        //-------------Override a price for a sold item
        public bool DBUpdateSalesItemPrice(int salesitemid, decimal amount,bool specialpricing)
        {
            string querystring = "";
            querystring = "update salesitem set price =" + amount + ", specialpricing=" + (specialpricing?"1":"0") +  " where id=" + salesitemid;
            return _dbconnect.Command(querystring);

        }
        //-------------update quantity for a sold item
        public bool DBUpdateSalesItemQuantity(int salesitemid, decimal quantity)
        {
            string querystring = "";
            querystring = "update salesitem set quantity =" + quantity + " where id=" + salesitemid;
            return _dbconnect.Command(querystring);

        }

        //-------------update quantity for a sold item
        public bool DBUpdateSalesItemWeight(int salesitemid, decimal weight)
        {
            string querystring = "";
            querystring = "update salesitem set weight =" + weight + " where id=" + salesitemid;
            return _dbconnect.Command(querystring);

        }

        //-------------update seat for a sold item
        public bool DBUpdateSalesItemSeat(int salesitemid, decimal seatnumber)
        {
            string querystring = "";
            querystring = "update salesitem set seatnumber =" + seatnumber + " where id=" + salesitemid;
            return _dbconnect.Command(querystring);

        }

        //-------------Discount for a sold item
        public bool DBUpdateSalesItemDiscount(int salesitemid, decimal amount, string discounttype,string reason)
        {
            string querystring = "";
            querystring = "update salesitem set discount =" + amount + ", discounttype='" + discounttype + "', discountname='" + reason + "' where id=" + salesitemid;
            return _dbconnect.Command(querystring);

        }






        //------------------Assign a customer to the ticket / order 
        public bool DBUpdateCustomerID(int salesid, int customerid)
        {
            string querystring = "";
            querystring = "update sales set customerid =" + customerid + " where id=" + salesid;
            return _dbconnect.Command(querystring);

        }


        public bool DBUpdateGratuity(int paymentid, decimal amount)
        {
            string querystring = "";
            querystring = "update payment set tipamount=" + amount + " where id=" + paymentid;
            return _dbconnect.Command(querystring);

        }

    }
}
