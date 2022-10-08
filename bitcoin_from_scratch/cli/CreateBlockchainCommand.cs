using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using LevelDB;

namespace bitcoin_from_scratch.cli
{
    [Command("create-blockchain")]
    public class CreateBlockchainCommand : ICommand
    {
        public readonly string DbFileName = "./blockchainDb";

        public ValueTask ExecuteAsync(IConsole console)
        {
            Blockchain blockchain;
            var options = new Options { CreateIfMissing = true };
            using (var db = new DB(options, DbFileName))
            {
                var chainTipValue = db.Get("1");
                db.Close();
                if (chainTipValue == null)
                {
                    console.Output.WriteLine("Blockchain does not exist");
                    console.Output.WriteLine($"Mining genesis block...");
                    blockchain = new Blockchain(DbFileName);
                    console.Output.WriteLine("Blockchain created!");
                    return default;
                }

                console.Output.WriteLine("Blockchain already exists");
                blockchain = new Blockchain(DbFileName, chainTipValue);
                console.Output.WriteLine($"TipHashString: {blockchain.TipHashString}");
                return default;
            }
        }
    }
}
