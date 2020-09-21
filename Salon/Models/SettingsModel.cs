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
    class SettingsModel
    {

        private IDataInterface m_dbsettings;
        public SettingsModel()
        {
            m_dbsettings = GlobalSettings.Instance.RedDotData;
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
