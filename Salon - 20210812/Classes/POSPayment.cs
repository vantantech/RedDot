using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class POSPayment : POSExchange
    {
        public String ExternalID { get; set; }
        public enum Status
        {
            PAID, VOIDED, REFUNDED, AUTHORIZED
        }
        public POSPayment(string paymentID, string externalID, string orderID, string employeeID, long amount, long tip = 0, long cashBack = 0) : base(paymentID, orderID, employeeID, amount)
        {
            TipAmount = tip;
            CashBackAmount = cashBack;
            OrderID = orderID;
            EmployeeID = employeeID;
            ExternalID = externalID;
        }

        private Status _status;

        public Status PaymentStatus
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status != value)
                {
                    _status = value;
                }
            }
        }
        public bool Voided
        {
            get
            {
                return _status == Status.VOIDED;
            }
        }
        public bool Refunded
        {
            get
            {
                return _status == Status.REFUNDED;
            }
        }

        public long TipAmount { get; set; }
        public long CashBackAmount { get; set; }
    }


    public class POSExchange
    {
        public POSExchange(string paymentID, string orderID, string employeeID, long amount)
        {
            PaymentID = paymentID;
            OrderID = orderID;
            EmployeeID = employeeID;
            Amount = amount;
        }

        public string PaymentID { get; set; }
        public string OrderID { get; set; }
        public string EmployeeID { get; set; }
        public long Amount { get; set; }
    }
}
