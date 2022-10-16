namespace bitcoin_from_scratch
{
    public class MerkleTree
    {
        public MerkleNode Root { get; set; }

        public MerkleTree(byte[][] data)
        {
            var nodes = new List<MerkleNode>();

            // Number of tree leaves should be even
            if (data.Length % 2 != 0)
            {
                data.Concat(new byte[][] { data[data.Length - 1] });
            }

            foreach (var datum in data)
            {
                var node = new MerkleNode(null, null, datum);
                nodes.Add(node);
            }

            for (var i = 0; i < data.Length / 2; i++)
            { 
                var newLevel = new List<MerkleNode>();

                for (var j = 0; j < nodes.Count; j += 2)
                {
                    var node = new MerkleNode(nodes[j], nodes[j + 1], null);
                    newLevel.Add(node);
                }

                nodes = newLevel;
            }

            Root = nodes[0];
        }
    }
}
