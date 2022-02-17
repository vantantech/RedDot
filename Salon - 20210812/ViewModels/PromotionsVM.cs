using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class PromotionsVM : INPCBase
    {
        DataTable m_promotions;
        DataTable m_selectedcategories;
        DataTable m_selectedproducts;

        PromotionsModel m_promomodel;
        Promotion m_currentpromotion;

        Visibility m_promotionvisibility;


        int m_promotionid = 0;
        int m_categoryid = 0;
        int m_productid = 0;



        private MenuSetupModel m_inventorymodel;

        public ICommand PromotionClicked { get; set; }
        public ICommand CategoryClicked { get; set; }
        public ICommand ProductClicked { get; set; }

        public ICommand RefreshClicked { get; set; }
        public ICommand AddClicked { get; set; }
        public ICommand DeleteClicked { get; set; }

        public ICommand AddCategoryClicked { get; set; }
        public ICommand DeleteCategoryClicked { get; set; }



        public ICommand AddProductClicked { get; set; }
        public ICommand DeleteProductClicked { get; set; }



        public PromotionsVM()
        {
        


            PromotionClicked = new RelayCommand(ExecutePromotionClicked, param => true);
            CategoryClicked = new RelayCommand(ExecuteCategoryClicked, param => true);
            ProductClicked = new RelayCommand(ExecuteProductClicked, param => true);


            RefreshClicked = new RelayCommand(ExecuteRefreshClicked, param => true);
            AddClicked = new RelayCommand(ExecuteAddClicked, param => true);
            DeleteClicked = new RelayCommand(ExecuteDeleteClicked, param => this.IsButtonSelected);

            AddCategoryClicked = new RelayCommand(ExecuteAddCategoryClicked, param => true);
            DeleteCategoryClicked = new RelayCommand(ExecuteDeleteCategoryClicked, param => this.IsCategorySelected);


            AddProductClicked = new RelayCommand(ExecuteAddProductClicked, param => true);
            DeleteProductClicked = new RelayCommand(ExecuteDeleteProductClicked, param => this.IsProductSelected);



            m_promomodel = new PromotionsModel();
            m_inventorymodel = new MenuSetupModel();


            Categories = m_inventorymodel.GetCategoryList();

            Products = m_inventorymodel.GetAllProducts();

            m_promotionvisibility = Visibility.Hidden;

            DiscountTypes = new List<ListType>();
            DiscountTypes.Add(new ListType() { Name = "DISCOUNT" });
            DiscountTypes.Add(new ListType() { Name = "AUTO PRICE" });
            DiscountTypes.Add(new ListType() { Name = "EMPLOYEE MEAL" });


            DiscountMethods = new List<ListType>();
            DiscountMethods.Add(new ListType() { Name = "PERCENT" });
            DiscountMethods.Add(new ListType() { Name = "AMOUNT" });


            UsageRestrictions = new List<ListType>();
            UsageRestrictions.Add(new ListType() { Name = "Unlimited" });
            UsageRestrictions.Add(new ListType() { Name = "Numbered" });
            UsageRestrictions.Add(new ListType() { Name = "Once/Person/Day" });

            //security selection list
            SecurityLevels = new List<ListPair>();
            SecurityLevels.Add(new ListPair() { Description = "Employee", Value = 10 });
            SecurityLevels.Add(new ListPair() { Description = "Manager", Value = 50 });
            SecurityLevels.Add(new ListPair() { Description = "Admin/Owner", Value = 100 });


            PromotionList = m_promomodel.GetPromotions();
  
        }


        /***
 *      ____        _     _ _        ____                            _         
 *     |  _ \ _   _| |__ | (_) ___  |  _ \ _ __ ___  _ __   ___ _ __| |_ _   _ 
 *     | |_) | | | | '_ \| | |/ __| | |_) | '__/ _ \| '_ \ / _ \ '__| __| | | |
 *     |  __/| |_| | |_) | | | (__  |  __/| | | (_) | |_) |  __/ |  | |_| |_| |
 *     |_|    \__,_|_.__/|_|_|\___| |_|   |_|  \___/| .__/ \___|_|   \__|\__, |
 *                                                  |_|                  |___/ 
 */

        public List<ListPair> SecurityLevels { get; set; }


        public Visibility PromotionVisibility
        {
            get { return m_promotionvisibility; }

            set
            {
                m_promotionvisibility = value;
                NotifyPropertyChanged("PromotionVisibility");
            }
        }


        public DataTable PromotionList
        {
            get {return m_promotions; }

            set
            {
                m_promotions = value;
                NotifyPropertyChanged("PromotionList");
            }
        }


        public DataTable SelectedCategories
        {
            get { return m_selectedcategories; }

            set
            {
                m_selectedcategories = value;
                NotifyPropertyChanged("SelectedCategories");
            }
        }

        public DataTable SelectedProducts
        {
            get { return m_selectedproducts; }

            set
            {
                m_selectedproducts = value;
                NotifyPropertyChanged("SelectedProducts");
            }
        }


        public List<ListType> DiscountTypes { get; set; }
        public List<ListType> DiscountMethods { get; set; }
        public List<ListType> UsageRestrictions { get; set; }

        public DataTable Categories { get; set; }
        public DataTable Products { get; set; }

        public Promotion CurrentPromotion
        {
            get { return m_currentpromotion; }
            set { m_currentpromotion = value;
            NotifyPropertyChanged("CurrentPromotion");

            }
        }

        public bool IsButtonSelected
        {

            get
            {
                if (m_promotionid == 0) return false;
                else return true;
            }
        }

        public bool IsCategorySelected
        {

            get
            {
                if (m_promotionid == 0) return false;
                if (m_categoryid == 0) return false;
                else return true;
            }
        }


        public bool IsProductSelected
        {

            get
            {
                if (m_promotionid == 0) return false;
                if (m_productid == 0) return false;
                else return true;
            }
        }







        //-----------------------------------------------------------------METHODS -----------------------------------------
        private void RefreshPromotionList()
        {
            m_promotionid = 0;
            PromotionList = m_promomodel.GetPromotions();
            PromotionVisibility = Visibility.Hidden;
        }
        public void ExecutePromotionClicked(object objturnid)
        {
            try
            {

                if (objturnid != null) m_promotionid = (int)objturnid;

                var row = m_promomodel.GetPromotionbyID(m_promotionid);

                CurrentPromotion = new Promotion(row, true);
                PromotionVisibility = Visibility.Visible;

                SelectedCategories =  m_promomodel.GetPromotionCategories(m_promotionid);

                SelectedProducts = m_promomodel.GetPromotionProducts(m_promotionid);

                m_categoryid = 0;
                m_productid = 0;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Promotion Selected:" + e.Message);

            }

        }


        public void ExecuteCategoryClicked(object objturnid)
        {
            try
            {
                if (objturnid != null) m_categoryid = (int)objturnid;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Category Selected:" + e.Message);
            }

        }

        public void ExecuteProductClicked(object objturnid)
        {
            try
            {
                if (objturnid != null) m_productid = (int)objturnid;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Product Selected:" + e.Message);
            }

        }

        public void ExecuteRefreshClicked(object obj)
        {
            RefreshPromotionList();
        }

        public void ExecuteAddClicked(object obj)
        {
           
            m_promomodel.AddPromotion();
            RefreshPromotionList();
        }

        public void ExecuteDeleteClicked(object obj)
        {
            if (m_promotionid == 0) return;

          
            m_promomodel.DeletePromotion(m_promotionid);

            RefreshPromotionList();

        }


        public void ExecuteAddCategoryClicked(object obj)
        {
            if (m_promotionid == 0) return;

            PickCategory pck = new PickCategory();
            pck.Topmost = true;
            pck.ShowDialog();

            if(pck.CategoryID > 0)
            {
                m_promomodel.AddPromotionCategory(m_promotionid, pck.CategoryID);
                SelectedCategories = m_promomodel.GetPromotionCategories(m_promotionid);
            }
       
        }

        public void ExecuteDeleteCategoryClicked(object obj)
        {
            if (m_promotionid == 0) return;
            if (m_categoryid == 0) return;

            m_promomodel.DeletePromotionCategory(m_promotionid, m_categoryid);
            SelectedCategories = m_promomodel.GetPromotionCategories(m_promotionid);

        
        }


        public void ExecuteAddProductClicked(object obj)
        {
            if (m_promotionid == 0) return;
            PickProduct pck = new PickProduct();
            pck.Topmost = true;
            pck.ShowDialog();

            if (pck.ProductID > 0)
            {
                m_promomodel.AddPromotionProduct(m_promotionid, pck.ProductID);
                SelectedProducts = m_promomodel.GetPromotionProducts(m_promotionid);
            }
         
        }

        public void ExecuteDeleteProductClicked(object obj)
        {
            if (m_promotionid == 0) return;
            if (m_productid == 0) return;

            m_promomodel.DeletePromotionProduct(m_promotionid, m_productid);
            SelectedProducts = m_promomodel.GetPromotionProducts(m_promotionid);
        }









    }
}
