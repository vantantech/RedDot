using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TriPOS.ResponseModels;
using TriPOS.SignatureFiles;

namespace RedDot
{
    public class VirtualPaymentModel
    {

        public SaleResponse2 ExecuteCreditSale(int salesid, decimal amt)
        {
            VirtualPinPad pinpad = new VirtualPinPad("SALE",amt);
            pinpad.Topmost = true;
            pinpad.ShowDialog();


            SaleResponse2 resp = new SaleResponse2();

            resp.Emv = new Emv();

            resp.Signature = new TriPOS.ResponseModels.Signature();
            resp.Signature.SignatureStatusCode = SignatureStatusCode.SignatureRequired.ToString();
            resp.PaymentType = "CREDIT";  //cardgroup
            resp.ApprovalNumber = "A" + Utility.RandomPin(5).ToString();
            resp.CardLogo = "VISA";   //cardtype
            resp.AccountNumber = "9083290433" + Utility.RandomPin(4).ToString();
            resp.EntryMode = pinpad.EntryMode;
            resp.TransactionId = Utility.RandomPin(8).ToString();
            resp.ApprovedAmount = pinpad.TotalAmount;
            resp.CashbackAmount = 0;

            NumPad np = new NumPad("Enter Tip:",false, false,"0");
            np.Topmost = true;
            np.ShowDialog();


            resp.TipAmount =decimal.Parse(np.Amount);
            resp.CardHolderName = "John Doe";

            resp.StatusCode = pinpad.Status;


            return resp;
        }


        public AuthorizationResponse ExecuteCreditAuthorize(int salesid, decimal amount)
        {
            VirtualPinPad pinpad = new VirtualPinPad("SALE", amount);
            pinpad.Topmost = true;
            pinpad.ShowDialog();


            AuthorizationResponse resp = new AuthorizationResponse();
            resp.Emv = new Emv();

            resp.Signature = new TriPOS.ResponseModels.Signature();
            resp.Signature.SignatureStatusCode = SignatureStatusCode.SignatureRequired.ToString();
            resp.PaymentType = "CREDIT";  //cardgroup
            resp.ApprovalNumber = "A" + Utility.RandomPin(5).ToString();
            resp.CardLogo = "VISA";   //cardtype
            resp.AccountNumber = "9083290433" + Utility.RandomPin(4).ToString();
            resp.EntryMode = pinpad.EntryMode;
            resp.TransactionId = Utility.RandomPin(8).ToString();
            resp.ApprovedAmount = pinpad.TotalAmount;
            resp.CashbackAmount = 0;
            resp.TipAmount = 0;
            resp.CardHolderName = "John Doe";

            resp.StatusCode = pinpad.Status;


            return resp;
        }

        public AuthorizationCompletionResponse ExecuteCreditAuthorizeCompletion(string transactionid, decimal amount)
        {
            AuthorizationCompletionResponse resp = new AuthorizationCompletionResponse();
            resp.StatusCode = "APPROVED";
            resp.TransactionId = Utility.RandomPin(8).ToString();
            //resp.ApprovedAmount = amount;
            resp.TotalAmount = amount;
            Thread.Sleep(1000);

            return resp;
        }

        public VoidResponse ExecuteVoid(string transactionid)
        {
            VoidResponse resp = new VoidResponse();
            resp.StatusCode = "APPROVED";
            return resp;
        }

        public ReversalResponse ExecuteReversal(string transactionid, string paymenttype, decimal amount)
        {
            ReversalResponse resp = new ReversalResponse();
            resp.StatusCode = "APPROVED";
            return resp;
        }

        public ReturnResponse ExecuteReturn(string transactionid, string paymenttype, decimal amount)
        {
            ReturnResponse resp = new ReturnResponse();
            resp.StatusCode = "APPROVED";
            return resp;
        }

        public RefundResponse ExecuteRefund(decimal amount)
        {
            RefundResponse resp = new RefundResponse();
            resp.StatusCode = "APPROVED";
            return resp;
        }


        public BatchCloseResponse ExecuteBatch()
        {
            BatchCloseResponse resp = new BatchCloseResponse();
           

            return resp;
        }

    }
}
