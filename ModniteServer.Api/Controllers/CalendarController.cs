using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ModniteServer.API.Controllers
{
    public class CalendarController : Controller
    {
        [Route("GET", "/fortnite/api/calendar/v1/timeline")]
        public void GetTimeline()
        {
            if (!Authorize()) { }

            // Events never expire in Modnite Server. The start/end dates are always relative to today.
            var clientEvents = new List<object>();
            foreach (string evt in ApiConfig.Current.ClientEvents)
            {
                clientEvents.Add(new
                {
                    eventType = "EventFlag." + evt,
                    activeUntil = DateTime.Now.Date.AddDays(7).ToDateTimeString(),
                    activeSince = DateTime.Now.Date.AddDays(-1).ToDateTimeString()
                });
            }

            var response = new
            {
                channels = new Dictionary<string, object>
                {
                    ["client-matchmaking"] = new
                    {
                        // This section is used to disable game modes in certain regions.
                        states = new [] {
                            new
                            {
                                validFrom = DateTime.Now.Date.AddDays(-1).ToDateTimeString(),
                                activeEvents = new string[0],
                                state = new
                                {
                                    region = new { }
                                }
                            }
                        },
                        cacheExpire = DateTime.Now.Date.AddDays(7).ToDateTimeString()
                    },
                    ["client-events"] = new
                    {
                        // You can set event flags in config.json. Visit https://modnite.net for a list of event flags.
                        // Event flags control stuff like lobby theme, battle bus skin, etc.
                        states = new[]
                        {
                            new
                            {
                                validFrom = DateTime.Now.AddDays(-1).ToDateTimeString(),
                                activeEvents = clientEvents,
                                state = new
                                {
                                    activeStorefronts = new string[0],
                                    eventNamedWeights = new { },
                                    activeEvents = new string[0],
                                    seasonNumber = 0,
                                    seasonTemplateId = "",
                                    matchXpBonusPoints = 0, // Bonus XP Event
                                    seasonBegin = DateTime.Now.Date.AddDays(-30).ToDateTimeString(),
                                    seasonEnd = DateTime.Now.Date.AddDays(30).ToDateTimeString(),
                                    seasonDisplayedEnd = DateTime.Now.Date.AddDays(30).ToDateTimeString(),
                                    weeklyStoreEnd = DateTime.Now.Date.AddDays(7).ToDateTimeString(),
                                    stwEventStoreEnd = DateTime.Now.Date.AddDays(7).ToDateTimeString(),
                                    stwWeeklyStoreEnd = DateTime.Now.Date.AddDays(7).ToDateTimeString()
                                }
                            }
                        },
                        cacheExpire = DateTime.Now.Date.AddDays(7).ToDateTimeString()
                    }
                },
                eventsTimeOffsetHrs = 0.0,
                currentTime = DateTime.Now.ToDateTimeString()
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }
    }
}
