using Newtonsoft.Json;
using System.Collections.Generic;

namespace ModniteServer.API.Controllers
{
    public sealed class MatchmakingController : Controller
    {
        /// <summary>
        /// Finds the available sessions for the player to rejoin.
        /// </summary>
        [Route("GET", "/fortnite-gameapi/fortnite/api/matchmaking/session/findPlayer/*")]
        public void FindPlayer()
        {
            var response = new List<object>();

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }
    }
}