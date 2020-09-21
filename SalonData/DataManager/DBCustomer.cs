using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;


namespace RedDot
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

        public DataTable GetCustomerByID(int id)
        {
            return _dbconnect.GetData("select * from customer where id=" + id.ToString(), "Customer");

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
            return _dbconnect.GetData("select customer.*  from customer  order by lastname,firstname", "Customer");

        }
        public DataTable GetCustomerByLastName(string lastname)
        {
            return _dbconnect.GetData("select * from customer where lastname like '%" + lastname + "%' order by firstname", "Customer");
        }
        public DataTable GetCustomerByFirstName(string firstname)
        {
            return _dbconnect.GetData("select * from customer where firstname like '%" + firstname + "%' order by lastname", "Customer");
        }

        public DataTable GetCustomerByNames(string firstname, string lastname)
        {
            return _dbconnect.GetData("select * from customer where firstname like '%" + firstname + "%' and  lastname like '%" + lastname + "%' order by lastname, firstname", "Customer");
        }

        public DataTable GetCreditHistory(int customerid)
        {
            return _dbconnect.GetData("select id,points,creditdate,cash, note from credit where customerid =" + customerid, "Customer");

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
            return _dbconnect.GetData("select id, firstname, lastname,  phone1 , '--' as sent from customer where allowsms=1" , "Customer");

        }
        public int CreateCustomer(string phone1)
        {

            _dbconnect.Command("Insert into customer(phone1) values ('" + phone1 + "')");
            DataTable maxtable = _dbconnect.GetData("Select max(id) as maxid from customer where phone1 ='" + phone1 + "'", "max");
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



            query = "select sales.id,saledate,  total, si.rewardtotal as rewardearned,  reward.amount as rewardused, status " +
                " from sales  " +
                " inner join (select salesid, sum(rewardamount) as rewardtotal from salesitem group by salesid) as si on sales.id = si.salesid " + 
                " left outer join (select * from payment where payment.description='Reward') as reward on sales.id = reward.salesid " +
                " where sales.CustomerID = " + id + " and sales.status='Closed' ";

     

            return _dbconnect.GetData(query, "Table");
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

        public decimal GetRewardBalance(int customerid, decimal rewardpercent)
        {
            string query = " select  sum((total - total * coalesce(rewardexception,0)) * 0.05) + creditamount - sum(coalesce(reward.amount,0)) as rewardbalance from sales  " + 
                        " left outer join (select * from payment where payment.description='Reward') as reward on sales.id = reward.salesid " + 
                         " left outer join ( select customerid, sum(cash) as creditamount from credit where customerid=" + customerid + " ) as credit on sales.customerid = credit.customerid " +
                          " where sales.CustomerID = " + customerid + " and sales.status='Closed'";
          DataTable dt =  _dbconnect.GetData(query, "Table");
          if (dt.Rows[0]["rewardbalance"] != null) return Math.Round( (decimal)dt.Rows[0]["rewardbalance"],2);
          else return 0;

        }


    }
}
