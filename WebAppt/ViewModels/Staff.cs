using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAccess.Models;

namespace WebAccess.ViewModels
{
    public class Staff
    {
        public string firstletter { get; set; }
        public IEnumerable<employee> Employees { get; set; }
    }
}