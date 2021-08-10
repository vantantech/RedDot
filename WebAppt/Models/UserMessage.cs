using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAccess.Models
{
    public class UserMessage
    {

        public UserMessage()
        {
            TimeOut = 500;
        }
        public string CssClassName { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int TimeOut { get; set; }

    }
}