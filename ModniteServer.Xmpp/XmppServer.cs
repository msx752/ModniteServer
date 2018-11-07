using ModniteServer.Xmpp.Websockets;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace ModniteServer.Xmpp
{
    public sealed class XmppServer : IDisposable
    {
        private Dictionary<EndPoint, XmppClient> _clients;
        private bool _isDisposed;

        private readonly WebsocketServer _server;

        public XmppServer(ushort port)
        {
            Port = port;

            _server = new WebsocketServer(Port);
            _server.MessageReceived += OnMessageReceived;

            _clients = new Dictionary<EndPoint, XmppClient>();
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

        internal void SendXmppMessage(Socket socket, XElement message)
        {
            _server.SendMessage(socket, MessageType.Text, Encoding.UTF8.GetBytes(message.ToString()));
        }

        private void OnMessageReceived(object sender, WebsocketMessageReceivedEventArgs e)
        {
            var element = XElement.Parse(e.Message.TextContent);
            if (element.Name.LocalName == "open")
            {
                // XMPP handshake
                XNamespace xmlns = "urn:ietf:params:xml:ns:xmpp-framing";
                XNamespace xml = "xml";
                var response = new XElement(
                    xmlns + "open",
                    new XAttribute("xmlns", "urn:ietf:params:xml:ns:xmpp-framing"),
                    new XAttribute("from", ""),
                    new XAttribute("id", "xmpp_id_yolo"),
                    new XAttribute(xml + "lang", "en"),
                    new XAttribute("version", "1.0")
                );
                SendXmppMessage(e.Socket, response);

                _clients.Add(e.Socket.RemoteEndPoint, new XmppClient(this, e.Socket));
                Log.Information("New XMPP client {Client}", e.Socket.RemoteEndPoint);
            }
            else
            {
                _clients[e.Socket.RemoteEndPoint].HandleMessage(element, out XElement response);
                if (response != null)
                {
                    SendXmppMessage(e.Socket, response);
                }
            }
        }
    }
}