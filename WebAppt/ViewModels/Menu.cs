using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAccess.Models;

namespace WebAccess.ViewModels
{
    public class Menu
    {
        public string CatDescription { get; set; }
        public int catid { get; set; }
        public IEnumerable<product> Products { get; set; }
    }
}