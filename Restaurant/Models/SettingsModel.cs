using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;

namespace RedDot
{
    public class SettingsModel
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

        public DataTable GetSettingCategories()
        {
            return m_dbsettings.GetSettingCategories();
        }
    }
}
