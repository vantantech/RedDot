using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriPOS.ResponseModels
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot("rebootLaneResponse", Namespace = "http://tripos.vantiv.com/2014/09/TriPos.Api")]
    [DataContract(Name = "rebootLaneResponse")]
    public class RebootLaneResponse
    {
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

        /// <summary>
        ///     Gets or sets the warnings.
        /// </summary>
        [DataMember(Name = "_warnings")]
        [XmlArray(ElementName = "_warnings")]
        [XmlArrayItem(ElementName = "warning")]
        public List<ApiWarning> Warnings { get; set; }



    }
}
