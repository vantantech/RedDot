using RedDotBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class SettingsTabItem : INPCBase
    {
        private DataTable m_settings;
        public string Header { get; set; }
        public DataTable Settings
        {
            get { return m_settings; }
            set
            {
                m_settings = value;
                NotifyPropertyChanged("Settings");
            }
        }
    }
}
