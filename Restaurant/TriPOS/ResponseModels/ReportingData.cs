using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TriPOS.ResponseModels
{
    [DataContract(Name = "ReportingData")]
    public class ReportingData
    {

        [DataMember(Name = "Items")]
        [XmlArray(ElementName = "Items")]
        [XmlArrayItem(ElementName = "Item")]
        public List<Item> Items { get; set; }
    }
}
