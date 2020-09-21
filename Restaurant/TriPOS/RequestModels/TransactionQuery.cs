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
    [Serializable]
    [XmlRoot("TransactionQuery", Namespace = "https://reporting.elementexpress.com")]
    [DataContract(Name = "TransactionQuery")]
    public class TransactionQuery
    {
        /// <summary>
        ///     Credentials
        /// </summary>
        [DataMember(Name = "Credentials")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("Credentials")]
        public Credentials ExpCredentials { get; set; }
        /// <summary>
        ///     Applications
        /// </summary>
        [DataMember(Name = "Application")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("Application")]
        public Application ExpApplication { get; set; }

        /// <summary>
        ///     Applications
        /// </summary>
        [DataMember(Name = "Parameters")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("Parameters")]
        public Parameters ExpParameters { get; set; }

     
    }
}
