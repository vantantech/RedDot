using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    [DataContract]
    public class Table : INPCBase
    {

        private int _number;
        private int _tableleft;
        private int _tabletop;
        private int _height;
        private int _width;
        private int _areaid;
        private int _seats;

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public Ticket CurrentTicket { get; set; }

        [DataMember]
        public int TableLeft {
            get { return _tableleft; }
            set { _tableleft = value;
            NotifyPropertyChanged("TableLeft");
            }
        }

        [DataMember]
        public int TableTop {
            get { return _tabletop; }
            set { _tabletop = value;
            NotifyPropertyChanged("TableTop");
            }
        }

        [DataMember]
        public int AreaId
        {
            get { return _areaid; }
            set
            {
                _areaid = value;
                NotifyPropertyChanged("AreaId");
            }
        }

        private int _elapsedtime;
        [DataMember]
        public int ElapsedTime
        {
            get { return _elapsedtime; }
            set
            {
                _elapsedtime = value;
                NotifyPropertyChanged("ElapsedTime");
            }
        }
        [DataMember]
        public int Seats
        {
            get { return _seats; }
            set
            {
                _seats = value;
                NotifyPropertyChanged("Seats");
                NotifyPropertyChanged("ImageSrc");
            }
        }

        private string _server;
        [DataMember]
        public string Server
        {
            get { return _server; }
            set
            {
                _server = value;
                NotifyPropertyChanged("Server");
            }
        }

        [DataMember]
        public string ImageSrc
        {
            get
            {
                return "/media/tablepic" + _seats + ".png";

            }

        }

        [DataMember]
        public int Height {
            get { return _height; }
            set
            {
                _height = value;
                NotifyPropertyChanged("Height");
                NotifyPropertyChanged("ImageHeight");
            }
    }

        [DataMember]
        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                NotifyPropertyChanged("Width");
                NotifyPropertyChanged("ImageWidth");
            }
        }

        [DataMember]
        public int ImageHeight
        {
            get { return _height-5; }
  
        }

        [DataMember]
        public int ImageWidth
        {
            get { return _width-5; }
     
        }

        [DataMember]
        public int TicketCount { get; set; }


        private string color;
        [DataMember]
        public string Color
        {
            get { return color; }
            set
            {
                color = value;
                NotifyPropertyChanged("Color");
            }
        }


        [DataMember]
        public int Number
        {
            get
            {
                return _number;
            }

            set
            {
                _number = value;
                NotifyPropertyChanged("Number");
            }
        }

        public Table()
        {

        }

        public Table(DataRow row)
        {

            if (row == null) return;

            ID = int.Parse(row["id"].ToString());
            Number = int.Parse(row["number"].ToString());
            Width = int.Parse(row["width"].ToString());
            Height= int.Parse(row["height"].ToString());
            TableTop = int.Parse(row["tabletop"].ToString());
            TableLeft = int.Parse(row["tableleft"].ToString());
            AreaId = int.Parse(row["areaid"].ToString());
            Seats = int.Parse(row["seats"].ToString());
            Color = row["color"].ToString();

            if (Color == "") Color = "transparent";
        }


    }
}
