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
   public  class CategoryVM:INPCBase
    {
       Category m_currentcategory;
       MenuSetupModel m_inventorymodel;

       public ICommand CopyColorToAllClicked { get; set; }
        public ICommand PictureClicked { get; set; }
        public ICommand ClearImageClicked { get; set; }


        public CategoryVM(Category cat)
       {
           m_currentcategory = cat;
           m_inventorymodel = new MenuSetupModel();


           CopyColorToAllClicked = new RelayCommand(ExecuteCopyColorToAllClicked, param => this.CanCopy);
            PictureClicked = new RelayCommand(ExecutePictureClicked, param => true);
            ClearImageClicked = new RelayCommand(ExecuteClearImageClicked, param => true);

        }

       public Category CurrentCategory
       {
           get { return m_currentcategory; }
           set { m_currentcategory = value;
           NotifyPropertyChanged("CurrentCategory");
          }
       }

       public bool CanCopy
       {
           get
           {
               if (m_currentcategory.CategoryColorCode == null) return false;
               else return true;
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
    }
}
