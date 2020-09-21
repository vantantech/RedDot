using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.clover.remotepay.sdk;
using com.clover.remotepay.transport;
using com.clover.sdk.v3.payments;
using NLog;
using RedDot;

namespace Clover
{
    public class CloverListener : DefaultCloverConnectorListener
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public Boolean deviceReady { get; set; }
        public Boolean deviceConnected { get; set; }
        public Boolean saleDone { get; set; }
        public Boolean resetDone { get; set; }
        public Boolean refundDone { get; set; }
        public String paymentId { get; set; }
        public String orderId { get; set; }

        public Boolean tipDone { get; set; }

        public Boolean closeoutDone { get; set; }

        public bool voidDone { get; set; }

        public string message { get; set; }
        public string reason { get; set; }

        public ResponseCode result { get; set; }

        public com.clover.sdk.v3.payments.Payment Payment { get; set; }

        public Batch Batch { get; set; }
        public Refund PaymentRefund { get; set; }

        public com.clover.sdk.v3.payments.Credit ManualRefund { get; set; }

        public CloverListener(ICloverConnector cloverConnector) : base(cloverConnector)
        {
        }

        public override void OnDeviceReady(MerchantInfo merchantInfo)
        {
            base.OnDeviceReady(merchantInfo);
            deviceReady = true;
            //Connected and available to process requests
        }

        public override void OnDeviceConnected()
        {
            base.OnDeviceConnected();
            deviceConnected = true;
            // Connected, but not available to process requests

        }

        public override void OnDeviceDisconnected()
        {
            base.OnDeviceDisconnected();
            // Console.WriteLine("Disconnected");
            //Disconnected
            deviceReady = false;
            deviceConnected = false;
        }

        public override void OnSaleResponse(SaleResponse response)
        {
            base.OnSaleResponse(response);
           
            message = response.Message;
            result = response.Result;
            reason = response.Reason;

            if (response.Success)
            {
                paymentId = response.Payment.id;
                orderId = response.Payment.order.id;
              
                // POSPayment payment = new POSPayment(response.Payment.id, response.Payment.externalPaymentId, response.Payment.order.id, response.Payment.employee.id, response.Payment.amount, response.Payment.tipAmount, response.Payment.cashbackAmount);


                Payment = response.Payment;
            }else
            {
                Payment = null;
                paymentId = "";
                
            }
            saleDone = true;

        }

        public override void OnAuthResponse(AuthResponse response)
        {
            base.OnAuthResponse(response);

            saleDone = true;
            message = response.Message;
            result = response.Result;


            if (response.Success)
            {
                paymentId = response.Payment.id;
                orderId = response.Payment.order.id;
                var test = response.Payment.cardTransaction;

                // POSPayment payment = new POSPayment(response.Payment.id, response.Payment.externalPaymentId, response.Payment.order.id, response.Payment.employee.id, response.Payment.amount, response.Payment.tipAmount, response.Payment.cashbackAmount);


                Payment = response.Payment;
            }
            else
            {
                Payment = null;
                paymentId = "";

            }

        }

        public override void OnResetDeviceResponse(ResetDeviceResponse response)
        {
            base.OnResetDeviceResponse(response);
            resetDone = true;
            message = response.State.ToString();
        }


        public override void OnTipAdjustAuthResponse(TipAdjustAuthResponse response)
        {
            base.OnTipAdjustAuthResponse(response);
            tipDone = true;
            message = response.Message;
            result = response.Result;

            if (response.Success)
            {
             
                paymentId = response.PaymentId;
              

                // POSOrder order = Store.GetOrder(response.PaymentId);
                // order.ModifyTipAmount(response.PaymentId, response.TipAmount);
            }
          
        }


        public override void OnTipAdded(TipAddedMessage message)
        {
            base.OnTipAdded(message);

            if (message.tipAmount > 0)
            {
                string msg = "Tip Added: " + (message.tipAmount / 100.0).ToString("C2");
                OnDeviceActivityStart(new CloverDeviceEvent(0, msg));
            }
        }




        public override void OnConfirmPaymentRequest(ConfirmPaymentRequest request)
        {

        }

        public override void OnManualRefundResponse(ManualRefundResponse response)
        {
            base.OnManualRefundResponse(response);
         

                message = response.Message;
                result = response.Result;
                reason = response.Reason;

                if (response.Success)
                {
                    ManualRefund = response.Credit;
                    paymentId = response.Credit.id;
                orderId = response.Credit.orderRef.id;

                
                }
                else
                {
                    ManualRefund = null;
                    paymentId = "";
                    orderId = "";

                }
          
        

            refundDone = true;

        }

        public override void OnVoidPaymentResponse(VoidPaymentResponse response)
        {
            base.OnVoidPaymentResponse(response);

            message = response.Message;
            result = response.Result;
            reason = response.Reason;

            if (response.Success)
            {
                paymentId = response.PaymentId;
            }
            else
            {
                paymentId = "";
                orderId = "";
            }


            voidDone = true;

        }



        public override void OnCloseoutResponse(CloseoutResponse response)
        {
            base.OnCloseoutResponse(response);


            reason = response.Reason;
            result = response.Result;
            message = response.Message;
            Batch = response.Batch;
            if (response != null && response.Success)
            {
               

            }
            if (response != null && response.Result.Equals(ResponseCode.FAIL))
            {
                Batch = null;

            }

            closeoutDone = true;
        }


    }
}
