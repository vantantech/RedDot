using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Forms;

namespace DataManager
{
    public class DBSQLite
    {
        private SQLiteConnection _connection;
        private string _connectionstring;
        public DBSQLite()
        {
            Initialize();

        }

        private void Initialize()
        {
            _connectionstring = Properties.Settings.Default.ConnectString;
            _connection = new SQLiteConnection(_connectionstring);


        }


        private bool OpenConnection()
        {
            try
            {
                
                if (_connection.State == System.Data.ConnectionState.Closed) _connection.Open();
                return true;
            }
            catch(Exception ex)
            {

                MessageBox.Show(ex.Message);
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
            catch (Exception ex)
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









    }
}
