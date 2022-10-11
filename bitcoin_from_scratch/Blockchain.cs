﻿using LevelDB;
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

        public Block CreateBlock(Transaction transaction)
        {
            var previousHash = Utils.StringToBytes(TipHashString);
            var block = new Block(new Transaction[] { transaction }, previousHash);

            block.MineBlock();

            if (block.Hash != null)
            {
                using (var db = new DB(new Options(), DbFilePath))
                {
                    var hashString = Utils.BytesToString(block.Hash);
                    db.Put(hashString, JsonConvert.SerializeObject(block));
                    db.Put("1", hashString);

                    TipHashString = hashString;
                    db.Close();
                }
            }
            else
            {
                throw new Exception("Block hash is null");
            }

            return block;
        }

        public Tuple<int, Dictionary<byte[], int[]>> FindSpendableOutputs(Wallet address, int amount)
        {
            var publicKeyHash = Utils.HashPublicKey(address.PublicKey);

            var unspentOutputs = new Dictionary<byte[], int[]>();
            var unspentTransactions = FindUnspentTransactions(address);
            var accumulatedValue = 0;

            foreach (var transaction in unspentTransactions)
            {
                for (var i = 0; i < transaction.Outputs.Length; i++)
                {
                    var transactionOutput = transaction.Outputs[i];
                    if (transactionOutput.IsLockedWithKey(publicKeyHash) && accumulatedValue < amount)
                    {
                        accumulatedValue += transactionOutput.Value;

                        if (!unspentOutputs.ContainsKey(transaction.Id))
                        {
                            unspentOutputs.Add(transaction.Id, new int[] { i });
                        }

                        if (accumulatedValue >= amount)
                        { 
                            return Tuple.Create(accumulatedValue, unspentOutputs);
                        }
                    }
                }
            }

            return Tuple.Create(accumulatedValue, unspentOutputs);
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
            var spentTransactions = new Dictionary<byte[], List<int>>();

            var blockchainIterator = new BlockchainIterator(this);

            var publicKeyHash = Utils.HashPublicKey(wallet.PublicKey);

            while (!string.IsNullOrEmpty(blockchainIterator.CurrentHash))
            {
                var block = blockchainIterator.Next();

                foreach (var transaction in block.Transactions)
                {
                    for (var i = 0; i < transaction.Outputs.Length; i++)
                    {
                        if (IsTransactionOutputSpent(transaction.Id, spentTransactions, i))
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
                                    var referencedTransactionOutputId = transactionInput.ReferencedTransactionOutputId;

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

        private bool IsTransactionOutputSpent(byte[] transactionId, Dictionary<byte[], List<int>> spentTransactions, int outputIndexInTransaction)
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
