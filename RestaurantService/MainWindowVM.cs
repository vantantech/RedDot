using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using System.Data;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Globalization;
using System.Security.Cryptography;
using System.IO;
using System.Management;
using System.Threading;
using NLog;
using System.Data.SQLite;

namespace RestaurantService
{
    public class MainWindowVM : INPCBase
    {
        public static Boolean SearchContactFound;
        public static string SearchContact;
        public static string MyLine;
        public static string MyType;
        public static string MyDate;
        public static string MyTime;
        public static string MyCheckSum;
        public static string MyRings;
        public static string MyDuration;
        public static string MyIndicator;
        public static string MyNumber;
        public static string MyName;
        public static int FoundIndex;


        // Start record variables
        public static string SMyLine;
        public static string SMyTime;
        public static string SMyDate;
        public static string SMyNumber;
        private DataTable m_callerlist;
        public DataTable CallerList
        {
            get { return m_callerlist; }

            set
            {
                m_callerlist = value;
                NotifyPropertyChanged("CallerList");
            }
        }
        private DBConnect dbConnect;


        private string m_message = "";
        public string Message
        {
            get { return m_message; }
            set
            {
                m_message = value;
                NotifyPropertyChanged("Message");
            }
        }



        private string m_message2 = "";
        public string Message2
        {
            get { return m_message2; }
            set
            {
                m_message2 = value;
                NotifyPropertyChanged("Message2");
            }
        }


        private string m_message3 = "";
        public string Message3
        {
            get { return m_message3; }
            set
            {
                m_message3 = value;
                NotifyPropertyChanged("Message3");
            }
        }

        //--------------Caller ID --------------------------------------------------------
        // Receiving Thread setup
        public static UdpReceiverClass UdpReceiver = new UdpReceiverClass();
        Thread UdpReceiveThread = new Thread(UdpReceiver.UdpIdleReceive);

        // Call Record reception <reception_string, seconds_since_it_came_in>
        Dictionary<string, int> previousReceptions = new Dictionary<string, int>();


        private DBCallerID m_dbcallerid;




        public MainWindowVM()
        {
            m_dbcallerid = new DBCallerID();


            // Start listener for UDP traffic
            UdpReceiver.DataReceived += new UdpReceiverClass.UdpEventHandler(HeardIt);

            // Start Receiver thread
            UdpReceiveThread.IsBackground = true;
            UdpReceiveThread.Start();


          //  CheckForDatabase();


            dbConnect = new DBConnect();

            RefreshList();
        }


     public void RefreshList()
        {
            if (dbConnect == null) dbConnect = new DBConnect();
            string query = "Select * from callerid order by calltime desc limit 10";
            CallerList = dbConnect.GetData(query, "Table");
        }

        public void CheckForDatabase()
        {

            string callsfile = GlobalSettings.Instance.ApplicationPath + "\\callsDatabase.db3";
            string contactsfile = GlobalSettings.Instance.ApplicationPath + "\\contactsDatabase.db3";
            //--------------------------- Log File database---------------------
            if (File.Exists(callsfile) == false)
            {
                // Create new SQLite (db3) file for new database since none exist
                // You could use any database type for logging.  We used SQLite since one only one DLL file
                // is required for installation of this database type. 

                SQLiteConnection.CreateFile(callsfile);

                // Connect to database
                SQLiteConnection myConnection = new SQLiteConnection();
                myConnection.ConnectionString = @"Data Source=" + GlobalSettings.Instance.ApplicationPath + "\\callsDatabase.db3;";

                // Log into log database
                try
                {
                    myConnection.Open();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQL exception: " + ex.ToString());
                }

                // Create all needed columns in new table called 'calls'
                SQLiteCommand myCommand = new SQLiteCommand("CREATE TABLE calls(Date varchar(10),Time varchar(10),Line varchar(10),Type varchar(10),Indicator varchar(10),Duration varchar(10),Checksum varchar(10),Rings varchar(10),Number varchar(15),Name varchar(20));", myConnection);
                if (myConnection.State == ConnectionState.Open)
                {
                    myCommand.ExecuteNonQuery();
                }

                try
                {
                    myConnection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQL Exception: " + ex.ToString());
                }

            }

            // --------------------------- Database for contacts ------------------------------------
            // In order to show sample code for databse lookups, we created an extremely simple contact database.  // It only contains name and phone number.  Your existing contact manager database would be acessed to 
            // find matching records based on phone numbers.

            if (File.Exists(contactsfile) == false)
            {
                // Create new SQLite (db3) file for new database since none exist
                SQLiteConnection.CreateFile(contactsfile);

                // Connect to database
                SQLiteConnection myConnection = new SQLiteConnection();
                myConnection.ConnectionString = @"Data Source=" + GlobalSettings.Instance.ApplicationPath + "\\contactsDatabase.db3;";

                // Log into log database
                try
                {
                    myConnection.Open();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQL exception: " + ex.ToString());
                }

                // Create all needed columns in new table called 'contacts'
                SQLiteCommand myCommand = new SQLiteCommand("CREATE TABLE contacts(Name varchar(20),Phone varchar(15));CREATE TABLE contactsTemp(Name varchar(20),Phone varchar(15));", myConnection);
                if (myConnection.State == ConnectionState.Open)
                {
                    myCommand.ExecuteNonQuery();
                }

                try
                {
                    myConnection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQL Exception: " + ex.ToString());
                }

            }
        }

        public void setVars()
        {

            // Clear all variables
            SearchContactFound = false;
            SearchContact = "";
            MyLine = "";
            MyType = "";
            MyDate = "";
            MyTime = "";
            MyCheckSum = "";
            MyRings = "";
            MyDuration = "";
            MyIndicator = "";
            MyNumber = "-------";
            MyName = "-------";
            FoundIndex = -1;

            // Get UDP received message from event
            string receivedMessage = UdpReceiverClass.ReceivedMessage;

            if (receivedMessage.Length < 20) return;

            // Remove header
            string rawData = receivedMessage.Substring(21, receivedMessage.Length - 21);
            int index = rawData.IndexOf(" ");

            //---- EXTRACTING INDIVIDUAL FIELDS FROM CALL RECORDS -------                   

            // Deluxe units are capable of sending both Start and End Records on Incoming and Outgoing calls
            // Deluxe units can also send detail records such as Ring, On-Hook and Off Hook.
            // This section extracts data from fields and places it into varibles
            // The code allows for all types of call records that can be sent.  			
            // 
            // Note: Basic unit units only send Start Records on Incoming Calls.


            // Get Line number from string
            MyLine = rawData.Substring(0, index);

            // Update Raw data
            rawData = rawData.Substring(index, rawData.Length - index);
            index = rawData.IndexOf(" ");

            while (rawData.IndexOf(" ") == 0)
            {
                rawData = rawData.Substring(1, rawData.Length - 1);
            }

            // Get type from string.  
            // For Deluxe units, the Data type can be either [I]nbound, [O]utbound, [R]ing, o[N]-hook, of[F]-hook
            // For Basic units, the only Data type will be "I".

            index = rawData.IndexOf(" ");
            MyType = rawData.Substring(0, index);

            // Update Raw data
            rawData = rawData.Substring(index, rawData.Length - index);
            index = rawData.IndexOf(" ");

            while (rawData.IndexOf(" ") == 0)
            {
                rawData = rawData.Substring(1, rawData.Length - 1);
            }

            // Check whether the record is a Incoming/Outgoing.  If not, it is a detail record
            // which needs to be processed diffrently below.
            if (MyType == "I" || MyType == "O")
            {

                // Get start or end indicator
                index = rawData.IndexOf(" ");
                MyIndicator = rawData.Substring(0, index);

                // Update Raw data
                rawData = rawData.Substring(index, rawData.Length - index);
                index = rawData.IndexOf(" ");

                while (rawData.IndexOf(" ") == 0)
                {
                    rawData = rawData.Substring(1, rawData.Length - 1);
                }

                // Get duration from string
                index = rawData.IndexOf(" ");
                MyDuration = rawData.Substring(0, index);

                // Update Raw data
                rawData = rawData.Substring(index, rawData.Length - index);
                index = rawData.IndexOf(" ");

                while (rawData.IndexOf(" ") == 0)
                {
                    rawData = rawData.Substring(1, rawData.Length - 1);
                }

                // Get checksum from string
                index = rawData.IndexOf(" ");
                MyCheckSum = rawData.Substring(0, index);

                // Update Raw data
                rawData = rawData.Substring(index, rawData.Length - index);
                index = rawData.IndexOf(" ");

                while (rawData.IndexOf(" ") == 0)
                {
                    rawData = rawData.Substring(1, rawData.Length - 1);
                }

                // Get rings from string
                index = rawData.IndexOf(" ");
                MyRings = rawData.Substring(0, index);

                // Update Raw data
                rawData = rawData.Substring(index, rawData.Length - index);
                index = rawData.IndexOf(" ");

                while (rawData.IndexOf(" ") == 0)
                {
                    rawData = rawData.Substring(1, rawData.Length - 1);
                }

                // Get date from string
                index = rawData.IndexOf(" ");
                MyDate = rawData.Substring(0, index);

                // Update Raw data
                rawData = rawData.Substring(index, rawData.Length - index);
                index = rawData.IndexOf(" ");

                while (rawData.IndexOf(" ") == 0)
                {
                    rawData = rawData.Substring(1, rawData.Length - 1);
                }

                // Get time from string
                index = rawData.IndexOf(" ");
                MyTime = rawData.Substring(0, index);

                // Update Raw data
                rawData = rawData.Substring(index, rawData.Length - index);
                index = rawData.IndexOf(" ");

                while (rawData.IndexOf(" ") == 0)
                {
                    rawData = rawData.Substring(1, rawData.Length - 1);
                }

                // Get am/pm from string
                index = rawData.IndexOf(" ");
                MyTime += rawData.Substring(0, index);

                // Update Raw data
                rawData = rawData.Substring(index, rawData.Length - index);
                index = rawData.IndexOf(" ");

                while (rawData.IndexOf(" ") == 0)
                {
                    rawData = rawData.Substring(1, rawData.Length - 1);
                }

                // Get number from string
                index = rawData.IndexOf(" ");
                MyNumber = rawData.Substring(0, index);

                // Update Raw data
                rawData = rawData.Substring(index, rawData.Length - index);
                index = rawData.IndexOf(" ");

                while (rawData.IndexOf(" ") == 0)
                {
                    rawData = rawData.Substring(1, rawData.Length - 1);
                }

                // Get name from string
                MyName = rawData;

            }
            //----------------EXTRACT DATA FROM ON-HOOK, OFF-HOOK, AND RING DATA TYPES-----------
            else if (MyType == "N" || MyType == "F" || MyType == "R")
            {
                // Get date from string
                index = rawData.IndexOf(" ");
                MyDate = rawData.Substring(0, index);

                // Update Raw data
                rawData = rawData.Substring(index, rawData.Length - index);
                index = rawData.IndexOf(" ");

                while (rawData.IndexOf(" ") == 0)
                {
                    rawData = rawData.Substring(1, rawData.Length - 1);
                }

                // Get time from string (in case we need the time from these real-time events)
                if ((rawData.IndexOf(" ")) == -1)
                {
                    MyTime = rawData;
                }
                else
                {
                    MyTime = rawData.Substring(0, index);
                }

            }
        }


        private void HeardIt(UdpReceiverClass u, EventArgs e)
        {

            // ----------THIS SECTION HANDLES DUPLICATE RECORDS WHEN USING DUPLICATE FEATURE --------------
            string reception = UdpReceiverClass.ReceivedMessage;

            if (previousReceptions.ContainsKey(reception))
            {
                if (previousReceptions[reception] < 60) ;
            }
            else
            {

                if (previousReceptions.Count > 30)
                {
                    previousReceptions.Add(reception, 0);

                    string removeKey = "";
                    foreach (string key in previousReceptions.Keys)
                    {
                        removeKey = key;
                        break;
                    }

                    if (!string.IsNullOrEmpty(removeKey))
                    {
                        previousReceptions.Remove(removeKey);
                    }

                }
                else
                {
                    previousReceptions.Add(reception, 0);
                }
            }

            // ------------------------------------------------------------------------------------------

            // Extract all variables from incoming data string
            // this.Invoke((MethodInvoker)(() => setVars()));

            //  m_parent.Dispatcher.BeginInvoke(new Action(delegate{setVars(); }));

            setVars();



            // ----------THIS SECTION HANDLES ALLL THE CALLER ID WINDOW VISUALS--------------
            // The code below could easily be condensed into one method handling different line numbers.
            // We used 4 occurances of the same method for 4 lines hoping that clarity could be provided.
            Message = "MyLIne:" + MyLine;

            switch (MyLine)
            {
                case "01":

                    switch (MyType + MyIndicator)
                    {

                        //ringing
                        case "R":

                            //-----------------------INCOMING CALL--------------------
                            //--------------------------------------------------------
                            // Change picture of phone to ringing
                            // picPhoneLine1.Image = ExampleApplication_Csharp.Properties.Resources.phoneRing;

                            // Light-up panel green for incoming call
                            //  panLine1.BackColor = Color.LightGreen;

                            // Show time on panel
                            // lbLine1Time.Invoke((MethodInvoker)(() => lbLine1Time.Text = MyTime));

                            // Show name and number of panel
                            // lbLine1Number.Invoke((MethodInvoker)(() => lbLine1Number.Text = MyNumber));
                            //  lbLine1Name.Invoke((MethodInvoker)(() => lbLine1Name.Text = MyName));

                            break;

                        //call picked up
                        case "IS":

                            // ----------------------Ring answered------------------
                            //------------------------------------------------------
                            // Light-up panel green for incoming call
                            //  panLine1.BackColor = Color.LightGreen;

                            // Show time on panel
                            //  lbLine1Time.Invoke((MethodInvoker)(() => lbLine1Time.Text = MyTime));

                            // Show name and number of panel
                            //  lbLine1Number.Invoke((MethodInvoker)(() => lbLine1Number.Text = MyNumber));
                            //  lbLine1Name.Invoke((MethodInvoker)(() => lbLine1Name.Text = MyName));

                            break;
                        case "F":

                            //-----------------------Phone pickup-------------------
                            //------------------------------------------------------
                            // picPhoneLine1.Image = ExampleApplication_Csharp.Properties.Resources.phoneOffHook;

                            break;
                        case "N":

                            //-----------------------Phone hangup--------------------
                            //-------------------------------------------------------

                            // Change panel color
                            //  panLine1.BackColor = Color.Silver;

                            // Change picture of phone to not-ringing
                            //  picPhoneLine1.Image = ExampleApplication_Csharp.Properties.Resources.phoneOnHook;

                            break;
                        case "IE":

                            //-----------------------Phone End Call--------------------
                            //-------------------------------------------------------

                            // Change panel color
                            //  panLine1.BackColor = Color.Silver;

                            // Change picture of phone to not-ringing
                            //  picPhoneLine1.Image = ExampleApplication_Csharp.Properties.Resources.phoneOnHook;

                            break;
                        case "OS":

                            //-----------------------Outgoing CALL--------------------
                            //--------------------------------------------------------
                            // Change picture of phone to ringing
                            // picPhoneLine1.Image = ExampleApplication_Csharp.Properties.Resources.phoneOffHook;

                            // Light-up panel blue for incoming call
                            //  panLine1.BackColor = Color.LightBlue;

                            // Show time on panel
                            //  lbLine1Time.Invoke((MethodInvoker)(() => lbLine1Time.Text = MyTime));

                            // Show name and number of panel
                            //  lbLine1Number.Invoke((MethodInvoker)(() => lbLine1Number.Text = MyNumber));
                            //  lbLine1Name.Invoke((MethodInvoker)(() => lbLine1Name.Text = MyName));

                            break;
                        case "OE":

                            //-----------------------Phone End Call--------------------
                            //-------------------------------------------------------

                            // Change panel color
                            // panLine1.BackColor = Color.Silver;

                            // Change picture of phone to not-ringing
                            //  picPhoneLine1.Image = ExampleApplication_Csharp.Properties.Resources.phoneOnHook;

                            break;
                        default:
                            break;
                    }

                    break;

                case "02":

                    switch (MyType + MyIndicator)
                    {
                        case "R":
                            Message = "INCOMING...";
                            //-----------------------INCOMING CALL--------------------
                            //--------------------------------------------------------
                            // Change picture of phone to ringing
                            // picPhoneLine2.Image = ExampleApplication_Csharp.Properties.Resources.phoneRing;

                            // Light-up panel green for incoming call
                            // panLine2.BackColor = Color.LightGreen;

                            // Show time on panel
                            //  lbLine2Time.Invoke((MethodInvoker)(() => lbLine2Time.Text = MyTime));

                            // Show name and number of panel
                            //  lbLine2Number.Invoke((MethodInvoker)(() => lbLine2Number.Text = MyNumber));
                            //   lbLine2Name.Invoke((MethodInvoker)(() => lbLine2Name.Text = MyName));

                            break;
                        case "IS":

                            // ----------------------Ring answered------------------
                            //------------------------------------------------------
                            // Light-up panel green for incoming call
                            //  panLine2.BackColor = Color.LightGreen;

                            // Show time on panel
                            // lbLine2Time.Invoke((MethodInvoker)(() => lbLine2Time.Text = MyTime));

                            // Show name and number of panel
                            // lbLine2Number.Invoke((MethodInvoker)(() => lbLine2Number.Text = MyNumber));
                            //  lbLine2Name.Invoke((MethodInvoker)(() => lbLine2Name.Text = MyName));

                            break;
                        case "F":

                            //-----------------------Phone pickup-------------------
                            //------------------------------------------------------
                            // picPhoneLine2.Image = ExampleApplication_Csharp.Properties.Resources.phoneOffHook;

                            break;
                        case "N":
                            Message = "";
                            //-----------------------Phone hangup--------------------
                            //-------------------------------------------------------

                            // Change panel color
                            // panLine2.BackColor = Color.Silver;

                            // Change picture of phone to not-ringing
                            //  picPhoneLine2.Image = ExampleApplication_Csharp.Properties.Resources.phoneOnHook;

                            break;
                        case "IE":

                            Message = "";
                            //-----------------------Phone End Call--------------------
                            //-------------------------------------------------------

                            // Change panel color
                            // panLine2.BackColor = Color.Silver;

                            // Change picture of phone to not-ringing
                            // picPhoneLine2.Image = ExampleApplication_Csharp.Properties.Resources.phoneOnHook;

                            break;
                        case "OS":

                            //-----------------------Outgoing CALL--------------------
                            //--------------------------------------------------------
                            // Change picture of phone to ringing
                            // picPhoneLine2.Image = ExampleApplication_Csharp.Properties.Resources.phoneOffHook;

                            // Light-up panel blue for incoming call
                            //  panLine2.BackColor = Color.LightBlue;

                            // Show time on panel
                            //  lbLine2Time.Invoke((MethodInvoker)(() => lbLine2Time.Text = MyTime));

                            // Show name and number of panel
                            // lbLine2Number.Invoke((MethodInvoker)(() => lbLine2Number.Text = MyNumber));
                            //  lbLine2Name.Invoke((MethodInvoker)(() => lbLine2Name.Text = MyName));

                            break;
                        case "OE":

                            //-----------------------Phone End Call--------------------
                            //-------------------------------------------------------

                            // Change panel color
                            // panLine2.BackColor = Color.Silver;

                            // Change picture of phone to not-ringing
                            // picPhoneLine2.Image = ExampleApplication_Csharp.Properties.Resources.phoneOnHook;

                            break;
                        default:
                            break;
                    }

                    break;





                default:
                    break;
            }
            // ---------------------------------------------
            //         START & END RECORD PROCESSING 
            // ---------------------------------------------
            // Start Records will be processed below for 3 reasons:
            //   1. To perform a customer lookup.
            //	 2. Change visuals on the Caller ID main screen.
            //   3. Add phone call records to the log file properly.
            //		- Data will be stored for Start Records such that  
            //		  corresponding End records will replace them in log file. 

            // Combine MyType and MyIndicator to create a 'command' variable that allows us to 
            // determine if the call is an Incoming or Outgoing Start record.
            string command = MyType + MyIndicator;


            Message2 = "command: " + command;

            // If 'command' is Start record
            if (command == "IS" || command == "OS")
            {
                // Set values to be checked against database during end record
                SMyLine = MyLine;
                SMyTime = MyTime;
                SMyDate = MyDate;
                SMyNumber = MyNumber;

                Message3 = DateTime.Now.ToLongTimeString() + " " +  MyName + "\r\n" + MyNumber;
                

                m_dbcallerid.SaveCaller(MyNumber, MyName);

               // TouchMessageBox.Show("Caller:" + MyName + " " + MyNumber, 1);

                RefreshList();

                // Look for phone number in database
                // this.Invoke((MethodInvoker)(() => searchContacts()));

                // m_parent.Dispatcher.BeginInvoke(new Action(delegate { searchContacts(); }));

              //  searchContacts();



                // If found in database
                if (SearchContactFound == true)
                {
                    // Number found: change icon for contacts to found
                    switch (MyLine)
                    {
                        case "01":
                            // Change image to show that contact was found
                            // picDatabaseLine1.Image = Properties.Resources.databaseFound;

                            // Change tag to 'change' so when clicked we update contact
                            // instead of creating new one
                            // picDatabaseLine1.Tag = "change";

                            // Change name on display for current line
                            // lbLine1Name.Invoke((MethodInvoker)(() => lbLine1Name.Text = SearchContact));
                            break;

                        case "02":
                            // Change image to show that contact was found
                            // picDatabaseLine2.Image = Properties.Resources.databaseFound;

                            // Change tag to 'change' so when clicked we update contact
                            // instead of creating new one
                            // picDatabaseLine2.Tag = "change";

                            // Change name on display for current line
                            // lbLine2Name.Invoke((MethodInvoker)(() => lbLine2Name.Text = SearchContact));
                            break;



                    }

                    // Change Name to name found in database
                    MyName = SearchContact;
                }
                else
                {
                    // Not found: change icon for contacts to insert
                    switch (MyLine)
                    {
                        case "01":
                            // Change image to show contact not found
                            //  picDatabaseLine1.Image = Properties.Resources.databaseInsert;

                            // Change tag to 'insert' so program knows to add new contact
                            // if button is clicked.
                            // picDatabaseLine1.Tag = "insert";
                            break;

                        case "02":
                            // Change image to show contact not found
                            //  picDatabaseLine2.Image = Properties.Resources.databaseInsert;

                            // Change tag to 'insert' so program knows to add new contact
                            // if button is clicked.
                            // picDatabaseLine2.Tag = "insert";
                            break;



                    }
                }

                // Log start record into database
                //this.Invoke((MethodInvoker)(() => logCall()));
                // m_parent.Dispatcher.BeginInvoke(new Action(delegate { logCall(); }));

               // logCall();
            }

            // ----------END RECORD PROCESSING----------

            // if incoming or outgoing end record
            else if (command == "IE" || command == "OE")
            {
                Message = "";

                // Look for phone number in database
                // m_parent.Dispatcher.BeginInvoke(new Action(delegate { searchContacts(); }));
              //  searchContacts();


                // If found in database
                if (SearchContactFound == true)
                {
                    // Take database name and use it for updating
                    MyName = SearchContact;
                }

                // On deluxe unit the End Record replaces the
                // Start record to provide database with more
                // accurate information (duration,rings,name,etc.)

                // Updates record to new duration count for deluxe units
                // this.Invoke((MethodInvoker)(() => updateRecord()));
                // m_parent.Dispatcher.BeginInvoke(new Action(delegate { updateRecord(); }));

               // updateRecord();
            }

        }
        // Finds contact name from database that corresponds to Caller ID number
        // and store into global varible. 



        private void searchContacts()
        {
            // Create connection to database
            SQLiteConnection myConnection = new SQLiteConnection();
            myConnection.ConnectionString = @"Data Source=" + GlobalSettings.Instance.ApplicationPath + "\\contactsDatabase.db3;";

            // Log into log database
            try
            {
                myConnection.Open();

            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL exception: " + ex.ToString());
            }

            // Search if phone number already in database
            SQLiteCommand myCommand = new SQLiteCommand("SELECT Name FROM contacts WHERE Phone='" + MyNumber + "';", myConnection);
            if (myConnection.State == ConnectionState.Open)
            {
                SQLiteDataReader reader = myCommand.ExecuteReader();

                // If phone found return the name matching the number
                if (reader.HasRows)
                {
                    SearchContactFound = true;
                    while (reader.Read())
                    {
                        SearchContact = reader.GetString(0);
                    }

                }
                else
                {
                    SearchContactFound = false;
                }

                // Close data reader
                reader.Close();

            }

            try
            {
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL Exception: " + ex.ToString());
            }

        }


        public void logCall()
        {
            // Create connection to database
            SQLiteConnection myConnection = new SQLiteConnection();
            myConnection.ConnectionString = @"Data Source=" + GlobalSettings.Instance.ApplicationPath + "\\callsDatabase.db3;";

            // Log into log database
            try
            {
                myConnection.Open();

            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL exception: " + ex.ToString());
            }

            // Insert new data into database
            SQLiteCommand myCommand = new SQLiteCommand("INSERT INTO calls(Line,Type,Indicator,Duration,Checksum,Rings,Date,Time,Number,Name) Values ('" + MyLine + "','" + MyType + "','" + MyIndicator + "','" + MyDuration + "','" + MyCheckSum + "','" + MyRings + "','" + MyDate + "','" + MyTime + "','" + MyNumber + "','" + MyName + "')", myConnection);
            if (myConnection.State == ConnectionState.Open)
            {
                myCommand.ExecuteNonQuery();
            }

            // Close connection to database
            try
            {
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL Exception: " + ex.ToString());
            }

            // Update logfile form
            // frmLog.refreshDGV();

        }


        public void updateRecord()
        {
            // Create database connection
            SQLiteConnection myConnection = new SQLiteConnection();
            myConnection.ConnectionString = @"Data Source=" + GlobalSettings.Instance.ApplicationPath + "\\callsDatabase.db3;";

            // Log into log database
            try
            {
                myConnection.Open();

            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL exception: " + ex.ToString());
            }

            // Alter data already in database to new values
            SQLiteCommand myCommand = new SQLiteCommand("UPDATE calls SET Duration='" + MyDuration + "', Rings='" + MyRings + "', Indicator='" + MyIndicator + "', Name='" + MyName +
                "' WHERE Line='" + SMyLine + "' AND Time='" + SMyTime + "' AND Date='" + SMyDate + "' AND Number='" + SMyNumber + "';", myConnection);
            if (myConnection.State == ConnectionState.Open)
            {
                myCommand.ExecuteNonQuery();
            }

            // Close connection to database
            try
            {
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL Exception: " + ex.ToString());
            }

            // Update logfile form
            // frmLog.refreshDGV();

        }



        public void executeExit()
        {
            // End Receiver thread
            if (UdpReceiveThread.IsAlive)
            {
                UdpReceiveThread.Abort();
            }
        }
    }
}
