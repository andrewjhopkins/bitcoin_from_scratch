using LevelDB;
using Newtonsoft.Json;
using System.Text;

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

        public void NewBlockchain(Wallet wallet)
        {
            var publicKeyHash = Utils.HashPublicKey(wallet.PublicKey);
            var coinbaseTransaction = CreateCoinbaseTransaction("", wallet.BitcoinAddress, publicKeyHash);
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

        public TransactionOutput[] FindUnspentTransactionOutputs(Wallet wallet)
        {
            var unspentTransactionOutputs = new List<TransactionOutput>();
            var unspentTransactions = FindUnspentTransactions(wallet);

            var publicKeyHash = Utils.HashPublicKey(wallet.PublicKey);

            foreach (var transaction in unspentTransactions)
            {
                foreach (var output in transaction.Outputs)
                {
                    if (output.IsLockedWithKey(publicKeyHash))
                    {
                        unspentTransactionOutputs.Add(output);
                    }
                }
            }

            return unspentTransactionOutputs.ToArray();
        }

        public Transaction[] FindUnspentTransactions(Wallet wallet)
        {
            var unspentTransactions = new List<Transaction>();
            var spentTransactions = new Dictionary<string, List<int>>();

            var blockchainIterator = new BlockchainIterator(this);

            var publicKeyHash = Utils.HashPublicKey(wallet.PublicKey);

            while (!string.IsNullOrEmpty(blockchainIterator.CurrentHash))
            {
                var block = blockchainIterator.Next();

                foreach (var transaction in block.Transactions)
                {
                    var transactionId = Encoding.UTF8.GetString(transaction.Id);

                    for (var i = 0; i < transaction.Outputs.Length; i++)
                    {
                        if (IsTransactionOutputSpent(transactionId, spentTransactions, i))
                        {
                            continue;
                        }

                        // add to unspentTransactions if output can be unlocked with address (qualifies as not spent and belongs to address)
                        if (transaction.Outputs[i].IsLockedWithKey(publicKeyHash))
                        {
                            unspentTransactions.Add(transaction);
                        }

                        if (!transaction.IsCoinbase())
                        {
                            foreach (var transactionInput in transaction.Inputs)
                            {
                                if (transactionInput.UsesKey(publicKeyHash))
                                {
                                    // If a transaction output id appears here, it is spent
                                    var referencedTransactionOutputId = Encoding.UTF8.GetString(transactionInput.ReferencedTransactionOutputId);

                                    if (!spentTransactions.ContainsKey(referencedTransactionOutputId))
                                    {
                                        spentTransactions.Add(referencedTransactionOutputId, new List<int>());
                                    }
                                    spentTransactions[referencedTransactionOutputId].Add(transactionInput.ReferencedTransactionOutputIndex);
                                }
                            }
                        }
                    }
                }
            }

            return unspentTransactions.ToArray();
        }

        private bool IsTransactionOutputSpent(string transactionId, Dictionary<string, List<int>> spentTransactions, int outputIndexInTransaction)
        {
            if (spentTransactions.ContainsKey(transactionId))
            {
                return spentTransactions[transactionId].Contains(outputIndexInTransaction);
            }

            return false;
        }

        private Block CreateNewGenesisBlock(Transaction transaction)
        {
            var genesisBlock = new Block(new[] { transaction }, new byte[0]);
            genesisBlock.MineBlock();
            return genesisBlock;
        }

        private Transaction CreateCoinbaseTransaction(string data, string toAddress, byte[] toPublicKeyHash)
        { 
            if (string.IsNullOrEmpty(data))
            {
                data = $"Reward to {toAddress}";
            }

            var transactionInput = new TransactionInput(new byte[0], -1, null, Encoding.UTF8.GetBytes(data));

            var transactionOutput = new TransactionOutput(coinbaseReward, toPublicKeyHash);

            var coinbaseTransaction = new Transaction(new byte[0], new[] { transactionInput }, new[] { transactionOutput });

            return coinbaseTransaction;
        }
    }
}
