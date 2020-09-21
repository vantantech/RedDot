using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace RedDot
{
    /// <summary>
    /// Interaction logic for GiftCardScanner.xaml
    /// </summary>
    public partial class GetDriverLicense : Window
    {
        public string RawString = "";
 

        int trackcount = 0;

        public DriverLicense CustomerLicense = new DriverLicense();
       
        public ObservableCollection<DriverLicense> Licenses { get; set; }
        public GetDriverLicense(ObservableCollection<DriverLicense> lic)
        {
            InitializeComponent();
            this.DataContext = this;

            CustomerLicense.LicenseNo = "";
            Licenses = lic;
        }




        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "%")
            {
                this.tbTemp.Foreground = Brushes.White;
                this.tbTemp.Text = "";
                txtDL.Text = "Reading ...";
                RawString = "";
                trackcount = 0;
            }
            RawString = RawString + e.Text;

            if (e.Text == "?") trackcount++;

            if(trackcount == 2) ProcessInput(RawString);

            if (e.Text == "\r") ProcessInput(RawString);

            this.tbTemp.Text = RawString;

        }

        private void ProcessInput(string inputstr)
        {



            try
            {


                if (trackcount >= 2)
                {
                    string[] tracks = RawString.Split(';');

                    //process track 1
                    string[] namedata = tracks[0].Split('^');
                    string[] dobdata = tracks[1].Split('=');

                    string location="";
                    string[] customername;
                    string customeraddress;

                    if (namedata.Length >= 1)
                    {

                        location = namedata[0].Replace("%","");
                        customername = namedata[1].Split('$');
                        customeraddress = namedata[2];



                        CustomerLicense.FirstName = customername[1];
                        CustomerLicense.LastName = customername[0];
                        CustomerLicense.Address = customeraddress;
                        CustomerLicense.City = location.Substring(2,location.Length - 2);
                        CustomerLicense.State = location.Substring(0, 2);


                        RawString = "";


                    
                        this.tbTemp.Text = "";
                        this.txtFirstName.Text = customername[1] ;
                        this.txtLastName.Text = customername[0];
                        this.txtAddress.Text = customeraddress + "\r\n" + CustomerLicense.City + "," + CustomerLicense.State;


                        trackcount = 0;
                    }

                    string dobstr = "";
                    string dl = "";
                    if(dobdata.Length >= 1)
                    {
                        dl = "****" + dobdata[0].Substring(dobdata[0].Length-4);
                        dobstr = dobdata[1];

                        this.txtDL.Text = dl;
                        string temp = dobstr.Substring(4, 8);
                        DateTime dob = DateTime.Parse(temp.Substring(0, 4) + "-" + temp.Substring(4, 2) + "-" + temp.Substring(6, 2));
                        this.txtDOB.Text = dob.ToShortDateString();
                        this.txtExpire.Text = "20" +  dobstr.Substring(0, 2) + "/" + dobstr.Substring(2, 2);

                        CustomerLicense.DOB = dob;
                        CustomerLicense.LicenseNo = dl;

                   
                      
                        this.txtAge.Text = CustomerLicense.Age.ToString();

                        if(CustomerLicense.Age < GlobalSettings.Instance.MinimumAgeRestriction)
                        {
                            this.ageborder.Background = Brushes.Red;
                            this.txtAlert.Text = "Customer is less than " + GlobalSettings.Instance.MinimumAgeRestriction;
                        }else
                        {
                            this.txtAlert.Text = "";
                            this.ageborder.Background = Brushes.Transparent;
                        }
                    }



                }
            }
            catch (Exception e)
            {
                tbTemp.Foreground = Brushes.Red;
                tbTemp.Text = "Error reading card .. please try again" + e.Message;
                RawString = "";
                trackcount = 0;
            }
        }



        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtDOB_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string temp = txtDOB.Text;
                if (temp.Length >= 10)
                {
                    DateTime dob = DateTime.Parse(temp);
                    CustomerLicense.DOB = dob;
                 
                    this.txtAge.Text = CustomerLicense.Age.ToString();
                }
            }
            catch
            {
                this.txtAge.Text = "";
            }
      
         
        }

        private void txtDL_KeyUp(object sender, KeyEventArgs e)
        {
            CustomerLicense.LicenseNo = txtDL.Text;
        }

 

        private void txtFirstName_KeyUp(object sender, KeyEventArgs e)
        {
            CustomerLicense.FirstName = txtFirstName.Text;
        }

        private void txtLastName_KeyUp(object sender, KeyEventArgs e)
        {
            CustomerLicense.LastName = txtLastName.Text;
        }

        private void CustomerClicked(object sender, RoutedEventArgs e)
        {
            Button picked = sender as Button;
            string license = picked.Tag.ToString();

            CustomerLicense = Licenses.Where(x => x.LicenseNo == license).FirstOrDefault();
            this.Close();
        }
    }
}
