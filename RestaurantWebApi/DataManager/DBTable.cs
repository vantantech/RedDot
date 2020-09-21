using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DBTable
    {
        DBConnect _dbconnect;

        public DBTable()
        {

            _dbconnect = new DBConnect();
        }



        public int InsertArea(string description)
        {
            string query;

            query = "insert into area  (description) values('" + description + "')";
             _dbconnect.Command(query);

            DataTable maxtable = _dbconnect.GetData("Select max(id) as maxid from area" , "max");
            if (maxtable.Rows.Count > 0)
            {
                return int.Parse(maxtable.Rows[0]["maxid"].ToString());
            }
            else return 0;

        }

        public Boolean DeleteArea(int areaid)
        {
            string query;

            query = "delete from area where id=" + areaid;
            return _dbconnect.Command(query);
        }

        public Boolean DeleteTable(int tableid)
        {
            string query;

            query = "delete from tablelayout where id=" + tableid;
            return _dbconnect.Command(query);
        }

        public string GetAreaName(int id)
        {
            string query;

            query = "select description from area where id =" +id;
            DataTable dt = _dbconnect.GetData(query, "area");
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["description"].ToString();
            }
            else return "error";
        }

        public DataTable GetAreas()
        {
            string query;

            query = "select 0 as selected, area.*,count(tablelayout.id) as numtables from area left outer join tablelayout on area.id = tablelayout.areaid group by area.id";
            return _dbconnect.GetData(query, "area");
        }
       public  DataTable GetTables(int areaid)
        {
            string query;

            query = "select 0 as selected,tablelayout.* from tablelayout where areaid =" + areaid;
            return _dbconnect.GetData(query, "TableLayout");
        }

       public DataRow GetTableByNumber(int tablenumber)
       {
           string query;

           query = "select * from tablelayout where number =" + tablenumber;
          var dt = _dbconnect.GetData(query, "TableLayout");
          if (dt.Rows.Count > 0)
          {
              return dt.Rows[0];
          }
          else return null;
       }



       public DataTable GetMaxTable()
       {
           string query;

           query = "select   * from tablelayout order by number desc limit 1";
           return _dbconnect.GetData(query, "TableLayout");
       }

   

        public bool SaveTable(int id,int left, int top, int width, int height, int number,int seats)
       {
           string query;

           query = "update tablelayout set tableleft=" + left + ",tabletop=" + top + ",width=" + width + ",height=" + height + ",number=" + number + ",seats=" + seats + " where id=" + id;
           return _dbconnect.Command(query);

       }

        public bool InsertTable(int areaid,int left, int top, int width, int height, int number)
        {
            string query;

            if (areaid == 0) return false;

            query = "insert into tablelayout  (areaid,tableleft,tabletop,width,height,number) values(" + areaid + "," + left + "," + top + "," + width + "," + height + "," + number + ")";
            return _dbconnect.Command(query);

        }

 

    }
}
