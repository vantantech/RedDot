using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;

namespace RedDot
{
    public class ModProfile:INPCBase
    {
        DBProducts m_dbproducts;
        public ModProfile(int id)
        {
           m_dbproducts = new DBProducts();

            DataTable dt = m_dbproducts.GetProfileByID(id);
            InitData(dt.Rows[0]);

        }
        public ModProfile(DataRow row)
        {
           InitData(row);
        }

        private void InitData(DataRow row)
        {

             ID = (int)row["id"];
            Description = row["description"].ToString();
        }

        public void Save()
        {
            if (m_dbproducts == null) m_dbproducts = new DBProducts();
            m_dbproducts.UpdateProfileString(ID, "description", Description);

        }

        private string m_description;
        public string Description { get { return m_description; } set { m_description = value; NotifyPropertyChanged("Description"); } }
       public int ID { get; set; }
    }
}
