using System;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

using System.Data;

using System.Configuration;
using Microsoft.Win32;
using System.Data.SqlClient;


namespace RedDotService
{
    public class DBConnect
    {
        private SqlConnection _connection;
   

        private string m_connectionstring="";
      

        public string DataBase
        {
            get { return _connection.Database; }

        }
  
        //Constructor
        public DBConnect(string connectionstring="")
        {
            Initialize(connectionstring);
            OpenConnection();

        }

        //Initialize values
        private void Initialize(string connectionstring)
        {

            if (connectionstring != "") m_connectionstring = connectionstring;
            else  m_connectionstring = GetConnectionStringINI();

              
            _connection = new SqlConnection(m_connectionstring);

        }




        private string GetConnectionStringINI()
        {
   
            try
            {



                return "Data Source=SQL5033.myASP.NET;Initial Catalog=DB_A0B2A3_webaccess;User Id=DB_A0B2A3_webaccess_admin;Password=Sparcman95!;";

            }
            catch (Exception e)
            {

              
                return "";
            }
        }


 

    
        //open connection to database
        public bool OpenConnection()
        {
            try
            {
                if (_connection.State == ConnectionState.Open) return true;
                else
                {
                    //close connection first then try to open
                    _connection.Close();
                    if (_connection.State == ConnectionState.Closed) _connection.Open();

                    //test to see if connection is open
                    if (_connection.State == ConnectionState.Open) return true;
                    else return false;
                }

               
               
            }
            catch (SqlException ex)
            {
                //When handling errors, you can your application's response based on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                var mes = ex;

                return false;
            }
        }

        //Close connection
        public bool CloseConnection()
        {
            try
            {
                _connection.Close();
                return true;
            }
            catch (SqlException ex)
            {
           
                return false;
            }
        }


        public bool TestConnection()
        {

            try
            {
                //Open Connection

                if (this.OpenConnection() == true)
                {
                    this.CloseConnection();
                    return true;
                }
                else return false;

            }
            catch (Exception ex)
            {
        
                return false;
            }
        }
 

       
        public SqlDataReader DeleGetDataReader(string sql)
        {
            SqlCommand command = new SqlCommand(sql, _connection);
            if (this.OpenConnection() == true)
            {
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return reader;
                }
                else return null;

            }
            else return null;

        }



        public string Command(string query)
        {
            try
            {
               

                if (this.OpenConnection() == true)
                {
                    SqlCommand cmd = new SqlCommand(query, _connection);
                    cmd.ExecuteNonQuery();
               
                    return "success";
                }
                else return "failed connection";

            }catch(Exception e)
            {
         
                return e.Message;
            }

        }



        public DataTable GetData(string query)
        {

            return GetData(query, "Table");
        }


        public DataTable GetData(string query,string tblName)
        {
            DataTable table = new DataTable(tblName);

            try
            {


                if (this.OpenConnection() == true)
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = _connection;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = query;

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(table);
                }
              
                return table;

            }
            catch (SqlException ex)
            {
      
                return table;
            }


        }





   
    }
}
