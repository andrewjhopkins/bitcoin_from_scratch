using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bitcoin_from_scratch
{
    public class Blockchain
    {
        public Block[] Blocks { get; set; }

        public Blockchain()
        {
            var genesisBlock = CreateNewGenesisBlock();
            Blocks = new Block[] { genesisBlock };
        }

        private Block CreateNewGenesisBlock()
        {
            return new Block("Genesis Block", new byte[0]);
        }
    }
}
