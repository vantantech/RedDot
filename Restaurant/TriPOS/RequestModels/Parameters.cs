using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TriPOS.RequestModels
{
    [DataContract(Name="Parameters")]
    public class Parameters
    {
        [DataMember(Name = "ReferenceNumber")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("ReferenceNumber")]
        public string ReferenceNumber { get; set; }


    }
}
