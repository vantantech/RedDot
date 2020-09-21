// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Emv.cs" company="Element Payment Services, Inc., A Vantiv Company">
//   Copyright © 2015 Element Payment Services, Inc., A Vantiv Company. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TriPOS.ResponseModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    ///     Holds the EMV data for the API response/receipt
    /// </summary>
    [DataContract(Name = "emv")]
    public class Emv
    {
        #region Public Properties

        /// <summary>
        ///     Identifies the application as described in ISO/IEC 7816-5
        /// </summary>
        [DataMember(Name = "applicationIdentifier")]
        [XmlElement(ElementName = "applicationIdentifier")]
        [Description("The Application Identifier also known as the AID. Identifies the application as described in ISO/IEC 7816-5. Printed receipts are required to contain the AID as hexadecimal characters.")]
        public string ApplicationIdentifier { get; set; }

        /// <summary>
        ///     Mnemonic associated with the AID according to ISO/IEC 7816-5
        /// </summary>
        [DataMember(Name = "applicationLabel")]
        [XmlElement(ElementName = "applicationLabel")]
        [Description("Mnemonic associated with the AID according to ISO/IEC 7816-5. If the Application Preferred Name is not available or the Issuer code table index is not supported, then the Application Label should be used on the receipt instead of the Application Preferred Name.")]
        public string ApplicationLabel { get; set; }

        /// <summary>
        ///     Preferred mnemonic associated with the AID
        /// </summary>
        [DataMember(Name = "applicationPreferredName")]
        [XmlElement(ElementName = "applicationPreferredName")]
        [Description("Preferred mnemonic associated with the AID. When the Application Preferred Name is present and the Issuer code table index is supported, then this data element is mandatory on the receipt.")]
        public string ApplicationPreferredName { get; set; }

        /// <summary>
        ///     The EMV cryptogram type and value. It is a preferred best practice to include this data element on the receipt, but
        ///     is not mandatory. This field contains cryptogram type followed by the cryptogram value.
        /// </summary>
        [DataMember(Name = "cryptogram")]
        [XmlElement(ElementName = "cryptogram")]
        [Description("The EMV cryptogram type and value. It is a preferred best practice to include this data element on the receipt, but is not mandatory. This field contains cryptogram type followed by the cryptogram value.")]
        public string Cryptogram { get; set; }

        /// <summary>
        ///     A name value collection of additional EMV tags that are required to appear on the receipt.
        /// </summary>
        [DataMember(Name = "tags")]
        [XmlArray(ElementName = "tags")]
        [XmlArrayItem(ElementName = "tag")]
        [Description("A name value collection of additional EMV tags that are required to appear on the receipt.")]
        public List<Tag> Tags { get; set; }

        /// <summary>
        ///     Indicates the code table according to ISO/IEC 8859 for displaying the Application Preferred Name
        /// </summary>
        [DataMember(Name = "issuerCodeTableIndex")]
        [XmlElement(ElementName = "issuerCodeTableIndex")]
        [Description("Indicates the code table according to ISO/IEC 8859 for displaying the Application Preferred Name.")]
        public string IssuerCodeTableIndex { get; set; }

        #endregion
    }
}