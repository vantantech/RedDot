using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;


namespace RedDot
{
    public class Location
    {
        private DBSettings _dbsettings;

        public Location()
        {
            _dbsettings = new DBSettings();
        }
        public string Name
        {
            get { return _dbsettings.StringGetSetting("Store", "StoreName", "Store Name", "ABC Salon"); }
            set { _dbsettings.SaveSetting("Store", "StoreName", value.ToString()); }
        }
        public string Address1
        {
            get { return _dbsettings.StringGetSetting("Store", "StoreAddress1", "Address 1", ""); }
            set { _dbsettings.SaveSetting("Store", "StoreAddress1", value.ToString()); }
        }
        public string Address2
        {
            get { return _dbsettings.StringGetSetting("Store", "StoreAddress2", "Address 2", ""); }
            set { _dbsettings.SaveSetting("Store", "StoreAddress2", value.ToString()); }
        }

        public string City
        {
            get { return _dbsettings.StringGetSetting("Store", "StoreCity", "City", ""); }
            set { _dbsettings.SaveSetting("Store", "StoreCity", value.ToString()); }
        }
        public string State
        {
            get { return _dbsettings.StringGetSetting("Store", "StoreState", "State", ""); }
            set { _dbsettings.SaveSetting("Store", "StoreState", value.ToString()); }
        }
        public string Zip
        {
            get { return _dbsettings.StringGetSetting("Store", "StoreZip", "Zip", ""); }
            set { _dbsettings.SaveSetting("Store", "StoreZip", value.ToString()); }
        }
        public string Phone
        {
            get { return _dbsettings.StringGetSetting("Store", "StorePhone", "Phone Number", ""); }
            set { _dbsettings.SaveSetting("Store", "StorePhone", value.ToString()); }
        }




    }
}
