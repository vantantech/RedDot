using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.Models.CardConnect
{
    public class CCCaptureRequest
    {
        public string merchid { get; set; } 
        public string retref { get; set; }
        public string authcode { get; set; }
        public string amount { get; set; }
    }
}
