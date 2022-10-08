using LevelDB;
using Newtonsoft.Json;
using System.Text;

namespace bitcoin_from_scratch
{
    public class Blockchain
    {
        public string DbFilePath { get; set; }
        public string TipHashString { get; set; }

        public Blockchain(string dbFilePath)
        {
            DbFilePath = dbFilePath;
            var genesisBlock = CreateNewGenesisBlock();

            if (genesisBlock.Hash != null)
            {
                using (var db = new DB(new Options(), DbFilePath))
                {
                    var hashString = Utils.BytesToString(genesisBlock.Hash);

                    db.Put(hashString, JsonConvert.SerializeObject(genesisBlock));
                    db.Put("1", hashString);

                    TipHashString = hashString;
                    db.Close();
                }
            }
            else
            {
                throw new Exception("Genesis block not created");
            }
        }

        public Blockchain(string dbFilePath, string tipHashString)
        {
            DbFilePath = dbFilePath;
            TipHashString = tipHashString;
        }

        private Block CreateNewGenesisBlock()
        {
            return new Block("Genesis Block", new byte[0]);
        }
    }
}
