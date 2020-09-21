using System;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
//Add MySql Library
using MySql.Data.MySqlClient;
using System.Data;


namespace RedDot
{
    public class DBConnect
    {
       
        private string m_server="";
        private string m_database="";
        private string m_uid="";
        private string m_password="";
        private string m_connectionstring="";
        private int m_port ;



           //Constructor
        public DBConnect()
        {
            Initialize();

        }

        //Initialize values
        private void Initialize()
        {
                m_connectionstring = GetConnectionStringINI();
 
        }




        private string GetConnectionStringINI()
        {
            return  "SERVER=localhost;Port=3306;DATABASE=fuquacrawfish;UID=root;PASSWORD=sparcman;";
      
        }
 



        //open connection to database
        private MySqlConnection OpenConnection()
        {
            try
            {
                MySqlConnection conn =   new MySqlConnection(m_connectionstring);
                conn.Open();
                return conn;
            }
            catch (MySqlException ex)
            {
     
                return null;
            }
        }

        //Close connection


 



        public bool Command(string query)
        {
            try
            {
                MySqlConnection _connection = OpenConnection();

                if (_connection.State == ConnectionState.Open)
                {
                    MySqlCommand cmd = new MySqlCommand(query, _connection);
                    cmd.ExecuteNonQuery();
                    _connection.Close();
                    return true;
                }
                else return false;

            }catch(Exception e)
            {

                return false;
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
                MySqlConnection _connection = OpenConnection();

                if (_connection.State == ConnectionState.Open)
                {
                    MySqlCommand command = new MySqlCommand();
                    command.Connection = _connection;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = query;

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(table);
                }
                _connection.Close();
                return table;

            }
            catch (MySqlException ex)
            {

                return table;
            }


        }




    }
}
