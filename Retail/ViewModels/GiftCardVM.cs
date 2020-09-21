using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace RedDot
{
    public class GiftCardVM:INPCBase
    {

        private DataTable m_giftcardlist;
        public DataTable GiftCardList {
            get { return m_giftcardlist; } 
            set { m_giftcardlist = value; NotifyPropertyChanged("GiftCardList"); } 
        }

        private GiftCardModel m_giftcardmodel;
        public GiftCardVM()
        {
            m_giftcardmodel = new GiftCardModel();
            GiftCardList = m_giftcardmodel.GetGiftCardList();
        }

        public decimal TotalPurchased { get { return m_giftcardmodel.TotalPurchased; } }
        public decimal TotalBalance { get { return m_giftcardmodel.TotalBalance; } }
        public decimal TotalUsed { get { return m_giftcardmodel.TotalUsed; } }
    }
}
