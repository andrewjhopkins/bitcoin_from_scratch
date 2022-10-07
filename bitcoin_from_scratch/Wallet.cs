using System.Security.Cryptography;
using System.Text;

namespace bitcoin_from_scratch
{
    public class Wallet
    {
        private static readonly byte Version = 0;
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
            var hashedPublicKey = Utils.HashPublicKey(publicKeyBytes);

            var hashedPublicKeyWithVersion = AppendBitcoinVersion(hashedPublicKey, Version);
            byte[] address = AppendCheckSum(hashedPublicKeyWithVersion);

            var bitcoinAddress = Encoding.UTF8.GetString(Utils.Base58Encode(address));

            return bitcoinAddress;
        }

        public void LoadWalletFromFile(string fileName)
        {
            return;
        }

        private byte[] AppendBitcoinVersion(byte[] publicKeyHash, byte version)
        {
            var outputByteArray = new byte[publicKeyHash.Length + 1];
            outputByteArray[0] = version;
            Array.Copy(publicKeyHash, 0, outputByteArray, 1, publicKeyHash.Length);

            return outputByteArray;
        }

        public static byte[] AppendCheckSum(byte[] hashedKey)
        {
            var twiceShaHashedKey = Utils.Sha256(Utils.Sha256(hashedKey));
            var outputByteArray = new byte[hashedKey.Length + 4];
            Array.Copy(hashedKey, outputByteArray, hashedKey.Length);

            Array.Copy(twiceShaHashedKey, 0, outputByteArray, hashedKey.Length, 4);
            return outputByteArray;
        }
    }
}
