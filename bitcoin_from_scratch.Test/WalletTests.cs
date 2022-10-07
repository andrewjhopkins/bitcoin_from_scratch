using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bitcoin_from_scratch.Test
{
    public class WalletTests
    {
        [TestCase]
        public void WalletGeneratesValidP2PKHBitcoinAddress()
        {
            var wallet = new Wallet();
            var bitcoinAddress = wallet.GenerateBitcoinAddress();

            Assert.IsTrue(bitcoinAddress.Length >= 26 && bitcoinAddress.Length <= 34);
            Assert.IsTrue(bitcoinAddress[0] == '1');
            Assert.IsTrue(Encoding.ASCII.GetByteCount(bitcoinAddress) == 34);
        }
    }
}
