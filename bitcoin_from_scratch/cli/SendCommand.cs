using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using LevelDB;

namespace bitcoin_from_scratch.cli
{
    [Command("send")]
    public class SendCommand : ICommand
    {
        [CommandParameter(0, Description = "Sending Bitcoin address")]
        public string SenderAddress { get; init; }

        [CommandParameter(1, Description = "Reciever Bitcoin address")]
        public string RecieverAddress { get; init; }

        [CommandParameter(2, Description = "Amount of Bitcoin to send")]
        public int Amount { get; init; }

        [CommandOption("reward", 'r', Description = "Include mining reward")]
        public bool IncludeMiningReward { get; init; } = false;

        public ValueTask ExecuteAsync(IConsole console)
        {
            using (var db = new DB(new Options(), Constants.BlockChainDbFile))
            {
                var chainTipHash = db.Get("1");
                db.Close();
                if (chainTipHash == null)
                {
                    console.Output.WriteLine($"Blockchain does not exist. Create a new one first");
                }
                else
                {
                    var blockchain = new Blockchain(Constants.BlockChainDbFile, chainTipHash);

                    var fromWallet = new Wallet();
                    fromWallet.LoadWalletFromFile($"./wallets/{SenderAddress}.dat");

                    var publicKeyHash = Utils.HashPublicKey(fromWallet.PublicKey);

                    var toWalletHashedPublicKey = Utils.GetPublicKeyHashFromBitcoinAddress(RecieverAddress);

                    var transaction = new Transaction(blockchain, toWalletHashedPublicKey, fromWallet, Amount);
                    var transactions = new List<Transaction>() { transaction };

                    if (IncludeMiningReward)
                    { 
                        var miningRewardTransaction = blockchain.CreateCoinbaseTransaction("", fromWallet.BitcoinAddress, publicKeyHash);
                        transactions.Add(miningRewardTransaction);
                    }

                    var newBlock = blockchain.CreateBlock(transactions.ToArray());

                    console.Output.WriteLine($"{Amount} bitcoin sent from address: {SenderAddress} to address: {RecieverAddress}");
                }
            }
            return default;
        }
    }
}
