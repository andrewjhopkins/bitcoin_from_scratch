using System.Security.Cryptography;
using System.Text;

namespace bitcoin_from_scratch
{
    public class Wallet
    {
        private static readonly string Version = "00";
        byte[] PrivateKey { get; set; }
        byte[] PublicKey { get; set; }
        string Address { get; set; }

        public string GenerateBitcoinAddress()
        {
            var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var ecParameters = key.ExportParameters(true);
            var privateKey = ecParameters.D;
            var publicKey = ecParameters.Q;

            if (privateKey == null || publicKey.X == null || publicKey.Y == null)
            {
                throw new Exception();
            }

            var publicKeyBytes = publicKey.X.Concat(publicKey.Y).ToArray();

            PrivateKey = privateKey;
            PublicKey = publicKeyBytes;

            var hashedPublicKey = Utils.HashPublicKey(publicKeyBytes);

            var prependNetworkByte = Version + Encoding.UTF8.GetString(hashedPublicKey);

            var hash = Utils.CheckSum(prependNetworkByte);
            var checkSum = String.Concat(hash.Take(8));

            var appendCheckSum = prependNetworkByte + checkSum;

            var bitcoinAddress = Utils.Base58Encode(Encoding.ASCII.GetBytes(appendCheckSum));

            return bitcoinAddress;
        }

        public void LoadWalletFromFile(string fileName)
        {
            return;
        }
    }
}
