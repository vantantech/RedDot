using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TriPOS.ResponseModels
{
    [DataContract(Name = "Item")]
    public class Item
    {

        [DataMember(Name = "TransactionID")]
        [XmlElement("TransactionID")]
        public string TransactionId { get; set; }


        [DataMember(Name = "Name")]
        [XmlElement("Name")]
        public string CardHolderName { get; set; }

        [DataMember(Name = "TerminalID")]
        [XmlElement("TerminalID")]
        public string TerminalId { get; set; }

        [DataMember(Name = "ApprovalNumber")]
        [XmlElement("ApprovalNumber")]
        public string ApprovalNumber { get; set; }

        [DataMember(Name = "ApprovedAmount")]
        [XmlElement("ApprovedAmount")]
        public decimal ApprovedAmount { get; set; }

        [DataMember(Name = "TicketNumber")]
        [XmlElement("TicketNumber")]
        public int TicketNumber { get; set; }

        [DataMember(Name = "TransactionType")]
        [XmlElement("TransactionType")]
        public string TransactionType { get; set; }

        [DataMember(Name = "CardNumberMasked")]
        [XmlElement("CardNumberMasked")]
        public string CardNumberMasked { get; set; }

        /// <summary>
        ///     The card logo (e.g. Visa, Mastercard, etc)
        /// </summary>
        [DataMember(Name = "CardLogo")]
        [XmlElement("CardLogo")]
        public string CardLogo { get; set; }

        [DataMember(Name = "CardType")]
        [XmlElement("CardType")]
        public string CardType { get; set; }

        [DataMember(Name = "TipAmount")]
        [XmlElement("TipAmount")]
        public decimal TipAmount { get; set; }

        [DataMember(Name = "TransactionStatus")]
        [XmlElement("TransactionStatus")]
        public string StatusCode { get; set; }
    }
}
