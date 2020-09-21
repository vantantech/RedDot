using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    [DataContract]
    public class Seat:INPCBase
    {
        [DataMember]
        public int SeatNumber { get; set; }
        public bool Selected { get; set; }
        public string Ticket_Seat_ID { get; set; }

        private ObservableCollection<LineItem> m_lineitems;

        [DataMember]
        public ObservableCollection<LineItem> LineItems
        {
            get { return m_lineitems; }
            set
            {
                m_lineitems = value;
                NotifyPropertyChanged("LineItems");
            }
        }
    }
}
