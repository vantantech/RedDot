using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;
using RedDot.DataManager;

namespace RedDot
{
    public class MenuSetupModel:INPCBase
    {

        private DBProducts m_dbproducts;
   
 

        public MenuSetupModel()
        {
            m_dbproducts = new DBProducts();
           
        }

        public DataTable GetCategories(int apptid)
        {
            return  m_dbproducts.GetCategories(apptid);
        }

        public DataTable GetCategoryList()
        {
            // ObservableCollection<Category> data = new ObservableCollection<Category>();

            // Category category;

            DataTable data_category = GetCategories(0);


            DataRow newrow = data_category.NewRow();
            newrow["id"] = 999;
            newrow["sortorder"] = 99;
            newrow["colorcode"] = "RoyalBlue";
            newrow["description"] = "Recent 20 Items";
         
            data_category.Rows.InsertAt(newrow, data_category.Rows.Count);

            newrow = data_category.NewRow();
            newrow["id"] = 1000;
            newrow["sortorder"] = 100;
            newrow["colorcode"] = "Black";
            newrow["description"] = "All Inventory Items";
       
            data_category.Rows.InsertAt(newrow, data_category.Rows.Count);


            return data_category;

       
        }
        public ObservableCollection<Category> FillCategorybyType()
        {
            ObservableCollection<Category> data = new ObservableCollection<Category>();

            Category category;

            DataTable data_category = m_dbproducts.GetCategories(0);

            DataRow newrow = data_category.NewRow();
            newrow["id"] = 1000;
            newrow["sortorder"] = 100;
            newrow["colorcode"] = "Black";
            newrow["description"] = "All Inventory Items";
           

            data_category.Rows.InsertAt(newrow, data_category.Rows.Count);

               

            foreach (DataRow cat in data_category.Rows)
            {
               // category = new Category((int)cat["id"], cat["description"].ToString().Trim(), cat["imagesrc"].ToString(), cat["colorcode"].ToString(), cat["cattype"].ToString());
                category = new Category(cat);
                data.Add(category);
            }

            return data;
        }





        /// <summary>
        /// FindProducts : Search for products based on type and description
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        /// <returns>A collection of "product" object</returns>



        public List<ListPair> GetReportCategories()
        {
            List<ListPair> list = new List<ListPair>();
            DataTable dt = m_dbproducts.GetReportRevenue();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ListPair() { Description = row["reportcategory"].ToString(), StrValue = row["reportcategory"].ToString() });
            }

            return list;
        }


        public DataTable GetCateogories()
        {
            return m_dbproducts.GetCategories(0);
        }

        public DataTable GetProducts()
        {
            return m_dbproducts.GetProductsByType("product");
        }



        public DataTable GetAllProducts()
        {
            return m_dbproducts.GetAllProducts(" order by SUBSTRING(product.menuprefix,1,1), cast(SUBSTRING(product.menuprefix,2) as unsigned), product.id");

        }
        public DataTable GetProductsNOTbyCatID(string type, int catid)
        {
            return m_dbproducts.GetProductsNOTByCat(type, catid, " order by SUBSTRING(product.menuprefix,1,1), cast(SUBSTRING(product.menuprefix,2) as unsigned), product.id");

        }

        public DataTable GetProductsbyCatID(int catid)
        {
            return m_dbproducts.GetProductsByCat(catid, " order by SUBSTRING(product.menuprefix,1,1), cast(SUBSTRING(product.menuprefix,2) as unsigned), product.id");
        }

        public bool AddItemToCategory(int catid, int prodid)
        {
            return m_dbproducts.AddItemToCategory(catid, prodid);
        }

        public bool RemoveItemFromCategory(int catid, int prodid)
        {
            return m_dbproducts.RemoveItemFromCategory(catid, prodid);
        }


        public bool AddCategory(string type, string catname)
        {

            return m_dbproducts.AddCategory(type, catname);
        }

        public bool DeleteCategory(int catid)
        {
            return m_dbproducts.DeleteCategory(catid);
        }



        public string GetCategoryName(int catid)
        {
            return m_dbproducts.GetCategoryName(catid);
        }

        public bool CopyColorToProducts(int catid, string colorcode)
        {

            return m_dbproducts.CopyColorToProducts(catid, colorcode);
        }
        public MenuItem AddNewProduct()
        {

            DataTable dt = m_dbproducts.AddNewProduct();
            if (dt.Rows.Count > 0)
            {
                return new MenuItem(dt.Rows[0],true);

            }
            else return null;
        }

        public MenuItem CloneProduct(int id)
        {
            DataTable dt = m_dbproducts.CloneProduct(id);
            if (dt.Rows.Count > 0)
            {
                return new MenuItem(dt.Rows[0],true);

            }
            else return null;
        }




   

        public bool DeleteProduct(int productid)
        {
            return m_dbproducts.DeleteProduct(productid);

        }

 
    



    }


}
