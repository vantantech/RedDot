using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
   public class ShipOrder:INPCBase
    {

        private DBTicket dbTicket = new DBTicket();

        public int id { get; set; }
        public int salesid { get; set; }
        public string ShipmentNumber { get; set; }
        public bool IsEnabled { get; set; }
        public int TabIndex { get; set; }


        private string _custname;
        public string CustName {
            get { return _custname; }
            set
            {
                _custname = value;
                dbTicket.UpdateShipOrderString(id, nameof(CustName), value);
                NotifyPropertyChanged(nameof(CustName));
            }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                dbTicket.UpdateShipOrderString(id, nameof(Address), value);
                NotifyPropertyChanged(nameof(Address));
            }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
                dbTicket.UpdateShipOrderString(id, nameof(City), value);
                NotifyPropertyChanged(nameof(City));
            }
        }

        private string _state;
        public string State
        {
            get { return _state; }
            set
            {
                _state = value;
                dbTicket.UpdateShipOrderString(id, nameof(State), value);
                NotifyPropertyChanged(nameof(State));
            }
        }

        private string _zipcode;
        public string ZipCode
        {
            get { return _zipcode; }
            set
            {
                _zipcode = value;
                dbTicket.UpdateShipOrderString(id, nameof(ZipCode), value);
                NotifyPropertyChanged(nameof(ZipCode));
            }
        }


        private string _phonenumber;
        public string PhoneNumber
        {
            get { return _phonenumber; }
            set
            {
                _phonenumber = value;
                dbTicket.UpdateShipOrderString(id, nameof(PhoneNumber), value);
                NotifyPropertyChanged(nameof(PhoneNumber));
            }
        }


        private string _shipper;
        public string Shipper
        {
            get { return _shipper; }
            set
            {
                _shipper = value;
                dbTicket.UpdateShipOrderString(id, nameof(Shipper), value);
                NotifyPropertyChanged(nameof(Shipper));
            }
        }


        private string _shipdate;
        public string ShipDate
        {
            get { return _shipdate; }
            set
            {
                _shipdate = value;
                dbTicket.UpdateShipOrderString(id, nameof(ShipDate), value);
                NotifyPropertyChanged(nameof(ShipDate));
            }
        }


        private string _trackingno;
        public string TrackingNo
        {
            get { return _trackingno; }
            set
            {
                _trackingno = value;
                dbTicket.UpdateShipOrderString(id, nameof(TrackingNo), value);
                NotifyPropertyChanged(nameof(TrackingNo));
            }
        }


        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                dbTicket.UpdateShipOrderString(id, nameof(Notes), value);
                NotifyPropertyChanged(nameof(Notes));
            }
        }

        private DataTable _selected;
        public DataTable Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
     
                NotifyPropertyChanged(nameof(Selected));
            }
        }

        public ShipOrder(DataRow row)
        {
            salesid = int.Parse(row["salesid"].ToString());
            CustName = row["custname"].ToString();
            Address = row["address"].ToString();
            City = row["city"].ToString();
            State = row["state"].ToString();
            ZipCode = row["zipcode"].ToString();

            Shipper = row["shipper"].ToString();
            ShipDate = row["shipdate"].ToString();
            TrackingNo = row["trackingno"].ToString();
            Notes = row["notes"].ToString();
            PhoneNumber = row["phonenumber"].ToString();

            ShipmentNumber = row["id"].ToString();
            id = int.Parse(ShipmentNumber);
        }


  

 

    }
}
