using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Input;
using System.Windows;
using RedDotBase;

namespace RedDot
{
    public class GiftCardVM : INPCBase
    {

        private DataTable m_giftcardlist;
        private DataTable m_giftcarddetails;
        private SecurityModel _security;
        private Window _parent;

        public ICommand PrintClicked { get; set; }
        public ICommand GiftCardClicked { get; set; }

        public ICommand OpenOrderClicked { get; set; }




        private GiftCardModel m_giftcardmodel;
        public GiftCardVM(Window parent, SecurityModel security)
        {
            _parent = parent;
            _security = security;

            PrintClicked = new RelayCommand(PrintGiftCard, param => true);
            GiftCardClicked = new RelayCommand(ExecuteGiftCardClicked, param => true);
            OpenOrderClicked = new RelayCommand(ExecuteOpenOrderClicked, param => true);

            m_giftcardmodel = new GiftCardModel();
            LoadList();
        }

        private void LoadList()
        {


            GiftCardList = m_giftcardmodel.GetGiftCardList(m_showall);


            TotalPurchased = m_giftcardmodel.TotalPurchased;
            TotalBalance = m_giftcardmodel.TotalBalance;
            TotalUsed = m_giftcardmodel.TotalUsed;
        }








        public DataTable GiftCardList
        {
            get { return m_giftcardlist; }
            set { m_giftcardlist = value; NotifyPropertyChanged("GiftCardList"); }
        }


        public DataTable GiftCardDetails
        {
            get { return m_giftcarddetails; }
            set { m_giftcarddetails = value; NotifyPropertyChanged("GiftCardDetails"); }
        }

        private bool m_showall = false;
        public bool ShowAll
        {
            get { return m_showall; }
            set
            {
                m_showall = value;
                NotifyPropertyChanged("ShowAll");
                LoadList();
            }
        }

        decimal m_totalpurchased;
        decimal m_totalbalance;
        decimal m_totalused;
        public decimal TotalPurchased
        {
            get { return m_totalpurchased; }
            set
            {
                m_totalpurchased = value;
                NotifyPropertyChanged("TotalPurchased");
            }
        }
        public decimal TotalBalance
        {
            get { return m_totalbalance; }
            set
            {
                m_totalbalance = value;
                NotifyPropertyChanged("TotalBalance");
            }
        }
        public decimal TotalUsed
        {
            get { return m_totalused; }
            set
            {
                m_totalused = value;
                NotifyPropertyChanged("TotalUsed");
            }
        }




        public void PrintGiftCard(object obj)
        {
            if (GiftCardList != null)
                ReceiptPrinterModel.PrintGiftCard(GiftCardList);
        }

        public void ExecuteGiftCardClicked(object obj)
        {

            string accountno = obj.ToString();
            GiftCardDetails = m_giftcardmodel.GetGiftCardDetails(accountno);
        }

        public void ExecuteOpenOrderClicked(object salesid)
        {

            int id;

            if (salesid == null) return;

            if (salesid.ToString() != "") id = int.Parse(salesid.ToString());
            else id = 0;

            SalesView ord = new SalesView(id);
            Utility.OpenModal(_parent, ord);



        }
    }
}
