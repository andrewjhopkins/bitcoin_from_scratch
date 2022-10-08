namespace bitcoin_from_scratch
{
    public class TransactionInput
    {
        public byte[] ReferencedTransactionOutputId { get; set; }
        public int ReferencedTransactionOutputIndex { get; set; }
        public string ScriptSig { get; set; }

        public TransactionInput(byte[] referencedTransactionOutputId, int referencedTransactionOutputIndex, string scriptSig)
        {
            ReferencedTransactionOutputId = referencedTransactionOutputId;
            ReferencedTransactionOutputIndex = referencedTransactionOutputIndex;
            ScriptSig = scriptSig;
        }
    }
}
