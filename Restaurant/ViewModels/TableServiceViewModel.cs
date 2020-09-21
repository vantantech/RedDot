using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using RedDot;
using System.Windows.Input;
using System.Windows;

namespace RedDot
{
    public class TableServiceVM:INPCBase
    {

        private ObservableCollection<Table> m_tablelist;
        private TableModel m_tablemodel;
      
    
       private SecurityModel _security;
       private int m_currentareaid=0;

       public ICommand NewTableClicked { get; set; }

       public ICommand NewAreaClicked { get; set; }

       public ICommand DeleteTableClicked { get; set; }

       public ICommand DeleteAreaClicked { get; set; }


       public ICommand TableClicked { get; set; }

       public ICommand EditClicked { get; set; }

       public ICommand SaveClicked { get; set; }

       public ICommand AreaClicked { get; set; }

        public ICommand ImageClicked { get; set; }


       public ObservableCollection<TablePicture> TablePictures { get; set; }

        private ObservableCollection<Area> m_areas;
        public ObservableCollection<Area> Areas
        {
            get { return m_areas; }
            set
            {
                m_areas = value;
                NotifyPropertyChanged("Areas");
            }
        }

        private Window m_parent;

    
        private Area m_currentarea;
        public Area CurrentArea
        {
            get { return m_currentarea; }   
            set
            {
                m_currentarea = value;
                NotifyPropertyChanged("CurrentArea");
            }
        }

        public TableServiceVM(SecurityModel security, Window parent)
        {
            m_parent = parent;

            NewTableClicked = new RelayCommand(ExecuteNewTableClicked, param => this.CanAddTable);
            ImageClicked = new RelayCommand(ExecuteImageClicked, param => this.CanAddTable);
            NewAreaClicked = new RelayCommand(ExecuteNewAreaClicked, param => true);

            DeleteTableClicked = new RelayCommand(ExecuteDeleteTableClicked, param => this.CanDeleteTable);
            DeleteAreaClicked = new RelayCommand(ExecuteDeleteAreaClicked, param => this.CanDeleteArea);

            TableClicked = new RelayCommand(ExecuteTableClicked, param => true);
            EditClicked = new RelayCommand(ExecuteEditClicked, param => true);
            SaveClicked = new RelayCommand(ExecuteSaveClicked, param => true);
            AreaClicked = new RelayCommand(ExecuteAreaClicked, param => true);



            _security = security;

      
         
            m_tablemodel = new TableModel();



            TablePictures = new ObservableCollection<TablePicture>();

            TablePictures.Add(new TablePicture() { Description = "2 Seat",Seats=2, ImageSrc = "/media/tablepic2.png" });
            TablePictures.Add(new TablePicture() { Description = "4 Seat", Seats = 4, ImageSrc = "/media/tablepic4.png" });
            TablePictures.Add(new TablePicture() { Description = "6 Seat", Seats = 6, ImageSrc = "/media/tablepic6.png" });
            TablePictures.Add(new TablePicture() { Description = "8 Seat", Seats =8, ImageSrc = "/media/tablepic8.png" });
            TablePictures.Add(new TablePicture() { Description = "10 Seat", Seats = 10, ImageSrc = "/media/tablepic10.png" });


         
            Areas = m_tablemodel.LoadAreas(GlobalSettings.Instance.LastAreaId);

            foreach (Area area in Areas)
            {
                if (area.ID == GlobalSettings.Instance.LastAreaId) CurrentArea = area;
            }

            if (Areas.Count == 1)
            {
                GlobalSettings.Instance.LastAreaId = Areas[0].ID;
                CurrentArea = Areas[0];
            }


            TableList = m_tablemodel.LoadTables(GlobalSettings.Instance.LastAreaId);
            m_currentareaid = GlobalSettings.Instance.LastAreaId;


        }


        public void RefreshList()
        {
            TableList = m_tablemodel.LoadTables(GlobalSettings.Instance.LastAreaId);
            Areas = m_tablemodel.LoadAreas(GlobalSettings.Instance.LastAreaId);
        }


        private Table m_currenttable;
        public Table CurrentTable
        {
            get { return m_currenttable; }
            set
            {
                m_currenttable = value;
                NotifyPropertyChanged("CurrentTable");
            }
        }


     


        public ObservableCollection<Table> TableList
        {
            get
            {
                return m_tablelist;
            }

            set
            {
                m_tablelist = value;
                NotifyPropertyChanged("TableList");
            }
        }

        public bool CanAddTable
        {
            get { if (m_currentareaid > 0) return true; else return false; }
        }

        public bool CanDeleteTable
        {
            get { if (CurrentTable != null) return true; else return false; }
        }

        public bool CanDeleteArea
        {
            get {
                if (TableList == null) return false;

                if (m_currentareaid > 0 && TableList.Count == 0) return true;
            else return false; }
        }

        public void ExecuteTableClicked(object id)
        {

            if (_security.WindowAccess("TableService"))
            {
                int tableid = 0;
                if (id.ToString() != "") tableid = int.Parse(id.ToString());
                Table selected = new Table();

                //1) find out if table has any tickets
                foreach (Table tab in TableList)
                    if (tab.ID == tableid) selected = tab;


                

                //no tickets yet , then go straight to Order Creation
                if(selected.TicketCount == 0)
                {
                    int customercount = 0;

                    if(GlobalSettings.Instance.AskCustomerCount)
                    {
                        NumPad num = new NumPad("How many Customers?", true, false, selected.Seats.ToString());
                        Utility.OpenModal(m_parent, num);

                        if (num.Amount == "") return;

                        if (num.Amount != "") customercount = int.Parse(num.Amount);
                    }
                    QuickSales salonsales = new QuickSales(_security, null, selected.Number,customercount,OrderType.DineIn,SubOrderType.TableService);
                    Utility.OpenModal(m_parent, salonsales);
                    
                }else
                {
                    // take user to the open list
                    OpenTickets list = new OpenTickets(_security, 0, selected.Number) { Topmost = true };
                    list.ShowDialog();

                }

              

                m_parent.Close();
                _security.LogOff();
            }


        }

        public void ExecuteEditClicked(object id)
        {

          
                int tableid = 0;
                if (id.ToString() != "") tableid = int.Parse(id.ToString());

            foreach(Table tab in TableList)
            {
                if (tab.ID == tableid) CurrentTable = tab;
            }

            m_tablemodel.SaveTables(m_tablelist);

        }

        public void ExecuteSaveClicked(object obj)
        {

            m_tablemodel.SaveTables(m_tablelist);


        }

        public void ExecuteAreaClicked(object areano)
        {
            //save current tables before loading new ones
            m_tablemodel.SaveTables(m_tablelist);


            int area_id = 0;
            if (areano.ToString() != "") area_id = int.Parse(areano.ToString());
            GlobalSettings.Instance.LastAreaId = area_id;
            m_currentareaid = area_id;

            foreach(Area area in Areas)
            {
                if (area.ID == area_id) CurrentArea = area;
            }
          

            TableList = m_tablemodel.LoadTables(area_id);
            CurrentTable = null;
          

        }

        public void ExecuteNewTableClicked(object areano)
        {
            m_tablemodel.SaveTables(m_tablelist);


            DataTable last = m_tablemodel.GetMaxTable();
            if(last.Rows.Count > 0)
            {

                m_tablemodel.InsertTable(m_currentareaid, int.Parse(last.Rows[0]["tableleft"].ToString()) + 10, int.Parse(last.Rows[0]["tabletop"].ToString()) + 10, int.Parse(last.Rows[0]["width"].ToString()), int.Parse(last.Rows[0]["height"].ToString()), int.Parse(last.Rows[0]["number"].ToString()) + 1);
            }else
            {
                m_tablemodel.InsertTable(m_currentareaid,0, 0, 100,100,1);

            }

            TableList = m_tablemodel.LoadTables(m_currentareaid);
            CurrentTable = null;

            m_tablemodel.SaveTables(m_tablelist);
        }

        public void ExecuteNewAreaClicked(object areano)
        {

            TextPad tp = new TextPad("Add Area:", "") { Topmost = true };
            tp.ShowDialog();
            if (tp.ReturnText != null)
                if (tp.ReturnText != "")
                {

                    //save current tables before loading new ones
                    m_tablemodel.SaveTables(m_tablelist);



                    m_currentareaid = m_tablemodel.InsertArea(tp.ReturnText);
                    Areas = m_tablemodel.LoadAreas(GlobalSettings.Instance.LastAreaId);
                  
                    
                    CurrentArea = Areas.Last();
                    GlobalSettings.Instance.LastAreaId = CurrentArea.ID;



                    TableList = m_tablemodel.LoadTables(CurrentArea.ID);
                    CurrentTable = null;
                    m_currentareaid = CurrentArea.ID;
                }


        }


        public void ExecuteDeleteTableClicked(object obj)
        {

            if (CurrentTable == null) return;
             m_tablemodel.DeleteTable(CurrentTable.ID);

             Areas = m_tablemodel.LoadAreas(m_currentareaid);
             TableList = m_tablemodel.LoadTables(m_currentareaid);
             CurrentTable = null;

        }

        public void ExecuteDeleteAreaClicked(object areano)
        {


                    //save current tables before loading new ones
            m_tablemodel.DeleteArea(m_currentareaid);

              Areas = m_tablemodel.LoadAreas(m_currentareaid);

             TableList = m_tablemodel.LoadTables(m_currentareaid);
            CurrentTable = null;

            m_currentareaid = 0;


        }

        public void ExecuteImageClicked(object obj)
        {
            string NewImageSrc = Utility.GetImageFilePath(CurrentArea.BackGroundImage);
            CurrentArea.BackGroundImage = "pack://siteoforigin:,,,/" + NewImageSrc;
            m_tablemodel.SaveAreaImage(CurrentArea.ID, NewImageSrc);

        }

    }
}
