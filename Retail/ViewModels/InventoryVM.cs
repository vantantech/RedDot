using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Drawing.Printing;
using System.Drawing;





namespace RedDot
{
    public class InventoryVM : INPCBase
    {

        private InventoryModel m_inventorymodel;
      
        private DataTable m_selected;
        private DataTable m_available;

 
        private Security m_security;

        private Category m_parentcategory;
   

        private Product m_currentproduct;




        private DataTable m_categories;
  

        private string m_filter = "";
        private Window m_parent;
        private int m_selectedindex;
        private int m_lastcatid = 0;

        private int m_lastprodid = 0;
   

        public ICommand TabClicked { get; set; }

        //Products
        public ICommand AddProductClicked { get; set; }
        public ICommand AllProductClicked { get; set; }
        public ICommand AddNewProductClicked { get; set; }
        public ICommand EditProductClicked { get; set; }
        public ICommand CloneProductClicked { get; set; }
        public ICommand DeleteProductClicked { get; set; }
        public ICommand ProductClicked { get; set; }

        //  public ICommand BackClicked { get; set; }

        //Product Category
        public ICommand CategoryClicked { get; set; }


        public ICommand AddToCategoryClicked { get; set; }
        public ICommand RemoveItemFromCategoryClicked { get; set; }
        public ICommand AddCategoryClicked { get; set; }
        public ICommand DeleteCategoryClicked { get; set; }
        public ICommand EditCategoryClicked { get; set; }
        public ICommand OpenCategoryClicked { get; set; }

  

        public ICommand PrintAllClicked { get; set; }


        public ICommand ExportAllClicked { get; set; }
        public ICommand ExportActiveClicked { get; set; }
        public ICommand ExportInActiveClicked { get; set; }

        private Visibility m_visibleselected;

        private Visibility m_visiblenewproduct;

        private bool m_canaddcat = true;

        public InventoryVM(Window parent, int catid, Security security)
        {


            m_security                    = security;

            TabClicked                    = new RelayCommand(ExecuteTabClicked, param => true);
            CategoryClicked               = new RelayCommand(ExecuteCategoryClicked, param => true);
         


            AddToCategoryClicked          = new RelayCommand(ExecuteAddToCategoryClicked, param => true);
            AddProductClicked             = new RelayCommand(ExecuteAddProductClicked, param => this.CanAddProduct);
            AddNewProductClicked          = new RelayCommand(ExecuteAddNewProductClicked, param => true);
            EditProductClicked            = new RelayCommand(ExecuteEditProductClicked, param => this.CanEditProduct);
            CloneProductClicked           = new RelayCommand(ExecuteCloneProductClicked, param => this.CanEditProduct);
            DeleteProductClicked          = new RelayCommand(ExecuteDeleteProductClicked, param => this.CanDeleteProduct);
            ProductClicked                = new RelayCommand(ExecuteProductClicked, param => true);

            // BackClicked                = new RelayCommand(ExecuteBackClicked, param => true);


            RemoveItemFromCategoryClicked = new RelayCommand(ExecuteRemoveItemFromCategoryClicked, param => this.CanRemoveFromCategory);


            //Category functions
            AddCategoryClicked            = new RelayCommand(ExecuteAddCategoryClicked, param => this.m_canaddcat);
            DeleteCategoryClicked         = new RelayCommand(ExecuteDeleteCategoryClicked, param => this.CanDeleteCategory);
            EditCategoryClicked           = new RelayCommand(ExecuteEditCategoryClicked, param => this.CanEditCategory);
            OpenCategoryClicked           = new RelayCommand(ExecuteOpenCategoryClicked, param => this.CanOpenCategory);




            PrintAllClicked = new RelayCommand(ExecutePrintAllClicked, param => true);





            ExportAllClicked              = new RelayCommand(ExecuteExportAllClicked, param => true);
            ExportActiveClicked = new RelayCommand(ExecuteExportActiveClicked, param => true);
            ExportInActiveClicked = new RelayCommand(ExecuteExportInActiveClicked, param => true);


            m_parent                      = parent;

            VisibleSelected               = Visibility.Visible;
            VisibleNewProduct             = Visibility.Collapsed;





            m_inventorymodel              = new InventoryModel();
         
            if(catid > 0)
            {
                LoadCategories("subcat", catid);
          
            }

            if (catid == 0)
            {
                LoadCategories("cat1", 0);
                SelectedIndex = 0;
            }

            if (catid == -1) m_canaddcat = false;

            LoadProducts(catid);
     

        
        }


        //--------------------------------------------------------------Public Properties --------------------------------------

        public Visibility VisibleNewProduct
        {
            get { return m_visiblenewproduct; }
            set
            {
                m_visiblenewproduct = value;
                NotifyPropertyChanged("VisibleNewProduct");
            }
        }
        public Visibility VisibleSelected
        {
            get { return m_visibleselected; }
            set
            {
                m_visibleselected = value;
                NotifyPropertyChanged("VisibleSelected");
            }
        }


        public int SelectedIndex
        {
            get { return m_selectedindex; }
            set
            {

                m_selectedindex = value;
                NotifyPropertyChanged("SelectedIndex");


            }
        }


        public DataTable Selected
        {
            get { return m_selected; }
            set
            {
                m_selected = value;
                NotifyPropertyChanged("Selected");
            }
        }



        public DataTable Available
        {
            get { return m_available; }
            set
            {
                m_available = value;
                NotifyPropertyChanged("Available");
            }
        }

  
        public Category ParentCategory
        {
            get { return m_parentcategory; }
            set
            {
                m_parentcategory = value;
                NotifyPropertyChanged("CurrentCategory");
            }
        }

        private Category m_selectedcategory;
        public Category SelectedCategory
        {
            get { return m_selectedcategory; }
            set
            {
                m_selectedcategory = value;
                NotifyPropertyChanged("SelectedCategory");
            }
        }



        public DataTable Categories
        {
            get { return m_categories; }
            set
            {
                m_categories = value;
                NotifyPropertyChanged("Categories");
            }

        }



        //---------------------------------------- Can Clicked -------------------------

        #region CanClicked
        public bool CanAddProduct
        {
            get
            {
                if (m_parentcategory == null) return false; else return true;

            }

        }
        public bool CanEditProduct
        {
            get
            {
                if (m_currentproduct == null) return false; else return true;

            }

        }

        public bool CanDeleteProduct
        {
            get
            {
                if (m_currentproduct == null) return false;
                else
                {
                    if (ParentCategory == null) return true; else return false;
                }

            }

        }
        public bool CanRemoveFromCategory
        {
            get
            {
                if (m_currentproduct == null) return false;
                else
                {
                    if (ParentCategory != null) return true; else return false;
                }
            }

        }




        public bool CanDeleteCategory
        {
            get { if (m_selectedcategory == null) return false; else return true; }
        }


        public bool CanEditCategory
        {
            get { if (m_selectedcategory == null) return false; else return true; }
        }


        public bool CanOpenCategory
        {
            get { if (SelectedCategory == null) return false; else return true; }
        }


        #endregion


        //-------------------------------------------Button Commands ----------------------------------------------

        //========================================================  INVENTORY =================================================
        #region Inventory
        public void ExecuteTabClicked(object obj_filter)
        {


            try
            {
                if (obj_filter != null)
                {
                    if (obj_filter.ToString() == "uncat")
                    {
                        ExecuteOpenCategoryClicked(-1);
                        Selected = null;
                        Available = null;
                    }
                    else
                    {
                        LoadCategories(obj_filter.ToString(), 0);
                        Selected = null;
                        Available = null;
                    }
               
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteCategoryProductClicked:" + e.Message);

            }
        }


  

        public void ExecuteProductClicked(object obj_prodid)
        {
            try
            {
                if (obj_prodid != null)
                {

                    int id = int.Parse(obj_prodid.ToString());
                    //current product
                    m_currentproduct = new Product(id);


                    //if category is clicked second time,then edit
                    if (m_lastprodid == id)
                    {
                        //open selected inventory page
                       // SelectedInventory dlg = new SelectedInventory(m_parent, this);
                        //dlg.ShowDialog();
                        ExecuteEditProductClicked( obj_prodid);
                    }


                    m_lastprodid = id;


                }
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteCategoryProductClicked:" + e.Message);

            }
        }

        public void ExecuteAddProductClicked(object obj)
        {
            try
            {
                Available = m_inventorymodel.GetProductsNOTbyCatID("", ParentCategory.ID);
                AvailableInventory dlg = new AvailableInventory(m_parent, this);
                dlg.ShowDialog();


            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteAddProductClicked:" + e.Message);

            }
        }

        public void ExecuteEditProductClicked(object obj_productid)
        {
            try
            {


                if (m_currentproduct != null)
                {
                    if (m_security.HasAccess("ProductAdmin"))
                    {
                        ProductAdmin prod = new ProductAdmin(m_parent, ParentCategory, m_currentproduct);
                        prod.ShowDialog();
                    }
                    else
                    {
                        ProductDetail prod2 = new ProductDetail(m_parent, ParentCategory, m_currentproduct);
                        prod2.ShowDialog();
                    }

                     


                    //refresh list
                    if (ParentCategory != null) LoadProducts(ParentCategory.ID);
                    else LoadProducts(1000);

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Edit Product:" + e.Message);

            }
        }
        public void ExecuteCloneProductClicked(object obj_productid)
        {
            try
            {


                if (m_currentproduct != null)
                {

                    Product clone = m_inventorymodel.CloneProduct(m_currentproduct.ID);

                    if (ParentCategory != null) //cloning was done in the category page so move to category
                    {
                        m_inventorymodel.AddItemToCategory(ParentCategory.ID, clone.ID);
                    }


                    //refresh list
                    if (ParentCategory != null) LoadProducts(ParentCategory.ID);
                    else LoadProducts(1000);

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Clone Product:" + e.Message);

            }
        }


        public void ExecuteDeleteProductClicked(object obj_productid)
        {
            try
            {

                if (!m_security.HasAccess("ProductAdmin"))
                {
                    TouchMessageBox.Show("Access Denied.");
                    return;
                }

                if (m_currentproduct != null)
                {
                    Confirm conf = new Confirm("Delete Item Permanently?");
                    conf.ShowDialog();

                    if (conf.Action.ToUpper() == "YES")
                    {

                        m_inventorymodel.DeleteProduct(m_currentproduct.ID);
                    }


                    //refresh list
                    if (ParentCategory != null) LoadProducts(ParentCategory.ID);
                    else LoadProducts(-1);

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Clone Product:" + e.Message);

            }
        }
        public void ExecuteAddNewProductClicked(object obj_catid)
        {
            try
            {

                Product product;
                product = m_inventorymodel.AddNewProduct("product");
                product.CommissionType = "percent";
                product.CommissionAmt = 3.0m;
                product.ReportCategory = "Product";
                product.Save();

                if (m_security.HasAccess("ProductAdmin"))
                {
                    ProductAdmin prod = new ProductAdmin(m_parent, ParentCategory, product);
                    prod.ShowDialog();
                }
                else
                {
                    ProductDetail pd = new ProductDetail(m_parent, ParentCategory, product);
                    pd.ShowDialog();
                }

                //after product is added, if it was added from the available list, then add to category automatically

                if (ParentCategory != null) //selected category so add item to category
                {
                    m_inventorymodel.AddItemToCategory(ParentCategory.ID, product.ID);
                    LoadProducts(ParentCategory.ID); //editing has been done .. refresh available and selected list
                }
                else
                {
                    LoadProducts(1000);
                }






            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteAddProductClicked:" + e.Message);

            }
        }
        #endregion
   //=============================================================  CATEGORY    =======================================================
        #region Category
        public void ExecuteCategoryClicked(object obj_catid)
        {
            try
            {

                m_currentproduct = null;

                if (obj_catid != null)
                {
                    int id = int.Parse(obj_catid.ToString());
                    //current category is set in loadproducts()
                    SelectedCategory = new Category(id);
                   

                    //if category is clicked second time,then edit
                    if (m_lastcatid == id)
                    {
                  
                        m_lastcatid = 0;
                        //open selected inventory page
                        ExecuteOpenCategoryClicked(obj_catid);
                    }

                    m_lastcatid = id;


                }
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteCategoryProductClicked:" + e.Message);

            }
        }


        public void ExecuteOpenCategoryClicked(object obj_catid)
        {
            try
            {
                if ( obj_catid != null)
                {

                    int id = int.Parse(obj_catid.ToString());
                  
                    

            

                    //open selected inventory page
                    SelectedInventory dlg = new SelectedInventory(id,m_security);
                        dlg.ShowDialog();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("OPen category" + e.Message);

            }
        }


        public void ExecuteAddToCategoryClicked(object obj_productid)
        {
            try
            {

                int productid = 0;
                if (obj_productid != null) productid = (int)obj_productid;
                if (productid > 0)
                {
                 
                    m_inventorymodel.AddItemToCategory(ParentCategory.ID, productid);
                    LoadProducts(ParentCategory.ID);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Add to Category:" + e.Message);

            }
        }
        public void ExecuteRemoveItemFromCategoryClicked(object obj_productid)
        {
            try
            {
                if (!m_security.HasAccess("ProductAdmin"))
                {
                    TouchMessageBox.Show("Access Denied.");
                    return;
                }
                if (m_currentproduct != null && ParentCategory.ID > 0)
                {


                    m_inventorymodel.RemoveItemFromCategory(ParentCategory.ID, m_currentproduct.ID);

                    LoadProducts(ParentCategory.ID);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Execute Remove item from Category :" + e.Message);

            }
        }
        public void ExecuteAddCategoryClicked(object obj_catid)
        {
            try
            {
                TextPad tp = new TextPad("Add Category:", "");
                Utility.OpenModal(m_parent, tp);
                if (tp.ReturnText != null)
                    if (tp.ReturnText != "")
                    {
               
                        m_inventorymodel.AddCategory(m_filter, tp.ReturnText, ParentCategory);
                        LoadCategories(m_filter,ParentCategory.ID);
                    }
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteAddCategoryClicked:" + e.Message);

            }
        }




        public void ExecuteDeleteCategoryClicked(object obj_id)
        {
            try
            {

                //check to see if cat is empty
                int cnt = m_inventorymodel.GetProductCount(SelectedCategory.ID);
                if(cnt > 0)
                {
                    TouchMessageBox.Show("Category is not empty.");
                    return;
                }
            
                if (!m_security.HasAccess("ProductAdmin"))
                {
                    TouchMessageBox.Show("Access Denied.");
                    return;
                }
                Confirm conf = new Confirm("Delete Category:" + SelectedCategory.Description + "?");
                conf.ShowDialog();

                if (conf.Action.ToUpper() == "YES")
                {
                 
                    m_inventorymodel.DeleteCategory(SelectedCategory.ID);
                   

                        if(ParentCategory != null)                 
                            LoadCategories(m_filter,ParentCategory.ID); // in subcategory
                        else
                            LoadCategories(m_filter,0); //root
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Execute Delete Category Clicked:" + e.Message);

            }

        }

        public void ExecuteEditCategoryClicked(object obj_id)
        {
            try
            {
                if (!m_security.HasAccess("ProductAdmin"))
                {
                    TouchMessageBox.Show("Access Denied.");
                    return;
                }
                CategoryDetail catdetail = new CategoryDetail(m_parent, SelectedCategory);
                catdetail.ShowDialog();

                //refresh categories list
                if(ParentCategory != null)
                    LoadCategories(m_filter,ParentCategory.ID);
                else
                    LoadCategories(m_filter, 0);
            }
            catch (Exception e)
            {
                MessageBox.Show("Execute Edit Category Clicked:" + e.Message);

            }

        }



        public void ExecutePrintAllClicked(object obj_id)
        {
            try
            {
                m_inventorymodel.PrintAllInventory();
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteDeleteCategoryClicked:" + e.Message);

            }

        }

        public void ExecuteExportAllClicked(object obj_id)
        {
            try
            {
                System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog();
                ofd.DefaultExt = "csv";
                ofd.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                   
                    m_inventorymodel.ExportAllInventoryCSV(ofd.FileName);
                }

               

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteExportClicked:" + e.Message);

            }

        }

        public void ExecuteExportActiveClicked(object obj_id)
        {
            try
            {
                System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog();
                ofd.DefaultExt = "csv";
                ofd.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    m_inventorymodel.ExportActiveInventoryCSV(ofd.FileName);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteExportClicked:" + e.Message);

            }

        }

        public void ExecuteExportInActiveClicked(object obj_id)
        {
            try
            {
                System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog();
                ofd.DefaultExt = "csv";
                ofd.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    m_inventorymodel.ExportInActiveInventoryCSV(ofd.FileName);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteExportClicked:" + e.Message);

            }

        }
        #endregion


        private void LoadCategories(string filter, int catid)
        {

            Categories = m_inventorymodel.GetCategorybyType(filter,catid);

       
            m_filter = filter;
            m_lastcatid = 0;
        }

        private void LoadProducts(int catid)
        {

            if (catid == 0) return;


          //  ObservableCollection<Product> selprod = new ObservableCollection<Product>();

           if(catid >= 0)
           ParentCategory = new Category(catid);


            DataTable sel = m_inventorymodel.GetProductsbyCatID(catid);

            /*
            foreach (DataRow row in sel.Rows)
            {
                Product prod = new Product(row);
                selprod.Add(prod);
            }

            Selected = selprod;

    */

            Selected = sel;

            m_currentproduct = null;

           // Available = m_inventorymodel.GetProductsNOTbyCatID("", catid);
            m_lastprodid = 0;

        }



 


    }
}
