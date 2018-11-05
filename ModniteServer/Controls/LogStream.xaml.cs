using Serilog.Events;
using Serilog.Parsing;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModniteServer.Controls
{
    public partial class LogStream : UserControl
    {
        public LogStream()
        {
            InitializeComponent();

            AutoScroll = true;
            this.scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            this.scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
        }

        public bool AutoScroll { get; set; }

        public async void AppendEvent(LogEvent logEvent)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                var message = new StringBuilder();
                var properties = new ObservableCollection<(string Key, string Value)>();

                foreach (var token in logEvent.MessageTemplate.Tokens)
                {
                    switch (token)
                    {
                        case TextToken textToken:
                            message.Append(textToken);
                            break;

                        case PropertyToken propToken:
                            string propValue = logEvent.Properties[propToken.PropertyName].ToString();
                            properties.Add((propToken.PropertyName, propValue));
                            break;
                    }
                }

                var entry = new LogStreamEntry
                {
                    Text = message.ToString(),
                    Properties = properties
                };

                this.logStreamPanel.Children.Add(entry);

                if (AutoScroll)
                {
                    this.scrollViewer.ScrollToEnd();
                    this.scrollViewer.ScrollToLeftEnd();
                }
            });
        }

        void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            AutoScroll = (this.scrollViewer.VerticalOffset == this.scrollViewer.ScrollableHeight);
        }

        void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.scrollViewer.ScrollToVerticalOffset(this.scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}