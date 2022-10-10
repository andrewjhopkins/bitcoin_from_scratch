using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using LevelDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bitcoin_from_scratch.cli
{
    [Command("get-balance")]
    public class GetBalanceCommand : ICommand
    {
        private readonly string DbFileName = "./blockchainDb";

        [CommandParameter(0, Description = "Bitcoin address to get the balance of")]
        public string Address { get; init; }

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
                    var walletPath = $"./wallets/{Address}.dat";

                    var blockChain = new Blockchain(DbFileName, chainTipHash);
                    var wallet = new Wallet();

                    wallet.LoadWalletFromFile(walletPath);
                       
                    var unspentTransactionOutputs = blockChain.FindUnspentTransactionOutputs(wallet);

                    var balance = unspentTransactionOutputs.Sum(x => x.Value);

                    console.Output.WriteLine($"Balance: {balance}");
                }
            }
            
            return default;
        }
    }
}
