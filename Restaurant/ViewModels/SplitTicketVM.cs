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
    public class SplitTicketVM:INPCBase
    {
        public ICommand LineItemClicked { get; set; }

        public ICommand SeatClicked { get; set; }

        public ICommand AddClicked { get; set; }

        public ICommand NewTicketClicked { get; set; }

        public ICommand SeatsToTicketsClicked { get; set; }

        public ICommand BackClicked { get; set; }

        private ObservableCollection<Ticket> m_tickets;
        private SalesModel m_salesModel;

    
        private Visibility m_visibleaddbutton;
        private SecurityModel m_security;
        public Visibility VisibleAddButton
        {
            get { return m_visibleaddbutton; }
            set
            {
                m_visibleaddbutton = value;
                NotifyPropertyChanged("VisibleAddButton");
            }
        }

        int selected = 0;
        string selected_seat="";
        Window m_parent;
        string m_tracker = "";

        public Ticket CurrentTicket { get; set; }
        public SplitTicketVM(Window parent,SecurityModel security,Ticket currentticket)
        {
            m_security = security;
            m_parent = parent;
            CurrentTicket = currentticket;
      
            VisibleAddButton = Visibility.Collapsed;

            LineItemClicked = new RelayCommand(ExecuteLineItemClicked, param => true);
            SeatClicked = new RelayCommand(ExecuteSeatClicked, param => true);


            AddClicked = new RelayCommand(ExecuteAddClicked, param => true);

            NewTicketClicked = new RelayCommand(ExecuteNewTicketClicked, param => true);
            SeatsToTicketsClicked = new RelayCommand(ExecuteSeatsToTicketsClicked, param => true);

            BackClicked = new RelayCommand(ExecuteBackClicked, param => true);

            m_salesModel = new SalesModel(m_security);

            m_tracker = DateTime.Now.ToString("yyyyMMdd-HH:mm:ss");

            CurrentTicket.SetTracker(m_tracker);
            CurrentTicket.SplitQuantities();
            LoadTickets();
            
        }

        public void LoadTickets()
        {
          

           Tickets = m_salesModel.LoadSplitTickets(m_tracker);
          
       
            VisibleAddButton = Visibility.Collapsed;
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


        public void ExecuteLineItemClicked(object obj)
        {

            if (obj.ToString() != "")
            {
                selected_seat = "";
                selected = int.Parse(obj.ToString());
                VisibleAddButton = Visibility.Visible;
            }


        }

        public void ExecuteSeatClicked(object obj)
        {
            if(obj.ToString() != "")
            {
                selected = 0;
                selected_seat =obj.ToString();
                VisibleAddButton = Visibility.Visible;
            }
        }

        public void ExecuteAddClicked(object obj)
        {
            
            if(obj.ToString() != "")
            {
                int salesid = int.Parse(obj.ToString());

                if(selected != 0)
                {
                    m_salesModel.DBUpdateTicketID(selected, salesid);
                }else
                 if(selected_seat != "")
                  {
                    foreach(Ticket tick in Tickets)
                        foreach(Seat seat in tick.Seats)
                        {
                            if(seat.Ticket_Seat_ID == selected_seat)
                            {
                                //move all the items in the matching seat to new ticket id
                                foreach(LineItem line in seat.LineItems)
                                {
                                    m_salesModel.DBUpdateTicketID(line.ID, salesid);
                                }

                            }
                        }
                    }
              
                LoadTickets();

            }


        }

        public void ExecuteSeatsToTicketsClicked(object obj)
        {
            
            foreach (Ticket tick in Tickets)
            {
                if(tick.Seats.Count > 1)  //if ticket contain more than one seats
                {
                    int current_seat = 0;
                    foreach (Seat seat in tick.Seats)
                    {

                        if (current_seat > 0)
                        {
                            //create new ticket
                            Ticket newticket = createnewticket();
                            //move all the items in the matching seat to new ticket id
                            foreach (LineItem line in seat.LineItems)
                            {
                                m_salesModel.DBUpdateTicketID(line.ID, newticket.SalesID);
                            }

                        }
                        current_seat++;
                    }
                }
          
            }

            LoadTickets();

        }


        public void ExecuteNewTicketClicked(object button)
        {

            if (m_security.CurrentEmployee == null) return;

            createnewticket();
           
         
            LoadTickets();



        }

        private Ticket createnewticket()
        {
            Ticket newticket = new Ticket(CurrentTicket.CurrentEmployee);


            //creates a sales record
            newticket.CreateTicket(CurrentTicket.TicketOrderType, CurrentTicket.TicketSubOrderType, CurrentTicket.TableNumber);
            newticket.SetTracker(m_tracker);
            newticket.AssignOrderNumber();
            return newticket;
        }


        public void ExecuteBackClicked(object button)
        {

            foreach(Ticket tck in Tickets)
            {

                if (tck.AllItemCount == 0 && tck.TotalPayment == 0)
                    tck.VoidTicket("empty - split ticket");

            }

            m_parent.Close();



        }
    }
}
