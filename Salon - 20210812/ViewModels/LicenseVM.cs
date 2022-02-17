using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class LicenseVM:INPCBase
    {
        public SoftwareLicense PurchasedLicense { get; set; }
        public LicenseVM()
        {
            PurchasedLicense = new SoftwareLicense();
        }

   


    }
}
