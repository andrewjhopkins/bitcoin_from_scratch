using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace bitcoin_from_scratch.cli
{
    [Command("create-wallet")]
    public class CreateWalletCommand : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            var wallet = new Wallet();
            var bitcoinAddress = wallet.GenerateBitcoinAddress();
            wallet.SaveWalletToFile();

            console.Output.WriteLine($"Bitcoin address: {bitcoinAddress}");
            console.Output.WriteLine($"Saved to file: /wallets/{bitcoinAddress}.dat");

            return default;
        }
    }
}
