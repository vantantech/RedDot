using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace TriPOS.ResponseModels
{
    [Serializable]
    [XmlRoot("refundResponse", Namespace = "http://tripos.vantiv.com/2014/09/TriPos.Api")]
    [DataContract(Name = "refundResponse")]
    public class RefundResponse
    {
        [DataMember(Name = "approvalNumber")]
        [XmlElement("approvalNumber")]
        public string ApprovalNumber { get; set; }

        [DataMember(Name = "isApproved")]
        [XmlElement("isApproved")]
        public bool IsApproved { get; set; }

        [DataMember(Name = "statusCode")]
        [XmlElement("statusCode")]
        public string StatusCode { get; set; }

        /// <summary>
        ///     The total amount of the transaction
        /// </summary>
        [DataMember(Name = "totalAmount")]
        [XmlElement("totalAmount")]
        public decimal TotalAmount { get; set; }


        /// <summary>
        ///     The card account number
        /// </summary>
        [DataMember(Name = "accountNumber")]
        [XmlElement("accountNumber")]
        public string AccountNumber { get; set; }

        /// <summary>
        ///     The cardholder name
        /// </summary>
        [DataMember(Name = "cardHolderName")]
        [XmlElement("cardHolderName")]
        public string CardHolderName { get; set; }

        /// <summary>
        ///     The card logo (e.g. Visa, Mastercard, etc)
        /// </summary>
        [DataMember(Name = "cardLogo")]
        [XmlElement("cardLogo")]
        public string CardLogo { get; set; }

        /// <summary>
        ///     Description of how card was entered: Keyed, Swiped, Chip
        /// </summary>
        [DataMember(Name = "entryMode")]
        [XmlElement("entryMode")]
        public string EntryMode { get; set; }

        /// <summary>
        ///     Description of how card payment type: None, Credit, Debit
        /// </summary>
        [DataMember(Name = "paymentType")]
        [XmlElement("paymentType")]
        public string PaymentType { get; set; }


        /// <summary>
        ///     Gets or sets whether a PIN was verified for this transaction.
        /// </summary>
        [DataMember(Name = "pinVerified")]
        [XmlElement("pinVerified")]
        public bool PinVerified { get; set; }


        /// <summary>
        ///     The transaction ID from the processor
        /// </summary>
        [DataMember(Name = "transactionId")]
        [XmlElement("transactionId")]
        public string TransactionId { get; set; }



        [DataMember(Name = "_errors")]
        [XmlArray(ElementName = "_errors")]
        [XmlArrayItem(ElementName = "error")]
        public List<ApiError> Errors { get; set; }

        /// <summary>
        ///     Indicates if there are errors
        /// </summary>
        [DataMember(Name = "_hasErrors")]
        [XmlElement("_hasErrors")]
        public bool HasErrors { get; set; }



        /// <summary>
        ///     Gets or sets the links.
        /// </summary>
        [DataMember(Name = "_links")]
        [XmlArray(ElementName = "_links")]
        [XmlArrayItem(ElementName = "link")]
        public List<ApiLink> Links { get; set; }


        [DataMember(Name = "_logs")]
        [XmlArray(ElementName = "_logs")]
        [XmlArrayItem(ElementName = "log")]
        public List<string> Logs { get; set; }

        /// <summary>
        ///     The type of object held in the result, if any
        /// </summary>
        [DataMember(Name = "_type")]
        [XmlElement("_type")]
        public string Type { get; set; }

        [DataMember(Name = "_warnings")]
        [XmlArray(ElementName = "_warnings")]
        [XmlArrayItem(ElementName = "warning")]
        public List<ApiWarning> Warnings { get; set; }

        /// <summary>
        /// The fields used on the receipt for an EMV transaction. Null if the transaction was not EMV.
        /// </summary>
        [DataMember(Name = "emv")]
        [XmlElement(ElementName = "emv")]
        public Emv Emv { get; set; }

        /// <summary>
        ///     Response information from the processor
        /// </summary>
        [DataMember(Name = "_processor")]
        [XmlElement("_processor")]
        public Processor ProcessorResponse { get; set; }

        /// <summary>
        ///     The signature data
        /// </summary>
        [DataMember(Name = "signature")]
        [XmlElement("signature")]
        public Signature Signature { get; set; }

    }
}
