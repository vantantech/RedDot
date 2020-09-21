using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.DataManager
{
   public  interface IDataInterface
    {
        //Settings

        bool BoolGetSetting(string category, string item, string description, string defaultvalue);

        int IntGetSetting(string category, string item, string description, string defaultvalue);

        decimal DecimalGetSetting(string category, string item, string description, string defaultvalue);

        string StringGetSetting(string category, string item, string description, string defaultvalue, string datatype="string", string data="");

        string GetSettingFromDB(string item);
        bool CreateStringSetting(string item, string value, string category, string description, string type, string data);
        DateTime GetDateSetting(string category, string item, string description, string defaultvalue);

        void BoolSaveSetting(string item, bool value);
        void IntSaveSetting(string item, int value);
        void DecimalSaveSetting(string item, decimal value);
        void StringSaveSetting(string item, string value);

        void SaveSetting(string item, string value);


        DataTable GetSettingsbyCategory(string cat);
        DataTable GetSettingCategories();
        DataTable GetSettingbyID(int id);

        bool WipeSalesData();
        bool WipeCustomerData();
        bool WipeGiftCardData();



        //Sales
        DataTable GetAllOpenSales();
   
        DataTable GetSalesbyID(int salesid);

        DataTable GetOpenSalesbyEmployee(int employeeid);

        DataTable GetOpenSalesbyCustomer(int customerid);

        DataTable GetOpenSalesbyTicket(int id);

        DataTable GetOpenSalesbyDates(DateTime startdate, DateTime enddate);

        DataTable GetClosedSalesbyDates(DateTime startdate, DateTime enddate);

        DataTable GetSalonSalesCount(int employeeid);

        DataTable GetPayment(string txn);
        DataTable GetPayment(int id);

        bool DBUpdateTipAmount(int paymentid, decimal tipamount, decimal totalamount);

        //bool DBUpdateTipAmount(int paymentid, decimal tipamount);

        bool UpdatePaymentCapture(int paymentid, decimal tipamount, decimal amount, decimal netamount, string transactionid);
        bool UpdatePaymentCapture(int paymentid, decimal tipamount, decimal amount);

        //Appointment
        bool DeleteAppt(int appointmentid);

        bool UpdateApptDate(DateTime apptdate, int appointmentid);

        bool UpdateEmployee(int employeeid, int appointmentid);

        bool UpdateCustomer(int customerid, int appointmentid);

        bool UpdateString(int id, string field, string fieldvalue);

        bool UpdateNumeric(int id, string field, int fieldvalue);

        bool CheckLength(DateTime apptdate, int employeeid, int length);

        bool SpaceAvailable(int currentapptid, DateTime apptdate, int employeeid, int length);

        int CreateNewAppointment(DateTime apptdate, int employeeid, int customerid, int length, string note);

        DataTable GetAppointments(int employeeid, DateTime appointmentdate);

        DataTable GetAppointments(DateTime appointmentdate);

        DataRow GetAppointment(int id);

        DataTable GetApptCategories(int apptid);

        void AddApptCategory(int apptid, int catid);
        void RemoveApptCategory(int catid);

    }


   






}
