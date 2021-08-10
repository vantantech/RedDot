using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.Models.CardConnect
{
    public class CCTipRequest
    {
        public string merchantId { get; set; }
        public string hsn { get; set; }
        public string amount { get; set; }
        public string prompt { get; set; }
        public int[] tipPercentPresets { get; set; }
    }
}
