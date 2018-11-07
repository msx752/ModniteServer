using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ModniteServer.API.Controllers
{
    public sealed class FriendsController : Controller
    {
        [Route("GET", "/friends/api/public/friends/*")]
        public void GetFriends()
        {
            string accountId = Request.Url.Segments.Last();

            var response = new List<object>();

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }

        [Route("GET", "/friends/api/public/blocklist/*")]
        public void GetBlocklist()
        {
            string accountId = Request.Url.Segments.Last();

            var response = new
            {
                blockedUsers = new List<object>()
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }

        [Route("GET", "/friends/api/public/list/fortnite/*/recentPlayers")]
        public void GetRecentPlayers()
        {
            string accountId = Request.Url.Segments[Request.Url.Segments.Length - 2].Replace("/", "");

            var response = new List<object>();

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }

        [Route("GET", "/friends/api/v1/*/settings")]
        public void GetSettings()
        {
            string accountId = Request.Url.Segments[Request.Url.Segments.Length - 2].Replace("/", "");

            var response = new
            {
                acceptInvites = "public"
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }

        [Route("GET", "/account/api/public/account/displayName/*")]
        public void FindFriendByDisplayName()
        {
            string displayName = Request.Url.Segments.Last();
            string type = Request.Url.Segments[Request.Url.Segments.Length - 2].Replace("/", "");

            var response = new
            {
                errorCode = "errors.com.epicgames.account.account_not_found",
                errorMessage = "Sorry, we couldn't find an account for " + displayName,
                messageVars = new List<string> { displayName },
                numericErrorCode = 18007,
                originatingService = "com.epicgames.account.public",
                intent = "prod"
            };

            Response.StatusCode = 404;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }

        [Route("GET", "/account/api/public/account/email/*")]
        public void FindFriendByEmail()
        {
            string email = Request.Url.Segments.Last();

            var response = new
            {
                errorCode = "errors.com.epicgames.account.account_not_found",
                errorMessage = "Sorry, we couldn't find an account for " + email,
                messageVars = new List<string> { email },
                numericErrorCode = 18007,
                originatingService = "com.epicgames.account.public",
                intent = "prod"
            };

            Response.StatusCode = 404;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }
    }
}