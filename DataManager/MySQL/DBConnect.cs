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

using System.Configuration;
using Microsoft.Win32;
using System.Windows.Forms;

namespace RedDot.DataManager
{
    public class DBConnect
    {
        private MySqlConnection _connection;
       
        private string m_connectionstring="";
        private string DatabaseServer= GetINIString("Server", "DataBase", "localhost");
        private string DatabaseName = GetINIString("Database", "DataBase", "salon");
        private string PortNo = GetINIString("Port", "DataBase","3306");
        private string UserName = GetINIString("DBUser", "DataBase", "root");
        private string Password = GetINIString("DBPassword", "DataBase", "sparcman");


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
                MessageBox.Show("GetSettings:" + e.Message);
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
                MessageBox.Show("Create SEtting:" + e.Message);

                return;
            }
           
        }

        private string GetConnectionStringINI()
        {
            //  _connectionstring = "SERVER=localhost;Port=3306;DATABASE=reddot;UID=root;PASSWORD=sparcman;";
            try
            {
               


                return "SERVER=" + DatabaseServer + ";Port=" + PortNo + ";DATABASE=" + DatabaseName + ";UID=" + UserName + ";PASSWORD=" + Password + ";";
                   

            }
            catch (Exception e)
            {

                MessageBox.Show("GetConnectionString:" + e.Message);
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
                        MessageBox.Show("Cannot connect to server -- Contact support");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password -- Contact support");
                        break;
                    case 1049:
                         MessageBox.Show("Unknown Database -- Try Restarting program ");
                        break;
                }
                return false;
            }
        }

        public int OpenTest()
        {
            try
            {
                if (_connection.State == ConnectionState.Closed) _connection.Open();
                return 1;
            }
            catch (MySqlException ex)
            {
                if (ex.InnerException != null)
                    return -1;
                else
                    return ex.Number;
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


        public int TestConnection()
        {

            try
            {
                //Open Connection

                if (this.OpenConnection() == true)
                {
                    this.CloseConnection();
                    return 1;
                }
                else return 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public bool TestSchema()
        {

            try
            {
                //Open Connection

                if (this.OpenConnection() == true)
                {
                    String[] tableRestrictions = new String[4];
                    tableRestrictions[2] = "employee";
                    var schema = _connection.GetSchema("Columns",tableRestrictions);

            

                    foreach (DataRow row in schema.Rows)
                    {
                        var column = row["Column_name"].ToString();
                        var datatype = row["Data_Type"].ToString();
                        Debug.WriteLine("{0}", column);
                        if (column.ToUpper() == "SUPPLYFEEDEDUCTION") return true;
                    }
                   // MessageBox.Show("Missing Supply Fee Deduction field in Employee Table");
                    return false;
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

            }catch
            {
               
                return false;
            }

        }



    


        public DataTable GetData(string query)
        {
            DataTable table = new DataTable();

            try
            {


                if (this.OpenConnection() == true)
                {
                    //need to remove the force group column restrictions for newer version of Mysql
                   // string query1 ="set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';";
                   // MySqlCommand cmd = new MySqlCommand(query1, _connection);
                   // cmd.ExecuteNonQuery();

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
            catch(Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error(query);
                logger.Error(ex);

                return null;
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


                string constring = string.Format(@"server={0};user={1};pwd={2};database={3};", DatabaseServer, "root", Password, DatabaseName);

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
                string constring = string.Format(@"server={0};user={1};pwd={2};database={3};",DatabaseServer,"root",Password,DatabaseName);
              
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
