// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiLink.cs" company="Element Payment Services, Inc., A Vantiv Company">
//   Copyright © 2015 Element Payment Services, Inc., A Vantiv Company. All Rights Reserved.
// </copyright>
// <summary>
//   A hypermedia link
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TriPOS.ResponseModels
{
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    ///     A hypermedia link
    /// </summary>
    [DataContract(Name = "link")]
    public class ApiLink
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the href.
        /// </summary>
        [DataMember(Name = "href")]
        [XmlElement("href")]
        public string Href { get; set; }

        /// <summary>
        ///     Gets or sets the method.
        /// </summary>
        [DataMember(Name = "method")]
        [XmlElement("method")]
        public string Method { get; set; }

        /// <summary>
        ///     Gets or sets the relation.
        /// </summary>
        [DataMember(Name = "rel")]
        [XmlElement("rel")]
        public string Relation { get; set; }

        #endregion
    }
}