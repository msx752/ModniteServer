using ModniteServer.API.Accounts;
using Newtonsoft.Json;
using Serilog;
using System.Linq;

namespace ModniteServer.API.Controllers
{
    /// <summary>
    /// Handles requests for retrieving info on accounts.
    /// </summary>
    public sealed class AccountController : Controller
    {
        /// <summary>
        /// Retrieves basic info for an account.
        /// </summary>
        [Route("GET", "/account/api/public/account/*")]
        public void GetAccountInfo()
        {
            if (!Authorize()) return;

            string accountId = Request.Url.Segments.Last();

            if (!AccountManager.AccountExists(accountId))
            {
                Response.StatusCode = 404;
                return;
            }

            var account = AccountManager.GetAccount(accountId);

            var response = new
            {
                id = account.AccountId,
                displayname = account.DisplayName,
                name = account.FirstName,
                email = account.Email,
                failedLoginAttempts = account.FailedLoginAttempts,
                lastLogin = account.LastLogin.ToDateTimeString(),
                numberOfDisplayNameChanges = account.DisplayNameHistory?.Count ?? 0,
                ageGroup = "UNKNOWN",
                headless = false,
                country = account.Country,
                preferredLanguage = account.PreferredLanguage,

                // We're not supporting 2FA for Modnite.
                tfaEnabled = false
            };

            Log.Information($"Account info retrieved for {accountId} {{AccountInfo}}", response);

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// Gets the linked account info for an account. Since we don't provide the capability of
        /// signing into accounts using Facebook, Google, or whatever, we're just going to provide
        /// the same info as <see cref="GetAccountInfo"/>.
        /// </summary>
        [Route("GET", "/account/api/public/account/*/externalAuths")]
        public void GetExternalAuthInfo()
        {
            if (!Authorize()) return;

            string accountId = Request.Url.Segments[Request.Url.Segments.Length - 2];

            if (!AccountManager.AccountExists(accountId))
            {
                Response.StatusCode = 404;
                return;
            }

            var account = AccountManager.GetAccount(accountId);

            var response = new
            {
                url = "",
                id = account.AccountId,
                externalAuthId = account.AccountId,
                externalDisplayName = account.DisplayName,
                externalId = account.AccountId,
                externalauths = "epic",
                users = new []
                {
                    new
                    {
                        externalAuthId = account.AccountId,
                        externalDisplayName = account.DisplayName,
                        externalId = account.AccountId
                    }
                }
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }
    }
}
