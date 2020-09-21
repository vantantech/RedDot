using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class ComboGroup: INPCBase
    {
        public int ID { get; set; }
        private string m_description;
        public string Description
        {
            get { return m_description; }
            set
            {
                m_description = value;
                NotifyPropertyChanged("Description");
            }
        }

        private DBProducts m_dbproducts;

   

        public ComboGroup(int id)
        {
            m_dbproducts = new DBProducts();
            DataTable dt = m_dbproducts.GetComboGroup(id);
            if (dt.Rows.Count > 0) InitData(dt.Rows[0]);
        }
        public ComboGroup(DataRow row)
        {
            InitData(row);
        }
        private void InitData(DataRow row)
        {

            if (row["id"].ToString() != "") ID = int.Parse(row["id"].ToString());
            else ID = 0;
    
            Description = row["description"].ToString();
           
           
        }


        public void Save()
        {
            if (m_dbproducts == null) m_dbproducts = new DBProducts();

            m_dbproducts.UpdateComboGroupString(ID, "description", Description);
         
        }

    }
}
