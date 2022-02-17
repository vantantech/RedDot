using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using System.Data;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;
using RedDot;


namespace RedDot
{




    public class MainWindowVM : INPCBase
    {

        // private bool canExecute = true;
        private Window _parent;
        private Security _security;
        private DBEmployee m_dbemployee;

        public Visibility Demo { get; set; }

        public ICommand BackofficeClicked { get; set; }

        public ICommand TimecardClicked { get; set; }

        public ICommand EODSettleClicked { get; set; }

        public Visibility VisibleHide { get; set; }

        public Visible ButtonVisibility { get; set; }
        //private string _appmode;
       // private int m_employeeid;
        public MainWindowVM(Window parent)
        {

  
            BackofficeClicked = new RelayCommand(ExecuteBackofficeClicked, param => true);
            TimecardClicked = new RelayCommand(ExecuteTimecardClicked, param => true);
            EODSettleClicked = new RelayCommand(ExecuteEODSettleClicked, param => true);


            _parent = parent;
            //_appmode = GlobalSettings.Instance.ApplicationMode;
            m_dbemployee = new DBEmployee();

            ButtonVisibility = new Visible();

            if (GlobalSettings.Instance.Demo) Demo = Visibility.Visible;
            else Demo = Visibility.Hidden;


            _security = new Security();
            SetStoreType();

            VFD.WriteRaw("VTT Red Dot POS", "Ready...");
   

        }

 

        public string StoreLogo
        {
            get { return "pack://siteoforigin:,,,/" + GlobalSettings.Instance.StoreLogo; }
        }

        public string MainBackgroundImage
        {
            get { return "pack://siteoforigin:,,,/" + GlobalSettings.Instance.MainBackgroundImage; }
        }
        public string RedDotLogo
        {
            get
            {
                if (MainBackgroundImage == "pack://siteoforigin:,,,/") return "/RedDot;component/media/sphere.png";
                else return "";
            }
        }

        public void SetStoreType()
        {

            string storetype;
            storetype = GlobalSettings.Instance.Shop.Type;

            //Default visible buttons for all service
            ButtonVisibility.BackOffice = Visibility.Visible;
            ButtonVisibility.BackUp = Visibility.Visible;


            ButtonVisibility.NoSale = Visibility.Visible;

            ButtonVisibility.History = Visibility.Visible;
            ButtonVisibility.Customer = Visibility.Visible;

            ButtonVisibility.GiftCertificate = Visibility.Visible;
            ButtonVisibility.GiftCard = Visibility.Visible;
            ButtonVisibility.EmployeeCommission = Visibility.Visible; //employee portal


     
            ButtonVisibility.Appointment = Visibility.Collapsed;
            ButtonVisibility.TimeCard = Visibility.Collapsed;
            ButtonVisibility.Operation = Visibility.Collapsed;
            ButtonVisibility.CashierIn = Visibility.Collapsed;
            ButtonVisibility.CashierOut = Visibility.Collapsed;
            ButtonVisibility.Refund = Visibility.Collapsed;
            ButtonVisibility.CreditCard = Visibility.Collapsed;
            ButtonVisibility.PayOut = Visibility.Collapsed;
            ButtonVisibility.CheckIn = Visibility.Collapsed;


            //hide or reveal buttons base on service
            switch (storetype)
            {

                case "Quick":
                    ButtonVisibility.QuickSales = Visibility.Visible;
                    ButtonVisibility.TimeCard = Visibility.Visible;
     

                    break;
                case "Retail":
                    ButtonVisibility.RetailSales = Visibility.Visible;
                    ButtonVisibility.Appointment = Visibility.Visible; //Reservations for restaurant
                    ButtonVisibility.TimeCard = Visibility.Visible;
                    ButtonVisibility.Operation = Visibility.Visible;


                    break;
             
                default:


                    break;


            }




        }

            //--------------------------------------------------------------  Button Enable ----------------------------------------------






        //---------------------------------------------------------------------- Button Commands ------------------------------------------------



        public void ExecuteBackofficeClicked(object button)
        {
            if (_security.WindowNewAccess("BackOffice"))
            {
                BackOfficeMenu backoffice = new BackOfficeMenu(_parent,_security);
                Utility.OpenModal(_parent, backoffice);
                _security.LogOff();
            }

          
        }


        public void ExecuteTimecardClicked(object button)
        {
            if (_security.WindowNewAccess("TimeCard"))
            {
                TimeCardView timecard = new TimeCardView(_security);
                Utility.OpenModal(_parent, timecard);
                _security.LogOff();
            }

           
        }

        public void ExecuteEODSettleClicked(object button)
        {
            if (_security.WindowNewAccess("EODSettle"))
            {
                EODSettle dlg = new EODSettle();
                dlg.ShowDialog();
                _security.LogOff();
            }
        }

   


   


    }


}
