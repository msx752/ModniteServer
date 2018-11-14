using ModniteServer.Websockets;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ModniteServer.Matchmaker
{
    public sealed class MatchmakerServer : IDisposable
    {
        private bool _isDisposed;

        private readonly WebsocketServer _server;

        public MatchmakerServer(ushort port)
        {
            Port = port;

            _server = new WebsocketServer(Port);
            _server.NewConnection += OnNewConnection;
            _server.MessageReceived += OnMessageReceived;
        }

        public ILogger Logger
        {
            get { return Log.Logger; }
            set { Log.Logger = value; }
        }

        public ushort Port { get; }

        public void Start() => _server.Start();

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _server.Dispose();
                _isDisposed = true;
            }
        }

        private void OnNewConnection(object sender, System.Net.Sockets.Socket e)
        {
            Log.Information("[Matchmaker] New connection");
        }

        private async void OnNewConnection(object sender, WebsocketMessageNewConnectionEventArgs e)
        {
            // The payload obtained in /fortnite/api/game/v2/matchmakingservice/ticket/player/*
            // is supposed to be included in the authorization header, but for some reason it's
            // blank. Not a big deal for now, but will need to fix to support multiplayer.

            string[] authParts = e.Authorization.Split(' ');

            // Epic-Signed
            string ticketType = authParts[1];
            string payload = authParts[2];
            // signature

            // The client uses a state machine for matchmaking, so we'll have to go through the
            // state machine with valid states. For instance, jumping from ConnectingToService
            // directly to InQueue skipping other states will cause an error.

            // Changes client state from ObtainingTicket to ConnectingToService
            _server.SendMessage(e.Socket, MessageType.Text, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                payload = new
                {
                    state = "Connecting"
                },
                name = "StatusUpdate"
            })));

            await Task.Delay(10);

            // Changes client state from ConnectingToService to WaitingForParty
            _server.SendMessage(e.Socket, MessageType.Text, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                payload = new
                {
                    state = "Waiting",
                    totalPlayers = 1,
                    connectedPlayers = 1
                },
                name = "StatusUpdate"
            })));

            await Task.Delay(10);

            // Changes client state from WaitingForParty to InQueue
            _server.SendMessage(e.Socket, MessageType.Text, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                payload = new
                {
                    state = "Queued",
                    queuedPlayers = 1,
                    estimatedWaitSec = 10,
                    status = 1,
                    ticketId = "ticket_id"
                },
                name = "StatusUpdate"
            })));

            await Task.Delay(10);

            // Changes client state from InQueue to InReadyCheck
            _server.SendMessage(e.Socket, MessageType.Text, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                payload = new
                {
                    state = "ReadyCheck",
                    matchId = "match_id",
                    status = "PASSED",
                    durationSec = 0,
                    ready = true,
                    responded = 1,
                    totalPlayers = 1,
                    readyPlayers = 1
                },
                name = "StatusUpdate"
            })));

            // Changes client state from InReadyCheck to InSessionAssignment
            _server.SendMessage(e.Socket, MessageType.Text, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                payload = new
                {
                    state = "SessionAssignment",
                    matchId = "match_id"
                },
                name = "StatusUpdate"
            })));

            // Changes client state from InSessionAssignment to Finished
            _server.SendMessage(e.Socket, MessageType.Text, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                payload = new
                {
                    sessionId = "session_id",
                    joinDelaySec = 1,
                    matchId = "match_id"
                },
                name = "Play"
            })));
        }

        private void OnMessageReceived(object sender, WebsocketMessageReceivedEventArgs e)
        {
            if (e.Message.TextContent == "ping")
            {
                _server.SendMessage(e.Socket, MessageType.Text, Encoding.UTF8.GetBytes("pong"));
                Log.Information("[Matchmaker] Client sent 'ping'. Responded with 'pong'.");
            }
        }
    }
}