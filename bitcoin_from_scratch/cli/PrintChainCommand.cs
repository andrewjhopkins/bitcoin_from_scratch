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
    [Command("print-chain")]
    public class PrintChainCommand : ICommand
    {
        private readonly string DbFileName = "./blockchainDb";

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
                    var blockchainIterator = new BlockchainIterator(blockchain);

                    while (!string.IsNullOrEmpty(blockchainIterator.CurrentHash))
                    {
                        var block = blockchainIterator.Next();
                        console.Output.WriteLine($"Hash: {Utils.BytesToString(block.Hash)}");
                    }
                }
            }

            return default;
        }
    }
}
