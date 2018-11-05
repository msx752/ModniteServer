using ModniteServer.Commands;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace ModniteServer
{
    public static class CommandManager
    {
        private static readonly Dictionary<string, IUserCommand> Handlers;

        static CommandManager()
        {
            Handlers = new Dictionary<string, IUserCommand>
            {
                ["commands"] = new ShowCommandsListCommand(),
                ["update"] = new UpdateCommand(),

                // Account Commands
                ["create"] = new CreateAccountCommand(),
                ["giveitem"] = new GiveItemCommand(),
                ["getitems"] = new GetItemsCommand(),
            };
        }

        public static void InvokeCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return;

            string[] commandParts = command.Split(' ');
            if (Handlers.ContainsKey(commandParts[0]))
            {
                Handlers[commandParts[0]].Handle(commandParts.Skip(1).ToArray());
            }
            else
            {
                Log.Error($"Command '{command}' does not exist");
            }
        }

        private class ShowCommandsListCommand : IUserCommand
        {
            public string Description => "Shows a list of available commands";
            public string Args => "";
            public string ExampleArgs => "";

            public void Handle(string[] args)
            {
                Log.Information("AVAILABLE COMMANDS ------------------------");
                foreach (var handler in Handlers)
                {
                    if (handler.Value is ShowCommandsListCommand)
                        continue;

                    Log.Information($"{handler.Key} {handler.Value.Args}{{Description}}{{Example}}", handler.Value.Description, handler.Key + (string.IsNullOrEmpty(handler.Value.ExampleArgs) ? "" : " " + handler.Value.ExampleArgs));
                }
                Log.Information("-------------------------------------------");
            }
        }
    }
}
