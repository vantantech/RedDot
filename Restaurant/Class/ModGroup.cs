using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;

namespace RedDot
{
    public class ModGroup : INPCBase
    {
        public int ID { get; set; }
        private int m_profileid;
        public int ProfileID { get { return m_profileid; } set { m_profileid = value; NotifyPropertyChanged("ProfileID"); } }
        private string m_description;
   
        private int m_min;
        private int m_max;
        private bool m_allowduplicate;

        private ObservableCollection<Modifier> m_modifiers;
        public ObservableCollection<Modifier> Modifiers
        {
            get
            {
                return m_modifiers;
            }
            set
            {
                m_modifiers = value;
                NotifyPropertyChanged("Modifiers");
            }
        }

        private DBProducts m_dbproducts;

        public ModGroup(DataRow row)
        {
            
            InitData(row);
        }


        public ModGroup()
        {
            if (m_dbproducts == null) m_dbproducts = new DBProducts();

            ID = 0;
            ProfileID = 0;
            m_description = "";
            m_min = 0;
            m_max = 999999;
            m_sortorder = 0;
            m_allowduplicate = true;
        }



        private void InitData(DataRow row)
        {
            if (m_dbproducts == null) m_dbproducts = new DBProducts();

            if (row["modgroupid"].ToString() != "") ID = int.Parse(row["modgroupid"].ToString());
            else ID = 0;
            if (row["modprofileid"].ToString() != "") ProfileID = int.Parse(row["modprofileid"].ToString());
            else ProfileID = 0;

            m_description = row["description"].ToString();
      
            if (row["min"].ToString() != "") m_min = int.Parse(row["min"].ToString());
            else m_min = 0;

            if (row["max"].ToString() != "") m_max = int.Parse(row["max"].ToString());
            else m_max= 0;

            if (row["sortorder"].ToString() != "") m_sortorder = int.Parse(row["sortorder"].ToString());
            else m_sortorder = 0;

           m_allowduplicate = (row["allowduplicate"].ToString() == "1");

            Editable = true;
        }

        public Modifier FindModifier(int id)
        {
            foreach(Modifier mod in Modifiers)
            {
                if (mod.ID == id) return mod;
            }
            return null;
        }

        public string Description
        { get { return m_description; }
            set {
                m_description = value;
                m_dbproducts.UpdateModGroupString(ID, "description", value);
                NotifyPropertyChanged("Description");
            }
        }

    

        private int m_sortorder;
        public int SortOrder
        {
            get { return m_sortorder; }
            set
            {
                m_sortorder = value;
                m_dbproducts.UpdateModGroupInt(ProfileID, ID, "sortorder", value);
                NotifyPropertyChanged("SortOrder");
            }
        }

        public int Min
        {
            get { return m_min; }

            set
            {
                m_min = value;
                m_dbproducts.UpdateModGroupInt(ProfileID, ID, "min", value);
                NotifyPropertyChanged("Min");
            }

        }
        public int Max
        {
            get { return m_max; }

            set
            {
                if (value > 0)
                    m_max = value;
                else
                    m_max = 1; //do not allow any number less than 1

                m_dbproducts.UpdateModGroupInt(ProfileID, ID, "max", m_max);
                NotifyPropertyChanged("Max");
            }

        }

        public bool AllowDuplicate
        {
            get
            {
                return m_allowduplicate;
            }

            set
            {
                m_allowduplicate = value;
                m_dbproducts.UpdateModGroupInt(ProfileID, ID, "allowduplicate", (value ? 1 : 0));
                NotifyPropertyChanged("AllowDuplicate");
            }
        }

        public bool Editable { get; set; }
      

    }
}
