using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class TableModel
    {

        private DBTable _dbtables;
        private DBSales _dbsales;
        public TableModel()
        {
            _dbtables = new DBTable();
            _dbsales = new DBSales();

        }

        public ObservableCollection<Table> LoadTables(int areaid)
        {

            ObservableCollection<Table> tablearea;
            tablearea = new ObservableCollection<Table>();


            DataTable data_table;
            Table table;



            /*
            for (int i = 0; i < 25;i++ )
            {
                table = new Table();
                table.ID =
                table.Number = i + 1;
       
                table.TableLeft = i % 8 * 100;
                table.TableTop = i / 8 * 100;
                table.TicketCount = _dbsales.GetTicketCountbyTable(i + 1);

                tablearea.Add(table);
            }
            */


            data_table = _dbtables.GetTables(areaid);
            foreach (DataRow tab in data_table.Rows)
            {
                table = new Table(tab);
              

                table.TicketCount = _dbsales.GetTicketCountbyTable(table.Number);
                table.ElapsedTime = _dbsales.GetTicketElapsedTimeTable(table.Number);
                table.Server = _dbsales.GetTicketServerByTable(table.Number);

                tablearea.Add(table);



            }

            return  tablearea;
        }


        public ObservableCollection<Area> LoadAreas(int current)
        {
            DataTable dt =  _dbtables.GetAreas();

            ObservableCollection<Area> areas = new ObservableCollection<Area>();


           

            foreach (DataRow row in dt.Rows)
            {
                Area newarea = new Area(row);
                if (newarea.ID== current) newarea.Selected = true;
                areas.Add(newarea);
            }

            if (dt.Rows.Count == 1) areas[0].Selected = true;
            return areas;
        }


        public void SaveTables(ObservableCollection<Table> m_tablelist)
        {
            foreach (Table tab in m_tablelist)
            {
                _dbtables.SaveTable(tab.ID, tab.TableLeft, tab.TableTop, tab.Width, tab.Height, tab.Number,tab.Seats,tab.Color);
            }
        }

        public void SaveAreaImage(int areaid, string imagesrc)
        {
            _dbtables.SaveAreaImage(areaid, imagesrc);
        }

        public int InsertArea(string area )
        {
            return _dbtables.InsertArea(area);
        }


        public bool InsertTable(int areaid,int left, int top, int width, int height, int number)
        {
            return _dbtables.InsertTable(areaid, left, top, width, height, number);
        }

        public DataTable GetMaxTable()
        {
            return _dbtables.GetMaxTable();
        }

        public string GetAreaName(int areaid)
        {
            return _dbtables.GetAreaName(areaid);
        }

        public Table GetTableByNumber(int tablenumber)
        {
            return new Table( _dbtables.GetTableByNumber(tablenumber));
        }

        public bool DeleteArea(int areaid)
        {
            return _dbtables.DeleteArea(areaid);
        }

        public bool DeleteTable(int tableid)
        {
            return _dbtables.DeleteTable(tableid);
        }
    }
}
