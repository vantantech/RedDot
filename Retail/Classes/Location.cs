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
            get { return _dbsettings.GetStringSetting("StoreName"); }
            set { _dbsettings.SaveSetting("StoreName", value.ToString()); }
        }
        public string Address1
        {
            get { return _dbsettings.GetStringSetting("StoreAddress1"); }
            set { _dbsettings.SaveSetting("StoreAddress1", value.ToString()); }
        }
        public string Address2
        {
            get { return _dbsettings.GetStringSetting("StoreAddress2"); }
            set { _dbsettings.SaveSetting("StoreAddress2", value.ToString()); }
        }

        public string City
        {
            get { return _dbsettings.GetStringSetting("StoreCity"); }
            set { _dbsettings.SaveSetting("StoreCity", value.ToString()); }
        }
        public string State
        {
            get { return _dbsettings.GetStringSetting("StoreState"); }
            set { _dbsettings.SaveSetting("StoreState", value.ToString()); }
        }
        public string Zip
        {
            get { return _dbsettings.GetStringSetting("StoreZip"); }
            set { _dbsettings.SaveSetting("StoreZip", value.ToString()); }
        }
        public string Phone
        {
            get { return _dbsettings.GetStringSetting("StorePhone"); }
            set { _dbsettings.SaveSetting("StorePhone", value.ToString()); }
        }
        public string Type
        {
            get { return _dbsettings.GetStringSetting("StoreType"); }
            set { _dbsettings.SaveSetting("StoreType", value.ToString()); }
        }




    }
}
