namespace bitcoin_from_scratch
{
    public class TransactionOutput
    {
        public int Value { get; set; }
        public string ScriptPubKey { get; set; }
        public TransactionOutput(int value, string scriptPubKey)
        {
            Value = value;
            ScriptPubKey = scriptPubKey;
        }
    }
}
