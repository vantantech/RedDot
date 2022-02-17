using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.DataManager
{
    public class MySQLData:IDataInterface
    {
        DBSales dbsales = new DBSales();
        MYSQLDBAppointment dbappt = new MYSQLDBAppointment();
        MySQLDBSettings dbsettings = new MySQLDBSettings();

        public MySQLData()
        {

        }

        //Settings
        public bool BoolGetSetting(string category, string item, string description, string defaultvalue)
        {
            return dbsettings.BoolGetSetting(category, item, description, defaultvalue);
        }
        public int IntGetSetting(string category, string item, string description, string defaultvalue)
        {
            return dbsettings.IntGetSetting(category, item, description, defaultvalue);
        }
        public decimal DecimalGetSetting(string category, string item, string description, string defaultvalue)
        {
            return dbsettings.DecimalGetSetting(category, item, description, defaultvalue);
        }
        public string StringGetSetting(string category, string item, string description, string defaultvalue, string datatype, string data)
        {
            return dbsettings.StringGetSetting(category, item, description, defaultvalue, datatype,data);
        }
        public string GetSettingFromDB(string item)
        {
            return dbsettings.GetSettingFromDB(item);
        }
        public bool CreateStringSetting(string item, string value, string category, string description, string type, string data)
        {
            return dbsettings.CreateStringSetting(item, value, category, description, type, data);
        }
        public DateTime GetDateSetting(string category, string item, string description, string defaultvalue)
        {
            return dbsettings.GetDateSetting(category, item, description, defaultvalue);
        }
        public void SaveSetting(string item, string value)
        {
            dbsettings.SaveSetting(item, value);
        }

        public void BoolSaveSetting(string item, bool value)
        {
            dbsettings.BoolSaveSetting(item, value);
        }

        public void IntSaveSetting(string item, int value)
        {
            dbsettings.IntSaveSetting(item, value);
        }

        public void DecimalSaveSetting(string item, decimal value)
        {
            dbsettings.DecimalSaveSetting(item, value);
        }

        public void StringSaveSetting(string item, string value)
        {
            dbsettings.StringSaveSetting(item, value);
        }

















        public DataTable GetSettingsbyCategory(string cat)
        {
            return dbsettings.GetSettingsbyCategory(cat);
        }
        public DataTable GetSettingCategories()
        {
            return dbsettings.GetSettingCategories();
        }
        public DataTable GetSettingbyID(int id)
        {
            return dbsettings.GetSettingbyID(id);
        }

        public bool WipeSalesData()
        {
            return dbsettings.WipeSalesData();
        }

        public bool WipeSalesDataCustom(DateTime startdate, DateTime enddate)
        {
            return dbsettings.WipeSalesDataCustom(startdate,enddate);
        }

        public bool WipeCustomerData()
        {
            return dbsettings.WipeCustomerData();
        }

        public bool WipeGiftCardData()
        {
            return dbsettings.WipeGiftCardData();
        }
        //Appointment
        public bool DeleteAppt(int appointmentid)
        {
           return dbappt.DeleteAppt(appointmentid);
        }

        public bool UpdateApptDate(DateTime apptdate, int appointmentid)
        {
            return dbappt.UpdateApptDate(apptdate, appointmentid);
        }

        public bool UpdateEmployee(int employeeid, int appointmentid)
        {
            return dbappt.UpdateEmployee(employeeid, appointmentid);
        }

        public bool UpdateCustomer(int customerid, int appointmentid)
        {
            return dbappt.UpdateCustomer(customerid, appointmentid);
        }

        public bool UpdateString(int id, string field, string fieldvalue)
        {
            return dbappt.UpdateString(id, field, fieldvalue);
        }

        public bool UpdateNumeric(int id, string field, int fieldvalue)
        {
            return dbappt.UpdateNumeric(id, field, fieldvalue);
        }

        public bool CheckLength(DateTime apptdate, int employeeid, int length)
        {
            return dbappt.CheckLength(apptdate, employeeid, length);
        }

        public bool SpaceAvailable(int currentapptid, DateTime apptdate, int employeeid, int length)
        {
            return dbappt.SpaceAvailable(currentapptid, apptdate, employeeid, length);
        }

        public int CreateNewAppointment(DateTime apptdate, int employeeid, int customerid, int length, string note)
        {
            return dbappt.CreateNewAppointment(apptdate, employeeid, customerid, length, note);
        }

        public DataTable GetAppointments(int employeeid, DateTime appointmentdate)
        {
            return dbappt.GetAppointments(employeeid, appointmentdate);
        }

        public DataTable GetAppointments(DateTime appointmentdate)
        {
            return dbappt.GetAppointments(appointmentdate);
        }

        public DataRow GetAppointment(int id)
        {
            return dbappt.GetAppointment(id);
        }

        public DataTable GetApptCategories(int apptid)
        {
            return dbappt.GetApptCategories(apptid);
        }

        public void AddApptCategory(int apptid, int catid)
        {
            dbappt.AddApptCategory(apptid, catid);
        }

        public void RemoveApptCategory(int catid)
        {
            dbappt.RemoveApptCategory( catid);
        }


        //-------------------------------  SALES ------------------------------------------------

        // public bool DBUpdateTipAmount(int paymentid, decimal tipamount)
        // {
        //     return dbsales.DBUpdateTipAmount(paymentid, tipamount);
        // }


        public bool DBUpdateTipAmount(int paymentid, decimal tipamount, decimal totalamount)
        {
            return dbsales.DBUpdateTipAmount(paymentid, tipamount, totalamount);
        }

        public bool UpdatePaymentCapture(int paymentid, decimal tipamount, decimal amount, decimal netamount, string transactionid)
        {
            return dbsales.UpdatePaymentCapture(paymentid, tipamount, amount, netamount, transactionid);
        }
        public bool UpdatePaymentCapture(int paymentid, decimal tipamount, decimal amount)
        {
            return dbsales.UpdatePaymentCapture(paymentid, tipamount, amount);
        }

        public DataTable GetAllOpenSales()
        {
            return dbsales.GetAllOpenSales();
        }

        public DataTable GetClosedSalesbyDates(DateTime startdate, DateTime enddate)
        {
            return dbsales.GetClosedSalesbyDates(startdate, enddate);
        }

        public DataTable GetOpenSalesbyCustomer(int customerid)
        {
            return dbsales.GetOpenSalesbyCustomer(customerid);
        }

        public DataTable GetOpenSalesbyDates(DateTime startdate, DateTime enddate)
        {
            return dbsales.GetOpenSalesbyDates(startdate, enddate);
        }

        public DataTable GetOpenSalesbyEmployee(int employeeid)
        {
            return dbsales.GetOpenSalesbyEmployee(employeeid);
        }

        public DataTable GetOpenSalesbyTicket(int id)
        {
            return dbsales.GetOpenSalesbyTicket(id);
        }

        public DataTable GetPayment(string txn)
        {
            return dbsales.GetPayment(txn);
        }

        public DataTable GetPayment(int id)
        {
            return dbsales.GetPayment(id);
        }

        public DataTable GetSalesbyID(int salesid)
        {
            return dbsales.GetSalesbyID(salesid);
        }

        public DataTable GetSalonSalesCount(int employeeid)
        {
            return dbsales.GetSalonSalesCount(employeeid);
        }

      
    }
}
