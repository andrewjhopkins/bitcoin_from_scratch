using LevelDB;
using Newtonsoft.Json;

namespace bitcoin_from_scratch
{
    public class UtxoSet
    {
        Blockchain blockchain { get; set; }

        public UtxoSet(Blockchain blockchain)
        {
            this.blockchain = blockchain;
        }

        public void ReIndex()
        {
            var dbFile = blockchain.UtxoSetDbFilePath;

            if (File.Exists(dbFile))
            {
                File.Delete(dbFile);
            }

            var options = new Options()
            {
                CreateIfMissing = true
            };

            using (var db = new DB(options, dbFile))
            {
                var utxo = blockchain.FindUtxo();
                foreach (var entry in utxo)
                {
                    db.Put(Utils.BytesToString(entry.Key), JsonConvert.SerializeObject(entry.Value.ToArray()));
                }

                db.Close();
            }

            return;
        }

        public void Update(Block block)
        {
            var dbFile = blockchain.UtxoSetDbFilePath;

            using (var db = new DB(new Options(), dbFile))
            {
                foreach (var transaction in block.Transactions)
                {
                    if (!transaction.IsCoinbase())
                    {
                        foreach (var transactionInput in transaction.Inputs)
                        {
                            var dbKey = Utils.BytesToString(transactionInput.ReferencedTransactionOutputId);

                            var updatedOutputs = new List<TransactionOutput>();
                            var outputTransactions = JsonConvert.DeserializeObject<TransactionOutput[]>(db.Get(dbKey));

                            if (outputTransactions == null)
                            {
                                throw new Exception("Transaction outputs not found in DB");
                            }

                            for (var i = 0; i < outputTransactions.Length; i++)
                            {
                                if (i != transactionInput.ReferencedTransactionOutputIndex)
                                {
                                    updatedOutputs.Add(outputTransactions[i]);
                                }
                            }

                            if (updatedOutputs.Count == 0)
                            {
                                db.Delete(dbKey);
                            }
                            else
                            {
                                db.Put(dbKey, JsonConvert.SerializeObject(updatedOutputs.ToArray()));
                            }
                        }
                    }

                    var newOutputs = new List<TransactionOutput>();
                    foreach (var transactionOutput in transaction.Outputs)
                    {
                        newOutputs.Add(transactionOutput);
                    }

                    db.Put(Utils.BytesToString(transaction.Id), JsonConvert.SerializeObject(newOutputs.ToArray()));
                }

                db.Close();
            }
        }
    }
}
