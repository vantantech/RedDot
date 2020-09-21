using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace TriPOS.RequestModels
{
   

    /// <summary>
    ///     The request passed in for a card sale
    /// </summary>
    [DataContract(Name = "voidRequest")]
    [XmlRoot("voidRequest", Namespace = "http://tripos.vantiv.com/2014/09/TriPos.Api")]
    public class VoidRequest
    {
        [DataMember(Name = "laneId")]
        [XmlElement("laneId")]
        public int LaneId { get; set; }

        /// <summary>
        ///     The reference number for the transaction
        /// </summary>
        [DataMember(Name = "referenceNumber")]
        [XmlElement("referenceNumber")]
        public string ReferenceNumber { get; set; }


    }
}
