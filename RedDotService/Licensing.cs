using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Collections;
using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace RedDotService
{
    public class Licensing
    {

        private static UnicodeEncoding _encoder = new UnicodeEncoding();

        public Licensing()
        {

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

        public static LicenseRequest GetLicense(LicenseRequest request, string publickey)
        {


            string Encrypted_licensestring="test";
            string Encrypted_permissionstring="test";
            // SoftwareLicense license = new SoftwareLicense();


     
            DBLicense _dblicense = new DBLicense();

            //create license by encrypting

            LicenseRequest response = _dblicense.GetPermission(request);

            if (response.Activated)
            {
               Encrypted_licensestring = Licensing.Encrypt(response.MachineID, publickey);
                Encrypted_permissionstring = Licensing.Encrypt(response.CodeString, publickey);

               string codestring = "public static class License {  public static string LoadEncryptedLicense(){ return \"" + Encrypted_licensestring + "\";}    public static string LoadEncryptedPermission(){ return \"" + Encrypted_permissionstring + "\";}  }";
                response.CodeString = codestring;

            }


            return response;


        }

    }


}
