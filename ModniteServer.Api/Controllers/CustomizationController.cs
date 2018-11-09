using ModniteServer.API.Accounts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModniteServer.API.Controllers
{
    public sealed class CustomizationController : Controller
    {
        /// <summary>
        /// Updates the player's equipped item choice.
        /// </summary>
        [Route("POST", "/fortnite/api/game/v2/profile/*/client/EquipBattleRoyaleCustomization")]
        public void EquipBattleRoyaleCustomization()
        {
            string accountId = Request.Url.Segments[Request.Url.Segments.Length - 3].Replace("/", "");

            string profileId = Query["profileId"];
            int rvn = Convert.ToInt32(Query["rvn"]);

            byte[] buffer = new byte[Request.ContentLength64];
            Request.InputStream.Read(buffer, 0, buffer.Length);

            var request = JObject.Parse(Encoding.UTF8.GetString(buffer));
            string slotName = (string)request["slotName"];
            string itemToSlot = (string)request["itemToSlot"];
            int indexWithinSlot = (int)request["indexWithinSlot"];
            var variantUpdates = new List<object>(); // TODO: variantUpdates

            string name = "";
            switch (slotName)
            {
                case "Character":
                    name = "favorite_character";
                    break;

                case "Dance":
                    name = "favorite_dance";
                    break;

                case "Backpack":
                    name = "favorite_backpack";
                    break;

                case "Pickaxe":
                    name = "favorite_pickaxe";
                    break;

                case "Glider":
                    name = "favorite_glider";
                    break;

                case "SkyDiveContrail":
                    name = "favorite_skydivecontrail";
                    break;

                case "MusicPack":
                    name = "favorite_musicpack";
                    break;

                case "LoadingScreen":
                    name = "favorite_loadingscreen";
                    break;

                // TODO: add support for sprays

                default:
                    Log.Error($"'{accountId}' tried to update unknown item slot '{slotName}' in profile '{profileId}'");
                    Response.StatusCode = 500;
                    return;
            }

            object profileChange;
            Account account = AccountManager.GetAccount(accountId);

            if (account.EquippedItems == null)
                account.EquippedItems = ApiConfig.Current.EquippedItems; // updates previous accounts that don't have that value

            if (name == "favorite_dance")
            {
                // Dances use an array
                string dance = "favorite_dance" + indexWithinSlot;
                account.EquippedItems[dance] = itemToSlot;
                string[] slots = new string[6];
                for (int i = 0; i < slots.Length; i++)
                {
                    slots[i] = account.EquippedItems["favorite_dance" + i];
                }
                slots[indexWithinSlot] = itemToSlot;
                profileChange = new
                {
                    changeType = "statModified",
                    name,
                    value = slots
                };
            }
            else
            {
                account.EquippedItems[name] = itemToSlot; // add it to equipped items
                profileChange = new
                {
                    changeType = "statModified",
                    name,
                    value = itemToSlot
                };
            }

            var response = new
            {
                profileRevision = rvn + 1,
                profileId,
                profileChangesBaseRevision = rvn,
                profileChanges = new List<object>
                {
                    profileChange
                }
            };

            // TODO: update in Account model
            Log.Information($"'{accountId}' changed favorite {slotName}");

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// Updates the player's banner.
        /// </summary>
        [Route("POST", "/fortnite/api/game/v2/profile/*/client/SetBattleRoyaleBanner")]
        public void EquipBanner()
        {
            string accountId = Request.Url.Segments[Request.Url.Segments.Length - 3].Replace("/", "");

            string profileId = Query["profileId"];
            int rvn = Convert.ToInt32(Query["rvn"]);

            byte[] buffer = new byte[Request.ContentLength64];
            Request.InputStream.Read(buffer, 0, buffer.Length);

            var request = JObject.Parse(Encoding.UTF8.GetString(buffer));

            // TODO: return entire athena profile data
            Response.StatusCode = 500;
        }
    }
}