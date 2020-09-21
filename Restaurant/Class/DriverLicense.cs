using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DriverLicense
    {

        public string LicenseNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public DateTime DOB { get; set; }
        public DateTime RecordedDate { get; set; }
        public decimal Age {
            get {

                if (DOB == DateTime.MinValue) return 0;
                if (RecordedDate == DateTime.MinValue)
                {
                    TimeSpan tp = (DateTime.Now - DOB);
                    return (decimal) Math.Round(tp.TotalDays / 365.25,2);
                }
                else
                {
                    TimeSpan tp = (RecordedDate - DOB);
                    return (decimal) Math.Round(tp.TotalDays / 365.25,2);
                }

               
            }
        }

        public string FullName { get { return FirstName + " " + LastName; } }

        public DriverLicense()
        {

        }

        public DriverLicense(DataRow row)
        {
            LicenseNo = row["driverslicense"].ToString();
            FirstName = row["firstname"].ToString();
            LastName = row["lastname"].ToString();
            if(row["dob"].ToString() != "")DOB = (DateTime)row["dob"];
            if(row["datecreated"].ToString() != "") RecordedDate = (DateTime)row["datecreated"];

        }
    }
}
