using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAccess.Models;

namespace WebAccess.ViewModels
{
    public class Ticket:sale
    {
        public string EmployeeName { get; set; }
        public decimal? Commission { get; set; }
        public decimal? TipTotal { get; set; }
        public IEnumerable<TicketItem> TicketItems { get; set; }
        public IEnumerable<TicketGratuity> TicketTips { get; set; }

        public IEnumerable<TicketPayment> Payments { get; set; }
    }

    public class TicketItem:salesitem
    {
        public string EmployeeName { get; set; }
    }

    public class TicketGratuity:gratuity
    {
        public string EmployeeName { get; set; }
    }

    public class TicketPayment : payment
    {
        public string paymenttype { get; set; }
    }

}