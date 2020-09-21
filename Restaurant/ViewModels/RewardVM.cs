using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
   
   
    public class RewardVM:INPCBase
    {

         private Ticket CurrentTicket;
    private Window m_parent;
    public ICommand RewardClicked { get; set; }



    public RewardVM(Window parent, Ticket m_ticket)
        {
          

            m_parent = parent;
            CurrentTicket = m_ticket;

            RewardClicked = new RelayCommand(ExecuteRewardClicked, param => this.CanExecuteRewardClicked);

        
        }



        public decimal UsableBalance
        {
            get
            {
                if (CurrentTicket.CustomerID > 0) return CustomerModel.GetUsableReward(CurrentTicket.CustomerID);
                else return 0;
            }
        }




        public string MaxRewardStr
        {
            get
            {
                if (UsableBalance > CurrentTicket.Balance) return BalanceStr;
                else
                    return String.Format("{0:0.00}", UsableBalance);

            }
        }

        public string BalanceStr
        {
            get
            {
                return String.Format("{0:0.00}", CurrentTicket.Balance);
            }
        }

        public bool CanExecuteRewardClicked
        {
            get
            {

                if (CurrentTicket.SalesID == 0) return false;
                if (SalesModel.HasBeenPaid(CurrentTicket.SalesID, "STORECREDIT")) return false; //aslo need to check if has any reward
                return true;
            }

        }



        public void ExecuteRewardClicked(object amount)
        {

            string temp;
            decimal amt;

            try
            {
                temp = amount.ToString();
                if (temp != "")
                {

                    amt = decimal.Parse(temp);

                    //validate amount before adding to ticket
                    if (amt > UsableBalance)
                    {
                        MessageBox.Show("Not Enough Store Credit!");
                    }
                    else
                    {
                        if (amt > CurrentTicket.Balance)
                        {
                            MessageBox.Show("Amount is greater than balance!!");
                        }
                        else
                        {
                            PaymentModel.InsertPayment(CurrentTicket.SalesID, "REWARD/CREDIT", amt, amt, "", "", "", 0, DateTime.Now,"SALE");

                            //deduct out of reward table 
                            SalesModel.RedeemReward(CurrentTicket.CurrentCustomer.ID, CurrentTicket.SalesID, CurrentTicket.SaleDate, CurrentTicket.Total, amt, "");


                            GlobalSettings.CustomerDisplay.WriteDisplay("Store Credit:", amt, "Balance:", CurrentTicket.Balance);
                            NotifyPropertyChanged("CurrentTicket");
                            m_parent.Close();

                        }

                    }


                }
                else MessageBox.Show("Please Enter Amount");

            }
            catch (Exception e)
            {

                MessageBox.Show("Payment - Store Credit Clicked: " + e.Message);
            }
        }



    }
}
