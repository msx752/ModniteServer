using Newtonsoft.Json;

namespace ModniteServer.API.Controllers
{
    public sealed class WaitingRoomController : Controller
    {
        /// <summary>
        /// Checks the queue to see if there's room for the client to join.
        /// </summary>
        [Route("GET", "/waitingroom/api/waitingroom")]
        public void CheckWaitQueue()
        {
            // There's no reason to have a wait queue in our case, but you can replace this with
            // your own logic if you want to limit the number of players on your server.
            bool hasQueue = false;

            if (hasQueue)
            {
                var response = new
                {
                    retryTime = 10,    // Seconds to wait before re-checking this API.
                    expectedWait = 10  // Estimated time (in seconds) to display to the client
                };

                Response.StatusCode = 200;
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(response));
            }
            else
            {
                // No content means no queue. The client is free to continue.
                Response.StatusCode = 204;
            }
        }

        /// <summary>
        /// Serves a static json with the queue info to the client. Normally this is hosted on AWS,
        /// but we don't want the client to query any URL outside our server. Occasionally, the
        /// client will ask for this file and we're providing it because it's better than a 404.
        /// </summary>
        [Route("GET", "/launcher-resources/waitingroom/Fortnite/retryconfig.json")]
        [Route("GET", "/launcher-resources/waitingroom/retryconfig.json")]
        public void GetRetryConfig()
        {
            var response = new
            {
                maxRetryCount = 2,
                retryInterval = 60,
                retryJitter = 10,
                failAction = "ABORT" // ABORT | CONTINUE
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }
    }
}