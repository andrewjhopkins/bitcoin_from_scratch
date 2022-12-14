using SshNet.Security.Cryptography;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using SHA256 = SshNet.Security.Cryptography.SHA256;

namespace bitcoin_from_scratch
{
    public static class Utils
    {
        public static byte[] HashPublicKey(byte[] input)
        {
            var inputShaHash = Sha256(input);
            var inputRipeHash = Ripemd160(inputShaHash);
            return inputRipeHash;
        }
        public static byte[] Sha256(byte[] data)
        {
            using (var sha256 = new SHA256())
            { 
                return sha256.ComputeHash(data);
            }
        }

        public static byte[] Ripemd160(byte[] data)
        {
            using (var ripemd160 = new RIPEMD160())
            {
                return ripemd160.ComputeHash(data);
            }
        }

        private const string Base58Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static byte[] Base58Encode(byte[] input)
        {
            var base58Bytes = Encoding.UTF8.GetBytes(Base58Alphabet);

            BigInteger intInput = 0;
            for (var i = 0; i < input.Length; i++)
            {
                intInput = intInput * 256 + input[i];
            }

            var result = new List<byte>();

            while (intInput > 0)
            {
                var remainder = (int)(intInput % 58);
                intInput /= 58;
                result.Add(base58Bytes[remainder]);
            }

            for (var i = 0; i < input.Length && input[i] == 0; i++)
            {
                result.Add((byte)'1');
            }

            result.Reverse();

            return result.ToArray();
        }

        public static byte[] Base58Decode(string input)
        {
            BigInteger intInput = 0;
            var zeroBytes = input.TakeWhile(x => x == '1').Count();
            var payload = input.Skip(zeroBytes).ToArray();

            for (var i = 0; i < payload.Length; i++)
            {
                var charIndex = Base58Alphabet.IndexOf(payload[i]);
                intInput *= 58;
                intInput += new BigInteger(Convert.ToInt64(charIndex));
            }

            var decode = intInput.ToByteArray().Reverse().SkipWhile(x => x == 0);
            var zeroByteArray = new byte[zeroBytes];
            return zeroByteArray.Concat(decode).ToArray();
        }
        public static void SerializeObjectToFile(string path, object objectToSerialize)
        {
            using var stream = File.Open(path, FileMode.Create);
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, objectToSerialize);
            stream.Close();
        }

        public static object DeserializeObjectFromFile(string path)
        {
            using var stream = File.Open(path, FileMode.Open);
            var binaryFormatter = new BinaryFormatter();
            var objectToFetch = binaryFormatter.Deserialize(stream);
            stream.Close();
            return objectToFetch;
        }

        public static byte[] GetPublicKeyHashFromBitcoinAddress(string bitcoinAddress)
        {
            var decodedAddress = Base58Decode(bitcoinAddress);
            return decodedAddress.Skip(1).Take(decodedAddress.Length - 5).ToArray();
        }

        public static string BytesToString(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static byte[] StringToBytes(string inputString)
        {
            return Convert.FromBase64String(inputString);
        }
    }
}
