using ModniteServer.API.Accounts;
using ModniteServer.Controls;
using ModniteServer.ViewModels;
using Serilog;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ModniteServer.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Log.Logger = new LoggerConfiguration().WriteTo.Sink(new LogStreamSink(this.logStream)).CreateLogger();
            DataContext = ViewModel = new MainViewModel();

            this.commandTextBox.GotFocus += delegate { this.commandTextBox.Tag = ""; };
            this.commandTextBox.LostFocus += delegate { this.commandTextBox.Tag = "Enter command"; };
            this.commandTextBox.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                    ViewModel.InvokeCommand();
            };

            this.Closing += OnMainWindowClosing;
        }

        public MainViewModel ViewModel { get; }

        void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            AccountManager.SaveAccounts();
        }
    }
}