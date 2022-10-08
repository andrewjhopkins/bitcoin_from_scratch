using LevelDB;
using Newtonsoft.Json;

namespace bitcoin_from_scratch
{
    public class BlockchainIterator
    {
        public string DbFile { get; set; }
        public string CurrentHash { get; set; }

        public BlockchainIterator(string dbFile, string currentHash)
        {
            DbFile = dbFile;
            CurrentHash = currentHash;
        }

        public string Next()
        {
            using (var db = new DB(new Options(), DbFile))
            {
                if (db.Get(CurrentHash) == null)
                {
                    throw new Exception("Current hash not found in database");
                }

                var currentBlock = JsonConvert.DeserializeObject<Block>(db.Get(CurrentHash));

                if (currentBlock == null)
                {
                    throw new Exception("Unable to deserialize block from database");
                }

                var nextHash = Utils.BytesToString(currentBlock.PreviousBlockHash);
                CurrentHash = nextHash;
                db.Close();
            }

            return CurrentHash;
        }
    }
}
