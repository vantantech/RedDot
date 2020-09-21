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
using TriPOS.RequestModels;

namespace TriPOS.ResponseModels
{


    /// <summary>
    /// The properties of a billing and shipping address.
    /// </summary>
    [DataContract(Name = "Response")]
    public class Response
    {

        [DataMember(Name = "ExpressResponseCode")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("ExpressResponseCode")]
        public int ExpressResponseCode { get; set; }

        [DataMember(Name = "ExpressResponseMessage")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("ExpressResponseMessage")]
        public string ExpressResponseMessage { get; set; }


        [DataMember(Name = "ExpressTransactionDate")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("ExpressTransactionDate")]
        public string ExpressTransactionDate { get; set; }

        [DataMember(Name = "ExpressTransactionTime")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("ExpressTransactionTime")]
        public string ExpressTransactionTime { get; set; }

        [DataMember(Name = "ExpressTransactionTimezone")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("ExpressTransactionTimezone")]
        public string ExpressTransactionTimezone { get; set; }

        
        [DataMember(Name = "ReportingData")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("ReportingData")]
        public ReportingData ExpReportingData { get; set; }
        



        [DataMember(Name = "Batch")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("Batch")]
        public BatchResponse ExpBatch { get; set; }



        [DataMember(Name = "Transaction")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("Transaction")]
        public Transaction ExpTransaction { get; set; }



    }
}
