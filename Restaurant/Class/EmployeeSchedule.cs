using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RedDot
{
   // [DataContract(Name = "employeeSchedule")]
    [XmlRoot("employeeSchedule")]
    public class EmployeeSchedule
    {
        public EmployeeSchedule()
        {
            Monday = new EmployeeShift();
            Tuesday = new EmployeeShift();
            Wednesday = new EmployeeShift();
            Thursday = new EmployeeShift();
            Friday = new EmployeeShift();
            Saturday = new EmployeeShift();
            Sunday = new EmployeeShift();
        }
      //  [DataMember(Name = "monday")]
        [XmlElement("monday")]
        public EmployeeShift Monday { get; set; }

       // [DataMember(Name = "tuesday")]
        [XmlElement("tuesday")]
        public EmployeeShift Tuesday { get; set; }

       // [DataMember(Name = "wednesday")]
        [XmlElement("wednesday")]
        public EmployeeShift Wednesday { get; set; }

      //  [DataMember(Name = "thursday")]
       [XmlElement("thursday")]
        public EmployeeShift Thursday { get; set; }

       // [DataMember(Name = "friday")]
        [XmlElement("friday")]
        public EmployeeShift Friday { get; set; }


       // [DataMember(Name = "saturday")]
        [XmlElement("saturday")]
        public EmployeeShift Saturday { get; set; }


       // [DataMember(Name = "sunday")]
        [XmlElement("sunday")]
        public EmployeeShift Sunday { get; set; }
    }
}
