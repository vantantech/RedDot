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
    public class MenuSetupVM : INPCBase
    {

        private MenuSetupModel m_inventorymodel;
        private QSReports m_reports = new QSReports();



        private DataTable m_selected;
        private DataTable m_available;
        private DataTable m_availableproduct;

        private DataTable m_availablemodgroups;
        private SecurityModel m_security;
        

        private Category m_currentcategory;
        private ModGroup m_currentmodgroup;
        private ModProfile m_currentprofile;
        private Product m_currentproduct;
     
        private Modifier m_currentmodifier;
   


        private ObservableCollection<Category> m_categories;
        private ObservableCollection<ModProfile> m_modprofiles;
        private ObservableCollection<ModGroup> m_modgroups;


        private ObservableCollection<ComboGroup> m_combogroups;
        private ObservableCollection<ComboSet> m_combosets;

        private ComboGroup m_currentcombogroup;
        private ComboSet m_currentcomboset;

        // private ObservableCollection<Modifier> m_modifiers;

        private string m_filter = "";
        private Window m_parent;
        public Window PopUp { get; set; }
        private int m_selectedindex;
        private int m_lastcatid = 0;
        private int m_lastprodid = 0;
        private int m_lastmodgroupid = 0;
    

        //for removing and adding modifier to modgroup
        int modifierclickcount = 0;
        int lastmodifierid = 0;


        private int m_lastcombosetid = 0;


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

        public ICommand OpenCategoryClicked { get; set; }
        public ICommand CopyColorToAllClicked { get; set; }
        public ICommand PictureClicked { get; set; }
        public ICommand ClearImageClicked { get; set; }
        public ICommand CatMoveUpClicked { get; set; }
        public ICommand CatMoveDownClicked { get; set; }

        //Modifier Profiles
        public ICommand ProfileClicked { get; set; }
        public ICommand AddProfileClicked { get; set; }
        public ICommand EditProfileClicked { get; set; }
        public ICommand DeleteProfileClicked { get; set; }

        //Modifier Groups
        public ICommand ModGroupClicked { get; set; }
        public ICommand OpenModGroupClicked { get; set; }
        public ICommand AddModGroupClicked { get; set; }
        public ICommand AddNewModGroupClicked { get; set; }
        public ICommand AddModGroupToProfileClicked { get; set; }
        public ICommand EditModGroupClicked { get; set; }
       // public ICommand ModGroupDescriptionClicked { get; set; }
        public ICommand DeleteModGroupClicked { get; set; }
        public ICommand RemoveModGroupClicked { get; set; }



        //Modifiers
        public ICommand ModifierClicked { get; set; }
        public ICommand AddNewModifierClicked { get; set; }
  
        public ICommand AddModifierToModGroupClicked { get; set; }
        public ICommand AddNewModifierToModGroupClicked { get; set; }
        public ICommand EditModifierClicked { get; set; }
        public ICommand DeleteModifierClicked { get; set; }
        public ICommand RemoveModifierClicked { get; set; }
        public ICommand CloneModifierClicked { get; set; }


        //Combo Groups

        public ICommand ComboGroupClicked { get; set; }
        public ICommand AddComboGroupClicked { get; set; }
        public ICommand EditComboGroupClicked { get; set; }
        public ICommand DeleteComboGroupClicked { get; set; }


        //Combo Set
        public ICommand ComboSetClicked { get; set; }
        public ICommand OpenComboSetClicked { get; set; }
        public ICommand AddComboSetClicked { get; set; }
        public ICommand AddNewComboSetClicked { get; set; }
         public ICommand EditComboSetClicked { get; set; }
         public ICommand DeleteComboSetClicked { get; set; }
        public ICommand RemoveComboSetClicked { get; set; }

        public ICommand AddComboSetToComboGroupClicked { get; set; }


        public ICommand RemoveFromComboSetClicked { get; set; }

        public ICommand AddToComboSetClicked { get; set; }  //adds actual item to comboset


        private Visibility m_visibleselected;

        private Visibility m_visiblenewproduct;



        public MenuSetupVM(Window parent, SecurityModel security)
        {


            m_security = security;
            m_parent = parent;




            TabClicked = new RelayCommand(ExecuteTabClicked, param => true);
        
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
            CategoryClicked = new RelayCommand(ExecuteCategoryClicked, param => true);
            AddCategoryClicked = new RelayCommand(ExecuteAddCategoryClicked, param => true);
            DeleteCategoryClicked = new RelayCommand(ExecuteDeleteCategoryClicked, param => this.CanDeleteCategory);

            OpenCategoryClicked = new RelayCommand(ExecuteOpenCategoryClicked, param => this.CanOpenCategory);
            CopyColorToAllClicked = new RelayCommand(ExecuteCopyColorToAllClicked, param => this.CanCopyCategoryColorCode);
            PictureClicked = new RelayCommand(ExecutePictureClicked, param => this.CanEditCategory);
            ClearImageClicked = new RelayCommand(ExecuteClearImageClicked, param => this.CanEditCategory);
            CatMoveUpClicked = new RelayCommand(ExecuteCatMoveUpClicked, param => this.CanEditCategory);
            CatMoveDownClicked = new RelayCommand(ExecuteCatMoveDownClicked, param => this.CanEditCategory);



            //Profile functions
            ProfileClicked = new RelayCommand(ExecuteProfileClicked, param => true);
            AddProfileClicked = new RelayCommand(ExecuteAddProfileClicked, param => true);
            DeleteProfileClicked = new RelayCommand(ExecuteDeleteProfileClicked, param => this.CanDeleteProfile);
            EditProfileClicked = new RelayCommand(ExecuteEditProfileClicked, param => this.CanEditProfile);


            //Modifier Group functions
            ModGroupClicked = new RelayCommand(ExecuteModGroupClicked, param => true);
            AddModGroupClicked = new RelayCommand(ExecuteAddModGroupClicked, param => this.CanAddModGroup);
            OpenModGroupClicked = new RelayCommand(ExecuteOpenModGroupClicked, param => this.CanOpenModGroup);
            AddModGroupToProfileClicked = new RelayCommand(ExecuteAddModGroupToProfileClicked, param => true);
            AddNewModGroupClicked = new RelayCommand(ExecuteAddNewModGroupClicked, param => true);
            DeleteModGroupClicked = new RelayCommand(ExecuteDeleteModGroupClicked, param => this.CanDeleteModGroup);
            RemoveModGroupClicked = new RelayCommand(ExecuteRemoveModGroupClicked, param => this.CanRemoveModGroup);
            EditModGroupClicked = new RelayCommand(ExecuteEditModGroupClicked, param => this.CanEditModGroup);
           // ModGroupDescriptionClicked = new RelayCommand(ExecuteModGroupDescriptionClicked, param => this.CanEditModGroup);

            //Modifier
            ModifierClicked = new RelayCommand(ExecuteModifierClicked, param => true);
            AddNewModifierClicked = new RelayCommand(ExecuteAddNewModifierClicked, param => true);
            AddNewModifierToModGroupClicked = new RelayCommand(ExecuteAddNewModifierToModGroupClicked, param => true);
            AddModifierToModGroupClicked = new RelayCommand(ExecuteAddModifierToModGroupClicked, param => true);
            RemoveModifierClicked = new RelayCommand(ExecuteRemoveModifierFromModGroupClicked, param => true);


            EditModifierClicked = new RelayCommand(ExecuteEditModifierClicked, param => this.CanEditModifier);
            DeleteModifierClicked = new RelayCommand(ExecuteDeleteModifierClicked, param => this.CanEditModifier);
            CloneModifierClicked = new RelayCommand(ExecuteCloneModifierClicked, param => this.CanEditModifier);


            //ComboGroup functions
            ComboGroupClicked = new RelayCommand(ExecuteComboGroupClicked, param => true);
            AddComboGroupClicked = new RelayCommand(ExecuteAddComboGroupClicked, param => true);
            DeleteComboGroupClicked = new RelayCommand(ExecuteDeleteComboGroupClicked, param => this.CanDeleteComboGroup);
            EditComboGroupClicked = new RelayCommand(ExecuteEditComboGroupClicked, param => this.CanEditComboGroup);

            //ComboSet functions
            ComboSetClicked = new RelayCommand(ExecuteComboSetClicked, param => true);
            AddComboSetClicked = new RelayCommand(ExecuteAddComboSetClicked, param => this.CanAddComboSet);
            OpenComboSetClicked = new RelayCommand(ExecuteOpenComboSetClicked, param => this.CanOpenComboSet);
            AddNewComboSetClicked = new RelayCommand(ExecuteAddNewComboSetClicked, param => true);
            DeleteComboSetClicked = new RelayCommand(ExecuteDeleteComboSetClicked, param => this.CanDeleteComboSet);
            RemoveComboSetClicked = new RelayCommand(ExecuteRemoveComboSetClicked, param => this.CanRemoveComboSet);
            EditComboSetClicked = new RelayCommand(ExecuteEditComboSetClicked, param => this.CanEditComboSet);
            AddComboSetToComboGroupClicked = new RelayCommand(ExecuteAddComboSetToComboGroupClicked, param => true);

       
         
            RemoveFromComboSetClicked = new RelayCommand(ExecuteRemoveFromComboSetClicked, param => true);
            AddToComboSetClicked = new RelayCommand(ExecuteAddToComboSetClicked, param => true);









        

            VisibleSelected = Visibility.Visible;
            VisibleNewProduct = Visibility.Collapsed;
            m_inventorymodel = new MenuSetupModel();


            ColorCodes = new List<ListPair>();
            ColorCodes.Add(new ListPair() { Description = "Black", StrValue = "Black" });
            ColorCodes.Add(new ListPair() { Description = "Red", StrValue = "Red" });



            CategoryWidth = GlobalSettings.Instance.CategoryWidth;
            CategoryHeight = GlobalSettings.Instance.CategoryHeight;
            CategoryFontSize = GlobalSettings.Instance.CategoryFontSize;

            ProductWidth = GlobalSettings.Instance.ProductWidth;
            ProductHeight = GlobalSettings.Instance.ProductHeight;
            ProductFontSize = GlobalSettings.Instance.ProductFontSize;






            LoadCategories("menu1");

            SelectedIndex = 0;

            LoadModifierProfiles();

            LoadComboGroups();

          

 

        }


        //--------------------------------------------------------------Public Properties --------------------------------------

        public int CategoryWidth { get; set; }


        public int CategoryHeight { get; set; }


        public int ProductWidth { get; set; }


        public int ProductHeight { get; set; }


        public int ProductFontSize { get; set; }

        public int CategoryFontSize { get; set; }


        public List<ListPair>   ColorCodes { get; set; }

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

                switch(m_selectedindex)
                {
                    case 3:
                        AllModifiers = m_inventorymodel.GetAllModifiers();
                        CurrentModifier = null;
                        break;
                }

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

        public ObservableCollection<ModProfile> ModProfiles
        {
            get { return m_modprofiles; }
            set
            {
                m_modprofiles = value;
                NotifyPropertyChanged("ModProfiles");
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

        public DataTable AvailableModGroups
        {
            get { return m_availablemodgroups; }
            set
            {
                m_availablemodgroups = value;
                NotifyPropertyChanged("AvailableModGroups");
            }
        }


        private ObservableCollection<Modifier> m_allmodifiers;
        public ObservableCollection<Modifier> AllModifiers
        {
            get
            {
                return m_allmodifiers;
            }
            set
            {
                m_allmodifiers = value;
                NotifyPropertyChanged("AllModifiers");
            }
        }



        private ObservableCollection<Modifier> m_availablemodifiers;
        public ObservableCollection<Modifier> AvailableModifiers
        {
            get
            {
                return m_availablemodifiers;
            }
            set
            {
                m_availablemodifiers = value;
                NotifyPropertyChanged("AvailableModifiers");
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

        public ModGroup CurrentModGroup
        {
            get { return m_currentmodgroup; }
            set
            {
                m_currentmodgroup = value;
                NotifyPropertyChanged("CurrentModGroup");
            }
        }

        public Modifier CurrentModifier
        {
            get { return m_currentmodifier; }
            set
            {
                m_currentmodifier = value;
                NotifyPropertyChanged("CurrentModifier");
            }
        }

        public ObservableCollection<ModGroup> ModGroups
        {
            get { return m_modgroups; }
            set
            {
                m_modgroups = value;
                NotifyPropertyChanged("ModGroups");
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



        //---------------------------------------  COMBO Groups ------------------------

        public ObservableCollection<ComboGroup> ComboGroups
        {
            get { return m_combogroups; }
            set
            {
                m_combogroups = value;
                NotifyPropertyChanged("ComboGroups");
            }
        }


        public ObservableCollection<ComboSet> ComboSets
        {
            get { return m_combosets; }
            set
            {
                m_combosets = value;
                NotifyPropertyChanged("ComboSets");
            }
        }



        public ComboGroup CurrentComboGroup
        {
            get { return m_currentcombogroup; }
            set
            {
                m_currentcombogroup = value;
                NotifyPropertyChanged("CurrentComboGroup");
            }
        }

        public ComboSet CurrentComboSet
        {
            get { return m_currentcomboset; }
            set
            {
                m_currentcomboset = value;
                NotifyPropertyChanged("CurrentComboSet");
            }
        }

        private DataTable m_availablecombosets;
        public DataTable AvailableComboSets
        {
            get { return m_availablecombosets; }
            set
            {
                m_availablecombosets = value;
                NotifyPropertyChanged("AvailableComboSets");
            }
        }


        public DataTable AvailableProducts
        {
            get { return m_availableproduct; }
            set
            {
                m_availableproduct = value;
                NotifyPropertyChanged("AvailableProducts");
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


        public bool CanEditProfile
        {
            get
            {
                if (m_currentprofile == null) return false; else return true;
            }
        }

        public bool CanDeleteProfile
        {
            get
            {
                if (m_currentprofile == null) return false;
                else
                {
                    //check to see if it has childrens
                    if (ModGroups.Count == 0) return true; else return false;
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
                    if (Selected.Rows.Count == 0) return true; else return false;
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

        public bool CanCopyCategoryColorCode
        {
            get
            {
                if (m_currentcategory == null) return false;

                if (m_currentcategory.CategoryColorCode == null) return false;
                else return true;
            }
        }

        public bool CanAddModGroup
        {
            get
            {
                if (m_currentprofile == null) return false; else return true;
            }
        }

        public bool CanOpenModGroup
        {
            get { if (CurrentModGroup == null) return false; else return true; }
        }
        public bool CanEditModGroup
        {
            get { if (CurrentModGroup == null || m_currentprofile == null) return false; else return true; }
        }
        public bool CanDeleteModGroup
        {
            get
            {
                if (CurrentModGroup == null) return false;
                else
                {
                    if (CurrentModGroup.Modifiers.Count == 0 && m_currentprofile == null) return true; else return false;

                }
            }
        }
        public bool CanRemoveModGroup
        {
            get
            {
                if (m_currentprofile == null) return false;
                if (CurrentModGroup == null) return false;
                return true;
            }
        }

        public bool CanAddModifier
        {
            get
            {
                if (m_currentmodgroup == null) return false; else return true;
            }
        }
        public bool CanEditModifier
        {
            get { if (m_currentmodifier == null) return false; else return true; }
        }

        //------------------------------------------- Can execute Combo group ---------------------

        public bool CanEditComboGroup
        {
            get
            {
                if (m_currentcombogroup == null) return false; else return true;
            }
        }

        public bool CanDeleteComboGroup
        {
            get
            {
                if (m_currentcombogroup == null) return false;
                else
                {
                    //check to see if it has childrens
                    if (ComboSets.Count == 0) return true; else return false;
                }
            }
        }


        //------------------------------- Can execute Combo Set --------------------------
        public bool CanAddComboSet
        {
            get
            {
                if (m_currentcombogroup == null) return false; else return true;
            }
        }

        public bool CanOpenComboSet
        {
            get { if (CurrentComboSet == null) return false; else return true; }
        }
        public bool CanEditComboSet
        {
            get { if (CurrentComboSet == null || CurrentComboGroup == null) return false; else return true; }
        }
        public bool CanDeleteComboSet
        {
            get
            {
                if (CurrentComboSet == null) return false;
                else
                {
                    if (CurrentComboSet.Products.Count == 0 && m_currentcombogroup == null) return true; else return false;

                }
            }
        }
        public bool CanRemoveComboSet
        {
            get
            {
                if (m_currentcombogroup == null) return false;
                if (CurrentComboSet == null) return false;
                return true;
            }
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
                    LoadCategories(obj_filter.ToString());
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
                    m_currentproduct = new Product(id,0);


                    //if category is clicked second time,then edit
                    if (m_lastprodid == id)
                    {
                        ExecuteEditProductClicked( obj_prodid);
                        m_lastprodid = 0;
                    }else
                    {
                        m_lastprodid = id;
                    }

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
                AvailableInventory dlg = new AvailableInventory(m_parent, this);
                Utility.OpenModal(m_parent, dlg);
             


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

                    ProductDetail prod = new ProductDetail(m_parent, CurrentCategory, m_currentproduct,m_security);
                    Utility.OpenModal(m_parent, prod);

                


                    //refresh list
                    if (CurrentCategory != null) LoadProducts(CurrentCategory.ID);
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

                    if (CurrentCategory != null) //cloning was done in the category page so move to category
                    {
                        m_inventorymodel.AddItemToCategory(CurrentCategory.ID, clone.ID);
                        Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Inventory Edit", "Clone Product", "product", clone.ID);
                    }

                    ProductDetail prod = new ProductDetail(m_parent, CurrentCategory, clone, m_security);
                    Utility.OpenModal(m_parent, prod);

                    //refresh list  
                    if (CurrentCategory != null) LoadProducts(CurrentCategory.ID);
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


                if (m_currentproduct != null)
                {
                    Confirm conf = new Confirm("Delete Item Permanently?");
                    Utility.OpenModal(m_parent, conf);


                    if (conf.Action.ToUpper() == "YES")
                    {

                        m_inventorymodel.DeleteProduct(m_currentproduct.ID);
                        Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Delete Product", m_currentproduct.Description, "product", m_currentproduct.ID);
                    }


                    //refresh list
                    if (CurrentCategory != null) LoadProducts(CurrentCategory.ID);
                    else LoadProducts(1000);

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Delete Product:" + e.Message);

            }
        }
        public void ExecuteAddNewProductClicked(object obj_catid)
        {
            try
            {

                Product product;
                product = m_inventorymodel.AddNewProduct();
                product.ColorCode = CurrentCategory.CategoryColorCode;  //assign the category color to the product by default
                ProductDetail pd = new ProductDetail(m_parent, CurrentCategory, product, m_security);
                Utility.OpenModal(m_parent, pd);



                Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Add New Product", product.Description, "product", product.ID);

                //after product is added, if it was added from the available list, then add to category automatically

                if (CurrentCategory != null) //selected category so add item to category
                {
                    m_inventorymodel.AddItemToCategory(CurrentCategory.ID, product.ID);
                    LoadProducts(CurrentCategory.ID); //editing has been done .. refresh available and selected list
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
                if (obj_catid != null)
                {
                    int id = int.Parse(obj_catid.ToString());
                    if (CurrentCategory != null) CurrentCategory.Selected = false;

                    //current category is set in loadproducts()
                    LoadProducts(id);

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
                        SelectedInventory dlg = new SelectedInventory(m_parent, this);
                    Utility.OpenModal(m_parent, dlg);
                     
                }
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("OPen category" + e.Message);

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
                    Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Add to Category", CurrentCategory.Description, "category", productid);
                    LoadProducts(CurrentCategory.ID);
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

                if (m_currentproduct != null && CurrentCategory.ID > 0)
                {


                    m_inventorymodel.RemoveItemFromCategory(CurrentCategory.ID, m_currentproduct.ID);
                    Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Remove from Category", CurrentCategory.Description, "category", m_currentproduct.ID);

                    LoadProducts(CurrentCategory.ID);
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

                        m_inventorymodel.AddCategory(m_filter, tp.ReturnText);
                        Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Add Category", tp.ReturnText, "category", 0);
                        LoadCategories(m_filter);


                        //Ask user if they want to create new Report Category also
                       if( Confirm.Ask("Create Report Group/Category to match also?"))
                        {
                            int groupid = m_reports.AddNewGroupList("RevenueCategory", tp.ReturnText);
                            m_reports.AddNewCatList(groupid, tp.ReturnText);
                        }
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
                    m_inventorymodel.DeleteCategory(CurrentCategory.ID);
                    Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Delete Category", CurrentCategory.Description, "category", CurrentCategory.ID);
                    LoadCategories(m_filter);
                }

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteDeleteCategoryClicked:" + e.Message);

            }

        }




        public void ExecuteCopyColorToAllClicked(object obj)
        {
            try
            {
                m_inventorymodel.CopyColorToProducts(CurrentCategory.ID, CurrentCategory.CategoryColorCode);
            }
            catch (Exception e)
            {
                MessageBox.Show("Copy color to all Clicked:" + e.Message);

            }
        }


        public void ExecutePictureClicked(object button)
        {
            var picfile = Utility.GetPictureFile();

            //will return null if user cancels
            if (picfile != null) CurrentCategory.ImageSrc = picfile;


            // Utility.PlaySound();

        }


        public void ExecuteClearImageClicked(object obj)
        {
            CurrentCategory.ImageSrc = null;
        }

        public void ExecuteCatMoveUpClicked(object obj)
        {
            CurrentCategoryMoveUp();
            LoadCategories(m_filter);
        }

        public void ExecuteCatMoveDownClicked(object obj)
        {
           CurrentCategoryMoveDown();
            LoadCategories(m_filter);
        }

        #endregion
        //========================================================     MODIFIER PROFILE    ======================================================
        #region modprofile
        public void ExecuteProfileClicked(object obj_profileid)
        {
            try
            {
                if (obj_profileid != null)
                {

                    int profileid = int.Parse(obj_profileid.ToString());
                    m_currentprofile = GetCurrentProfile(profileid);
                    LoadModGroups(profileid);
                }
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Execute Profile Clicked:" + e.Message);

            }
        }

        public void ExecuteAddProfileClicked(object obj_catid)
        {
            try
            {
                TextPad tp = new TextPad("Add New Profile:", "");
                Utility.OpenModal(m_parent, tp);
                if (tp.ReturnText != null)
                    if (tp.ReturnText != "")
                    {

                        m_inventorymodel.AddProfile(tp.ReturnText);
                        Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Add Profile", tp.ReturnText, "profile",0);
                        LoadModifierProfiles();
                    }
            }
            catch (Exception e)
            {
                MessageBox.Show("Add Profile Cliecked:" + e.Message);

            }
        }

        public void ExecuteDeleteProfileClicked(object obj_id)
        {
            try
            {
                Confirm conf = new Confirm("Delete Profile: " + m_currentprofile.Description + "?");
                Utility.OpenModal(m_parent, conf);
              
                if (conf.Action.ToUpper() == "YES")
                {
                    m_inventorymodel.DeleteProfile(m_currentprofile.ID);
                    Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Delete Profile", m_currentprofile.Description, "profile",m_currentprofile.ID);
                    LoadModifierProfiles();
                }

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Delete Profile Clicked:" + e.Message);

            }

        }

        public void ExecuteEditProfileClicked(object obj_id)
        {
            try
            {
                TextPad tp = new TextPad("Profile Description", m_currentprofile.Description);
                Utility.OpenModal(m_parent, tp);

                if (tp.Action == "Ok")
                {
                    m_currentprofile.Description = tp.ReturnText;
                    m_currentprofile.Save();
                    Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Edit Profile", m_currentprofile.Description, "profile", m_currentprofile.ID);
                }

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Edit Profile Clicked:" + e.Message);

            }

        }

        #endregion
        //========================================================     MODIFIER GROUP   ======================================================


        #region modgroup

        public void ExecuteModGroupClicked(object obj_groupid)
        {
            try
            {

                int modgroupid = 0;
                if (obj_groupid != null) modgroupid = int.Parse(obj_groupid.ToString());

                CurrentModGroup = GetCurrentModGroup(modgroupid);

               // Modifiers = m_inventorymodel.GetModifiers(modgroupid);

                if (m_lastmodgroupid == modgroupid)
                {
                    ExecuteOpenModGroupClicked(obj_groupid);
                }
                m_lastmodgroupid = modgroupid;

            }
            catch (Exception e)
            {
                MessageBox.Show("ModGroup Clicked:" + e.Message);

            }

        }


        public void ExecuteOpenModGroupClicked(object obj_groupid)
        {
            try
            {

                AvailableModifiers = m_inventorymodel.GetAvailableModifiers(m_currentmodgroup.ID);


                SelectedModifier dlg = new SelectedModifier(m_parent, this);
                Utility.OpenModal(m_parent, dlg);


                CurrentModGroup.Modifiers = m_inventorymodel.GetModGroupModifiers(CurrentModGroup.ID);  //load updated list


            }
            catch (Exception e)
            {
                MessageBox.Show("ModGroup Open Clicked:" + e.Message);

            }

        }
        public void ExecuteAddModGroupClicked(object obj_catid)
        {
            try
            {
                AvailableModGroups = m_inventorymodel.GetAvailableModGroups(m_currentprofile.ID);
                ModGroupList modgrouplist = new ModGroupList(m_parent, this);
                Utility.OpenModal(m_parent, modgrouplist);
                PopUp = null;
               LoadModGroups(m_currentprofile.ID);
            }
            catch (Exception e)
            {
                MessageBox.Show("Add ModGroup Clicked:" + e.Message);

            }
        }

        public void ExecuteAddModGroupToProfileClicked(object obj_modgroupid)
        {
            try
            {
                int modgroupid = 0;
                if (obj_modgroupid != null) modgroupid = (int)obj_modgroupid;
                m_inventorymodel.AddModGroupToProfile(m_currentprofile.ID, modgroupid);
                Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Mod to Profile", m_currentprofile.Description, "profile", modgroupid);
                LoadModGroups(m_currentprofile.ID);
                PopUp.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Add ModGroupToProfile Clicked:" + e.Message);

            }
        }
        public void ExecuteAddNewModGroupClicked(object obj_catid)
        {
            try
            {


                TextPad tp = new TextPad("Add New Modifier Group:", "");
                Utility.OpenModal(m_parent, tp);
                if (tp.ReturnText != null)
                    if (tp.ReturnText != "")
                    {

                        int newmodgroupid = m_inventorymodel.AddNewModGroup( tp.ReturnText);
                        Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "New ModGroup", tp.ReturnText, "modgroup", m_currentprofile.ID);
                        // AvailableModGroups = m_inventorymodel.GetAvailableModGroups(m_currentprofile.ID);
                        ExecuteAddModGroupToProfileClicked(newmodgroupid);
                     
                    }
            }
            catch (Exception e)
            {
                MessageBox.Show("Add New ModGroup Clicked:" + e.Message);

            }
        }
        public void ExecuteRemoveModGroupClicked(object obj_id)
        {
            try
            {
                m_inventorymodel.RemoveModGroup(m_currentprofile.ID, m_currentmodgroup.ID);
                Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Remove ModGroup", m_currentprofile.Description, "modgroup", m_currentmodgroup.ID);

                LoadModGroups(m_currentprofile.ID);
            }
            catch (Exception e)
            {
                MessageBox.Show("RemoveModifier Group:" + e.Message);

            }

        }

        public void ExecuteDeleteModGroupClicked(object obj_id)
        {
            try
            {
                if (Confirm.Ask("Are you sure you wan to to Delete:" + m_currentmodgroup.Description))
                {
                    //first must check to see if CombSet is still beign used
                    if (m_inventorymodel.IsModGroupUsed(m_currentmodgroup.ID))
                    {
                        TouchMessageBox.Show("This Mod Group is being used in one or more Mod Profile!!");
                        return;
                    }

                    m_inventorymodel.DeleteModGroup(m_currentmodgroup.ID);
                    Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Delete ModGroup", m_currentmodgroup.Description, "modgroup", m_currentmodgroup.ID);
                    LoadModGroups(1000); //load all mod groups

                }
              
            }
            catch (Exception e)
            {
                MessageBox.Show("Delete Modifier Group:" + e.Message);

            }

        }


        /*
        public void ExecuteModGroupDescriptionClicked(object obj)
        {
            try
            {

                TextPad tp = new TextPad("ModGroup Description", CurrentModGroup.Description);
                Utility.OpenModal(m_parent, tp);

                CurrentModGroup.Description = tp.ReturnText;

                CurrentModGroup.Save();

                //refresh profiles list
                // ModGroups = m_inventorymodel.GetModGroups(m_currentprofile.ID, false);
            }
            catch (Exception e)
            {
                MessageBox.Show("Edit Modifier Group :" + e.Message);

            }
        }

        */
        public void ExecuteEditModGroupClicked(object obj_id)
        {

            try
            {


                ModGroupEdit dlg = new ModGroupEdit(m_parent, this); //pass this view model with  the CurrentModGroup
                Utility.OpenModal(m_parent, dlg);

                //refresh profiles list
                // ModGroups = m_inventorymodel.GetModGroups(m_currentprofile.ID, false);
            }
            catch (Exception e)
            {
                MessageBox.Show("Edit Modifier Group :" + e.Message);

            }


           

        }

        #endregion

        //========================================================     MODIFIERS  ======================================================
        #region Modifier
        public void ExecuteModifierClicked(object obj_modid)
        {

            try
            {
                int modid = 0;

                if (obj_modid != null) modid = (int)obj_modid;

                if (modid > 0)
                {

                    CurrentModifier = GetCurrentModifier(modid);
                }


              //  if (m_lastmodid == modid)
              //  {
                   // ExecuteEditModifierClicked(obj_modid);
               // }
              // m_lastmodid = modid;
            }
            catch (Exception e)
            {
                MessageBox.Show("Modifier Click:" + e.Message);

            }
        }
        public void ExecuteAddNewModifierClicked(object obj_catid)
        {
            try
            {

                TextPad tp = new TextPad("Modifier Description", "");
                Utility.OpenModal(m_parent, tp);
                if (tp.ReturnText == "") return;

                //Need to code so that does not create record until confirmed
                Modifier modifier = m_inventorymodel.AddNewModifier(tp.ReturnText);
                AllModifiers = m_inventorymodel.GetAllModifiers();


                Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Add Modifier", modifier.Description, "modifier", 0);


                CurrentModifier = GetCurrentModifier(modifier.ID); //select new modifier from list

         

            }
            catch (Exception e)
            {
                MessageBox.Show("Add New Modifier" + e.Message);

            }
        }

        public void ExecuteAddNewModifierToModGroupClicked(object obj_catid)
        {
            try
            {
                if (CurrentModGroup == null) return;

                TextPad tp = new TextPad("Modifier Description", "");
                Utility.OpenModal(m_parent, tp);
                if (tp.ReturnText == "") return;


                //Need to code so that does not create record until confirmed
                Modifier modifier = m_inventorymodel.AddNewModifier(tp.ReturnText);
                CurrentModifier = modifier;

                //run edit code
                ModifierDetail prod = new ModifierDetail(m_parent, this);
                Utility.OpenModal(m_parent, prod);
                Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Add Modifier", modifier.Description, "modifier", CurrentModGroup.ID);


                //add modifier to modgroup
                m_inventorymodel.AddModifierToModGroup(CurrentModGroup.ID, modifier.ID);

                Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Modifier to MOdgroup", m_currentmodgroup.Description, "modgroup", modifier.ID);
                CurrentModGroup.Modifiers = m_inventorymodel.GetModGroupModifiers(CurrentModGroup.ID);  //load updated list
                AvailableModifiers = m_inventorymodel.GetAvailableModifiers(CurrentModGroup.ID);


               

            }
            catch (Exception e)
            {
                MessageBox.Show("Add New Modifier" + e.Message);

            }
        }

   
        public void ExecuteAddModifierToModGroupClicked(object obj_modifierid)
        {
            try
            {
              

                int modifierid = 0;
                if (obj_modifierid != null) modifierid = (int)obj_modifierid;
                else TouchMessageBox.Show("ERROR!! Modifier ID is null , can not add!!!");

                if (modifierid > 0)
                {

                    CurrentModifier = GetCurrentModifierFromAvailable(modifierid);
                }

                //check for double click
                modifierclickcount++;
                if (lastmodifierid == modifierid)
                {
                    if (modifierclickcount != 2) return;

                    modifierclickcount = 0;
                }
                else
                {
                    modifierclickcount = 1;
                    lastmodifierid = modifierid;
                    return;
                }


                m_inventorymodel.AddModifierToModGroup(CurrentModGroup.ID, modifierid);

                Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Modifier to MOdgroup", m_currentmodgroup.Description, "modgroup", modifierid);
               CurrentModGroup.Modifiers    = m_inventorymodel.GetModGroupModifiers(CurrentModGroup.ID);  //load updated list
                AvailableModifiers          = m_inventorymodel.GetAvailableModifiers(CurrentModGroup.ID);

            }
            catch (Exception e)
            {
                MessageBox.Show("Add Modifier to MOdgroup Clicked:" + e.Message);

            }
        }

        
        public void ExecuteEditModifierClicked(object obj_modid)
        {
            try
            {
                if (m_currentmodifier != null)
                {
                    //run edit code
                    ModifierDetail prod = new ModifierDetail(m_parent, this);
                    Utility.OpenModal(m_parent, prod);
                    CurrentModifier = null;

                }
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Modifier Edit Click:" + e.Message);
            }
        }
      

        //Remove modifier from Group
        public void ExecuteRemoveModifierFromModGroupClicked(object obj_modifierid)
        {
            try
            {

                int modifierid = 0;
                if (obj_modifierid != null) modifierid = (int)obj_modifierid;
                else TouchMessageBox.Show("ERROR!! Modifier ID is null , can not add!!!");

                if (modifierid > 0)
                {

                    CurrentModifier = GetCurrentModifierFromSelected(modifierid);
                }


                //check for double click
                modifierclickcount++;
                if (lastmodifierid == modifierid)
                {
                    if (modifierclickcount != 2) return;
                 
                    modifierclickcount = 0;
                }
                else
                {
                    modifierclickcount = 1;
                    lastmodifierid = modifierid;
                    return;
                }



                m_inventorymodel.RemoveModifier(CurrentModGroup.ID,modifierid);

                logger.Debug("Remove Modifier:" + modifierid);

                CurrentModGroup.Modifiers = m_inventorymodel.GetModGroupModifiers(CurrentModGroup.ID);  //load updated list
                AvailableModifiers = m_inventorymodel.GetAvailableModifiers(CurrentModGroup.ID);

            }
            catch (Exception e)
            {
                MessageBox.Show("Modifier Delete Click:" + e.Message);
            }
        }
        public void ExecuteDeleteModifierClicked(object obj_modid)
        {
            try
            {
                if (m_currentmodifier != null)
                {
                    Confirm conf = new Confirm("Delete Item Permanently? This will remove Modifier from ALL Mod Groups Also.");
                    Utility.OpenModal(m_parent, conf);


                    if (conf.Action == "Yes")
                    {
                        m_inventorymodel.DeleteModifier(m_currentmodifier.ID);
                        Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Delete Modifier Permanently", m_currentmodifier.Description, "modifier", m_currentmodifier.ID);
                        m_currentmodifier = null;
                        AllModifiers = m_inventorymodel.GetAllModifiers();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Modifier Delete Click:" + e.Message);
            }
        }


        public void ExecuteCloneModifierClicked(object obj_modid)
        {
            try
            {
                if (m_currentmodifier != null)
                {
                    Modifier newmod = m_inventorymodel.CloneModifier(m_currentmodifier.ID);
                    Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Clone Modifier", m_currentmodifier.Description, "modifier", newmod.ID);
                    CurrentModGroup.Modifiers = m_inventorymodel.GetModGroupModifiers(CurrentModGroup.ID);
                    m_currentmodifier = null;
                }
            } 
            catch (Exception e)
            {
                MessageBox.Show("Modifier Clone Click:" + e.Message);
            }
        }
        #endregion


        //========================================================  COMBO GROUPS =====================================================
        #region combogroups
        public void ExecuteComboGroupClicked(object obj_profileid)
        {
            try
            {
                if (obj_profileid != null)
                {

                    int combogroupid = int.Parse(obj_profileid.ToString());
                    m_currentcombogroup = GetCurrentComboGroup(combogroupid);
                    LoadComboSets(combogroupid);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Execute Profile Clicked:" + e.Message);

            }
        }

        public void ExecuteAddComboGroupClicked(object obj_catid)
        {
            try
            {
                TextPad tp = new TextPad("Add New Combo Group:", "");
                Utility.OpenModal(m_parent, tp);
                if (tp.ReturnText != null)
                    if (tp.ReturnText != "")
                    {

                        m_inventorymodel.AddComboGroup(tp.ReturnText);
                        Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Add Combo Group", tp.ReturnText, "combogroup", 0);
                        LoadComboGroups();
                    }
            }
            catch (Exception e)
            {
                MessageBox.Show("Add Combo Group Clicked:" + e.Message);

            }
        }

        public void ExecuteDeleteComboGroupClicked(object obj_id)
        {
            try
            {
                Confirm conf = new Confirm("Delete Combo Group: " + m_currentcombogroup.Description + "?");
                Utility.OpenModal(m_parent, conf);

        
                if (conf.Action.ToUpper() == "YES")
                {
                    m_inventorymodel.DeleteComboGroup(m_currentcombogroup.ID);
                    Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Delete Combo Group", m_currentcombogroup.Description, "combogroup", m_currentcombogroup.ID);
                    LoadComboGroups();
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Delete Profile Clicked:" + e.Message);

            }

        }

        public void ExecuteEditComboGroupClicked(object obj_id)
        {
            try
            {
                TextPad tp = new TextPad("Combo Group Description", m_currentcombogroup.Description);
                Utility.OpenModal(m_parent, tp);


                if (tp.Action == "Ok")
                {
                    m_currentcombogroup.Description = tp.ReturnText;
                    m_currentcombogroup.Save();
                    Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Edit Combo Group", m_currentcombogroup.Description, "combogroup", m_currentcombogroup.ID);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Edit Profile Clicked:" + e.Message);

            }

        }



        #endregion

        //========================================================     COMBO SETS   ======================================================


        #region comboset

        public void ExecuteComboSetClicked(object obj_groupid)
        {
            try
            {

                int combosetid = 0;
                if (obj_groupid != null) combosetid = int.Parse(obj_groupid.ToString());

                CurrentComboSet = GetCurrentComboSet(combosetid);

                if (m_lastcombosetid == combosetid)
                {
                    ExecuteOpenComboSetClicked(obj_groupid);
                }
                m_lastcombosetid = combosetid;

            }
            catch (Exception e)
            {
                MessageBox.Show("ComboSet Clicked:" + e.Message);

            }

        }


        public void ExecuteOpenComboSetClicked(object obj_groupid)
        {
            try
            {
                AvailableProducts = m_inventorymodel.GetAvailableComboSetProducts(CurrentComboSet.ID);

                SelectedProduct dlg = new SelectedProduct(m_parent, this);
                Utility.OpenModal(m_parent, dlg);



            }
            catch (Exception e)
            {
                MessageBox.Show("ComboSet Open Clicked:" + e.Message);

            }

        }
        public void ExecuteAddComboSetClicked(object obj_catid)
        {
            try
            {
                AvailableComboSets = m_inventorymodel.GetAvailableComboSets(m_currentcombogroup.ID);
                ComboSetList combosetlist = new ComboSetList(m_parent, this);
                Utility.OpenModal(m_parent, combosetlist);

                LoadComboSets(m_currentcombogroup.ID);
            }
            catch (Exception e)
            {
                MessageBox.Show("Add ModGroup Clicked:" + e.Message);

            }
        }

    
        public void ExecuteAddNewComboSetClicked(object obj_catid)
        {
            try
            {


                TextPad tp = new TextPad("Add New Combo Set:", "");
                Utility.OpenModal(m_parent, tp);
                if (tp.ReturnText != null)
                    if (tp.ReturnText != "")
                    {

                        m_inventorymodel.AddNewComboSet( tp.ReturnText);
                        Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "New ComboSet", tp.ReturnText, "combogroup", m_currentcombogroup.ID);
                        AvailableComboSets = m_inventorymodel.GetAvailableComboSets(m_currentcombogroup.ID);
                       // LoadComboSets(m_currentcombogroup.ID);
                    }
            }
            catch (Exception e)
            {
                MessageBox.Show("Add New ComboSet Clicked:" + e.Message);

            }
        }
        public void ExecuteRemoveComboSetClicked(object obj_id)
        {
            try
            {
                m_inventorymodel.RemoveComboSet(m_currentcombogroup.ID, m_currentcomboset.ID);
                Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Remove ComboSet", m_currentcombogroup.Description, "comboset", m_currentcomboset.ID);

                LoadComboSets(m_currentcombogroup.ID);
            }
            catch (Exception e)
            {
                MessageBox.Show("RemoveComboSet:" + e.Message);

            }

        }

        public void ExecuteDeleteComboSetClicked(object obj_id)
        {
            try
            {
                

                if(Confirm.Ask("Are you sure you wan to to Delete:" + m_currentcomboset.Description))
                {
                    //first must check to see if CombSet is still beign used
                    if (m_inventorymodel.IsComboSetUsed(m_currentcomboset.ID))
                    {
                        TouchMessageBox.Show("This Combo Set is being used in one or more Combo Group!!");
                        return;
                    }


                    m_inventorymodel.DeleteComboSet(m_currentcomboset.ID);
                    Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Delete ComboSet", m_currentcomboset.Description, "modgroup", m_currentcomboset.ID);
                    LoadComboSets(1000); //load all como sets

                }
              
            }
            catch (Exception e)
            {
                MessageBox.Show("Delete ComboSet:" + e.Message);

            }

        }


   
        public void ExecuteEditComboSetClicked(object obj_id)
        {

            try
            {


                ComboSetEdit dlg = new ComboSetEdit(m_parent, this); //pass this view model with  the CurrentModGroup
                Utility.OpenModal(m_parent, dlg);


                CurrentComboSet.Save();

           
            }
            catch (Exception e)
            {
                MessageBox.Show("Edit ComboSet :" + e.Message);

            }




        }


        public void ExecuteAddComboSetToComboGroupClicked(object obj_combosetid)
        {
            try
            {
                int combosetid = 0;
                if (obj_combosetid != null) combosetid = (int)obj_combosetid;
                m_inventorymodel.AddComboSetToComboGroup(m_currentcombogroup.ID, combosetid);
                Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Combo Set to ComboGroup", m_currentcombogroup.Description, "combogroup", combosetid);
                LoadComboSets(m_currentcombogroup.ID);
            }
            catch (Exception e)
            {
                MessageBox.Show("Add ComboSet 2 ComboGroup Clicked:" + e.Message);

            }
        }


        //--------------------COMBO SET , product items ----------------------------------------------------

        public void ExecuteRemoveFromComboSetClicked(object obj)
        {
            try
            {
                int productid = 0;
                if (obj != null) productid = (int)obj;
                m_inventorymodel.RemoveFromComboSet(m_currentcomboset.ID, productid);
                Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Remove Product From ComboSet", m_currentcomboset.Description, "comboset", productid);
                //refresh current comboset
                CurrentComboSet.Products = m_inventorymodel.GetComboProducts(CurrentComboSet.ID,0);
                AvailableProducts = m_inventorymodel.GetAvailableComboSetProducts(CurrentComboSet.ID);

            }
            catch (Exception e)
            {
                MessageBox.Show("Add product to comboset Clicked:" + e.Message);

            }
        }


        public void ExecuteAddToComboSetClicked(object obj)
        {
            try
            {
                int productid = 0;
                if (obj != null) productid = (int)obj;
                m_inventorymodel.AddToComboSet(m_currentcomboset.ID, productid);
                Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Product To ComboSet", m_currentcomboset.Description, "comboset", productid);
                //refresh current comboset
                CurrentComboSet.Products = m_inventorymodel.GetComboProducts(CurrentComboSet.ID,0);
                AvailableProducts = m_inventorymodel.GetAvailableComboSetProducts(CurrentComboSet.ID);
            }
            catch (Exception e)
            {
                MessageBox.Show("Add product to comboset Clicked:" + e.Message);

            }
        }



        #endregion


















        //========================================================    FUNCTIONS  ======================================================

        private void LoadCategories(string filter)
        {

            Categories = m_inventorymodel.FillCategorybyType(filter);

            foreach(Category cat in Categories)
            {
                if(cat.ID == m_lastcatid)
                {
                    CurrentCategory = cat;
                    CurrentCategory.Selected = true;
                    return;
                }
                cat.Selected = false;
            }

            CurrentCategory = null;
            Selected = null;
            Available = null;
            m_filter = filter;
            m_lastcatid = 0;
        }

        private void LoadProducts(int id)
        {

            if (id == 1000)
            {
                CurrentCategory = null;
                Selected = m_inventorymodel.GetAllProducts();
            }
            else
            {
                CurrentCategory = GetCurrentCategory(id);
                CurrentCategory.Selected = true;
                Selected = m_inventorymodel.GetProductsbyCatID(id);
            }

            m_currentproduct = null;

            Available = m_inventorymodel.GetProductsNOTbyCatID("", id);
            m_lastprodid = 0;

        }

        Category GetCurrentCategory(int id)
        {
            foreach(Category cat in Categories)
            {
                if (cat.ID == id) return cat;
            }
            return null;
        }


        private void CurrentCategoryMoveUp()
        {
            int last=0;
            int current=0;
            Category LastCat = null;
            foreach (Category cat in Categories)
            {
                current = cat.SortOrder;
                if(cat == CurrentCategory)
                    if(LastCat != null)
                    {
                        LastCat.SortOrder = current;
                        CurrentCategory.SortOrder = last;
                    }

                last = cat.SortOrder;
                LastCat = cat;
            }

        }

        private void CurrentCategoryMoveDown()
        {
          
            int current = 0;
            bool found = false;
   
            foreach (Category cat in Categories)
            {

                if (found)
                {
                    if (cat.SortOrder == 0) return; //last one is not a valid category and should have sort order of 0

                    current = cat.SortOrder; //save first
                    cat.SortOrder = CurrentCategory.SortOrder; //replace
                    CurrentCategory.SortOrder = current;
                    found = false;
                    return;
                }

              
                if (cat == CurrentCategory) found = true;


            }

        }


        private void LoadModifierProfiles()
        {
            ModProfiles = m_inventorymodel.GetProfiles(false);
            m_currentprofile = null;
            CurrentModGroup = null;
        }

        private void LoadModGroups(int profileid)
        {
            if (profileid == 1000) m_currentprofile = null;

            ModGroups = m_inventorymodel.GetModGroups(profileid);
         
            CurrentModGroup = null;
            m_lastmodgroupid = 0;
   
        }


        public ModProfile GetCurrentProfile(int id)
        {

            foreach (ModProfile prof in ModProfiles)
            {
                if (prof.ID == id) return prof;
            }
            return null;
        }


        public ModGroup GetCurrentModGroup(int modgroupid)
        {
            foreach (ModGroup mod in ModGroups)
            {
                if (mod.ID == modgroupid) return  mod;
            }
            return null;
        }

        public Modifier GetCurrentModifier(int modid)
        {
            foreach (Modifier mod in AllModifiers)
            {
                if (mod.ID == modid) return mod;
            }
            return null;
        }

        public Modifier GetCurrentModifierFromAvailable(int modid)
        {
            foreach (Modifier mod in AvailableModifiers)
            {
                if (mod.ID == modid) return mod;
            }
            return null;
        }

        public Modifier GetCurrentModifierFromSelected(int modid)
        {
            foreach (Modifier mod in CurrentModGroup.Modifiers)
            {
                if (mod.ID == modid) return mod;
            }
            return null;
        }


        //--------------------------------------------------COMBO GROUPS -------------------------------------------------------------
        public ComboGroup GetCurrentComboGroup(int id)
        {

            foreach (ComboGroup prof in ComboGroups)
            {
                if (prof.ID == id) return prof;
            }
            return null;
        }

        private void LoadComboGroups()
        {

            ComboGroups = m_inventorymodel.GetComboGroups(false);
            CurrentComboGroup = null;
            CurrentComboSet = null;
        }
        private void LoadComboSets(int combogroupid)
        {
            if (combogroupid == 1000) CurrentComboGroup = null;

            ComboSets = m_inventorymodel.GetComboSets(combogroupid);
            CurrentComboSet = null;
            m_lastcombosetid = 0;

        }

        public ComboSet GetCurrentComboSet(int combosetid)
        {
            foreach (ComboSet combo in ComboSets)
            {
                if (combo.ID == combosetid) return combo;
            }
            return null;
        }
    }
}
