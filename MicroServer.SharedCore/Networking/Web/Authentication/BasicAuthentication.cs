﻿using System;

namespace MicroServer.Networking.Web.Authentication
{
    /// <summary>
    /// Basic authentication
    /// </summary>
    public class BasicAuthentication : IAuthenticator
    {
        private readonly string _realm;
        private readonly IAccountService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthentication" /> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="realm">The realm.</param>
        /// <exception cref="System.ArgumentNullException">
        /// userService
        /// or
        /// realm
        /// </exception>
        public BasicAuthentication(IAccountService userService, string realm)
        {
            if (userService == null) throw new ArgumentNullException("userService");
            if (realm == null) throw new ArgumentNullException("realm");
            _userService = userService;
            _realm = realm;
        }

        /// <summary>
        /// Gets authenticator scheme
        /// </summary>
        /// <value></value>
        /// <example>
        /// digest
        /// </example>
        public string Scheme
        {
            get { return "basic"; }
        }

        /// <summary>
        /// Generate a HA1 hash
        /// </summary>
        /// <param name="realm">Realm that the user want to authenticate in</param>
        /// <param name="userName">User Name</param>
        /// <param name="password">Password</param>
        /// /// <para>
        /// A HA1 hash is simply a Md5 encoded string: "UserName:Realm:Password". The quotes should
        /// not be included. Realm is the currently requested Host (as in <c>Request.Headers["host"]</c>).
        /// </para>
        /// <returns></returns>
        private string Encode(string realm, string userName, string password)
        {
            //string toHash = String.Concat(userName, ":", realm, ":", password);

            //KeyedHashAlgorithm crypto = new KeyedHashAlgorithm(KeyedHashAlgorithmType.HMACMD5);
            //byte[] value = crypto.ComputeHash(Encoding.UTF8.GetBytes(toHash));

            //return ByteUtility.GetString(value);

            return null;
        }

        #region IAuthenticator Members

        /// <summary>
        /// Gets name of the authentication scheme
        /// </summary>
        /// <remarks>"BASIC", "DIGEST" etc.</remarks>
        public string AuthenticationScheme
        {
            get { return "basic"; }
        }

        /// <summary>
        /// Create a WWW-Authenticate header
        /// </summary>
        public void CreateChallenge(IHttpContext context)
        {
            context.Response.Headers.Add("WWW-Authenticate", "Basic realm=\"" + _realm + "\"");
            context.Response.StatusCode = Windows.Web.Http.HttpStatusCode.Unauthorized;
            context.Response.ReasonPhrase = "Authentication Failed";
        }

        /// <summary>
        /// Authorize a request.
        /// </summary>
        /// <param name="request">Request being authenticated</param>
        /// <returns>Authenticated user if successful; otherwise null.</returns>
        public string Authenticate(IHttpContext context)
        {
            //string authHeader = context.Request.Headers["Authorization"];
            //if (authHeader == null)
            //    return null;

            //authHeader = authHeader.Replace("Basic ", string.Empty);

            //string decoded = ByteUtility.GetString(Convert.FromBase64String(authHeader));

            //int pos = decoded.IndexOf(':');
            //if (pos == -1)
            //{
            //    Logger.WriteDebug(this, "Invalid basic authentication header, failed to find colon. Header: " + authHeader);
            //    return null;
            //}

            //string password = decoded.Substring(pos + 1, decoded.Length - pos - 1);
            //string userName = decoded.Substring(0, pos);

            //var user = _userService.Lookup(userName, context.Request.Uri);

            //if (user == null)
            //    return null;

            //if (user.Password == null)
            //{
            //    string ha1 = Encode(context.Request.Uri.Host, userName, password);

            //    if (ha1 != user.HA1)
            //    {
            //        Logger.WriteInfo(this, "Incorrect user name or password. User Name: " + user.Username);
            //        return null;
            //    }
            //}
            //else
            //{
            //    if (userName != user.Username || password != user.Password)
            //    {
            //        Logger.WriteInfo(this, "Incorrect user name or password. User Name: " + user.Username);
            //        return null;
            //    }
            //}

            //return user.Username;

            return null;
        }

        #endregion
    }
}