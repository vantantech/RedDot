using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;


namespace RedDot.DataManager
{
    public class Location
    {
        private MySQLDBSettings _dbsettings;

        public Location()
        {
            _dbsettings = new MySQLDBSettings();
        }
        public string Name
        {
            get { return _dbsettings.StringGetSetting("Store","StoreName","Store Name","ABC Salon"); }
            set { _dbsettings.SaveSetting("StoreName", value.ToString()); }
        }
        public string Address1
        {
            get { return _dbsettings.StringGetSetting("Store", "StoreAddress1","Address 1",""); }
            set { _dbsettings.SaveSetting("StoreAddress1", value.ToString()); }
        }
        public string Address2
        {
            get { return _dbsettings.StringGetSetting("Store", "StoreAddress2","Address 2",""); }
            set { _dbsettings.SaveSetting("StoreAddress2", value.ToString()); }
        }

        public string City
        {
            get { return _dbsettings.StringGetSetting("Store", "StoreCity", "City",""); }
            set { _dbsettings.SaveSetting("StoreCity", value.ToString()); }
        }
        public string State
        {
            get { return _dbsettings.StringGetSetting("Store", "StoreState","State",""); }
            set { _dbsettings.SaveSetting("StoreState", value.ToString()); }
        }
        public string Zip
        {
            get { return _dbsettings.StringGetSetting("Store", "StoreZip","Zip",""); }
            set { _dbsettings.SaveSetting("StoreZip", value.ToString()); }
        }
        public string Phone
        {
            get { return _dbsettings.StringGetSetting("Store", "StorePhone","Phone Number",""); }
            set { _dbsettings.SaveSetting("StorePhone", value.ToString()); }
        }
  




    }
}
