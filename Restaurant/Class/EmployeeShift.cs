using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RedDot
{
    public class EmployeeShift
    {
        public EmployeeShift()
        {
            Shift1 = new ScheduleTime();
            Shift2 = new ScheduleTime();
            Shift3 = new ScheduleTime();
        }


       // [DataMember(Name = "shift1")]
        [XmlElement("shift1")]
        public ScheduleTime Shift1 { get; set; }


       // [DataMember(Name = "shift2")]
        [XmlElement("shift2")]
        public ScheduleTime Shift2 { get; set; }


       // [DataMember(Name = "shift3")]
        [XmlElement("shift3")]
        public ScheduleTime Shift3 { get; set; }
    }


   
}
