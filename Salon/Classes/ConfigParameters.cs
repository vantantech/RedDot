using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace HeartPOS
{
    [Serializable]
    public class CustomForms
    {
        public string FormName
        {
            get;
            set;
        }
        public bool IsPreForm
        {
            get;
            set;
        }
        public bool IsPostForm
        {
            get;
            set;
        }
    }


    [Serializable]
    public class SIPIPAddress
    {
        public string IPAddress
        {
            get;
            set;
        }
        public bool IsSelected
        {
            get;
            set;
        }

    }

    [Serializable]
    [XmlRoot("ConfigParameters")]
    public class ConfigParameters : INotifyPropertyChanged, ICloneable
    {
        private string ecrId;
        private int resetDelay;
        private bool confirmationAmount = false;
        private decimal taxPercent;
        private decimal tipPercent;
        private decimal ebtPercent;
        private int fieldCount;

        private bool enableTipPercent;
        private bool enableEBTPercent;

        private string serverLabel1;
        private string serverLabel2;
        private string serverLabel3;
        private string serverLabel4;
        private string serverLabel5;

        private bool enableTaxPercent;
        private bool enableLineItem;
        private string ackDelay;
        private bool boldMandatorytags;
        private bool enableMultiCardSelection;
        private string terminalId;
        private string applicationId;
        private string downloadType;
        private string downloadTime;
        private string downloadPort;
        private string downloadURL;
        private string header1;
        private string header2;
        private string header3;
        private string header4;
        private string footer1;
        private string footer2;
        private string footer3;
        private string footer4;
        private string requestId;

        private List<CustomForms> customFormCollection = new List<CustomForms>();
        private List<SIPIPAddress> IPAddressCollection = new List<SIPIPAddress>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [XmlElement("ECRId")]
        public string ECRId
        {
            get
            {
                return ecrId;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SetProperty(ref ecrId, value);
                }
            }
        }

        [XmlElement("ResetDelay")]
        public int ResetDelay
        {
            get
            {
                return resetDelay;
            }
            set
            {
                SetProperty(ref resetDelay, value);
            }
        }

        [XmlElement("ConfirmAmount")]
        public bool ConfirmAmount
        {
            get
            {
                return confirmationAmount;
            }
            set
            {
                SetProperty(ref confirmationAmount, value);
            }
        }


        [XmlElement("EnableTaxPercent")]
        public bool EnableTaxPercent
        {
            get
            {
                return enableTaxPercent;
            }
            set
            {
                SetProperty(ref enableTaxPercent, value, "EnableTaxPercent");
            }
        }

        [XmlElement("TaxPercent")]
        public decimal TaxPercent
        {
            get
            {
                return taxPercent;
            }
            set
            {
                SetProperty(ref taxPercent, value, "TaxPercent");
            }
        }

        [XmlElement("EnableTipPercent")]
        public bool EnableTipPercent
        {
            get
            {
                return enableTipPercent;
            }
            set
            {
                SetProperty(ref enableTipPercent, value, "EnableTipPercent");
            }
        }

        [XmlElement("TipPercent")]
        public decimal TipPercent
        {
            get
            {
                return tipPercent;
            }
            set
            {
                SetProperty(ref tipPercent, value, "TipPercent");
            }
        }

        [XmlElement("EnableEBTPercent")]
        public bool EnableEBTPercent
        {
            get { return enableEBTPercent; }
            set
            {
                SetProperty(ref enableEBTPercent, value, "EnableEBTPercent");
            }
        }

        [XmlElement("EBTPercent")]
        public decimal EBTPercent
        {
            get
            {
                return ebtPercent;
            }
            set
            {
                SetProperty(ref ebtPercent, value, "EBTPercent");
            }
        }


        [XmlElement("EnableLineItem")]
        public bool EnableLineItem
        {
            get { return enableLineItem; }
            set
            {
                SetProperty(ref enableLineItem, value);
            }
        }

        [XmlElement("FieldCount")]
        public int FieldCount
        {
            get
            {
                return fieldCount;
            }
            set
            {
                SetProperty(ref fieldCount, value);
            }
        }

        [XmlElement("ServerLabel1")]
        public string ServerLabel1
        {
            get { return serverLabel1; }
            set
            {
                SetProperty(ref serverLabel1, value);
            }
        }

        [XmlElement("ServerLabel2")]
        public string ServerLabel2
        {
            get { return serverLabel2; }
            set
            {
                SetProperty(ref serverLabel2, value);
            }
        }

        [XmlElement("ServerLabel3")]
        public string ServerLabel3
        {
            get { return serverLabel3; }
            set
            {
                SetProperty(ref serverLabel3, value);
            }
        }

        [XmlElement("ServerLabel4")]
        public string ServerLabel4
        {
            get { return serverLabel4; }
            set
            {
                SetProperty(ref serverLabel4, value);
            }
        }

        [XmlElement("ServerLabel5")]
        public string ServerLabel5
        {
            get { return serverLabel5; }
            set
            {
                SetProperty(ref serverLabel5, value);
            }
        }

        [XmlElement("CustomForms")]
        public List<CustomForms> CustomFormCollection
        {
            get
            {
                return customFormCollection;
            }
            set
            {
                customFormCollection = value;
            }
        }



        [XmlElement("AckDelay")]
        public string AckDelay
        {
            get { return ackDelay; }
            set
            {
                SetProperty(ref ackDelay, value);
            }
        }


        [XmlElement("BoldMandatorytags")]
        public bool BoldMandatorytags
        {
            get { return boldMandatorytags; }
            set
            {
                SetProperty(ref boldMandatorytags, value);
            }
        }


        [XmlElement("EnableMultiCardSelection")]
        public bool EnableMultiCardSelection
        {
            get { return enableMultiCardSelection; }
            set
            {
                SetProperty(ref enableMultiCardSelection, value);
            }
        }


        [XmlElement("TerminalId")]
        public string TerminalId
        {
            get { return terminalId; }
            set
            {
                SetProperty(ref terminalId, value);
            }
        }


        [XmlElement("ApplicationId")]
        public string ApplicationId
        {
            get { return applicationId; }
            set
            {
                SetProperty(ref applicationId, value);
            }
        }


        [XmlElement("DownloadType")]
        public string DownloadType
        {
            get { return downloadType; }
            set
            {
                SetProperty(ref downloadType, value);
            }
        }

        [XmlElement("DownloadTime")]
        public string DownloadTime
        {
            get { return downloadTime; }
            set
            {
                SetProperty(ref downloadTime, value);
            }
        }

        [XmlElement("DownloadPort")]
        public string DownloadPort
        {
            get { return downloadPort; }
            set
            {
                SetProperty(ref downloadPort, value);
            }
        }

        [XmlElement("DownloadURL")]
        public string DownloadURL
        {
            get { return downloadURL; }
            set
            {
                SetProperty(ref downloadURL, value);
            }
        }

        [XmlElement("Header1")]
        public string Header1
        {
            get { return header1; }
            set
            {
                SetProperty(ref header1, value);
            }
        }

        [XmlElement("Header2")]
        public string Header2
        {
            get { return header2; }
            set
            {
                SetProperty(ref header2, value);
            }
        }

        [XmlElement("Header3")]
        public string Header3
        {
            get { return header3; }
            set
            {
                SetProperty(ref header3, value);
            }
        }

        [XmlElement("Header4")]
        public string Header4
        {
            get { return header4; }
            set
            {
                SetProperty(ref header4, value);
            }
        }

        [XmlElement("Footer1")]
        public string Footer1
        {
            get { return footer1; }
            set
            {
                SetProperty(ref footer1, value);
            }
        }

        [XmlElement("Footer2")]
        public string Footer2
        {
            get { return footer2; }
            set
            {
                SetProperty(ref footer2, value);
            }
        }

        [XmlElement("Footer3")]
        public string Footer3
        {
            get { return footer3; }
            set
            {
                SetProperty(ref footer3, value);
            }
        }

        [XmlElement("Footer4")]
        public string Footer4
        {
            get { return footer4; }
            set
            {
                SetProperty(ref footer4, value);
            }
        }

        [XmlElement("RequestId")]
        public string RequestId
        {
            get
            {
                return requestId;
            }
            set
            {
                SetProperty(ref requestId, value);
            }
        }

        [XmlElement("IsSerial")]
        public bool IsSerial
        {
            get;
            set;
        }

        [XmlElement("IsTCP")]
        public bool IsTCP
        {
            get;
            set;
        }

        [XmlElement("IsOther")]
        public bool IsOther
        {
            get;
            set;
        }

        private int sipClient;
        [XmlElement("Client")]
        public int Client
        {
            get { return sipClient; }
            set
            {
                SetProperty(ref sipClient, value);
            }
        }

        [XmlElement("IsSSL")]
        public bool IsSSL
        {
            get;
            set;
        }

        [XmlElement("IsBluetooth")]
        public bool IsBluetooth
        {
            get;
            set;
        }

        [XmlElement("IsHTTP")]
        public bool IsHTTP
        {
            get;
            set;
        }

        //private string sipIPAddress;
        //[XmlElement("IPAddress")]
        //public string IPAddress
        //{
        //    get { return sipIPAddress; }
        //    set
        //    {
        //        SetProperty(ref sipIPAddress, value);
        //    }
        //}


        private string defaultIPAddress;
        [XmlElement("IPAddress")]
        public string DefaultSIPIPAddress
        {
            get
            {
                return defaultIPAddress;
            }
            set
            {
                defaultIPAddress = value;
            }

        }

        private string getSelectedIPAddress;
        public string GetSelectedIPAddress
        {
            get
            {
                var SIPIPAddress = SIPIPAddressCollection
                        .Where(m => m.IsSelected == true).ToList();
                if (SIPIPAddress.Count > 0)
                {
                    getSelectedIPAddress = SIPIPAddress[0].IPAddress;
                }
                else
                {
                    getSelectedIPAddress = defaultIPAddress;
                }

                return getSelectedIPAddress;
            }
        }

        private bool isIPConnectFirst;
        public bool IsIPConnectFirst
        {
            get
            {
                return isIPConnectFirst;
            }
            set
            {
                SetProperty(ref isIPConnectFirst, value);
            }
        }


        private bool isConnectClick;
        public bool IsConnectClick
        {
            get
            {
                return isConnectClick;
            }
            set
            {
                SetProperty(ref isConnectClick, value);
            }
        }

        [XmlElement("SIPIPAddressCollection")]
        public List<SIPIPAddress> SIPIPAddressCollection
        {
            get
            {
                return IPAddressCollection;
            }
            set
            {
                IPAddressCollection = value;
            }
        }

        private string sipIPPort;
        [XmlElement("IPPort")]
        public string IPPort
        {
            get { return sipIPPort; }
            set
            {
                SetProperty(ref sipIPPort, value);
            }
        }

        private string serialPort;
        [XmlElement("SerialPort")]
        public string SerialPort
        {
            get { return serialPort; }
            set
            {
                SetProperty(ref serialPort, value);
            }
        }

        private string serialParity;
        [XmlElement("SerialParity")]
        public string SerialParity
        {
            get { return serialParity; }
            set
            {
                SetProperty(ref serialParity, value);
            }
        }

        private string baudRate;
        [XmlElement("BaudRate")]
        public string BaudRate
        {
            get { return baudRate; }
            set
            {
                SetProperty(ref baudRate, value);
            }
        }

        [XmlElement("ParityValue")]
        public Parity ParityValue
        {
            get;
            set;
        }

        [XmlElement("DataBits")]
        public int DataBits
        {
            get;
            set;
        }

        [XmlElement("StopBits")]
        public StopBits StopBits
        {
            get;
            set;
        }

        private int sipForceNAK;
        [XmlElement("SIPForceNAK")]
        public int SIPForceNAK
        {
            get { return sipForceNAK; }
            set
            {
                SetProperty(ref sipForceNAK, value);
            }
        }

        private int posForceNAK;
        [XmlElement("POSForceNAK")]
        public int POSForceNAK
        {
            get { return posForceNAK; }
            set
            {
                SetProperty(ref posForceNAK, value);
            }
        }

        private int maxBufferSize;
        [XmlElement("MaxBufferSize")]
        public int MaxBufferSize
        {
            get { return maxBufferSize; }
            set
            {
                SetProperty(ref maxBufferSize, value);
            }
        }

        [XmlElement("VersionNumber")]
        public string VersionNumber
        {
            get;
            set;
        }

        public void SaveConfigParameter()
        {
            RestrictDefaultIP();
            XmlSerializer serializer = new XmlSerializer(typeof(ConfigParameters));
            XmlDocument xmlDocument = new XmlDocument();
            using (MemoryStream stream = new MemoryStream())
            {
                string xmlPath = ConfigurationManager.AppSettings["XMLPath"] + "UserModifiedConfig.xml";
                serializer.Serialize(stream, this);
                stream.Position = 0;

                xmlDocument.Load(stream);
                try
                {
                    if (File.Exists(xmlPath))
                    {
                        File.SetAttributes(xmlPath, FileAttributes.Normal);
                    }
                    xmlDocument.Save(xmlPath);
                }
                catch
                {
                    throw new Exception("Error in saving the user configuration.");
                }
                finally
                {
                    if (File.Exists(xmlPath))
                    {
                        File.SetAttributes(xmlPath, FileAttributes.Hidden);
                    }
                    stream.Close();
                }
            }
        }

        public void RestrictDefaultIP()
        {
            SIPIPAddressCollection.RemoveAll((m) => m.IPAddress == DefaultSIPIPAddress);            
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
