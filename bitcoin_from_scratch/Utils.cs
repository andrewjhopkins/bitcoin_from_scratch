using SshNet.Security.Cryptography;
using System.Numerics;
using System.Text;
using SHA256 = SshNet.Security.Cryptography.SHA256;

namespace bitcoin_from_scratch
{
    public static class Utils
    {
        public static byte[] HashPublicKey(byte[] input)
        {
            var sha256 = new SHA256();
            var sha256PublicKeyBytes = sha256.ComputeHash(input);

            // check to see if this needs to be converted to a string before hashing it
            var ripemd160 = new RIPEMD160();
            var ripemd160FromSha256 = ripemd160.ComputeHash(sha256PublicKeyBytes);
            return ripemd160FromSha256;
        }

        public static string CheckSum(string input)
        {
            var sha256 = new SHA256();

            // SHA256 it twice
            for (var i = 0; i < 2; i++)
            {
                input = Encoding.UTF8.GetString(sha256.ComputeHash(Encoding.ASCII.GetBytes(input)));
            }

            return input;
        }

        private const string Base58Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static byte[] StringToBytes(string input)
        { 
            var utf8 = new UTF8Encoding();
            return utf8.GetBytes(Base58Alphabet);
        }

        public static string Base58Encode(byte[] input)
        {
            // Decode byte to bigInt
            BigInteger intInput = 0;
            for (var i = 0; i < input.Length; i++)
            {
                intInput = intInput * 256 + input[i];
            }

            // Encode BigInteger to Base58 string
            var result = "";
            while (intInput > 0)
            {
                var remainder = (int)(intInput % 58);
                intInput /= 58;

                result = Base58Alphabet[remainder] + result;
            }

            // Append 1 for each leading 0 byte
            for (var i = 0; i < input.Length && input[i] == 0; i++)
            {
                result = '1' + result;
            }

            return result;
        }

        public static byte[] Base58Decode(byte[] input)
        {
            BigInteger intInput = 0;
            var zeroBytes = 0;

            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] == 0x00)
                {
                    zeroBytes += 1;
                }
            }

            var payload = input.Skip(zeroBytes).ToArray();
            var base58Bytes = StringToBytes(Base58Alphabet);

            for (var i = 0; i < payload.Length; i++)
            {
                var charIndex = Array.IndexOf(base58Bytes, payload[i]);
                intInput *= 58;
                intInput += new BigInteger(System.Convert.ToInt64(charIndex));
            }

            var decode = intInput.ToByteArray();
            var zeroByteArray = new byte[zeroBytes];

            return zeroByteArray.Concat(decode).ToArray();
        }
    }
}
