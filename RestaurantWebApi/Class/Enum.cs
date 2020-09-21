using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedDot.OrderService.Class
{

    public enum AssignmentType
    {
        Ticket, SalesItem
    }

    public enum OrderType
    {
        DineIn, Bar, ToGo, Delivery
    }

    public enum SubOrderType
    {
        WalkIn, CallIn, CounterService, TableService, None
    }
}