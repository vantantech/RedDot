using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.Models.CardConnect
{
    public class EMV
    {
        public string TVR { get; set; }
        public string PIN { get; set; }
        public string Signature { get; set; }
        public string Mode { get; set; }

        [JsonProperty(PropertyName = "Network Label")]
        public string Network_Label { get; set; }
        public string TSI { get; set; }
        public string AID { get; set; }
        public string IAD { get; set; }

        [JsonProperty(PropertyName = "Entry method")]
        public string Entry_method { get; set; }

        [JsonProperty(PropertyName = "Application Label")]
        public string Application_Label { get; set; }
    }
}
