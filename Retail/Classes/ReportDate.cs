using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class ReportDate
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int ReportDateID { get; set; }

        public bool Monthly { get; set; }

        public ReportDate()
        {
            Monthly = false;
        }
        public string ReportString
        {
            get
            {
                if (Monthly)
                {
                    return StartDate.ToString("MMMM yyyy");
                }
                else
                {
                    if (StartDate == DateTime.Today && StartDate == EndDate)
                        //return "Today";
                        return StartDate.ToShortDateString();
                    else return StartDate.ToShortDateString() + " to " + EndDate.ToShortDateString();
                }

            }
        }

        public string DateString
        {
            get
            {
                if(Monthly)
                {
                    return StartDate.ToString("MMMM yyyy");
                }else
                {
                    if (StartDate == DateTime.Today && StartDate == EndDate)
                        //return "Today";
                        return StartDate.ToString("yyyyMMdd");
                    else return StartDate.ToString("yyyyMMdd") + "_" + EndDate.ToString("yyyyMMdd");
                }

            }
        }

    }
}
