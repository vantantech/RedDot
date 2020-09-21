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
     [DataContract(Name = "Terminal")]
    public class Terminal
    {
        /// <summary>
        ///     Gets or sets applicationid.
        /// </summary>
        [DataMember(Name = "TerminalID")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("TerminalID")]
        public string TerminalID { get; set; }


 



    }
}
