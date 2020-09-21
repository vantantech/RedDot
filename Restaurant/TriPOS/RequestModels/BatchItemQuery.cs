// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Address.cs" company="Element Payment Services, Inc., A Vantiv Company">
//   Copyright © 2015 Element Payment Services, Inc., A Vantiv Company. All Rights Reserved.
// </copyright>
// <summary>
//   The properties of a billing and shipping address.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using Newtonsoft.Json;

namespace TriPOS.RequestModels
{


    /// <summary>
    ///     Batch close command
    /// </summary>
    [Serializable]
    [XmlRoot("BatchItemQuery", Namespace = "https://transaction.elementexpress.com")]
    [DataContract(Name = "BatchItemQuery")]
    public class BatchItemQuery
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
        ///     Gets or sets the name of the city used for billing purposes.
        /// </summary>
        [DataMember(Name = "Terminal")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("Terminal")]
        public Terminal ExpTerminal { get; set; }

        /// <summary>
        ///     Gets or sets the e-mail address used for billing purposes.
        /// </summary>
        [DataMember(Name = "Batch")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("Batch")]
        public BatchRequest ExpBatch { get; set; }


    }
}
