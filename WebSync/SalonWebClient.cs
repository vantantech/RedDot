using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSync.ServiceReference1;
using System.Data;
using NLog;


namespace WebSync
{
    public class SalonWebClient
    {
        private DBTicket m_dbticket;
        private DBMenu m_dbmenu;
        private DBHistory m_dbhistory;
        private DBEmployee m_dbemployee;
        private DBConnect m_dbconnect;
        private static Logger logger = LogManager.GetCurrentClassLogger();
    
      public SalonWebClient()
        {
          
            m_dbticket = new DBTicket();
            m_dbmenu = new DBMenu();
            m_dbhistory = new DBHistory();
            m_dbemployee = new DBEmployee();
            m_dbconnect = new DBConnect();

        }



        public void SyncTicket(string storecode,string password, int salesid)
        {
            try
            {
                SalonServiceClient client = new SalonServiceClient();

             

                client.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://salon.reddotpos.com/SalonService.svc");
                client.Authenticate(storecode, password);

                SalesRecord sr = GetSaleRecord(salesid);

                sr.SaleItems =  GetSalesItemRecord(salesid);
                sr.PaymentRecords = GetPaymentRecord(salesid);
                sr.GratuityRecords = GetGratuityRecord(salesid);
              

                string result = client.WriteSalesTicket(0, sr);
            
                m_dbhistory.UpdateSyncDate(salesid);


                client.CloseConnection();
                client.Close();
                logger.Info("Ticket Synced Successfully:" + salesid.ToString());


            }
            catch (Exception ex)
            {
                logger.Error("SyncTicket:" + ex.Message);
              
            }

        }


        public void SyncEmployees(int userid)
        {
            try
            {
                SalonServiceClient client = new SalonServiceClient();
                client.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://salon.reddotpos.com/SalonService.svc");

                List<EmployeeRecord> employeelist = GetEmployeeRecord();

                foreach (var emp in employeelist)
                {
                    string result = client.WriteEmployeeList(userid, emp);
                   // m_dbconnect.Command("insert into audit (action,reason,tablename,recordid,username) values ('websync','" + result.Replace("'","[") + "','websync'," + emp.EmployeeId + ",'" + )");

                }

                client.CloseConnection();
                client.Close();
                logger.Info("Employee Synced Successfully:" + userid.ToString());
            }
            catch (Exception ex)
            {
                logger.Error("SyncEmployee:" + ex.Message);

            }
           
        }


        public void SyncMenu(string storecode, string password)
        {
            try
            {
                SalonServiceClient client = new SalonServiceClient();
                client.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://salon.reddotpos.com/SalonService.svc");
                string ret = client.Authenticate(storecode, password);

                string connstr = client.GetStatus();

             

                List<Category> catlist = GetCategoryRecord();

                foreach (var cat in catlist)
                {
                    string result = client.WriteCategory(cat);
 
                }

                client.CloseConnection();
                client.Close();
                logger.Info("Category Successfully:" );
            }
            catch (Exception ex)
            {
                logger.Error("SyncCategory:" + ex.Message);

            }

        }

        public  void SyncSalesItem(int userid, int salesid)
        {
            try
            {
                SalonServiceClient client = new SalonServiceClient();
                
             



                //-----------------------------------------------------Sales Item -------------------------------------------------
                var lineitems = m_dbticket.GetLineItems(salesid);

                foreach (DataRow rec in lineitems.Rows)
                {
                    SalesItemRecord si = new SalesItemRecord();
             
                    si.TicketNo= int.Parse(rec["salesid"].ToString());
                    si.Description = rec["description"].ToString();
                    si.Discount = decimal.Parse(rec["discount"].ToString());
                    si.Price = decimal.Parse(rec["price"].ToString());
                    si.Quantity = int.Parse(rec["quantity"].ToString());
                    si.EmployeeId = int.Parse(rec["employeeid"].ToString());
                    si.Note = rec["note"].ToString();
                    si.CommissionType = rec["commissiontype"].ToString();
                    si.Type = rec["type"].ToString();
                    si.Custom1 = rec["custom1"].ToString();
                    si.Custom2 = rec["custom2"].ToString();
                    si.Custom3 = rec["custom3"].ToString();
                    si.Custom4 = rec["custom4"].ToString();
                    si.ReportCategory = rec["reportcategory"].ToString();
                    si.CommissionAmt = decimal.Parse(rec["commissionamt"].ToString());
                    si.SupplyFee = decimal.Parse(rec["supplyfee"].ToString());
                    si.TurnValue = decimal.Parse(rec["turnvalue"].ToString());
                    si.RewardAmount = 0;
                    si.RewardException = 0;

                    client.WriteSalesItem(userid, si);

                }

           

            

            }
            catch (Exception ex)
            {
               // AuditModel.WriteLog("system", "Web Sync", ex.Message, "sales", salesid);
                logger.Error("SyncSalesItem:" + ex.Message);
            }

        }




        public  void SyncGratuity(int userid, int salesid)
        {
            try
            {
                SalonServiceClient client = new SalonServiceClient();
                           



                //-------------------------------------Gratuity ----------------------------------------

                var gratuities = m_dbticket.GetGratuities(salesid);
                if (gratuities.Rows.Count > 0)
                    foreach (DataRow rec in gratuities.Rows)
                    {
                        GratuityRecord gr = new GratuityRecord();

                  
                        gr.TicketNo = int.Parse(rec["salesid"].ToString());
                        gr.EmployeeId = int.Parse(rec["employeeid"].ToString());
                        gr.Amount = decimal.Parse(rec["amount"].ToString());

                        client.WriteSalesGratuity(userid, gr);

                    }

            }
            catch (Exception ex)
            {
              //  AuditModel.WriteLog("system", "Web Sync", ex.Message, "sales", salesid);
                logger.Error("SyncGratuity:" + ex.Message);
            }

        }

       
           
       
     
        private SalesRecord GetSaleRecord(int salesid)
        {
          
            SalesRecord sr = new SalesRecord();
            var dt = m_dbticket.GetSalesTicket(salesid);

            if (dt.Rows.Count > 0)
            {
                var rec = dt.Rows[0];
                sr.TicketNo = int.Parse(rec["Id"].ToString());
                sr.SalesDate = (DateTime)rec["saledate"];
                sr.LastUpdated = (DateTime)rec["lastupdated"];
                sr.Adjustment = decimal.Parse(rec["adjustment"].ToString());
                sr.Total = decimal.Parse(rec["total"].ToString());
                sr.SubTotal = decimal.Parse(rec["subtotal"].ToString());
                sr.Status = rec["status"].ToString();
                sr.Note = rec["note"].ToString();
                sr.EmployeeId = int.Parse(rec["employeeid"].ToString());
                sr.Custom1 = rec["custom1"].ToString();
                sr.Custom2 = rec["custom2"].ToString();
                sr.Custom3 = rec["custom3"].ToString();
                sr.Custom4 = rec["custom4"].ToString();
                sr.RewardException = int.Parse(rec["rewardexception"].ToString());
                if (rec["stationno"].ToString() != "") sr.StationNo = int.Parse(rec["stationno"].ToString());
                else sr.StationNo = 0;

                return sr;

            }
            else return null;
        }


        private  List<SalesItemRecord> GetSalesItemRecord(int salesid)
        {
           

            //-----------------------------------------------------Sales Item -------------------------------------------------
            var lineitems = m_dbticket.GetLineItems(salesid);


            List<SalesItemRecord> salesitems = new List<SalesItemRecord>();

            foreach (DataRow rec in lineitems.Rows)
            {
                SalesItemRecord si = new SalesItemRecord();

                si.TicketNo = int.Parse(rec["salesid"].ToString());
                si.Description = rec["description"].ToString();
                si.Discount = decimal.Parse(rec["discount"].ToString());
                si.Price = decimal.Parse(rec["price"].ToString()) + decimal.Parse(rec["surcharge"].ToString());
                si.Quantity = int.Parse(rec["quantity"].ToString());
                si.EmployeeId = int.Parse(rec["employeeid"].ToString());
                si.Note = rec["note"].ToString();
                si.CommissionType = rec["commissiontype"].ToString();
                si.Type = rec["type"].ToString();
                si.Custom1 = rec["custom1"].ToString();
                si.Custom2 = rec["custom2"].ToString();
                si.Custom3 = rec["custom3"].ToString();
                si.Custom4 = rec["custom4"].ToString();
                si.ReportCategory = rec["reportcategory"].ToString();
                si.CommissionAmt = decimal.Parse(rec["commissionamt"].ToString());
                si.SupplyFee = decimal.Parse(rec["supplyfee"].ToString());
                si.TurnValue = decimal.Parse(rec["turnvalue"].ToString());

                 si.RewardAmount = 0;
                si.RewardException = 0;

                // result = client.WriteSalesItem(userid, si);
                // AuditModel.WriteLog("system", "Web Sync", result, "salesitem", salesid);

                salesitems.Add(si);
            }


            return salesitems;
        }



        private  List<PaymentRecord> GetPaymentRecord(int salesid)
        {
           
            List<PaymentRecord> payments = new List<PaymentRecord>();

            //---------------------------------------------Payments ------------------------------------
            var pay = m_dbticket.GetPayments(salesid);
            bool hascardgroup = false;
            bool hasvoid = false;

            DataColumnCollection columns = pay.Columns;
            if (columns.Contains("cardgroup")) hascardgroup = true;
            if (columns.Contains("void")) hasvoid = true;

            foreach (DataRow rec in pay.Rows)
                {
                    PaymentRecord pm = new PaymentRecord();


                    pm.TicketNo = int.Parse(rec["salesid"].ToString());
                   if(hascardgroup)
                    pm.Description = rec["cardgroup"].ToString();
                   else
                    pm.Description = rec["description"].ToString();

                pm.Amount = decimal.Parse(rec["amount"].ToString());
                pm.NetAmount = decimal.Parse(rec["netamount"].ToString());

                if(hasvoid)
                {
                    if (rec["void"].ToString() == "0")
                    {

                        pm.AuthorCode = rec["authorcode"].ToString();
                    }
                    else
                    {

                        pm.AuthorCode = "VOID";
                    }
                }else
                {
                    pm.AuthorCode = rec["authorcode"].ToString();
                }
           
                   
                   

                    payments.Add(pm);
                }

                return payments;
       
   
        }


        private  List<GratuityRecord> GetGratuityRecord(int salesid)
        {
            

            List<GratuityRecord> gratuity = new List<GratuityRecord>();
            //-------------------------------------Gratuity ----------------------------------------

            var gratuities = m_dbticket.GetGratuities(salesid);
       
                foreach (DataRow rec in gratuities.Rows)
                {
                    GratuityRecord gr = new GratuityRecord();


                    gr.TicketNo = int.Parse(rec["salesid"].ToString());
                    gr.EmployeeId = int.Parse(rec["employeeid"].ToString());
                    gr.Amount = decimal.Parse(rec["amount"].ToString());

                    gratuity.Add(gr);
                }
                return gratuity;
        }

        private List<EmployeeRecord> GetEmployeeRecord()
        {

            List<EmployeeRecord> employee = new List<EmployeeRecord>();

            var employees = m_dbemployee.GetEmployeeList();
            foreach(DataRow rec in employees.Rows)
            {
                EmployeeRecord emp = new EmployeeRecord();
                emp.EmployeeId = int.Parse(rec["id"].ToString());
                emp.FirstName = rec["firstname"].ToString();
                emp.MiddleName = rec["middlename"].ToString();
                emp.LastName = rec["lastname"].ToString();
                emp.Active = int.Parse(rec["active"].ToString());
                employee.Add(emp);
               
            }

            return employee;
        }


        private  List<Category> GetCategoryRecord()
        {
            List<Category> category = new List<Category>();

            var categories = m_dbmenu.GetCategoryList();
            foreach(DataRow rec in categories.Rows)
            {
                Category cat = new Category();
                cat.cattype = rec["cattype"].ToString();
                cat.id = int.Parse(rec["id"].ToString());
                cat.description = rec["description"].ToString();
                cat.colorcode = rec["colorcode"].ToString();
                cat.imagesrc = rec["imagesrc"].ToString();
                cat.lettercode = rec["lettercode"].ToString();
                cat.sortorder = int.Parse(rec["sortorder"].ToString());
                category.Add(cat);
            }

            return category;
        }
    }
}
