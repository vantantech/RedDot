using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManager;
using System.Data;

namespace RedDot
{
    class SettingsModel
    {

        private DBSettings m_dbsettings;
        public SettingsModel()
        {
            m_dbsettings = new DBSettings();
        }

        public DataTable GetSettingsByCategory(string filter)
        {
            return m_dbsettings.GetSettingsbyCategory(filter);
        }
    }
}
