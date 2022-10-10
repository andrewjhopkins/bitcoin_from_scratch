namespace bitcoin_from_scratch
{
    public class TransactionOutput
    {
        public int Value { get; set; }
        public byte[] PublicKeyHash { get; set; }

        public TransactionOutput(int value, byte[] publicKeyHash)
        {
            Value = value;
            PublicKeyHash = publicKeyHash;
        }

        public bool IsLockedWithKey(byte[] publicKeyHash)
        {
            return PublicKeyHash.SequenceEqual(publicKeyHash);
        }
    }
}
