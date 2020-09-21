// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignatureStatusCode.cs" company="Element Payment Services, Inc., A Vantiv Company">
//   Copyright © 2015 Element Payment Services, Inc., A Vantiv Company. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TriPOS.SignatureFiles
{
    /// <summary>
    /// Status codes for signatures
    /// </summary>
    public enum SignatureStatusCode
    {
        /// <summary>
        ///     The unknown.
        /// </summary>
        Unknown = 0, 

        /// <summary>
        ///     The signature required.
        /// </summary>
        SignatureRequired = 1, 

        /// <summary>
        ///     The signature present.
        /// </summary>
        SignaturePresent = 2, 

        /// <summary>
        ///     The signature required cancelled by cardholder.
        /// </summary>
        SignatureRequiredCancelledByCardholder = 100, 

        /// <summary>
        ///     The signature required not supported by pin pad.
        /// </summary>
        SignatureRequiredNotSupportedByPinPad = 101, 

        /// <summary>
        ///     The signature required pin pad error.
        /// </summary>
        SignatureRequiredPinPadError = 102, 

        /// <summary>
        ///     The signature not required by threshold amount.
        /// </summary>
        SignatureNotRequiredByThresholdAmount = 200, 

        /// <summary>
        ///     The signature not required by payment type.
        /// </summary>
        SignatureNotRequiredByPaymentType = 201, 

        /// <summary>
        ///     The signature not required by transaction type.
        /// </summary>
        SignatureNotRequiredByTransactionType = 202
    }
}