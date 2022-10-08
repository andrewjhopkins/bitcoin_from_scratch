using System.Text;

namespace bitcoin_from_scratch
{
    public class Block
    {
        public DateTime Timestamp { get; set; }
        public string Data { get; set; }
        public byte[] PreviousBlockHash { get; set; }
        public byte[]? Hash { get; set; }
        public long Nonce { get; set; }

        public ProofOfWork ProofOfWork { get; set; }

        public Block(string data, byte[] previousBlockHash)
        {
            Timestamp = DateTime.UtcNow;
            Data = data;
            PreviousBlockHash = previousBlockHash;

            ProofOfWork = new ProofOfWork();
            ProofOfWork.Run(this);
        }

        public byte[] BlockDataInBytes()
        {
            List<byte[]> byteArrays = new List<byte[]>
            {
                PreviousBlockHash,
                Encoding.UTF8.GetBytes(Data),
                BitConverter.GetBytes(Timestamp.Ticks),
                BitConverter.GetBytes(ProofOfWork.TargetBits)
            };

            var preparedData = byteArrays.SelectMany(x => x).ToArray();
            return preparedData;
        }
    }
}
