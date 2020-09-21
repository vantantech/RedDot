using System;
using System.Collections.Generic;
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
       InventoryModel m_inventorymodel;

       public ICommand CopyColorToAllClicked { get; set; }
       public CategoryVM(Category cat)
       {
           m_currentcategory = cat;
           m_inventorymodel = new InventoryModel();


           CopyColorToAllClicked = new RelayCommand(ExecuteCopyColorToAllClicked, param => this.CanCopy);


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

    }
}
