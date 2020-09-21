using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DBProducts
    {

        DBConnect m_dbconnect;

        public DBProducts()
        {

            m_dbconnect = new DBConnect();
        }



        //------------------------------------------------------------------- PRODUCTS ------------------------------------------------------------------------
        #region PRODUCTS

        public DataTable GetProductsByType(string type)
        {
            string query;

            query = "select * from  product where type='" + type + "' order by id " ;


            return m_dbconnect.GetData(query, "Product");
        }


        public int GetProductIDByMenuPrefix(string menuprefix)
        {
            string query;
            DataTable dt;

            query = "select id from product where menuprefix='" + menuprefix + "'";


            dt =  m_dbconnect.GetData(query, "Menu");

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["id"].ToString() != "") return int.Parse(dt.Rows[0]["id"].ToString());
                else return 0;

            }
            else return 0;
        }


        public DataTable GetMenuPrefix()
        {
            string query;

            query = "select distinct substr(menuprefix,1,1) as menuprefix from product where menuprefix is not null order by menuprefix asc ";


            return m_dbconnect.GetData(query, "Menu");
        }


        public DataTable GetMenuPrefix2(string menuprefix)
        {
            string query;

            query = "select distinct substr(menuprefix,2) as menuprefix from product where substr(menuprefix,1,1) ='" + menuprefix + "' order by cast(substr(menuprefix,2) as unsigned)  ";


            return m_dbconnect.GetData(query, "Menu");
        }
        public DataTable GetAllProducts(string orderby)
        {
            string query;

           // query = "select * from  product order by SUBSTRING(product.menuprefix,1,1), cast(SUBSTRING(product.menuprefix,2) as unsigned), product.modelnumber, product.id ";
             query = "select 'all items' as category, product.* from  product  " + orderby;


            return m_dbconnect.GetData(query, "Product");
        }

        public DataTable GetRecentProducts(string orderby)
        {
            string query;

            query = "select   product.* from  product inner join (select distinct productid from salesitem order by id desc limit 20) as list on product.id = list.productid  " + orderby;


            return m_dbconnect.GetData(query, "Product");
        }
        public DataTable GetProductsByTypeDescription(string type, string description)
        {
            string query;

            query = "select * from  product where type='" + type + "' and (description like '%" + description.Trim() + "%' or modelnumber like '%" + description.Trim() + "%') order by  product.modelnumber, product.id";


            return m_dbconnect.GetData(query, "Product");
        }

 


        public DataTable GetProductsByCat(int catid, string orderby)
        {
            string query;

             query = "select category.description as category, category.colorcode as catcolorcode, product.* from category inner join cat2prod on category.id = cat2prod.catid inner join product on cat2prod.prodid = product.id where category.id =" + catid + orderby ;


            return m_dbconnect.GetData(query, "Product");
        }

        public DataTable GetProductsNOTByCat(string type,int catid, string orderby)
        {
            string query;

            if (type == "") query = "select * from product where id not in ( select prodid from cat2prod where catid=" + catid + ")  order by SUBSTRING(product.menuprefix,1,1), cast(SUBSTRING(product.menuprefix,2) as unsigned),product.id";
            else query = "select * from product where id not in ( select prodid from cat2prod where catid=" + catid + ") and type='" + type + "' " + orderby;


            return m_dbconnect.GetData(query, "Product");
        }

        public DataTable GetProductByID(int productID)
        {
            string query;

            query = "select * from product where id=" + productID;


            return m_dbconnect.GetData(query, "Product");
        }


        public DataTable AddNewProduct()
        {
            string query;

            query = "insert into product (description,colorcode) values ('new item','AliceBlue')";
            m_dbconnect.Command(query);

            query = "select * from product where id = LAST_INSERT_ID()";
            return m_dbconnect.GetData(query);

        }

        public DataTable CloneProduct(int id)
        {
            string query;



            query = "insert into product (menuprefix,description,modprofileid,price,imagesrc,type,taxexempt,commissiontype,colorcode,reportcategory,cost,locationcode,commissionamt) " +
      " select  menuprefix,concat(description,'(clone)'),modprofileid,price,imagesrc,type,taxexempt,commissiontype,colorcode,reportcategory,cost,locationcode,commissionamt from product where id=" + id;
            m_dbconnect.Command(query);

            query = "select * from product where id = LAST_INSERT_ID()";
            return m_dbconnect.GetData(query);

        }
 
        public bool UpdateProductString(int id, string field, string fieldvalue)
        {
            string query;

            query = "update product set " + field + " ='" + fieldvalue + "' where id=" + id;
            return m_dbconnect.Command(query);

        }


  
        public bool CopyColorToProducts(int catid, string colorcode)
        {
            string query;

            query = "update product  inner join cat2prod on product.id = cat2prod.prodid set colorcode ='" + colorcode + "' where catid=" + catid;
            return m_dbconnect.Command(query);
        }

        public bool DeleteProduct(int productid)
        {
            string query;

            query = "delete from product where id=" + productid;
             m_dbconnect.Command(query);

            query = "delete from cat2prod where prodid=" + productid;
            return m_dbconnect.Command(query);

        }

        public bool DBDeductInventory(int productid)
        {
            string query;

            query = "update product set qoh = qoh - 1  where id=" + productid;
            return m_dbconnect.Command(query);

        }
        public bool UpdateNumeric(int id, string field, int fieldvalue)
        {
            string query;

            query = "update product set " + field + " =" + fieldvalue + " where id=" + id;
            return m_dbconnect.Command(query);
        }

        public bool UpdateNumeric(int id, string field, decimal fieldvalue)
        {
            string query;

            query = "update product set " + field + " =" + fieldvalue + " where id=" + id;
            return m_dbconnect.Command(query);
        }

        #endregion

        //------------------------------------------------------------------  Category ---------------------------------------------------------------
        #region CATEGORIES
        public DataTable GetCategoryByID(int catID)
        {
            string query;

            query = "select * from category where id=" + catID;


            return m_dbconnect.GetData(query, "Product");
        }

        public DataTable GetCategories()
        {
            string query;

            query = "select * from category   order by sortorder ";

            return m_dbconnect.GetData(query, "Category");
        }

        public string GetCategoryName(int catid)
        {
            DataTable dt;
            string query = "select * from category where id=" + catid;
            dt = m_dbconnect.GetData(query);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["description"].ToString();
            }
            else return "";

        }
        public bool AddItemToCategory(int catid, int prodid)
        {
            string query;

            query = "insert into cat2prod (catid,prodid) values ( " + catid + "," + prodid + ")";
            return m_dbconnect.Command(query);
        }

        public bool RemoveItemFromCategory(int catid, int prodid)
        {
            string query;

            query = "delete from cat2prod where catid=" + catid + " and prodid=" + prodid;
            return m_dbconnect.Command(query);
        }

        public bool AddCategory(string type, string catname)
        {
            string query;

            query = "insert into category (cattype,description,colorcode) values ('" + type + "','" + catname + "','gray')";
            return m_dbconnect.Command(query);
        }


    
        public bool UpdateCategoryString(int id, string field, string fieldvalue)
        {
            string query;

            query = "update category set " + field + " ='" + fieldvalue + "' where id=" + id;
            return m_dbconnect.Command(query);

        }


        public bool UpdateCategoryInt(int id, string field, int fieldvalue)
        {
            string query;

            query = "update category set " + field + " =" + fieldvalue.ToString() + " where id=" + id;
            return m_dbconnect.Command(query);

        }
        public bool DeleteCategory(int catid)
        {
            string query;

            query = "delete from  category where id=" + catid;
            return m_dbconnect.Command(query);
        }

        #endregion


   

    }
}
