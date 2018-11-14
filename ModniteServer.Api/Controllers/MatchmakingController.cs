using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModniteServer.API.Controllers
{
    public sealed class MatchmakingController : Controller
    {
        /// <summary>
        /// Finds the available sessions for the player to rejoin.
        /// </summary>
        [Route("GET", "/fortnite/api/matchmaking/session/findPlayer/*")]
        public void FindPlayer()
        {
            var response = new List<object>();

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// Gets a matchmaking ticket.
        /// </summary>
        [Route("GET", "/fortnite/api/game/v2/matchmakingservice/ticket/player/*")]
        public void GetTicket()
        {
            string accountId = Request.Url.Segments.Last();

            Query.TryGetValue("bucketId", out string bucketId);
            Query.TryGetValue("player.subregions", out string playerSubregions);
            Query.TryGetValue("player.platform", out string playerPlatform);

            /* Query fields:
             *      partyPlayerIds
             *      bucketId
             *      player.platform
             *      player.subregions
             *      player.option.custom_match_option.CreativeMutator_HealthAndShield:StartingShield
             *      player.option.custom_match_option.CreativeMutator_HealthAndShield:StartingHealth
             *      player.option.custom_match_option.CreativeMutator_ItemDropOverride:DropAllItemsOverride
             *      player.option.custom_match_option.CreativeMutator_FallDamage:FallDamageMultiplier
             *      player.option.custom_match_option.CreativeMutator_Gravity:GravityOverride
             *      player.option.custom_match_option.CreativeMutator_TimeOfDay:TimeOfDayOverride
             *      player.option.custom_match_option.CreativeMutator_NamePlates:DisplayMode
             *      party.WIN
             *      input.KBM
             */

            var payload = new
            {
                playerId = accountId,
                partyPlayerIds = new []
                {
                    accountId
                },
                bucketId = bucketId + ":PC:public:1",
                attributes = new Dictionary<string, object>
                {
                    ["player.subregions"] = playerSubregions,
                    ["player.hasMultipleInputTypes"] = true,
                    ["player.platform"] = playerPlatform,
                    ["player.preferredSubregion"] = playerSubregions.Split(',')[0]
                },
                expireAt = DateTime.Now.AddMinutes(30).ToDateTimeString(),
                nonce = "this_is_a_nonce"
            };

            var response = new
            {
                serviceUrl = "ws://127.0.0.1:60103",
                ticketType = "mms-player",
                playload = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload))),
                signature = "signature" // base64 encoded (sha256?)
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }

        [Route("GET", "/fortnite/api/matchmaking/session/*")]
        public void GetMatchmakingSession()
        {
            string sessionId = Request.Url.Segments.Last();

            Log.Information($"Queried matchmaking session '{sessionId}'");

            // This shows a "Session has expired" error. Not sure why yet...
            var response = new
            {
                ownerId = "owner_id",
                ownerName = "owner_name",
                serverName = "server_name",
                openPrivatePlayers = 5,
                openPublicPlayers = 5,
                sessionSettings = new
                {
                    maxPublicPlayers = 100,
                    maxPrivatePlayers = 100,
                    shouldAdvertise = true,
                    allowJoinInProgress = true,
                    isDedicated = true,
                    usesStats = true,
                    allowInvites = true,
                    usesPresence = false,
                    allowJoinViaPresence = false,
                    allowJoinViaPresenceFriendsOnly = false,
                    buildUniqueId = "build_unique_id",
                    attributes = ""
                }
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }

        // /fortnite/api/matchmaking/session/`id/heartbeat
        // /fortnite/api/matchmaking/session/`id/join?accountId=`accountId
        // /fortnite/api/matchmaking/session/`id/start
        // /fortnite/api/matchmaking/session/`id/stop
        // /fortnite/api/matchmaking/session/`id/invite
        // /fortnite/api/matchmaking/session/`id/players
        // /fortnite/api/matchmaking/session/`id/matchMakingRequest
    }
}