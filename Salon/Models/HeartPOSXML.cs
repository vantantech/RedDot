using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using HPS;
using HeartPOS;

namespace RedDot
{

    public partial class HeartPOS
    {
        // XML message stuff     
        // XMLPack() uses these values to pack XML request messages from a template .xml file.
        public struct XMLRequestTagValues
        {
            public String ECRId;
            public String RequestId;
            public String BaseAmount;
            public String TaxAmount;
            public String TipAmount;
            public String EBTAmount;
            public String TotalAmount;
            public String ConfirmAmount;

            public String CardGroup;
            public String TransactionId;
            public String InvoiceNumber;
            public String ApprovalCode;
            public String BatchReportType;
            public String SAFReport;
            public String FieldCount;
            public String FileName;
            public String FileSizeTotal;
            public String FileData;
            //public String FileDataBlockSize;
            public String MultipleMessage;
            public String[] ParamKeys;
            public String[] ParamValues;
            public String TrainingMode;
            public String DisplayString1;
            public String DisplayString2;
            public String DisplayString3;
            public String TerminalID;
            public String ApplicationID;
            public String DownloadURL;
            public String DownloadPort;
            public String DownloadType;
            public String DownloadTime;
            public String HudsURL;
            public String HudsPort;
            public String LineItemTextLeft;
            public String LineItemTextRight;
            public String LineItemRunningTextLeft;
            public String LineItemRunningTextRight;
            public String Time;
        }

        // SIP commands
        public const String ManagerMenuCommand = "ManagerMenu.xml";
        public const String GetAppInfoCommand = "GetAppInfoReport.xml";
        public const String GetParameterReportCommand = "GetParameterReport.xml";
        public const String GetEMVParameterReportCommand = "GetEMVParameterReport.xml";
        public const String GetSAFReportCommand = "GetSAFReport.xml";
        public const String GetBatchReportCommand = "GetBatchReport.xml";
        public const String SendSAFCommand = "SendSAF.xml";
        public const String CloseBatchCommand = "CloseBatch.xml";
        public const String ClearBatchCommand = "ClearBatch.xml";
        public const String RebootCommand = "Reboot.xml";
        public const String TrainingCommand = "Training.xml";
        public const String LaneOpenCommand = "LaneOpen.xml";

        public const String SaleCommand = "Sale.xml";
        public const String RefundCommand = "Refund.xml";
        public const String VoidCommand = "Void.xml";
        public const String VoiceAuthCommand = "VoiceAuth.xml";
        public const String CardVerify = "CardVerify.xml";
        public const String ActivateCommand = "Activate.xml";
        public const String BalanceInquiryCommand = "BalanceInquiry.xml";
        public const String StartCardCommand = "StartCard.xml";
        public const String AddValueCommand = "AddValue.xml";
        public const String TipAjustCommand = "TipAdjust.xml";
        public const String CreditTipAjustCommand = "CreditTipAdjust.xml";
        public const String GetDataCommand = "GetCardData.xml";
        public const String ResetCommand = "Reset.xml";
        public const String LaneCloseCommand = "LaneClose.xml";
        public const String DisplayCommand = "Display.xml";
        public const String CreditAuthCommand = "CreditAuth.xml";
        public const String CreditAuthCompleteCommand = "CreditAuthComplete.xml";
        public const String EODCommand = "EOD.xml";
        public const String SendFileCommand = "SendFile.xml";
        public const String SetParameterCommand = "SetParameter.xml";

        public const String ActivateTerminalCommand = "ActivateTerminal.xml";
        public const String CustomCommand = "Custom.xml";
        public const String DownloadRequestCommand = "DownloadRequest.xml";

        public const String LineItemCommand = "LineItem.xml";
        public const String ReversalCommand = "Reversal.xml";

        public const String SetClockCommand = "SetClock.xml";
        public const String GetPiggyBackReportCommand = "GetPiggyBackReport.xml";

        private String XMLPath = ConfigurationManager.AppSettings["XMLPath"];
        private XDocument XMLDoc = new XDocument();

        public String XMLPack(String XMLCommand, XMLRequestTagValues xmlRequestTagValues)
        {
            try
            {
                // Load the template
                XMLDoc = XDocument.Load(XMLPath + XMLCommand);
            }
            catch
            {
                try
                {
                    // get the xml Content
                    string xmlContent = File.ReadAllText(XMLPath + XMLCommand);

                    // this will encode only the content and not the tags.
                    var encodedContent = Regex.Replace(xmlContent, @"<[^>^<]+?>",
                    m => XmlConvert.EncodeName(m.Value)).Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

                    // decode the tags back to the orginal one.
                    encodedContent = XmlConvert.DecodeName(encodedContent);

                    XMLDoc = XDocument.Parse(encodedContent);
                }
                catch (XmlException exception)
                {
                    XMLDoc = null;
                    TouchMessageBox.Show("Unable to parse the xml request! \n\n" + exception.Message);
                }
                catch
                {
                    XMLDoc = null;
                    TouchMessageBox.Show("Unable to process the request!");
                }
            }
            if (XMLDoc == null)
                return "";

            if (XMLCommand == "Custom.xml")
                return XMLDoc.ToString() + "\n";

            XMLPackElement("ECRId", xmlRequestTagValues.ECRId);

            // Add the RequestId to all requests
            if (XMLDoc.Descendants("RequestId").Count() > 0)
            {
                XMLPackElement("RequestId", (xmlRequestTagValues.RequestId == null ? RequestId : xmlRequestTagValues.RequestId));
            }
            else
            {
                XMLDoc.Descendants("Request").FirstOrDefault().AddAfterSelf(new XElement("RequestId", RequestId));
            }
            XMLPackElement("BaseAmount", xmlRequestTagValues.BaseAmount);
            if (configParameters.EnableTaxPercent)
            {
                XMLPackElement("TaxAmount", xmlRequestTagValues.TaxAmount);
            }
            else
            {
                if (XMLDoc.Descendants("TaxAmount") != null)
                {
                    XMLDoc.Descendants("TaxAmount").Remove();
                }
            }


            XMLPackElement("TipAmount", xmlRequestTagValues.TipAmount);

            XMLPackElement("EBTAmount", xmlRequestTagValues.EBTAmount);

            XMLPackElement("TotalAmount", xmlRequestTagValues.TotalAmount);
            XMLPackElement("ConfirmAmount", xmlRequestTagValues.ConfirmAmount);
            XMLPackElement("CardGroup", xmlRequestTagValues.CardGroup);
            XMLPackElement("TransactionId", xmlRequestTagValues.TransactionId);
            XMLPackElement("InvoiceNbr", xmlRequestTagValues.InvoiceNumber);
            if (SIPCommand == SaleCommand)
            {
                for (int ServerNbr = 1; ServerNbr <= ServerCount; ServerNbr++)
                {
                    if (ServerNbr == 1 && !string.IsNullOrWhiteSpace(this.configParameters.ServerLabel1))
                        XMLDoc.Root.Add(new XElement("ServerLabel", this.configParameters.ServerLabel1));
                    else if (ServerNbr == 2 && !string.IsNullOrWhiteSpace(this.configParameters.ServerLabel2))
                        XMLDoc.Root.Add(new XElement("ServerLabel", this.configParameters.ServerLabel2));
                    else if (ServerNbr == 3 && !string.IsNullOrWhiteSpace(this.configParameters.ServerLabel3))
                        XMLDoc.Root.Add(new XElement("ServerLabel", this.configParameters.ServerLabel3));
                    else if (ServerNbr == 4 && !string.IsNullOrWhiteSpace(this.configParameters.ServerLabel4))
                        XMLDoc.Root.Add(new XElement("ServerLabel", this.configParameters.ServerLabel4));
                    else if (ServerNbr == 5 && !string.IsNullOrWhiteSpace(this.configParameters.ServerLabel5))
                        XMLDoc.Root.Add(new XElement("ServerLabel", this.configParameters.ServerLabel5));
                }
            }
            XMLPackElement("ApprovalCode", xmlRequestTagValues.ApprovalCode);
            XMLPackElement("BatchReportType", xmlRequestTagValues.BatchReportType);
            XMLPackElement("SAFReport", xmlRequestTagValues.SAFReport);
            XMLPackElement("FieldCount", xmlRequestTagValues.FieldCount);
            XMLPackElement("FileName", xmlRequestTagValues.FileName);
            XMLPackElement("FileSize", xmlRequestTagValues.FileSizeTotal);
            XMLPackElement("FileData", xmlRequestTagValues.FileData);
            XMLPackElement("MultipleMessage", xmlRequestTagValues.MultipleMessage);
            XMLPackElement("TrainingMode", xmlRequestTagValues.TrainingMode);

            if (xmlRequestTagValues.FieldCount != null)
            {
                if (XMLCommand == SetParameterCommand)
                {
                    //Do this only for SetParameter
                    for (int i = 0; i < Int32.Parse(xmlRequestTagValues.FieldCount); i++)
                    {
                        if (xmlRequestTagValues.ParamKeys != null &&
                            xmlRequestTagValues.ParamValues != null &&
                            xmlRequestTagValues.ParamKeys[i] != null &&
                            xmlRequestTagValues.ParamValues[i] != null)
                        {
                            XMLAddElement("Key", xmlRequestTagValues.ParamKeys[i]);
                            XMLAddElement("Value", xmlRequestTagValues.ParamValues[i]);
                        }
                    }
                }
            }

            XMLPackElement("LineItemTextLeft", xmlRequestTagValues.LineItemTextLeft);
            XMLPackElement("LineItemTextRight", xmlRequestTagValues.LineItemTextRight);
            XMLPackElement("LineItemRunningTextLeft", xmlRequestTagValues.LineItemRunningTextLeft);
            XMLPackElement("LineItemRunningTextRight", xmlRequestTagValues.LineItemRunningTextRight);

            XMLPackElement("TerminalID", xmlRequestTagValues.TerminalID);
            XMLPackElement("ApplicationID", xmlRequestTagValues.ApplicationID);
            XMLPackElement("DownloadType", xmlRequestTagValues.DownloadType);
            XMLPackElement("DownloadTime", xmlRequestTagValues.DownloadTime);
            XMLPackElement("HUDSURL", xmlRequestTagValues.HudsURL);
            XMLPackElement("HUDSPORT", xmlRequestTagValues.HudsPort);

            XMLPackElement("Time", xmlRequestTagValues.Time);

            return XMLDoc.ToString() + "\n";
        }

        public bool XMLPackElement(String sElement, String sValue)
        {
            var XMLElement = XMLDoc.Root.Element(sElement);

            //If no element, nothing to do
            if (XMLElement == null)
                return false;

            //if no value, remove element
            if (sValue == null)
            {
                XMLElement.Remove();
                return false;
            }

            //set value to element
            XMLElement.Value = sValue;
            return true;
        }

        public bool XMLAddElement(String sElement, String sValue)
        {
            var XMLElement = new XElement(sElement);

            //if no value, remove element
            if (sValue == null)
            {
                XMLElement.Remove();
                return false;
            }

            //set value to element
            XMLElement.Value = sValue;
            //add element to xml
            XMLDoc.Root.Add(XMLElement);

            return true;
        }

        public bool XMLIsValidRequest()
        {
            if ((SIPRequestMessage == null) || (SIPRequestMessage == ""))
                return false;
            else
                return true;
        }

        public bool XMLIsValidResponse()
        {
            if ((SIPResponseMessage == null) || (SIPResponseMessage == ""))
                return false;
            else
                return true;
        }

        public bool XMLIsTagPresentInRequest(String Tag)
        {
            if (!XMLIsValidRequest())
                return false;

            XMLDoc = XDocument.Parse(SIPRequestMessage);

            if (XMLDoc.Root.Element(Tag) != null)
                return true;
            else
                return false;
        }

        public bool XMLIsTagPresentInResponse(String Tag)
        {
            if (!XMLIsValidResponse())
                return false;

            XMLDoc = XDocument.Parse(SIPResponseMessage);

            if (XMLDoc.Root.Element(Tag) != null)
                return true;
            else
                return false;
        }

        public String XMLGetTagValueInRequest(String Tag)
        {
            if (!XMLIsValidRequest())
                return "";

            XMLDoc = XDocument.Parse(SIPRequestMessage);

            XElement element = XMLDoc.Root.Element(Tag);
            if (element != null)
                return element.Value;

            return "";
        }

        public String XMLGetTagValueInResponse(String Tag)
        {
            if (!XMLIsValidResponse())
                return "";

            XMLDoc = XDocument.Parse(SIPResponseMessage);

            XElement element = XMLDoc.Root.Element(Tag);
            if (element != null)
                return element.Value;

            return "";
        }


        public String XMLGetTagValue(string Message, String Tag)
        {
          

            XMLDoc = XDocument.Parse(Message);

            XElement element = XMLDoc.Root.Element(Tag);
            if (element != null)
                return element.Value;

            return "";
        }

        public bool XMLIsResponseMultiMessage()
        {
            if (XMLGetTagValueInResponse("MultipleMessage") == "1")
                return true;

            return false;
        }

        public bool XMLIsLineItem()
        {
            if (XMLGetTagValueInResponse("Response") == "LineItem")
                return true;

            return false;
        }

        public bool XMLIsErrorSIPDeviceBusy()
        {
            if (XMLGetTagValueInResponse("Result") == "1507")
                return true;

            return false;
        }
        public bool XMLIsTransaction()
        {
            String Transaction;

            // Notification text comes in upper case which breaks the logic, so converting the text to lower case to avoid any futher issues.
            Transaction = XMLGetTagValueInResponse("Response").ToLower();

            int result = 0;
            int.TryParse(XMLGetTagValueInResponse("Result").ToString(), out result);
            switch (Transaction)
            {
                case "reset":
                case "laneopen":
                case "laneclose":
                case "notification":
                    if (result > 0)
                    {
                        LastResponse = string.Empty;
                        return true;
                    }
                    else
                    {
                        LastResponse = "Notification";
                    }
                    return false;
                case "sentfile":
                case "lineitem":
                case "startcard":
                case "scan":
                    LastResponse = "Notification";
                    if (XMLGetTagValueInResponse("Result") == "102")
                    {
                        SIPCommand = ResetCommand;
                        return true;
                    }
                    return false;
                default:
                    LastResponse = Transaction;
                    return true;
            }
        }

        public bool XMLIsCustomForm()
        {
            var Transaction = XMLGetTagValueInResponse("Response").ToLower();

            switch (Transaction)
            {
                case "choiceform":
                case "signatureform":
                case "editform":
                case "statusform":
                    return true;
                default:
                    return false;
            }
        }
    }

}
