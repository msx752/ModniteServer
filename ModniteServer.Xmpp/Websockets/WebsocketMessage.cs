using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModniteServer.Xmpp.Websockets
{
    internal enum MessageType
    {
        ContinuationFrame,
        Text,
        Binary
    }

    internal sealed class WebsocketMessage
    {
        internal WebsocketMessage(byte[] buffer)
        {
            IsCompleted = (buffer[0] & 0b_1000_0000) != 0;

            byte opcode = (byte)(buffer[0] & 0b_0000_1111);
            bool maskBit = (buffer[1] & 0b_1000_0000) != 0;
            int payloadLength = buffer[1] - 128;

            if (!maskBit)
            {
                // Invalid client message (all client messages must be masked)
                //Log.Warning("Invalid Websocket message received from client");
            }

            if (payloadLength == 126)
            {
                // TODO: next 2 bytes are length
            }
            else if (payloadLength == 127)
            {
                // TODO: next 4 bytes are length
            }

            byte[] mask = BitConverter.GetBytes(BitConverter.ToInt32(buffer, 2));

            byte[] decoded = new byte[payloadLength];
            for (int i = 0; i < payloadLength; i++)
            {
                decoded[i] = (byte)(buffer[i + 6] ^ mask[i % 4]);
            }

            switch (opcode)
            {
                case 0:
                    MessageType = MessageType.ContinuationFrame;
                    break;

                case 1:
                    MessageType = MessageType.Text;
                    TextContent = Encoding.UTF8.GetString(decoded);
                    break;

                case 2:
                    MessageType = MessageType.Binary;
                    BinaryContent = decoded;
                    break;

                case 9: // ping
                    // todo
                    break;

                case 10: // pong
                    // todo
                    break;

                default:
                    Log.Warning("Invalid opcode received");
                    break;
            }
        }

        public MessageType MessageType { get; private set; }

        public bool IsCompleted { get; private set; }

        public string TextContent { get; private set; }

        public byte[] BinaryContent { get; private set; }

        internal static WebsocketMessage Defragment(IEnumerable<WebsocketMessage> fragments)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            if (MessageType == MessageType.Text)
            {
                return TextContent;
            }
            else if (MessageType == MessageType.Binary)
            {
                return BitConverter.ToString(BinaryContent).Replace("-", " ");
            }
            else
            {
                return nameof(WebsocketMessage);
            }
        }
    }
}
