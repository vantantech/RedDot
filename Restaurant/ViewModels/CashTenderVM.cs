using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class CashTenderVM : INPCBase
    {

        public ICommand CashClicked { get; set; }
        private Ticket CurrentTicket;
        private Window m_parent;

        private decimal m_auto1;
        public decimal Auto1
        {
            get{ return m_auto1; }
            set
            {
                m_auto1 = value;
                NotifyPropertyChanged("Auto1");
            }
        }


        private decimal m_auto2;
        public decimal Auto2
        {
            get { return m_auto2; }
            set
            {
                m_auto2 = value;
                NotifyPropertyChanged("Auto2");
            }
        }

        private decimal m_auto3;
        public decimal Auto3
        {
            get { return m_auto3; }
            set
            {
                m_auto3 = value;
                NotifyPropertyChanged("Auto3");
            }
        }

        private decimal m_auto4;
        public decimal Auto4
        {
            get { return m_auto4; }
            set
            {
                m_auto4 = value;
                NotifyPropertyChanged("Auto4");
            }
        }


        private decimal m_auto5;
        public decimal Auto5
        {
            get { return m_auto5; }
            set
            {
                m_auto5 = value;
                NotifyPropertyChanged("Auto5");
            }
        }


        private decimal m_auto6;
        public decimal Auto6
        {
            get { return m_auto6; }
            set
            {
                m_auto6 = value;
                NotifyPropertyChanged("Auto6");
            }
        }



        public CashTenderVM(Window parent, Ticket m_ticket)
        {
            m_parent = parent;
            CurrentTicket = m_ticket;
            CashClicked = new RelayCommand(ExecuteCashClicked, param => this.CanExecuteCashTenderClicked);
        }


        public decimal Balance
        {
            get
            {
              
                    CalculateButtons(CurrentTicket.Balance);
                    //return String.Format("{0:0.00}", CurrentTicket.Total);
                   return  CurrentTicket.Balance;
               
              
            }
        }

        public string BalanceStr
        {
            get
            {     return String.Format("{0:0.00}", Balance);}
            }
       
        private void CalculateButtons(decimal balance)
        {
            Auto1 = (int)balance + 1;
            if(Auto1 < 5)
            {
                Auto2 = 5;
                Auto3 = 10;
                Auto4 = 20;
                Auto5 = 50;
                Auto6 = 100;
                return;
            }

            if (Auto1 < 10)
            {
                Auto2 = 10;
                Auto3 = 20;
                Auto4 = 50;
                Auto5 = 100;
                Auto6 = 120;
                return;
            }

            if (Auto1 < 15)
            {
                Auto2 = 15;
                Auto3 = 20;
                Auto4 = 50;
                Auto5 = 100;
                Auto6 = 120;
                return;
            }


            if(Auto1 < 20)
            {
                Auto2 = 20;
                Auto3 = 50;
                Auto4 = 100;
                Auto5 = 120;
                Auto6 = 150;
                return;
            }

            if (Auto1 < 25)
            {
                Auto2 = 25;
                Auto3 = 30;
                Auto4 = 40;
                Auto5 = 50;
                Auto6 = 100;
                return;
            }

            if (Auto1 < 30)
            {
             
                Auto2 = 30;
                Auto3 = 40;
                Auto4 = 50;
                Auto5 = 100;
                Auto6 = 120;
                return;
            }

            if (Auto1 < 35)
            {

                Auto2 = 35;
                Auto3 = 40;
                Auto4 = 50;
                Auto5 = 100;
                Auto6 = 120;
                return;
            }

            if (Auto1 < 40)
            {

                Auto2 = 40;
                Auto3 = 50;
                Auto4 = 100;
                Auto5 = 120;
                Auto6 = 150;
                return;
            }

            if (Auto1 < 45)
            {

                Auto2 = 45;
                Auto3 = 50;
                Auto4 = 60;
                Auto5 = 100;
                Auto6 = 120;
                return;
            }

            if (Auto1 < 50)
            {
                Auto2 = 50;
                Auto3 = 60;
                Auto4 = 100;
                Auto5 = 120;
                Auto6 = 150;
                return;
            }

            //50 to 100
            if (Auto1 < 100)
            {
                Auto2 = Math.Round(Auto1/5) * 5 + 5;
                Auto3 = Math.Round(Auto1 / 10) * 10 + 10;
                Auto4 = Math.Round(Auto1 / 20) * 20 + 20;
                Auto5 = Math.Round(Auto1 / 50) * 50 + 50;
                Auto6 = Math.Round(Auto1 / 100) * 100 + 50;
                return;
            }
        
        }


        public bool CanExecuteCashTenderClicked
        {
            get
            {
                if (CurrentTicket.SalesID == 0) return false;
                return true;
            }

        }

        public void ExecuteCashClicked(object amount)
        {

            string temp = amount.ToString();
            decimal amt;

            try
            {
                if (temp != "")
                {

                    amt = decimal.Parse(temp);
                    decimal netamount;

                    if (CurrentTicket.Balance == 0 && amt == 0)
                    {
                        CurrentTicket.TryCloseTicket();
                        return;
                    }



                    //if change is given to customers, then record the balance, not the tender 
                    if (amt > CurrentTicket.Balance) netamount = Balance;
                    else netamount = amt;


                    bool succeed = PaymentModel.InsertPayment(CurrentTicket.SalesID, "CASH", amt, netamount, "", "", "", 0, DateTime.Now, "SALE");
                    if (succeed)
                    {
                        CurrentTicket.Reload();
                        GlobalSettings.CustomerDisplay.WriteDisplay("Cash:", amt, "Balance:", CurrentTicket.Balance);

                        CurrentTicket.TryCloseTicket(); //need to load payment  to refresh object first before trying to close ticket
                    

                        NotifyPropertyChanged("CurrentTicket");
                        m_parent.Close();
                    }
                    else
                        TouchMessageBox.Show("Error Inserting payment record.");

                }
                else MessageBox.Show("Please Enter Amount");

            }
            catch (Exception e)
            {

                MessageBox.Show("Error Cash Clicked: " + e.Message);
            }
        }




    }
}
