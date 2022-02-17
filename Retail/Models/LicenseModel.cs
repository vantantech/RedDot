﻿using System;
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

namespace RedDot
{
    public class LicenseModel
    {

        private static UnicodeEncoding _encoder = new UnicodeEncoding();
        private static string m_licensename = "license.lic";

        public LicenseModel()
        {

        }



        public static bool CheckLicense(string softwaretype)
        {
            try
            {

                string machineid = GetMachineID();


                string license = GetLicenseID();


                if (license == machineid)
                {
                    if (GetSoftware() != softwaretype)
                    {
                        MessageBox.Show("Not licensed for Retail App.  Exiting.");
                        Application.Current.Shutdown();
                        Environment.Exit(0);

                    }

                }
                else
                {


                    if (!RequestLicense(softwaretype))
                    {

                        TouchMessageBox.Show("Request has been sent to Server for license .. please check back soon.");
                        return false;
                    }


                }

                return true;

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
            catch (Exception ex)
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
            return "<RSAKeyValue><Modulus>lZhdkPj/8eNBDMMqV4CaskhSPkLjBiAGNP0ahqcRzm3FnyTlxcMi587vf7F8aoUo+WTtMXJ/ojGikYQGCY5CKOWA+FTKW/Fw5CcF7OWxqqvA7Fc8MwAJGff+dSNL50tYXg1L8y+jkuuuJ0C0Jv2HnWoKrma3geLn7fSWhZlLqDM=</Modulus><Exponent>AQAB</Exponent><P>zIW9RjIBwpKWL3rZe3o5w+gd9ZcT3GzHm2mPVdWiJMgeDvMeQeBLp8Pj4WPMP0oudNqLFJKKyMHVmEnkKAfzNQ==</P><Q>uz9tZFqUV09uH9h2JMgerkVpAVzvVzKh/xfwLhmaRP1vU4WvQfpKUpjOR7/Njico37ZJjVqgWH1diuwe95Byxw==</Q><DP>wVJ5guyp9T2ScPbytGDoUWILT6WqxGSemSzCrCPvzEzM/NyZ8TMO0Fm+AcLPwdNg7lvVs3CBdxqhx/2wiJZbMQ==</DP><DQ>irpcZsyBOQq9UVTDe07VBFKikIL1REpcIS3wIYeQd2q5D01tYll4tSdHNXtSZO54Zv6cEeFCCS2gn2t/KfXYQw==</DQ><InverseQ>eSGLJWSx+jWU4vjd6znOKeoXSh0NpRZMUjfF0jljbEnJAKgkkJ8LqqaFJzY63UoIa60WCyGFzlUW+XTHL7w+qg==</InverseQ><D>Ak9cAoUVzrJnqr7IxJkjymT7gCKgRtPvxJmz8yhZSgq/5nk3YxpkS5gScrJW7X4o59D7KKxP9D03+W9EeJlNxDXaVI6wbYGqnyJBi9Qqp+ZjtEkMVFCfwS1wZIqVb74T+kzo/cftTpqbW2eFv+hiL6rdOtOjHDvZUykwiFNAxWE=</D></RSAKeyValue>";

        }

        public static bool RequestLicense(string application)
        {
            string machineid = GetMachineID();
            string licensepath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + m_licensename;


            //Get license from server
            SalonServiceClient client = new SalonServiceClient();
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://salon.reddotpos.com/SalonService.svc");
            LicenseRequest request = new LicenseRequest();
            request.Application = application;
            request.MachineID = machineid;
            LicenseRequest response = client.GetLicense(request, GetPublicKey());

            if (response.Activated)
            {

                SaveLicense(response.CodeString);
                TouchMessageBox.Show("License downloaded successfully");

                if (GetSoftware() != application)
                {
                    MessageBox.Show("Not licensed for Retail App.  Exiting.");
                    Application.Current.Shutdown();
                    Environment.Exit(0);

                }
                return true;
            }
            else
                TouchMessageBox.Show("Request sent to server on " + response.CreateDate + " .. check back later");
            return false;
        }


        public static void SaveLicense(string codestring)
        {
            string licensepath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + m_licensename;

            //first delete any existing license
            File.Delete(licensepath);

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
               // Assembly DLL = Assembly.LoadFile(@licensepath);

                //This method does not load the file but just read the data from the file and closes it
                //therefore the file is not locked by the Assembly so can be overwritten by new license if needed
                Assembly DLL= Assembly.Load(File.ReadAllBytes(@licensepath));


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
            return data[0];

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
            }
            catch (Exception ex)
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
                // fingerPrint = GetHash("CPU >> " + cpuId() + "\nBIOS >> " + 	biosId() + "\nBASE >> " + baseId() );
                fingerPrint = GetHash("CPU >> " + cpuId() + "\nBASE >> " + baseId());
            }
            return fingerPrint;
        }
        public static string GetHash(string s)
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
        #region Original Device ID Getting Code
        //Return a hardware identifier
        private static string identifier
        (string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string result = "";
            System.Management.ManagementClass mc =
        new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() == "True")
                {
                    //Only get the first one
                    if (result == "")
                    {
                        try
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return result;
        }
        //Return a hardware identifier
        private static string identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            System.Management.ManagementClass mc =
        new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                //Only get the first one
                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }
        private static string cpuId()
        {
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as it is very time consuming
            string retVal = identifier("Win32_Processor", "UniqueId");
            if (retVal == "") //If no UniqueID, use ProcessorID
            {
                retVal = identifier("Win32_Processor", "ProcessorId");
                if (retVal == "") //If no ProcessorId, use Name
                {
                    retVal = identifier("Win32_Processor", "Name");
                    if (retVal == "") //If no Name, use Manufacturer
                    {
                        retVal = identifier("Win32_Processor", "Manufacturer");
                    }
                    //Add clock speed for extra security
                    retVal += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return retVal;
        }
        //BIOS Identifier
        private static string biosId()
        {
            return identifier("Win32_BIOS", "Manufacturer")
            + identifier("Win32_BIOS", "SMBIOSBIOSVersion")
            + identifier("Win32_BIOS", "IdentificationCode")
            + identifier("Win32_BIOS", "SerialNumber")
            + identifier("Win32_BIOS", "ReleaseDate")
            + identifier("Win32_BIOS", "Version");
        }
        //Main physical hard drive ID
        private static string diskId()
        {
            return identifier("Win32_DiskDrive", "Model")
            + identifier("Win32_DiskDrive", "Manufacturer")
            + identifier("Win32_DiskDrive", "Signature")
            + identifier("Win32_DiskDrive", "TotalHeads");
        }
        //Motherboard ID
        private static string baseId()
        {
            return identifier("Win32_BaseBoard", "Model")
            + identifier("Win32_BaseBoard", "Manufacturer")
            + identifier("Win32_BaseBoard", "Name")
            + identifier("Win32_BaseBoard", "SerialNumber");
        }
        //Primary video controller ID
        private static string videoId()
        {
            return identifier("Win32_VideoController", "DriverVersion")
            + identifier("Win32_VideoController", "Name");
        }
        //First enabled network card ID
        private static string macId()
        {
            return identifier("Win32_NetworkAdapterConfiguration",
                "MACAddress", "IPEnabled");
        }
        #endregion
    }

}
