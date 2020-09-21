using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataManager;


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
        Audit _audit;
        private string RawString;
           bool m_msrcard = false;
           int trackcount = 0;

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
            _audit = new Audit();

        }

        private int CheckPassword(string password)
        {
            int rtn; //employee id
               
                rtn = _dbsecurity.UserAuthenticate("",password.Substring(0, passlen), passlen);  //function returns id
                
                if (rtn > 0)
                {
                    _audit.WriteLog("system", "login succeed", password, "Login", 0);
                    PIN = password.Substring(0, passlen);
                    ID = rtn;
                    return rtn;
                }

            _audit.WriteLog("system", "login fail", password, "Login", 0);
            return 0;
        }

        private int CheckMSRCard(string password)
        {
            int rtn; //employee id


                rtn = _dbsecurity.UserAuthenticate( password,"", 0);  //function returns id

                if (rtn > 0)
                {
                    _audit.WriteLog("system", "login succeed", password, "Login", 0);
                    PIN = "";
                    ID = rtn;
                    return rtn;
                }


            _audit.WriteLog("system", "login fail", password, "Login", 0);
            return 0;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

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
                if (CheckPassword(_password) > 0) this.Close();
                else MessageBox.Show("Login Failed from Onscreen Entry");
                _password = "";
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
            string msrcardprefix;

            msrcardprefix = GlobalSettings.Instance.MSRCardPrefix;
            m_msrcard = false;

            if (trackcount >= 1)
            {
                string[] tracks = RawString.Split(';');
                string[] data = tracks[0].Split('^');

                string strID = data[0];
                //string name = data[1];
                //reminder:  fix name bug for auditing

                test.Text = strID;
                if (strID.Length > 1)
                    if (strID.Contains(msrcardprefix))
                    {
                        strID = strID.Replace(msrcardprefix, "").Replace("?", "").Replace("\r", "");
                        if (CheckMSRCard(strID) > 0) this.Close();
                        else MessageBox.Show("Login Failed from Card");
                    }

                trackcount = 0;

            }else
            {
                if (RawString.Length >= passlen)
                {
                    if (CheckPassword(RawString) > 0) this.Close();
                    else MessageBox.Show("Login Failed from Keyboard Entry");
                    _password = "";
                    RawString = "";
                }

            }


        }

   
    }
}
