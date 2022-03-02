using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;
using RedDotBase;

namespace RedDot
{
    public class GratuityViewModel: INPCBase
    {
        private Ticket _currentticket;
        public ICommand EmployeeClicked { get; set; }
        public ICommand EqualClicked { get; set; }
        public ICommand PercentClicked { get; set; }

        public ICommand NoTipsClicked { get; set; }

        public ICommand AdjustTipClicked { get; set; }

        public ICommand ExitClicked { get; set; }


        private Window _parent;
    
        private bool m_integrated = (GlobalSettings.Instance.CreditCardProcessor != "External");
        private HeartPOS m_ccp;
        private SalesModel m_salesmodel;
        public HeartPOS CCP
        {
            get { return m_ccp; }
        }

        private bool m_settled = false;
        private bool cashonly = true;

        List<Payment> payment;

        public GratuityViewModel(Window win, Ticket ticket, HeartPOS ccp)
        {
            CurrentTicket = ticket;
            m_ccp = ccp;

            _parent = win;
            EmployeeClicked = new RelayCommand(ExecuteEmployeeClicked, param => true);
            EqualClicked = new RelayCommand(ExecuteEqualClicked, param => true);
            PercentClicked = new RelayCommand(ExecutePercentClicked, param => true);
            NoTipsClicked = new RelayCommand(ExecuteNoTipsClicked, param => this.CanClickNoTip);
             ExitClicked = new RelayCommand(ExecuteExitClicked, param => this.CanClose);

            m_salesmodel = new SalesModel(null);
          

            if(GlobalSettings.Instance.AllowTipOnCashSales)
            {
                payment = CurrentTicket.Payments.Where(x => x.CardGroup.ToUpper() == "CREDIT" || x.CardGroup.ToUpper() == "DEBIT" || x.CardGroup.ToUpper() == "CASH").Where(x => x.Voided == false).ToList();


            }
            else
            {
                payment = CurrentTicket.Payments.Where(x => x.CardGroup.ToUpper() == "CREDIT" || x.CardGroup.ToUpper() == "DEBIT").Where(x => x.Voided == false).ToList();

            }


            if (payment == null)
            {
                TouchMessageBox.ShowSmall("Payment not found!!");
                _parent.Close();
            }


            m_settled = true;
            cashonly = true;
            //if any one of payment is not setteld , then 
            foreach (Payment pay in payment)
            {
                if (pay.TransType.ToUpper() == "AUTH") m_settled = false;
                if (pay.CardGroup.ToUpper() != "CASH") cashonly = false;
            }
                

        }

        public bool CanClickNoTip
        {
            get
            {
                if (cashonly) return true;
                if (m_settled && CurrentTicket.TipAmount > 0 && m_integrated) return false; else return true;
            }
        }

        public bool CanClose
        {
            get
            {
                if (cashonly) return true;

                if (m_integrated && m_settled )
                {
                  
                    if (CurrentTicket.GratuityTotal == CurrentTicket.TipAmount) return true;
                    else return false;
                } return true;
            }
        }
        public void ExecuteEmployeeClicked(object idstring)
        {

            decimal amt = 0;
            string temp = (string)idstring;
            string[] ids = temp.Split(','); // ids[0] = gratuity id,ids[1] = employeeid

            int gratuityid = int.Parse(ids[0]);
            int employeeid = int.Parse(ids[1]);



            NumPad np = new NumPad("Enter Tip Amount:", false)
            {
                Topmost = true
            };

            np.ShowDialog();
            if (np.Amount != "")
            {
                amt = decimal.Parse(np.Amount);
                if (ids[0] == "0")
                {
                    //no gratuity assigned yet
                    CurrentTicket.AddGratuity(employeeid, amt);

                }
                else
                {
                    CurrentTicket.UpdateGratuity(gratuityid, amt);
                }

                //Can not update if payment is already settled
                if (m_settled == false || cashonly)
                {
                    decimal tip = CurrentTicket.GratuityTotal;

                    if(payment.Count > 1)
                    {
                        if(tip != payment.Sum(x=>x.TipAmount))
                        {
                            //ask for new tip amount for each card / cash payment
                            foreach(Payment pay in payment)
                            {
                                NumPad np2 = new NumPad("Enter Tip Amount for " + pay.CardGroup + ":" +  pay.AuthorCode + ":", false, pay.TipAmount.ToString());
                                Utility.OpenModal(_parent, np2);
                                decimal newtip = decimal.Parse(np2.Amount);
                                m_salesmodel.UpdateTipAmount(pay,newtip);
                            }
                        }
                    }else
                    {
                        Payment pay = payment.FirstOrDefault();
                        m_salesmodel.UpdateTipAmount(pay, tip);
                    }
        
                   
                }



                CurrentTicket.LoadPayment();
            }




        }

        public void ExecuteNoTipsClicked(object idstring)
        {

   
                foreach (Gratuity grat in CurrentTicket.Gratuities)
                {
      
                    if (grat.ID == 0)
                    {
                        //no gratuity assigned yet
                        CurrentTicket.AddGratuity(grat.CurrentEmployee.ID, 0);
                    }
                    else
                    {
                        CurrentTicket.UpdateGratuity(grat.ID, 0);
                    }
                 

                }


                //update tip amount to zero
            if (payment != null)
            {
             
                    foreach(Payment pay in payment)
                        m_salesmodel.UpdateTipAmount(pay, 0 );


                    CurrentTicket.LoadPayment();

            }
            else
            {
                TouchMessageBox.Show("No Valid Credit transaction available!!");
            }

        }
        public void ExecuteEqualClicked(object idstring)
        {
            decimal equalamt = 0;

            EnterTipAmount();

            if (CurrentTicket.TipAmount == 0)
                TouchMessageBox.Show("Tip Amount is ZERO!!!");

           
                equalamt = Math.Round(CurrentTicket.TipAmount / CurrentTicket.Gratuities.Count, 2);

                int count = 0;
                decimal adjamt = 0;
                foreach (Gratuity grat in CurrentTicket.Gratuities)
                {
                    if (count == 0)
                    {
                        adjamt = CurrentTicket.TipAmount - equalamt * (CurrentTicket.Gratuities.Count - 1);
                        count++;
                    }
                    else adjamt = equalamt;

                    if (grat.ID == 0)
                    {
                        //no gratuity assigned yet
                        CurrentTicket.AddGratuity(grat.CurrentEmployee.ID, adjamt);
                    }
                    else
                    {
                        CurrentTicket.UpdateGratuity(grat.ID, adjamt);
                    }

                }

         


        }

        /*
        public void ExecuteAdjustTipClicked(object obj)
        {
           // Payment payment = CurrentTicket.Payments.Where(x => x.CardGroup.ToUpper() == "CREDIT").Where(x=>x.Voided == false).FirstOrDefault();

            if(payment != null)
            {
                NumPad np = new NumPad("Enter New Tip Amount", false,CurrentTicket.TipAmount.ToString());
                np.ShowDialog();

                if(np.Amount != CurrentTicket.TipAmount.ToString())
                {
                    decimal amt = decimal.Parse(np.Amount);
                    Random rand = new Random();


                    if(GlobalSettings.Instance.CreditCardProcessor == "HeartSIP")
                    {
                        m_ccp.TipPaymentId = payment.ID;
                        m_ccp.TipRequestId = rand.Next(1000, 9999999).ToString();
                        m_ccp.ExecuteAdjustTipCommand(amt, payment.ResponseId, m_ccp.TipRequestId);
                    }
                 
                }
                
            }else
            {
                TouchMessageBox.Show("No Valid Credit transaction available!!");
            }

        }
        */



        public void ExecuteExitClicked(object obj)
        {
            _parent.Close();
        }
        public void ExecutePercentClicked(object idstring)
        {
            decimal percentamt = 0;
            decimal percent = 0;


            EnterTipAmount();

            if (CurrentTicket.TipAmount == 0)
                TouchMessageBox.Show("Tip Amount is ZERO!!!");

            decimal sharetotal = 0;


                foreach (Gratuity grat in CurrentTicket.Gratuities)
                {
                    sharetotal = sharetotal + grat.TicketShareAmount;

                }

                int totalemployee = CurrentTicket.Gratuities.Count();
                decimal totalused = 0;
                int count = 0;

                foreach (Gratuity grat in CurrentTicket.Gratuities)
                {
                    count++;
                    // cannot use ticket total because it is discounted.
                    percent = grat.TicketShareAmount / sharetotal;

                    //if last person then they get remaining to remove rounding error.
                    if (count == totalemployee)
                        percentamt = CurrentTicket.TipAmount - totalused;
                    else
                        percentamt = Math.Round(CurrentTicket.TipAmount * percent,2);

                    totalused = totalused + percentamt;


                    if (grat.ID == 0)
                    {
                        //no gratuity assigned yet
                        CurrentTicket.AddGratuity(grat.CurrentEmployee.ID, percentamt);

                    }
                    else
                    {
                        CurrentTicket.UpdateGratuity(grat.ID, percentamt);
                    }

                }







        }



        private void EnterTipAmount()
        {
           

            if (payment != null)
            {

                foreach(Payment pay in payment)
                {
                    if(pay.CardGroup.ToUpper() == "CASH" || pay.TransType.ToUpper() == "AUTH" || !m_integrated)
                    {
                        NumPad np = new NumPad("Enter Tip Amount for " + pay.CardGroup.ToUpper() + " payment:" + pay.AuthorCode + ":", false, CurrentTicket.TipAmount.ToString());
                        np.Topmost = true;
                        np.ShowDialog();


                        if (np.Amount != "")
                        {
                            decimal tip = decimal.Parse(np.Amount);
                            m_salesmodel.UpdateTipAmount(pay, tip);

                        }
                    }
                 
                }
           


                CurrentTicket.LoadPayment();
            }
            else
            {
                TouchMessageBox.Show("No Valid Credit transaction available!!");
            }
        }






        public Ticket CurrentTicket
        {
            get { return _currentticket; }

            set
            {
                _currentticket = value;
                NotifyPropertyChanged("CurrentTicket");
            }
        }

  


  

    }
}
