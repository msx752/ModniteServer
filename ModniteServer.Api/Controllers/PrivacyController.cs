using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;

namespace ModniteServer.API.Controllers
{
    public sealed class PrivacyController : Controller
    {
        [Route("GET", "/fortnite/api/game/v2/privacy/account/*")]
        public void GetPrivacySetting()
        {
            string accountId = Request.Url.Segments.Last();

            var response = new
            {
                accountId,
                optOutOfPublicLeaderboards = true
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }

        [Route("POST", "/fortnite/api/game/v2/privacy/account/*")]
        public void SetPrivacySetting()
        {
            string accountId = Request.Url.Segments.Last();

            byte[] buffer = new byte[Request.ContentLength64];
            Request.InputStream.Read(buffer, 0, buffer.Length);

            var request = JObject.Parse(Encoding.UTF8.GetString(buffer));

            bool optOutOfPublicLeaderboards = (bool)request["optOutOfPublicLeaderboards"];
            bool optOutOfFriendsLeaderboards = (bool)request["optOutOfFriendsLeaderboards"];

            var response = new
            {
                accountId,
                optOutOfPublicLeaderboards
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }
    }
}
