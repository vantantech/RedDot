using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManager;
using System.Data;

namespace RedDot
{
    public class GiftCardModel
    {
        DBTicket m_dbticket;
        decimal m_totalpurchased=0;
        decimal m_totalbalance=0;
        decimal m_totalused=0;
        public GiftCardModel()
        {

            m_dbticket = new DBTicket();

        }

        public DataTable GetGiftCardList()
        {
            DataTable dt;

            dt =  m_dbticket.GetGiftCardList();
            foreach(DataRow row in dt.Rows)
            {
                m_totalpurchased = m_totalpurchased + decimal.Parse(row["amount"].ToString());
                m_totalbalance = m_totalbalance + decimal.Parse(row["balance"].ToString());

            }

            m_totalused = m_totalpurchased - m_totalbalance;

            return dt;
        }

        public static decimal GetGiftCardBalance(string accountno)
        {
            DBTicket dbticket = new DBTicket();
            return dbticket.GetGiftCardBalance(accountno);

        }

        public decimal TotalPurchased { get { return m_totalpurchased; } }
        public decimal TotalBalance { get { return m_totalbalance; } }
        public decimal TotalUsed { get { return m_totalused; } }

    }
}
