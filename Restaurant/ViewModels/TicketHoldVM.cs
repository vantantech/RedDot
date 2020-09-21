using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class TicketHoldVM:INPCBase
    {
        public ICommand AddMinutesClicked { get; set; }
        public ICommand ClearHoldClicked { get; set; }



        public Ticket CurrentTicket { get; set; }
        private Window m_parent;
        public TicketHoldVM(Window parent, Ticket m_ticket)
        {
            CurrentTicket = m_ticket;
            m_parent = parent;

            AddMinutesClicked = new RelayCommand(ExecuteAddMinutesClicked, param => true);
            ClearHoldClicked = new RelayCommand(ExecuteClearHoldClicked, param => true);



        }


        public void ExecuteAddMinutesClicked(object min)
        {
            if(min.ToString() != "")
            {
                int minutes = int.Parse(min.ToString());
                if(minutes == -99)
                    CurrentTicket.SetHoldDate(999999);
                else
                    CurrentTicket.SetHoldDate(minutes);
            }
            
        }

        public void ExecuteClearHoldClicked(object obj)
        {
            CurrentTicket.ClearHoldDate();
        }

    }
}
