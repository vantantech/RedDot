using System;
using System.Windows;

using System.IO;

using System.Data;

using Microsoft.Win32;
using NLog;
using MySql.Data.MySqlClient;

namespace WebSync
{
    public class DBConnect
    {
        private MySqlConnection _connection;
     
        private static Logger logger = LogManager.GetCurrentClassLogger();
       
        private string m_connectionstring="";
     

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

        private string GetSettingValue(string keystring, string defaultstring)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software");
                key = key.OpenSubKey("RedDot");
                if (key != null)
                {
                    key = key.OpenSubKey("DataBase");

                    return key.GetValue(keystring).ToString();

                }
                else return "";

            }
            catch (Exception e)
            {

                logger.Error("GetSettings:" + e.Message);
                CreateSetting(keystring, defaultstring);
                return defaultstring;
            }


        }

        private void CreateSetting(string keystring, string value)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software");
                key = key.OpenSubKey("RedDot");
                if (key != null)
                {
                    key = key.OpenSubKey("DataBase", true);

                    key.SetValue(keystring, value);

                }
                

            }
            catch (Exception e)
            {
               // MessageBox.Show("Create SEtting:" + e.Message);
       
                logger.Error("Create SEtting:" + e.Message);

                return;
            }
           
        }

        private string GetConnectionStringINI()
        {
            //  _connectionstring = "SERVER=localhost;Port=3306;DATABASE=reddot;UID=root;PASSWORD=sparcman;";
            try
            {
               


                return "SERVER=" + GlobalSettings.Instance.DatabaseServer + ";Port=" + GlobalSettings.Instance.PortNo + ";DATABASE=" + GlobalSettings.Instance.DatabaseName + ";UID=root;PASSWORD=sparcman;";
                   

            }
            catch (Exception e)
            {

               // MessageBox.Show("GetConnectionString:" + e.Message);
       
                logger.Error("GetConectionStringINI:" + e.Message);
                return "";
            }
        }

        /*
        private string GetConnectionString()
        {
            //  _connectionstring = "SERVER=localhost;Port=3306;DATABASE=reddot;UID=root;PASSWORD=sparcman;";
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software");
                key = key.OpenSubKey("RedDot");
                if (key != null)
                {
                    key = key.OpenSubKey("DataBase");
                    if (key != null)
                    {
                        m_server = GetSettingValue("Server", "localhost");
                        m_database = GetSettingValue("Database", "reddot");
                        m_port = GetSettingValue("Port", "3306");

                        return "SERVER=" + m_server + ";Port=" + m_port + ";DATABASE=" + m_database + ";UID=root;PASSWORD=sparcman;";
                    }
                    else return CreateRegistryDatabase();



                }
                else return  CreateRegistryAll();

            }catch(Exception e)
            {

                MessageBox.Show("GetConnectionString:" + e.Message);
                return "";
            }
        }
        */


        private string CreateRegistryAll()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);

            key.CreateSubKey("RedDot");
            key = key.OpenSubKey("RedDot", true);


            key.CreateSubKey("DataBase");
            key = key.OpenSubKey("DataBase", true);

            key.SetValue("Server", "localhost");
            key.SetValue("Database", "reddot");
            key.SetValue("Port", "3306");

            return "SERVER=localhost;Port=3306;DATABASE=reddot;UID=root;PASSWORD=sparcman;";

        }

        private string CreateRegistryDatabase()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);

 
            key = key.OpenSubKey("RedDot", true);


            key.CreateSubKey("DataBase");
            key = key.OpenSubKey("DataBase", true);

            key.SetValue("Server", "localhost");
            key.SetValue("Database", "reddot");
            key.SetValue("Port", "3306");

            return "SERVER=localhost;Port=3306;DATABASE=reddot;UID=root;PASSWORD=sparcman;";

        }
        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                if(_connection.State == ConnectionState.Closed) _connection.Open();
              //  _logger.Trace(_connection.ConnectionString);
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
                       // MessageBox.Show("Cannot connect to server -- Contact administrator");
                        logger.Error("Cannot connect to server:" + _connection.DataSource + "/" +  _connection.Database);
                        break;

                    case 1045:
                        //MessageBox.Show("Invalid username/password -- Contact Administrator");
                        logger.Error("invalid username/password:" + _connection.DataSource + "/" + _connection.Database);
                        break;
                    case 1049:
                        // MessageBox.Show("Unknown Database -- Contact Administrator");
                        logger.Error("Unknown Database:" + _connection.DataSource + "/" + _connection.Database);
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
              //  MessageBox.Show(ex.Message);
                logger.Error("Close Connection:" + ex.Message);
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
                //MessageBox.Show(ex.Message);
                logger.Error("Test Connection:" + ex.Message);
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
               // MessageBox.Show("DBConnect:Command:" + query + ":"  + e.Message);
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

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(table);
                }
                this.CloseConnection();
                return table;

            }
            catch 
            {
                logger.Error("GetData() Error:" + query);

                return null;
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
                string file = backupdirectory + "\\reddot-" + GlobalSettings.Instance.DatabaseName + year + "-" + month + "-" + day + "-" + hour + "-" + minute + ".sql";


                string constring = string.Format(@"server={0};user={1};pwd={2};database={3};", GlobalSettings.Instance.DatabaseServer, "root", "sparcman", GlobalSettings.Instance.DatabaseName);

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


    }
}
