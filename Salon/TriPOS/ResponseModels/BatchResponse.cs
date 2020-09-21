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
    public class BatchResponse
    {
        /// <summary>
        ///     Gets or sets applicationid.
        /// </summary>
        [DataMember(Name = "BatchCloseType")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("BatchCloseType")]
        public int BatchCloseType { get; set; } //0=regular , 1=Force

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




        [DataMember(Name = "HostCreditSaleCount")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("HostCreditSaleCount")]
        public int HostCreditSaleCount { get; set; }

        [DataMember(Name = "HostCreditSaleAmount")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("HostCreditSaleAmount")]
        public decimal HostCreditSaleAmount { get; set; }

        [DataMember(Name = "HostCreditReturnCount")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("HostCreditReturnCount")]
        public int HostCreditReturnCount { get; set; }

        [DataMember(Name = "HostCreditReturnAmount")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("HostCreditReturnAmount")]
        public decimal HostCreditReturnAmount { get; set; }


        [DataMember(Name = "HostDebitSaleCount")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("HostDebitSaleCount")]
        public int HostDebitSaleCount { get; set; }

        [DataMember(Name = "HostDebitSaleAmount")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("HostDebitSaleAmount")]
        public decimal HostDebitSaleAmount { get; set; }


        [DataMember(Name = "HostDebitReturnCount")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("HostDebitReturnCount")]
        public int HostDebitReturnCount { get; set; }

        [DataMember(Name = "HostDebitReturnAmount")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("HostDebitReturnAmount")]
        public decimal HostDebitReturnAmount { get; set; }


        [DataMember(Name = "HostBatchCount")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("HostBatchCount")]
        public int HostBatchCount { get; set; }

        [DataMember(Name = "HostBatchnAmount")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("HostBatchAmount")]
        public decimal HostBatchAmount { get; set; }

        [DataMember(Name = "BatchControlNumber")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("BatchControlNumber")]
        public string BatchControlNumber { get; set; }
    }
}
