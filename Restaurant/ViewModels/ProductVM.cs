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
    public class ProductVM:INPCBase
    {
        private Product m_product;
        private Category m_currentcategory;
        ReceiptPrinterModel printermodel = new ReceiptPrinterModel();
        private MenuSetupModel m_inventorymodel;
        
        private SecurityModel m_security;
        public ICommand CopyCategoryColorClicked { get; set; }
        public ICommand PictureClicked { get; set; }
        public ICommand SaveClicked { get; set; }

        public List<ProductType> ProductTypes { get; set; }
        public List<ListPair> ReportCategories { get; set; }

        public Window m_parent;
        public ProductVM(Window parent, Category category,Product product, SecurityModel security)
        {
            m_parent = parent;
            m_product = product;
            m_currentcategory = category;
            m_security = security;
            m_inventorymodel = new MenuSetupModel();

            CopyCategoryColorClicked = new RelayCommand(ExecuteCopyCategoryColorClicked, param => this.CanCopy);
            PictureClicked = new RelayCommand(ExecutePictureClicked, param => true);
            SaveClicked = new RelayCommand(ExecuteSaveClicked, param => true);

            ProductTypes = new List<ProductType>();
            ProductTypes.Add(new ProductType() { Name = "product" });
            ProductTypes.Add(new ProductType() { Name = "combo" });
            ProductTypes.Add(new ProductType() { Name = "Gift Card" });
            ProductTypes.Add(new ProductType() { Name = "Gift Certificate" });

            // ProductTypes.Add(new ProductType() { Name = "service" });


            ModProfiles = m_inventorymodel.GetProfiles(true);
            ComboGroups = m_inventorymodel.GetComboGroups(true);
            ReportCategories = m_inventorymodel.GetReportCategories();

            Locations = printermodel.GetPrinterLocations();
            DataRow newrow;

          
                //Add empty row on top so user can select "none"
                newrow = Locations.NewRow();
                newrow["id"] = 0;
                newrow["Description"] = "None";
                Locations.Rows.InsertAt(newrow, 0);
          

        }

 

        DataTable m_locations;
        public DataTable Locations
        {
            get { return m_locations; }
            set
            {
                m_locations = value;
                NotifyPropertyChanged("Locations");
            }
        }


        private ObservableCollection<ModProfile> m_profiles;
        public ObservableCollection<ModProfile> ModProfiles
        {
            get { return m_profiles; }
            set
            {
                m_profiles = value;
                NotifyPropertyChanged("ModProfiles");
            }
        }

        private ObservableCollection<ComboGroup> m_combogroups;
        public ObservableCollection<ComboGroup> ComboGroups
        {
            get { return m_combogroups; }
            set
            {
                m_combogroups = value;
                NotifyPropertyChanged("ComboGroups");
            }
        }

        public Product CurrentProduct
        {

            get { return m_product; }
            set { m_product = value;
            NotifyPropertyChanged("CurrentProduct");
            }
        }

        public bool CanCopy
        {
            get
            {
                if (m_currentcategory == null) return false;
                else return true;
            }
        }
        public void ExecuteCopyCategoryColorClicked(object obj)
        {
            try
            {
                CurrentProduct.ColorCode = m_currentcategory.CategoryColorCode;
                
            }
            catch (Exception e)
            {
                MessageBox.Show("Copy Category Clicked:" + e.Message);

            }
        }

        public void ExecutePictureClicked(object button)
        {
            var picfile = Utility.GetPictureFile();

            //will return null if user cancels
            if (picfile != null) CurrentProduct.ImageSrc = picfile;

            // Utility.PlaySound();

        }


        public void ExecuteSaveClicked(object button)
        {
            CurrentProduct.Save();
            Audit.WriteLog(m_security.CurrentEmployee.DisplayName, "Inventory Edit", "Save Product", "product", CurrentProduct.ID);
            m_parent.Close();
        }
    }

    public class ProductType
    {
        public string Name { get; set; }
    }
}
