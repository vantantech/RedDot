using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RedDot
{
    public class ResponseEventArgs
    {
        public string Result { get; set; }
        public string ResultText { get; set; }
        public string ResponseText { get; set; }
        public string Response { get; set; }
        public string ResponseId { get; set; }
        public string ResponseCode { get; set; }
        public string MaskedPAN { get; set; }

        public string CardType { get; set; }
        public string ApprovalCode { get; set; }
        public string AuthorizedAmount { get; set; }
        public string TaxAmount { get; set; }
        public string TipAmount { get; set; }
        public string CashbackAmount { get; set; }

        public string CardAcquisition { get; set; }
        public string CardholderName { get; set; }
        public string CardGroup { get; set; }

        public string GatewayRspCode { get; set; }
        public string GatewayRspMsg { get; set; }


        public string PinVerified { get; set; }
        public string SignatureLine { get; set; }
        public string TipAdjustAllowed { get; set; }
        public string EMV_ApplicationName { get; set; }
        public string EMV_Cryptogram { get; set; }
        public string EMV_CryptogramType { get; set; }
        public string EMV_AID { get; set; }

        public string TransType { get; set; }
        

        public ResponseEventArgs(string SIPResponseMessage)
        {
            XDocument XMLDoc = new XDocument();

            XMLDoc = XDocument.Parse(SIPResponseMessage);
            XElement element;


            element = XMLDoc.Root.Element("AuthorizedAmount");
            AuthorizedAmount = (element != null)? element.Value : "";

            element = XMLDoc.Root.Element("ResponseId");
            if(element == null)
            {
                element = XMLDoc.Root.Element("TransactionId");
                ResponseId = (element != null) ? element.Value : "";
            }
            else
            {
                ResponseId = (element != null) ? element.Value : "";
            }
        

            element = XMLDoc.Root.Element("TipAmount");
            TipAmount = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("MaskedPAN");
            MaskedPAN = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("CardType");
            CardType = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("Response");
            Response = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("ResponseText");
            ResponseText = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("Result");
            Result = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("ResultText");
            ResultText = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("CardholderName");
            CardholderName = (element != null) ? element.Value : "";


            //cardgroup is not available in version 3.2
            element = XMLDoc.Root.Element("CardGroup");
            CardGroup = (element != null) ? element.Value : "";

            //for HeartSIP 3.2 , the cardgroup is blank
            if (CardGroup == "")
            {
                if (CardType.ToUpper().Substring(0, 4) == "GIFT") CardGroup = "GIFT";
                else if (CardType.ToUpper().Substring(0, 4) == "DEBI") CardGroup = "DEBIT";
                else CardGroup = "CREDIT";
            }





            element = XMLDoc.Root.Element("ResponseCode");
            ResponseCode = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("CashbackAmount");
            CashbackAmount = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("CardAcquisition");
            CardAcquisition = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("ApprovalCode");
            ApprovalCode = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("GatewayRspMsg");
            GatewayRspMsg = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("GatewayRspCode");
            GatewayRspCode = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("PinVerified");
            PinVerified = (element != null) ? element.Value : "0";

            element = XMLDoc.Root.Element("SignatureLine");
            SignatureLine = (element != null) ? element.Value : "0";

            element = XMLDoc.Root.Element("TipAdjustAllowed");
            TipAdjustAllowed = (element != null) ? element.Value : "0";

            element = XMLDoc.Root.Element("EMV_ApplicationName");
            EMV_ApplicationName = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("EMV_Cryptogram");
            EMV_Cryptogram = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("EMV_CryptogramType");
            EMV_CryptogramType = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("EMV_AID");
            EMV_AID = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("Response");
            TransType = (element != null) ? element.Value : "";

        }
    }


    public class VoidResponseArgs
    {
        public string Result { get; set; }
        public string ResultText { get; set; }
        public string GatewayRspCode { get; set; }
        public string GatewayRspMsg { get; set; }

        public string OrigTransactionId { get; set; }
    }

    public class GiftResponseArgs 
    {
        public string Result { get; set; }
        public string ResultText { get; set; }
        public string ResponseText { get; set; }
        public string Response { get; set; }
        public string ResponseId { get; set; }
        public string ResponseCode { get; set; }
        public string MaskedPAN { get; set; }

        public string CardType { get; set; }
        public string ApprovalCode { get; set; }
        public string AuthorizedAmount { get; set; }
    


        public string CardAcquisition { get; set; }

        public string CardGroup { get; set; }

        public string GatewayRspCode { get; set; }
        public string GatewayRspMsg { get; set; }


        public string PinVerified { get; set; }
        public string AvailableBalance { get; set; }
  


        public string TransType { get; set; }


        public GiftResponseArgs(string SIPResponseMessage)
        {
            XDocument XMLDoc = new XDocument();

            XMLDoc = XDocument.Parse(SIPResponseMessage);
            XElement element;


            element = XMLDoc.Root.Element("AuthorizedAmount");
            AuthorizedAmount = (element != null) ? element.Value : "";


            element = XMLDoc.Root.Element("ResponseId");
            if (element == null)
            {
                element = XMLDoc.Root.Element("TransactionId");
                ResponseId = (element != null) ? element.Value : "";
            }
            else
            {
                ResponseId = (element != null) ? element.Value : "";
            }



            element = XMLDoc.Root.Element("MaskedPAN");
            MaskedPAN = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("CardType");
            CardType = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("Response");
            Response = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("ResponseText");
            ResponseText = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("Result");
            Result = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("ResultText");
            ResultText = (element != null) ? element.Value : "";

      

            element = XMLDoc.Root.Element("CardGroup");
            CardGroup = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("ResponseCode");
            ResponseCode = (element != null) ? element.Value : "";



            element = XMLDoc.Root.Element("CardAcquisition");
            CardAcquisition = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("ApprovalCode");
            ApprovalCode = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("GatewayRspMsg");
            GatewayRspMsg = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("GatewayRspCode");
            GatewayRspCode = (element != null) ? element.Value : "";

            element = XMLDoc.Root.Element("PinVerified");
            PinVerified = (element != null) ? element.Value : "0";

            element = XMLDoc.Root.Element("AvailableBalance");
            AvailableBalance = (element != null) ? element.Value : "";


            element = XMLDoc.Root.Element("Response");
            TransType = (element != null) ? element.Value : "";

        }
    }


}
