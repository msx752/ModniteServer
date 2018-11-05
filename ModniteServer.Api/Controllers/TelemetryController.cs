using Serilog;
using System.Text;

namespace ModniteServer.API.Controllers
{
    public sealed class TelemetryController : Controller
    {
        [Route("POST", "/datarouter/api/v1/public/data")]
        public void ReceiveEvents()
        {
            Query.TryGetValue("SessionID", out string sessionId);
            Query.TryGetValue("AppID", out string appId);
            Query.TryGetValue("AppVersion", out string appVersion);
            Query.TryGetValue("UserID", out string userId);
            Query.TryGetValue("AppEnvironment", out string appEnvironment);
            Query.TryGetValue("UploadType", out string uploadType);

            byte[] body = new byte[Request.ContentLength64];
            Request.InputStream.Read(body, 0, body.Length);
            string bodyText = Encoding.UTF8.GetString(body);

            Log.Information("Telemetry data received {SessionId}{AppId}{AppVersion}{UserId}{AppEnvironment}{UploadType}{Data}", sessionId, appId, appVersion, userId, appEnvironment, uploadType, bodyText);

            Response.StatusCode = 200;
        }
    }
}
