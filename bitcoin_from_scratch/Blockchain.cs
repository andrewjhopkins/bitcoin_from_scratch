using LevelDB;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace bitcoin_from_scratch
{
    public class Blockchain
    {
        public const int coinbaseReward = 10;
        public string DbFilePath { get; set; }
        public string TipHashString { get; set; }

        public Blockchain(string dbFilePath)
        {
            DbFilePath = dbFilePath;
        }

        public Blockchain(string dbFilePath, string tipHashString) : this(dbFilePath)
        {
            TipHashString = tipHashString;
        }

        public void NewBlockchain(string toBitcoinAddress)
        { 
            var coinbaseTransaction = CreateCoinbaseTransaction("", toBitcoinAddress);
            var genesisBlock = CreateNewGenesisBlock(coinbaseTransaction);

            if (genesisBlock.Hash != null)
            {
                using (var db = new DB(new Options(), DbFilePath))
                {
                    var hashString = Utils.BytesToString(genesisBlock.Hash);

                    db.Put(hashString, JsonConvert.SerializeObject(genesisBlock));
                    db.Put("1", hashString);

                    TipHashString = hashString;
                    db.Close();
                }
            }
            else
            {
                throw new Exception("Genesis block not created");
            }
        }

        public TransactionOutput[] FindUnspentTransactionOutputs(string address)
        {
            var unspentTransactionOutputs = new List<TransactionOutput>();
            var unspentTransactions = FindUnspentTransactions(address);

            return unspentTransactionOutputs.ToArray();
        }

        public Transaction[] FindUnspentTransactions(string address)
        {
            var unspentTransactions = new List<Transaction>();
            var spentTransactions = new Dictionary<string, int[]>();

            return unspentTransactions.ToArray();
        }

        private Block CreateNewGenesisBlock(Transaction transaction)
        {
            return new Block(new[] { transaction }, new byte[0]);
        }

        private Transaction CreateCoinbaseTransaction(string data, string to)
        { 
            if (string.IsNullOrEmpty(data))
            {
                data = $"Reward to {to}";
            }

            var transactionInput = new TransactionInput(new byte[0], -1, data);
            var transactionOutput = new TransactionOutput(coinbaseReward, to);
            var coinbaseTransaction = new Transaction(new byte[0], new[] { transactionInput }, new[] { transactionOutput });

            return coinbaseTransaction;
        }
    }
}
