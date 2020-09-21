using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Http;

namespace RedDot.Controllers
{
    public class TableController: ApiController
    {
        private static TableModel m_tablemodel = new TableModel();

        public IHttpActionResult Get()
        {
            ObservableCollection<Area> Areas;
            Areas = m_tablemodel.LoadAreas(GlobalSettings.Instance.LastAreaId);

            IHttpActionResult rtn = Ok(Areas);

            return rtn;
        }


        public IHttpActionResult Get(int id)
        {
            ObservableCollection<Table> TableList;


            TableList = m_tablemodel.LoadTables(id);
            IHttpActionResult rtn = Ok(TableList);

            return rtn;

        }
         
    
    }
}
