
using System;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
//Add MySql Library
using System.Data.SqlClient;
using System.Data;

namespace DotNet
{
    class MSSQLConnect
    {

        private SqlConnection connection;
            private string server;
            private string database;
            private string uid;
            private string password;

            //Constructor
            public MSSQLConnect()
            {
                Initialize();
            }

            //Initialize values
            private void Initialize()
            {
                server = "developer\\SQLEXPRESS2";
                database = "simplesales";
                uid = "hoa";
                password = "hoa";
                string connectionString;
                connectionString = "SERVER=" + server  + ";DATABASE=" + database + ";" + "user id=" + uid + ";" + "PASSWORD=" + password + ";Connection Timeout=10;";

                connection = new SqlConnection(connectionString);
            }


            //open connection to database
            private bool OpenConnection()
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (SqlException ex)
                {
                    //When handling errors, you can your application's response based on the error number.
                    //The two most common error numbers when connecting are as follows:
                    //0: Cannot connect to server.
                    //1045: Invalid user name and/or password.
                    switch (ex.Number)
                    {
                        case 0:
                            MessageBox.Show("Cannot connect to server.  Contact administrator");
                            break;

                        case 1045:
                            MessageBox.Show("Invalid username/password, please try again");
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
                    connection.Close();
                    return true;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }

    



            //Delete statement
            public void Command(string query)
            {
               

                if (this.OpenConnection() == true)
                {
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    this.CloseConnection();
                }
            }


            public DataTable GetData(string query)
            {
                DataTable table = new DataTable("Table");

                try
                {

           
                            if (this.OpenConnection() == true)
                            {
                                SqlCommand command = new SqlCommand();
                                command.Connection = connection;
                                command.CommandType = System.Data.CommandType.Text;
                                command.CommandText = query;

                                SqlDataAdapter adapter = new SqlDataAdapter(command);
                                adapter.Fill(table);
                            }
                            this.CloseConnection();
                            return table;

                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return table;
                }


            }

            //Select statement
            public List<string>[] Select(string query)
            {
                

                //Create a list to store the result
                List<string>[] list = new List<string>[3];
                list[0] = new List<string>();
                list[1] = new List<string>();
                list[2] = new List<string>();

                //Open connection
                if (this.OpenConnection() == true)
                {
                    //Create Command
                    SqlCommand cmd = new SqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        list[0].Add(dataReader["id"] + "");
                        list[1].Add(dataReader["lastname"] + "");
                        list[2].Add(dataReader["firstname"] + "");
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

            //Count statement
            public int Count(string query)
            {
                
                int Count = -1;

                //Open Connection
                if (this.OpenConnection() == true)
                {
                    //Create Mysql Command
                    SqlCommand cmd = new SqlCommand(query, connection);

                    //ExecuteScalar will return one value
                    Count = int.Parse(cmd.ExecuteScalar() + "");

                    //close Connection
                    this.CloseConnection();

                    return Count;
                }
                else
                {
                    return Count;
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
                    path = "C:\\" + year + "-" + month + "-" + day + "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".sql";
                    StreamWriter file = new StreamWriter(path);


                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "sqldump";
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
                    path = "C:\\SqlBackup.sql";
                    StreamReader file = new StreamReader(path);
                    string input = file.ReadToEnd();
                    file.Close();


                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "sql";
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
