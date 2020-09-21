// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaleResponse.cs" company="Element Payment Services, Inc., A Vantiv Company">
//   Copyright © 2015 Element Payment Services, Inc., A Vantiv Company. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace TriPOS.ResponseModels
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using TriPOS.RequestModels;

    /// <summary>
    ///     The response returned from a card sale request
    /// </summary>
    [Serializable]
    [XmlRoot("BatchCloseResponse", Namespace = "https://transaction.elementexpress.com")]
    [DataContract(Name = "BatchCloseResponse")]
    public class BatchCloseResponse
    {
        #region Public Properties

        /// <summary>
        ///     The cashback amount the cardholder wants
        /// </summary>
        [DataMember(Name = "Response")]
        [XmlElement("Response")]
        public Response ExpResponse { get; set; }

       

        #endregion
    }
}