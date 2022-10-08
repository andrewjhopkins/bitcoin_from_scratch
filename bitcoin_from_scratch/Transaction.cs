using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bitcoin_from_scratch
{
    public class Transaction
    {
        public byte[] Id { get; set; }
        public TransactionInput[] Inputs { get; set; }
        public TransactionOutput[] Outputs { get; set; }

        public Transaction(byte[] id, TransactionInput[] inputs, TransactionOutput[] outputs)
        {
            Id = id;
            Inputs = inputs;
            Outputs = outputs;
        }

        public bool IsCoinbase() =>
            (Inputs.Length == 1 && Inputs[0].ReferencedTransactionOutputId.Length == 0 && Inputs[0].ReferencedTransactionOutputIndex == -1);
    }
}
