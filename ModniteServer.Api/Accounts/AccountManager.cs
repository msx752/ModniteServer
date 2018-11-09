using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ModniteServer.API.Accounts
{
    /// <summary>
    /// Provides methods for accessing and creating player accounts.
    /// </summary>
    public static class AccountManager
    {
        public const string AccountsFolder = @"\Accounts\";
        public static readonly string AccountsFolderPath;

        private static ISet<Account> _retrievedAccounts;

        /// <summary>
        /// Initializes the account manager.
        /// </summary>
        static AccountManager()
        {
            string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AccountsFolderPath = location + AccountsFolder;

            if (!Directory.Exists(AccountsFolderPath))
            {
                Directory.CreateDirectory(AccountsFolderPath);
            }

            _retrievedAccounts = new HashSet<Account>();
        }

        /// <summary>
        /// Saves all loaded accounts. This should be called before the program closes.
        /// </summary>
        public static void SaveAccounts()
        {
            foreach (var account in _retrievedAccounts)
            {
                string path = Path.Combine(AccountsFolderPath, account.AccountId) + ".json";
                File.WriteAllText(path, JsonConvert.SerializeObject(account, Formatting.Indented));
            }
        }

        /// <summary>
        /// Checks whether the given account exists.
        /// </summary>
        /// <param name="accountId">The email or id associated with the account.</param>
        /// <returns><c>true</c> if the account exists; Otherwise, <c>false</c>.</returns>
        public static bool AccountExists(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                return false;

            if (accountId.Contains("@") || accountId.Contains("%40"))
            {
                accountId = accountId.Replace("%40", "@");
                accountId = accountId.Substring(0, accountId.IndexOf('@'));
            }

            string path = Path.Combine(AccountsFolderPath, accountId) + ".json";
            return File.Exists(path);
        }

        /// <summary>
        /// Gets the deserialized account data for the given user.
        /// </summary>
        /// <param name="accountId">The email or id associated with the account.</param>
        /// <returns>The deserialized account data.</returns>
        public static Account GetAccount(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                return null;

            if (accountId.Contains("@") || accountId.Contains("%40"))
            {
                accountId = accountId.Replace("%40", "@");
                accountId = accountId.Substring(0, accountId.IndexOf('@'));
            }

            foreach (var loadedAccount in _retrievedAccounts)
            {
                if (loadedAccount.AccountId == accountId)
                    return loadedAccount;
            }

            string path = Path.Combine(AccountsFolderPath, accountId) + ".json";
            var account = JsonConvert.DeserializeObject<Account>(File.ReadAllText(path));

            _retrievedAccounts.Add(account);
            return account;
        }

        /// <summary>
        /// Creates a new account with default values with the given credentials.
        /// </summary>
        /// <param name="email">The email to associate with this account.</param>
        /// <param name="passwordHash">The hashed password for this account.</param>
        /// <returns>The deserialized account data for the newly created account.</returns>
        public static Account CreateAccount(string email, string passwordHash)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            if (passwordHash == null) throw new ArgumentNullException(nameof(passwordHash));

            email = email.Replace("%40", "@");
            string username = email.Substring(0, email.IndexOf('@'));

            var account = new Account
            {
                Email = email,
                PasswordHash = passwordHash,
                DisplayName = username,
                AccountId = username,
                LastLogin = DateTime.UtcNow,
                AthenaItems = new HashSet<string>(ApiConfig.Current.DefaultAthenaItems),
                CoreItems = new HashSet<string>(ApiConfig.Current.DefaultCoreItems),
                EquippedItems = new Dictionary<string, string>(ApiConfig.Current.EquippedItems)
            };

            string path = Path.Combine(AccountsFolderPath, username) + ".json";
            File.WriteAllText(path, JsonConvert.SerializeObject(account, Formatting.Indented));

            Log.Information($"Created new account {account.DisplayName} {{Email}}{{DisplayName}}", email, account.DisplayName);

            _retrievedAccounts.Add(account);
            return account;
        }
    }
}
