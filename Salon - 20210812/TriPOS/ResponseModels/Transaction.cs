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

namespace TriPOS.ResponseModels
{


    /// <summary>
    /// The properties of a billing and shipping address.
    /// </summary>
    [DataContract(Name = "Transaction")]
    public class Transaction
    {
        /// <summary>
        ///     Gets or sets applicationid.
        /// </summary>
        [DataMember(Name = "ProcessorName")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("ProcessorName")]
        public string ProcessorName { get; set; }

        [DataMember(Name = "ApprovalNumber")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("ApprovalNumber")]
        public string ApprovalNumber { get; set; }

        [DataMember(Name = "TransactionAmount")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("TransactionAmount")]
        public decimal TransactionAmount { get; set; }


        [DataMember(Name = "TransactionStatus")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("TransactionStatus")]
        public string TransactionStatus { get; set; }

        [DataMember(Name = "TransactionStatusCode")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("TransactionStatusCode")]
        public string TransactionStatusCode { get; set; }




    }
}

