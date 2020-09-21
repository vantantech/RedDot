using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class TimeSlot
    {

        public TimeSlot(DateTime time, int interval)
        {


            int numofinterval = 60 / interval;
            Intervals = new List<TimeString>();
            Intervals.Add(new TimeString() { Time = time.ToShortTimeString() });
            for(int i=1; i < numofinterval;i++)
            {
                Intervals.Add(new TimeString() { Time =":" +  (interval * i).ToString() });
            }
        }
        
        public List<TimeString> Intervals{get;set;}
    }

    public class TimeString
    {
        public string Time { get; set; }
    }
}
