using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class RemoteScreenVM : INPCBase
    {
        private Ticket m_currentticket;
        public Ticket CurrentTicket
        {
            get { return m_currentticket; }

            set
            {
                m_currentticket = value;
                NotifyPropertyChanged("CurrentTicket");
            }
        }

        private SalesModel m_salesmodel;



        public RemoteScreenVM()
        {

            m_salesmodel = new SalesModel(null);
        }

        public void RefreshTicket()
        {
            CurrentTicket = GlobalSettings.Instance.CurrentTicket;
        }

        public void LoadTicket(int salesid)
        {
            CurrentTicket = m_salesmodel.LoadTicket(salesid);


        }
    }
}
