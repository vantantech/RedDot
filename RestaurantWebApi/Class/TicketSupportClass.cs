using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Web;

namespace RedDot.OrderService.Class
{







  


    public class TicketItem
    {
        public int ID { get; set; }
        public int SeatNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal Weight { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }

    public class NewTicketItem
    {
        public int salesid { get; set; }
        public int productid { get; set; }
    }

}