// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpUtility.cs" company="Element Payment Services, Inc., A Vantiv Company">
//   Copyright © 2015 Element Payment Services, Inc., A Vantiv Company. All Rights Reserved.
// </copyright>
// <summary>
//   Contains utility functions that can be used when handling HTTP requests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TriPOS.RequestHeaderFiles
{
    using System;

    public sealed class HttpUtility
    {
        /// <summary>
        /// Extracts the query substring starting with the character
        /// right after '?' and returns it in an HttpValueCollection URL encoded.
        /// </summary>
        /// <param name="query">
        /// The query string from which to extract the substring.
        /// </param>
        /// <returns>
        /// The URL encoded HttpValueCollection of the query substring.
        /// </returns>
        public static HttpValueCollection ParseQueryString(string query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if ((query.Length > 0) && (query[0] == '?'))
            {
                query = query.Substring(1);
            }

            return new HttpValueCollection(query, true);
        }
    }
}