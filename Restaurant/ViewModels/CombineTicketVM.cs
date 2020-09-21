using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class CombineTicketVM : INPCBase
    {
 

        public ICommand BackClicked { get; set; }
        public ICommand CombineClicked { get; set; }

        private ObservableCollection<Ticket> m_tickets;
        private SalesModel m_salesModel;


        private SecurityModel m_security;


    
        Window m_parent;
        public Ticket CurrentTicket { get; set; }
        public CombineTicketVM(Window parent, SecurityModel security, Ticket currentticket)
        {
            m_security = security;
            m_parent = parent;
            CurrentTicket = currentticket;



            BackClicked = new RelayCommand(ExecuteBackClicked, param => true);
            CombineClicked = new RelayCommand(ExecuteCombineClicked, param => true);

            m_salesModel = new SalesModel(m_security);
            LoadTickets();

        }

        public void LoadTickets()
        {


            // Tickets = m_salesModel.LoadOpenTicketsByOrderNumber(m_ordernumber,m_security.CurrentEmployee.ID);
            Tickets = m_salesModel.LoadCombineTickets(m_security.CurrentEmployee.ID, CurrentTicket.SalesID);

        }


        public ObservableCollection<Ticket> Tickets
        {
            get { return m_tickets; }
            set
            {
                m_tickets = value;
                NotifyPropertyChanged("Tickets");
            }
        }




        public void ExecuteCombineClicked(object obj)
        {

            if (obj.ToString() != "")
            {
                int salesid = int.Parse(obj.ToString());   //source
                Ticket SourceTicket = new Ticket(salesid);

                foreach(Seat seat in SourceTicket.Seats)
                    foreach(LineItem line in seat.LineItems)
                    {
                        m_salesModel.DBUpdateTicketID(line.ID, CurrentTicket.SalesID);
                    }

                SourceTicket.VoidTicket("combine ticket");



      

                m_parent.Close();

            }


        }





        public void ExecuteBackClicked(object button)
        {

      

            m_parent.Close();



        }
    }
}
