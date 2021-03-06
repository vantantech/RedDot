using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Collections;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Windows;
using System.Reflection;
using System.IO;
using RedDot.ServiceReference1;
using System.CodeDom.Compiler;
using DeviceId;

namespace RedDot
{
    public class LicenseModel
    {

        private static UnicodeEncoding _encoder = new UnicodeEncoding();
        private static string m_licensename = "license.lic";
      
        public LicenseModel()
        {
            
        }



        public static bool CheckLicense()
        {
            try
            {

                string machineid = GetMachineID();


                string license = GetLicenseID();

                if (license == "") return false;


                if (license == machineid)
                {
                    return true;
                }
                else
                {

                    TouchMessageBox.Show("License does not match machine ID.  MachineID=" + machineid + "  LicenseID=" + license);

                    return false;
                }

            }
            catch (Exception ex)
            {

                return false;
            }
        }


        public static string Decrypt(string data, string _privateKey)
        {
            var rsa = new RSACryptoServiceProvider();
            var dataArray = data.Split(new char[] { ',' });
            byte[] dataByte = new byte[dataArray.Length];
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataByte[i] = Convert.ToByte(dataArray[i]);
            }

            rsa.FromXmlString(_privateKey);
            var decryptedByte = rsa.Decrypt(dataByte, false);
            return _encoder.GetString(decryptedByte);
        }

        public static string Encrypt(string data, string _publicKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(_publicKey);
            var dataToEncrypt = _encoder.GetBytes(data);
            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            var length = encryptedByteArray.Count();
            var item = 0;
            var sb = new StringBuilder();
            foreach (var x in encryptedByteArray)
            {
                item++;
                sb.Append(x);

                if (item < length)
                    sb.Append(",");
            }

            return sb.ToString();
        }

        public static string GetPermission()
        {
            string returnstring = "";

            //return Decrypt(License.LoadEncryptedPermission(), _privateKey2);

            try
            {
                string licensepath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + m_licensename;

                if (File.Exists(licensepath))
                {
                    Assembly DLL = Assembly.LoadFile(@licensepath);

                    Type type = DLL.GetType("License");

                    returnstring = Decrypt((string)type.GetMethod("LoadEncryptedPermission").Invoke(null, null), GetPrivateKey());

                    return returnstring;
                }
                else return "";

            }
            catch(Exception ex)
            {
                return ex.Message;
            }

           
        }


        public static string GetPublicKey()
        {
            return "<RSAKeyValue><Modulus>lZhdkPj/8eNBDMMqV4CaskhSPkLjBiAGNP0ahqcRzm3FnyTlxcMi587vf7F8aoUo+WTtMXJ/ojGikYQGCY5CKOWA+FTKW/Fw5CcF7OWxqqvA7Fc8MwAJGff+dSNL50tYXg1L8y+jkuuuJ0C0Jv2HnWoKrma3geLn7fSWhZlLqDM=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        }

        public static string GetPrivateKey()
        {
            return  "<RSAKeyValue><Modulus>lZhdkPj/8eNBDMMqV4CaskhSPkLjBiAGNP0ahqcRzm3FnyTlxcMi587vf7F8aoUo+WTtMXJ/ojGikYQGCY5CKOWA+FTKW/Fw5CcF7OWxqqvA7Fc8MwAJGff+dSNL50tYXg1L8y+jkuuuJ0C0Jv2HnWoKrma3geLn7fSWhZlLqDM=</Modulus><Exponent>AQAB</Exponent><P>zIW9RjIBwpKWL3rZe3o5w+gd9ZcT3GzHm2mPVdWiJMgeDvMeQeBLp8Pj4WPMP0oudNqLFJKKyMHVmEnkKAfzNQ==</P><Q>uz9tZFqUV09uH9h2JMgerkVpAVzvVzKh/xfwLhmaRP1vU4WvQfpKUpjOR7/Njico37ZJjVqgWH1diuwe95Byxw==</Q><DP>wVJ5guyp9T2ScPbytGDoUWILT6WqxGSemSzCrCPvzEzM/NyZ8TMO0Fm+AcLPwdNg7lvVs3CBdxqhx/2wiJZbMQ==</DP><DQ>irpcZsyBOQq9UVTDe07VBFKikIL1REpcIS3wIYeQd2q5D01tYll4tSdHNXtSZO54Zv6cEeFCCS2gn2t/KfXYQw==</DQ><InverseQ>eSGLJWSx+jWU4vjd6znOKeoXSh0NpRZMUjfF0jljbEnJAKgkkJ8LqqaFJzY63UoIa60WCyGFzlUW+XTHL7w+qg==</InverseQ><D>Ak9cAoUVzrJnqr7IxJkjymT7gCKgRtPvxJmz8yhZSgq/5nk3YxpkS5gScrJW7X4o59D7KKxP9D03+W9EeJlNxDXaVI6wbYGqnyJBi9Qqp+ZjtEkMVFCfwS1wZIqVb74T+kzo/cftTpqbW2eFv+hiL6rdOtOjHDvZUykwiFNAxWE=</D></RSAKeyValue>";

        }

        public static bool RequestLicense(string application)
        {
            string machineid = GetMachineID();
            TouchMessageBox.Show("MachineID:" + machineid,30);

            string licensepath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + m_licensename;


            //Get license from server
            SalonServiceClient client = new SalonServiceClient();
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://salon.reddotpos.com/SalonService.svc");
            LicenseRequest request = new LicenseRequest();
            request.Application = application;
            request.MachineID = machineid;

            TouchMessageBox.Show("Connecting to license server...");

            LicenseRequest response = client.GetLicense(request, GetPublicKey());

            if (response.Activated)
            {

                SaveLicense(response.CodeString);
                TouchMessageBox.Show("License downloaded successfully");
                return true;
            } else
                TouchMessageBox.Show("Request sent to server on " + response.CreateDate + " .. check back later");
            return false;
        }


        public static void SaveLicense(string codestring)
        {
            string licensepath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + m_licensename;

            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.OutputAssembly = licensepath;
            CompilerResults r = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(parameters, codestring);

        }


        public static string GetLicenseID()
        {
        
            string licensepath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + m_licensename;

            if (File.Exists(licensepath))
            {
                Assembly DLL = Assembly.LoadFile(@licensepath);
                Type type = DLL.GetType("License");
                return Decrypt((string)type.GetMethod("LoadEncryptedLicense").Invoke(null, null), GetPrivateKey());

            }
            else
            {

                return "";
            }
      
            
        }
        public static string GetSoftware()
        {
            string[] data = GetPermission().Split(',');
           return  data[0];

        }

        public static string GetSoftwareType()
        {
            string[] data = GetPermission().Split(',');
            if (data.Count() > 1) return data[1].ToString();
            else return "";

        }


        public static DateTime GetStartDate()
        {
            try
            {
                string[] data = GetPermission().Split(',');
                if (data.Count() > 2) return Convert.ToDateTime(data[2]);
                else return DateTime.Now;
            }catch(Exception ex)
            {
                TouchMessageBox.Show("License:GetStartDate: " + ex.Message);
                return DateTime.Now.AddYears(20);
            }
      

        }

        public static DateTime GetEndDate()
        {
            try
            {
                string[] data = GetPermission().Split(',');
                if (data.Count() > 3) return Convert.ToDateTime(data[3]);
                else return DateTime.Now.AddYears(10);
            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("License:GetEndDate: " + ex.Message);
                return DateTime.Now.AddYears(-20);
            }
           

        }
        public static bool CheckPermission(string permission)
        {

            try
            {
                 //read string from DLL

                string permissionstring = "";

                //decrypt string using private key2
                //get license info
                string[] data = GetPermission().Split(',');
                if (data.Count() > 4) permissionstring = data[4].ToUpper();


                return permissionstring.Contains(permission.ToUpper());
              

            }
            catch 
            {
               // TouchMessageBox.Show("License File Not Found..." + e.Message);
                return false;
            }


        }

  


        private static string fingerPrint = string.Empty;
        public static string GetMachineID()
        {


            if (string.IsNullOrEmpty(fingerPrint))
            {

                fingerPrint = GetHash(new DeviceIdBuilder().AddMotherboardSerialNumber().AddProcessorId().AddSystemDriveSerialNumber().AddSystemUUID().ToString());
            }
            return fingerPrint;
        }
        public  static string GetHash(string s)
        {
            MD5 sec = new MD5CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bt = enc.GetBytes(s);
            return GetHexString(sec.ComputeHash(bt));
        }
        private static string GetHexString(byte[] bt)
        {
            string s = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                int n, n1, n2;
                n = (int)b;
                n1 = n & 15;
                n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + (int)'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char)(n1 - 10 + (int)'A')).ToString();
                else
                    s += n1.ToString();
                if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
            }
            return s;
        }



    }

}
