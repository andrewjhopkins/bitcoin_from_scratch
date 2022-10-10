using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using LevelDB;

namespace bitcoin_from_scratch.cli
{
    [Command("create-blockchain")]
    public class CreateBlockchainCommand : ICommand
    {
        private readonly string DbFileName = "./blockchainDb";

        [CommandParameter(0, Description = "Bitcoin address that recieves the genesis mining reward")]
        public string Address { get; init; }


        public ValueTask ExecuteAsync(IConsole console)
        {
            Blockchain blockchain;

            var options = new Options { CreateIfMissing = true };
            using (var db = new DB(options, DbFileName))
            {
                var chainTipHash = db.Get("1");
                db.Close();
                if (chainTipHash == null)
                {
                    console.Output.WriteLine("Blockchain does not exist");
                    var walletPath = $"./wallets/{Address}.dat";

                    var wallet = new Wallet();
                    wallet.LoadWalletFromFile(walletPath);

                    console.Output.WriteLine($"Mining genesis block...");
                    blockchain = new Blockchain(DbFileName);
                    blockchain.NewBlockchain(wallet);
                    console.Output.WriteLine("Blockchain created!");
                    return default;
                }

                console.Output.WriteLine("Blockchain already exists");
                blockchain = new Blockchain(DbFileName, chainTipHash);
                console.Output.WriteLine($"TipHashString: {blockchain.TipHashString}");
                return default;
        }
    }
}
}
