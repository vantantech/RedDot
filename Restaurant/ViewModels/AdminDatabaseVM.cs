using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot.ViewModels
{
    public class AdminDatabaseVM:INPCBase
    {

        public ICommand RefreshData { get; set; }
        public ICommand CustomClicked { get; set; }
        public ICommand AutoSelect { get; set; }
        public ICommand SelectClicked { get; set; }
        public ICommand ArchiveTickets { get; set; }
        public ICommand PaymentSelect { get; set; }

        private DBTicket m_dbTicket;
        private int paymentselect = 1;

        private DBConnect dbConnect;
        public AdminDatabaseVM()
        {
            RefreshData = new RelayCommand(ExecuteRefreshData, param => true);
            CustomClicked = new RelayCommand(ExecuteCustomClicked, param => true);
            AutoSelect = new RelayCommand(ExecuteAutoSelect, param => true);
            SelectClicked = new RelayCommand(ExecuteSelectClicked, param => true);
            ArchiveTickets = new RelayCommand(ExecuteArchiveTickets, param => true);
            PaymentSelect = new RelayCommand(ExecutePaymentSelect, param => true);
            dbConnect = new DBConnect();
            m_dbTicket = new DBTicket();
            StartDate = DateTime.Today;
            EndDate = StartDate;
           
        }


        public void ExecuteRefreshData(object obj)
        {
            Tickets = LoadData(StartDate, EndDate);

            Totalize();

        }
        public DataTable LoadData( DateTime startdate, DateTime enddate)
        {
            string filter = "";

            if (paymentselect == 2) filter = " sub.other = 0 and ";
            if (paymentselect == 3) filter = " sub.cash = 0 and ";

            string sql = "select 0 as selected, sub.* from (select sales.*,sum(if (payment.cardgroup = 'cash',payment.netamount,0)) as cash, sum(if (payment.cardgroup != 'cash',payment.netamount,0)) as other from sales inner join payment on sales.id = payment.salesid group by sales.id ) as sub where " + filter + " status = 'Closed' and  saledate between '" + startdate.ToString("yyyy-MM-dd 00:00") + "' and '" + enddate.ToString("yyyy-MM-dd 23:59") + "' ";
            DataTable data = dbConnect.GetData(sql);

       

            return data;
        }

        public void Totalize()
        {
            decimal total = 0;
            decimal selected = 0;

            foreach (DataRow row in Tickets.Rows)
            {
                total += decimal.Parse(row["total"].ToString());
                if(row["selected"].ToString() == "1")
                {
                    selected += decimal.Parse(row["total"].ToString());
                }
            }

            Total = total;
            SelectedTotal = selected;
        }

        public void ExecutePaymentSelect(object obj)
        {
            string sel = obj.ToString();
            paymentselect = int.Parse(sel);

            //refresh
            Tickets = LoadData(StartDate, EndDate);

            Totalize();

        }
        public void ExecuteSelectClicked(object obj)
        {
           

            Totalize();
            NotifyPropertyChanged("Tickets");
        }
        public void ExecuteAutoSelect(object obj)
        {
            bool toggle = false;
            foreach (DataRow row in Tickets.Rows)
            {
                if (toggle)
                {
                    row["selected"] = 1;
                    toggle = !(toggle);
                }
                else
                    toggle = true;
               
            }

            Totalize();
            NotifyPropertyChanged("Tickets");
        }


        public void ExecuteArchiveTickets(object obj)
        {
            foreach (DataRow row in Tickets.Rows)
            {
                total += decimal.Parse(row["total"].ToString());
                if (row["selected"].ToString() == "1")
                {
                    //archive
                    int m_salesid = int.Parse(row["id"].ToString());
                    m_dbTicket.DBVoidTicket(m_salesid, "archive");
                }
            }


            //refresh
            Tickets = LoadData(StartDate, EndDate);

            Totalize();
        }

        public void ExecuteCustomClicked(object salesid)
        {

            try
            {
                CustomDate cd = new CustomDate(Visibility.Visible);
                cd.Topmost = true;
                cd.ShowDialog();


                StartDate = cd.StartDate;
                EndDate = cd.EndDate;


                Tickets = LoadData(StartDate, EndDate);

                Totalize();
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteCustomClicked: " + e.Message);
            }
        }

        private DataTable _tickets;
        public DataTable Tickets
        {
            get { return _tickets; }
            set
            {
                _tickets = value;
                NotifyPropertyChanged("Tickets");
            }
        }

        private decimal total;
        public decimal Total
        {
            get { return total; }
            set
            {
                total = value;
                NotifyPropertyChanged("Total");
            }
        }

        private decimal selectedtotal;
        public decimal SelectedTotal
        {
            get { return selectedtotal; }
            set
            {
                selectedtotal = value;
                NotifyPropertyChanged("SelectedTotal");
            }
        }

        private DateTime m_startdate;
        private DateTime m_enddate;


        public DateTime StartDate
        {
            get { return m_startdate; }
            set { m_startdate = value; NotifyPropertyChanged("StartDate"); }
        }

        public DateTime EndDate
        {
            get { return m_enddate; }
            set { m_enddate = value; NotifyPropertyChanged("EndDate"); }
        }

 
    }
}
