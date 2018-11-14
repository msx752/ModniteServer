using ModniteServer.API;
using ModniteServer.Matchmaker;
using ModniteServer.Xmpp;
using Serilog;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ModniteServer.ViewModels
{
    public sealed class MainViewModel : ObservableObject
    {
        public const int CurrentVersion = 2;

        private string _title;
        private string _command;

        private ApiServer _apiServer;
        private XmppServer _xmppServer;
        private MatchmakerServer _matchmakerServer;

        public MainViewModel()
        {
            Title = "Modnite Server";

            Log.Information("Modnite Server v1.1-Alpha");
            Log.Information("Made with ❤️ by the Modnite Contributors");

            StartServices();

            Log.Information("Type 'commands' to see a list of available commands");

            Task.Run(CheckForUpdatesAsync);
        }

        /// <summary>
        /// Gets or sets the title of this app.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetValue(ref _title, value); }
        }

        /// <summary>
        /// Gets or sets the current command.
        /// </summary>
        public string Command
        {
            get { return _command; }
            set { SetValue(ref _command, value); }
        }

        /// <summary>
        /// Invokes the command stored in <see cref="Command"/>.
        /// </summary>
        public void InvokeCommand()
        {
            CommandManager.InvokeCommand(Command);
            Command = "";
        }

        /// <summary>
        /// Compares the current version with the newest version to see if an update is available.
        /// </summary>
        private async Task CheckForUpdatesAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    int latestVersion = Convert.ToInt32(await client.GetStringAsync("https://modniteimages.azurewebsites.net/update.txt"));
                    if (CurrentVersion < latestVersion)
                    {
                        Log.Information("NEW VERSION AVAILABLE! Type 'update' to get the latest version.");
                    }
                    else
                    {
                        Log.Information("Modnite Server is up to date");
                    }
                }
                catch
                {
                    Log.Error("Could not check for updates");
                }
            }
        }

        /// <summary>
        /// Starts services.
        /// </summary>
        private void StartServices()
        {
            var config = ApiConfig.Current;
            ApiConfig.Current.Logger = Log.Logger;

            // Start the API server.
            _apiServer = new ApiServer(ApiConfig.Current.Port)
            {
                Logger = Log.Logger,
                LogHttpRequests = ApiConfig.Current.LogHttpRequests,
                Log404 = ApiConfig.Current.Log404,
                AlternativeRoute = "/fortnite-gameapi"
            };
            try
            {
                _apiServer.Start();
                Log.Information("API server started on port " + _apiServer.Port);
            }
            catch (HttpListenerException) when (_apiServer.Port == 80)
            {
                Log.Fatal("You must run this program as an administrator to use port 80!");
                return;
            }

            // Start the XMPP service.
            _xmppServer = new XmppServer(ApiConfig.Current.XmppPort)
            {
                Logger = Log.Logger
            };
            _xmppServer.Start();
            Log.Information("XMPP server started on port " + _xmppServer.Port);

            // Start the matchmaker server.
            _matchmakerServer = new MatchmakerServer(ApiConfig.Current.MatchmakerPort)
            {
                Logger = Log.Logger
            };
            _matchmakerServer.Start();
            Log.Information("Matchmaker server started on port " + _matchmakerServer.Port);

            // TODO: Start the game server.
        }
    }
}