namespace bitcoin_from_scratch
{
    public class ProofOfWork
    {
        public readonly int TargetBits = 18; // Difficulty of mining
        public ulong Target { get; set; }

        public ProofOfWork()
        {
            ulong target = 1;
            target = target << (256 - TargetBits);

            Target = target;
        }

        public void Run(Block block)
        {
            long nonce = 0;
            byte[] hashedBytes = new byte[0];

            while (nonce < long.MaxValue)
            {
                var bytesToHash = block.BlockDataInBytes().Concat(BitConverter.GetBytes(nonce)).ToArray();

                hashedBytes = Utils.Sha256(bytesToHash);

                var hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                Console.Write($"\r{hashedString}");

                var hashedLong = BitConverter.ToUInt64(hashedBytes);

                if (hashedLong < Target)
                {
                    Console.WriteLine(nonce);
                    break;
                }
                else
                {
                    nonce += 1;
                }
            }

            block.Hash = hashedBytes;
            block.Nonce = nonce;

            return;
        }
    }
}
