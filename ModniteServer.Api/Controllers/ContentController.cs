using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ModniteServer.API.Controllers
{
    public sealed class ContentController : Controller
    {
        [Route("GET", "/content/api/pages/fortnite-game")]
        public void GetNews()
        {
            var response = new
            {
                _title = "Fortnite Game",
                _activeDate = DateTime.Now.AddDays(-1).ToDateTimeString(),
                lastModified = DateTime.Now.ToDateTimeString(),
                _locale = "en-US",
                subgameselectdata = new
                {
                    _activeDate = DateTime.Now.AddDays(-1).ToDateTimeString(),
                    lastModified = DateTime.Now.ToDateTimeString(),
                    _locale = "en-US",
                    title = "subgameselectdata",
                    saveTheWorldUnowned = new
                    {
                        _type = "CommonUI Simple Message",
                        message = new
                        {
                            image = "https://modniteimages.azurewebsites.net/modnite-selectgame.png",
                            hidden = false,
                            messagetype = "normal",
                            _type = "CommonUI Simple Message Base",
                            title = "Co-op PVE",
                            body = "Cooperative PvE storm-fighting adventure!",
                            spotlight = false
                        }
                    },
                    saveTheWorld = new
                    {
                        _type = "CommonUI Simple Message",
                        message = new
                        {
                            image = "https://modniteimages.azurewebsites.net/modnite-selectgame.png",
                            hidden = false,
                            messagetype = "normal",
                            _type = "CommonUI Simple Message Base",
                            title = "Co-op PVE",
                            body = "Cooperative PvE storm-fighting adventure!",
                            spotlight = false
                        }
                    },
                    battleRoyale = new
                    {
                        _type = "CommonUI Simple Message",
                        message = new
                        {
                            image = "https://modniteimages.azurewebsites.net/modnite-selectgame.png",
                            hidden = false,
                            messageType = "normal",
                            title = "1 Player PvP",
                            body = "Single Player Battle Royale",
                            spotlight = true
                        }
                    }
                },
                loginmessage = new
                {
                    _activeDate = DateTime.Now.AddDays(-1).ToDateTimeString(),
                    lastModified = DateTime.Now.ToDateTimeString(),
                    _locale = "en-US",
                    _title = "LoginMessage",
                    loginmessage = new
                    {
                        _type = "CommonUI Simple Message Base",
                        title = "",
                        body = ""
                    }
                },
                playlistimages = new
                {
                    _activeDate = DateTime.Now.AddDays(-1).ToDateTimeString(),
                    lastModified = DateTime.Now.ToDateTimeString(),
                    _locale = "en-US",
                    _title = "playlistimages",
                    playlistimages = new
                    {
                        images = new List<object>
                        {
                            new
                            {
                                image = "https://cdn2.unrealengine.com/Fortnite/fortnite-game/playlistinformation/LlamaLower-512x512-5fda7d81162d9d918e72d8deadedc280959a8d1b.png",
                                _type = "PlaylistImageEntry",
                                playlistname = "Playlist_Playground"
                            }
                        }
                    }
                },
                playlistinformation = new
                {
                    _activeDate = DateTime.Now.AddDays(-1).ToDateTimeString(),
                    lastModified = DateTime.Now.ToDateTimeString(),
                    _locale = "en-US",
                    _title = "playlistinformation",
                    frontend_matchmaking_header_style = "None",
                    frontend_matchmaking_header_text = "",
                    playlist_info = new
                    {
                        _type = "Playlist Information",
                        playlists = new List<object>
                        {
                            new
                            {
                                image = "https://cdn2.unrealengine.com/Fortnite/fortnite-game/playlistinformation/LlamaLower-512x512-5fda7d81162d9d918e72d8deadedc280959a8d1b.png",
                                playlist_name = "Playlist_Playground",
                                special_border = "None",
                                _type = "FortPlaylistInfo"
                            }
                        }
                    }
                },
                tournamentinformation = new
                {
                    _activeDate = DateTime.Now.AddDays(-1).ToDateTimeString(),
                    lastModified = DateTime.Now.ToDateTimeString(),
                    _locale = "en-US",
                    _title = "tournamentinformation",
                    tournament_info = new
                    {
                        tournaments = new List<object>(),
                        _type = "Tournaments Info"
                    }
                },
                emergencynotice = new
                {
                    news = new
                    {
                        _type = "Battle Royale News",
                        messages = new List<object>()
                    },
                    _title = "emergencynotice",
                    _activeDate = DateTime.Now.AddDays(-1).ToDateTimeString(),
                    lastModified = DateTime.Now.ToDateTimeString(),
                    _locale = "en-US"
                }
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }
    }
}
