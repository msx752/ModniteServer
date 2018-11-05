namespace ModniteServer
{
    public interface IUserCommand
    {
        string Description { get; }

        string ExampleArgs { get; }

        string Args { get; }

        void Handle(string[] args);
    }
}
