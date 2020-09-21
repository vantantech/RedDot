using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RedDot;
using RedDot.DataManager;


namespace RedDot
{
    public class EmployeeSimple : INPCBase
    {

        private string _imgurl;

        private DBEmployee _dbEmployee;





        private string _displayname;

        public int ID { get; set; }

        private int _sortorder;



        private DataRow m_datarow;



        public EmployeeSimple(DataRow employeerow)
        {
            m_datarow = employeerow;
            InitEmployee(employeerow);
            if (_dbEmployee == null) _dbEmployee = new DBEmployee();
        }




        public void InitEmployee(DataRow employeerow)
        {
            if (employeerow["id"].ToString() != "") ID = int.Parse(employeerow["id"].ToString()); else ID = 0;


            if (employeerow["sortorder"].ToString() != "") _sortorder = int.Parse(employeerow["sortorder"].ToString()); else _sortorder = 0;

            _imgurl = employeerow["imagesrc"].ToString().Replace("\\", "\\\\");

            _displayname = employeerow["displayname"].ToString();
        }

        private void InitDefaultEmployee()
        {


            ID = 0;

            _imgurl = "Images\\\\exclamation.jpg";



        }




        public string DisplayName
        {
            get { return _displayname; }
            set
            {
                _displayname = value;
                _dbEmployee.UpdateString(ID, "DisplayName", _displayname);
                NotifyPropertyChanged("DisplayName");

            }
        }




        public int SortOrder
        {
            get { return _sortorder; }
            set
            {
                _sortorder = value;
                _dbEmployee.UpdateNumeric(ID, "sortorder", _sortorder);
                NotifyPropertyChanged("SortOrder");
            }
        }




        public string ImageSrc
        {
            get
            {
                return "pack://siteoforigin:,,,/" + _imgurl;
                // return "pack://application:,,,/" + _imgurl;

            }
            set
            {
                if (value != null)
                {
                    _imgurl = value;
                    _dbEmployee.UpdateString(ID, "imagesrc", _imgurl);
                    NotifyPropertyChanged("ImageSrc");
                }

            }
        }
    }

 }
       



  

