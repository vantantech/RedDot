using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;

namespace RedDot
{
    public class InventoryModel:INPCBase
    {

        private DBProducts m_dbproducts;

        int rowcount = 0;
        int pagecount = 0;

        public InventoryModel()
        {
            m_dbproducts = new DBProducts();
           
        }

        public DataTable GetCategoryList(string cattype,int parentid)
        {
            // ObservableCollection<Category> data = new ObservableCollection<Category>();

            // Category category;

            DataTable data_category = m_dbproducts.GetCategorybyType(cattype,parentid);

            if(parentid ==0)
            {
                DataRow newrow = data_category.NewRow();
                newrow["id"] = 1000;
                newrow["colorcode"] = "Black";
                newrow["description"] = "All Inventory Items";
                newrow["cattype"] = cattype;

                data_category.Rows.InsertAt(newrow, data_category.Rows.Count);
            }


            return data_category;

            /*

            foreach (DataRow cat in data_category.Rows)
            {
               // category = new Category((int)cat["id"], cat["description"].ToString().Trim(), cat["imagesrc"].ToString(), cat["colorcode"].ToString(), cat["cattype"].ToString());
                category = new Category(cat);
                data.Add(category);
            }

            return data;*/
        }


        public DataTable GetCategorybyType(string cattype, int parentid)
        {
        

           return  m_dbproducts.GetCategorybyType(cattype, parentid);


        
        }

        public ObservableCollection<Category> FillCategorybyType(string cattype,int parentid)
        {
            ObservableCollection<Category> data = new ObservableCollection<Category>();

            Category category;

            DataTable data_category = m_dbproducts.GetCategorybyType(cattype,parentid);


            //all item takes too long to load .. no longer used
           /* if(parentid ==0)
            {
                DataRow newrow = data_category.NewRow();
                newrow["id"] = 1000;
                newrow["colorcode"] = "Black";
                newrow["description"] = "All Inventory Items";
                newrow["cattype"] = cattype;

                data_category.Rows.InsertAt(newrow, data_category.Rows.Count);
            }
            */


            foreach (DataRow cat in data_category.Rows)
            {
               // category = new Category((int)cat["id"], cat["description"].ToString().Trim(), cat["imagesrc"].ToString(), cat["colorcode"].ToString(), cat["cattype"].ToString());
                category = new Category(cat);
                data.Add(category);
            }

            return data;
        }


        public ObservableCollection<Product> FillProductbyCatID(int catid)
        {
            ObservableCollection<Product> products = new ObservableCollection<Product>();

            DataTable data_category;
            Product newitem;

            m_dbproducts = new DBProducts();

            data_category = m_dbproducts.GetProductsByCat(catid);
            foreach (DataRow row in data_category.Rows)
            {

                newitem = new Product(row);


                products.Add(newitem);

            }
            return products;

        }


        /// <summary>
        /// FindProducts : Search for products based on type and description
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        /// <returns>A collection of "product" object</returns>
  
   

        public Product GetProductbyID(int id)
        {
            DataTable dt = m_dbproducts.GetProductByID(id);
            if (dt.Rows.Count > 0)
            {
                Product prod = new Product(dt.Rows[0]);
                return prod;
            }
            else return null;


        }

        public DataTable GetCateogories(string cattype,int parentid)
        {
            return m_dbproducts.GetCategorybyType(cattype,parentid);
        }

        public DataTable GetProducts()
        {
            return m_dbproducts.GetProductsByType("product");
        }

        public DataTable GetAllProducts()
        {
            return m_dbproducts.GetAllProducts();

        }

        public DataTable GetLocations()
        {
            return m_dbproducts.GetLocations();

        }

        public DataTable GetAllServices()
        {
            return m_dbproducts.GetAllServices();

        }

        public DataTable GetAllShipping()
        {
            return m_dbproducts.GetAllShipping();

        }
        public DataTable GetProductsNOTbyCatID(string type, int catid)
        {
            return m_dbproducts.GetProductsNOTByCat(type, catid);

        }

        public int GetProductCount(int catid)
        {
            return m_dbproducts.GetProductCount(catid);
        }

        public DataTable GetProductsbyCatID(int catid)
        {
            return m_dbproducts.GetProductsByCat(catid);
        }

        public bool AddItemToCategory(int catid, int prodid)
        {
            return m_dbproducts.AddItemToCategory(catid, prodid);
        }

        public bool RemoveItemFromCategory(int catid, int prodid)
        {
            return m_dbproducts.RemoveItemFromCategory(catid, prodid);
        }


        public bool AddCategory(string type, string catname, Category current)
        {

            return m_dbproducts.AddCategory(type, catname, current);
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
        public Product AddNewProduct(string type)
        {

            int id = m_dbproducts.AddNewProduct(type);
            DataTable dt = m_dbproducts.GetProductByID(id);
            if (dt.Rows.Count > 0)
            {
                return new Product(dt.Rows[0]);

            }
            else return null;
        }

        public Product CloneProduct(int id)
        {
            int id2 = m_dbproducts.CloneProduct(id);
            DataTable dt = m_dbproducts.GetProductByID(id2);
            if (dt.Rows.Count > 0)
            {
                return new Product(dt.Rows[0]);

            }
            else return null;
        }

 



        public bool DeleteProduct(int productid)
        {
            return m_dbproducts.DeleteProduct(productid);

        }




        public void PrintAllInventory()
        {

            PrintDocument pdoc = new PrintDocument();

            rowcount = 0;
            pagecount = 0;
           // pdoc.PrintPage += (sender, e) => pdoc_PrintAllInventory( e);  //this method allows you to pass parameters
            pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintAllInventory);  //simple method


            if (GlobalSettings.Instance.LargeFormatPrinter == "")
            {
                MessageBox.Show("Large format printer name not set.");
                return;
            }

           // pdoc.DefaultPageSettings.Landscape = true;

            pdoc.PrinterSettings.PrinterName = GlobalSettings.Instance.LargeFormatPrinter;

            System.Windows.Forms.DialogResult result;
            PrintPreviewDialog previewDlg = new PrintPreviewDialog();
            previewDlg.Document = pdoc;
           
            //result = previewDlg.ShowDialog();

           // if (result == System.Windows.Forms.DialogResult.OK)
           // {
           //     pdoc.Print();
          //  }

            pdoc.Print();


        }
        public void pdoc_PrintAllInventory(object sender, PrintPageEventArgs e)
        {
            try
            {
                DataTable products = m_dbproducts.GetAllProducts();


                Graphics graphics = e.Graphics;
                Font font = new Font("Courier New", 8, System.Drawing.FontStyle.Bold);
                Font fontitalic = new Font("Courier New", 8, System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold);
                Font fontbold = new Font("Courier New", 10, System.Drawing.FontStyle.Bold);

                Font fonttitle = new Font("Courier New", 15, System.Drawing.FontStyle.Bold);

                // Create pen.
                Pen blackPen = new Pen(Color.Black, 2);
                Brush blackBrush = new SolidBrush(Color.Black);
                Location store = GlobalSettings.Instance.Shop;

                float fontHeight = font.GetHeight() + 4; //12
                float fontBoldHeight = fontbold.GetHeight() + 4; //15

                int startX = 20;
                int startY = 30;

                float YOffset = 0;
             
                string receiptstr;



                int printcount = 0;
              



                graphics.DrawString("Product Inventory List as of:" + DateTime.Now.ToShortDateString(), fonttitle, blackBrush, startX, startY + YOffset);

                YOffset = YOffset + fonttitle.GetHeight() * 2;

           
                 


                    //Title for ticket items
               
          
                    graphics.DrawString("QOH", fontbold, new SolidBrush(Color.Black), startX + 10, startY + YOffset);
                    graphics.DrawString("Model #", fontbold, new SolidBrush(Color.Black), startX + 100, startY + YOffset);
                    graphics.DrawString("Description", fontbold, new SolidBrush(Color.Black), startX + 300, startY + YOffset);


         

                    // line beneath title line
                    YOffset = YOffset + fontBoldHeight;
                    graphics.DrawLine(blackPen, startX, startY + YOffset, startX + 700, startY + YOffset);



                    for (int i = rowcount; i < products.Rows.Count;i++ )
                    {
                        DataRow row = products.Rows[i];
                        rowcount = rowcount + 1;
                        printcount = printcount + 1;
                        YOffset = YOffset + fontBoldHeight;


                        receiptstr =row["qoh"].ToString().PadLeft(5,' ') ;
                    
                        graphics.DrawString(receiptstr, fontbold, new SolidBrush(Color.Black), startX, startY + YOffset);

                        receiptstr = row["modelnumber"].ToString();
                        if (receiptstr.Length > 15) receiptstr = receiptstr.Substring(0, 15);
                        graphics.DrawString(receiptstr, fontbold, new SolidBrush(Color.Black), startX + 60, startY + YOffset);


                        receiptstr = row["description"].ToString().Replace((char)13, ' ').Replace((char)10, ' ');
                        if (receiptstr.Length > 70) receiptstr = receiptstr.Substring(0, 70);
                        graphics.DrawString(receiptstr, fontbold, new SolidBrush(Color.Black), startX + 200, startY + YOffset);


                        if (printcount > 45)
                        {
                            // line beneath title line
                            YOffset = YOffset + 2 * fontBoldHeight;
                            graphics.DrawLine(blackPen, startX, startY + YOffset, startX + 700, startY + YOffset);
                            pagecount = pagecount + 1;
                            graphics.DrawString("Page:" +pagecount.ToString(), fontbold, new SolidBrush(Color.Black), startX + 400, startY + YOffset);
                            e.HasMorePages = true;
                            return;
                        }

                    }

                    e.HasMorePages = false;

                    // line beneath title line
                    YOffset = YOffset + fontBoldHeight;
                    graphics.DrawLine(blackPen, startX, startY + YOffset, startX + 700, startY + YOffset);


      





            }
            catch (Exception ex)
            {

                MessageBox.Show("Print Inventory:" + ex.Message);
            }
        }

        public void ExportAllInventoryCSV(string filename)
        {

            DataTable dt = m_dbproducts.GetAllProductsCSV();
            CSVWriter.WriteDataTable(dt, filename, true);

        }

        public void ExportActiveInventoryCSV(string filename)
        {

            DataTable dt = m_dbproducts.GetActiveProductsCSV();
            CSVWriter.WriteDataTable(dt, filename, true);

        }

        public void ExportInActiveInventoryCSV(string filename)
        {

            DataTable dt = m_dbproducts.GetInActiveProductsCSV();
            CSVWriter.WriteDataTable(dt, filename, true);

        }

        public void ImportInventoryCSV(string filename)
        {

            DataTable dt = m_dbproducts.GetInActiveProductsCSV();
            CSVWriter.WriteDataTable(dt, filename, true);

        }
    }


}
