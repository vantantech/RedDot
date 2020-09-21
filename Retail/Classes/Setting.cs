using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;

namespace RedDot
{
    public class Setting:INPCBase
    {
        public string Item { get; set; }
        public string Description { get; set; }

        public string Category { get; set; }
        public string Type { get; set; }
        public string TypeData { get; set; }

        DBSettings m_dbsetting;
        
        public Setting(int id)
        {
            DataTable dt;
            m_dbsetting = new DBSettings();

            dt = m_dbsetting.GetSettingbyID(id);
            if(dt.Rows.Count > 0)
            {

                DataRow row = dt.Rows[0];

                Item        = row["item"].ToString();
                Description = row["description"].ToString();
                Value       = row["value"].ToString();
                Category    = row["category"].ToString();
                Type        = row["type"].ToString();
                TypeData    = row["typedata"].ToString();



            }



        }

        private string m_value;
        public string Value {
            get { return m_value; }

            set { m_value = value;
            NotifyPropertyChanged("Value");
           
            }
        }


        public void Save()
        {
            m_dbsetting.SaveSetting(Item, Value);

        }


    }
}
