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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ISalonService
    {

        [OperationContract]
        string Authenticate(string storecode, string password);


        [OperationContract]
        bool CloseConnection();




        [OperationContract]
        bool RemoveTicket(int userid, int ticketno);




        [OperationContract]
        string WriteSalesTicket(int clientid, SalesRecord salesrecord);


        [OperationContract]
        string WriteSalesItem(int clientid, SalesItemRecord salesitem);





        [OperationContract]
        string WriteSalesPayment(int clientid, PaymentRecord payment);



        [OperationContract]
        string WriteSalesGratuity(int clientid, GratuityRecord gratuity);

        [OperationContract]
        string WriteEmployeeList(int clientid, EmployeeRecord employee);

        [OperationContract]
        LicenseRequest GetLicense(LicenseRequest request, string publickey);


        [OperationContract]
        string WriteCategory(Category cat);

        [OperationContract]
        string GetConnectionString();

        [OperationContract]
        string GetStatus();

    }




    [DataContract]
    public class SalesRecord
    {
        [DataMember]
        public int TicketNo { get; set; }
        [DataMember]
        public int CustomerId { get; set; }
        [DataMember]
        public DateTime SalesDate { get; set; }
        [DataMember]
        public DateTime LastUpdated { get; set; }
        [DataMember]
        public decimal Adjustment { get; set; }
        [DataMember]
        public decimal Total { get; set; }
        [DataMember]
        public decimal SubTotal { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Note { get; set; }
        [DataMember]
        public int EmployeeId { get; set; }
        [DataMember]
        public string Custom1 { get; set; }
        [DataMember]
        public string Custom2 { get; set; }
        [DataMember]
        public string Custom3 { get; set; }
        [DataMember]
        public string Custom4 { get; set; }
        [DataMember]
        public int RewardException { get; set; }
        [DataMember]
        public int StationNo { get; set; }
        [DataMember]
        public List<SalesItemRecord> SaleItems { get; set; }
        [DataMember]
        public List<PaymentRecord> PaymentRecords { get; set; }
        [DataMember]
        public List<GratuityRecord> GratuityRecords { get; set; }
    }

    [DataContract]
    public class SalesItemRecord
    {


        [DataMember]
        public int TicketNo { get; set; }
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public decimal Discount { get; set; }

        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public int Quantity { get; set; }
        [DataMember]
        public int EmployeeId { get; set; }
        [DataMember]
        public string Note { get; set; }
        [DataMember]
        public string CommissionType { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Custom1 { get; set; }
        [DataMember]
        public string Custom2 { get; set; }
        [DataMember]
        public string Custom3 { get; set; }
        [DataMember]
        public string Custom4 { get; set; }

        [DataMember]
        public string ReportCategory { get; set; }
        [DataMember]
        public decimal CommissionAmt { get; set; }
        [DataMember]
        public decimal SupplyFee { get; set; }
        [DataMember]
        public decimal TurnValue { get; set; }
        [DataMember]
        public decimal RewardAmount { get; set; }
        [DataMember]
        public int RewardException { get; set; }

    }


    [DataContract]
    public class PaymentRecord
    {


        [DataMember]
        public int TicketNo { get; set; }
        [DataMember]
        public string Description { get; set; }


        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public decimal NetAmount { get; set; }

        [DataMember]
        public string AuthorCode { get; set; }



    }



    [DataContract]
    public class GratuityRecord
    {


        [DataMember]
        public int TicketNo { get; set; }
        [DataMember]
        public int EmployeeId { get; set; }


        [DataMember]
        public decimal Amount { get; set; }




    }


    [DataContract]
    public class EmployeeRecord
    {
        [DataMember]
        public int EmployeeId { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public int Active { get; set; }

    }


    [DataContract]
    public class LicenseRequest
    {
        [DataMember]
        public string MachineID { get; set; }
        [DataMember]
        public string Application { get; set; }
        [DataMember]
        public string CodeString { get; set; }
        [DataMember]
        public string CreateDate { get; set; }
        [DataMember]
        public bool Activated { get; set; }
        [DataMember]
        public string Comment { get; set; }

    }

    [DataContract]
    public class Category
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string colorcode { get; set; }
        [DataMember]
        public string lettercode { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string imagesrc { get; set; }
        [DataMember]
        public string cattype { get; set; }
        [DataMember]
        public int sortorder { get; set; }
    }
}