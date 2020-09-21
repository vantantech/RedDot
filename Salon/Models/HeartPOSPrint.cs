using System;
using System.Linq;
//using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Configuration;
using DColor = System.Drawing.Color;
using System.IO;
using HPS;
using System.Xml;
using System.Xml.Linq;
using System.Drawing;
using System.Text.RegularExpressions;

namespace RedDot
{
    public partial class HeartPOS
    {
        // Virtual receipt stuff
     
        System.Drawing.Font ReceiptFont = new System.Drawing.Font("Courier New", 12);
        String ReceiptLineItems = "";
      
        String ReceiptSeparator = "-";
        String ReceiptDivider = "~";
        String ReceiptPath = ConfigurationManager.AppSettings["ReceiptPath"];
      //  String ReceiptFile = "";
        const int ReceiptWidth = 30;

        int BatchRecordIdx;

        private String PrintLeftRight(String Left, String Right)
        {
            if ((Left.Length + Right.Length) <= (ReceiptWidth - 2))     //Leave space for two dots.
            {
                // form the display text
                string displayText = Left.PadRight(ReceiptWidth / 2, '.') + Right.PadLeft(ReceiptWidth / 2, '.');

                // check if display text extends beyond 42 characters and remove the dots to make it to 42.
                if (displayText.Length > ReceiptWidth)
                {
                    int index = displayText.IndexOf(".");
                    int truncatinglength = (displayText.Length - ReceiptWidth);
                    displayText = displayText.Remove(index, truncatinglength);
                }
                return displayText + "\n";
            }
            else
            {
                return Left.PadRight(ReceiptWidth, '.') + "\n" + Right.PadLeft(ReceiptWidth, '.') + "\n";
            }
        }

   


    

  





        private void PrintReceipt()
        {
            switch (XMLGetTagValueInRequest("Request").ToLower())
            {
                case "laneopen":
                case "laneclose":
                case "reset":
               // case "getcarddata":
                case "startcard":
                case "choiceform":
                case "statusform":
                case "editform":
                case "notification":
                    return;
            }

            // Clear image on Reset
 


            if (!string.IsNullOrWhiteSpace(XMLGetTagValueInResponse("Response").ToLower()))
            {
                switch (XMLGetTagValueInResponse("Response").ToLower())
                {
                    case "laneopen":
                    case "laneclose":
                    case "reset":
                   // case "getcarddata":
                    case "startcard":
                    case "choiceform":
                    case "statusform":
                    case "editform":
                    case "lineitem":
                    case "notification":
                        return;
                }
            }

            if (XMLGetTagValueInResponse("Result") == "0")
            {
                //Result tag is good
                //Print specific receipt/report template if we support it
                switch (XMLGetTagValueInResponse("Response").ToLower())
                {
                    case "getsafreport":
                    case "sendsaf":
                        PrintReport();
                        break;
                    
                   // case "closebatch":
                   // case "clearbatch":
                    case "getcarddata":
                    case "getparameterreport":
                    case "getemvparameterreport":
                    case "getappinforeport":
                        PrintReport();
                        break;
                    case "eod":
                        PrintReport();
                       // PrintEODReport();
                        break;


                    case "getbatchreport":
                        PrintReport();
                      //  PrintEODReport();
                        break;

                    case "signatureform":
                        PrintAllTagValues(false);
                        break;
                    default:
                        //Print all the tag/values
                        PrintAllTagValues();
                        break;
                }
            }
            else if (XMLIsValidResponse())
            {
                //Result tag has some error
                //Print all the tag/values
                PrintAllTagValues();
            }

        

         
        }

        private void PrintAllTagValues(bool printResponse = true)
        {
            XDocument XMLDoc;

            if (!XMLIsValidResponse())
                return;

            XMLDoc = XDocument.Parse(SIPResponseMessage);

            // print the response in the receipt when printResponse is true.
            if (printResponse)
                POSMessage += ReceiptSeparator.PadRight(ReceiptWidth, '-') + "\n";

            // Parse transaction response, all fields
            foreach (XElement element in XMLDoc.Root.DescendantNodes().OfType<XElement>())
            {
                //Dont print signaturedata
                if (element.Name.ToString().Trim() == "AttachmentData")
                {
                   // SaveSignatureImage(element.Value);
                    continue;
                }


                // print the response in the receipt when printResponse is true.
                if (printResponse)
                    POSMessage += PrintLeftRight(element.Name.ToString(), element.Value);
            }
        }

        private void PrintSAFReport()
        {
            XDocument XMLDoc;
            XElement Element;

            XMLDoc = XDocument.Parse(SIPResponseMessage);

            int Result = (int)XMLDoc.Root.Element("Result");
            if (Result != 0)
                return;

            /*if (XMLDoc.Root.Element("ApprovedSAFSummaryRecord") != null)
            {
                ReceiptResponse += ReceiptSeparator.PadRight(42, '-') + "\n";
                ReceiptResponse += "[Approved SAF Summary] \n";
                Element = XMLDoc.Root.Element("ApprovedSAFSummaryRecord").Element("NumberTransactions");
                ReceiptResponse += PrintLeftRight("Count:", Element.Value);
                Element = XMLDoc.Root.Element("ApprovedSAFSummaryRecord").Element("TotalAmount");
                ReceiptResponse += PrintLeftRight("Amount:", Element.Value);
            } */
            if (XMLDoc.Root.Element("PendingSAFSummaryRecord") != null)
            {
                POSMessage += ReceiptSeparator.PadRight(ReceiptWidth, '-') + "\n";
                POSMessage += "[Pending SAF Summary] \n";
                Element = XMLDoc.Root.Element("PendingSAFSummaryRecord").Element("NumberTransactions");
                POSMessage += PrintLeftRight("Count:", Element.Value);
                Element = XMLDoc.Root.Element("PendingSAFSummaryRecord").Element("TotalAmount");
                POSMessage += PrintLeftRight("Amount:", Element.Value);
            }
            if (XMLDoc.Root.Element("DeclinedSAFSummaryRecord") != null)
            {
                POSMessage += ReceiptSeparator.PadRight(ReceiptWidth, '-') + "\n";
                POSMessage += "[Declined SAF Summary] \n";
                Element = XMLDoc.Root.Element("DeclinedSAFSummaryRecord").Element("NumberTransactions");
                POSMessage += PrintLeftRight("Count:", Element.Value);
                Element = XMLDoc.Root.Element("DeclinedSAFSummaryRecord").Element("TotalAmount");
                POSMessage += PrintLeftRight("Amount:", Element.Value);
            }
            if (XMLDoc.Root.Element("StoredOfflineApprovedSummaryRecord") != null)
            {
                POSMessage += ReceiptSeparator.PadRight(ReceiptWidth, '-') + "\n";
                POSMessage += "[Stored Offline Approved Summary] \n";
                Element = XMLDoc.Root.Element("StoredOfflineApprovedSummaryRecord").Element("NumberTransactions");
                POSMessage += PrintLeftRight("Count:", Element.Value);
                Element = XMLDoc.Root.Element("StoredOfflineApprovedSummaryRecord").Element("TotalAmount");
                POSMessage += PrintLeftRight("Amount:", Element.Value);
            }
            /* if (XMLDoc.Root.Element("ApprovedSAFRecord") != null)
             {
                 ReceiptResponse += ReceiptSeparator.PadRight(42, '-') + "\n";
                 ApprovedSAFTransactionCount = (ulong)XMLDoc.Root.Element("ApprovedSAFRecord").Element("ReferenceNumber");
                 ReceiptResponse += "[Approved SAF Transaction #" + ApprovedSAFTransactionCount + " Details] \n";
                 foreach (XElement element in XMLDoc.Root.Element("ApprovedSAFRecord").DescendantNodes().OfType<XElement>())
                 {
                     // TAG...VALUE
                     ReceiptResponse += PrintLeftRight(element.Name.ToString(), element.Value);
                 }
             }*/
            if (XMLDoc.Root.Element("PendingSAFRecord") != null)
            {
                POSMessage += ReceiptSeparator.PadRight(ReceiptWidth, '-') + "\n";
                var pendingRefElement = XMLDoc.Root.Element("PendingSAFRecord").Element("TransactionId");
                if (pendingRefElement != null)
                {
                    POSMessage += "[Pending SAF# " + (ulong)pendingRefElement + " Details]\n";
                }
                foreach (XElement element in XMLDoc.Root.Element("PendingSAFRecord").DescendantNodes().OfType<XElement>())
                {
                    // TAG...VALUE
                    POSMessage += PrintLeftRight(element.Name.ToString(), element.Value);
                }
            }
            if (XMLDoc.Root.Element("DeclinedSAFRecord") != null)
            {
                POSMessage += ReceiptSeparator.PadRight(ReceiptWidth, '-') + "\n";
                var declinedRefElement = XMLDoc.Root.Element("DeclinedSAFRecord").Element("TransactionId");
                if (declinedRefElement != null)
                {
                    POSMessage += "[Declined SAF# " + (ulong)declinedRefElement + " Details]\n";
                }
                foreach (XElement element in XMLDoc.Root.Element("DeclinedSAFRecord").DescendantNodes().OfType<XElement>())
                {
                    // TAG...VALUE
                    POSMessage += PrintLeftRight(element.Name.ToString(), element.Value);
                }
            }
            if (XMLDoc.Root.Element("StoredOfflineApprovedRecord") != null)
            {
                POSMessage += ReceiptSeparator.PadRight(ReceiptWidth, '-') + "\n";
                var storedofflineRefElement = XMLDoc.Root.Element("StoredOfflineApprovedRecord").Element("TransactionId");
                if (storedofflineRefElement != null)
                {
                    POSMessage += "[StoredApproval# " + (ulong)storedofflineRefElement + " Details]\n";
                }
                foreach (XElement element in XMLDoc.Root.Element("StoredOfflineApprovedRecord").DescendantNodes().OfType<XElement>())
                {
                    // TAG...VALUE
                    POSMessage += PrintLeftRight(element.Name.ToString(), element.Value);
                }
            }
        }

        private void PrintBatchReport()
        {
            XDocument XMLDoc;

            String CardType;
            String TransactionType;

            XMLDoc = XDocument.Parse(SIPResponseMessage);

            int Result = (int)XMLDoc.Root.Element("Result");
            if (Result != 0)
                return;

            PrintSAFReport();

            if (XMLDoc.Root.Element("MerchantName") != null) // PORTICO BATCH REPORT HEADER
            {
                POSMessage += ReceiptSeparator.PadRight(ReceiptWidth, '-') + "\n";
                String MerchantName = (String)XMLDoc.Root.Element("MerchantName");
                POSMessage += PrintLeftRight("Merchant Name", MerchantName);

                String SiteId = (String)XMLDoc.Root.Element("SiteId");
                POSMessage += PrintLeftRight("Site ID", SiteId);

                String DeviceId = (String)XMLDoc.Root.Element("DeviceId");
                POSMessage += PrintLeftRight("Device ID", DeviceId);

                String BatchId = (String)XMLDoc.Root.Element("BatchId");
                POSMessage += PrintLeftRight("Batch ID", BatchId);

                String BatchSeqNbr = (String)XMLDoc.Root.Element("BatchSeqNbr");
                POSMessage += PrintLeftRight("Batch Sequence #", BatchSeqNbr);

                String BatchStatus = (String)XMLDoc.Root.Element("BatchStatus");
                POSMessage += PrintLeftRight("Batch Status", BatchStatus);

                if (BatchStatus == "OPEN")
                {
                    String OpenUtcDT = (String)XMLDoc.Root.Element("OpenUtcDT");
                    POSMessage += PrintLeftRight("Batch Open Time", OpenUtcDT);

                    String OpenTxnId = (String)XMLDoc.Root.Element("OpenTxnId");
                    POSMessage += PrintLeftRight("Batch Open Txn", OpenTxnId);
                }
                else if (BatchStatus == "CLOSED")
                {
                    String CloseUtcDT = (String)XMLDoc.Root.Element("CloseUtcDT");
                    POSMessage += PrintLeftRight("Batch Close Time", CloseUtcDT);

                    String CloseTxnId = (String)XMLDoc.Root.Element("CloseTxnId");
                    POSMessage += PrintLeftRight("Batch Close Txn", CloseTxnId);
                }

                var XMLElement = XMLDoc.Root.Element("BatchTxnCnt");
                if (XMLElement != null)
                {
                    String BatchTxnCnt = (String)XMLDoc.Root.Element("BatchTxnCnt");
                    POSMessage += PrintLeftRight("Batch Trans Count", BatchTxnCnt);
                }

                XMLElement = XMLDoc.Root.Element("BatchTxnAmt");
                if (XMLElement != null)
                {
                    String BatchTxnAmt = (String)XMLDoc.Root.Element("BatchTxnAmt");
                    POSMessage += PrintLeftRight("Batch Trans Amount", string.Format("{0:C2}", Convert.ToDecimal(BatchTxnAmt) / 100));
                }
                BatchRecordIdx = 1;
            }
            else if (XMLDoc.Root.Element("BatchNumber") != null) // EXCHANGE BATCH REPORT HEADER
            {
                POSMessage += ReceiptSeparator.PadRight(ReceiptWidth, '-') + "\n";
                String BatchNumber = (String)XMLDoc.Root.Element("BatchNumber");
                if (XMLGetTagValueInRequest("Request") == "GetBatchReport")
                    POSMessage += PrintLeftRight("Batch Record # ", BatchNumber);
                else
                    POSMessage += PrintLeftRight("Accepted Batch # ", BatchNumber);
            }


            foreach (XElement xelement in XMLDoc.Root.Descendants("CardSummaryRecord"))
            {
                POSMessage += ReceiptSeparator.PadRight(ReceiptWidth, '-') + "\n";
                CardType = (string)xelement.Element("CardType");
                POSMessage += "[" + CardType + " Card Summary Record] \n";
                foreach (XElement element in xelement.DescendantNodes().OfType<XElement>())
                {
                    // TAG...VALUE
                    if (element.Name.ToString() == "TotalAmount")
                        POSMessage += PrintLeftRight(element.Name.ToString(), string.Format("{0:C2}", Convert.ToDecimal(element.Value) / 100));
                    else
                        POSMessage += PrintLeftRight(element.Name.ToString(), element.Value);
                }
            }

            foreach (XElement xelement in XMLDoc.Root.Descendants("TransactionSummaryRecord"))
            {
                POSMessage += ReceiptSeparator.PadRight(ReceiptWidth, '-') + "\n";
                TransactionType = (string)xelement.Element("TransactionSummary");
                POSMessage += "[" + TransactionType + " Transaction Summary Record] \n";
                foreach (XElement element in xelement.DescendantNodes().OfType<XElement>())
                {
                    // TAG...VALUE
                    if (element.Name.ToString() != "TransactionSummary")
                    {
                        if (element.Name.ToString() == "TotalAmount")
                            POSMessage += PrintLeftRight(element.Name.ToString(), string.Format("{0:C2}", Convert.ToDecimal(element.Value) / 100));
                        else
                            POSMessage += PrintLeftRight(element.Name.ToString(), element.Value);
                    }
                }
            }
            if (XMLDoc.Root.Element("BatchDetailRecord") != null)
            {
                POSMessage += ReceiptSeparator.PadRight(ReceiptWidth, '-') + "\n";
                POSMessage += "[Batch Record #" + BatchRecordIdx + " Details] \n";
                foreach (XElement element in XMLDoc.Root.Element("BatchDetailRecord").DescendantNodes().OfType<XElement>())
                {
                    // TAG...VALUE
                    if (element.Name.ToString() != "TransactionSummary")
                    {
                        if (element.Name.ToString() == "TotalAmount")
                            POSMessage += PrintLeftRight(element.Name.ToString(), string.Format("{0:C2}", Convert.ToDecimal(element.Value) / 100));
                        else if (element.Name.ToString() == "RequestedAmount")
                            POSMessage += PrintLeftRight(element.Name.ToString(), string.Format("{0:C2}", Convert.ToDecimal(element.Value) / 100));
                        else if (element.Name.ToString() == "AuthAmount")
                            POSMessage += PrintLeftRight(element.Name.ToString(), string.Format("{0:C2}", Convert.ToDecimal(element.Value) / 100));
                        else if (element.Name.ToString() == "CashbackAmount")
                            POSMessage += PrintLeftRight(element.Name.ToString(), string.Format("{0:C2}", Convert.ToDecimal(element.Value) / 100));
                        else if (element.Name.ToString() == "TipAmount")
                            POSMessage += PrintLeftRight(element.Name.ToString(), string.Format("{0:C2}", Convert.ToDecimal(element.Value) / 100));
                        else if (element.Name.ToString() == "SettleAmount")
                            POSMessage += PrintLeftRight(element.Name.ToString(), string.Format("{0:C2}", Convert.ToDecimal(element.Value) / 100));
                        else
                            POSMessage += PrintLeftRight(element.Name.ToString(), element.Value);
                    }
                }
                BatchRecordIdx++;
            }
        }

        private void PrintKeyValueReport()
        {
            XDocument XMLDoc;

            XMLDoc = XDocument.Parse(SIPResponseMessage);

            if (!XMLIsTagPresentInResponse("Record"))
            {
                // print the report in the receipt.
                PrintAllTagValues();
                return;
            }

            // Print <TableCategory> if any
            XElement EleCat = XMLDoc.Root.Element("Record").Element("TableCategory");
            if (EleCat != null)
            {
                POSMessage += ReceiptSeparator.PadRight(ReceiptWidth, '-') + "\n";
                POSMessage += "[" + EleCat.Value + "]" + "\n";
            }

            foreach (XElement element in XMLDoc.Root.Element("Record").Elements("Field"))
            {
                // KEY...VALUE
                POSMessage += PrintLeftRight((string)element.Element("Key"), (string)element.Element("Value"));
            }
        }

        private void PrintParameterReport()
        {
            int Result = (int)XMLDoc.Root.Element("Result");
            if (Result == 0)
                PrintKeyValueReport();
        }

        private void PrintEMVParameterReport()
        {
            int Result = (int)XMLDoc.Root.Element("Result");
            if (Result == 0)
                PrintKeyValueReport();
        }

        private void PrintAppInfoReport()
        {
            int Result = (int)XMLDoc.Root.Element("Result");
            if (Result == 0)
                PrintKeyValueReport();
        }

        private void PrintReport()
        {
            int Result = (int)XMLDoc.Root.Element("Result");
            if (Result == 0)
                PrintKeyValueReport();
        }

        private void PrintEODReport()
        {
            XDocument XMLDoc;

            XMLDoc = XDocument.Parse(SIPResponseMessage);


            int Result = (int)XMLDoc.Root.Element("Result");
            if (Result == 0)
            {
                // Print <TableCategory> if any
                XElement EleCat = XMLDoc.Root.Element("Record").Element("TableCategory");
                if (EleCat != null)
                {
                   // if (EleCat.Value.Substring(0, 3) == "PAY") return;

                   // if (EleCat.Value.Substring(0,5) == "BATCH") return;

                   // if (EleCat.Value.Substring(0,5) == "TRANS") return;




                    POSMessage += ReceiptSeparator.PadRight(ReceiptWidth, '-') + "\n";
                    POSMessage += "[" + EleCat.Value + "]" + "\n";
                }

                foreach (XElement element in XMLDoc.Root.Element("Record").Elements("Field"))
                {
                    // KEY...VALUE
                    POSMessage += PrintLeftRight((string)element.Element("Key"), (string)element.Element("Value"));
                }


            }
              
        }

        private void ReceiptClear()
        {
            // Do not clear the reciept when start card is active. Added for defect DE24594
            if (!isStartCardActive && !XMLIsCustomForm())
            {
              
                ReceiptLineItems = "";
                POSMessage = "";
                SIPRequestMessage = null;
                SIPResponseMessage = null;

               
            }
        }

  
    }
}
