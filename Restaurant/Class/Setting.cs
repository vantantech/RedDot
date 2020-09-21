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
            m_dbsetting = GlobalSettings.Instance.DBSettings;

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
            switch (Type)
            {

                case "bool":
                    m_dbsetting.BoolSaveSetting(Category,Item, Value.ToUpper() == "TRUE"?true:false);
                    break;
                case "numeric":
                    if (Value == "") break;
                    m_dbsetting.NumericSaveSetting(Category, Item, Decimal.Parse(Value));
                    break;
                case "decimal":
                    if (Value == "") break;
                    m_dbsetting.DecimalSaveSetting(Category, Item, Decimal.Parse(Value));
                    break;
                case "integer":
                    if (Value == "") break;
                    m_dbsetting.IntSaveSetting(Category, Item, int.Parse(Value));
                    break;
                case "image":
                case "list":
                case "color":
                case "printer":
                case "string":
                    m_dbsetting.StringSaveSetting(Category, Item, Value);
                    break;
                


            }
           

        }


    }
}
