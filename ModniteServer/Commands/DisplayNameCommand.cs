using System.Linq;
using ModniteServer.API.Accounts;
using Serilog;

namespace ModniteServer.Commands
{
    /// <summary>
    /// User command to change the display name for an account.
    /// </summary>
    public sealed class DisplayNameCommand : IUserCommand
    {
        public string Description => "Sets the display name for an account.";
        public string ExampleArgs => "player123 Cool name";
        public string Args => "<userid> <displayname>";

        public void Handle(string[] args)
        {
            if (args.Length >= 2)
            {
                if (AccountManager.AccountExists(args[0]))
                {
                    var account = AccountManager.GetAccount(args[0]);
                    string newName = string.Join(" ", args.Skip(1));

                    if (account.DisplayName == newName)
                    {
                        Log.Error($"Account '{args[0]}' already has display name '{account.DisplayName}'");
                    }
                    else
                    {
                        account.DisplayName = newName;

                        Log.Information($"Set display name for account '{args[0]}' to '{newName}'");
                        AccountManager.SaveAccounts();
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
