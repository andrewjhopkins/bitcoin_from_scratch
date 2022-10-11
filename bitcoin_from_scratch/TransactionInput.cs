namespace bitcoin_from_scratch
{
    public class TransactionInput
    {
        public byte[] ReferencedTransactionOutputId { get; set; }
        public int ReferencedTransactionOutputIndex { get; set; }
        public byte[] Signature { get; set; }
        public byte[] PublicKey { get; set; }

        public TransactionInput(byte[] referencedTransactionOutputId, int referencedTransactionOutputIndex, byte[] signature, byte[] publicKey)
        {
            ReferencedTransactionOutputId = referencedTransactionOutputId;
            ReferencedTransactionOutputIndex = referencedTransactionOutputIndex;
            Signature = signature;
            PublicKey = publicKey;
        }

        public bool UsesKey(byte[] publicKeyHash)
        {
            var lockingHash = Utils.HashPublicKey(PublicKey);
            return lockingHash.SequenceEqual(publicKeyHash);
        }
    }
}
