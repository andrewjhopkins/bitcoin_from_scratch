﻿using System.Security.Cryptography;
using System.Text;

namespace bitcoin_from_scratch
{
    [Serializable()]
    public class Wallet
    {
        private static readonly byte Version = 0;
        private static readonly string FolderPath = "./wallets";

        public byte[] PrivateKey { get; set; }
        public byte[] PublicKey { get; set; }
        public string BitcoinAddress { get; set; }

        public Wallet()
        {
            PrivateKey = new byte[0];
            PublicKey = new byte[0];
            BitcoinAddress = "";
        }

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

            var hashedPublicKeyWithVersion = AppendBitcoinVersion(hashedPublicKey, Version);
            byte[] address = AppendCheckSum(hashedPublicKeyWithVersion);

            var bitcoinAddress = Encoding.UTF8.GetString(Utils.Base58Encode(address));

            BitcoinAddress = bitcoinAddress;

            return bitcoinAddress;
        }
        
        public void LoadWalletFromFile(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                throw new Exception("File does not exist or can not be found.");
            }

            var walletToLoad = (Wallet)Utils.DeserializeObject(path);
            
            PrivateKey = walletToLoad.PrivateKey;
            PublicKey = walletToLoad.PublicKey;
            BitcoinAddress = walletToLoad.BitcoinAddress;

            return;
        }

        public void SaveWalletToFile()
        {
            var fileName = $"{BitcoinAddress}.dat";

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            var path = $"{FolderPath}/{fileName}";

            Utils.SerializeObject(path, this);
            return;
        }

        private byte[] AppendBitcoinVersion(byte[] publicKeyHash, byte version)
        {
            var outputByteArray = new byte[publicKeyHash.Length + 1];
            outputByteArray[0] = version;
            Array.Copy(publicKeyHash, 0, outputByteArray, 1, publicKeyHash.Length);

            return outputByteArray;
        }

        private static byte[] AppendCheckSum(byte[] hashedKey)
        {
            var twiceShaHashedKey = Utils.Sha256(Utils.Sha256(hashedKey));
            var outputByteArray = new byte[hashedKey.Length + 4];
            Array.Copy(hashedKey, outputByteArray, hashedKey.Length);

            Array.Copy(twiceShaHashedKey, 0, outputByteArray, hashedKey.Length, 4);
            return outputByteArray;
        }
    }
}
