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
                    var blockChain = new Blockchain(DbFileName, chainTipHash);

                    //var transaction = new Transaction();
                    //Blockchain.Mineblock();
                }
            }
            return default;
        }
    }
}
