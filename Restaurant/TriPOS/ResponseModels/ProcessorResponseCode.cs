// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessorResponseCode.cs" company="Element Payment Services, Inc., A Vantiv Company">
//   Copyright © 2015 Element Payment Services, Inc., A Vantiv Company. All Rights Reserved.
// </copyright>
// <summary>
//   Codes returned from the processor
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TriPOS.ResponseModels
{
    /// <summary>
    ///     Codes returned from the processor
    /// </summary>
    public enum ProcessorResponseCode
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Approved
        /// </summary>
        Approved = 1,

        /// <summary>
        /// Partially approved
        /// </summary>
        PartialApproval = 2,

        /// <summary>
        /// Declined
        /// </summary>
        Decline = 3,

        /// <summary>
        /// Expired card
        /// </summary>
        ExpiredCard = 4,

        /// <summary>
        /// Duplicate
        /// </summary>
        Duplicate = 5,

        /// <summary>
        ///     The pick up card.
        /// </summary>
        PickUpCard = 6,

        /// <summary>
        /// Referral, call issuer
        /// </summary>
        ReferralCallIssuer = 7,

        /// <summary>
        /// Invalid data
        /// </summary>
        InvalidData = 8,
    }
}