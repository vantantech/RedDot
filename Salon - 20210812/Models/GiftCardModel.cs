using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;
using RedDot.DataManager;

namespace RedDot
{
    public class GiftCardModel
    {
        DBTicket m_dbticket;
        decimal m_totalpurchased = 0;
        decimal m_totalbalance = 0;
        decimal m_totalused = 0;

        public GiftCardModel()
        {

            m_dbticket = new DBTicket();

        }

        public DataTable GetGiftCardList(bool showall)
        {
            DataTable dt;
            m_totalpurchased = 0;
            m_totalbalance = 0;
            m_totalused = 0;

            dt = m_dbticket.GetGiftCardList(showall);
            foreach (DataRow row in dt.Rows)
            {
                m_totalpurchased = m_totalpurchased + decimal.Parse(row["amount"].ToString());
                m_totalbalance = m_totalbalance + decimal.Parse(row["balance"].ToString());

            }

            m_totalused = m_totalpurchased - m_totalbalance;

            return dt;
        }

        public DataTable GetGiftCardDetails(string accountno)
        {

            return m_dbticket.GetGiftCardDetails(accountno);

        }

        public static decimal GetGiftCardBalance(string accountno)
        {
            DBTicket dbticket = new DBTicket();
            return dbticket.GetGiftCardBalance(accountno);

        }

        public static bool GiftCertificateExist(string accountno)
        {
            DBTicket dbticket = new DBTicket();
            return dbticket.GiftCertificateExist(accountno);
        }


        public decimal TotalPurchased { get { return m_totalpurchased; } }
        public decimal TotalBalance { get { return m_totalbalance; } }
        public decimal TotalUsed { get { return m_totalused; } }

    }
}
