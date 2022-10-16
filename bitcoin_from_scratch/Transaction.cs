using EllipticCurve;
using Newtonsoft.Json;
using System.Numerics;
using System.Text;

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

        public Transaction (Blockchain blockchain, byte[] toPublicKeyHash, Wallet from, int amount)
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

            outputs.Add(new TransactionOutput(amount, toPublicKeyHash));

            if (accumulatedFunds > amount)
            {
                outputs.Add(new TransactionOutput(accumulatedFunds - amount, Utils.HashPublicKey(from.PublicKey)));
            }

            Inputs = inputs.ToArray();
            Outputs = outputs.ToArray();
            Id = Hash();

            blockchain.SignTransaction(this, from.PrivateKey);
        }

        public void Sign(byte[] privateKey, Dictionary<string, Transaction> previousTransactions)
        {
            if (IsCoinbase())
            {
                return;
            }

            foreach (var inputs in Inputs)
            {
                if (previousTransactions[Utils.BytesToString(inputs.ReferencedTransactionOutputId)].Id == null)
                {
                    throw new Exception("Previous transaction is not correct");
                }
            }

            var transactionCopy = GetTrimmedCopy();

            for (var i = 0; i < transactionCopy.Inputs.Length; i++)
            {
                var transactionInput = transactionCopy.Inputs[i];

                var previousTransaction = previousTransactions[Utils.BytesToString(transactionInput.ReferencedTransactionOutputId)];

                // double check signature is set to empty;
                transactionCopy.Inputs[i].Signature = new byte[0];
                transactionCopy.Inputs[i].PublicKey = previousTransaction.Outputs[transactionInput.ReferencedTransactionOutputIndex].PublicKeyHash;
                transactionCopy.Id = transactionCopy.Hash();

                // reset publicKey so it doesn't impact futher iterations
                transactionCopy.Inputs[i].PublicKey = new byte[0];

                var signature = Ecdsa.sign(Utils.BytesToString(transactionCopy.Id), PrivateKey.fromDer(privateKey));

                Inputs[i].Signature = signature.toDer();
            }
        }

        public bool Verify(Dictionary<string, Transaction> previousTransactions)
        {
            if (IsCoinbase())
            {
                return true;
            }

            foreach (var inputs in Inputs)
            {
                if (previousTransactions[Utils.BytesToString(inputs.ReferencedTransactionOutputId)].Id == null)
                {
                    throw new Exception("Previous transaction is not correct");
                }
            }

            var transactionCopy = GetTrimmedCopy();

            for (var i = 0; i < Inputs.Length; i++)
            {
                var transactionInput = Inputs[i];

                var previousTransaction = previousTransactions[Utils.BytesToString(transactionInput.ReferencedTransactionOutputId)];
                transactionCopy.Inputs[i].Signature = new byte[0];
                transactionCopy.Inputs[i].PublicKey = previousTransaction.Outputs[transactionInput.ReferencedTransactionOutputIndex].PublicKeyHash;
                transactionCopy.Id = transactionCopy.Hash();
                transactionCopy.Inputs[i].PublicKey = new byte[0];

                var curveByName = Curves.getCurveByName("secp256k1");
                var keyLength = curveByName.length();

                // point bytes sometimes 33 bytes with appeneded 0 at the end
                if (transactionInput.PublicKey.ElementAt(keyLength) == 0)
                {
                    keyLength += 1;
                }

                var xBytes = transactionInput.PublicKey.Take(keyLength).ToArray();
                var yBytes = transactionInput.PublicKey.Skip(keyLength).ToArray();

                var x = new BigInteger(xBytes);
                var y = new BigInteger(yBytes);

                var rawPublicKey = new PublicKey(new Point(x, y), Curves.secp256k1);

                if (transactionInput.Signature.Length == 0 || 
                    !Ecdsa.verify(Utils.BytesToString(transactionCopy.Id), Signature.fromDer(transactionInput.Signature), rawPublicKey))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsCoinbase() =>
            (Inputs.Length == 1 && Inputs[0].ReferencedTransactionOutputId.Length == 0 && Inputs[0].ReferencedTransactionOutputIndex == -1);

        private byte[] Hash()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            var bytes = Encoding.UTF8.GetBytes(jsonString);
            return Utils.Sha256(bytes);
        }

        private Transaction GetTrimmedCopy()
        {
            var inputs = Inputs.Select(x => new TransactionInput(x.ReferencedTransactionOutputId, x.ReferencedTransactionOutputIndex, new byte[0], new byte[0])).ToArray();
            var outputs = Outputs.Select(x => new TransactionOutput(x.Value, x.PublicKeyHash)).ToArray();

            return new Transaction(Id, inputs, outputs);
        }
    }
}
