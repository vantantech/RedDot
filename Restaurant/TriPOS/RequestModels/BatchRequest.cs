// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Address.cs" company="Element Payment Services, Inc., A Vantiv Company">
//   Copyright © 2015 Element Payment Services, Inc., A Vantiv Company. All Rights Reserved.
// </copyright>
// <summary>
//   The properties of a billing and shipping address.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System.Runtime.Serialization;
using System.Xml.Serialization;

using Newtonsoft.Json;

namespace TriPOS.RequestModels
{


    /// <summary>
    /// The properties of a billing and shipping address.
    /// </summary>
    [DataContract(Name = "Batch")]
    public class BatchRequest
    {
        /// <summary>
        ///     Gets or sets applicationid.
        /// </summary>
 

        [DataMember(Name = "BatchQueryType")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("BatchQueryType")]
        public int BatchQueryType { get; set; } //0=Total , 1 = Item



        [DataMember(Name = "HostBatchID")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("HostBatchID")]
        public string HostBatchID { get; set; }

        [DataMember(Name = "HostItemID")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("HostItemID")]
        public string HostItemID { get; set; }



    }
}
