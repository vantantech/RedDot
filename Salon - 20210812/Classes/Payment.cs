using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class Payment
    {

        public string CardGroup {get; set;}

        public decimal Amount {get; set;}
        public decimal NetAmount { get; set; }
        public decimal TipAmount { get; set; }
        public int ID { get; set; }
        public string AuthorCode { get; set; }

        public DateTime PaymentDate { get; set; }
        public DateTime VoidDate { get; set; }

        public string CardType { get; set; }

        public string MaskedPAN { get; set; }

        public string ResponseId { get; set; }
        public string CloverPaymentId { get; set; }
        public string CloverOrderId { get; set; }

        public string CardAcquisition { get; set; }

        public string TransType { get; set; }

        public decimal CashbackAmount { get; set; }

        public bool Voided { get; set; }

        public int SignatureLine { get; set; }
        public int PinVerified { get; set; }

        public string EMV_ApplicationName { get; set; }
        public string EMV_Cryptogram { get; set; }

        public string EMV_CryptogramType { get; set; }

        public string EMV_AID { get; set; }

        public string CardHolderName { get; set; }

        public bool TipAdustAllowed { get; set; }



        public string AmountStr {
          get {
                if (Voided)
                {
                    return "**";
                }
                else
                    switch (CardGroup.ToUpper())
                    {
                        case "CASH":
                 
                        case "DEBIT":
                        case "GIFTCARD":
                        case "GIFTCERTIFICATE":
                            return String.Format("{0:0.00}", Amount);


                   
                        case "CREDIT":
                        case "STAMP CARD":
                            return String.Format("{0:0.00}", NetAmount);


                        default:
                            return String.Format("{0:0.00}", Amount);
                    }
               
          } 

        }

      public string ReceiptStr
      {
          get
          {
                var m_cardtype = CardType;

                if (m_cardtype == "") m_cardtype = CardGroup;

                if (TransType.ToUpper() == "REFUND") m_cardtype =m_cardtype + " REFUND"; 

                if(Voided)
                {
                    return Utility.FormatPrintRow(m_cardtype + "(VOIDED) ", "**", 35);
                }
                else
                {
                   return Utility.FormatPrintRow(m_cardtype + " " + AuthorCode, AmountStr, 35);
                }
           
            }

      }


        public string CardTypeStr
        {
            get
            {
                if (CardType == "") return CardGroup;
                else return CardType;
            }
        }

      public string ReceiptDateStr
      {
          get
          {
             
              
                return Utility.FormatPrintRow(PaymentDate.ToShortDateString() + " " +  ReceiptStr, AmountStr, 28);
          }

      }
        public Payment(DataRow row)
        {
            //ints
            ID = int.Parse(row["id"].ToString());

            //strings
            CardGroup = row["cardgroup"].ToString();   //same as cardgroup
            AuthorCode = row["authorcode"].ToString();
            CardType = row["cardtype"].ToString();
            MaskedPAN = row["maskedpan"].ToString();
            CardAcquisition = row["cardacquisition"].ToString();
            ResponseId = row["responseid"].ToString();
            CloverPaymentId = row["custom1"].ToString();
            CloverOrderId = row["custom2"].ToString();
            TransType = row["transtype"].ToString();
            EMV_ApplicationName = row["emv_applicationname"].ToString();
            EMV_AID = row["emv_aid"].ToString();
            EMV_Cryptogram = row["emv_cryptogram"].ToString();
            EMV_CryptogramType = row["emv_cryptogramtype"].ToString();
            CardHolderName = row["cardholdername"].ToString();

            //booleans
            Voided = Convert.ToBoolean(row["void"]);
      
            TipAdustAllowed = Convert.ToBoolean(row["tipadjustallowed"]);


            //decimals
            if (row["cashbackamount"].ToString() != "") CashbackAmount = (decimal)row["cashbackamount"]; else CashbackAmount = 0;
            if (row["amount"].ToString() != "") Amount = (decimal)row["amount"]; else Amount = 0;
            if (row["netamount"].ToString() != "") NetAmount = (decimal)row["netamount"]; else NetAmount = 0;
            if (row["tipamount"].ToString() != "") TipAmount = (decimal)row["tipamount"]; else TipAmount = 0;


            if (row["pinverified"].ToString() != "") PinVerified= int.Parse(row["pinverified"].ToString()); else PinVerified = 0;
            if (row["signatureline"].ToString() != "") SignatureLine = int.Parse(row["signatureline"].ToString()); else SignatureLine = 0;
         


            if (row["paymentdate"].ToString() != "") PaymentDate = (DateTime)row["paymentdate"];

            if (row["voiddate"].ToString() != "") VoidDate = (DateTime)row["voiddate"];

        }
    }


}
