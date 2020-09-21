using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;
using RedDot.DataManager;

namespace RedDot
{
    public class Setting:INPCBase
    {
        public string Item { get; set; }
        public string Description { get; set; }

        public string Category { get; set; }
        public string Type { get; set; }
        public string TypeData { get; set; }

        IDataInterface m_dbsetting;
        
        public Setting(int id)
        {
            DataTable dt;
            m_dbsetting = GlobalSettings.Instance.RedDotData;

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

                case "image":
                    m_dbsetting.StringSaveSetting(Item, Value);
                    break;


                case "bool":

                    m_dbsetting.BoolSaveSetting(Item, Value.ToUpper() == "TRUE" ? true : false);
                    break;

                case "string":
                    m_dbsetting.StringSaveSetting(Item, Value);
                    break;

                case "numeric":
                    if (Value == "") break;
                    m_dbsetting.DecimalSaveSetting(Item, Decimal.Parse(Value));
                    break;

                case "decimal":
                    if (Value == "") break;
                    m_dbsetting.DecimalSaveSetting(Item, Decimal.Parse(Value));
                    break;
                case "integer":
                    if (Value == "") break;
                    m_dbsetting.IntSaveSetting(Item, int.Parse(Value));
                    break;
                case "list":
                    m_dbsetting.StringSaveSetting(Item, Value);
                    break;

                case "color":
                    m_dbsetting.StringSaveSetting(Item, Value);
                    break;

            }

        }


    }
}
