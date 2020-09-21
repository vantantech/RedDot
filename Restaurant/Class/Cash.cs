using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class CashCount:INPCBase
    {

        private DateTime m_cashdate;
        public DateTime CashDate
        {
            get { return m_cashdate; }
            set
            {
                m_cashdate = value;
                NotifyPropertyChanged("CashDate");
            }

        }

        private int m_Cash100;
        public int Cash100 {
            get { return m_Cash100; }
            set
            {
                m_Cash100 = value;
                NotifyPropertyChanged("Cash100");
                NotifyPropertyChanged("Cash100Val");
                NotifyPropertyChanged("CashTotal");
            }

        }
        public decimal Cash100Val { get { return Cash100 * 100; } }


        private int m_Cash50;
        public int Cash50
        {
            get { return m_Cash50; }
            set
            {
                m_Cash50 = value;
                NotifyPropertyChanged("Cash50");
                NotifyPropertyChanged("Cash50Val");
                NotifyPropertyChanged("CashTotal");
            }

        }
        public decimal Cash50Val { get { return Cash50 * 50; } }


        private int m_Cash20;
        public int Cash20
        {
            get { return m_Cash20; }
            set
            {
                m_Cash20 = value;
                NotifyPropertyChanged("Cash20");
                NotifyPropertyChanged("Cash20Val");
                NotifyPropertyChanged("CashTotal");
            }

        }
        public decimal Cash20Val { get { return Cash20 * 20; } }


        private int m_Cash10;
        public int Cash10
        {
            get { return m_Cash10; }
            set
            {
                m_Cash10 = value;
                NotifyPropertyChanged("Cash10");
                NotifyPropertyChanged("Cash10Val");
                NotifyPropertyChanged("CashTotal");
            }

        }
        public decimal Cash10Val { get { return Cash10 * 10; } }


        private int m_Cash5;
        public int Cash5
        {
            get { return m_Cash5; }
            set
            {
                m_Cash5 = value;
                NotifyPropertyChanged("Cash5");
                NotifyPropertyChanged("Cash5Val");
                NotifyPropertyChanged("CashTotal");
            }

        }
        public decimal Cash5Val { get { return Cash5 * 5; } }


        private int m_Cash2;
        public int Cash2
        {
            get { return m_Cash2; }
            set
            {
                m_Cash2 = value;
                NotifyPropertyChanged("Cash2");
                NotifyPropertyChanged("Cash2Val");
                NotifyPropertyChanged("CashTotal");
            }

        }
        public decimal Cash2Val { get { return Cash2 * 2; } }


        private int m_Cash1;
        public int Cash1
        {
            get { return m_Cash1; }
            set
            {
                m_Cash1 = value;
                NotifyPropertyChanged("Cash1");
                NotifyPropertyChanged("Cash1Val");
                NotifyPropertyChanged("CashTotal");
            }

        }
        public decimal Cash1Val { get { return Cash1 * 1; } }


        private int m_Cash50cent;
        public int Cash50cent
        {
            get { return m_Cash50cent; }
            set
            {
                m_Cash50cent = value;
                NotifyPropertyChanged("Cash50cent");
                NotifyPropertyChanged("Cash50centVal");
                NotifyPropertyChanged("CashTotal");
            }

        }
        public decimal Cash50centVal { get { return Cash50cent * 0.50m; } }

        private int m_Cash25cent;
        public int Cash25cent
        {
            get { return m_Cash25cent; }
            set
            {
                m_Cash25cent = value;
                NotifyPropertyChanged("Cash25cent");
                NotifyPropertyChanged("Cash25centVal");
                NotifyPropertyChanged("CashTotal");
            }

        }
        public decimal Cash25centVal { get { return Cash25cent * 0.25m; } }


        private int m_Cash10cent;
        public int Cash10cent
        {
            get { return m_Cash10cent; }
            set
            {
                m_Cash10cent = value;
                NotifyPropertyChanged("Cash10cent");
                NotifyPropertyChanged("Cash10centVal");
                NotifyPropertyChanged("CashTotal");
            }

        }
        public decimal Cash10centVal { get { return Cash10cent * 0.10m; } }

        private int m_Cash5cent;
        public int Cash5cent
        {
            get { return m_Cash5cent; }
            set
            {
                m_Cash5cent = value;
                NotifyPropertyChanged("Cash5cent");
                NotifyPropertyChanged("Cash5centVal");
                NotifyPropertyChanged("CashTotal");
            }

        }
        public decimal Cash5centVal { get { return Cash5cent * 0.05m; } }

        private int m_Cash1cent;
        public int Cash1cent
        {
            get { return m_Cash1cent; }
            set
            {
                m_Cash1cent = value;
                NotifyPropertyChanged("Cash1cent");
                NotifyPropertyChanged("Cash1centVal");
                NotifyPropertyChanged("CashTotal");
            }

        }
        public decimal Cash1centVal { get { return Cash1cent * 0.01m; } }



        public decimal CashTotal
        {
            get
            {
                decimal total = Cash100Val + Cash50Val + Cash20Val + Cash10Val + Cash5Val + Cash2Val + Cash1Val;
                total += Cash50centVal + Cash25centVal + Cash10centVal + Cash5centVal + Cash1centVal;

                return total;
            }
        }
    }


    public class CashDrawer:INPCBase
    {
        public CashCount CashIn { get; set; }
        public CashCount CashOut { get; set; }
        public int StationNo { get; set; }
        public string Note { get; set; }
        public int ID { get; set; }
    }
}
