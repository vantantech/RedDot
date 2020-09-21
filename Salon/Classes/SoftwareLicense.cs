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
            LicenseString = LicenseModel.GetLicenseID();
            StartDate = LicenseModel.GetStartDate();
            EndDate = LicenseModel.GetEndDate();
            SoftwareType = LicenseModel.GetSoftwareType();
            SoftwareString = LicenseModel.GetSoftware();

            string fingerprint = LicenseModel.GetMachineID();

            if (LicenseString == fingerprint)
            {
                Message = "Valid license found.";
            }else
            {
                Message = "Valid license not found.";
            }
        }
        public string SoftwareString { get; set; }
        public string LicenseString { get; set; }
        public string SoftwareType { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Message { get; set; }
    }
}
