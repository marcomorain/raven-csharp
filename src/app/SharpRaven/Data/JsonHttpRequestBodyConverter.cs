﻿#region License

// Copyright (c) 2014 The Sentry Team and individual contributors.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
//     1. Redistributions of source code must retain the above copyright notice, this list of
//        conditions and the following disclaimer.
//
//     2. Redistributions in binary form must reproduce the above copyright notice, this list of
//        conditions and the following disclaimer in the documentation and/or other materials
//        provided with the distribution.
//
//     3. Neither the name of the Sentry nor the names of its contributors may be used to
//        endorse or promote products derived from this software without specific prior written
//        permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpRaven.Data
{
    /// <summary>
    /// HTTP media type converter for JSON.
    /// </summary>
    public class JsonHttpRequestBodyConverter : IHttpRequestBodyConverter
    {
        /// <summary>
        /// Checks whether the specified <paramref name="contentType"/> can be converted by this
        /// <see cref="IHttpRequestBodyConverter"/> implementation or not.
        /// </summary>
        /// <param name="contentType">The media type to match.</param>
        /// <returns>
        /// Returns <c>true</c> if the <see cref="IHttpRequestBodyConverter"/> implementation can convert
        /// the specified <paramref name="contentType"/> cref="contentType"/>; otherwise <c>false</c>.
        /// </returns>
        public bool Matches(string contentType)
        {
            if (String.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var mimeType = contentType.Split(';').First();

            return mimeType.Equals("application/json", StringComparison.OrdinalIgnoreCase) ||
                mimeType.StartsWith("application/json-", StringComparison.OrdinalIgnoreCase) ||
                mimeType.Equals("text/json", StringComparison.OrdinalIgnoreCase) ||
                mimeType.EndsWith("+json", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Tries to convert the HTTP request body of the specified <paramref name="httpContext"/> to
        /// a structured type.
        /// </summary>
        /// <param name="httpContext">The HTTP context containing the request body to convert.</param>
        /// <param name="converted">
        /// The converted, structured type for the specified <paramref name="httpContext"/>'s request
        /// body or <c>null</c> if the <paramref name="httpContext"/> is null, or the somehow conversion
        /// fails.
        /// </param>
        /// <returns>
        /// <c>true</c> if the conversion succeeds; otherwise <c>false</c>.
        /// </returns>
        public bool TryConvert(dynamic httpContext, out object converted)
        {
            converted = null;

            if (httpContext == null)
            {
                return false;
            }

            try
            {
                string body = null;

                using (var stream = new MemoryStream())
                {
                    httpContext.Request.InputStream.Seek(0, SeekOrigin.Begin);
                    httpContext.Request.InputStream.CopyTo(stream);
                    body = Encoding.UTF8.GetString(stream.ToArray());
                }

                converted = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return false;
        }
    }
}
