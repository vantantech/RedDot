// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiWarning.cs" company="Element Payment Services, Inc., A Vantiv Company">
//   Copyright © 2015 Element Payment Services, Inc., A Vantiv Company. All Rights Reserved.
// </copyright>
// <summary>
//   Warnings that occurred during the request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TriPOS.ResponseModels
{
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    ///     Warnings that occurred during the request.
    /// </summary>
    [DataContract(Name = "warning")]
    public class ApiWarning
    {
        #region Public Properties

        /// <summary>
        ///     A developerMessage of the error
        /// </summary>
        [DataMember(Name = "developerMessage")]
        [XmlElement("developerMessage")]
        public string DeveloperMessage { get; set; }

        /// <summary>
        ///     A developerMessage of the error
        /// </summary>
        [DataMember(Name = "userMessage")]
        [XmlElement("userMessage")]
        public string UserMessage { get; set; }

        #endregion
    }
}