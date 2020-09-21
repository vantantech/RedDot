using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class SalesReport:INPCBase
    {
        private DataTable m_sales;
        public DataTable Sales
        {
            get { return m_sales; }
            set
            {
                m_sales = value;



                decimal tiptotal = 0;
                decimal saletotal = 0;
                decimal totalticket = 0;
                decimal totalcash = 0;
                decimal totalcredit = 0;
                decimal totaldebit = 0;
                decimal totalother = 0;

                foreach (DataRow row in m_sales.Rows)
                {
                    string tipstr = row["tipamount"].ToString();
                    if (tipstr != "") tiptotal += decimal.Parse(tipstr);

                    string totalstr = row["total"].ToString();
                    if (totalstr != "") saletotal += decimal.Parse(totalstr);

                    string cashstr = row["cash"].ToString();
                    if (cashstr != "") totalcash += decimal.Parse(cashstr);

                    string creditstr = row["credit"].ToString();
                    if (creditstr != "") totalcredit += decimal.Parse(creditstr);

                    string debitstr = row["debit"].ToString();
                    if (debitstr != "") totaldebit += decimal.Parse(debitstr);

                    string otherstr = row["other"].ToString();
                    if (otherstr != "") totalother += decimal.Parse(otherstr);

                    string totalticketstr = row["totalticket"].ToString();
                    if (totalticketstr != "") totalticket += decimal.Parse(totalticketstr);

                  
                }
                TotalTips = tiptotal;
                TotalSales = saletotal;
                TotalTickets = totalticket;

                TotalCash = totalcash;
                TotalCredit = totalcredit;
                TotalDebit = totaldebit;
                TotalOther = totalother;



                NotifyPropertyChanged("Sales");
            }
        }


        private decimal _totaltips;
        public decimal TotalTips
        {
            get { return _totaltips; }
            set
            {
                _totaltips = value;
                NotifyPropertyChanged("TotalTips");
            }
        }


        private decimal _totalsales;
        public decimal TotalSales
        {
            get { return _totalsales; }
            set
            {
                _totalsales = value;
                NotifyPropertyChanged("TotalSales");
            }
        }

        private decimal _totaltickets;
        public decimal TotalTickets
        {
            get { return _totaltickets; }
            set
            {
                _totaltickets = value;
                NotifyPropertyChanged("TotalTickets");
            }
        }

        private decimal _totalcash;
        public decimal TotalCash
        {
            get { return _totalcash; }
            set
            {
                _totalcash = value;
                NotifyPropertyChanged("TotalCash");
            }
        }

        private decimal _totalcredit;
        public decimal TotalCredit
        {
            get { return _totalcredit; }
            set
            {
                _totalcredit = value;
                NotifyPropertyChanged("TotalCredit");
            }
        }

        private decimal _totaldebit;
        public decimal TotalDebit
        {
            get { return _totaldebit; }
            set
            {
                _totaldebit = value;
                NotifyPropertyChanged("TotalDebit");
            }
        }

        private decimal _totalother;
        public decimal TotalOther
        {
            get { return _totalother; }
            set
            {
                _totalother = value;
                NotifyPropertyChanged("TotalOther");
            }
        }

    }
}
