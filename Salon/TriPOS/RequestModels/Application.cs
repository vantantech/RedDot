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
    [DataContract(Name = "Application")]
    public class Application
    {
        /// <summary>
        ///     Gets or sets applicationid.
        /// </summary>
        [DataMember(Name = "ApplicationID")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("ApplicationID")]
        public string ApplicationID { get; set; }

        /// <summary>
        ///     Gets or sets the applicationname
        /// </summary>
        [DataMember(Name = "ApplicationName")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("ApplicationName")]
        public string ApplicationName { get; set; }

        /// <summary>
        ///     Gets or sets the application version
        /// </summary>
        [DataMember(Name = "ApplicationVersion")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("ApplicationVersion")]
        public string ApplicationVersion { get; set; }







    }
}
