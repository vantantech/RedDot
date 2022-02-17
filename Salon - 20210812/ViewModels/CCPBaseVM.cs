using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class CCPBaseVM:INPCBase
    {

        protected static Logger logger = LogManager.GetCurrentClassLogger();

        public ICommand CreditAuthClicked { get; set; }
        public ICommand CreditSaleClicked { get; set; }
        public ICommand DebitSaleClicked { get; set; }
        public ICommand LineItemClicked { get; set; }
        public ICommand CreditRefundClicked { get; set; }
        public ICommand DebitRefundClicked { get; set; }
        public ICommand VoidClicked { get; set; }
        public ICommand CommandClicked { get; set; }

        protected bool enabledebitsale = GlobalSettings.Instance.EnableDebitSale;
        protected bool enablecreditsale = GlobalSettings.Instance.EnableCreditSale;
        protected bool enableauthcapture = GlobalSettings.Instance.EnableAuthCapture;
        protected bool allowduplicates = GlobalSettings.Instance.AllowDuplicates;
        protected bool signatureonscreen = GlobalSettings.Instance.SignatureOnScreen;

        protected string m_transtype = "";
     



        protected SalesModel m_salesmodel { get; set; }
        
        protected VFD vfd;
        protected Payment m_payment;
        protected Window m_parent;
        protected CCPBaseVM(Window parent, Ticket m_ticket, string transtype, Payment payment)
        {

            CurrentTicket = m_ticket;
            m_parent = parent;
            m_transtype = transtype;
            m_payment = payment;

            if (transtype.ToUpper() == "VOID") NumPadVisibility = Visibility.Hidden; else NumPadVisibility = Visibility.Visible;

            m_salesmodel = new SalesModel(null);
            vfd = new VFD(GlobalSettings.Instance.DisplayComPort);

            LineItemClicked = new RelayCommand(ExecuteLineItemClicked, param => true);

            // NotifyPropertyChanged("BalanceStr");
        }



        public Ticket CurrentTicket { get; set; }
        public string BalanceStr
        {
            get
            {
                return String.Format("{0:0.00}", CurrentTicket.Balance);
            }
        }


        public Visibility NumPadVisibility { get; set; }

        private string m_refundamount;
        public string RefundAmount
        {
            get { return m_refundamount; }
            set
            {
                m_refundamount = value;
                NotifyPropertyChanged("RefundAmount");
            }
        }

        public bool CanExecuteChargeClicked
        {
            get
            {
                return (m_transtype.ToUpper() == "SALE");
            }

        }

        public bool CanExecuteDebitSaleClicked
        {
            get
            {
                if (CanExecuteChargeClicked)
                {
                    return enabledebitsale;
                }
                else
                    return false;
            }

        }




        public bool CanExecuteCreditSaleClicked
        {
            get
            {
                if (CanExecuteChargeClicked)
                {
                    return enablecreditsale;
                }
                else
                    return false;
            }

        }


        public bool CanExecuteAuthCaptureClicked
        {
            get
            {
                if (CanExecuteChargeClicked)
                {
                    return enableauthcapture;
                }
                else
                    return false;
            }

        }



        public bool CanExecuteVoidClicked
        {
            get
            {
                return (m_transtype.ToUpper() == "VOID");
            }

        }


        public void ExecuteLineItemClicked(object iditemtype)
        {

            // string temp = iditemtype as string;
            //  string[] portion = temp.Split(',');

            //  int id = 0;
            // string itemtype = portion[1]; // type = service, product , giftcard .. etc..


            try
            {

                // id = int.Parse(portion[0]);

                // if (id == 0) return;

                decimal total = 0;
                decimal producttotal = 0;

                decimal labortotal = 0;
                decimal taxtotal = 0;

                decimal taxabletotal = 0;

                foreach (TicketEmployee emp in CurrentTicket.TicketEmployees)
                    foreach (LineItem line in emp.LineItems)
                    {
                        if (line.Selected)
                        {
                            if (line.ItemEnumType == ProductType.Service)
                            {
                                labortotal = labortotal + line.AdjustedPrice * line.Quantity;
                            }
                            else
                            {
                                producttotal = producttotal + line.AdjustedPrice * line.Quantity;
                            }


                            //doesn't matter if its a service or product or giftcard , only cares if it's marked as taxable
                            if (line.Taxable)
                                taxabletotal = taxabletotal + line.AdjustedPrice * line.Quantity;
                        }

                    }



                taxtotal = Math.Round(taxabletotal * GlobalSettings.Instance.SalesTaxPercent / 100, 2);



                total = labortotal + producttotal + taxtotal;



                RefundAmount = total.ToString();

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Execute Line Item Clicked: " + e.Message);
            }
        }


    }
}
