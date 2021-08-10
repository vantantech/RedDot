using System;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
//Add MySql Library
using MySql.Data.MySqlClient;
using System.Data;
using NLog;

namespace RedDot
{
    public class DBConnect
    {
        private MySqlConnection _connection;
        private string m_server="";
        private string m_database="";
        private string m_uid="";
        private string m_password="";
        private string m_connectionstring="";
        private string DatabaseServer = GetINIString("Server", "DataBase", "localhost");
        private string DatabaseName = GetINIString("Database", "DataBase", "salon");
        private string PortNo = GetINIString("Port", "DataBase", "3306");
        private int m_port ;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public string DataBase
        {
            get { return _connection.Database; }

        }
  
        //Constructor
        public DBConnect()
        {
            Initialize();

        }

        //Initialize values
        private void Initialize()
        {



                m_connectionstring = GetConnectionStringINI();

              
            _connection = new MySqlConnection(m_connectionstring);

           
        }


        public static string GetINIString(string key, string section, string defaultvalue)
        {
            var MyIni = new IniFile("RedDot.ini");
            return MyIni.Read(key, defaultvalue, section);


        }


        private string GetConnectionStringINI()
        {
            //  _connectionstring = "SERVER=localhost;Port=3306;DATABASE=reddot;UID=root;PASSWORD=sparcman;";
            try
            {
                var MyIni = new IniFile("RedDot.Ini");


                m_server = GlobalSettings.Instance.DatabaseServer;
                m_database = GlobalSettings.Instance.DatabaseName;
                m_port = GlobalSettings.Instance.PortNo;


                return "SERVER=" + m_server + ";Port=" + m_port + ";DATABASE=" + m_database + ";UID=root;PASSWORD=sparcman;";
                   

            }
            catch (Exception e)
            {

                MessageBox.Show("GetConnectionString:" + e.Message);
                return "";
            }
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
                MessageBox.Show(ex.Message);
                return false;
            }
        }
 

       
        public MySqlDataReader DeleGetDataReader(string sql)
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



        public bool Command(string query)
        {
            try
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

            }catch(Exception e)
            {
                MessageBox.Show("DBConnect:Command:" + query + ":"  + e.Message);
                logger.Error("DBConnect:Command:" + query + ":" + e.Message);
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


                if (this.OpenConnection() == true)
                {
                    MySqlCommand command = new MySqlCommand();
                    command.Connection = _connection;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = query;
                    command.CommandTimeout = 864000;

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(table);
                }
                this.CloseConnection();
                return table;

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                logger.Error("GetData:" + ex.Message);
                return table;
            }


        }




        public List<string>[] GetTimeSheetList(int id, DateTime lcDate, DateTime lcEndDate)
        {
            string query = "select * from TimeInOut where employeeid=" + id + " and timein between '" + lcDate.ToString("yyyy-MM-dd 00:00:00") + "' and '" + lcEndDate.ToString("yyyy-MM-dd 00:00:00") + "'  order by TimeIn desc";

            //Create a list to store the result
            List<string>[] list = new List<string>[4];
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();
            list[3] = new List<string>();

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, _connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["id"] + "");
                    list[1].Add(dataReader["timein"] + "");
                    if (String.IsNullOrEmpty(dataReader["timeout"] + ""))
                    {
                        list[2].Add("null");
                    }
                    else
                    {
                        list[2].Add(dataReader["timeout"] + "");
                    }

                    if (String.IsNullOrEmpty(dataReader["note"] + ""))
                    {
                        list[3].Add(" ");
                    }
                    else
                    {
                        list[3].Add(dataReader["note"] + "");
                    }
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return list;
            }
            else
            {
                return list;
            }
        }



        //Backup
        public void Backup(string backupdirectory)
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
                string file = backupdirectory + "\\reddot-" + DatabaseName + year + "-" + month + "-" + day + "-" + hour + "-" + minute + ".sql";


                string constring = string.Format(@"server={0};user={1};pwd={2};database={3};", DatabaseServer, "root", "sparcman", DatabaseName);

                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            mb.ExportToFile(file);
                            conn.Close();
                        }
                    }
                }



            }
            catch (IOException ex)
            {
                MessageBox.Show("Error , unable to backup!" + ex.Message);
            }
        }

        //Restore
        public void Restore(string file)
        {
            try
            {
                string constring = string.Format(@"server={0};user={1};pwd={2};database={3};", DatabaseServer, "root", "sparcman", "mysql");

                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            mb.ImportFromFile(file);
                            conn.Close();
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error , unable to Restore!" + ex.Message);
            }
        }
    }
}
