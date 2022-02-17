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
    public class ProductVM:INPCBase
    {
        private MenuItem m_product;
        private Category m_currentcategory;
        private MenuSetupModel m_menusetupmodel;
        private int m_productwidth;
        private int m_productheight;
        private int m_productfontsize;
        private Window m_parent;


        private Visibility m_commissionvisibility;


        public ICommand CopyCategoryColorClicked { get; set; }
        public ICommand PictureClicked { get; set; }
        public ICommand DeleteProductClicked { get; set; }
        public ICommand CloneProductClicked { get; set; }
        public ICommand PrintClicked { get; set; }

        public List<ListType> ProductTypes { get; set; }
        public List<ListType> CommissionTypes { get; set; }
        public List<ListPair> ReportCategories { get; set; }



        public ICommand PriceClicked { get; set; }


        public ProductVM(Window parent,Category category,MenuItem product)
        {
            m_parent = parent;
            m_product = product;
            m_currentcategory = category;
            m_menusetupmodel = new MenuSetupModel();


            CopyCategoryColorClicked = new RelayCommand(ExecuteCopyCategoryColorClicked, param => this.CanCopy);
            PictureClicked = new RelayCommand(ExecutePictureClicked, param => true);
            DeleteProductClicked = new RelayCommand(ExecuteDeleteProductClicked, param => this.CanDeleteProduct);
            CloneProductClicked = new RelayCommand(ExecuteCloneProductClicked, param=>true);
            PrintClicked = new RelayCommand(ExecutePrintClicked, param => true);
            PriceClicked = new RelayCommand(ExecutePriceClicked, param => true);

            ProductTypes = new List<ListType>();
            ProductTypes.Add(new ListType() { Name = "Product" });
            ProductTypes.Add(new ListType() { Name = "Gift Card" });
            ProductTypes.Add(new ListType() { Name = "Gift Certificate" });
            ProductTypes.Add(new ListType() { Name = "Service" });

            CommissionTypes = new List<ListType>();
            CommissionTypes.Add(new ListType() { Name = "percent" });
            CommissionTypes.Add(new ListType() { Name = "fixed" });
            CommissionTypes.Add(new ListType() { Name = "none" });

         
            m_productwidth = GlobalSettings.Instance.ProductWidth - 50;  //minus 50 for the Prefix box width which is always 50 constant
            m_productheight = GlobalSettings.Instance.ProductHeight ;
  
            m_productfontsize = GlobalSettings.Instance.ProductFontSize;

            ReportCategories = m_menusetupmodel.GetReportCategories();

        }



        public int ProductWidth
        {
            get { return m_productwidth; }
        }

        public int ProductHeight
        {
            get { return m_productheight; }
        }


        public int DescriptionHeight
        {
            get { return m_productheight-25; }
        }
        public int ProductFontSize
        {
            get { return m_productfontsize; }
        }


        public Visibility CommissionVisibility
        {
            get { return m_commissionvisibility; }
            set
            {
                m_commissionvisibility = value;
                NotifyPropertyChanged("CommissionVisibility");
            }
        }

        public MenuItem CurrentProduct
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

        public bool CanDeleteProduct
        {
            get
            {
                return true;

            }

        }











        public void ExecuteDeleteProductClicked(object obj_productid)
        {
            try
            {

                    Confirm conf = new Confirm("Delete Item Permanently?");
                Utility.OpenModal(m_parent, conf);


                    if (conf.Action.ToUpper() == "YES")
                    {

                        m_menusetupmodel.DeleteProduct(m_product.ID);
                    }

                    m_parent.Close();
                   
               
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Delete Product:" + e.Message);

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
                TouchMessageBox.Show("Copy Category Clicked:" + e.Message);

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
        private void ExecutePrintClicked(object obj)
        {
            string printername = GlobalSettings.Instance.ReceiptPrinter;
            ReceiptPrinter printer = new ReceiptPrinter(printername);

            printer.PrintLF(m_product.Description + " : " + m_product.PriceStr);
            printer.Send();

            if (m_product.BarCode == "")  m_product.BarCode = m_product.ID.ToString().PadLeft(12,'0');
            
         
           printer.PrintBarCode(m_product.BarCode);

            printer.LineFeed();
            printer.LineFeed();
            printer.Send();
            printer.Cut();
        }

        public void ExecutePriceClicked(object button)
        {
            NumPad num = new NumPad("Enter Price", false);
            Utility.OpenModal(m_parent, num);
            if(num.Amount != "") m_product.Price = Decimal.Parse(num.Amount);
           

        }

        public void ExecuteCloneProductClicked(object obj)
        {
            try
            {

                MenuItem product;
                product = m_menusetupmodel.AddNewProduct();
                product.ColorCode = CurrentProduct.ColorCode;
                if(CurrentProduct.MenuPrefix != "")
                {

                    string test = Regex.Match(CurrentProduct.MenuPrefix, @"\d+").Value;
                    if (test != "")
                    {
                        int val = int.Parse(test);

                        

                        product.MenuPrefix = m_currentcategory.LetterCode + (val + 1).ToString();
                    }
                }
               
                product.ReportCategory = CurrentProduct.ReportCategory;
                product.Price = CurrentProduct.Price;
                product.Description = CurrentProduct.Description;
                product.SupplyFee = CurrentProduct.SupplyFee;
                product.Taxable = CurrentProduct.Taxable;
                product.TurnValue = CurrentProduct.TurnValue;
                product.CommissionAmt = CurrentProduct.CommissionAmt;
                product.CommissionType = CurrentProduct.CommissionType;



                ProductDetail pd = new ProductDetail(m_parent,m_currentcategory, product);
                Utility.OpenModal(m_parent, pd);


               m_menusetupmodel.AddItemToCategory(m_currentcategory.ID, product.ID);

                m_parent.Close();

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Clone ProductClicked:" + e.Message);

            }

        }
    }

    public class ListType
    {
        public string Name { get; set; }
    }

  
}
