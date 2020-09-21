using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.CodeDom.Compiler;
using System.Reflection;


namespace CreateLicenseFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<string> data = new List<string>();

            data.Add("demo");
            data.Add("salon");
            data.Add("appointment");
            data.Add("retail");
            data.Add("restaurant");
 
            cmbApplication.ItemsSource = data;
            cmbApplication.SelectedIndex = 0;


            List<string> data2 = new List<string>();
            data2.Add("pro");
            data2.Add("base");
            data2.Add("sale");
            data2.Add("appointment");

            cmbPermission.ItemsSource = data2;
            cmbPermission.SelectedIndex = 0;

        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {

            string _privateKey;
            string _publicKey2;

            string licenserequest;

            OpenFileDialog picfile = new OpenFileDialog();
            picfile.DefaultExt = "txt";
            picfile.Filter = "txt Files (txt files (*.txt)|*.txt|All files (*.*)|*.*";
            string AppPath;
            string selectedPath;
            string outputPath="";

            string machineid;
            string licensestring;
            string permissionstring;
            string codestring;

            AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            AppPath = AppPath.ToUpper();

            if (picfile.ShowDialog() == true)
            {
                selectedPath = picfile.FileName;
                outputPath = selectedPath.Replace(picfile.SafeFileName,"");
                selectedPath = selectedPath.ToUpper();
                selectedPath = selectedPath.Replace(AppPath, "");
                licenserequest = Utility.ReadStringFromFile(selectedPath);
                _privateKey = "<RSAKeyValue><Modulus>yQ/uP0rOX41e6on5A44HCmGSxDBN+kKY8zolcYu1cN4GTwcyzWM4wv6ZRKBf3+5hH5Yt335NyisyUu+cfkfvs1ST5wwRgfV1Ql2azXADDKMGPZRhGux/x7oQEDufwDm4yY4qfJ/kx/IyHrs+tl60J1wiiE+s/CnxQMvrbSaFnVE=</Modulus><Exponent>AQAB</Exponent><P>/MHVwFcaFUHQLvLl5blzBhZptceckbYofX90zjU1vX5uFOKLOQX1r1vUg+ng3Qt5CPS+FPi3mPLHD931zMPnvQ==</P><Q>y6ROkZ+f2h5iXrXDs7UrCFyPsYWhx015TG5ff5K6fHXj2TGZE98gDOIgRsAS7KcdPZsmwBst4GnlE5b6REALJQ==</Q><DP>v0ztvQ+vnBsduAr7WW2M0zSveXfE1rvp1WJcQ54eOHeyVXhJKzWJh9mW9OhU2rhOOSsTmsfMHaTSaP3zhbFYeQ==</DP><DQ>K/yVrAbatHaTsPl6CDs9zFSSBTpkM3ScmtHMdvXuqiucx7Fa61vqxF2jsySR8eQ3ALOerygvxKWbAZw++rcKsQ==</DQ><InverseQ>dlt+ktdRHgbP6Gl+oTD2+E6oQTu2V/Fe70hXcZ6IS8+iG1/CNhzNqiX9nN7okl2blQVV9RCxGOYprjvC+3KKdA==</InverseQ><D>wWkssv+/5BT3IEDXJL9EMI1KBKW+7SWRQjBGAqL1R/ycLvtquD5hRNprD6QCdkQ2c48g06QJKphBEZzjemqyvaVuA6rIxm9rZUmQ0T7CjFkqDOQb/gx12XzVQbb66mBMWxLJS++Tfpf+JzYBs+homGsbI1101GnKk/I2AP9mO1E=</D></RSAKeyValue>";

                machineid = Licensing.Decrypt(licenserequest, _privateKey);
                MessageBox.Show("MachineID:" + machineid);

                _publicKey2 = "<RSAKeyValue><Modulus>lZhdkPj/8eNBDMMqV4CaskhSPkLjBiAGNP0ahqcRzm3FnyTlxcMi587vf7F8aoUo+WTtMXJ/ojGikYQGCY5CKOWA+FTKW/Fw5CcF7OWxqqvA7Fc8MwAJGff+dSNL50tYXg1L8y+jkuuuJ0C0Jv2HnWoKrma3geLn7fSWhZlLqDM=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";


              



                licensestring = Licensing.Encrypt(machineid , _publicKey2);

                permissionstring = Licensing.Encrypt(cmbApplication.SelectedValue.ToString() + "," +   cmbPermission.SelectedValue.ToString() + "," + txtStartDate.Text + "," + txtEndDate.Text , _publicKey2);



                //Utility.WriteStringToFile(outputPath + "license.txt", licensestring);
                //MessageBox.Show("license.txt file created");

                //compile DLL
                System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
                parameters.GenerateExecutable = false;
                parameters.OutputAssembly = outputPath +  "License.dll";

                codestring = "public static class License {          public static string LoadEncryptedLicense(){ return \"" + licensestring + "\";}    public static string LoadEncryptedPermission(){ return \"" + permissionstring + "\";}           }";

                CompilerResults r = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(parameters, codestring);

                MessageBox.Show("license.dll created");
            }


            //  var rsa = new RSACryptoServiceProvider();










        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
