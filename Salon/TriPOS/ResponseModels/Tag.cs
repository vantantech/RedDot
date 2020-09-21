// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tag.cs" company="Element Payment Services, Inc., A Vantiv Company">
//   Copyright © 2015 Element Payment Services, Inc., A Vantiv Company. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TriPOS.ResponseModels
{
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [DataContract(Name = "tag")]
    public class Tag
    {
        [DataMember(Name = "key")]
        [XmlElement(ElementName = "key")]
        public string Key { get; set; }

        [DataMember(Name = "value")]
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }
}
