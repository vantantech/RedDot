using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{

      public enum DisplayMode
        {
            Simple, Modern, Graphical, ThreeD
        };



    public enum AssignmentType
    {
        Ticket , SalesItem
    }

    public enum OrderType
    {
        DineIn, Bar, ToGo, Delivery
    }

    public enum SubOrderType
    {
        WalkIn, CallIn,DriveThru, CounterService, TableService, None
    }

    public enum TicketStatus
    {
        OpenTemp, OpenTablet, Open, Closed, Voided
    }
}
