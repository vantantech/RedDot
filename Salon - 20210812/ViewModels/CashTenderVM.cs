using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class CashTenderVM:INPCBase
    {

        public ICommand CashClicked { get; set; }
        private Ticket CurrentTicket;
        private Window m_parent;

        private VFD vfd;

        public CashTenderVM(Window parent, Ticket m_ticket)
        {
            m_parent = parent;
            CurrentTicket = m_ticket;

            CashClicked = new RelayCommand(ExecuteCashClicked, param => this.CanExecuteCashTenderClicked);

            vfd = new VFD(GlobalSettings.Instance.DisplayComPort);

        }


        public string BalanceStr
        {
            get
            {
                return String.Format("{0:0.00}", CurrentTicket.Balance);
            }
        }
        public bool CanExecuteCashTenderClicked
        {
            get
            {

                if (CurrentTicket.SalesID == 0) return false;
                // if (_currentticket.HasBeenPaid("Cash")) return false;
                return true;
            }

        }

        public void ExecuteCashClicked(object amount)
        {

            string temp = amount as string;
            decimal amt;

            try
            {
                if (temp != "")
                {

                    amt = decimal.Parse(temp);
                    decimal netamount;

                    //if change is given to customers, then record the balance, not the tender 
                    if (amt > CurrentTicket.Balance) netamount = CurrentTicket.Balance;
                    else netamount = amt;


                   bool succeed =  CurrentTicket.InsertPayment( "Cash", amt, netamount, "", "", "", 0, DateTime.Now,"Sale");
                    if (succeed)
                    {
                        vfd.WriteDisplay("Cash:", amt, "Balance:", CurrentTicket.Balance);


                        NotifyPropertyChanged("CurrentTicket");
                        m_parent.Close();
                    }
                    else
                        TouchMessageBox.Show("Error Inserting payment record.");

                   

                }
                else TouchMessageBox.Show("Please Enter Amount");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Error Cash Clicked: " + e.Message);
            }
        }



  
    }
}
