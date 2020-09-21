using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RedDot.OrderService.Class
{
    public class Category
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string ColorCode { get; set; }

        public Category(DataRow row)
        {
            ID = (int)row["id"];
            Description = row["description"].ToString();
            ColorCode = row["colorcode"].ToString();
        }
    }


  
}