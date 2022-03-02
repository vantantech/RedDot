using HeartPOS;
using HPS;
using NLog;
using RedDotBase;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace RedDot
{
    public partial class HeartPOS:INPCBase
    {
      
        public HeartSIP iSC250;
        public Thread SIPRecvThread;

        String SIPCommand;
        bool SIPSent;
        bool SIPReceving;
        bool SIPReset;
        string LastResponse;

        String SIPRequestMessage;
        String SIPResponseMessage;
        public XMLRequestTagValues xmlRequestTagValues;

      
        Int32 MaxFileSizeBuffer;
        public Int32 ServerCount;
        System.Timers.Timer timerReset = new System.Timers.Timer();
        System.Timers.Timer timerThreadRestart = new System.Timers.Timer();
 
        public string LastCommand { get; set; }

        //PrePostForms prePostForm;


        public bool SIPTransactionStarted = false;
        public bool SIPIsLaneOpen = false;
        // Added new parameter to check whether the start card is active
        public bool isStartCardActive = false;
        public bool isStartCardInitiatedTrans = false;
      
        // Set this variable to true when AllCard is set to true.
        private bool isAllCardOptionSelected = false;
        private bool isSipSendFileSuccess = false;
        private DateTime currentDateTime;
        private bool isVoiceAuthEnabled = true;

        public ConfigParameters configParameters { get; set; }

     

        long TotalBytesToSend;
        long BytesRemainingToSend;
        bool bSendFileFirstMessage;
        StringReader srHex;

        const int mtSend = 1;
        const int mtRecv = 2;
        const int mtOther = 3;

  
        public delegate void SalesResponse(ResponseEventArgs response);
        public delegate void VoidResponse(VoidResponseArgs response);
        public delegate void RefundResponse(ResponseEventArgs response);
        public delegate void GiftResponse(GiftResponseArgs response);
        public delegate void MessageResponse(int msgtype, string message, string command);
        public delegate void TipAdjustResponse(int paymentid, string tipamount, string totalamount);
        public delegate void GetDataResponse(string requestid, string trackdata);
        public delegate void AllResponse(string response);


        public delegate void CloseForm();


        public SalesResponse salesResponse;
        public RefundResponse refundResponse;
        public VoidResponse voidResponse;
        public GiftResponse giftResponse;
        public TipAdjustResponse tipadjustResponse;
        public GetDataResponse getdataResponse;
        public AllResponse allResponse;
       
        public MessageResponse messageResponse;
        public CloseForm closeform;

        private bool ExitThread = false;

        public string TipRequestId { get; set; }
        public int TipPaymentId { get; set; }

        public string OriginalTransactionId { get; set; }
     
        public string TrackData { get; set; }
        //public event EventHandler<ResponseEventArgs> SalesResponse;
     

        public HeartPOS( MessageResponse messageresponse, SalesResponse salesResponse, VoidResponse voidResponse, RefundResponse refundResponse, GiftResponse giftResponse, TipAdjustResponse tipadjustResponse, GetDataResponse getdataresponse, AllResponse allresponse)
        {

           

            InitializeConfigData();

            xmlRequestTagValues.ConfirmAmount = configParameters.ConfirmAmount ? "1" : "0";


            this.salesResponse = salesResponse;
            this.voidResponse = voidResponse;
            this.refundResponse = refundResponse;
            this.messageResponse = messageresponse;
            this.giftResponse = giftResponse;
            this.tipadjustResponse = tipadjustResponse;
            this.getdataResponse = getdataresponse;
            this.allResponse = allresponse;
       

            // Start with Any Card selection
            isAllCardOptionSelected = true;
            xmlRequestTagValues.CardGroup = GlobalSettings.Instance.CreditCardChoices;

            // Init SIP connection settings
            timerReset.Elapsed += new ElapsedEventHandler(OnTimedEventReset);
            timerThreadRestart.Elapsed += new ElapsedEventHandler(RestartThread);
      

         

            // Create SIP
            iSC250 = new HeartSIP(configParameters, DisplayDebugText, null);

            // Start the SIP send and recv in a thread so it does not block the UI/Main thread
            // Any loops or waiting in the UI thread / windows call backs will block the UI/Main thread
            SIPRecvThread = new Thread(() => SIPProcessInThread());
            SIPRecvThread.IsBackground = true;
         

        }


        public void StartThread()
        {
            SIPRecvThread.Start();
        }


        private void RestartThread(object source, ElapsedEventArgs e)
        {
            timerThreadRestart.Enabled = false;
            SIPRecvThread = new Thread(() => SIPProcessInThread());
            SIPRecvThread.IsBackground = true;
            SIPRecvThread.Start();
        }

       /* protected virtual void OnSalesResponse(ResponseEventArgs e)
        {
            if (SalesResponse != null) SalesResponse(this, e);
        }*/


  
        public String RequestId { get; set; }
     

        private void SIPProcessInThread()
        {
            bool SIPSendMore = false;
            bool SIPResultGood = false;
            SIPTransactionStarted = false;
            string command = string.Empty;
            // Read the configuration for persistent Connection
            var IsPersistentConnection = ConfigurationManager.AppSettings["IsPersistentConnection"] == null ? false : ConfigurationManager.AppSettings["IsPersistentConnection"].ToString().ToLower() == "true";
            string debugtrack = "";

            try
            {
                DisplayDebugText(mtOther, "SIP Thread started...");


                // Keep Sending and Receiving... for the life of the app.
            while (true)
            {
                    //sends heartbeat
                    debugtrack = "heartbeat";
                    HeartBeat();


                    if (ExitThread) break;


                    // This will start teh timer for transaction reset
                    if (SIPTransactionStarted && !SIPReset && SIPIsLaneOpen)
                    {
                        if (String.IsNullOrEmpty(SIPCommand))
                        {
                            SIPTransactionStarted = false;


                            if (configParameters.ResetDelay >= 0)
                            {
                                debugtrack = "timerReset";
                                timerReset.AutoReset = false;
                                timerReset.Interval = configParameters.ResetDelay == 0 ? 1 : configParameters.ResetDelay * 1000;
                                timerReset.Enabled = true;
                            }
                        }
                    }


           

                // Connect to SIP if not connected already. It will take care of both Ethernet and Serial
                // It will also reconnect automatically if the other side ( HeartSIP) is restarted !
                if (!SIPIsConnected())
                {
                    debugtrack = "reconnect";
                    SIPReconnect();
                }

                // If there is no Command set, nothing to send
                if ((SIPCommand == null) || (SIPCommand == ""))
                {
                    if (iSC250.IsRecvReady())
                    {
                        debugtrack = "receive";
                        SIPReceiveCommands();
                    }

                    continue;
                }
                command = SIPCommand;

               

                // Custom forms clears the ServerCount, tax and tip amount in the request, So saving the request and sending it after sending the custom forms


               // XMLRequestTagValues xmlTagValues = xmlRequestTagValues;
                //var serverCount = ServerCount;


                // Process the preform
                // ProcessPrePostForms(command, FormType.PreForm, true);

               // xmlRequestTagValues = xmlTagValues;
               // ServerCount = serverCount;






                do
                {


                    // Will send one command. And set the SIPCommand variable for the next send (if any) - for example SendFile, Pre/Post forms
                    SIPSent = false;
                    SIPSendMore = SIPSendCommand();
                    // Recv only if sent something
                    // Will recv multiple commands for one single send command and dump on the virtual receipt

                    if (xmlRequestTagValues.MultipleMessage == "1" && SIPCommand == SendFileCommand && xmlRequestTagValues.FileName == null)
                    {
                        if (!configParameters.IsTCP)
                        {
                            while (true) //wait for ack
                            {
                                Thread.Sleep(50);
                                if (iSC250.spPort.BytesToRead > 0)
                                {
                                    getAck();
                                    SIPSendMore = true;
                                    break;
                                }
                            }
                            continue;
                        }
                        else
                        {
                            SIPSendMore = true;
                            continue;
                        }
                    }

                    if (SIPSent)
                        SIPResultGood = SIPReceiveCommands();

                    // Set startcard flag as false when heartsip response fails (scenario: initiating start card after line item) - fix for DE24594
                    if (SIPCommand == StartCardCommand && !SIPResultGood)
                        isStartCardActive = false;

                } while (SIPSendMore && SIPResultGood);

                if (SIPResultGood)
                {
                    // Process the Post Form
                    // ProcessPrePostForms(command, formtype: FormType.PostForm);
                }

                // Disconnects from SIP if the persistent connection is set to false. It will take care of both Ethernet and Serial
                if (!IsPersistentConnection)
                {
                    SIPDisconnect();
                }
                // Commented the below method as there already a condition to get the SIPResponse even if the command is null or empty
                // SIPReceiveCommandsNoWait();



                SIPClearCommand();


            }

            }catch(Exception ex)
            {
                TouchMessageBox.Show("Thread error:" + ex.Message + ":" + debugtrack);
                logger.Error("Thread error:" + ex.Message + ":" + debugtrack);
                timerThreadRestart.AutoReset = false;
                timerThreadRestart.Interval = 1000;
                timerThreadRestart.Enabled = true;
            }
        }

        public bool SIPConnect(bool stopConnnection = false)
        {
            bool Result = false;

            // Connect if necessary. We stay connected if Serial.
            if (!iSC250.IsSendReady())
            {
                 DisplayDebugText(mtOther, "POS Connecting to SIP...");
                Result = iSC250.Connect(stopConnnection);
                 if (Result)   DisplayDebugText(mtOther, "POS Connected to SIP :)");
            }
            else
                Result = true;

            if (!Result)
            {
                 DisplayDebugText(mtOther, "POS Connection to SIP failed :(");
                DisplayDebugText(mtOther, "Check your App.config and SIPListener settings.");
                return false;
            }

            return true;
        }
        public bool SIPReconnect()
        {

            SIPReset = false;
            timerReset.Enabled = false;
            // Disconnect 'properly' before reconnecting !
            SIPDisconnect();
            var isConnected = SIPConnect();
            if (isConnected)
            {
                // Send a LaneClose command at start
                SIPSetCommand(LaneCloseCommand);
                timerReset.Enabled = true;
            }
            else
            {
                SIPClearCommand();
            }


            return isConnected;
        }

        public bool SIPDisconnect(bool isOverride = false)
        {
            // Ethernet - Disconnect
            // Serial - Stay alive
            if (!configParameters.IsSerial || isOverride)
            {

                iSC250.Disconnect();

            }

            return true;
        }

        public void Dispose()
        {
            //stops thread but do not user Abort();
            ExitThread = true;  //sets flag so while loop can break out

            //Disconnect if Serial or Ethernet
            iSC250.Disconnect();
        }

        public bool SIPClearCommand()
        {
            SIPCommand = "";



            return true;
        }


        private void OnTimedEventReset(object source, ElapsedEventArgs e)
        {
            // Do not rest until the start card transaction is active
            if (!isStartCardActive && LastResponse != "Notification")
            {
                
                Abort();
            }
        }


        private void Abort()
        {
            isStartCardActive = false;
            isStartCardInitiatedTrans = false;

            // Reset the start card active when Abort/Sale/Refund is done - Added for defect DE24593
            if (SIPReceving || SIPCommand == SendFileCommand)
                SIPReset = true;
            else
            {
                RequestId = Utility.RandomPin(7).ToString();
                SIPSetCommand(ResetCommand);
            }
        }
 

        private bool ProcessThisResponse()
        {
            if (!XMLIsValidResponse())
                return false;

         //used to set training button

            return true;
        }


        public bool SIPSetCommand(String sCommand)
        {
            // Set the command so the SIP thread can pick it up and send/recv.
            SIPCommand = sCommand;
            return true;
        }

        private void InitializeConfigData()
        {
            XmlDocument configData = new XmlDocument();
    
     
            string configFilePath = ConfigurationManager.AppSettings["XMLPath"] + "UserConfig.xml";


            if (File.Exists(configFilePath))
            {
                configParameters = LoadConfigData(configFilePath);
            }else
            {
                TouchMessageBox.Show("XML Configuration file missing.");
            }


        }

        public ConfigParameters LoadConfigData(string configFilePath)
        {
            try
            {
                XmlDocument configData = new XmlDocument();
                ConfigParameters configParameters = null;
                configData.Load(configFilePath);
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigParameters));
                using (StringReader read = new StringReader(configData.OuterXml))
                {
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        configParameters = (ConfigParameters)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }

                configParameters.DefaultSIPIPAddress = GlobalSettings.Instance.SIPDefaultIPAddress;
                configParameters.IPPort = "12345";


                return configParameters;
            }
            catch(Exception ex)
            {
                TouchMessageBox.Show("XML Config load:" + ex.Message);
                return null;
            }
         



        }

        private bool SIPSendCommand()
        {
            // If there is no Command set, nothing to send
            if ((SIPCommand == null) || (SIPCommand == ""))
                return false;

            xmlRequestTagValues.RequestId = RequestId;

            SIPSend(SIPCommand);
            return false;

        }

        private bool SIPSend(String SIPCommand)
        {
            // Stop any ResetDelay timer
            timerReset.Stop();
            SIPRequestMessage = null;
            SIPResponseMessage = null;

      


            // Pack the XML request message
            xmlRequestTagValues.ECRId = configParameters.ECRId;
            SIPRequestMessage = XMLPack(SIPCommand, xmlRequestTagValues);
   
            ServerCount = 0;


            if (xmlRequestTagValues.MultipleMessage == "1" && SIPCommand == SendFileCommand && xmlRequestTagValues.FileName == null)
            {
                //SKIP & DO NOTHING
            }
            // Print the header etc.
            else
              
                    PrintReceipt();
            

           


            // Dont send the empty request to HeartSIP
            if (!string.IsNullOrWhiteSpace(SIPRequestMessage))
            {
                SIPRequestMessage = iSC250.Send(SIPRequestMessage);
                SIPSent = true;
            }
            else
            {
                SIPSent = false;
            }



            return SIPSent;
        }
   
        private bool SIPReceiveCommands()
        {
            bool SIPIsLastResponseMessage;
            bool SIPResultGood = false;
            bool SIPAborted = false;
            bool SIPResetProgress = false;
            isSipSendFileSuccess = true;

            // You may receive multiple messages from SIP for one command, like for reports
            SIPIsLastResponseMessage = false;

            try
            {
                while (!SIPIsLastResponseMessage)
                {
                    SIPReceving = true;
                    // Check if we have a reset from POS - we need to abort.
                    if (SIPReset)
                    {
                        // Clear reset
                        SIPReset = false;
                        SIPResetProgress = true;
                        // Send the reset
                        SIPSend(ResetCommand);
                    }

                    // Wait until we have something from SIP
                    if (!iSC250.IsRecvReady() && SIPIsConnected())
                        continue;

                    // Receive it
                    if (!SIPReceive())
                        return false;

                    // Stop receiving if its the last message from SIP. SIP will indicate.
                    if (!XMLIsResponseMultiMessage())
                    {
                        // In startcard, when notification is received, then set last response message to true, to exit the loop
                        // If not start card, if any transaction is initated then force the HeartPOS to wait for the response from the HeartSIP
                        if (String.Equals(XMLGetTagValueInResponse("Response"), "notification", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (isStartCardActive || SIPResetProgress)
                                SIPIsLastResponseMessage = true;
                            else
                                SIPIsLastResponseMessage = !SIPTransactionStarted;
                        }
                        else if (String.Equals(XMLGetTagValueInResponse("Response"), "reset", StringComparison.CurrentCultureIgnoreCase))
                        {
                            // When reset response is received then,
                            // Transaction is not started -> then set last response message to true, to exit the loop
                            // Transaction is started -> then wait for the HeartSIP to send the Sale Response to exit the loop
                            // Transaction is started and Abort is pressed again -> then set last response message to true, to exit the loop 
                            if (SIPTransactionStarted)
                            {
                                // SIPResetProgress -> indicates HeartPOS is processing the Reset command
                                // SIPAborted -> indicates HeartPOS has processed the Reset Command, 
                                // This varaible used to Reset HeartPOS when its not receiving transactional Response like Sale or Refund (to avoid heartPOS waiting indefinitely)
                                if (SIPResetProgress)
                                {
                                    if (SIPAborted)
                                    {
                                        SIPIsLastResponseMessage = true;
                                    }
                                    else
                                    {
                                        SIPIsLastResponseMessage = false;
                                        SIPAborted = true;
                                    }
                                }
                                else
                                {
                                    SIPIsLastResponseMessage = true;
                                    SIPAborted = false;
                                }
                            }
                            else
                            {
                                SIPIsLastResponseMessage = true;
                                SIPAborted = false;
                            }

                        }
                        else if (String.Equals(XMLGetTagValueInResponse("Response"), "SendFile", StringComparison.CurrentCultureIgnoreCase) && int.Parse(XMLGetTagValueInResponse("Result")) > 0)
                        {
                            isSipSendFileSuccess = false;
                            SIPIsLastResponseMessage = true;
                        }
                        else
                        {
                            SIPIsLastResponseMessage = true;
                            SIPAborted = false;
                        }
                    }
                }

                SIPReceving = false;

                if (!SIPAborted)
                    SIPResultGood = true;

                // Process the SIP response if valid
                if (XMLIsValidResponse())
                {
                    // Set a timer for ResetDelay, if a transaction
                    if (XMLIsTransaction())
                    {
                        if (configParameters.ResetDelay >= 0)
                        {
                            timerReset.AutoReset = false;
                            timerReset.Interval = configParameters.ResetDelay == 0 ? 1 : configParameters.ResetDelay * 1000;
                            timerReset.Enabled = true;
                        }
                    }


                    // Save the receipt to a file
                    //   ReceiptSave();
                    // Clear variables
                    //   if (!XMLIsLineItem() && !XMLIsErrorSIPDeviceBusy())
                    //     ReceiptClear();



                    var resp = XMLGetTagValueInResponse("Response");
                    var result = XMLGetTagValueInResponse("Result");

                    // this function is call for ALL responses .. so that it gets a trigger sent .. nothing specific is passed.
                    allResponse(resp);


                    if (resp.ToUpper() == "GETCARDDATA" && result == "0")
                    {
                        string requestid = XMLGetTagValueInResponse("RequestId");
                        string trackdata = XMLGetTagValueInResponse("TrackData");
                        getdataResponse(requestid, trackdata);
                    }

                    if (resp.ToUpper() == "SALE")
                    {
                        ResponseEventArgs e = new ResponseEventArgs(SIPResponseMessage);

                        //calls delegate
                        salesResponse(e);

                        LastCommand = "Sales";
                    }

                    if (resp.ToUpper() == "REFUND")
                    {
                        ResponseEventArgs e = new ResponseEventArgs(SIPResponseMessage);

                        //calls delegate
                        refundResponse(e);

                        LastCommand = "Refund";
                    }


                    if (resp.ToUpper() == "VOID")
                    {
                        VoidResponseArgs e = new VoidResponseArgs();
                        e.GatewayRspMsg = XMLGetTagValueInResponse("GatewayRspMsg");
                        e.GatewayRspCode = XMLGetTagValueInResponse("GatewayRspCode");
                        e.ResultText = XMLGetTagValueInResponse("ResultText");
                        e.Result = result;
                        e.OrigTransactionId = XMLGetTagValueInResponse("OrigTransactionId");
                        voidResponse(e);

                        LastCommand = "Void";
                    }

                    if (resp.ToUpper() == "ADDVALUE" || resp.ToUpper() == "BALANCEINQUIRY")
                    {
                        GiftResponseArgs e = new GiftResponseArgs(SIPResponseMessage);
                        giftResponse(e);
                        LastCommand = "Giftcard";
                    }

                    if ((resp.ToUpper() == "TIPADJUST" || resp.ToUpper() == "CREDITTIPADJUST") && XMLGetTagValueInResponse("GatewayRspMsg") == "Success")
                    {
                       // if (XMLGetTagValueInResponse("RequestId") == TipRequestId) tipadjustResponse(TipPaymentId, XMLGetTagValueInResponse("TipAmount"), XMLGetTagValueInResponse("TotalAmount"));
                       //revert back to version 3.2 so cannot use requestid
                         tipadjustResponse(TipPaymentId, XMLGetTagValueInResponse("TipAmount"), XMLGetTagValueInResponse("TotalAmount"));



                        LastCommand = "Tip Adjust";
                    }


                }

                // Return true if SIP result is good else false
                return SIPResultGood;
            }
            catch(Exception ex)
            {
                logger.Error("SIPReceiveCommands:" + ex.Message);
                return false;
            }

         
        }

        private bool SIPReceive()
        {
            // Receive from SIP
            SIPResponseMessage = iSC250.Recv();

            // Print the tag/values
            PrintReceipt();

            return !String.IsNullOrEmpty(SIPResponseMessage);
        }



        private bool SIPIsConnected()
        {
            return iSC250.IsConnected();
        }

        private bool getAck()
        {
            return iSC250.GetAck();
        }




        //-----------------------------------------   processing commands --------------------

        private decimal m_requestamount;
        public decimal RequestAmount
        { get
            { return m_requestamount; }
            set
            {
                m_requestamount = value;
                NotifyPropertyChanged("RequestAmount");
            }
        }

       

        string m_posmessage;
        public string POSMessage
        {
            get { return m_posmessage; }
            set
            {
                m_posmessage = value;
                NotifyPropertyChanged("POSMessage");

            }
        }


        string m_posxml;
        public string POSXML
        {
            get { return m_posxml; }
            set
            {
                m_posxml = value;
                NotifyPropertyChanged("POSXML");

            }
        }



        private string m_currenttime;
        public string CurrentTime
        {
            get { return m_currenttime; }
            set
            {
                m_currenttime = value;
                NotifyPropertyChanged("CurrentTime");
            }
        }

        public void ExecuteCommand(string command)
        {
            RequestId = Utility.RandomPin(7).ToString();
            switch (command)
            {
                case "OpenLane":
                   
                    SIPIsLaneOpen = true;
                    SIPSetCommand(HeartPOS.LaneOpenCommand);
                    break;

                case "CloseLane":
                   
                    SIPTransactionStarted = false;
                    SIPIsLaneOpen = false;
                    SIPSetCommand(HeartPOS.LaneCloseCommand);
                    break;

                case "GetBatch":
                  
                    POSMessage = "";
                    POSXML = "";
                    SIPSetCommand(HeartPOS.GetBatchReportCommand);
                    break;

                case "GetInfo":
                 
                    POSMessage = "";
                    POSXML = "";
                    SIPSetCommand(HeartPOS.GetAppInfoCommand);
                    break;


              

                case "GetParam":
                    
                    POSMessage = "";
                    POSXML = "";
                    SIPSetCommand(HeartPOS.GetParameterReportCommand);
                    break;



                case "CardVerify":
                    
                    POSMessage = "";
                    POSXML = "";
                    xmlRequestTagValues.CardGroup = "Credit";
                    SIPSetCommand(HeartPOS.CardVerify);
                    break;

                case "SendParam":
                    
                    POSMessage = "";
                    POSXML = "";
                    TextPad tp = new TextPad("Enter Parameter to Send", "");
                    tp.ShowDialog();

                   

                    string[] param = tp.ReturnText.Replace((char)10,' ').Trim().Split((char) 13);

                    xmlRequestTagValues.FieldCount = param.Count().ToString();
                    xmlRequestTagValues.ParamKeys = new String[param.Count()];
                    xmlRequestTagValues.ParamValues = new String[param.Count()];

                    int i = 0;

                    foreach (var pair in param)
                    {
                        string[] parts = pair.Split('=');

                        if (parts.Count() > 1)
                        {
                            xmlRequestTagValues.ParamKeys[i] = parts[0];

                            xmlRequestTagValues.ParamValues[i] = parts[1];

                            SIPSetCommand(HeartPOS.SetParameterCommand);
                            i++;
                        }
                    }

                
                 
                    break;


                case "EOD":
                   
                    POSMessage = "";
                    POSXML = "";
                    SIPSetCommand(HeartPOS.EODCommand);
                    break;


                case "Reset":
                    
                    SIPSetCommand(HeartPOS.ResetCommand);
                    break;

                case "SIPAdmin":

                    SIPSetCommand(HeartPOS.ManagerMenuCommand);
                    break;

                case "Reboot":
                   
                    Confirm conf = new Confirm("Are you sure?  Reboot Credit Card machine??");
                    conf.ShowDialog();
                    if(conf.Response == Confirm.OK)
                    {
                        SIPSetCommand(HeartPOS.RebootCommand);
                      
                    }
                    break;

             

                   
                  

            }
        }

        public void ExecuteSaleCommand(decimal amt, string requestid, string creditcardname)
        {
            //save this amount to enter into ticket
            RequestAmount = amt;
            //ccp.ServerCount = 2;
            POSMessage = "";
            // Reset the start card active when Abort/Sale/Refund is done - Added for defect DE24593
            isStartCardActive = false;



            xmlRequestTagValues.TipAmount = "0";
            xmlRequestTagValues.TotalAmount = Convert.ToInt32(amt * 100).ToString();
            xmlRequestTagValues.BaseAmount = Convert.ToInt32(amt * 100).ToString();

            xmlRequestTagValues.CardGroup = creditcardname;
       



            SIPTransactionStarted = SIPIsLaneOpen;



            RequestId = requestid;

            SIPSetCommand(HeartPOS.SaleCommand);
        }

        public void ExecuteVoidCommand(string transactionid, string requestid)
        {
            POSMessage = "";
            POSXML = "";
            RequestId = requestid;
            OriginalTransactionId = transactionid;
            xmlRequestTagValues.TransactionId = transactionid;
            SIPSetCommand(HeartPOS.VoidCommand);
        }

        public void ExecuteRefundCommand(decimal amt, string requestid, string creditcardname)
        {
            //save this amount to enter into ticket
            RequestAmount = amt;
            POSMessage = "";
            POSXML = "";

            // Reset the start card active when Abort/Sale/Refund is done - Added for defect DE24593
            isStartCardActive = false;

   

            xmlRequestTagValues.TotalAmount = Convert.ToInt32(amt * 100).ToString();

            xmlRequestTagValues.CardGroup = creditcardname;
    



            SIPTransactionStarted = SIPIsLaneOpen;

            RequestId = requestid;

            SIPSetCommand(RefundCommand);

        }

        public void ExecuteAddGiftCardCommand(decimal amt, string requestid)
        {
            //save this amount to enter into ticket
            RequestAmount = amt;
            POSMessage = "";
            POSXML = "";

            // Reset the start card active when Abort/Sale/Refund is done - Added for defect DE24593
            isStartCardActive = false;

            xmlRequestTagValues.TotalAmount = Convert.ToInt32(amt * 100).ToString();

  
            SIPTransactionStarted = SIPIsLaneOpen;

            RequestId = requestid;

            SIPSetCommand(AddValueCommand);

        }


        public void ExecuteGiftCardBalanceCommand(string requestid)
        {
            // Reset the start card active when Abort/Sale/Refund is done - Added for defect DE24593
            isStartCardActive = false;
            RequestId = requestid;
            POSMessage = "";
            POSXML = "";
            xmlRequestTagValues.CardGroup = "Gift";
            SIPTransactionStarted = SIPIsLaneOpen;
            SIPSetCommand(HeartPOS.BalanceInquiryCommand);
        }

        public void ExecuteGetCardData(string requestid)
        {
            // Reset the start card active when Abort/Sale/Refund is done - Added for defect DE24593
            isStartCardActive = false;
            RequestId = requestid;
            POSMessage = "";
            POSXML = "";
            xmlRequestTagValues.CardGroup = "Any";
            SIPTransactionStarted = SIPIsLaneOpen;
            SIPSetCommand(HeartPOS.GetDataCommand);
        }

        public void ExecuteAdjustTipCommand(decimal amt,string transactionid, string requestid)
        {
            POSMessage = "";
            POSXML = "";

            // Reset the start card active when Abort/Sale/Refund is done - Added for defect DE24593
            isStartCardActive = false;

            SIPTransactionStarted = SIPIsLaneOpen;

            RequestId = requestid;

            xmlRequestTagValues.TransactionId = transactionid;

         
            xmlRequestTagValues.TipAmount = Convert.ToInt32(amt * 100).ToString();


            SIPSetCommand(TipAjustCommand);
        }


        void HeartBeat()
        {
          
            CurrentTime = DateTime.Now.ToLongTimeString();
         
        }



        //converts string over to bytes so it can be passed
        private void DisplayDebugText(int MessageType, String Message)
        {
            DisplayDebugText(MessageType, Encoding.ASCII.GetBytes(Message));
        }

        private void DisplayDebugText(int MessageType, byte[] Message)
        {
            ProcessSIPResponse(MessageType, Message);



        }

        private void ProcessSIPResponse(int MessageType, byte[] Message)
        {


            String DisplayMessage;
            int i;
            string command="";
            string resulttext = "";


            // Exclude signature data
            DisplayMessage = System.Text.Encoding.ASCII.GetString(Message);

            // Do not display value of <Attachmentdata> tag in logs as it is near about 5300 bytes. 
            // Instead display <AttachmentData>Length</AttachmentData>
            // TODO: Should be converted to XMLDoc and then update the element value.
            // But for some reason, XMLDOC.parse() is failing. Will fix it during code cleanup
            i = DisplayMessage.IndexOf("<AttachmentData>");
            if (i > 0)
            {
                var attachEndTag = "</AttachmentData>";
                var rightStr = "";
                var leftStr = DisplayMessage.Substring(0, i);
                i = DisplayMessage.IndexOf(attachEndTag);
                if (i > 0)
                    rightStr = DisplayMessage.Substring((i + attachEndTag.Length + 1), (DisplayMessage.Length - i - attachEndTag.Length - 1));
                else
                    return;

                DisplayMessage = leftStr + rightStr;
            }

            DisplayMessage = DisplayMessage.Replace("&apos;", "'");

            // Take out nulls
            i = DisplayMessage.IndexOf('\0');
            if (i >= 0)
                DisplayMessage = DisplayMessage.Substring(0, i);

            // Hex print non-ASCII chars
            DisplayMessage = String.Concat(DisplayMessage.Select(c => Char.IsControl(c) ?
                                                            String.Format("[{0:X2}]", (int)c) :
                                                            c.ToString()));
            // Format for readability
            DisplayMessage = DisplayMessage.Replace("[0D]", "\r");
            DisplayMessage = DisplayMessage.Replace("[0A]", "\n");
            DisplayMessage = DisplayMessage.Replace("[02]", "[STX]");
            DisplayMessage = DisplayMessage.Replace("[03]", "[ETX]");
            DisplayMessage = DisplayMessage.Replace("[06]", "[ACK]");
            DisplayMessage = DisplayMessage.Replace("[15]", "[NAK]");
            DisplayMessage = DisplayMessage.Trim();
            DisplayMessage += "\n";

         

            switch (MessageType)
            {
                //REQUEST
                case 1:
                    command = XMLGetTagValue(DisplayMessage, "Request");

                    POSMessage = POSMessage + "[" + command + "] \r\n";


        
                    POSXML = POSXML + DisplayMessage;

                    break;

                //RESPONSE
                case 2:
                    command = XMLGetTagValue(DisplayMessage, "Response");
                    resulttext = XMLGetTagValue(DisplayMessage, "ResultText");

                    if (command == "LaneOpen" && resulttext.ToUpper() == "SUCCESS")
                    {
                        POSMessage = POSMessage + "Lane Opened Successfully..\r\n";

                        SIPIsLaneOpen = true;

                    }
                    else if (command == "LaneOpen")
                    {
                        POSMessage = POSMessage + "Lane Open Error: " + resulttext + "\r\n";
                        SIPIsLaneOpen = false;
                    }


                    else if (command == "LaneClose" && resulttext.ToUpper() == "SUCCESS")
                    {
                        POSMessage = POSMessage + "Lane Closed Successfully..\r\n";
                        SIPIsLaneOpen = false;
                    }
                    else if (command == "LaneClose")
                        POSMessage = POSMessage + "Lane Close Error: " + resulttext + "\r\n";

                    else if (command == "Reset" && resulttext.ToUpper() == "SUCCESS")
                        POSMessage = POSMessage + "Reset Successfully..\r\n";
                    else if (command == "Reset")
                        POSMessage = POSMessage + "Reset Error: " + resulttext + "\r\n";

                    //else POSMessage = POSMessage + "Response: " + command + "=>"  + resulttext + "\r\n";


                    POSXML = POSXML + DisplayMessage;

                    break;

                case 3:
                    command = "credit card info";
                    POSMessage = POSMessage + DisplayMessage;
                    break;
            }









            //calls the message handler from parent object  -  use to save message to auditing table
            messageResponse(MessageType, DisplayMessage, command);


         





        }



    }
}
