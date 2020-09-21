
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NLog;


namespace RedDot
{
    public class SalesBaseVM:INPCBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected Window m_parent;
        protected SecurityModel m_security;
        protected bool CanExecute = true;

        public ICommand NoSaleClicked { get; set; }
        public ICommand VoidClicked { get; set; }

        protected string m_mode="new";

        public SalesBaseVM(Window parent, SecurityModel security)
        {
            m_parent = parent;
            m_security = security;

            NoSaleClicked = new RelayCommand(ExecuteNoSaleClicked, param => this.CanExecute);
            VoidClicked = new RelayCommand(ExecuteVoidClicked, param => this.CanVoidTicket);
        }




        //------------------------------------------------------------------------------------------
        //  ____        _   _                ____  _           _     _      
        // | __ ) _   _| |_| |_ ___  _ __   |  _ \(_)___  __ _| |__ | | ___ 
        // |  _ \| | | | __| __/ _ \| '_ \  | | | | / __|/ _` | '_ \| |/ _ \
        // | |_) | |_| | |_| || (_) | | | | | |_| | \__ \ (_| | |_) | |  __/
        // |____/ \__,_|\__|\__\___/|_| |_| |____/|_|___/\__,_|_.__/|_|\___|                                                                 
        //
        //------------------------------------------------------------------------------------------ 

        public bool CanClearTicket
        {
            get
            {
                if (CurrentTicket == null) return false;

                //Can only void ticket if ticket is open/reverse and no payments
                if (m_mode != "edit" && !CurrentTicket.SentToKitchen && CurrentTicket.Payments.Where(x => !x.Voided).Count() == 0) return true;
                else return false;


            }

        }

        public bool CanVoidTicket
        {
            get
            {
                //Can only void ticket if ticket is open and no payments
                if (m_mode == "edit" && (CanExecuteOpenTicket ) && CurrentTicket.Payments.Where(x => !x.Voided).Count() == 0) return true;
                else return false;


            }

        }

        public bool CanExecuteHoldTicket
        {
            get
            {
                if (CanExecuteOpenTicket && CurrentTicket.AllFired == false && CurrentTicket.ActiveItemCount > 0) return true; else return false;
            }
        }

        public bool CanCancel
        {
            get
            {
                if (CurrentTicket == null || CanExecuteOpenTicket && !CurrentTicket.HasHoldDate) return true; else return false;
            }
        }

        public bool CanExecuteOpenTicket
        {
            get
            {
                if (m_currentticket == null) return false;
                if (m_currentticket.Status == "Open" || m_currentticket.Status == "OpenTemp")
                {
                    return true;
                }
                else return false;
            }
        }

        /*
        public bool CanExecuteReversedTicket
        {
            get
            {
                if (m_currentticket == null) return false;
                if (m_currentticket.Status == "Reversed")
                {
                    return true;
                }
                else return false;
            }
        }
        */

        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        ____                            _         
        // |  _ \ _   _| |__ | (_) ___  |  _ \ _ __ ___  _ __   ___ _ __| |_ _   _ 
        // | |_) | | | | '_ \| | |/ __| | |_) | '__/ _ \| '_ \ / _ \ '__| __| | | |
        // |  __/| |_| | |_) | | | (__  |  __/| | | (_) | |_) |  __/ |  | |_| |_| |
        // |_|    \__,_|_.__/|_|_|\___| |_|   |_|  \___/| .__/ \___|_|   \__|\__, |
        //                                              |_|                  |___/ 
        //------------------------------------------------------------------------------------------

        protected Ticket m_currentticket;
        public Ticket CurrentTicket
        {
            get { return m_currentticket; }

            set
            {
                m_currentticket = value;
                NotifyPropertyChanged("CurrentTicket");
            }
        }

        protected Employee m_currentemployee;
        public Employee CurrentEmployee
        {
            get { return m_currentemployee; }
            set { m_currentemployee = value; NotifyPropertyChanged("CurrentEmployee"); }
        }




        private decimal m_balance;
        public decimal Balance
        {
            get { return m_balance; }
            set
            {
                m_balance = value;
                NotifyPropertyChanged("Balance");
            }
        }

        private decimal m_auto1;
        public decimal Auto1
        {
            get { return m_auto1; }
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



        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        __  __      _   _               _     
        // |  _ \ _   _| |__ | (_) ___  |  \/  | ___| |_| |__   ___   __| |___ 
        // | |_) | | | | '_ \| | |/ __| | |\/| |/ _ \ __| '_ \ / _ \ / _` / __|
        // |  __/| |_| | |_) | | | (__  | |  | |  __/ |_| | | | (_) | (_| \__ \
        // |_|    \__,_|_.__/|_|_|\___| |_|  |_|\___|\__|_| |_|\___/ \__,_|___/
        //    
        //------------------------------------------------------------------------------------------  


        public void CalculateCashButtons()
        {
            if(CurrentTicket == null)
            {
                Auto1 = 0;
                Auto2 = 0;
                Auto3 = 0;
                Auto4 = 0;
                Auto5 = 0;
                Auto6 = 0;
                Balance = 0;
            }
            else
            {
              
                    CalculateButtons(CurrentTicket.Balance);
                    Balance = CurrentTicket.Balance;
               

            }

        }


        private void CalculateButtons(decimal balance)
        {
            Auto1 = (int)balance + 1;
            if (Auto1 < 5)
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


            if (Auto1 < 20)
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

            //50 and over
            if (Auto1 > 50)
            {
                Auto2 = (int)Auto1 / 5 * 5 + 5;
                Auto3 = (int)Auto1 / 10 * 10 + 10;
                if (Auto3 == Auto2) Auto3 += 10;
                Auto4 = (int)Auto1 / 20 * 20 + 20;
                if (Auto4 == Auto3) Auto4 += 20;
                Auto5 = (int)Auto1 / 50 * 50 + 50;
                if (Auto5 == Auto4) Auto5 += 50;
                Auto6 = (int)Auto1 / 100 * 100 + 100;
                return;
            }

          

        }






        //------------------------------------------------------------------------------------------
        //  ____        _   _                 ____                                          _     
        // | __ ) _   _| |_| |_ ___  _ __    / ___|___  _ __ ___  _ __ ___   __ _ _ __   __| |___ 
        // |  _ \| | | | __| __/ _ \| '_ \  | |   / _ \| '_ ` _ \| '_ ` _ \ / _` | '_ \ / _` / __|
        // | |_) | |_| | |_| || (_) | | | | | |__| (_) | | | | | | | | | | | (_| | | | | (_| \__ \
        // |____/ \__,_|\__|\__\___/|_| |_|  \____\___/|_| |_| |_|_| |_| |_|\__,_|_| |_|\__,_|___/
        //
        //------------------------------------------------------------------------------------------ 

        public void ExecuteNoSaleClicked(object button)
        {
            //Open cash drawer
            if (m_security.WindowAccess("NoSale") == true)
            {
                ReceiptPrinter.CashDrawer(GlobalSettings.Instance.ReceiptPrinter);
            }


        }

        public virtual void ExecuteVoidClicked(object salesid)
        {



            try
            {
                //if ticket is sent to kitchen or not
                if (CurrentTicket.SentToKitchen)
                {
                    if (m_security.ManagerOverrideAccess("VoidClosedTicket"))
                    {

                        // confirm reason for VOIDING ticket
                        ConfirmAudit win;
                        win = new ConfirmAudit();
                        Utility.OpenModal(m_parent, win);
                        if (win.Reason != "")
                        {


                            CurrentTicket.VoidTicket(win.Reason);

                            AuditModel.WriteLog(CurrentEmployee.DisplayName, "Void Ticket", win.Reason, "sales", CurrentTicket.SalesID);
                            logger.Info("Void Ticket:reason=>" + win.Reason + " , ticket=>" + CurrentTicket.SalesID + ", employee: " + CurrentEmployee.DisplayName);

                            CurrentTicket = null;

                            if (m_parent.Name == "OrderEntryWindow")
                                m_parent.Close();

                        }

                    }
                }
                else
                {
                    
                        if (Confirm.Ask("Delete Ticket Permanently ?!!"))
                        {
                            CurrentTicket.DeleteTicket();

                            AuditModel.WriteLog(CurrentEmployee.DisplayName, "Delete Ticket", "Unsent Ticket Delete", "sales", CurrentTicket.SalesID);
                            logger.Info("Void Ticket:reason=>Unsent Ticket Delete , ticket=>" + CurrentTicket.SalesID + ", employee: " + CurrentEmployee.DisplayName);

                            CurrentTicket = null;


                            if (m_parent.Name == "OrderEntryWindow")
                                m_parent.Close();
                        }

                }

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Error deleting line item: " + e.Message);
            }
        }


    }
}
