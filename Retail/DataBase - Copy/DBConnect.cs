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
    class DBConnect
    {
        private MySqlConnection _connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        private string _connectionstring;
        //Constructor
        public DBConnect()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {

            _connectionstring = Properties.Settings.Default.ConnectString;
            _connection = new MySqlConnection(_connectionstring);
            server = "localhost";
            database = "reddot";
            uid = "root";
            password = "sparcman";
        }


        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                if(_connection.State == ConnectionState.Closed) _connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server -- Contact administrator");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password -- Contact Administrator");
                        break;
                    case 1049:
                         MessageBox.Show("Unknown Database -- Contact Administrator");
                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                _connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }


        public string TestConnection()
        {

            try
            {
                //Open Connection

                if (this.OpenConnection() == true)
                {
                    this.CloseConnection();
                    return "Database Active";
                }
                else return "Database Connect Error";

            }
            catch (Exception e)
            {
                return "Database Test: " + e.Message;
            }
        }
 
        public int UserAuthenticate(string pin, int len)
        {

            try
            {

                DataTable table = GetData("select id, security from employee where substring(pin,1," + len.ToString() + ")='" + pin.ToString().Substring(0,len) + "'", "Security");

                if (table.Rows.Count >= 1)
                {
                    if ((int)table.Rows[0]["security"] > 0)
                    {
                        return (int)table.Rows[0]["id"];
                    }
                    else return 0;
                    
                }
                else return 0;
            }
            catch(Exception e)
            {

                MessageBox.Show(e.Message);
                return 0;
            }
           

        }
       
        public MySqlDataReader GetDataReader(string sql)
        {
            MySqlCommand command = new MySqlCommand(sql, _connection);
            if (this.OpenConnection() == true)
            {
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return reader;
                }
                else return null;

            }
            else return null;

        }

        public MySqlDataReader GetCustomer(string phone1)
        {
            string sql;
            sql = "select id from customer where phone1 ='" + phone1.Trim() + "'";
            return GetDataReader(sql);
        }

        public bool Command(string query)
        {
            this.CloseConnection();

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, _connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
                return true;
            }
            else return false;
        }


        public int CreateCustomer(string phone1)
        {

            Command("Insert into customer(phone1) values ('" + phone1 + "')");
            DataTable maxtable = GetData("Select max(id) as maxid from customer where phone1 ='" + phone1 + "'", "max");
            if (maxtable.Rows.Count > 0)
            {
                return int.Parse(maxtable.Rows[0]["maxid"].ToString());
            }
            else return 0;
        }



        public DataTable GetData(string query,string tblName)
        {
            DataTable table = new DataTable(tblName);

            try
            {


                if (this.OpenConnection() == true)
                {
                    MySqlCommand command = new MySqlCommand();
                    command.Connection = _connection;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = query;

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(table);
                }
                this.CloseConnection();
                return table;

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return table;
            }


        }



   


        //Backup
        public void Backup()
        {
            try
            {
                DateTime Time = DateTime.Now;
                int year = Time.Year;
                int month = Time.Month;
                int day = Time.Day;
                int hour = Time.Hour;
                int minute = Time.Minute;
                int second = Time.Second;
                int millisecond = Time.Millisecond;

                //Save file to C:\ with the current date as a filename
                string path;
                path = "C:\\release\\reddot" + year + "-" + month + "-" + day + "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".sql";
                StreamWriter file = new StreamWriter(path);

                
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "C:\\Program Files\\MySQL\\MySQL Server 5.6\\bin\\mysqldump";
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = true;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", uid, password, server, database);
                psi.UseShellExecute = false;

                Process process = Process.Start(psi);

                string output;
                output = process.StandardOutput.ReadToEnd();
                file.WriteLine(output);
                process.WaitForExit();
                file.Close();
                process.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error , unable to backup!" + ex.Message);
            }
        }

        //Restore
        public void Restore()
        {
            try
            {
                //Read file from C:\
                string path;
                path = "C:\\MySqlBackup.sql";
                StreamReader file = new StreamReader(path);
                string input = file.ReadToEnd();
                file.Close();


                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysql";
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = false;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", uid, password, server, database);
                psi.UseShellExecute = false;

                
                Process process = Process.Start(psi);
                process.StandardInput.WriteLine(input);
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error , unable to Restore!" + ex.Message);
            }
        }
    }
}
