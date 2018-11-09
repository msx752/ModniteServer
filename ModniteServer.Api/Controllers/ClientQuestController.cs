using ModniteServer.API.Accounts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ModniteServer.API.Controllers.Profile
{
    public sealed class ClientQuestController : Controller
    {
        [Route("POST", "/fortnite/api/game/v2/profile/*/client/ClientQuestLogin")]
        public void ClientQuestLogin()
        {
            // TODO: Remove duplicate profile code in this and QueryProfile endpoints.

            // Note: Quests are items in the 'athena' profile.

            string accountId = Request.Url.Segments[Request.Url.Segments.Length - 3].Replace("/", "");

            if (!AccountManager.AccountExists(accountId))
            {
                Response.StatusCode = 404;
                return;
            }

            var account = AccountManager.GetAccount(accountId);

            Query.TryGetValue("profileId", out string profileId);
            Query.TryGetValue("rvn", out string rvn);
            int revision = Convert.ToInt32(rvn ?? "-2");

            var items = account.AthenaItems;
            var itemsFormatted = new Dictionary<string, object>();
            foreach (string item in items)
            {
                var itemGuid = item; // Makes life easier - config file doesn't store numbers anymore, and it can be read anywhere.
                itemsFormatted.Add(itemGuid, new
                {
                    templateId = item,
                    attributes = new
                    {
                        max_level_bonus = 0,
                        level = 1,
                        item_seen = 1,
                        xp = 0,
                        variants = new List<object>(),
                        favorite = false
                    },
                    quantity = 1
                });
            }

            string[] dances = new string[6];
            for (int i = 0; i < dances.Length; i++)
            {
                dances[i] = account.EquippedItems["favorite_dance" + i];
            }

            var response = new
            {
                profileRevision = 10,
                profileId = "athena",
                profileChangesBaseRevision = 8,
                profileChanges = new List<object>
                {
                    new
                    {
                        changeType = "fullProfileUpdate",
                        profile = new
                        {
                            _id = accountId, // not really account id but idk
                            created = DateTime.Now.AddDays(-7).ToDateTimeString(),
                            updated = DateTime.Now.AddDays(-1).ToDateTimeString(),
                            rvn = 10,
                            wipeNumber = 5,
                            accountId,
                            profileId = "athena",
                            version = "fortnitemares_part4_fixup_oct_18",
                            items = itemsFormatted,
                            stats = new
                            {
                                attributes = new
                                {
                                    past_seasons = new string[0],
                                    season_match_boost = 1000,
                                    favorite_victorypose = "",
                                    mfa_reward_claimed = false,
                                    quest_manager = new
                                    {
                                        dailyLoginInterval = DateTime.Now.AddDays(1).ToDateTimeString(),
                                        dailyQuestRerolls = 1
                                    },
                                    book_level = 0,
                                    season_num = 0,
                                    favorite_consumableemote = "",
                                    banner_color = "defaultcolor1",
                                    favorite_callingcard = "",
                                    favorite_character = account.EquippedItems["favorite_character"],
                                    favorite_spray = new string[0],
                                    book_xp = 0,
                                    favorite_loadingscreen =  account.EquippedItems["favorite_loadingscreen"],
                                    book_purchased = false,
                                    lifetime_wins = 0,
                                    favorite_hat = "",
                                    level = 1000000,
                                    favorite_battlebus = "",
                                    favorite_mapmarker = "",
                                    favorite_vehicledeco = "",
                                    accountLevel = 1000000,
                                    favorite_backpack = account.EquippedItems["favorite_backpack"],
                                    favorite_dance = dances,
                                    inventory_limit_bonus = 0,
                                    favorite_skydivecontrail = account.EquippedItems["favorite_skydivecontrail"],
                                    favorite_pickaxe = account.EquippedItems["favorite_pickaxe"],
                                    favorite_glider = account.EquippedItems["favorite_glider"],
                                    daily_rewards = new { },
                                    xp = 0,
                                    season_friend_match_boost = 0,
                                    favorite_musicpack = account.EquippedItems["favorite_musicpack"],
                                    banner_icon = "standardbanner1"
                                }
                            },
                            commandRevision = 5
                        }
                    }
                },
                profileCommandRevision = 1,
                serverTime = DateTime.Now.ToDateTimeString(),
                multiUpdate = new []
                {
                    new
                    {
                        profileRevision = 10,
                        profileId = "common_core",
                        profileChangesBaseRevision = 8,
                        profileCommandRevision = 5,
                        profileChanges = new List<object>()
                    }
                },
                responseVersion = 1
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }
    }
}
