using ModniteServer.API.Accounts;
using ModniteServer.API.OAuth;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ModniteServer.API.Controllers
{
    /// <summary>
    /// Handles requests for generating OAuth tokens. This implementation is good enough for a small
    /// server where you know everybody. You should rewrite this class if you want a solidly secure
    /// authentication system.
    /// </summary>
    public sealed class OAuthController : Controller
    {
        public const string FortniteClientId = "ec684b8c687f479fadea3cb2ad83f5c6:e1f31c211f28413186262d37a13fc84d";

        public static readonly TimeSpan ClientAccessTokenExpiry = TimeSpan.FromHours(4);
        public static readonly TimeSpan UserAccessTokenExpiry = TimeSpan.FromHours(8);

        /// <summary>
        /// Generates an OAuth token when provided with valid credentials.
        /// </summary>
        [Route("POST", "/account/api/oauth/token")]
        public void GenerateToken()
        {
            Query.TryGetValue("grant_type", out string grantType);
            switch (grantType)
            {
                case "client_credentials":
                    GenerateTokenFromClientId();
                    return;

                case "password":
                    GenerateTokenFromCredentials();
                    return;

                // Exchange code is used if the user had logged in via the Epic Games Launcher.
                // However, we don't have a launcher so there is no reason to support this.
                case "exchange_code":
                default:
                    Response.StatusCode = 403;
                    return;
            }
        }

        /// <summary>
        /// Validates the provided token.
        /// </summary>
        [Route("GET", "/account/api/oauth/verify")]
        public void VerifyToken()
        {
            if (Authorize())
            {
                Response.StatusCode = 204;
            }
            else
            {
                Response.StatusCode = 403;
            }
        }

        /// <summary>
        /// Invalidates the provided token.
        /// </summary>
        [Route("DELETE", "/account/api/oauth/sessions/kill/*")]
        public void KillSession()
        {
            string token = Request.Url.Segments.Last();
            OAuthManager.DestroyToken(token);
            Response.StatusCode = 204;
        }

        /// <summary>
        /// Generates an OAuth token for an user from their credentials.
        /// </summary>
        private void GenerateTokenFromCredentials()
        {
            bool hasValidAuth = false;

            string email = Query["username"];
            string password = Query["password"];

            string passwordHash;
            using (var sha256 = new SHA256Managed())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashString = new StringBuilder();
                foreach (byte b in hash)
                {
                    hashString.AppendFormat("{0:x2}", b);
                }
                passwordHash = hashString.ToString();
            }

            Account account = null;
            if (AccountManager.AccountExists(email))
            {
                account = AccountManager.GetAccount(email);
                if (account.PasswordHash == passwordHash)
                {
                    if (!account.IsBanned)
                    {
                        Log.Information($"{account.DisplayName} logged in {{DisplayName}}{{AccountId}}", account.DisplayName, account.AccountId);
                        hasValidAuth = true;
                    }
                    else
                    {
                        Log.Information($"{account.DisplayName} tried to log in but was banned {{DisplayName}}{{AccountId}}", account.DisplayName, account.AccountId);
                    }

                    account.LastLogin = DateTime.UtcNow;
                }
            }
            else if (ApiConfig.Current.AutoCreateAccounts)
            {
                account = AccountManager.CreateAccount(email, passwordHash);
                hasValidAuth = true;
            }

            if (hasValidAuth)
            {
                var token = OAuthManager.CreateToken((int)UserAccessTokenExpiry.TotalSeconds);

                var response = new
                {
                    access_token = token.Token,
                    expires_in = token.ExpiresIn,
                    expires_at = token.ExpiresAt.ToDateTimeString(),
                    token_type = "bearer",
                    refresh_token = token.Token, // I know, I know...
                    refresh_expires = token.ExpiresIn,
                    refresh_expires_at = token.ExpiresAt.ToDateTimeString(),
                    account_id = account.AccountId,
                    client_id = FortniteClientId.Split(':')[0],
                    internal_client = true,
                    client_service = "fortnite",
                    app = "fortnite",
                    in_app_id = account.AccountId
                };

                Response.StatusCode = 200;
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(response));
            }
            else
            {
                Response.StatusCode = 403;
            }
        }

        /// <summary>
        /// Generates an OAuth token for the client (not user).
        /// </summary>
        private void GenerateTokenFromClientId()
        {
            bool hasValidAuth = false;

            string authorization = Request.Headers["Authorization"];
            if (authorization != null)
            {
                if (authorization.StartsWith("basic "))
                {
                    string token = authorization.Split(' ')[1];
                    token = Encoding.ASCII.GetString(Convert.FromBase64String(token));

                    if (token == FortniteClientId)
                    {
                        hasValidAuth = true;
                    }
                }
            }

            if (hasValidAuth)
            {
                var token = OAuthManager.CreateToken((int)ClientAccessTokenExpiry.TotalSeconds);

                var response = new
                {
                    access_token = token.Token,
                    expires_in = token.ExpiresIn,
                    expires_at = token.ExpiresAt.ToDateTimeString(),
                    token_type = "bearer",
                    client_id = FortniteClientId.Split(':')[0],
                    internal_client = true,
                    client_service = "fortnite"
                };

                Response.StatusCode = 200;
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(response));
            }
            else
            {
                Response.StatusCode = 403;
            }
        }
    }
}