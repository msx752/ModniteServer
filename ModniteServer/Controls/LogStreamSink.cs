using Serilog.Core;
using Serilog.Events;

namespace ModniteServer.Controls
{
    public class LogStreamSink : ILogEventSink
    {
        public LogStreamSink(LogStream logStream)
        {
            LogStream = logStream;
        }

        public LogStream LogStream { get; }

        public void Emit(LogEvent logEvent)
        {
            LogStream.AppendEvent(logEvent);
        }
    }
}