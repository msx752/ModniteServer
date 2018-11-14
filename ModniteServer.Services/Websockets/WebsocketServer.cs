using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModniteServer.Websockets
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

        public event EventHandler<WebsocketMessageNewConnectionEventArgs> NewConnection;

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

        public void SendMessage(Socket socket, MessageType messageType, byte[] data)
        {
            var message = new WebsocketMessage { MessageType = messageType };
            if (messageType == MessageType.Text)
            {
                message.TextContent = Encoding.UTF8.GetString(data);
            }
            else if (messageType == MessageType.Binary)
            {
                message.BinaryContent = data;
            }

            string dataString;
            if (messageType == MessageType.Text)
            {
                dataString = message.TextContent;
            }
            else
            {
                dataString = BitConverter.ToString(data).Replace("-", " ");
            }

            Log.Information($"Sent {data.Length} bytes {{Client}}{{MessageType}}{{Message}}", socket.RemoteEndPoint.ToString(), messageType, dataString);
            using (var stream = new NetworkStream(socket, false))
            {
                byte[] buffer = message.Serialize();
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
        }

        internal void SendMessage(Socket socket, MessageType text, object p)
        {
            throw new NotImplementedException();
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
                string authorization = null;
                string[] request = Encoding.UTF8.GetString(buffer).Split(new [] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in request)
                {
                    if (line.StartsWith("Sec-WebSocket-Key"))
                    {
                        string wsKey = line.Substring(line.IndexOf(' ') + 1);
                        wsKey += "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"; // magic
                        wsKeyHash = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(wsKey)));
                    }

                    if (line.StartsWith("Authorization"))
                    {
                        authorization = line.Substring(line.IndexOf(' ') + 1);
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
                await stream.FlushAsync();

                // Since we'll be using this task to receive incoming messages, we'll raise the
                // new connection event in a different task.
                var raiseEventTask = Task.Run(() =>
                {
                    NewConnection?.Invoke(this, new WebsocketMessageNewConnectionEventArgs(client, authorization));
                });

                // Handshake complete, now decode incoming messages.
                while (!_cts.IsCancellationRequested)
                {
                    while (!stream.DataAvailable && !_cts.IsCancellationRequested) { }
                    _cts.Token.ThrowIfCancellationRequested();

                    buffer = new byte[client.Available];
                    await stream.ReadAsync(buffer, 0, buffer.Length, _cts.Token);

                    string byteString = BitConverter.ToString(buffer).Replace("-", " ");

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