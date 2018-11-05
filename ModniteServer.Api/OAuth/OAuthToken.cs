using System;

namespace ModniteServer.API.OAuth
{
    /// <summary>
    /// Represents an OAuth token.
    /// </summary>
    internal struct OAuthToken
    {
        public OAuthToken(string token, int expiresIn)
        {
            Token = token;
            ExpiresIn = expiresIn;
            ExpiresAt = DateTime.UtcNow.Add(TimeSpan.FromSeconds(expiresIn));
        }

        public string Token { get; }

        public int ExpiresIn { get; }

        public DateTime ExpiresAt { get; }
    }
}