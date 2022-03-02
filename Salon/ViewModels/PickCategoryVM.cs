using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDotBase;

namespace RedDot
{
    public class PickCategoryVM:INPCBase
    {
        DataTable m_categories;
    
        MenuSetupModel m_menumodel;
        public PickCategoryVM()
        {

            m_menumodel = new MenuSetupModel();
            Categories = m_menumodel.GetCateogories();

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

    }
}
