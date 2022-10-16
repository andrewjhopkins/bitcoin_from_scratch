using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using LevelDB;

namespace bitcoin_from_scratch.cli
{
    [Command("create-blockchain")]
    public class CreateBlockchainCommand : ICommand
    {
        [CommandParameter(0, Description = "Bitcoin address that recieves the genesis mining reward")]
        public string Address { get; init; }


        public ValueTask ExecuteAsync(IConsole console)
        {
            Blockchain blockchain;
            UtxoSet utxoSet;

            var options = new Options { CreateIfMissing = true };
            using (var db = new DB(options, Constants.BlockChainDbFile))
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
                    blockchain = new Blockchain(Constants.BlockChainDbFile, Constants.UtxoSetDbFile);
                    blockchain.NewBlockchain(wallet);
                    console.Output.WriteLine("Blockchain created!");

                    utxoSet = new UtxoSet(blockchain);
                    utxoSet.ReIndex();

                    return default;
                }

                console.Output.WriteLine("Blockchain already exists");
                blockchain = new Blockchain(Constants.BlockChainDbFile, chainTipHash);

                utxoSet = new UtxoSet(blockchain);
                utxoSet.ReIndex();

                console.Output.WriteLine($"TipHashString: {blockchain.TipHashString}");
                return default;
        }
    }
}
}
