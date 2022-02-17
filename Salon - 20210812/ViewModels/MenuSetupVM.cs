using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class MenuSetupVM : INPCBase
    {

        private MenuSetupModel m_menusetupmodel;
        private DataTable m_selected;
        private DataTable m_available;
         private SecurityModel m_security;

        private Category m_currentcategory;
 
   
       // private MenuItem m_currentproduct;
 



        private ObservableCollection<Category> m_categories;

       // private ObservableCollection<Modifier> m_modifiers;

        private string m_filter = "";
        private Window m_parent;
        private int m_selectedindex;
     
        private int m_productwidth;
        private int m_productheight;
        private int m_productfontsize;

        private int m_largest = 0;

        public ICommand TabClicked { get; set; }

        //Products
        public ICommand AddProductClicked { get; set; }
        public ICommand AllProductClicked { get; set; }
        public ICommand AddNewProductClicked { get; set; }
     
        public ICommand CloneProductClicked { get; set; }
     
        public ICommand ProductClicked { get; set; }

        public ICommand PictureClicked { get; set; }

      

        //  public ICommand BackClicked { get; set; }

        //Product Category
        public ICommand CategoryClicked { get; set; }
        public ICommand AddToCategoryClicked { get; set; }
        public ICommand RemoveItemFromCategoryClicked { get; set; }
        public ICommand AddCategoryClicked { get; set; }
        public ICommand DeleteCategoryClicked { get; set; }
        public ICommand CopyColorToAllClicked { get; set; }

       



        private Visibility m_visibleselected;

        private Visibility m_visiblenewproduct;

        private Visibility m_visibleremove;



        public MenuSetupVM(Window parent, SecurityModel security)
        {


            m_security = security;

            TabClicked = new RelayCommand(ExecuteTabClicked, param => true);
            CategoryClicked = new RelayCommand(ExecuteCategoryClicked, param => true);


            CopyColorToAllClicked = new RelayCommand(ExecuteCopyColorToAllClicked, param => this.CanCopy);

            AddToCategoryClicked = new RelayCommand(ExecuteAddToCategoryClicked, param => true);
            AddProductClicked = new RelayCommand(ExecuteAddProductClicked, param => this.CanAddProduct);
            AddNewProductClicked = new RelayCommand(ExecuteAddNewProductClicked, param => true);
        
       
            ProductClicked = new RelayCommand(ExecuteProductClicked, param => true);

            // BackClicked = new RelayCommand(ExecuteBackClicked, param => true);
            RemoveItemFromCategoryClicked = new RelayCommand(ExecuteRemoveItemFromCategoryClicked, param => this.CanRemoveFromCategory);


            //Category functions
            AddCategoryClicked = new RelayCommand(ExecuteAddCategoryClicked, param => true);
            DeleteCategoryClicked = new RelayCommand(ExecuteDeleteCategoryClicked, param => this.CanDeleteCategory);

            PictureClicked = new RelayCommand(ExecutePictureClicked, param => true);

      

            m_parent = parent;

            VisibleSelected = Visibility.Visible;
            VisibleNewProduct = Visibility.Collapsed;

            VisibleRemove = Visibility.Collapsed;



            m_menusetupmodel = new MenuSetupModel();
            LoadCategories();

            SelectedIndex = 0;

            m_productwidth = GlobalSettings.Instance.ProductWidth - 50;  //minus 50 for the Prefix box width which is always 50 constant
            m_productheight = GlobalSettings.Instance.ProductHeight;

            m_productfontsize = GlobalSettings.Instance.ProductFontSize;
        }


        //--------------------------------------------------------------Public Properties --------------------------------------
        public int ProductWidth
        {
            get { return m_productwidth; }
        }


        public int ProductBorderWidth2
        {
            get { return m_productwidth - 50; }
        }
        public int ProductBorderWidth
        {
            get { return m_productwidth + 70; }
        }

        public int ProductHeight
        {
            get { return m_productheight; }
        }


        public int DescriptionHeight
        {
            get { return m_productheight - 25; }
        }
        public int ProductFontSize
        {
            get { return m_productfontsize; }
        }
   

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


        public Visibility VisibleRemove
        {
            get { return m_visibleremove; }
            set
            {
                m_visibleremove = value;
                NotifyPropertyChanged("VisibleRemove");
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

        public bool CanCopy
        {
            get
            {
                if (m_currentcategory == null) return false;
                if (m_currentcategory.CategoryColorCode == null) return false;
                else return true;
            }
        }


  

  
 


 


        public bool CanDeleteCategory
        {
            get
            {
                if (m_currentcategory == null) return false;
                else
                {
                    if (Selected.Rows.Count == 0) return true; else return false;
                }

            }
        }
        public bool CanEditCategory
        {
            get { if (m_currentcategory == null) return false; else return true; }
        }

        public bool CanRemoveFromCategory
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
                    LoadCategories();
                    Selected = null;
                    Available = null;
                }

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteCategoryProductClicked:" + e.Message);

            }
        }

        public void ExecuteCopyColorToAllClicked(object obj)
        {
            try
            {
                m_menusetupmodel.CopyColorToProducts(CurrentCategory.ID, CurrentCategory.CategoryColorCode);
                Selected = m_menusetupmodel.GetProductsbyCatID(CurrentCategory.ID);
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Copy color to all Clicked:" + e.Message);

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

                    MenuItem menuitem = new MenuItem(id, true);


                        ProductDetail prod = new ProductDetail(m_parent, CurrentCategory, menuitem);
                    Utility.OpenModal(m_parent, prod);
                 


                        //refresh list
                        if (CurrentCategory != null) LoadProducts(CurrentCategory.ID);
                        else LoadProducts(1000);


                }
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteProductClicked:" + e.Message);

            }
        }

        public void ExecuteAddProductClicked(object obj)
        {
            try
            {
                AvailableInventory dlg = new AvailableInventory(m_parent, this);
                Utility.OpenModal(m_parent, dlg);
             


            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteAddProductClicked:" + e.Message);

            }
        }

 
        /*
        public void ExecuteCloneProductClicked(object obj_productid)
        {
            try
            {


                if (m_currentproduct != null)
                {

                    MenuItem clone = m_menusetupmodel.CloneProduct(m_currentproduct.ID);

                    if (CurrentCategory != null) //cloning was done in the category page so move to category
                    {
                        m_menusetupmodel.AddItemToCategory(CurrentCategory.ID, clone.ID);
                    }


                    //refresh list
                    if (CurrentCategory != null) LoadProducts(CurrentCategory.ID);
                    else LoadProducts(1000);

                }
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Clone Product:" + e.Message);

            }
        }
        */

  
        public void ExecuteAddNewProductClicked(object obj_catid)
        {
            try
            {

                MenuItem product;
                product = m_menusetupmodel.AddNewProduct();
                product.ColorCode = CurrentCategory.CategoryColorCode;
                product.MenuPrefix = CurrentCategory.LetterCode + (m_largest + 1).ToString();
                ProductDetail pd = new ProductDetail(m_parent, CurrentCategory, product);
                Utility.OpenModal(m_parent, pd);
           

                //after product is added, if it was added from the available list, then add to category automatically

                if (CurrentCategory != null) //selected category so add item to category
                {
                    m_menusetupmodel.AddItemToCategory(CurrentCategory.ID, product.ID);
                    LoadProducts(CurrentCategory.ID); //editing has been done .. refresh available and selected list
                }
                else
                {
                    LoadProducts(1000);
                }






            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteAddProductClicked:" + e.Message);

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
                    LoadProducts(id);

             
                }
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteCategoryProductClicked:" + e.Message);

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
                    m_menusetupmodel.AddItemToCategory(CurrentCategory.ID, productid);
                    LoadProducts(CurrentCategory.ID);
                }
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Add to Category:" + e.Message);

            }
        }
        public void ExecuteRemoveItemFromCategoryClicked(object obj_prodid)
        {
            try
            {
                if (obj_prodid != null)
                {

                    int id = int.Parse(obj_prodid.ToString());
                    m_menusetupmodel.RemoveItemFromCategory(CurrentCategory.ID, id);

                    LoadProducts(CurrentCategory.ID);
                }
               
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Execute Remove item from Category :" + e.Message);

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

                        m_menusetupmodel.AddCategory(m_filter, tp.ReturnText);
                        CurrentCategory = null;
                        LoadCategories();
                        
                    }
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteAddCategoryClicked:" + e.Message);

            }
        }

        public void ExecuteDeleteCategoryClicked(object obj_id)
        {
            try
            {
                Confirm conf = new Confirm("Delete Category:" + CurrentCategory.Description + "?");
                Utility.OpenModal(m_parent, conf);
               

                if (conf.Action.ToUpper() == "YES")
                {
                    m_menusetupmodel.DeleteCategory(CurrentCategory.ID);
                    CurrentCategory = null;
                    LoadCategories();
                }

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteDeleteCategoryClicked:" + e.Message);

            }

        }


        public void ExecutePictureClicked(object button)
        {
            OpenFileDialog picfile = new OpenFileDialog();
            picfile.DefaultExt = "*.*";
            picfile.Filter = "All files (*.*)|*.*|PNG Files (*.png)|*.png|JPG files (*.jpg)|*.jpg|BMP files (*.bmp)|*.bmp";
            string AppPath;
            string selectedPath;

            AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            AppPath = AppPath.ToUpper();

            if (picfile.ShowDialog() == true)
            {
                selectedPath = picfile.FileName;
                selectedPath = selectedPath.ToUpper();
                selectedPath = selectedPath.Replace(AppPath, "");

                CurrentCategory.ImageSrc = selectedPath.Replace("\\", "\\\\");
            }
            // Utility.PlaySound();

        }

 
    

        #endregion


        //========================================================    FUNCTIONS  ======================================================

        private void LoadCategories()
        {

            Categories = m_menusetupmodel.FillCategorybyType();

      
                Selected = null;
                Available = null;
  
       
   
        }

        private void LoadProducts(int id)
        {

            if (id == 1000)
            {
                VisibleRemove = Visibility.Collapsed;
                CurrentCategory = null;
                Selected = m_menusetupmodel.GetAllProducts();
            }
            else
            {
                VisibleRemove = Visibility.Visible;
                CurrentCategory = new Category(id);
                Selected = m_menusetupmodel.GetProductsbyCatID(id);
            }


            m_largest = 1;
            foreach(DataRow prod in Selected.Rows)
            {
                string test = Regex.Match(prod["menuprefix"].ToString(), @"\d+").Value;
                if(test != "")
                {
                    int val = int.Parse(test);

                    if (val > m_largest) m_largest = val;
                }
              

            }

            Available = m_menusetupmodel.GetProductsNOTbyCatID("", id);
          

        }


       




    }
}
