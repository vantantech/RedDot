using NLog;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TriPos.Configuration.ConfigPath;
using TriPOS;
using TriPOS.RequestHeaderFiles;
using TriPOS.RequestModels;
using TriPOS.ResponseModels;
using TriPOS.SignatureFiles;

namespace RedDot
{
    public class TriPOSModel
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private int m_laneid;
        string url;
        string reportingURL;
        public TriPOSModel(int laneid)
        {
            m_laneid = laneid;
            url = GlobalSettings.Instance.ElementExpressURL;
            reportingURL = GlobalSettings.Instance.ElementExpressReportingURL;

            try
            {
               if( GlobalSettings.Instance.CreditCardProcessor.ToUpper() == "VANTIV")
                {
                    // Obtain developer key and secret from triPOS.config
                    var triPosConfig = new XmlDocument();

                    var triPosConfigPathProvider = TriPosConfigPathProvider.CreateWithDefaults();
                    var configFilePath = triPosConfigPathProvider.GetPathToValidTriPosConfig();

                    triPosConfig.Load(configFilePath);

                    var acceptorIds = triPosConfig.GetElementsByTagName("acceptorId");
                    string acceptorId = acceptorIds[0].InnerXml;

                    var accountIds = triPosConfig.GetElementsByTagName("accountId");
                    string accountId = accountIds[0].InnerXml;


                    var accountTokens = triPosConfig.GetElementsByTagName("accountToken");
                    string accountToken = accountTokens[0].InnerXml;

                    var terminalIDs = triPosConfig.GetElementsByTagName("terminalId");
                    string terminalID = terminalIDs[0].InnerXml;


                    if (GlobalSettings.Instance.AcceptorID == "") GlobalSettings.Instance.AcceptorID = acceptorId;
                    if (GlobalSettings.Instance.AccountID == "") GlobalSettings.Instance.AccountID = accountId;
                    if (GlobalSettings.Instance.AccountToken == "") GlobalSettings.Instance.AccountToken = accountToken;
                    if (GlobalSettings.Instance.TerminalID == "") GlobalSettings.Instance.TerminalID = terminalID;
                }
               

            }
            catch(Exception ex)
            {
                logger.Error("tripos config loading:" + ex.Message);
            }


        }



        //-------------------------------------------------------------------------TRIPOS Functions ------------------------------------------------------------
        public SaleResponse2 ExecuteCreditSale(int salesid, decimal amount,string referencenumber)
        {
            string port = GlobalSettings.Instance.SIPPort; //need to get from database
            string host = GlobalSettings.Instance.SIPDefaultIPAddress;
            string urlstring = "http://{0}:{1}/api/v1/sale"; // also need to get from database
            string url = string.Format(urlstring,host, port);

            string contentType = "application/xml";

     
 

            var request = new SaleRequest
            {

                LaneId = m_laneid,
                TransactionAmount = amount,
                TicketNumber = salesid.ToString(),
                ReferenceNumber = referencenumber
            };

            string actualResponse;

            // Set UI post body with sample request
            var stringBuilderXml = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(SaleRequest));
            var xmlWriter = XmlWriter.Create(stringBuilderXml, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            xmlSerializer.Serialize(xmlWriter, request);
            string xmlRequest = stringBuilderXml.ToString();


            logger.Debug("SALE REQUEST:");
            logger.Debug(BeautifyContent(xmlRequest));
            // Send the request and obtain the response
            HttpResponseMessage response = SendRequest(url, xmlRequest, contentType);
            Task<string> readAsync = response.Content.ReadAsStringAsync();
            readAsync.Wait();
            actualResponse = readAsync.Result;

            logger.Debug("SALE RESPONSE:");
            logger.Debug(BeautifyContent(actualResponse));


            // Deserialize response to response object
            var respObj = new SaleResponse2();

            try
            {
                var ser = new XmlSerializer(typeof(SaleResponse2));
                var reader = new StringReader(actualResponse);
                respObj = (SaleResponse2)ser.Deserialize(reader);
                return respObj;
            }
            catch (InvalidOperationException)
            {
        

                    TouchMessageBox.Show("Error in response ..please see logs.");
                  
                    return null;
               
            }



        }

        public AuthorizationResponse ExecuteCreditAuthorize(int salesid, decimal amount,string referencenumber)
        {
            string port = GlobalSettings.Instance.SIPPort; //need to get from database
            string host = GlobalSettings.Instance.SIPDefaultIPAddress;
            string urlstring = "http://{0}:{1}/api/v1/authorization"; // also need to get from database
            string url = string.Format(urlstring,host, port);

            string contentType = "application/xml";

            var request = new AuthorizationRequest
            {

                LaneId = m_laneid,
                TransactionAmount = amount,
                TicketNumber = salesid.ToString(),
                ReferenceNumber = referencenumber
            };

            string actualResponse;

            // Set UI post body with sample request
            var stringBuilderXml = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(AuthorizationRequest));
            var xmlWriter = XmlWriter.Create(stringBuilderXml, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            xmlSerializer.Serialize(xmlWriter, request);
            string xmlRequest= stringBuilderXml.ToString();

            logger.Debug("AUTH REQUEST:");
            logger.Debug(BeautifyContent(xmlRequest));

            // Send the request and obtain the response
            HttpResponseMessage response = SendRequest(url, xmlRequest, contentType);
            Task<string> readAsync = response.Content.ReadAsStringAsync();
            readAsync.Wait();
            actualResponse = readAsync.Result;

            logger.Debug("AUTH RESPONSE:");
            logger.Debug(BeautifyContent(actualResponse));

            // Deserialize response to response object
            var respObj = new AuthorizationResponse();

            try
            {
                var ser = new XmlSerializer(typeof(AuthorizationResponse));
                var reader = new StringReader(actualResponse);
                respObj = (AuthorizationResponse)ser.Deserialize(reader);
                return respObj;
            }
            catch (InvalidOperationException)
            {
                // This indicates that the repsonse is an error response that is not the form of a saleResponse. 
                // If the error occurred within header validation, the response will be in JSON because triPOS has not yet inspected the content type of the request.
                if (!string.IsNullOrWhiteSpace(actualResponse))
                {
                    string cleaned = BeautifyContent(actualResponse);

                    logger.Debug("Cleaned:" + cleaned);
                    TouchMessageBox.Show("Error in response ..please see logs.");

                    return null;
                }
                return null;
            }



        }

        public IncrementalAuthorizationResponse ExecuteCreditAuthorizeIncremental(string transactionid, decimal amount, string referencenumber)
        {
            string port = GlobalSettings.Instance.SIPPort; //need to get from database
            string host = GlobalSettings.Instance.SIPDefaultIPAddress;
            string urlstring = "http://{0}:{1}/api/v1/authorization/{2}/incremental"; // also need to get from database
            string url = string.Format(urlstring,host, port, transactionid);

            string contentType = "application/xml";




            var request = new IncrementalAuthorizationRequest
            {

                LaneId = m_laneid,

                TransactionAmount = amount,
                ReferenceNumber = referencenumber
            };

            string actualResponse;

            // Set UI post body with sample request
            var stringBuilderXml = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(IncrementalAuthorizationRequest));
            var xmlWriter = XmlWriter.Create(stringBuilderXml, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            xmlSerializer.Serialize(xmlWriter, request);
            string postBody = stringBuilderXml.ToString();

            // Send the request and obtain the response
            HttpResponseMessage response = SendRequest(url, postBody, contentType);
            Task<string> readAsync = response.Content.ReadAsStringAsync();
            readAsync.Wait();
            actualResponse = readAsync.Result;

            logger.Debug(actualResponse);

            // Deserialize response to response object
            var respObj = new IncrementalAuthorizationResponse();

            try
            {
                var ser = new XmlSerializer(typeof(IncrementalAuthorizationResponse));
                var reader = new StringReader(actualResponse);
                respObj = (IncrementalAuthorizationResponse)ser.Deserialize(reader);
                return respObj;
            }
            catch (InvalidOperationException)
            {
                // This indicates that the repsonse is an error response that is not the form of a saleResponse. 
                // If the error occurred within header validation, the response will be in JSON because triPOS has not yet inspected the content type of the request.
                if (!string.IsNullOrWhiteSpace(actualResponse))
                {
                    string cleaned = BeautifyContent(actualResponse);

                    logger.Debug("Cleaned:" + cleaned);
                    TouchMessageBox.Show("Error in response ..please see logs.");

                    return null;
                }
                return null;
            }



        }

        public AuthorizationCompletionResponse ExecuteCreditAuthorizeCompletion(string transactionid, decimal amount, string referencenumber)
        {
            string port = GlobalSettings.Instance.SIPPort; //need to get from database
            string host = GlobalSettings.Instance.SIPDefaultIPAddress;
            string urlstring = "http://{0}:{1}/api/v1/authorization/{2}/completion"; // also need to get from database
            string url = string.Format(urlstring,host, port,transactionid);

            string contentType = "application/xml";




            var request = new AuthorizationRequest
            {

                LaneId = m_laneid,

                TransactionAmount = amount,
                ReferenceNumber = referencenumber

            };

            string actualResponse;

            // Set UI post body with sample request
            var stringBuilderXml = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(AuthorizationRequest));
            var xmlWriter = XmlWriter.Create(stringBuilderXml, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            xmlSerializer.Serialize(xmlWriter, request);
            string postBody = stringBuilderXml.ToString();

            // Send the request and obtain the response
            HttpResponseMessage response = SendRequest(url, postBody, contentType);
            Task<string> readAsync = response.Content.ReadAsStringAsync();
            readAsync.Wait();
            actualResponse = readAsync.Result;

            logger.Debug(actualResponse);

            // Deserialize response to response object
            var respObj = new AuthorizationCompletionResponse();

            try
            {
                var ser = new XmlSerializer(typeof(AuthorizationCompletionResponse));
                var reader = new StringReader(actualResponse);
                respObj = (AuthorizationCompletionResponse)ser.Deserialize(reader);
                return respObj;
            }
            catch (InvalidOperationException)
            {
                // This indicates that the repsonse is an error response that is not the form of a saleResponse. 
                // If the error occurred within header validation, the response will be in JSON because triPOS has not yet inspected the content type of the request.
                if (!string.IsNullOrWhiteSpace(actualResponse))
                {
                    string cleaned = BeautifyContent(actualResponse);

                    logger.Debug("Cleaned:" + cleaned);
                    TouchMessageBox.Show("Error in response ..please see logs.");

                    return null;
                }
                return null;
            }



        }

        public VoidResponse ExecuteVoid(string transactionid, string referencenumber)
        {
            string port = GlobalSettings.Instance.SIPPort; //need to get from database
            string host = GlobalSettings.Instance.SIPDefaultIPAddress;
            string urlstring = "http://{0}:{1}/api/v1/void/{2}"; // also need to get from database
            string url = string.Format(urlstring,host, port, transactionid);

            string contentType = "application/xml";

            logger.Info("COMMAND:Credit Void , Transaction ID=" + transactionid);

            var request = new VoidRequest {
                LaneId = m_laneid,
                ReferenceNumber = referencenumber
            };

            string actualResponse;

            // Set UI post body with sample request
            var stringBuilderXml = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(VoidRequest));
            var xmlWriter = XmlWriter.Create(stringBuilderXml, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            xmlSerializer.Serialize(xmlWriter, request);
            string postBody = stringBuilderXml.ToString();

            // Send the request and obtain the response
            HttpResponseMessage response = SendRequest(url, postBody, contentType);
            Task<string> readAsync = response.Content.ReadAsStringAsync();
            readAsync.Wait();
            actualResponse = readAsync.Result;

            logger.Debug(actualResponse);

            // Deserialize response to response object
            var respObj = new VoidResponse();

            try
            {
                var ser = new XmlSerializer(typeof(VoidResponse));
                var reader = new StringReader(actualResponse);
                respObj = (VoidResponse)ser.Deserialize(reader);
                logger.Info("Credit Void:ApprovalNumber=" + respObj.ApprovalNumber);
                return respObj;
            }
            catch (InvalidOperationException)
            {
                // This indicates that the repsonse is an error response that is not the form of a saleResponse. 
                // If the error occurred within header validation, the response will be in JSON because triPOS has not yet inspected the content type of the request.
                if (!string.IsNullOrWhiteSpace(actualResponse))
                {
                    string cleaned = BeautifyContent(actualResponse);

                    logger.Debug("Cleaned:" + cleaned);
                    TouchMessageBox.Show("Error in response ..please see logs.");

                    return null;
                }
                return null;
            }
        }

        public ReversalResponse ExecuteReversal(string transactionid, string paymenttype , decimal amount, string referencenumber)
        {

            string port = GlobalSettings.Instance.SIPPort; //need to get from database
            string host = GlobalSettings.Instance.SIPDefaultIPAddress;
            string urlstring = "http://{0}:{1}/api/v1/reversal/{2}/{3}"; // also need to get from database
            string url = string.Format(urlstring,host, port, transactionid, paymenttype);

            string contentType = "application/xml";
            logger.Info("COMMAND:Credit Reversal , Transaction ID=" + transactionid);

            var request = new ReversalRequest {
                LaneId = m_laneid,
                TransactionAmount =amount,
                ReferenceNumber = referencenumber
            };

            string actualResponse;

            // Set UI post body with sample request
            var stringBuilderXml = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(ReversalRequest));
            var xmlWriter = XmlWriter.Create(stringBuilderXml, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            xmlSerializer.Serialize(xmlWriter, request);
            string postBody = stringBuilderXml.ToString();

            // Send the request and obtain the response
            HttpResponseMessage response = SendRequest(url, postBody, contentType);
            Task<string> readAsync = response.Content.ReadAsStringAsync();
            readAsync.Wait();
            actualResponse = readAsync.Result;

            logger.Debug(actualResponse);

            // Deserialize response to response object
            var respObj = new ReversalResponse();

            try
            {
                var ser = new XmlSerializer(typeof(ReversalResponse));
                var reader = new StringReader(actualResponse);
                respObj = (ReversalResponse)ser.Deserialize(reader);
                logger.Info("Credit Reversal:ApprovalNumber=" + respObj.ApprovalNumber);
                return respObj;
            }
            catch (InvalidOperationException)
            {
                // This indicates that the repsonse is an error response that is not the form of a saleResponse. 
                // If the error occurred within header validation, the response will be in JSON because triPOS has not yet inspected the content type of the request.
                if (!string.IsNullOrWhiteSpace(actualResponse))
                {
                    string cleaned = BeautifyContent(actualResponse);

                    logger.Debug("Cleaned:" + cleaned);
                    TouchMessageBox.Show("Error in response ..please see logs.");

                    return null;
                }
                return null;
            }
        }

        public ReturnResponse ExecuteReturn(string transactionid, string paymenttype, decimal amount, string referencenumber)
        {


            string port = GlobalSettings.Instance.SIPPort; //need to get from database
            string host = GlobalSettings.Instance.SIPDefaultIPAddress;
            string urlstring = "http://{0}:{1}/api/v1/return/{2}/{3}"; // also need to get from database
            string url = string.Format(urlstring,host, port, transactionid, paymenttype);

            string contentType = "application/xml";
            logger.Info("COMMAND:Credit Return , Transaction ID=" + transactionid);

            var request = new ReturnRequest {
                LaneId = m_laneid,
                TransactionAmount = amount,
                ReferenceNumber = referencenumber
            };

            string actualResponse;

            // Set UI post body with sample request
            var stringBuilderXml = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(ReturnRequest));
            var xmlWriter = XmlWriter.Create(stringBuilderXml, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            xmlSerializer.Serialize(xmlWriter, request);
            string postBody = stringBuilderXml.ToString();

            // Send the request and obtain the response
            HttpResponseMessage response = SendRequest(url, postBody, contentType);
            Task<string> readAsync = response.Content.ReadAsStringAsync();
            readAsync.Wait();
            actualResponse = readAsync.Result;

            logger.Debug(actualResponse);

            // Deserialize response to response object
            var respObj = new ReturnResponse();

            try
            {
                var ser = new XmlSerializer(typeof(ReturnResponse));
                var reader = new StringReader(actualResponse);
                respObj = (ReturnResponse)ser.Deserialize(reader);
                logger.Info("Credit Return:ApprovalNumber=" + respObj.ApprovalNumber);
                return respObj;
            }
            catch (InvalidOperationException)
            {
                // This indicates that the repsonse is an error response that is not the form of a saleResponse. 
                // If the error occurred within header validation, the response will be in JSON because triPOS has not yet inspected the content type of the request.
                if (!string.IsNullOrWhiteSpace(actualResponse))
                {
                    string cleaned = BeautifyContent(actualResponse);

                    logger.Debug("Cleaned:" + cleaned);
                    TouchMessageBox.Show("Error in response ..please see logs.");

                    return null;
                }
                return null;
            }
        }

        public RefundResponse ExecuteRefund(decimal amount, string referencenumber)
        {
            string port = GlobalSettings.Instance.SIPPort; //need to get from database
            string host = GlobalSettings.Instance.SIPDefaultIPAddress;
            string urlstring = "http://{0}:{1}/api/v1/refund"; // also need to get from database
            string url = string.Format(urlstring,host, port);

            string contentType = "application/xml";

         


            var request = new RefundRequest
            {

                LaneId = m_laneid,
                TransactionAmount = amount,
                ReferenceNumber = referencenumber
            };

            string actualResponse;

            // Set UI post body with sample request
            var stringBuilderXml = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(RefundRequest));
            var xmlWriter = XmlWriter.Create(stringBuilderXml, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            xmlSerializer.Serialize(xmlWriter, request);
            string postBody = stringBuilderXml.ToString();

            // Send the request and obtain the response
            HttpResponseMessage response = SendRequest(url, postBody, contentType);
            Task<string> readAsync = response.Content.ReadAsStringAsync();
            readAsync.Wait();
            actualResponse = readAsync.Result;

            logger.Debug(actualResponse);


            // Deserialize response to response object
            var respObj = new RefundResponse();

            try
            {
                var ser = new XmlSerializer(typeof(RefundResponse));
                var reader = new StringReader(actualResponse);
                respObj = (RefundResponse)ser.Deserialize(reader);
                return respObj;
            }
            catch (InvalidOperationException)
            {
                // This indicates that the repsonse is an error response that is not the form of a saleResponse. 
                // If the error occurred within header validation, the response will be in JSON because triPOS has not yet inspected the content type of the request.
                if (!string.IsNullOrWhiteSpace(actualResponse))
                {
                    string cleaned = BeautifyContent(actualResponse);

                    logger.Debug("Cleaned:" + cleaned);
                    TouchMessageBox.Show("Error in response ..please see logs.");

                    return null;
                }
                return null;
            }



        }

        public BatchCloseResponse ExecuteBatch()
        {

            // Obtain developer key and secret from triPOS.config
            var triPosConfig = new XmlDocument();

            var triPosConfigPathProvider = TriPosConfigPathProvider.CreateWithDefaults();
            var configFilePath = triPosConfigPathProvider.GetPathToValidTriPosConfig();

            triPosConfig.Load(configFilePath);

            var acceptorIds = triPosConfig.GetElementsByTagName("acceptorId");
            string acceptorId = acceptorIds[0].InnerXml;

            var accountIds = triPosConfig.GetElementsByTagName("accountId");
            string accountId = accountIds[0].InnerXml;


            var accountTokens = triPosConfig.GetElementsByTagName("accountToken");
            string accountToken = accountTokens[0].InnerXml;

            var terminalIDs = triPosConfig.GetElementsByTagName("terminalId");
            string terminalID = terminalIDs[0].InnerXml;



            var request = new BatchClose();
            request.ExpCredentials = new Credentials();
            request.ExpCredentials.AccountID = accountId;
            request.ExpCredentials.AccountToken = accountToken;
            request.ExpCredentials.AcceptorID = acceptorId;
            request.ExpApplication = new Application { ApplicationID = "9769", ApplicationName = "Red Dot POS", ApplicationVersion = "1.2.1" };
            request.ExpTerminal = new Terminal { TerminalID = terminalID };
            request.ExpBatch = new BatchResponse { BatchCloseType = 1, HostBatchID = "1" };  //Force Batch
         

            // Set UI post body with sample request
            var stringBuilderXml = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(BatchClose));
            var xmlWriter = XmlWriter.Create(stringBuilderXml, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            xmlSerializer.Serialize(xmlWriter, request);
            string postBody = stringBuilderXml.ToString();


            logger.Info("Close Batch Request:" + postBody);

   

            string response="";

            try
            {
               
                response =Send(postBody, url);
                logger.Info("Batch Close:" + response);

            }catch(Exception ex)
            {
                TouchMessageBox.Show(response);
                return null;
            }




            // Deserialize response to response object
            var respObj = new BatchCloseResponse();

            try
            {
                var ser = new XmlSerializer(typeof(BatchCloseResponse));
                var reader = new StringReader(response);
                respObj = (BatchCloseResponse)ser.Deserialize(reader);
                return respObj;
            }
            catch (InvalidOperationException)
            {
                // This indicates that the repsonse is an error response that is not the form of a saleResponse. 
                // If the error occurred within header validation, the response will be in JSON because triPOS has not yet inspected the content type of the request.
                if (!string.IsNullOrWhiteSpace(response))
                {
                    string cleaned = BeautifyContent(response);

                    logger.Debug("Cleaned:" + cleaned);
                    TouchMessageBox.Show("Error in response ..please see logs.");

                    return null;
                }
                return null;
            }



        }

        public BatchTotalsQueryResponse ExecuteBatchTotalsQuery()
        {
            string accountId = GlobalSettings.Instance.AccountID;
            string accountToken = GlobalSettings.Instance.AccountToken;
            string acceptorId = GlobalSettings.Instance.AcceptorID;
            string terminalID = GlobalSettings.Instance.TerminalID;
         

            var request = new BatchTotalsQuery();
            request.ExpCredentials = new Credentials { AccountID = accountId, AccountToken = accountToken, AcceptorID = acceptorId };
             request.ExpApplication = new Application { ApplicationID = "9769", ApplicationName = "Red Dot POS", ApplicationVersion = "1.2.1" };
            request.ExpTerminal = new Terminal { TerminalID = terminalID };
            request.ExpBatch = new BatchResponse { BatchQueryType = 0 };  //0= Totals Query  1=item query


            // Set UI post body with sample request
            var stringBuilderXml = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(BatchTotalsQuery));
            var xmlWriter = XmlWriter.Create(stringBuilderXml, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            xmlSerializer.Serialize(xmlWriter, request);
            string postBody = stringBuilderXml.ToString();


            logger.Info("Batch Totals Query Request:");
            logger.Info(BeautifyContent(postBody));


            string response = "";

            try
            {

                response = Send(postBody, url);
                logger.Info("Batch Totals Response:" );
                logger.Info(BeautifyContent(response));

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("error:" + ex.Message);
                logger.Error("Error:" + ex.Message);
                return null;
            }




            // Deserialize response to response object
            var respObj = new BatchTotalsQueryResponse();

            try
            {
                var ser = new XmlSerializer(typeof(BatchTotalsQueryResponse));
                var reader = new StringReader(response);
                respObj = (BatchTotalsQueryResponse)ser.Deserialize(reader);
                return respObj;
            }
            catch (InvalidOperationException)
            {
   
                    TouchMessageBox.Show("Error in response ..please see logs.");
                return null;
            
            }



        }

        public BatchItemQueryResponse ExecuteBatchItemQuery(int querytype)
        {

            string accountId = GlobalSettings.Instance.AccountID;
            string accountToken = GlobalSettings.Instance.AccountToken;
            string acceptorId = GlobalSettings.Instance.AcceptorID;
            string terminalID = GlobalSettings.Instance.TerminalID;

            var request = new BatchItemQuery();
            request.ExpCredentials = new Credentials { AccountID = accountId, AccountToken = accountToken, AcceptorID = acceptorId };
            request.ExpApplication = new Application { ApplicationID = "9769", ApplicationName = "Red Dot POS", ApplicationVersion = "1.2.1" };
            request.ExpTerminal = new Terminal { TerminalID = terminalID };
            request.ExpBatch = new BatchRequest { BatchQueryType = querytype };  //0= Totals Query  1=item query


            // Set UI post body with sample request
            var stringBuilderXml = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(BatchItemQuery));
            var xmlWriter = XmlWriter.Create(stringBuilderXml, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            xmlSerializer.Serialize(xmlWriter, request);
            string postBody = stringBuilderXml.ToString();


            logger.Info("Batch Item Query Request:" + postBody);

    
            string response = "";

            try
            {

                response = Send(postBody, url);
                logger.Info("Batch Item Query Response:" );
                logger.Info(BeautifyContent(response));

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show(response);
                return null;
            }




            // Deserialize response to response object
            var respObj = new BatchItemQueryResponse();

            try
            {
                var ser = new XmlSerializer(typeof(BatchItemQueryResponse));
                var reader = new StringReader(response);
                respObj = (BatchItemQueryResponse)ser.Deserialize(reader);
                return respObj;
            }
            catch (InvalidOperationException)
            {
                // This indicates that the repsonse is an error response that is not the form of a saleResponse. 
                // If the error occurred within header validation, the response will be in JSON because triPOS has not yet inspected the content type of the request.
                if (!string.IsNullOrWhiteSpace(response))
                {
                    string cleaned = BeautifyContent(response);

                    logger.Debug("Cleaned:" + cleaned);
                    TouchMessageBox.Show("Error in response ..please see logs.");

                    return null;
                }
                return null;
            }



        }

        public TransactionQueryResponse ExecuteTransactionQuery(string referenceNumber)
        {
            string accountId = GlobalSettings.Instance.AccountID;
            string accountToken = GlobalSettings.Instance.AccountToken;
            string acceptorId = GlobalSettings.Instance.AcceptorID;
            string terminalID = GlobalSettings.Instance.TerminalID;

            var request = new TransactionQuery();
            request.ExpCredentials = new Credentials { AccountID = accountId, AccountToken = accountToken, AcceptorID = acceptorId };
            request.ExpApplication = new Application { ApplicationID = "9769", ApplicationName = "Red Dot POS", ApplicationVersion = "1.2.1" };
            request.ExpParameters = new Parameters { ReferenceNumber = referenceNumber };
       


            // Set UI post body with sample request
            var stringBuilderXml = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(TransactionQuery));
            var xmlWriter = XmlWriter.Create(stringBuilderXml, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            xmlSerializer.Serialize(xmlWriter, request);
            string postBody = stringBuilderXml.ToString();

            logger.Info("***----------------------Transaction Query --------------------------------***");
            logger.Info("***----------------------Request:------------------------------------------***");
            logger.Info(BeautifyContent(postBody));


            string response = "";

            try
            {

                response = Send(postBody, reportingURL);
                logger.Debug("***-----------------------Response:-----------------------------------***");
                logger.Debug(BeautifyContent(response));

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("Send Error:" + ex.Message);
                return null;
            }




            // Deserialize response to response object
            var respObj = new TransactionQueryResponse();

            try
            {
                var ser = new XmlSerializer(typeof(TransactionQueryResponse));
                var reader = new StringReader(response);
                respObj = (TransactionQueryResponse)ser.Deserialize(reader);
                return respObj;
            }
            //catch (InvalidOperationException)
            catch(Exception ex)
            {
                TouchMessageBox.Show("Error in response ..please see logs." + ex.Message);
                return null;
            }



        }

        public RebootLaneResponse Reboot()
        {
            string port = GlobalSettings.Instance.SIPPort; //need to get from database
            string host = GlobalSettings.Instance.SIPDefaultIPAddress;
            string urlstring = "http://{0}:{1}/api/v1/reboot"; // also need to get from database
            string url = string.Format(urlstring, host, port);

            string contentType = "application/xml";




            var request = new RebootLaneRequest
            {

                LaneId = m_laneid,
          
            };

            string actualResponse;

            // Set UI post body with sample request
            var stringBuilderXml = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(RebootLaneRequest));
            var xmlWriter = XmlWriter.Create(stringBuilderXml, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            xmlSerializer.Serialize(xmlWriter, request);
            string xmlRequest = stringBuilderXml.ToString();


            logger.Debug("REBOOT LANE REQUEST:");
            logger.Debug(BeautifyContent(xmlRequest));
            // Send the request and obtain the response
            HttpResponseMessage response = SendRequest(url, xmlRequest, contentType);
            Task<string> readAsync = response.Content.ReadAsStringAsync();
            readAsync.Wait();
            actualResponse = readAsync.Result;

            logger.Debug("REBOOT LANE RESPONSE:");
            logger.Debug(BeautifyContent(actualResponse));


            // Deserialize response to response object
            var respObj = new RebootLaneResponse();

            try
            {
                var ser = new XmlSerializer(typeof(RebootLaneResponse));
                var reader = new StringReader(actualResponse);
                respObj = (RebootLaneResponse)ser.Deserialize(reader);
                return respObj;
            }
            catch (InvalidOperationException)
            {
                // This indicates that the repsonse is an error response that is not the form of a saleResponse. 
                // If the error occurred within header validation, the response will be in JSON because triPOS has not yet inspected the content type of the request.
                if (!string.IsNullOrWhiteSpace(actualResponse))
                {
                    string cleaned = BeautifyContent(actualResponse);

                    logger.Debug("Cleaned:" + cleaned);
                    TouchMessageBox.Show("Error in response ..please see logs.");

                    return null;
                }
                return null;
            }



        }


        //----------------------------------------------------------------- private functions --------------------------------------------

        private string BeautifyContent(string actualResponse)
        {
            try
            {
                var memStream = new MemoryStream();
                var settings = new XmlWriterSettings { Indent = true, IndentChars = "  ", NewLineChars = "\r\n", NewLineHandling = NewLineHandling.Replace, Encoding = new UTF8Encoding(false) };
                using (var writer = XmlWriter.Create(memStream, settings))
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(actualResponse);
                    doc.Save(writer);
                }

                return Encoding.UTF8.GetString(memStream.ToArray());
            }
            catch
            {
                return "";
            }

        }

        private string Send(string data, string url)
        {
            string result = string.Empty;

            var byteData = Encoding.ASCII.GetBytes(data);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            webRequest.ContentLength = data.Length;
        

            using (var stream = webRequest.GetRequestStream())
            {
                stream.Write(byteData, 0, byteData.Length);
            }

            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();

            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    result = rd.ReadToEnd();
                }
            }

            return result;
        }

        private HttpResponseMessage SendRequest(string url, string postBody, string contentType)
        {
            // Obtain developer key and secret from triPOS.config
            var triPosConfig = new XmlDocument();

            var triPosConfigPathProvider = TriPosConfigPathProvider.CreateWithDefaults();
            var configFilePath = triPosConfigPathProvider.GetPathToValidTriPosConfig();

            triPosConfig.Load(configFilePath);

            var developerKeys = triPosConfig.GetElementsByTagName("developerKey");
            string developerKey = developerKeys[0].InnerXml;

            var developerSecrets = triPosConfig.GetElementsByTagName("developerSecret");
            string developerSecret = developerSecrets[0].InnerXml;

            // Verify that necessary fields are not null
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(developerKey) || string.IsNullOrWhiteSpace(developerSecret))
            {
                throw new Exception("Values on config page cannot be empty");
            }

            // Send request
            HttpResponseMessage respMessage;
            using (var httpClient = new HttpClient())
            {
                var message = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
                CreateRequestHeaders(message, postBody, contentType, developerKey, developerSecret);
              
                Task<HttpResponseMessage> response = httpClient.SendAsync(message);
                response.Wait();
                respMessage = response.Result;
            }

            return respMessage;
        }

        private void CreateRequestHeaders(HttpRequestMessage message, string postBody, string contentType, string developerKey, string developerSecret)
        {
            AuthorizationHeader authorizationHeader = AuthorizationHeader.Create(message.Headers, message.RequestUri, postBody, message.Method.Method, "1.0", "TP-HMAC-SHA1", Guid.NewGuid().ToString(), DateTime.UtcNow.ToString("O"), developerKey, developerSecret);

            message.Headers.Add("tp-authorization", authorizationHeader.ToString());
            message.Headers.Add("tp-application-id", "1234");
            message.Headers.Add("tp-application-name", "TriPOS");
            message.Headers.Add("tp-application-version", "1.0.0");
            message.Headers.Add("tp-return-logs", "false");
            message.Headers.Add("accept", contentType);
            message.Content = new StringContent(postBody, Encoding.UTF8, contentType);
        }

        public static SaleResponse2 ConvertToSale(AuthorizationResponse input)
        {
            SaleResponse2 output = new SaleResponse2();

            output.AccountNumber = input.AccountNumber;
            output.ApprovalNumber = input.ApprovalNumber;
            output.ApprovedAmount = input.ApprovedAmount;
            output.CardHolderName = input.CardHolderName;
            output.CardLogo = input.CardLogo;
            output.CashbackAmount = input.CashbackAmount;
            output.Emv = input.Emv;
            output.EntryMode = input.EntryMode;
            output.PaymentType = input.PaymentType;
            output.PinVerified = input.PinVerified;
            output.ProcessorResponse = input.ProcessorResponse;
            output.Signature = input.Signature;
            output.StatusCode = input.StatusCode;
            output.SubTotalAmount = input.SubTotalAmount;
            output.TipAmount = input.TipAmount;
            output.TotalAmount = input.TotalAmount;
            output.TransactionId = input.TransactionId;



            return output;

        }

        public static SaleResponse2 ConvertToSale(RefundResponse input)
        {
            SaleResponse2 output = new SaleResponse2();

            output.AccountNumber = input.AccountNumber;
            output.ApprovalNumber = input.ApprovalNumber;
      
            output.CardHolderName = input.CardHolderName;
            output.CardLogo = input.CardLogo;
      
            output.Emv = input.Emv;
            output.EntryMode = input.EntryMode;
            output.PaymentType = input.PaymentType;
            output.PinVerified = input.PinVerified;
            output.ProcessorResponse = input.ProcessorResponse;
            output.Signature = input.Signature;
            output.StatusCode = input.StatusCode;
          
         
            output.TotalAmount = input.TotalAmount;
            output.ApprovedAmount = input.TotalAmount;  //copy over so database record can use this field
            output.SubTotalAmount = input.TotalAmount; //copy over so database record can use this field


            output.TransactionId = input.TransactionId;



            return output;

        }


        public static SaleResponse2 ConvertToSale(Item input)
        {
            SaleResponse2 output = new SaleResponse2();
            TriPOS.ResponseModels.Signature sig=  new TriPOS.ResponseModels.Signature();
            sig.SignatureStatusCode = "SignatureRequired";
            output.AccountNumber = input.CardNumberMasked;
            output.ApprovalNumber = input.ApprovalNumber;
            output.ApprovedAmount = input.ApprovedAmount;
            output.CardHolderName = input.CardHolderName;
            output.CardLogo = input.CardLogo;
            output.CashbackAmount = 0;
            output.Emv = null;
            output.EntryMode = "unknown";
            output.PaymentType = input.CardType;
            output.PinVerified = false;
            output.ProcessorResponse = null;
            output.Signature = sig;
            output.StatusCode = input.StatusCode;
            output.SubTotalAmount = input.ApprovedAmount - input.TipAmount;
            output.TipAmount = input.TipAmount;
            output.TotalAmount = input.ApprovedAmount; ;
            output.TransactionId = input.TransactionId;



            return output;

        }




    }
}
