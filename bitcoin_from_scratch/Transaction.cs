using Newtonsoft.Json;

namespace bitcoin_from_scratch
{
    public class Transaction
    {
        public byte[] Id { get; set; }
        public TransactionInput[] Inputs { get; set; }
        public TransactionOutput[] Outputs { get; set; }

        [JsonConstructor]
        public Transaction(byte[] id, TransactionInput[] inputs, TransactionOutput[] outputs)
        {
            Id = id;
            Inputs = inputs;
            Outputs = outputs;
        }

        public Transaction (Blockchain blockchain, Wallet to, Wallet from, int amount)
        {
            var inputs = new List<TransactionInput>();
            var outputs = new List<TransactionOutput>();

            var result = blockchain.FindSpendableOutputs(from, amount);
            var accumulatedFunds = result.Item1;
            var unspentTransactions = result.Item2;

            if (accumulatedFunds < amount)
            {
                throw new Exception($"Not enough funds. Balance: {accumulatedFunds}");
            }

            foreach (var keyValuePair in unspentTransactions)
            {
                foreach (var index in keyValuePair.Value)
                {
                    var transactionInput = new TransactionInput(keyValuePair.Key, index, new byte[0], from.PublicKey);
                    inputs.Add(transactionInput);
                }
            }

            outputs.Add(new TransactionOutput(amount, Utils.HashPublicKey(to.PublicKey)));

            if (accumulatedFunds > amount)
            {
                outputs.Add(new TransactionOutput(accumulatedFunds - amount, Utils.HashPublicKey(from.PublicKey)));
            }

            Inputs = inputs.ToArray();
            Outputs = outputs.ToArray();
            Id = new byte[0];
        }

        public bool IsCoinbase() =>
            (Inputs.Length == 1 && Inputs[0].ReferencedTransactionOutputId.Length == 0 && Inputs[0].ReferencedTransactionOutputIndex == -1);
    }
}
