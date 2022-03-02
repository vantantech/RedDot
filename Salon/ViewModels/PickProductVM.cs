using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using RedDotBase;

namespace RedDot
{
   
    public class PickProductVM : INPCBase
    {
        DataTable m_products;
        MenuSetupModel m_menumodel;
        public PickProductVM()
        {
            m_menumodel = new MenuSetupModel();
            Products = m_menumodel.GetAllProducts();
        }


        public DataTable Products
        {
            get { return m_products; }

            set
            {
                m_products = value;
                NotifyPropertyChanged("Products");
            }
        }
    }
}
