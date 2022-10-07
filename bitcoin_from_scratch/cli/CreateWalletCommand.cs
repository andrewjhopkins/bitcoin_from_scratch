using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace bitcoin_from_scratch.cli
{
    [Command("create-wallet")]
    public class CreateWalletCommand : ICommand
    {
        //Name: -file-name
        // Short name: -f 
        //[CommandOption("file-name", 'f', Description = "Specify file name. wallet.dat by default.")]
        //public string FileName { get; init; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var wallet = new Wallet();
            var bitcoinAddress = wallet.GenerateBitcoinAddress();

            console.Output.WriteLine($"bitcoin address: {bitcoinAddress}");

            return default;
        }
    }
}
