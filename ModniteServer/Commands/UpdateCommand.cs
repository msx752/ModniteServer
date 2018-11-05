using System.Diagnostics;

namespace ModniteServer.Commands
{
    public sealed class UpdateCommand : IUserCommand
    {
        public string Description => "Gets the latest version of Modnite Server";
        public string Args => "";
        public string ExampleArgs => "";

        public void Handle(string[] args)
        {
            Process.Start("https://github.com/ModniteNet/ModniteServer/releases");
        }
    }
}
