using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;
using System.Windows;

namespace RedDot
{
    public class AuthorizeDotNet
    {


        private string _apiloginid;
        private string _apitransactionkey;
        private string _returncode;

        public string ReturnCode { get { return _returncode; } }

        public AuthorizeDotNet(String ApiLoginID, String ApiTransactionKey)
        {
            _apiloginid = ApiLoginID;
            _apitransactionkey = ApiTransactionKey;


        }
        public  bool Authorize(string cardno, string exp , decimal amt)
        {
            
     

          var creditCard = new creditCardType
            {
                cardNumber = cardno,
                expirationDate = exp
            };

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = _apiloginid,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = _apitransactionkey,
            };



            //standard api call to retrieve response
            var paymentType = new paymentType { Item = creditCard };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authOnlyTransaction.ToString(),    // authorize only
                amount =amt,
                payment = paymentType
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the contoller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            //validate
            if (response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response.transactionResponse != null)
                {
                    _returncode = response.transactionResponse.authCode;
                    return true;
                }
                else
                {
                    _returncode = "error";
                    return false;
                }
            }
            else
            {
               // MessageBox.Show("Error: " + response.messages.message[0].code + "  " + response.messages.message[0].text);
                if (response.transactionResponse != null)
                {
                  // MessageBox.Show("Transaction Error : " + response.transactionResponse.errors[0].errorCode + " " + response.transactionResponse.errors[0].errorText);
                   _returncode = response.transactionResponse.errors[0].errorCode;
                }

                return false;
            }

        }



 
    }
}
