using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TriPOS.ResponseModels
{
    [Serializable]
    [XmlRoot("TransactionQueryResponse", Namespace = "https://reporting.elementexpress.com")]
    [DataContract(Name = "TransactionQueryResponse")]
    public class TransactionQueryResponse
    {
        /// <summary>
        ///     The cashback amount the cardholder wants
        /// </summary>
        [DataMember(Name = "Response")]
        [XmlElement("Response")]
        public Response ExpResponse { get; set; }
    }
}
