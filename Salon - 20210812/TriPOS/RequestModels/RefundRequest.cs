using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TriPOS.RequestModels
{
    [DataContract(Name = "refundRequest")]
    [XmlRoot("refundRequest", Namespace = "http://tripos.vantiv.com/2014/09/TriPos.Api")]
    public class RefundRequest
    {

        /// <summary>
        ///     Required. Specifies which lane to use for the card sale.
        /// </summary>
        [DataMember(Name = "laneId")]
        [XmlElement("laneId")]
        public int LaneId { get; set; }

        /// <summary>
        ///     The amount of the transaction
        /// </summary>
        [DataMember(Name = "transactionAmount")]
        [XmlElement("transactionAmount")]
        public decimal TransactionAmount { get; set; }

        /// <summary>
        ///     The reference number for the transaction
        /// </summary>
        [DataMember(Name = "referenceNumber")]
        [XmlElement("referenceNumber")]
        public string ReferenceNumber { get; set; }

    }
}
