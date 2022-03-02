using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.Models.CardConnect
{
    public class CCConfirmationRequest
    {
        public string merchantId { get; set; }
        public string hsn { get; set; }
    
        public bool beep { get; set; }
        public string prompt { get; set; }
 
    }
}
