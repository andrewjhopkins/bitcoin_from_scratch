namespace bitcoin_from_scratch
{
    public class MerkleNode
    {
        public MerkleNode Left { get; set; }
        public MerkleNode Right { get; set; }
        public byte[] Data { get; set; }

        public MerkleNode(MerkleNode left, MerkleNode right, byte[] data)
        {
            if (left == null && right == null)
            {
                Data = Utils.Sha256(data);
            }
            else
            {
                var previousHashes = left.Data.Concat(right.Data).ToArray();
                Data = Utils.Sha256(previousHashes);
            }

            Left = left;
            Right = right;
        }
    }
}
