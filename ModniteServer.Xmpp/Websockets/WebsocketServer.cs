using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModniteServer.Xmpp.Websockets
{
    internal sealed class WebsocketServer : IDisposable
    {
        private bool _isDisposed;
        private bool _serverStarted;
        private List<WebsocketMessage> _messageFragments;

        private readonly TcpListener _server;
        private readonly CancellationTokenSource _cts;

        public WebsocketServer(ushort port)
        {
            Port = port;

            _server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            _server.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            _cts = new CancellationTokenSource();
            _messageFragments = new List<WebsocketMessage>();
        }

        public event EventHandler<WebsocketMessageReceivedEventArgs> MessageReceived;

        public ushort Port { get; }

        public void Start()
        {
            if (_serverStarted)
                throw new InvalidOperationException("Server already started");

            _server.Start();
            _serverStarted = true;

            Task.Run(AcceptConnectionsAsync);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                using (_cts) _cts.Cancel();

                _server.Stop();
                _isDisposed = true;
            }
        }

        public async Task SendMessageAsync(Socket socket, MessageType messageType, byte[] data)
        {
            // todo
        }

        private async Task AcceptConnectionsAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                var client = await _server.AcceptSocketAsync();
                EstablishConnection(client);
            }
        }

        private void EstablishConnection(Socket client)
        {
            Task.Run(async () =>
            {
                var stream = new NetworkStream(client);

                // Handshake/upgrade connection
                while (!stream.DataAvailable && !_cts.IsCancellationRequested) { }
                _cts.Token.ThrowIfCancellationRequested();

                byte[] buffer = new byte[client.Available];
                await stream.ReadAsync(buffer, 0, buffer.Length, _cts.Token);

                // GET // HTTP/1.1
                // Pragma: no-cache
                // Cache-Control: no-cache
                // Host: 127.0.0.1
                // Origin: http://127.0.0.1
                // Upgrade: websocket
                // Connection: Upgrade
                // Sec-WebSocket-Key: <base64 encoded key>
                // Sec-WebSocket-Protocol: xmpp
                // Sec-WebSocket-Version: 13

                string wsKeyHash = null;
                string[] request = Encoding.UTF8.GetString(buffer).Split(new [] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in request)
                {
                    if (line.StartsWith("Sec-WebSocket-Key"))
                    {
                        string wsKey = line.Substring(line.IndexOf(' ') + 1);
                        wsKey += "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"; // magic
                        wsKeyHash = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(wsKey)));
                    }
                }

                var responseBuilder = new StringBuilder();
                responseBuilder.Append("HTTP/1.1 101 Switching Protocols\r\n");
                responseBuilder.Append("Connection: upgrade\r\n");
                responseBuilder.Append("Upgrade: websocket\r\n");
                responseBuilder.Append("Sec-WebSocket-Accept: " + wsKeyHash + "\r\n");
                responseBuilder.Append("\r\n");
                byte[] response = Encoding.UTF8.GetBytes(responseBuilder.ToString());
                await stream.WriteAsync(response, 0, response.Length);

                // Handshake complete, now decode incoming messages.
                while (!_cts.IsCancellationRequested)
                {
                    while (!stream.DataAvailable && !_cts.IsCancellationRequested) { }
                    _cts.Token.ThrowIfCancellationRequested();

                    buffer = new byte[client.Available];
                    await stream.ReadAsync(buffer, 0, buffer.Length, _cts.Token);

                    var message = new WebsocketMessage(buffer);
                    if (_messageFragments.Count > 0)
                    {
                        _messageFragments.Add(message);

                        if (message.IsCompleted)
                        {
                            // Combine all message fragments.
                            message = WebsocketMessage.Defragment(_messageFragments);
                            _messageFragments.Clear();

                            MessageReceived?.Invoke(this, new WebsocketMessageReceivedEventArgs(client, message));
                        }
                    }
                    else
                    {
                        if (message.IsCompleted)
                        {
                            MessageReceived?.Invoke(this, new WebsocketMessageReceivedEventArgs(client, message));
                        }
                        else
                        {
                            _messageFragments.Add(message);
                        }
                    }
                }
            });
        }
    }
}