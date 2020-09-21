using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class ShipOrderVM:INPCBase
    {
        public ICommand OKClicked { get; set; }

        public ICommand PrintWorkOrderClicked { get; set; }

        public ICommand PrintPDFClicked { get; set; }

        public ICommand NewClicked { get; set; }

        public ICommand AvailableProductClicked { get; set; }

        public ICommand SelectedProductClicked { get; set; }

        public ICommand DeleteClicked { get; set; }

        public Ticket CurrentTicket { get; set; }

        public List<DropDownListType> ShipperList { get; set; }

        Window m_parent;

        public int SelectedIndex { get; set; }

        private ObservableCollection<ShipOrder> _shipments;

        public ObservableCollection<ShipOrder> Shipments {
            get { return _shipments; }

            set
            {
                _shipments = value;
                NotifyPropertyChanged("Shipments");
            }
        }


        private DataTable _available;
        public DataTable Available
        {
            get { return _available; }
            set
            {
                _available = value;
                NotifyPropertyChanged("Available");
            }
        }


        public SalesModel m_salesmodel;


        public ShipOrderVM(Window parent,Ticket ticket)
        {
            CurrentTicket = ticket;
            m_parent = parent;
            OKClicked = new RelayCommand(ExecuteOKClicked, param => true);
            PrintWorkOrderClicked = new RelayCommand(ExecutePrintWorkOrderClicked, param => true);
            PrintPDFClicked = new RelayCommand(ExecutePrintPDFClicked, param => true);
            AvailableProductClicked = new RelayCommand(ExecuteAvailableProductClicked, param => true);
            SelectedProductClicked = new RelayCommand(ExecuteSelectedProductClicked, param => true);

            NewClicked = new RelayCommand(ExecuteNewClicked, param => true);
            DeleteClicked = new RelayCommand(ExecuteDeleteClicked, param => true);

            m_salesmodel = new SalesModel();

            ShipperList = new List<DropDownListType>();
            ShipperList.Add(new DropDownListType() { Name = "Fedex" });
            ShipperList.Add(new DropDownListType() { Name = "UPS" });
            ShipperList.Add(new DropDownListType() { Name = "DHL" });
            ShipperList.Add(new DropDownListType() { Name = "USPS" });
            ShipperList.Add(new DropDownListType() { Name = "Other" });


            LoadShipments();
            LoadAvailable();
        }

        private void LoadAvailable()
        {
            Available = m_salesmodel.GetShipAvailableItems(CurrentTicket.SalesID);
        }

        private DataTable LoadSelected(int shiporderid)
        {
            return m_salesmodel.GetShipSelectedItems(shiporderid);
        }

        private void LoadShipments()
        {
            int sel = SelectedIndex;

            Shipments = new ObservableCollection<ShipOrder>();
            DataTable cat = m_salesmodel.GetShipOrders(CurrentTicket.SalesID);

            int tabindex = 0;
            foreach (DataRow row in cat.Rows)
            {
                ShipOrder tabitem = new ShipOrder(row);
                tabitem.IsEnabled = true;
                tabitem.Selected = LoadSelected(tabitem.id);
                tabitem.TabIndex = tabindex;
                tabindex++;
             
                Shipments.Add(tabitem);
            }
            NotifyPropertyChanged("Shipments");
            SelectedIndex = sel;
            NotifyPropertyChanged("SelectedIndex");
        }

        public void ExecuteOKClicked(object obj)
        {
         
            m_parent.Close();

        }


        //no longer used
        public void ExecutePrintWorkOrderClicked(object obj)
        {
          //  CurrentTicket.WorkOrder.Print();

        }

        public void ExecutePrintPDFClicked(object obj)
        {


          //  CurrentTicket.WorkOrder.PrintPDF(true);


        }

        public void ExecuteNewClicked(object obj)
        {

            m_salesmodel.CreateShipOrder(CurrentTicket.SalesID);


            LoadShipments();
        }

        public void ExecuteDeleteClicked(object obj)
        {
            int id = (int)obj;

            Confirm conf = new Confirm("Delete this shipment??!!");
            conf.ShowDialog();

            if(conf.Action.ToUpper() == "YES")
            {
                m_salesmodel.DeleteShipment(id);

                LoadShipments();
                LoadAvailable();
            }
           
        }

        public void ExecuteAvailableProductClicked(object obj)
        {
            int id = (int)obj;

            ShipOrder ship = Shipments.Where(x => x.TabIndex == SelectedIndex).FirstOrDefault();
            m_salesmodel.AddItemToShipment(ship.id,id);



            ship.Selected = LoadSelected(ship.id);
            LoadAvailable();

        }

        public void ExecuteSelectedProductClicked(object obj)
        {
            int id = (int)obj;

            ShipOrder ship = Shipments.Where(x => x.TabIndex == SelectedIndex).FirstOrDefault();
            m_salesmodel.RemoveItemFromShipment(id);



            ship.Selected = LoadSelected(ship.id);
            LoadAvailable();
        }
    }
}
