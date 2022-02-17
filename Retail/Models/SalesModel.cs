
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Windows;

namespace RedDot
{
    public class SalesModel
    {
        private DBProducts _dbproducts;
        private DBSales _dbsales;
        private DBPromotions _dbpromotions;
        private DBTicket _dbticket;


        public SalesModel()
        {
            _dbproducts = new DBProducts();
            _dbsales = new DBSales();
            _dbpromotions = new DBPromotions();
            _dbticket = new DBTicket();

        }






        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        __  __      _   _               _     
        // |  _ \ _   _| |__ | (_) ___  |  \/  | ___| |_| |__   ___   __| |___ 
        // | |_) | | | | '_ \| | |/ __| | |\/| |/ _ \ __| '_ \ / _ \ / _` / __|
        // |  __/| |_| | |_) | | | (__  | |  | |  __/ |_| | | | (_) | (_| \__ \
        // |_|    \__,_|_.__/|_|_|\___| |_|  |_|\___|\__|_| |_|\___/ \__,_|___/
        //    
        //------------------------------------------------------------------------------------------  

        public void CreateShipOrder(int salesid)
        {
            _dbticket.CreateShipOrder(salesid);
        }

        public void DeleteShipment(int id)
        {
            _dbticket.DeleteShipment(id);
        }

        public void AddItemToShipment(int shiporderid,int salesitemid)
        {
            _dbticket.AddItemToShipment(shiporderid,salesitemid);
        }

        public void RemoveItemFromShipment(int shipitemid)
        {
            _dbticket.RemoveItemFromShipment(shipitemid);
        }

        public DataTable GetShipOrders(int salesid)
        {

           return  _dbticket.GetShipOrders(salesid);
        }

        public DataTable GetShipAvailableItems(int salesid)
        {

            return _dbticket.GetShipAvailableItems(salesid);
        }

        public DataTable GetShipSelectedItems(int shiporderid)
        {

            return _dbticket.GetShipSelectedItems(shiporderid);
        }

        public static int GetSalesCount(int employeeid)
        {
                DBSales _dbsales = new DBSales();
            DataTable dt;
            dt = _dbsales.GetSalesCount(employeeid);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["count"].ToString() != "")
                    return int.Parse(dt.Rows[0]["count"].ToString());
                else return 0;

            }
            else return 0;
        }


        /*
        public int AskEmployeeSelect(Window parent)
        {
            PickEmployee empl = new PickEmployee(parent);

            Utility.OpenModal(parent, empl);
            return empl.EmployeeID;
        }
        */

        public DataTable LoadOpenTicketsByEmployee(int employeeid, bool haspayment)
        {
            return _dbsales.GetOpenSalesbyEmployee(employeeid, haspayment);
        }
        public DataTable LoadAllOpenTickets(bool haspayment)
        {
            return _dbsales.GetAllOpenSales(haspayment);
        }

        public DataTable LoadOpenTicketsByCustomer(int customerid, bool haspayment)
        {
            return _dbsales.GetOpenSalesbyCustomer(customerid,haspayment);
        }

        public DataTable LoadOpenTicketsByTicket(int id)
        {
            return _dbsales.GetOpenSalesbyTicket(id);
        }
        public DataTable LoadOpenTicketsByDate(DateTime startdate, DateTime enddate, bool haspayment)
        {
            return _dbsales.GetOpenSalesbyDates(startdate, enddate,haspayment);
        }




        public DataTable GetProductsByCat(int catid)
        {

            return _dbproducts.GetProductsByCat(catid);
        }


        public DataTable GetRefundProducts(int salesid)
        {

            return _dbproducts.GetRefundProducts(salesid);
        }

        public DataTable FindProducts(string type, string description)
        {
            return _dbproducts.GetProductsByTypeDescription(type, description);

        }

        public void UpdateCost(int salesitemid, decimal newcost)
        {
            try
            {
                //if (Status == "Closed") return false;


              _dbticket.DBUpdateSalesItemValue(salesitemid, "cost", newcost);
            
            }
            catch (Exception e)
            {
                MessageBox.Show("Update Cost:" + e.Message);
             
            }
        }

        /*
        public ObservableCollection<Ticket> LoadTickets(Employee currentemployee)
        {


            DataTable dt;
            Ticket newticket;
            ObservableCollection<Ticket> ticketcollecttion = new ObservableCollection<Ticket>();


                dt =_dbsales.GetSalesbyEmployee( currentemployee.ID );



            foreach(DataRow row in dt.Rows)
            {
                newticket = new Ticket(int.Parse(row["ID"].ToString()));
                ticketcollecttion.Add(newticket);

            }

            return ticketcollecttion;
           
        }


        public ObservableCollection<Ticket> LoadAllOpenTickets()
        {


            DataTable dt;
            Ticket newticket;
            ObservableCollection<Ticket> ticketcollecttion = new ObservableCollection<Ticket>();


            dt = _dbsales.GetAllOpenSales();

            foreach (DataRow row in dt.Rows)
            {
                newticket = new Ticket(int.Parse(row["ID"].ToString()));
                ticketcollecttion.Add(newticket);

            }

            return ticketcollecttion;

        }

        */

        public bool ProcessCashTender(Window parent, Ticket currentticket)
        {
            try
            {
                CashTenderedView ctv = new CashTenderedView(parent, currentticket, 574);
                Utility.OpenModal(parent, ctv);

                currentticket.LoadPayment();
                currentticket.CloseTicket(); //need to load payment  to refresh object first before trying to close ticket
                return true;

            }
            catch (Exception e)
            {
                MessageBox.Show("Process Cash:" + e.Message);
                return false;
            }

        }

        public bool ProcessCheck(Window parent, Ticket currentticket)
        {

            try
            {
                decimal amt;
                NumPad ccv = new NumPad("Enter Check Amount",false, currentticket.Balance.ToString());
                Utility.OpenModal(parent, ccv);
                if (ccv.Amount == "") return false;

                amt = decimal.Parse(ccv.Amount);


                TextPad ccv2 = new TextPad("Enter Check Info","");
                Utility.OpenModal(parent, ccv2);



                currentticket.AddPayment("Check", amt, ccv2.ReturnText);
                return true;

            }
            catch (Exception e)
            {
                MessageBox.Show("ProcessCheck:" + e.Message);
                return false;
            }
        }


        public bool ProcessStoreCredit(Window parent, Ticket currentticket)
        {

            try
            {
                decimal amt;
                NumPad ccv = new NumPad("Enter Credit Amount",false, currentticket.Balance.ToString());
                Utility.OpenModal(parent, ccv);
                if (ccv.Amount == "") return false;

                amt = decimal.Parse(ccv.Amount);
                currentticket.AddPayment("Store Credit", amt, "");
                return true;

            }
            catch (Exception e)
            {
                MessageBox.Show("Process Store Credit:" + e.Message);
                return false;
            }
        }

        public bool ProcessGiftCertificate(Window parent, Ticket currentticket)
        {

            try
            {
                decimal amt;
                NumPad ccv = new NumPad("Enter Gift Certificate Amount",false, currentticket.Balance.ToString());
                Utility.OpenModal(parent, ccv);
                if (ccv.Amount == "") return false;

                amt = decimal.Parse(ccv.Amount);
                currentticket.AddPayment("Gift Certificate", amt, "");
                return true;

            }
            catch (Exception e)
            {
                MessageBox.Show("ProcessGiftCertificate:" + e.Message);
                return false;
            }
        }



        public static void EditShipOrder(Security m_security,Ticket m_ticket)
        {
            if (m_security.WindowAccess("ShippingOrder"))
            {


                AuditModel.WriteLog(m_security.CurrentEmployee.FullName, "Ship Order", "Open/Edit", "Ship Order",m_ticket.SalesID);

                ShipOrderEdit wk = new ShipOrderEdit(m_ticket);
                wk.ShowDialog();


            }

           
        }








    }
}
