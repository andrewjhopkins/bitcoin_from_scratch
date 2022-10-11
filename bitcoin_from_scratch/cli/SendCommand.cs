using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using LevelDB;

namespace bitcoin_from_scratch.cli
{
    [Command("send")]
    public class SendCommand : ICommand
    {
        private readonly string DbFileName = "./blockchainDb";

        [CommandParameter(0, Description = "Sending Bitcoin address")]
        public string SenderAddress { get; init; }

        [CommandParameter(1, Description = "Reciever Bitcoin address")]
        public string RecieverAddress { get; init; }

        [CommandParameter(2, Description = "Amount of Bitcoin to send")]
        public int Amount { get; init; }


        //TODO: Get public key from bitcoin address by decoding base58
        public ValueTask ExecuteAsync(IConsole console)
        {
            using (var db = new DB(new Options(), DbFileName))
            {
                var chainTipHash = db.Get("1");
                db.Close();
                if (chainTipHash == null)
                {
                    console.Output.WriteLine($"Blockchain does not exist. Create a new one first");
                }
                else
                {
                    var blockchain = new Blockchain(DbFileName, chainTipHash);

                    var toWallet = new Wallet();
                    toWallet.LoadWalletFromFile($"./wallets/{RecieverAddress}.dat");

                    var fromWallet = new Wallet();
                    fromWallet.LoadWalletFromFile($"./wallets/{SenderAddress}.dat");

                    var transaction = new Transaction(blockchain, toWallet, fromWallet, Amount);
                    blockchain.CreateBlock(transaction);

                    console.Output.WriteLine($"{Amount} bitcoin sent from address: {SenderAddress} to address: {RecieverAddress}");
                }
            }
            return default;
        }
    }
}
