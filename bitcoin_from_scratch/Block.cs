using System.Text;

namespace bitcoin_from_scratch
{
    public class Block
    {
        public DateTime Timestamp { get; set; }
        public Transaction[] Transactions { get; set; }
        public byte[] PreviousBlockHash { get; set; }
        public byte[]? Hash { get; set; }
        public long Nonce { get; set; }

        public ProofOfWork ProofOfWork { get; set; }

        public Block(Transaction[] transactions, byte[] previousBlockHash)
        {
            Timestamp = DateTime.UtcNow;
            Transactions = transactions;
            PreviousBlockHash = previousBlockHash;

            ProofOfWork = new ProofOfWork();
            ProofOfWork.Run(this);
        }

        public byte[] BlockDataInBytes()
        {
            List<byte[]> byteArrays = new List<byte[]>
            {
                PreviousBlockHash,
                HashTransactions(),
                BitConverter.GetBytes(Timestamp.Ticks),
                BitConverter.GetBytes(ProofOfWork.TargetBits)
            };

            var preparedData = byteArrays.SelectMany(x => x).ToArray();
            return preparedData;
        }

        private byte[] HashTransactions()
        {
            List<byte[]> byteArrays = new List<byte[]>();
            foreach (var transaction in Transactions)
            {
                byteArrays.Add(transaction.Id);
            }

            return Utils.Sha256(byteArrays.SelectMany(x => x).ToArray());
        }
    }
}
