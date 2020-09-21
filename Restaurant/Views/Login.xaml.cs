using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DPUruNet;
using RedDot;


namespace RedDot
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
       

  

        private int passlen;
        DBSecurity _dbsecurity;
        public string PIN { get; set; }
        public int ID { get; set; }
        private string _password;
        private string RawString;
        bool m_msrcard = false;
        int trackcount = 0;
        private  bool  fingerprintavailable = false;

        private Reader reader;
        private Fmd fmd = null;
        private static string[] usernames;
        private static int[] userids;

        private bool m_fingerprintonly;
    
        public Brush ButtonBackground
        {
            get { return Brushes.Turquoise; }
        }
        public Login(bool fingerprintonly,string message="")
        {
            InitializeComponent();
            this.DataContext = this;
            m_fingerprintonly = fingerprintonly;



            if(m_fingerprintonly)
            {
                this.chkFingerPrintOnly.IsChecked = true;
            }else
            {
                this.stack1.Visibility = Visibility.Collapsed;
            }

            passlen = GlobalSettings.Instance.PinLength;
    
           // txtPassword.Text = "";
            _password = "";
            _dbsecurity = new DBSecurity();

         
            tbPin.Text = message;
         

            InitializeReaders();
            StartIndentification();
        }


        private void InitializeReaders()
        {
            if (reader != null)
            {
                 reader.CancelCapture();
                reader.Dispose();
               
            }


            ReaderCollection rc = ReaderCollection.GetReaders();
            if (rc.Count == 0)
            {
                fingerprintavailable = false;
               // MessageBox.Show("Fingerprint Reader not found. Please check if reader is plugged in and try again");
            }
            else
            {
                reader = rc[0];
                Constants.ResultCode readersResult = reader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);
                fingerprintavailable = true;
            }
        }


        private void StartIndentification()
        {
            if (fingerprintavailable == false) return;


            Constants.ResultCode result = reader.GetStatus();
            CheckReaderStatus();
            if (result == Constants.ResultCode.DP_SUCCESS)
            {
                if (reader.Status.Status == Constants.ReaderStatuses.DP_STATUS_READY)
                {
                    reader.On_Captured += new Reader.CaptureCallback(reader_On_Captured);
                    IdentificationCapture();
                }
            }
            else
            {
                TouchMessageBox.Show("Could not perform capture. Reader result code :" + result.ToString());
            }
        }

        private void IdentificationCapture()
        {
            Constants.ResultCode captureResult = reader.CaptureAsync(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, reader.Capabilities.Resolutions[0]);
            if (captureResult != Constants.ResultCode.DP_SUCCESS)
            {
                MessageBox.Show("CaptureResult: " + captureResult.ToString());
            }

        }

        private void reader_On_Captured(CaptureResult capResult)
        {

            if (capResult.Quality == Constants.CaptureQuality.DP_QUALITY_GOOD)
            {
                DataResult<Fmd> fmdResult = FeatureExtraction.CreateFmdFromFid(capResult.Data, Constants.Formats.Fmd.DP_VERIFICATION);
                //If successfully extracted fmd then assign fmd
                if (fmdResult.ResultCode == Constants.ResultCode.DP_SUCCESS)
                {
                    fmd = fmdResult.Data;
                }
                else
                {
                    UpdateIdentifyMessage("Could not successfully create a verification FMD");
                }

                // Get view bytes to create bitmap.
                foreach (Fid.Fiv fiv in capResult.Data.Views)
                {
                    UpdateEnrollImage(Utility.CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height));
                    //UpdateEnrollImage(Utility.CopyDataToBitmap(fiv.RawImage, fiv.Width, fiv.Height));
                    UpdateIdentifyMessage("Fingerprint Captured");
                    break;
                }

                if (GlobalSettings.Instance.GetAllFmd1s == null)
                {
                    UpdateIdentifyMessage("No finger print in database");
                    return;
                }

                //Perform indentification of fmd of captured sample against enrolledFmds for userid 
                IdentifyResult iResult1 = Comparison.Identify(fmd, 0, GlobalSettings.Instance.GetAllFmd1s, 21474, 5);

                bool matchesfound = false;
                //If Identify was successfull
                if (iResult1.ResultCode == Constants.ResultCode.DP_SUCCESS)
                {
                    //If number of matches were greater than 0
                    if (iResult1.Indexes.Length > 0)
                    {
                        matchesfound = true;
                        string usersIdentified = null;
                        int idfound = 0;

                        //Get all usernames list
                        usernames = GlobalSettings.Instance.GetAllUserNames;
                        userids = GlobalSettings.Instance.GetallfingerIDs;

                        //for each matched fmd get its index and map that index to userid index
                        for (int i = 0; i < iResult1.Indexes.Length; i++)
                        {
                            int index = iResult1.Indexes[i][0];
                            if (i == 0)
                            {
                                usersIdentified = usernames[index];
                                idfound = userids[index];
                            }
                            else
                            {
                                usersIdentified = usersIdentified + ", " + usernames[index];
                                idfound = 0;
                            }
                        }
                        UpdateIdentifyMessage("User: " + usersIdentified + " Authorized");
                        Verified(idfound);
                    }
                }
                if (matchesfound != true)
                {
                    //Perform indentification of fmd of captured sample against enrolledFmds for userid 
                    IdentifyResult iResult2 = Comparison.Identify(fmd, 0, GlobalSettings.Instance.GetAllFmd2s, 21474, 5);
                    if (iResult2.ResultCode == Constants.ResultCode.DP_SUCCESS)
                    {
                        //If number of matches were greater than 0
                        if (iResult2.Indexes.Length > 0)
                        {
                            matchesfound = true;
                            string usersIdentified = null;
                            int idfound = 0;
                            //Get all usernames list
                            usernames = GlobalSettings.Instance.GetAllUserNames;
                            userids = GlobalSettings.Instance.GetallfingerIDs;
                            //for each matched fmd get its index and map that index to userid index
                            for (int i = 0; i < iResult2.Indexes.Length; i++)
                            {
                                int index = iResult2.Indexes[i][0];
                                if (i == 0)
                                {
                                    usersIdentified = usernames[index];
                                    idfound = userids[index];
                                }
                                else
                                {
                                    usersIdentified = usersIdentified + ", " + usernames[index];
                                    idfound = 0;
                                }
                            }
                            UpdateIdentifyMessage("User: " + usersIdentified + " Authorized");
                            Verified(idfound);
                        }
                    }
                }

                if (!matchesfound)
                {
                    UpdateIdentifyMessage2("NOT Unauthorized");
                }



            }
            else
            {
                UpdateIdentifyMessage("Please swipe finger again");
            }


        }


        private void CheckReaderStatus()
        {
            //If reader is busy, sleep
            if (reader.Status.Status == Constants.ReaderStatuses.DP_STATUS_BUSY)
            {
                Thread.Sleep(50);
            }
            else if ((reader.Status.Status == Constants.ReaderStatuses.DP_STATUS_NEED_CALIBRATION))
            {
                reader.Calibrate();
            }
            else if ((reader.Status.Status != Constants.ReaderStatuses.DP_STATUS_READY))
            {
                throw new Exception("Reader Status - " + reader.Status.Status);
            }
        }


        public void Verified(int id)
        {
            this.Dispatcher.Invoke(new System.Action(() =>
            {
  
                ID = id;
                this.Close();
                Console.Beep(5000,100);

            }));
           
        }

   


        public void CancelCaptureAndCloseReader()
        {
            if (reader != null)
            {
                reader.CancelCapture();
                reader.Dispose();
            }
        }



        private void UpdateIdentifyMessage(string text)
        {
            this.Dispatcher.Invoke(new System.Action(() =>
            {
                this.tbPin.Foreground = Brushes.Blue;
                this.tbPin.Text = text;

            }));

        }

        private void UpdateIdentifyMessage2(string text)
        {
            this.Dispatcher.Invoke(new System.Action(() =>
            {
                this.tbPin.Foreground = Brushes.Red;
                this.tbPin.Text = text;

            }));

        }

        private void UpdateEnrollImage(System.Drawing.Bitmap image)
        {
            this.Dispatcher.Invoke(new System.Action(() =>
            {

                if (image != null)
                {
                    this.fingerpic.Source = ToBitMapImage(image);

                }
            }));

        }

        public BitmapImage ToBitMapImage(System.Drawing.Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }
        //---------------------------------------------------------------------------------


        private int CheckPassword(string password)
        {
            int rtn; //employee id


      
            rtn = _dbsecurity.UserAuthPin(password.Substring(0, passlen), passlen);  //function returns id

            if (rtn > 0)
            {
                Employee emp = new Employee(rtn);
                if (m_fingerprintonly && !emp.FingerPrintBypass)
                {
                    TouchMessageBox.Show("Must Use FingerPrint Scanner!!");
                    return -99;
                }
                AuditModel.WriteLog("system", "login succeed", password, "Login", 0);
                PIN = password.Substring(0, passlen);

                return rtn;
            }
            tbPin.Text = "FAILED LOGIN";
            Console.Beep(2000, 100);
            Console.Beep(2000, 100);
            AuditModel.WriteLog("system", "login fail", password, "Login", 0);
            return 0;
        }

        private int CheckMSRCard(string password)
        {
            int rtn; //employee id
            if (m_fingerprintonly)
            {
                TouchMessageBox.Show("Must Use FingerPrint Scanner!!");
                return -99;
            }

            rtn = _dbsecurity.UserAuthenticate( password,"", 0);  //function returns id

                if (rtn > 0)
                {
                    AuditModel.WriteLog("system", "login succeed", password, "Login", 0);
                    PIN = "";
                  
                    return rtn;
                }

            tbPin.Text = "FAILED LOGIN";
            Console.Beep(2000, 100);
            Console.Beep(2000, 100);
            AuditModel.WriteLog("system", "login fail", password, "Login", 0);
            return 0;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CancelCaptureAndCloseReader();
            ID = -99;
            PIN = "-99";
             this.Close();
            
        }


        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            _password= _password + button.Content.ToString();
           // Console.Beep(1000, 50);

            if (_password.Length >= passlen && m_msrcard == false)
            {
               

               ID = CheckPassword(_password);
                _password = "";

                if(ID > 0)
                {
                    CancelCaptureAndCloseReader();

                    this.Close();
                }
            
              
               
            }else
            {
                this.tbPin.Foreground = Brushes.Blue;
                tbPin.Text = new String('*', _password.Length);
            }

           
        }

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {

            if (e.Text == "%") m_msrcard = true;

            RawString = RawString + e.Text;
        
            if (e.Text == "?") trackcount++;

            if (e.Text == "\r") ProcessInput(RawString);

        }

        private void ProcessInput(string inputstr)
        {
          
            m_msrcard = false;

            if (trackcount >= 1)
            {
                string[] tracks = RawString.Split(';');
                string[] data = tracks[0].Split('^');

                string strID = data[0];
                //string name = data[1];
                //reminder:  fix name bug for auditing

             
                if (strID.Length > 1)
                       strID = strID.Replace("%B", "").Replace("%R", "").Replace("?", "").Replace("\r", "");


                        ID = CheckMSRCard(strID);
                trackcount = 0;

                if (ID > 0)
                {
                    CancelCaptureAndCloseReader();

                    this.Close();
                }



              

            }else
            {
                if (RawString.Length >= passlen)
                {
                    ID=CheckPassword(RawString);
               
                    _password = "";

                    RawString = "";
                    if (ID > 0)
                    {
                        CancelCaptureAndCloseReader();

                        this.Close();
                    }

             
                  
                }

            }


        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            CancelCaptureAndCloseReader();
        }

        private void UnChecked(object sender, RoutedEventArgs e)
        {
            // ask for security override first
            if (SecurityModel.ManagerOverride("TimeCard", "Enter Manager Override Access!!"))
            {
                m_fingerprintonly = false;
            }else
            {
                this.chkFingerPrintOnly.IsChecked = true;
            }
        }

        private void Refresh(object sender, RoutedEventArgs eventArgs)
        {
            GlobalSettings.Instance.LoadAllFmdsUserIDs();
        }
    }
}
