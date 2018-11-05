using ModniteServer.API.Accounts;
using Serilog;
using System.Security.Cryptography;
using System.Text;

namespace ModniteServer.Commands
{
    /// <summary>
    /// User command for creating a new account.
    /// </summary>
    public sealed class CreateAccountCommand : IUserCommand
    {
        public string Description => "Creates an account";
        public string Args => "<userid> <password>";
        public string ExampleArgs => "player123 hunter2";

        public void Handle(string[] args)
        {
            if (args.Length > 1)
            {
                if (AccountManager.AccountExists(args[0]))
                {
                    Log.Error($"Account '{args[0]}' already exists");
                    return;
                }

                string email = args[0] + "@modnite.net";

                string passwordHash;
                using (var sha256 = new SHA256Managed())
                {
                    byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(args[1]));
                    var hashString = new StringBuilder();
                    foreach (byte b in hash)
                    {
                        hashString.AppendFormat("{0:x2}", b);
                    }
                    passwordHash = hashString.ToString();
                }

                AccountManager.CreateAccount(args[0] + "@modnite.net", passwordHash);
            }
            else
            {
                Log.Error("Invalid arguments");
            }
        }
    }
}