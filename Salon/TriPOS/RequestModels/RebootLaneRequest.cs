using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriPOS.RequestModels
{
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [DataContract(Name = "rebootLaneRequest")]
    [XmlRoot("rebootLaneRequest", Namespace = "http://tripos.vantiv.com/2014/09/TriPos.Api")]
    public class RebootLaneRequest
    {
        [DataMember(Name = "laneId")]
        [XmlElement("laneId")]
        public int LaneId { get; set; }

    }
}
