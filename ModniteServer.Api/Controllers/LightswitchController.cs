using Newtonsoft.Json;

namespace ModniteServer.API.Controllers
{
    public sealed class LightswitchController : Controller
    {
        /// <summary>
        /// Checks the status of Fortnite services. In our case, we're always up when the server is. ;)
        /// </summary>
        [Route("GET", "/lightswitch/api/service/bulk/status")]
        public void GetStatus()
        {
            var response = new[]
            {
                new
                {
                    serviceInstanceId = "fortnite",
                    status = "UP",
                    message = "Down for maintenance",
                    maintenanceUrl = (string)null,
                    overrideCatalogIds = new string[0],
                    allowedActions = new string[0],
                    banned = false, // we check for this in OAuthController
                    launcherInfoDTO = new
                    {
                        appName = "Fortnite",
                        catalogItemId = "",
                        @namespace = "fn"
                    }
                }
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }
    }
}