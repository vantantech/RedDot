using HeartPOS;
using System;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Linq;
//using InTheHand.Net;
//using InTheHand.Net.Bluetooth;
//using InTheHand.Net.Bluetooth.Widcomm;
//using InTheHand.Net.Sockets;

namespace HPS
{
    public class HeartSIP
    {
        private ConfigParameters configParameters = new ConfigParameters();

        public SerialPort spPort;
        private int POSNAKCount = 0;
        private int SIPNAKCount = 0;
        private int IPRecvPacketSize = 0;

        private TcpClient TCPClient = null;

        private TcpClient SSLClient = null;
        private SslStream sslStream = null;
        private String XMLRequestMessage;

        private HttpClient HTTPClient = null;
        private NetworkStream tcpStream = null;

        public delegate void POSDisplay(int MessageType, byte[] Message);
        public delegate void DisplayConnectionType();
        public POSDisplay posDisplay;
        public DisplayConnectionType displayConnectionType;
        private int isIpFirstConnect = 0;

        public ConfigParameters ConfigParameters
        {
            get { return configParameters; }
            set { configParameters = value; }
        }

        public string SIPCertificate
        {
            get
            {
                return ConfigurationManager.AppSettings["SIPCertificate"].ToString();
            }
        }

        public string CACertificate
        {
            get
            {
                return ConfigurationManager.AppSettings["CACertificate"].ToString();
            }
        }

        public HeartSIP(ConfigParameters configParameter, POSDisplay posDisplay,DisplayConnectionType displayConnectionType)
        {
            ConfigParameters = (ConfigParameters)configParameter.Clone();
            this.posDisplay = posDisplay;
            this.displayConnectionType = displayConnectionType;
        }

        public bool Connect(bool stopConnnection = false)
        {
            if (ConfigParameters.IsSerial)
                return (SerialConnect());
            else if (ConfigParameters.IsTCP)
                return (TCPConnect(stopConnnection));
            else if (ConfigParameters.IsOther)
            {
                if (ConfigParameters.Client == 3)
                    return (BluetoothConnect());
                else if (ConfigParameters.Client == 4)
                    return (HTTPConnect());
                else if (ConfigParameters.Client == 5)
                    return (SSLConnect());
                else
                    return false;
            }
            else
                return false;
        }

        public bool TCPConnect(bool stopConnnection)
        {
            if (isIpFirstConnect == 0)
            {
                ConfigParameters.IsIPConnectFirst = true;
                isIpFirstConnect = 1;
            }
            else
            {
                ConfigParameters.IsIPConnectFirst = false;
            }

            if (ConfigParameters.IsIPConnectFirst)
            {
                stopConnnection = true;

                bool checkIPConnect = false;
                string defaultSIPIPAddress = configParameters.DefaultSIPIPAddress;
                string getSelectedIPAddress = configParameters.GetSelectedIPAddress;
                if (getSelectedIPAddress != defaultSIPIPAddress)
                {
                    EthernetDisconnect();
                    checkIPConnect = EthernetConnect(stopConnnection);
                }

                if (!checkIPConnect)
                {

                    for (int i = 0; i < configParameters.SIPIPAddressCollection.Count; i++)
                    {

                        string ipaddress = configParameters.SIPIPAddressCollection[i].IPAddress;
                        if (getSelectedIPAddress != ipaddress)
                        {
                            EthernetDisconnect();
                            configParameters.SIPIPAddressCollection[i].IsSelected = true;
                            checkIPConnect = EthernetConnect(stopConnnection);

                            if (checkIPConnect)
                            {
                                configParameters.SIPIPAddressCollection[i].IsSelected = true;
                                for (int j = i + 1; j < configParameters.SIPIPAddressCollection.Count; j++)
                                {
                                    configParameters.SIPIPAddressCollection[j].IsSelected = false;
                                }

                                break;
                            }
                            else
                            {
                                configParameters.SIPIPAddressCollection[i].IsSelected = false;

                            }
                        }
                        else
                        {
                            configParameters.SIPIPAddressCollection[i].IsSelected = false;
                        }

                    }


                    if (!checkIPConnect)
                    {
                        stopConnnection = false;
                        EthernetDisconnect();
                        checkIPConnect = EthernetConnect(stopConnnection);

                    }
                }
                configParameters.IsIPConnectFirst = false;
                configParameters.SaveConfigParameter();
                return checkIPConnect;
            }
            else
            {
                if (configParameters.IsConnectClick)
                {
                    stopConnnection = true;
                }
                else
                {
                    stopConnnection = false;
                }
                configParameters.IsConnectClick = false;
                EthernetDisconnect();
                return (EthernetConnect(stopConnnection));
            }

        }

        public bool IsConnected()
        {
            if (ConfigParameters.IsSerial)
                return (spPort != null) ? true : false;
            else if (ConfigParameters.IsTCP)
            {
                // Commented the below line as this sending isConnected as false for the first time when there is a connection issue with HeartPOS and HaertSIP
                //   return (TCPClient != null) ? !(TCPClient.Client.Poll(0, SelectMode.SelectRead) && (TCPClient.Client.Available == 0)) : false;

                // Modified Ethernet isConnected logic to check the connection by sending a data to the HeartSIP.
                try
                {
                    if (TCPClient != null && TCPClient.Client != null && TCPClient.Client.Connected)
                    {
                        /* pear to the documentation on Poll:
                         * When passing SelectMode.SelectRead as a parameter to the Poll method it will return 
                         * -either- true if Socket.Listen(Int32) has been called and a connection is pending;
                         * -or- true if data is available for reading; 
                         * -or- true if the connection has been closed, reset, or terminated; 
                         * otherwise, returns false
                         */

                        // Detect if client disconnected
                        if (TCPClient.Client.Poll(0, SelectMode.SelectRead))
                        {
                            byte[] buff = new byte[1];
                            if (TCPClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                            {
                                // Client disconnected
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            else if (ConfigParameters.IsOther)
            {
                if (ConfigParameters.Client == 3)
                    return true;
                else if (ConfigParameters.Client == 4)
                    return (HTTPClient != null) ? true : false;
                else if (ConfigParameters.Client == 5)
                    return true;
                else
                    return true;
            }
            else
                return false;
        }

        public bool IsSendReady()
        {
            if (ConfigParameters.IsSerial)
                return SerialIsSendReady();
            else if (ConfigParameters.IsTCP)
                return EthernetIsSendReady();
            else if (ConfigParameters.IsOther)
            {
                if (ConfigParameters.Client == 3)
                    return false;
                else if (ConfigParameters.Client == 4)
                    return (HTTPClient != null) ? true : false;
                else if (ConfigParameters.Client == 5)
                    return false;
                else
                    return false;
            }
            else
                return false;
        }

        public String Send(String XMLRequestMessage)
        {
            // Avoid sending empty request to HeartSIP
            string requestMessage = string.Empty;
            if (!string.IsNullOrWhiteSpace(XMLRequestMessage))
            {
                this.XMLRequestMessage = XMLRequestMessage;

                if (ConfigParameters.IsSerial)
                    requestMessage = SerialSend(XMLRequestMessage);
                else if (ConfigParameters.IsTCP)
                    requestMessage = EthernetSend(XMLRequestMessage);
                else if (ConfigParameters.IsOther)
                {
                    if (ConfigParameters.Client == 3)
                        requestMessage = null;
                    else if (ConfigParameters.Client == 4)
                        requestMessage = HTTPSend(XMLRequestMessage);
                    else if (ConfigParameters.Client == 5)
                        requestMessage = null;
                    else
                        requestMessage = null;
                }
                else
                    requestMessage = null;
            }

            return requestMessage;
        }

        public bool IsRecvReady()
        {
            if (ConfigParameters.IsSerial)
                return SerialIsRecvReady();
            else if (ConfigParameters.IsTCP)
                return EthernetIsRecvReady();
            else if (ConfigParameters.IsOther)
            {
                if (ConfigParameters.Client == 3)
                    return false;
                else if (ConfigParameters.Client == 4)
                    return false;
                else if (ConfigParameters.Client == 5)
                    return false;

                else
                    return false;
            }
            else
                return false;
        }

        public String Recv()
        {
            if (ConfigParameters.IsSerial)
                return SerialRecv();
            else if (ConfigParameters.IsTCP)
                return EthernetRecv();
            else if (ConfigParameters.IsOther)
            {
                if (ConfigParameters.Client == 3)
                    return null;
                else if (ConfigParameters.Client == 4)
                    return null;
                else if (ConfigParameters.Client == 5)
                    return HTTPRecv();
                else
                    return null;
            }
            else
                return null;
        }

        public void Disconnect()
        {
            if (ConfigParameters.IsSerial)
                SerialDisconnect();
            else if (ConfigParameters.IsTCP)
                EthernetDisconnect();
            else if (ConfigParameters.IsOther)
            {
                if (ConfigParameters.Client == 3)
                    return;
                else if (ConfigParameters.Client == 4)
                    return;
                else if (ConfigParameters.Client == 5)
                    return;
                else
                    return;
            }
            else
                return;
        }

        private bool SerialConnect()
        {
            spPort = new SerialPort(configParameters.SerialPort, int.Parse(configParameters.BaudRate), configParameters.ParityValue, configParameters.DataBits, configParameters.StopBits);
            try
            {
                spPort.Open();
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("SerialOpen UnauthorizedAccessException: {0}", e);
                return false;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("SerialOpen ArgumentOutOfRangeException: {0}", e);
                return false;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("SerialOpen ArgumentException: {0}", e);
                return false;
            }
            catch (IOException e)
            {
                Console.WriteLine("SerialOpen IOException: {0}", e);
                return false;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("SerialOpen InvalidOperationException: {0}", e);
                return false;
            }

            spPort.BaudRate = int.Parse(configParameters.BaudRate);
            spPort.DataBits = configParameters.DataBits;
            spPort.Parity = configParameters.ParityValue;
            spPort.StopBits = configParameters.StopBits;
            spPort.ReadTimeout = 60000;     //60 seconds
            return true;
        }

        private bool SerialIsSendReady()
        {
            if (spPort == null)
                return false;

            return spPort.IsOpen;
        }

        private String SerialSend(String XMLRequestMessage)
        {
            byte[] XMLBytes = Encoding.ASCII.GetBytes(XMLRequestMessage);
            byte[] SendBytes = new byte[XMLBytes.Length + 3];

            // Add STX, ETX and LRC
            XMLBytes.CopyTo(SendBytes, 1);
            SendBytes[0] = 0x02;
            SendBytes[XMLBytes.Length + 1] = 0x03;
            SendBytes[XMLBytes.Length + 2] = CalculateLRC(SendBytes);
            // Force a SIP NAK if needed
            if (SIPNAKCount < configParameters.SIPForceNAK)
            {
                SendBytes[XMLBytes.Length + 2]--;
                SIPNAKCount++;
            }
            else
            {
                SIPNAKCount = 0;
            }

            try
            {
                spPort.Write(SendBytes, 0, SendBytes.Length);
            }
            catch (ArgumentNullException e) //The buffer passed is null.
            {
                Console.WriteLine("SerialSend ArgumentNullException: {0}", e);
                return null;
            }
            catch (InvalidOperationException e) //The specified port is not open.
            {
                Console.WriteLine("SerialSend InvalidOperationException: {0}", e);
                return null;
            }
            catch (ArgumentOutOfRangeException e) //The specified port is not open.
            {
                Console.WriteLine("SerialSend ArgumentOutOfRangeException: {0}", e);
                return null;
            }
            catch (ArgumentException e) //SendBytes.Length is greater than the length of the buffer SendBytes.
            {
                Console.WriteLine("SerialSend ArgumentException: {0}", e);
                return null;
            }
            catch (TimeoutException e) //The operation did not complete before the time-out period ended.
            {
                Console.WriteLine("SerialSend TimeoutException: {0}", e);
                return null;
            }

            // Call the POS Display to display SendBytes
            posDisplay(1, SendBytes);
            return Encoding.ASCII.GetString(XMLBytes);
        }

        private bool SerialIsRecvReady()
        {
            if (spPort == null)
                return false;
            if (!spPort.IsOpen)
                return false;

            // Ignore ACK/NAK byte
            if (spPort.BytesToRead > 1)
                return true;
            else
                return false;
        }

        private enum ProtocolState
        {
            psGetACK,
            psGetSTX,
            psGetETX,
            psGetLRC,
            psSendACK,
            psSendNAK,
            psSendEOT,
            psSuccess,
            psFail
        }

        private ProtocolState pState;

        private byte STX = 0x02;
        private byte ETX = 0x03;
        private byte ACK = 0x06;
        private byte NAK = 0x15;
        //private byte EOT = 0x04;

        private String SerialRecv()
        {
            byte[] ReadBytes = new byte[12000];
            byte[] CleanBytes = new byte[12000];
            int BytesRead = 0;
            byte ReceivedLRC;
            byte CalculatedLRC;
            int TotalBytesRead = 0;
            int NAKCount = 0;

            // Start with GetACK state
            pState = ProtocolState.psGetACK;
            while (true)
            {
                switch (pState)
                {
                    case ProtocolState.psGetACK:
                        // Clear the response message
                        Array.Clear(ReadBytes, 0, ReadBytes.Length);
                        TotalBytesRead = 0;

                        // Read 1 byte
                        BytesRead = spPort.Read(ReadBytes, 0, 1);

                        if ((BytesRead == 1) && (ReadBytes[0] == ACK))
                        {
                            // Call the POS Display to display ACK
                            posDisplay(2, ReadBytes);
                            // Just eat ACK for now
                            pState = ProtocolState.psGetSTX;
                        }
                        else if ((BytesRead == 1) && (ReadBytes[0] == STX))
                        {
                            // Call the POS Display when we get the entire message after LRC
                            // We got an STX. Thats ok. Proceed to get the message
                            pState = ProtocolState.psGetETX;
                            TotalBytesRead++;
                        }
                        else if ((BytesRead == 1) && (ReadBytes[0] == NAK))
                        {
                            // Call the POS Display to display NAK
                            posDisplay(2, ReadBytes);
                            // Resend the request message
                            SerialSend(XMLRequestMessage);
                            // Got to wait for response
                            pState = ProtocolState.psGetACK;
                        }
                        else
                            pState = ProtocolState.psFail;
                        break;

                    case ProtocolState.psGetSTX:
                        // ReadBytes contains STX, message, ETX and LRC
                        // Read STX
                        BytesRead = spPort.Read(ReadBytes, 0, 1);

                        TotalBytesRead++;
                        if ((BytesRead == 1) && (ReadBytes[0] == STX))
                            pState = ProtocolState.psGetETX;
                        else
                            pState = ProtocolState.psFail;
                        break;

                    case ProtocolState.psGetETX:
                        // Read until ETX
                        while ((BytesRead == 1) && (ReadBytes[TotalBytesRead] != ETX))
                        {
                            BytesRead = spPort.Read(ReadBytes, TotalBytesRead++, 1);
                            if (ReadBytes[TotalBytesRead - 1] == ETX)
                            {
                                pState = ProtocolState.psGetLRC;
                                break;
                            }
                        }

                        if (pState != ProtocolState.psGetLRC)
                            pState = ProtocolState.psFail;
                        break;

                    case ProtocolState.psGetLRC:
                        // Get LRC
                        BytesRead = spPort.Read(ReadBytes, TotalBytesRead++, 1);

                        ReceivedLRC = ReadBytes[TotalBytesRead - 1];
                        // Strip STX, LRC; Leave ETX
                        Array.Clear(CleanBytes, 0, CleanBytes.Length);
                        Array.Copy(ReadBytes, 1, CleanBytes, 0, TotalBytesRead - 2);
                        // Calculate LRC and compare
                        CalculatedLRC = CalculateLRC(CleanBytes);
                        // Force a POS NAK if needed
                        if (POSNAKCount < configParameters.POSForceNAK)
                        {
                            CalculatedLRC--;
                            POSNAKCount++;
                        }
                        else
                        {
                            POSNAKCount = 0;
                        }

                        // Call the POS Display to display received message
                        posDisplay(2, ReadBytes);
                        if (ReceivedLRC == CalculatedLRC)
                            pState = ProtocolState.psSendACK;
                        else
                            pState = ProtocolState.psSendNAK;
                        break;

                    case ProtocolState.psSendACK:
                        SerialSendACK();
                        pState = ProtocolState.psSuccess;
                        break;

                    case ProtocolState.psSendNAK:
                        if (NAKCount >= 3)
                            pState = ProtocolState.psFail;
                        else
                        {
                            SerialSendNAK();
                            NAKCount++;
                            pState = ProtocolState.psGetACK;
                        }
                        break;

                    case ProtocolState.psSuccess:
                        // Strip STX, ETX, LRC
                        // CleanBytes contains just the message without protocol chars
                        Array.Clear(CleanBytes, 0, CleanBytes.Length);
                        Array.Copy(ReadBytes, 1, CleanBytes, 0, TotalBytesRead - 3);
                        return Encoding.ASCII.GetString(CleanBytes, 0, TotalBytesRead - 3);

                    case ProtocolState.psFail:
                        return "";

                    default:
                        pState = ProtocolState.psFail;
                        break;
                }
            }
        }

        private void SerialDisconnect()
        {
            if (spPort != null)
            {
                spPort.Close();
                spPort.Dispose();
            }
        }

        private void SerialSendNAK()
        {
            byte[] Buffer = { 0x15, 0x00 };

            spPort.Write(Buffer, 0, 1);
            // Call the POS Display to display SendBytes
            posDisplay(1, Buffer);
        }

        private void SerialSendACK()
        {
            byte[] Buffer = { 0x06 };

            spPort.Write(Buffer, 0, 1);
            // Call the POS Display to display SendBytes
            posDisplay(1, Buffer);
        }

        private byte CalculateLRC(byte[] bytes)
        {
            byte LRC = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                LRC ^= bytes[i];
            }
            return LRC;
        }

        public bool GetAck()
        {

            byte[] ReadBytes = new byte[12000];
            byte[] CleanBytes = new byte[12000];
            int BytesRead = 0;
            byte ACK = 0x06;
            byte NAK = 0x15;

            // Clear the response message
            Array.Clear(ReadBytes, 0, ReadBytes.Length);
            try
            {
                BytesRead = spPort.Read(ReadBytes, 0, 1);
                if ((BytesRead == 1) && (ReadBytes[0] == ACK))
                {
                    spPort.BaseStream.Flush();
                    // Call the POS Display to display SendBytes
                    posDisplay(2, ReadBytes);
                    return true;
                }
                else if ((BytesRead == 1) && (ReadBytes[0] == NAK))
                {

                    spPort.BaseStream.Flush();
                    // Call the POS Display to display SendBytes
                    posDisplay(2, ReadBytes);
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        private bool EthernetConnect(bool stopConnnection = false)
        {
            if (TCPClient == null)
            {
                var connected = false;
                var count = 0;
                while (!connected)
                {
                    try
                    {
                        if (displayConnectionType != null)
                        {
                            displayConnectionType();
                        }
                        TCPClient = new TcpClient();
                        TCPClient.Connect(configParameters.GetSelectedIPAddress, Int32.Parse(configParameters.IPPort));
                        connected = TCPClient.Connected;
                    }
                    catch (SocketException e)
                    {
                        if (count < 3)
                        {
                            var excpetionBytes = Encoding.ASCII.GetBytes(String.Format("Connection attempt {0} failed {1} {2}", ++count, configParameters.GetSelectedIPAddress, e.ErrorCode));
                            posDisplay(3, excpetionBytes);
                        }
                        else if (stopConnnection)
                        {
                            break;
                        }
                    }

                    Thread.Sleep(500);
                }
            }
            if (tcpStream == null)
            {
                try
                {
                    tcpStream = TCPClient.GetStream();
                    tcpStream.ReadTimeout = 60000;      //60 seconds
                }
                catch (InvalidOperationException e) //The TcpClient is not connected to a remote host.
                {
                    Console.WriteLine("EthernetConnect GetStream InvalidOperationException: {0}", e);
                    return false;
                }
            }
            return true;
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        private bool EthernetIsSendReady()
        {
            if (TCPClient == null)
                return false;

            return TCPClient.Connected;
        }

        private String EthernetSend(String XMLRequestMessage)
        {
            byte[] XMLBytes = Encoding.ASCII.GetBytes(XMLRequestMessage);
            byte[] SendBytes = new byte[XMLBytes.Length + 2];
            byte[] MsgLength = BitConverter.GetBytes(XMLBytes.Length);

            // Add <SendBytes.Length> to message
            SendBytes[0] = MsgLength[1];
            SendBytes[1] = MsgLength[0];
            XMLBytes.CopyTo(SendBytes, 2);

            try
            {
                tcpStream.Write(SendBytes, 0, SendBytes.Length);
            }
            catch (IOException e)
            {
                Console.WriteLine("EthernetSend IOException: {0}", e);
                return null;
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine("EthernetSend ObjectDisposedException: {0}", e);
                return null;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("EthernetSend NullReferenceException: {0}", e);
                return null;
            }
            try
            {
                tcpStream.Flush();
            }
            catch (IOException e)
            {
                Console.WriteLine("EthernetFlush ObjectDisposedException: {0}", e);
                return null;
            }

            // Call the POS Display to display SendBytes
            posDisplay(1, XMLBytes);
            return Encoding.ASCII.GetString(XMLBytes);
        }

        private bool EthernetIsRecvReady()
        {
            //if (tcpStream == null)
            //    return false;

            return tcpStream == null ? false : tcpStream.DataAvailable;
        }

        private bool EthernetReadHeader()
        {
            byte[] ReadBytes = new byte[4];
            int BytesRead = 0;

            if (tcpStream == null)
                return false;

            try
            {
                BytesRead = tcpStream.Read(ReadBytes, 0, 2);
            }
            catch (IOException e)
            {
                Console.WriteLine("EthernetIsReadReady IOException: {0}", e);
                return false;
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine("EthernetIsReadReady ObjectDisposedException: {0}", e);
                return false;
            }

            if (BytesRead != 2)
                return false;

            IPRecvPacketSize = 0;
            ReadBytes[2] = ReadBytes[1];
            ReadBytes[1] = 0x00;
            ReadBytes[3] = 0x00;
            IPRecvPacketSize = (BitConverter.ToInt16(ReadBytes, 0) * 256) + BitConverter.ToInt16(ReadBytes, 2);

            return true;
        }

        private String EthernetRecv()
        {
            byte[] ReadBytes = new byte[9000];
            byte[] CleanBytes = new byte[9000];
            int BytesRead = 0;
            int BytesToRead = 0;
            int Offset;
            bool ReadFlag;

            EthernetReadHeader();
            BytesToRead = IPRecvPacketSize;

            if (tcpStream == null)
                return "";

            Offset = 0;
            ReadFlag = true;
            do
            {
                try
                {
                    // Fixed issue HeartPOS keeps waiting for the response from HeartSIP which is offline
                    if (IsConnected())
                        BytesRead = tcpStream.Read(ReadBytes, Offset, BytesToRead);
                    else
                        throw new Exception("Problem in getting the response from HeartSIP, Connection might be terminated!");  // Fixed issue HeartPOS keeps waiting for the response from HeartSIP which is offline - Santhana Krishnan
                }
                catch (IOException e)
                {
                    Console.WriteLine("EthernetIsReadReady IOException: {0}", e);
                    return "";
                }
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine("EthernetIsReadReady ObjectDisposedException: {0}", e);
                    return "";
                }
                // Fixed issue HeartPOS keeps waiting for the response from HeartSIP which is offline
                catch (Exception e)
                {
                    Console.WriteLine(e.Message, e);
                    return "";
                }

                if (BytesRead != BytesToRead)
                {
                    Offset = Offset + BytesRead;
                    BytesToRead = BytesToRead - BytesRead;
                }
                else
                    ReadFlag = false;
            } while (ReadFlag);

            posDisplay(2, ReadBytes);
            Array.Clear(CleanBytes, 0, CleanBytes.Length);
            Array.Copy(ReadBytes, 0, CleanBytes, 0, ReadBytes.Length);
            return Encoding.ASCII.GetString(CleanBytes, 0, IPRecvPacketSize);
        }

        private void EthernetDisconnect()
        {
            if (tcpStream != null)
            {
                tcpStream.Close();
                tcpStream = null;
            }
            if (TCPClient != null)
            {
                TCPClient.Close();
                TCPClient = null;
            }
        }

        private bool SSLConnect()
        {
            X509Certificate2 ca_x509 = new X509Certificate2();
            //Create X509Certificate2 object from .cer file.
            //FileStream f = new FileStream(@"C:\Users\sachin.desai\workspace\project\work\svn\svn_wrk\POS_Apps\Spectrum\v3.3\DEV\Software\bld\telium\Resources\Misc\CA_CERT.PEM", FileMode.Open, FileAccess.Read);
            if(!File.Exists(CACertificate))
            {
                return false;
            }
            FileStream f = new FileStream(CACertificate, FileMode.Open, FileAccess.Read);
            int size = (int)f.Length;
            byte[] data = new byte[size];
            size = f.Read(data, 0, size);
            f.Close();

            ca_x509.Import(data);

            //Print to console information contained in the certificate.
            Console.WriteLine("{0}Subject: {1}{0}", Environment.NewLine, ca_x509.Subject);
            Console.WriteLine("{0}Issuer: {1}{0}", Environment.NewLine, ca_x509.Issuer);
            Console.WriteLine("{0}Version: {1}{0}", Environment.NewLine, ca_x509.Version);
            Console.WriteLine("{0}Valid Date: {1}{0}", Environment.NewLine, ca_x509.NotBefore);
            Console.WriteLine("{0}Expiry Date: {1}{0}", Environment.NewLine, ca_x509.NotAfter);
            Console.WriteLine("{0}Thumbprint: {1}{0}", Environment.NewLine, ca_x509.Thumbprint);
            Console.WriteLine("{0}Serial Number: {1}{0}", Environment.NewLine, ca_x509.SerialNumber);
            Console.WriteLine("{0}Friendly Name: {1}{0}", Environment.NewLine, ca_x509.PublicKey.Oid.FriendlyName);
            Console.WriteLine("{0}Public Key Format: {1}{0}", Environment.NewLine, ca_x509.PublicKey.EncodedKeyValue.Format(true));
            Console.WriteLine("{0}Raw Data Length: {1}{0}", Environment.NewLine, ca_x509.RawData.Length);
            Console.WriteLine("{0}Certificate to string: {1}{0}", Environment.NewLine, ca_x509.ToString(true));
            Console.WriteLine("{0}Certificate to XML String: {1}{0}", Environment.NewLine, ca_x509.PublicKey.Key.ToXmlString(false));

            //Add the certificate to a X509Store.
            X509Store store = new X509Store();
            store.Open(OpenFlags.MaxAllowed);
            store.Add(ca_x509);
            store.Close();

            X509Certificate2 server_x509 = new X509Certificate2();
            //Create X509Certificate2 object from .cer file.
            //f = new FileStream(@"C:\Users\sachin.desai\workspace\project\work\svn\svn_wrk\POS_Apps\Spectrum\v3.3\DEV\Software\bld\telium\Resources\Misc\SIP_CERT.PEM", FileMode.Open, FileAccess.Read);
            f = new FileStream(SIPCertificate, FileMode.Open, FileAccess.Read);
            size = (int)f.Length;
            data = new byte[size];
            size = f.Read(data, 0, size);
            f.Close();

            server_x509.Import(data);

            //Print to console information contained in the certificate.
            Console.WriteLine("{0}Subject: {1}{0}", Environment.NewLine, server_x509.Subject);
            Console.WriteLine("{0}Issuer: {1}{0}", Environment.NewLine, server_x509.Issuer);
            Console.WriteLine("{0}Version: {1}{0}", Environment.NewLine, server_x509.Version);
            Console.WriteLine("{0}Valid Date: {1}{0}", Environment.NewLine, server_x509.NotBefore);
            Console.WriteLine("{0}Expiry Date: {1}{0}", Environment.NewLine, server_x509.NotAfter);
            Console.WriteLine("{0}Thumbprint: {1}{0}", Environment.NewLine, server_x509.Thumbprint);
            Console.WriteLine("{0}Serial Number: {1}{0}", Environment.NewLine, server_x509.SerialNumber);
            Console.WriteLine("{0}Friendly Name: {1}{0}", Environment.NewLine, server_x509.PublicKey.Oid.FriendlyName);
            Console.WriteLine("{0}Public Key Format: {1}{0}", Environment.NewLine, server_x509.PublicKey.EncodedKeyValue.Format(true));
            Console.WriteLine("{0}Raw Data Length: {1}{0}", Environment.NewLine, server_x509.RawData.Length);
            Console.WriteLine("{0}Certificate to string: {1}{0}", Environment.NewLine, server_x509.ToString(true));
            Console.WriteLine("{0}Certificate to XML String: {1}{0}", Environment.NewLine, server_x509.PublicKey.Key.ToXmlString(false));

            //Add the certificate to a X509Store.
            store.Open(OpenFlags.MaxAllowed);
            store.Add(server_x509);
            store.Close();

            if (SSLClient == null)
            {
                IPAddress SSLServerIPAddress = IPAddress.Parse(configParameters.GetSelectedIPAddress);
                Int32 SSLServerPort = Int32.Parse(configParameters.IPPort);
                SSLClient = new TcpClient("www.HeartSIP.com", SSLServerPort);
            }
            try
            {
                IPAddress SSLServerIPAddress = IPAddress.Parse(configParameters.GetSelectedIPAddress);
                Int32 SSLServerPort = Int32.Parse(configParameters.IPPort);
                SSLClient.Connect(SSLServerIPAddress, SSLServerPort);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SSLConnect GetStream InvalidOperationException: {0}", e);
                return false;
            }
            try
            {
                sslStream = new SslStream(SSLClient.GetStream(), true, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            }
            catch (InvalidOperationException e) //The TcpClient is not connected to a remote host.
            {
                Console.WriteLine("SSLConnect GetStream InvalidOperationException: {0}", e);
                return false;
            }

            if (sslStream != null)
            {
                // The server name must match the name on the server certificate.
                try
                {
                    //X509CertificateCollection clientCertificates = new X509CertificateCollection;
                    //sslStream.AuthenticateAsClient("HeartSIP", clientCertificates, SslProtocols.Tls12, false);
                    sslStream.AuthenticateAsClient("HeartSIP");
                }
                catch (AuthenticationException e)
                {
                    Console.WriteLine("SSLConnect AuthenticateAsClient AuthenticationException: {0}", e);
                    return false;
                }
                return true;
            }
            else
                return false;
        }

        private bool SSLIsSendReady()
        {
            return true;
        }

        private String SSLSend(String XMLRequestMessage)
        {
            byte[] XMLBytes = Encoding.ASCII.GetBytes(XMLRequestMessage);
            byte[] SendBytes = new byte[XMLBytes.Length + 2];
            byte[] MsgLength = BitConverter.GetBytes(XMLBytes.Length);

            // Add <SendBytes.Length> to message
            SendBytes[0] = MsgLength[1];
            SendBytes[1] = MsgLength[0];
            XMLBytes.CopyTo(SendBytes, 2);

            try
            {
                sslStream.Write(SendBytes, 0, SendBytes.Length);
            }
            catch (IOException e)
            {
                Console.WriteLine("EthernetSend IOException: {0}", e);
                return null;
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine("EthernetSend ObjectDisposedException: {0}", e);
                return null;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("EthernetSend NullReferenceException: {0}", e);
                return null;
            }
            try
            {
                sslStream.Flush();
            }
            catch (IOException e)
            {
                Console.WriteLine("EthernetFlush ObjectDisposedException: {0}", e);
                return null;
            }

            // Call the POS Display to display SendBytes
            posDisplay(1, XMLBytes);
            return Encoding.ASCII.GetString(XMLBytes);
        }

        private bool SSLIsRecvReady()
        {
            return true;
        }

        private String SSLRecv()
        {
            byte[] ReadBytes = new byte[9000];
            byte[] CleanBytes = new byte[9000];
            int BytesRead = 0;
            int BytesToRead = 0;
            int Offset;
            bool ReadFlag;

            if (sslStream == null)
                return null;

            Offset = 0;
            ReadFlag = true;
            do
            {
                try
                {
                    // Fixed issue HeartPOS keeps waiting for the response from HeartSIP which is offline
                    if (IsConnected())
                        BytesRead = sslStream.Read(ReadBytes, Offset, BytesToRead);
                    else
                        throw new Exception("Problem in getting the response from HeartSIP, Connection might be terminated!");  // Fixed issue HeartPOS keeps waiting for the response from HeartSIP which is offline - Santhana Krishnan
                }
                catch (IOException e)
                {
                    Console.WriteLine("EthernetIsReadReady IOException: {0}", e);
                    return null;
                }
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine("EthernetIsReadReady ObjectDisposedException: {0}", e);
                    return null;
                }
                // Fixed issue HeartPOS keeps waiting for the response from HeartSIP which is offline
                catch (Exception e)
                {
                    Console.WriteLine(e.Message, e);
                    return null;
                }

                if (BytesRead != BytesToRead)
                {
                    Offset = Offset + BytesRead;
                    BytesToRead = BytesToRead - BytesRead;
                }
                else
                    ReadFlag = false;
            } while (ReadFlag);

            posDisplay(2, ReadBytes);
            Array.Clear(CleanBytes, 0, CleanBytes.Length);
            Array.Copy(ReadBytes, 0, CleanBytes, 0, ReadBytes.Length);
            return Encoding.ASCII.GetString(CleanBytes, 0, IPRecvPacketSize);
        }

        private void SSLDisconnect()
        {
            return;
        }

        private bool BluetoothConnect()
        {
            //BluetoothClient bc = new BluetoothClient();
            //BluetoothDeviceInfo[] devices = bc.DiscoverDevices(8);
            return true;
        }

        private bool BluetoothIsSendReady()
        {
            return true;
        }

        public String BluetoothSend(String XMLRequestMessage)
        {
            // Avoid sending empty request to HeartSIP
            string requestMessage = string.Empty;

            return requestMessage;
        }

        public bool BluetoothIsRecvReady()
        {
            return true;
        }

        public String BluetoothRecv()
        {
            // Avoid sending empty request to HeartSIP
            string responseMessage = string.Empty;

            return responseMessage;
        }

        private void BluetoothDisconnect()
        {
            return;
        }

        private bool HTTPConnect()
        {
            HTTPClient = new HttpClient();
            HTTPClient.BaseAddress = new Uri("http://localhost/");
            HTTPClient.DefaultRequestHeaders.Host = "127.0.0.1";
            return true;
        }

        private bool HTTPIsSendReady()
        {
            return true;
        }

        public String HTTPSend(String XMLRequestMessage)
        {
            // Avoid sending empty request to HeartSIP
            string requestMessage = string.Empty;
            var responseMessage = HTTPClient.PostAsync("Echo", new StringContent(XMLRequestMessage)).Result;

            return requestMessage;
        }

        public bool HTTPIsRecvReady()
        {
            return true;
        }

        public String HTTPRecv()
        {
            // Avoid sending empty request to HeartSIP
            string responseMessage = string.Empty;

            return responseMessage;
        }

        private void HTTPDisconnect()
        {
            return;
        }

    }
}