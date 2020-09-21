using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class ShippingVM : INPCBase
    {

        private InventoryModel m_inventorymodel;
        private ObservableCollection<Product> m_selected;
        private DataTable m_available;


        private Security m_security;

        private Category m_currentcategory;

        private Product m_currentproduct;




        private ObservableCollection<Category> m_categories;


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




        private Visibility m_visibleselected;

        private Visibility m_visiblenewproduct;



        public ShippingVM(Window parent, Security security)
        {


            m_security = security;

            TabClicked = new RelayCommand(ExecuteTabClicked, param => true);
            CategoryClicked = new RelayCommand(ExecuteCategoryClicked, param => true);




            AddToCategoryClicked = new RelayCommand(ExecuteAddToCategoryClicked, param => true);
            AddProductClicked = new RelayCommand(ExecuteAddProductClicked, param => this.CanAddProduct);
            AddNewProductClicked = new RelayCommand(ExecuteAddNewProductClicked, param => true);
            EditProductClicked = new RelayCommand(ExecuteEditProductClicked, param => this.CanEditProduct);
            CloneProductClicked = new RelayCommand(ExecuteCloneProductClicked, param => this.CanEditProduct);
            DeleteProductClicked = new RelayCommand(ExecuteDeleteProductClicked, param => this.CanDeleteProduct);
            ProductClicked = new RelayCommand(ExecuteProductClicked, param => true);

            // BackClicked = new RelayCommand(ExecuteBackClicked, param => true);
            RemoveItemFromCategoryClicked = new RelayCommand(ExecuteRemoveItemFromCategoryClicked, param => this.CanRemoveFromCategory);


            //Category functions
            AddCategoryClicked = new RelayCommand(ExecuteAddCategoryClicked, param => true);
            DeleteCategoryClicked = new RelayCommand(ExecuteDeleteCategoryClicked, param => this.CanDeleteCategory);
            EditCategoryClicked = new RelayCommand(ExecuteEditCategoryClicked, param => this.CanEditCategory);
            OpenCategoryClicked = new RelayCommand(ExecuteOpenCategoryClicked, param => this.CanOpenCategory);



            m_parent = parent;

            VisibleSelected = Visibility.Visible;
            VisibleNewProduct = Visibility.Collapsed;





            m_inventorymodel = new InventoryModel();
            LoadCategories("cat4",0);

            SelectedIndex = 0;


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


        public ObservableCollection<Product> Selected
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


        public Category CurrentCategory
        {
            get { return m_currentcategory; }
            set
            {
                m_currentcategory = value;
                NotifyPropertyChanged("CurrentCategory");
            }
        }






        public ObservableCollection<Category> Categories
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
                if (m_currentcategory == null) return false; else return true;

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
                    if (CurrentCategory == null) return true; else return false;
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
                    if (CurrentCategory != null) return true; else return false;
                }
            }

        }




        public bool CanDeleteCategory
        {
            get
            {
                if (m_currentcategory == null) return false;
                else
                {
                    if (Selected.Count == 0) return true; else return false;
                }

            }
        }
        public bool CanEditCategory
        {
            get { if (m_currentcategory == null) return false; else return true; }
        }
        public bool CanOpenCategory
        {
            get { if (Selected == null) return false; else return true; }
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
                    LoadCategories(obj_filter.ToString(),0);
                    Selected = null;
                    Available = null;
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
                        ExecuteEditProductClicked(obj_prodid);
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
                AvailableShipping dlg = new AvailableShipping(m_parent, this);
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
                        ProductAdmin prod = new ProductAdmin(m_parent, CurrentCategory, m_currentproduct);
                        prod.ShowDialog();
                    }
                    else
                    {
                        ProductDetail prod2 = new ProductDetail(m_parent, CurrentCategory, m_currentproduct);
                        prod2.ShowDialog();
                    }



                    //refresh list
                    if (CurrentCategory != null) LoadShipping(CurrentCategory.ID);
                    else LoadShipping(1000);

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

                    if (CurrentCategory != null) //cloning was done in the category page so move to category
                    {
                        m_inventorymodel.AddItemToCategory(CurrentCategory.ID, clone.ID);
                    }


                    //refresh list
                    if (CurrentCategory != null) LoadShipping(CurrentCategory.ID);
                    else LoadShipping(1000);

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
                    if (CurrentCategory != null) LoadShipping(CurrentCategory.ID);
                    else LoadShipping(1000);

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
                product = m_inventorymodel.AddNewProduct("shipping");
                product.CommissionType = "none";
                product.CommissionAmt = 0;
                product.ReportCategory = "Shipping";
                product.Save();


                if (m_security.HasAccess("ProductAdmin"))
                {
                    ProductAdmin prod = new ProductAdmin(m_parent, CurrentCategory, product);
                    prod.ShowDialog();
                }
                else
                {
                    ProductDetail pd = new ProductDetail(m_parent, CurrentCategory, product);
                    pd.ShowDialog();
                }
                //after product is added, if it was added from the available list, then add to category automatically

                if (CurrentCategory != null) //selected category so add item to category
                {
                    m_inventorymodel.AddItemToCategory(CurrentCategory.ID, product.ID);
                    LoadShipping(CurrentCategory.ID); //editing has been done .. refresh available and selected list
                }
                else
                {
                    LoadShipping(1000);
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
                if (obj_catid != null)
                {
                    int id = int.Parse(obj_catid.ToString());
                    //current category is set in loadproducts()
                    LoadShipping(id);

                    //if category is clicked second time,then edit
                    if (m_lastcatid == id)
                    {
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
                if (Selected != null)
                {
                    //open selected inventory page
                    SelectedShipping dlg = new SelectedShipping(m_parent, this);
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
                    m_inventorymodel.AddItemToCategory(CurrentCategory.ID, productid);
                    LoadShipping(CurrentCategory.ID);
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
                if (m_currentproduct != null && CurrentCategory.ID > 0)
                {


                    m_inventorymodel.RemoveItemFromCategory(CurrentCategory.ID, m_currentproduct.ID);

                    LoadShipping(CurrentCategory.ID);
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
                        int parentid = 0;
                        if (CurrentCategory != null) parentid = CurrentCategory.ID;
                        m_inventorymodel.AddCategory(m_filter, tp.ReturnText,CurrentCategory);
                        LoadCategories(m_filter,parentid);
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
                if (!m_security.HasAccess("ProductAdmin"))
                {
                    TouchMessageBox.Show("Access Denied.");
                    return;
                }
                Confirm conf = new Confirm("Delete Category:" + CurrentCategory.Description + "?");
                conf.ShowDialog();

                if (conf.Action.ToUpper() == "YES")
                {
                    int parentid = CurrentCategory.parentid;
                    m_inventorymodel.DeleteCategory(CurrentCategory.ID);
                    LoadCategories(m_filter,parentid);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteDeleteCategoryClicked:" + e.Message);

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

                CategoryDetail catdetail = new CategoryDetail(m_parent, CurrentCategory);
                catdetail.ShowDialog();

                //refresh categories list
                LoadCategories(m_filter,CurrentCategory.parentid);
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteDeleteCategoryClicked:" + e.Message);

            }

        }

        #endregion


        private void LoadCategories(string filter, int parentid)
        {

            Categories = m_inventorymodel.FillCategorybyType(filter,parentid);

            CurrentCategory = null;
            Selected = null;
            Available = null;
            m_filter = filter;
            m_lastcatid = 0;
        }

        private void LoadShipping(int id)
        {
            ObservableCollection<Product> selprod = new ObservableCollection<Product>();
            DataTable sel;

            if (id == 1000)
            {
                CurrentCategory = null;

                sel = m_inventorymodel.GetAllShipping();

            }
            else
            {
                CurrentCategory = new Category(id);
                sel = m_inventorymodel.GetProductsbyCatID(id);
            }


            foreach (DataRow row in sel.Rows)
            {
                Product prod = new Product(row);
                selprod.Add(prod);
            }

            Selected = selprod;

            m_currentproduct = null;

            Available = m_inventorymodel.GetProductsNOTbyCatID("shipping", id);
            m_lastprodid = 0;

        }






    }
}
