using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class SoftwareLicense
    {

        public SoftwareLicense()
        {
            LicenseString = LicenseModel.GetLicense();
            string[] data = LicenseModel.GetPermission().Split(',');
            SoftwareString = data[0];
            if (data.Count() > 1) StartDate = Convert.ToDateTime( data[1]);
            if (data.Count() > 2) EndDate = Convert.ToDateTime(data[2]);
            if (data.Count() > 3) PermissionString = data[3];
        }
        public string SoftwareString { get; set; }
        public string LicenseString { get; set; }
        public string PermissionString { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
