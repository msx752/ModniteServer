using ModniteServer.API.Accounts;
using Serilog;

namespace ModniteServer.Commands
{
    /// <summary>
    /// User command for listing all items an account has.
    /// </summary>
    public sealed class GetItemsCommand : IUserCommand
    {
        public string Description => "Lists all items in an account. For cosmetics, use profile 'athena'. For banners, use profile 'common_core'";
        public string ExampleArgs => "player123 athena";
        public string Args => "<userid> <profile>";

        public void Handle(string[] args)
        {
            if (args.Length > 1)
            {
                if (AccountManager.AccountExists(args[0]))
                {
                    var account = AccountManager.GetAccount(args[0]);

                    if (args[1] == "athena")
                    {
                        Log.Information($"Items in '{args[0]}' for profile '{args[1]}':");
                        foreach (string item in account.AthenaItems)
                        {
                            Log.Information(" " + item);
                        }
                    }
                    else if (args[1] == "common_core")
                    {
                        Log.Information($"Items in '{args[0]}' for profile '{args[1]}':");
                        foreach (string item in account.CoreItems)
                        {
                            Log.Information(" " + item);
                        }
                    }
                    else
                    {
                        Log.Error($"Invalid profile '{args[1]}'");
                    }
                }
                else
                {
                    Log.Error($"Account '{args[0]}' does not exist");
                }
            }
            else
            {
                Log.Error("Invalid arguments");
            }
        }
    }
}