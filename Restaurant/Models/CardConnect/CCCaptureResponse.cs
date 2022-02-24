using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.Models.CardConnect
{
    public class CCCaptureResponse
    {
        public string merchid { get; set; }    
        public string account { get; set; }
        public string orderId { get; set; }
        public string amount { get; set; }
        public string retref { get; set; }
        public string batchid { get; set; }
        public string setlstat { get; set; }
        public string respcode { get; set; }
        public string resptext { get; set; }
    }
}
