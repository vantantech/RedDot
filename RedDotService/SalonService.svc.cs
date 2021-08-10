using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;


namespace RedDotService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class SalonService : ISalonService
    {
        DBTicket m_dbticket;
        DBMenu m_dbmenu;

        DB_A0B2A3_webaccessEntities context;

        string m_connectionstring="";

        public SalonService()
        {
            context = new DB_A0B2A3_webaccessEntities();

        }

        public string Authenticate(string storecode, string password)
        {
            try
            {
          
                var store = context.stores.Where(x => x.storecode == storecode).FirstOrDefault();


                //var salt = store.salt;
                // var pass = HMac.ComputeHMAC_SHA256(Encoding.ASCII.GetBytes(password), Encoding.ASCII.GetBytes(salt));
                // string hmac = System.Text.Encoding.ASCII.GetString(pass);

                var hashedpassword = store.hmac;

                if (HMac.VerifyHashedPassword(hashedpassword, password))
                {
                    m_connectionstring = store.connectionstring;
                }
                else return "User/pass not found";


                m_dbticket = new DBTicket(m_connectionstring);
                m_dbmenu = new DBMenu(m_connectionstring);

                return m_dbmenu.GetStatus();
            }catch(Exception ex)
            {
                return ex.Message;
            }

        }
   

        public bool CloseConnection()
        {
            return m_dbticket.CloseConnection();
        }


        public bool RemoveTicket(int userid,int ticketno)
        {
           
            return m_dbticket.DBRemoveTicket( ticketno);
        }

    



        public string WriteSalesTicket(int clientid, SalesRecord salesrecord)
        {
            try
            {
                var result = m_dbticket.DBCreateTicket(clientid,salesrecord);

                if(salesrecord.SaleItems != null)
                {
                    foreach(var saleitem in salesrecord.SaleItems)
                    {
                        WriteSalesItem(clientid, saleitem);
                    }
                }


                if (salesrecord.PaymentRecords != null)
                {
                    foreach (var item in salesrecord.PaymentRecords)
                    {
                        WriteSalesPayment(clientid, item);
                    }
                }

                if (salesrecord.GratuityRecords != null)
                {
                    foreach (var item in salesrecord.GratuityRecords)
                    {
                        WriteSalesGratuity(clientid, item);
                    }
                }


                return result;
            }catch(Exception ex)
            {
                return "Error:" + ex.Message;
            }

            
            
        }

        public string WriteSalesItem(int clientid, SalesItemRecord salesitem)
        {
            

           return  m_dbticket.DBCreateSalesItem(clientid,  salesitem);
          
        }

        public string WriteSalesPayment(int clientid, PaymentRecord payment)
        {
           

            return m_dbticket.DBCreateSalesPayment(clientid, payment);
        }

        public string WriteSalesGratuity(int clientid, GratuityRecord gratuity)
        {
           

            return m_dbticket.DBCreateSalesGratuity(clientid, gratuity);
        }


        public string WriteEmployeeList(int clientid, EmployeeRecord employee)
        {
            return m_dbticket.DBCreateEmployee(clientid, employee);
        }

        public LicenseRequest GetLicense(LicenseRequest request, string publickey)
        {
            return Licensing.GetLicense(request, publickey);
        }

        public string WriteCategory(Category cat)
        {
            return m_dbmenu.DBInsertCategory(cat);
        }

        public string GetConnectionString()
        {
            return m_connectionstring;
        }

        public string GetStatus()
        {
            if (m_dbmenu is null) return "null";
            return m_dbmenu.GetStatus();
        }



        /*  public CompositeType GetDataUsingDataContract(CompositeType composite)
          {
              if (composite == null)
              {
                  throw new ArgumentNullException("composite");
              }
              if (composite.BoolValue)
              {
                  composite.StringValue += "Suffix";
              }
              return composite;
          }

         */
    }





}
