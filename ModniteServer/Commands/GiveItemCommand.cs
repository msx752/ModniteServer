using ModniteServer.API.Accounts;
using Serilog;

namespace ModniteServer.Commands
{
    /// <summary>
    /// User command for giving an item to an account.
    /// </summary>
    public sealed class GiveItemCommand : IUserCommand
    {
        public string Description => "Gives an item to an account. For cosmetics, use profile 'athena'. For banners, use profile 'common_core'";
        public string Args => "<userid> <profile> <item>";
        public string ExampleArgs => "player123 athena AthenaCharacter:CID_185_Athena_Commando_M_DurrburgerHero";

        public void Handle(string[] args)
        {
            if (args.Length == 3)
            {
                string accountId = args[0];

                if (!AccountManager.AccountExists(accountId))
                {
                    Log.Error($"Account '{accountId}' does not exist");
                    return;
                }

                if (!args[2].Contains(":"))
                {
                    Log.Error($"Invalid item id");
                    return;
                }

                var account = AccountManager.GetAccount(accountId);

                if (args[1] == "athena")
                {
                    if (account.AthenaItems.Contains(args[2]))
                    {
                        Log.Error($"Account '{accountId}' already has the item '{args[2]}' in profile '{args[1]}'");
                    }
                    else
                    {
                        account.AthenaItems.Add(args[2]);
                        Log.Information($"Gave '{args[2]}' to '{accountId}' in profile '{args[1]}'");
                    }
                }
                else if (args[1] == "common_core")
                {
                    if (account.AthenaItems.Contains(args[2]))
                    {
                        Log.Error($"Account '{accountId}' already has the item '{args[2]}' in profile '{args[1]}'");
                    }
                    else
                    {
                        account.CoreItems.Add(args[2]);
                        Log.Information($"Gave '{args[2]}' to '{accountId}' in profile '{args[1]}'");
                    }
                }
                else
                {
                    Log.Error($"Invalid profile '{args[1]}'");
                }

                AccountManager.SaveAccounts();
            }
            else
            {
                Log.Error("Invalid arguments");
            }
        }
    }
}