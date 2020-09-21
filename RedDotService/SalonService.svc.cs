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

     

        public SalonService()
        {
            m_dbticket = new DBTicket();
        }

        public bool CloseConnection()
        {
            return m_dbticket.CloseConnection();
        }


        public bool RemoveTicket(int userid,int ticketno)
        {
           
            return m_dbticket.DBRemoveTicket(userid, ticketno);
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
