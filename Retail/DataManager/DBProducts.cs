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

        public DataTable GetLocations()
        {
            string query;

            query = "select * from location";


            return m_dbconnect.GetData(query, "Location");
        }


        //------------------------------------------------------------------- PRODUCTS ------------------------------------------------------------------------
        #region PRODUCTS



        public DataTable GetProductsByType(string type)
        {
            string query;

            query = "select product.*,coalesce(cost,0)*coalesce(qoh,0) as itemvalue, location.description as locationname from  product left outer join location on product.locationid = location.id where type='" + type + "' order by product.modelnumber" ;


            return m_dbconnect.GetData(query, "Product");
        }




        //Inventory export to excel
        public DataTable GetAllProductsCSV()
        {
            string query;

            query = "select category.description as category, product.*,coalesce(cost,0)*coalesce(qoh,0) as itemvalue, location.description as locationname   from  product left outer join cat2prod on product.id = cat2prod.prodid left join category on cat2prod.catid = category.id left outer join location on product.locationid = location.id  where type='product' order by category.description, product.modelnumber ";


            return m_dbconnect.GetData(query, "Product");
        }


        public DataTable GetActiveProductsCSV()
        {
            string query;

            query = "select category.description as category, product.*,coalesce(cost,0)*coalesce(qoh,0) as itemvalue, location.description as locationname   from  product inner join cat2prod on product.id = cat2prod.prodid inner join category on cat2prod.catid = category.id left outer join location on product.locationid = location.id  where type='product' order by category.description, product.modelnumber ";


            return m_dbconnect.GetData(query, "Product");
        }

        public DataTable GetInActiveProductsCSV()
        {
            string query;

            query = "select category.description as category, product.*,coalesce(cost,0)*coalesce(qoh,0) as itemvalue, location.description as locationname   from  product left join cat2prod on product.id = cat2prod.prodid left join category on cat2prod.catid = category.id left outer join location on product.locationid = location.id  where type='product' and cat2prod.prodid is null order by category.description, product.modelnumber ";


            return m_dbconnect.GetData(query, "Product");
        }


        public DataTable GetAllProducts()
        {
            string query;

             query = "select 'all items' as category, product.*,coalesce(cost,0)*coalesce(qoh,0) as itemvalue, location.description as locationname   from  product left outer join location on product.locationid = location.id  where type='product' order by product.modelnumber ";


            return m_dbconnect.GetData(query, "Product");
        }


        public DataTable GetAllServices()
        {
            string query;

            query = "select 'all items' as category, product.*, location.description as locationname  from  product left outer join location on product.locationid = location.id  where type='service' order by product.description ";


            return m_dbconnect.GetData(query, "Service");
        }

        public DataTable GetAllShipping()
        {
            string query;

            query = "select 'all items' as category, product.*, location.description as locationname  from  product left outer join location on product.locationid = location.id  where type='shipping' order by product.description";


            return m_dbconnect.GetData(query, "Service");
        }

        public DataTable GetProductsByTypeDescription(string type, string description)
        {
            string query;

            query = "select product.*,coalesce(cost,0)*coalesce(qoh,0) as itemvalue, location.description as locationname   from  product left outer join location on product.locationid = location.id  where type='" + type + "' and (product.description like '%" + description.Trim() + "%' or modelnumber like '%" + description.Trim() + "%') order by  product.modelnumber";

            return m_dbconnect.GetData(query, "Product");
        }


        public int GetProductCount(int catid)
        {
            string query;
            query = "select * from product inner join cat2prod on cat2prod.prodid = product.id where cat2prod.catid = " + catid;
            DataTable dat = m_dbconnect.GetData(query);
            if(dat != null)
            {
                return dat.Rows.Count;
            }else
            {
                query = "select * from category where parentid = " + catid;
                DataTable dat2 = m_dbconnect.GetData(query);
                if (dat2 != null) return dat2.Rows.Count;
                else return 0;
            }
        }

        public DataTable GetProductsByCat(int catid)
        {
            string query;

           // query = "select category.description as category, category.colorcode as catcolorcode, product.*,coalesce(cost,0)*coalesce(qoh,0) as itemvalue, location.description as locationname   from category inner join cat2prod on category.id = cat2prod.catid inner join product on cat2prod.prodid = product.id  left outer join location on product.locationid = location.id  where category.id =" + catid + " order by product.modelnumber ";

            query = "select * from CategoryProduct where catid =" + catid + " order by modelnumber ";

            if (catid == -1)
                query = "select * from UnCategorizedProduct";

     
            return m_dbconnect.GetData(query, "Product");
        }

        //Available products .. 
        public DataTable GetProductsNOTByCat(string type,int catid)
        {
            string query;

            if (type == "") query = "select product.*,coalesce(cost,0)*coalesce(qoh,0) as itemvalue, location.description as locationname   from product left outer join location on product.locationid = location.id  where product.id not in ( select prodid from cat2prod where catid=" + catid + ")  order by product.modelnumber";
            else query = "select product.*,coalesce(cost,0)*coalesce(qoh,0) as itemvalue, location.description as locationname  from product inner join location on product.locationid = location.id  where product.id not in ( select prodid from cat2prod where catid=" + catid + ") and type='" + type + "' order by product.modelnumber ";


            return m_dbconnect.GetData(query, "Product");
        }

        public DataTable GetProductByID(int productID)
        {
            string query;

            query = "select product.*,coalesce(cost,0)*coalesce(qoh,0) as itemvalue, location.description as locationname   from product left outer join location on product.locationid = location.id  where product.id=" + productID;


            return m_dbconnect.GetData(query, "Product");
        }


        public int AddNewProduct(string type)
        {
            string query;

            query = "insert into product (locationid, description,colorcode,type) values (1,'new item','AliceBlue','" + type + "')";
            m_dbconnect.Command(query);

            query = "select * from product where product.id = LAST_INSERT_ID()";
            DataTable dt =  m_dbconnect.GetData(query);

            return (int)dt.Rows[0]["id"];

        }

        public int CloneProduct(int id)
        {
            string query;



            query = "insert into product (locationid,description,price,surcharge,imagesrc,type,taxexempt,commissiontype,colorcode,reportcategory,cost,locationcode,commissionamt) " +
                " select  locationid,concat(description,'(clone)'),price,surcharge,imagesrc,type,taxexempt,commissiontype,colorcode,reportcategory,cost,locationcode,commissionamt from product where product.id=" + id;
            m_dbconnect.Command(query);

            query = "select * from product where product.id = LAST_INSERT_ID()";
            DataTable dt = m_dbconnect.GetData(query);

            return (int)dt.Rows[0]["id"];

        }
 
        public bool UpdateProductString(int id, string field, string fieldvalue)
        {
            string query;

            query = "update product set " + field + " ='" + fieldvalue + "' where product.id=" + id;
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

            query = "delete from product where product.id=" + productid;
             m_dbconnect.Command(query);

            query = "delete from cat2prod where prodid=" + productid;
            return m_dbconnect.Command(query);

        }

        public bool DBDeductInventory(int productid)
        {
            string query;

            query = "update product set qoh = qoh - 1  where product.id=" + productid;
            return m_dbconnect.Command(query);

        }

        public bool DBAddToInventory(int productid)
        {
            string query;

            query = "update product set qoh = qoh + 1  where product.id=" + productid;
            return m_dbconnect.Command(query);

        }
        public bool UpdateNumeric(int id, string field, int fieldvalue)
        {
            string query;

            query = "update product set " + field + " =" + fieldvalue + " where product.id=" + id;
            return m_dbconnect.Command(query);
        }

        public bool UpdateNumeric(int id, string field, decimal fieldvalue)
        {
            string query;

            query = "update product set " + field + " =" + fieldvalue + " where product.id=" + id;
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

        public DataTable GetCategorybyType(string cattype, int parentid)
        {
            string query;

            if (parentid > 0) cattype = "subcat";

            query = "select * from category where parentid = " + parentid + " and cattype='" + cattype + "' order by description ";

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

        public bool AddCategory(string type, string catname, Category current)
        {
            string query;
            int parentid = 0;

            if (current != null)
            {
                parentid = current.ID;
                type = "subcat";
            }

            query = "insert into category (cattype,description,colorcode, parentid) values ('" + type + "','" + catname + "','gray'," + parentid + ")";
            return m_dbconnect.Command(query);
        }


    
        public bool UpdateCategoryString(int id, string field, string fieldvalue)
        {
            string query;

            query = "update category set " + field + " ='" + fieldvalue + "' where id=" + id;
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
