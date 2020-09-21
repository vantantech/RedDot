using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class PayDay
    {
        public DateTime PayDate { get; set; }
        public decimal Shift1Hours { get; set; }
        public decimal Shift2Hours { get; set; }
        public decimal Shift3Hours { get; set; }

        public decimal HoursWorked { get { return Shift1Hours + Shift2Hours + Shift3Hours; } }
        public decimal Shift1Tip { get; set; }
        public decimal Shift2Tip { get; set; }
        public decimal Shift3Tip { get; set; }
        public decimal TipAmount {  get { return Shift1Tip + Shift2Tip + Shift3Tip; } }
    }
}
