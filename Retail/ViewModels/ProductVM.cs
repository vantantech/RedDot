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
        private InventoryModel m_inventorymodel;
        public ICommand CopyCategoryColorClicked { get; set; }
        public ICommand PictureClicked { get; set; }
        public ICommand SaveClicked { get; set; }

        public List<DropDownListType> ProductTypes { get; set; }
        public List<DropDownListType> CommissionTypes { get; set; }

        public DataTable Locations { get; set; }

        public ProductVM(Category category,Product product)
        {
            m_product = product;
            m_currentcategory = category;
            m_inventorymodel = new InventoryModel();


            CopyCategoryColorClicked = new RelayCommand(ExecuteCopyCategoryColorClicked, param => this.CanCopy);
            PictureClicked = new RelayCommand(ExecutePictureClicked, param => true);
            SaveClicked = new RelayCommand(ExecuteSaveClicked, param => true);

            ProductTypes = new List<DropDownListType>();
            ProductTypes.Add(new DropDownListType() { Name = "product" });
            ProductTypes.Add(new DropDownListType() { Name = "giftcard" });
            ProductTypes.Add(new DropDownListType() { Name = "service" });
            ProductTypes.Add(new DropDownListType() { Name = "shipping" });

            CommissionTypes = new List<DropDownListType>();
            CommissionTypes.Add(new DropDownListType() { Name = "none" });
            CommissionTypes.Add(new DropDownListType() { Name = "percent" });
            CommissionTypes.Add(new DropDownListType() { Name = "fixed" });

            Locations = m_inventorymodel.GetLocations();
     

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
            OpenFileDialog picfile = new OpenFileDialog();
            picfile.DefaultExt = "jpg";
            picfile.Filter = "PNG Files (*.png)|*.png|JPG files (*.jpg)|*.jpg|BMP files (*.bmp)|*.bmp|All files (*.*)|*.*";
            string AppPath;
            string selectedPath;

            AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            AppPath = AppPath.ToUpper();

            if (picfile.ShowDialog() == true)
            {
                selectedPath = picfile.FileName;
                selectedPath = selectedPath.ToUpper();
                selectedPath = selectedPath.Replace(AppPath, "");

                CurrentProduct.ImageSrc = selectedPath.Replace("\\", "\\\\");
            }
            // Utility.PlaySound();

        }


        public void ExecuteSaveClicked(object button)
        {
            CurrentProduct.Save();

        }
    }

    public class DropDownListType
    {
        public string Name { get; set; }
    }

 
}
