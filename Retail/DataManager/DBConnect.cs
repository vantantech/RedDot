using System;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
//Add MySql Library
using MySql.Data.MySqlClient;
using System.Data;

using System.Configuration;
using Microsoft.Win32;


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
        private string m_port = "";
       

        public string DataBase
        {
            get { return _connection.Database; }

        }

        public string DataBaseServer
        {
            get { return m_server; }

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
                var MyIni = new IniFile("RedDot.Ini");


                m_server = MyIni.Read("Server", "Database");
                m_database = MyIni.Read("Database", "Database");
                m_port = MyIni.Read("Port", "Database");
                m_uid = MyIni.Read("DBUser", "Database");
                m_password = MyIni.Read("DBPassword", "Database");

                // return "SERVER=" + m_server + ";Port=" + m_port + ";DATABASE=" + m_database + ";UID=a0b2a3_test15;PASSWORD=Sparcman99!;";
                return "SERVER=" + m_server + ";Port=" + m_port + ";DATABASE=" + m_database + ";UID=" + m_uid + ";PASSWORD=" + m_password + ";";


            }
            catch (Exception e)
            {
                MessageBox.Show("GetConnectionString:" + e.Message);
                return "";
            }
        }
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


        public static bool TestConnection()
        {

            try
            {
                //Open Connection
                DBConnect _dbconnect = new DBConnect();

                if (_dbconnect.OpenConnection() == true)
                {
                    _dbconnect.CloseConnection();
                    return true;
                }
                else return false;

            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message);
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
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return table;
            }


        }




        public List<string>[] GetTimeSheetList(int id, DateTime lcDate, DateTime lcEndDate)
        {
            string query = "select * from timeinout where employeeid=" + id + " and timein between '" + lcDate.ToString("yyyy-MM-dd 00:00:00") + "' and '" + lcEndDate.ToString("yyyy-MM-dd 00:00:00") + "'  order by TimeIn desc";

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
        public void Backup(string backupdirectory, string mysqldumppath)
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
                path = backupdirectory + "\\reddot-" + m_database + year + "-" + month + "-" + day + "-" + hour + "-" + minute + ".sql";
                StreamWriter file = new StreamWriter(path);

                
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = mysqldumppath;
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = true;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", "root", "sparcman", m_server, m_database);
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
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", m_uid, m_password, m_server, m_database);
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
