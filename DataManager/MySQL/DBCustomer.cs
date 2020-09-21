using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;


namespace RedDot.DataManager
{
    public class DBCustomer
    {

        private DBConnect _dbconnect;

        public DBCustomer()
        {
            _dbconnect = new DBConnect();
        }


        public  DataTable CheckID(int customerid)
        {
            DataTable dt;
    
            string query;
            query = "select * from customer where id=" + customerid;
            dt = _dbconnect.GetData(query);
            return dt;

        }

        public void UpdateAllowSMS(int id, bool setting)
        {
            string query;
            if(setting == true)
            {
                query = "update customer set allowsms =1 where id=" + id;

            }else
            {
                query = "update customer set allowsms =0 where id=" + id;

            }

            _dbconnect.Command(query);

        }


        public void AddCredit(int customerid, int points, decimal cash, string note)
        {
            string query;

            query = "insert into credit (customerid, points, cash , note) value (" + customerid + "," + points + "," + cash + ",'" + note + "')";

            _dbconnect.Command(query);

        }

        public void DeleteCredit(int creditid)
        {
            string query;

            query = "delete from credit  where id=" + creditid;


            _dbconnect.Command(query);

        }


        public bool DeleteCustomer(int id)
        {
            string query;

            query = "delete from customer where id=" + id;


           return _dbconnect.Command(query);

        }


        public void UpdateString(int id, string field, string fieldvalue)
        {
            string query;
       
                query = "update customer set " + field + " ='" + fieldvalue + "' where id=" + id;


            _dbconnect.Command(query);

        }

        public void UpdateInt(int id, string field, int fieldvalue)
        {
            string query;

            query = "update customer set " + field + " =" + fieldvalue + " where id=" + id;


            _dbconnect.Command(query);

        }


        public void MigrateTo(int oldcustomerid, int newcustomerid)
        {
            string query;

            query = "update sales set customerid=" + newcustomerid + " where customerid=" + oldcustomerid;
            _dbconnect.Command(query);


            query = "update credit set customerid=" + newcustomerid + " where customerid=" + oldcustomerid;
            _dbconnect.Command(query);

            query = "update customerrewards set customerid=" + newcustomerid + " where customerid=" + oldcustomerid;
            _dbconnect.Command(query);
        }

        public DataTable GetCustomerByID(int id)
        {
            return _dbconnect.GetData("select * from customer where id=" + id.ToString());

        }
        public DataTable LookupByPhone(string phone)
        {

            string sql;
            sql = "select * from customer where phone1 like '%" + phone.Trim() + "%' or phone2 like '%" + phone.Trim() + "%' or phone3 like '%" + phone.Trim() + "%' ";
            return _dbconnect.GetData(sql);
        }

        /// <summary>
        /// Gets list of customers with their ticket count 
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllCustomer()
        {
            return _dbconnect.GetData("select customer.*  from customer  order by lastname,firstname");

        }
        public DataTable GetCustomerByLastName(string lastname)
        {
            return _dbconnect.GetData("select * from customer where lastname like '%" + lastname + "%' order by firstname");
        }
        public DataTable GetCustomerByFirstName(string firstname)
        {
            return _dbconnect.GetData("select * from customer where firstname like '%" + firstname + "%' order by lastname");
        }

        public DataTable GetCustomerByNames(string firstname, string lastname)
        {
            return _dbconnect.GetData("select * from customer where firstname like '%" + firstname + "%' and  lastname like '%" + lastname + "%' order by lastname, firstname");
        }

        public DataTable GetCreditHistory(int customerid)
        {
            return _dbconnect.GetData("select id,points,creditdate,cash, note from credit where customerid =" + customerid);

        }

        public string GetCustomerPhone(int id)
        {
            DataTable dt = _dbconnect.GetData("select phone1 from customer where id=" + id);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["phone1"].ToString();

            }
            else return "";

        }
        public DataTable GetSMSList()
        {
            return _dbconnect.GetData("select id, firstname, lastname,  phone1 , '--' as sent from customer where allowsms=1");

        }
        public int CreateCustomer(string phone1)
        {

            _dbconnect.Command("Insert into customer(phone1) values ('" + phone1 + "')");
            DataTable maxtable = _dbconnect.GetData("Select max(id) as maxid from customer where phone1 ='" + phone1 + "'");
            if (maxtable.Rows.Count > 0)
            {
                return int.Parse(maxtable.Rows[0]["maxid"].ToString());
            }
            else return 0;
        }

        /// <summary>
        /// GetPurchaseHistory   -  Gets a customer's purchase record to calculate the amount of reward earned
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rewardpercent"></param>
        /// <returns></returns>
        public DataTable GetPurchaseHistory(int id)
        {
            string query;

            //exclude void payments 06/15/2018

            query = "select * from customerrewards where void=0 and customerid = " + id;

            return _dbconnect.GetData(query);
        }


   
        public decimal GetRewardCredit(int customerid)
        {
            string query;
            DataTable dt;
            query = "select sum(cash) as rewardcredit from credit where customerid = " + customerid;
            dt = _dbconnect.GetData(query);
            if (dt.Rows.Count > 0)
            {
                return decimal.Parse(dt.Rows[0]["rewardcredit"].ToString());

            }
            else return 0;


        }


        //exclude payments that are voided
        public decimal GetRewardBalance(int customerid)
        {
            string query = "select *,amount-coalesce( spent,0) as balance from (select sum(coalesce(amount,0)) as amount, customerid from giftcard where void=0 and transtype='ADD' and  customerid='" + customerid + "') as rewardadd " +
                             " left outer join (select sum(coalesce(amount,0)) as spent, customerid from giftcard where void=0 and transtype='REDEEM' and customerid = '" + customerid + "') as rewardspent  " +
                             " on rewardadd.customerid = rewardspent.customerid";


            DataTable dt =  _dbconnect.GetData(query);
          if (dt.Rows[0]["balance"].ToString() != "") return Math.Round( decimal.Parse(dt.Rows[0]["rewardbalance"].ToString()),2);
          else return 0;

        }


    }
}
