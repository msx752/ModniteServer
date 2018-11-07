using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ModniteServer.API.Controllers
{
    public sealed class TournamentController : Controller
    {
        [Route("GET", "/fortnite/api/game/v2/events/tournamentandhistory/*/*/*")]
        public void GetTournaments()
        {
            string accountId = Request.Url.Segments[Request.Url.Segments.Length - 3].Replace("/", "");
            string region = Request.Url.Segments[Request.Url.Segments.Length - 2].Replace("/", "");
            string clientType = Request.Url.Segments.Last();

            var response = new
            {
                eventTournaments = new List<object>(),
                eventWindowHistories = new List<object>()
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }
    }
}