using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.Class
{
    public class RedDotPrinter:INPCBase
    {

        public int id { get; set; }
        private string m_description;
        public string Description
        {
            get { return m_description; }
            set
            {
                m_description = value;
                NotifyPropertyChanged("Description");

            }
        }

        private string m_assignedprinter;
        public string AssignedPrinter
        {
            get { return m_assignedprinter; }
            set
            {
                m_assignedprinter = value;
                NotifyPropertyChanged("AssignedPrinter");

            }
        }
        private int m_receiptwidth;
        public int ReceiptWidth
        {
            get { return m_receiptwidth; }
            set
            {
                m_receiptwidth = value;
                NotifyPropertyChanged("ReceiptWidth");
            }
        }
        private bool m_islabel;
        public bool IsLabel
        {
            get { return m_islabel; }
            set
            {
                m_islabel = value;
                NotifyPropertyChanged("IsLabel");

            }
        }
        private bool m_landscape;
        public bool LandScape
        {
            get { return m_landscape; }
            set
            {
                m_landscape = value;
                NotifyPropertyChanged("LandScape");

            }
        }
        public decimal width { get; set; }
        public decimal height { get; set; }

        private string m_printmode;
        public string PrintMode
        {
            get { return m_printmode; }
            set
            {
                m_printmode = value;
                NotifyPropertyChanged("PrintMode");

            }
        }
    }
}
