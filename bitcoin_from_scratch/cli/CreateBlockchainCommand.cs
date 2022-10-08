using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace bitcoin_from_scratch.cli
{
    [Command("create-blockchain")]
    public class CreateBlockchainCommand : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine($"Mining genesis block...");
            var blockchain = new Blockchain();
            console.Output.WriteLine("Blockchain created!");
            return default;
        }
    }
}
