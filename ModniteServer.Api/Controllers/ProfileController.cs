using ModniteServer.API.Accounts;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;

namespace ModniteServer.API.Controllers
{
    public sealed class ProfileController : Controller
    {
        private string _accountId;
        private Account _account;
        private int _revision;

        [Route("POST", "/fortnite/api/game/v2/profile/*/client/QueryProfile")]
        public void QueryProfile()
        {
            _accountId = Request.Url.Segments[Request.Url.Segments.Length - 3].Replace("/", "");

            if (!AccountManager.AccountExists(_accountId))
            {
                Response.StatusCode = 404;
                return;
            }

            _account = AccountManager.GetAccount(_accountId);

            Query.TryGetValue("profileId", out string profileId);
            Query.TryGetValue("rvn", out string rvn);
            _revision = Convert.ToInt32(rvn ?? "-2");

            switch (profileId)
            {
                case "common_core":
                    QueryCommonCoreProfile();
                    break;

                case "common_public":
                    QueryCommonPublicProfile();
                    break;

                case "athena":
                    QueryAthenaProfile();
                    break;

                default:
                    Response.StatusCode = 500;
                    break;
            }
        }

        private void QueryCommonCoreProfile()
        {
            var items = _account.CoreItems;

            var itemsFormatted = new Dictionary<string, object>();
            foreach (string item in items)
            {
                itemsFormatted.Add(Guid.NewGuid().ToString(), new
                {
                    templateId = item,
                    attributes = new
                    {
                        item_seen = 1
                    },
                    quantity = 1
                });
            }

            var response = new
            {
                profileRevision = 8,
                profileId = "common_core",
                profileChangesBaseRevision = 8,
                profileChanges = new List<object>
                {
                    new
                    {
                        changeType = "fullProfileUpdate",
                        profile = new
                        {
                            _id = _accountId, // not really account id but idk
                            created = DateTime.Now.AddDays(-7).ToDateTimeString(),
                            updated = DateTime.Now.AddDays(-1).ToDateTimeString(),
                            rvn = 8,
                            wipeNumber = 9,
                            accountId = _accountId,
                            profileId = "common_core",
                            version = "grant_skirmish_banners_october_2018",
                            items = itemsFormatted,
                            stats = new
                            {
                                attributes = new
                                {
                                    mtx_grace_balance = 0,
                                    import_friends_claimed = new List<object>(),
                                    mtx_purchase_history = new List<object>(),
                                    inventory_limit_bonus = 0,
                                    current_mtx_platform = "EpicPC",
                                    mtx_affiliate = "",
                                    weekly_purchases = new List<object>(),
                                    daily_purchases = new List<object>(),
                                    ban_history = new List<object>(),
                                    in_app_purchases = new List<object>(),
                                    monthly_purchases = new List<object>(),
                                    allowed_to_send_gifts = false,
                                    mfa_enabled = false,
                                    allowed_to_receive_gifts = false,
                                    gift_history = new List<object>()
                                }
                            },
                            commandRevision = 4
                        }
                    }
                },
                profileCommandRevision = 4,
                serverTime = DateTime.Now.ToDateTimeString(),
                responseVersion = 1
            };

            Log.Information("Retrieved profile 'common_core' {AccountId}{Profile}{Revision}", _accountId, response, _revision);

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }

        private void QueryCommonPublicProfile()
        {
            var response = new
            {
                profileRevision = 1,
                profileId = "common_public",
                profileChangesBaseRevision = 1,
                profileChanges = new List<object>(),
                profileCommandRevision = 0,
                serverTime = DateTime.Now.ToDateTimeString(),
                responseVersion = 1
            };

            Log.Information("Retrieved profile 'common_public' {AccountId}{Profile}{Revision}", _accountId, response, _revision);

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }

        private void QueryAthenaProfile()
        {
            // NOTE: Athena items are actually given in ClientQuestController.

            var items = _account.AthenaItems;
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
                dances[i] = _account.EquippedItems["favorite_dance" + i];
            }

            var response = new
            {
                profileRevision = 10,
                profileId = "athena",
                profileChangesBaseRevision = 10,
                profileChanges = new List<object>
                {
                    new
                    {
                        changeType = "fullProfileUpdate",
                        profile = new
                        {
                            _id = _accountId,
                            created = DateTime.Now.AddDays(-7).ToDateTimeString(),
                            updated = DateTime.Now.AddDays(-1).ToDateTimeString(),
                            rvn = 10,
                            wipeNumber = 5,
                            accountId = _accountId,
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
                                        favorite_character = _account.EquippedItems["favorite_character"],
                                        favorite_spray = new string[0],
                                        book_xp = 0,
                                        favorite_loadingscreen =  _account.EquippedItems["favorite_loadingscreen"],
                                        book_purchased = false,
                                        lifetime_wins = 0,
                                        favorite_hat = "",
                                        level = 1000000,
                                        favorite_battlebus = "",
                                        favorite_mapmarker = "",
                                        favorite_vehicledeco = "",
                                        accountLevel = 1000000,
                                        favorite_backpack = _account.EquippedItems["favorite_backpack"],
                                        favorite_dance = dances,
                                        inventory_limit_bonus = 0,
                                        favorite_skydivecontrail = _account.EquippedItems["favorite_skydivecontrail"],
                                        favorite_pickaxe = _account.EquippedItems["favorite_pickaxe"],
                                        favorite_glider = _account.EquippedItems["favorite_glider"],
                                        daily_rewards = new { },
                                        xp = 0,
                                        season_friend_match_boost = 0,
                                        favorite_musicpack = _account.EquippedItems["favorite_musicpack"],
                                        banner_icon = "standardbanner1"
                                }
                            },
                            commandRevision = 5
                        }
                    }
                },
                profileCommandRevision = 5,
                serverTime = DateTime.Now.ToDateTimeString(),
                responseVersion = 1
            };

            Log.Information("Retrieved profile 'athena' {AccountId}{Profile}{Revision}", _accountId, response, _revision);

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }
    }
}
