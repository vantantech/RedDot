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
    /// The properties of credentials
    /// </summary>
    [DataContract(Name = "Credentials")]
    public class Credentials
    {
        /// <summary>
        ///     Gets or sets applicationid.
        /// </summary>
        [DataMember(Name = "AccountID")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("AccountID")]
        public string AccountID { get; set; }

        /// <summary>
        ///     Gets or sets the applicationname
        /// </summary>
        [DataMember(Name = "AccountToken")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("AccountToken")]
        public string AccountToken { get; set; }

        /// <summary>
        ///     Gets or sets the application version
        /// </summary>
        [DataMember(Name = "AcceptorID")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("AcceptorID")]
        public string AcceptorID { get; set; }







    }
}
