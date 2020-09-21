using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace TriPOS.ResponseModels
{
    [Serializable]
    [XmlRoot("voidResponse", Namespace = "http://tripos.vantiv.com/2014/09/TriPos.Api")]
    [DataContract(Name = "voidResponse")]
    public class VoidResponse
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

        [DataMember(Name = "totalAmount")]
        [XmlElement("totalAmount")]
        public decimal TotalAmount { get; set; }

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


        [DataMember(Name = "_processor")]
        [XmlElement("_processor")]
        public Processor ProcessorResponse { get; set; }


    }
}
