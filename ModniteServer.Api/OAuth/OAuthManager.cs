using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ModniteServer.API.OAuth
{
    // TODO: Save sessions to a file

    /// <summary>
    /// Creates, manages, and destroys OAuth tokens.
    /// </summary>
    internal static class OAuthManager
    {
        static OAuthManager()
        {
            Tokens = new Dictionary<string, OAuthToken>();
        }

        private static Dictionary<string, OAuthToken> Tokens { get; set; }

        /// <summary>
        /// Checks whether the given token is valid and has not expired.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <returns><c>true</c> if the token is valid; Otherwise, <c>false</c>.</returns>
        public static bool IsTokenValid(string token)
        {
            if (!Tokens.ContainsKey(token))
                return false;

            return Tokens[token].ExpiresAt > DateTime.UtcNow;
        }

        /// <summary>
        /// Invalidates the given token.
        /// </summary>
        /// <param name="token">The token to invalidate.</param>
        public static void DestroyToken(string token)
        {
            Tokens.Remove(token);
        }

        /// <summary>
        /// Creates a new token with the specified expiration time.
        /// </summary>
        /// <param name="expiresInSeconds">The duration (in seconds) this token will be valid for.</param>
        /// <returns>A valid OAuth token.</returns>
        public static OAuthToken CreateToken(int expiresInSeconds)
        {
            byte[] tokenRaw = new byte[20];

            using (var rng = new RNGCryptoServiceProvider())
                rng.GetNonZeroBytes(tokenRaw);

            string tokenStr = BitConverter.ToString(tokenRaw).Replace("-", "").ToLowerInvariant();
            var token = new OAuthToken(tokenStr, expiresInSeconds);
            Tokens.Add(tokenStr, token);
            return token;
        }
    }
}