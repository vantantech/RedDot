using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RedDot
{
    public class ScheduleTime
    {
        public ScheduleTime()
        {
            StartTime = new DateTime();
            EndTime = new DateTime();
        }
       // [DataMember(Name = "startTime")]
       // [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("startTime")]
        public DateTime StartTime { get; set; }

        [XmlIgnore]
        public string StartTimeStr
        {
            get
            {
                return StartTime.ToShortTimeString();
            }
            set
            {
                string temp = value;
                try
                {
                    StartTime = DateTime.Parse(temp);
                }catch
                {
                }
            }
        }

       // [DataMember(Name = "endTime")]
       // [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("endTime")]
        public DateTime EndTime { get; set; }

        [XmlIgnore]
        public string EndTimeStr
        {
            get
            {
                return EndTime.ToShortTimeString();
            }
            set
            {
                string temp = value;
                try
                {
                    EndTime = DateTime.Parse(temp);
                }
                catch
                {
                }
            }
        }
    }
}
