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

using NLog;
using RedDot;
using RedDot.DataManager;


namespace RedDot
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {


        private static Logger logger = LogManager.GetCurrentClassLogger();

        private int passlen;
        DBSecurity _dbsecurity;
        public string PIN { get; set; }
        public int ID { get; set; }
        private string _password;
        private string RawString;
           bool m_msrcard = false;
           int trackcount = 0;

        //fingerprint code
        private FingerPrint reader;
    
        private static string[] usernames;
        private static int[] userids;
   


        public Brush ButtonBackground
        {
            get { return Brushes.Turquoise; }
        }
        public Login()
        {
            InitializeComponent();
            this.DataContext = this;
            
            passlen = GlobalSettings.Instance.PinLength;
    
           // txtPassword.Text = "";
            _password = "";
            _dbsecurity = new DBSecurity();

            test.Text = "";
            tbPin.Text = "";


            if(GlobalSettings.Instance.EnableFingerPrint)
            {
                reader = new FingerPrint();

                try
                {
                    var result = reader.InitializeReaders();
                    if (result == false)
                    {

                        TouchMessageBox.Show("Finger Print libraries or device is missing");
                        return;
                    }

                    reader.StartIndentification();


                }
                catch (Exception ex)
                {
                    if (ex.Source == "DPUruNet")
                    {
                        TouchMessageBox.Show("Finger Print libraries are missing.  Please install.");
                    }
                    else
                        TouchMessageBox.Show(ex.Message);
                }
            }

         
        }

        //fingerpirnt code starts

    

    

        public void Verified(int id)
        {
            this.Dispatcher.Invoke(new System.Action(() =>
            {

                ID = id;
                this.Close();
                Console.Beep(5000, 100);

            }));

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
        //


        //fingerprint code ends

        private int CheckPassword(string password)
        {
            int rtn; //employee id
               
                rtn = _dbsecurity.UserAuthenticate("",password.Substring(0, passlen), passlen);  //function returns id
                
                if (rtn > 0)
                {
                    AuditModel.WriteLog("system", "login succeed", password, "Login", 0);
                    logger.Info("login succeeded" );
                    PIN = password.Substring(0, passlen);
                 
                    return rtn;
                }

            logger.Info("log in failed:" + password);
                AuditModel.WriteLog("system", "login fail", password, "Login", 0);
            return 0;
        }

        private int CheckMSRCard(string password)
        {
            int rtn; //employee id


                rtn = _dbsecurity.UserAuthenticate( password,"", 0);  //function returns id

                if (rtn > 0)
                {
                    AuditModel.WriteLog("system", "login succeed", password, "Login", 0);
                    logger.Info("login succeeded");
                    PIN = "";
                  
                    return rtn;
                }


                AuditModel.WriteLog("system", "login fail", password, "Login", 0);
            logger.Info("log in failed:" + password);
            return 0;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if(reader!=null)
            reader.CancelCaptureAndCloseReader();
            ID = -99;
            PIN = "-99";
             this.Close();
            
        }


        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            _password= _password + button.Content.ToString();
            if (_password.Length >= passlen && m_msrcard == false)
            {
               ID= CheckPassword(_password);
                _password = "";

                if (ID > 0)
                {
                    if(reader != null)
                        reader.CancelCaptureAndCloseReader();

                    this.Close();
                }


            }

            tbPin.Text = new String('*',_password.Length);
        }

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {

            if (e.Text == "%") m_msrcard = true;

            RawString = RawString + e.Text;
            test.Text = RawString;
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

                string strID = data[0].ToUpper();
                //string name = data[1];
                //reminder:  fix name bug for auditing

                test.Text = strID;

                if (strID.Length > 1)
                
                        strID = strID.Replace("%B", "").Replace("%R", "").Replace("?", "").Replace("\r", "");
                       ID= CheckMSRCard(strID);

                trackcount = 0;
                if (ID > 0)
                {
                    if(reader!=null)
                    reader.CancelCaptureAndCloseReader();

                    this.Close();
                }



            }
            else
            {
                if (RawString.Length >= passlen)
                {
                    ID = CheckPassword(RawString);

                    _password = "";

                    RawString = "";
                    if (ID > 0)
                    {
                        if(reader != null)
                        reader.CancelCaptureAndCloseReader();

                        this.Close();
                    }



                }

            }


        }


        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if(reader!=null)
            reader.CancelCaptureAndCloseReader();
        }

    }
}
