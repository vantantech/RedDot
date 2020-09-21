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
    public class Area:INPCBase
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int NumberOfTables { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string BackColor { get; set; }

        private string backgroundimage;
        [DataMember]
        public string BackGroundImage
        {
            get { return backgroundimage; }
            set
            {
                backgroundimage = value;
                NotifyPropertyChanged("BackGroundImage");
            }
        }
        public bool Selected { get; set; }

        public Area(DataRow row)
        {
            ID = (int)row["id"];
            if (row["numtables"].ToString() != "") NumberOfTables = int.Parse(row["numtables"].ToString()); else NumberOfTables = 0;
            Description = row["description"].ToString();
            BackColor = row["backcolor"].ToString();
            BackGroundImage = "pack://siteoforigin:,,,/" + row["backgroundimage"].ToString();
            Selected = false;
        }
    }
}
